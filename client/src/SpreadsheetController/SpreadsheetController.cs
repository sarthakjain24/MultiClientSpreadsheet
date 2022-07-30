using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SS
{
    public class SpreadsheetController
    {

        public delegate void CellUpdateHandler(string cell, string value);

        //Update ALL USER cell value in spreadsheet panel
        public event CellUpdateHandler CellUpdate;

        //Update Edit box for THIS user
        public event CellUpdateHandler EditUpdate;

        public delegate void CellSelectHandler(int clientId, string clientName, string cellName);

        //Update ALL USER cell selection in spreadsheet panel
        public event CellSelectHandler CellSelect;

        public delegate void ClientDisconnectHandler(int clientId);

        //Update drawing for disconnected user
        public event ClientDisconnectHandler ClientDisconnect;

        //A delegate for the ConnectedHandler
        public delegate void spreadsheetNamesRecieved(List<string> spreadsheetList);

        //Update drop-down list for displaying spreadsheets
        public event spreadsheetNamesRecieved namesRecieved;

        //A delegate for the ConnectedHandler
        public delegate void ConnectedHandler();

        //Draw a textbox inform user has successfully connected
        public event ConnectedHandler Connected;

        //A delegate for the ErrorHandling
        public delegate void ErrorHandler(string err);

        //An event for the delegate called by the view
        public event ErrorHandler Error;

        //An event for the delegate called by the view
        public event ErrorHandler ServerError;

        //A delegate for the MessageSender
        public delegate void MessageSender();

        //An event for the delegate called by the view
        public event MessageSender MessagesSent;


        //locks goes here
        private Object SheetLock = new Object();


        //A variable that represents the player's name
        private String userName;

        // User id received from server
        private int id;

        //A variable that represents the server
        private SocketState theServer;

        //A variable representing the spreadsheet 
        private Spreadsheet spreadsheet;

        private List<string> listOfSprd;

        private Dictionary<int, string> clients;

        private bool ConnectedCellClick;

        public SpreadsheetController()
        {
            id = -1;
            spreadsheet = new Spreadsheet();
            theServer = null;
            userName = null;
            ConnectedCellClick = false;
            listOfSprd = new List<string>();
            clients = new Dictionary<int, string>();
        }

        /// <summary>
        /// Connects to the server 
        /// </summary>
        public void Connect(string address, string name)
        {
            //Sets the player's name as the name
            userName = name;
            int port = 1100;
            //Connects to the server
            Networking.ConnectToServer(OnConnect, address, port);
        }

        /// <summary>
        /// Method to be invoked by the networking library when a connection is made
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        {
            if (state.ErrorOccured)
            {
                // inform the view
                Error("Error connecting to server");
                return;
            }

            //Sets theServer with the state
            theServer = state;

            //Transfer to the next stage
            state.OnNetworkAction = ReceiveSpreadsheetNames;
            Networking.Send(state.TheSocket, userName + "\n");
            Networking.GetData(state);
        }

        /// <summary>
        /// Method to be invoked by the networking library when 
        /// data is available
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveSpreadsheetNames(SocketState state)
        {
            if (state.ErrorOccured)
            {
                // inform the view
                Error("Lost connection to server");
                return;
            }

            //Call some method in view that allows the user to select the spreadsheet(Also in tandem with spreadsheet initializer)
            string data = state.GetData();

            //Create a jerry Array
            string[] sheets = data.Split('\n');
            if (data.Last() != '\n') // Meaning there is incomplete data in current buffer
            {
                // Keep partial data
                state.RemoveData(0, data.LastIndexOf('\n') + 1);
                // Set the last element to empty string (in the jerry Array)  
                sheets[sheets.Length - 1] = "";
            }

            //Inform the view that the list of available spreadsheets has been found
            if (data != "")
            {
                foreach (string msg in sheets)
                {
                    if (msg == "")
                        continue;
                    lock (SheetLock)
                    {
                        listOfSprd.Add(msg);
                    }
                }
            }

            //you can do what you need to do, one sec
            if (data.EndsWith("\n\n"))
            {
                state.ClearData();
                state.OnNetworkAction = spreadsheetUpdater;

                //tell the view to 
                namesRecieved(listOfSprd);
            }

            //Keep listening for more data
            Networking.GetData(state);
        }


        private string[] ProcessData(SocketState state)
        {
            string data = state.GetData();
            //Create a jerry Array
            string[] jArray = data.Split('\n');

            if (data.Last() != '\n') // Meaning there is incomplete data in current buffer
            {
                // Keep partial data
                state.RemoveData(0, data.LastIndexOf('\n') + 1);
                // Set the last element to empty string (in the jerry Array)  
                jArray[jArray.Length - 1] = "";
            }
            else
            {
                //All data are complete, clear the whole buffercellup
                state.ClearData();
            }

            return jArray;

        }

        /// <summary>
        /// Process any buffered messages separated by '\n'
        /// Then inform the view
        /// Handles all spreadsheet updates even the ones passed through the handshake
        /// </summary>
        /// <param name="state"></param>
        private void spreadsheetUpdater(SocketState state)
        {

            string[] parts = ProcessData(state);

            JObject obj;
            if (parts.Length > 0)
                foreach (string p in parts)
                {
                    if (p == "")
                        continue;

                    //if the string is the ID then we want that shit
                    if (int.TryParse(p, out int id) && this.id < 0)
                    {
                        this.id = id;
                        Connected();
                        ConnectedCellClick = true;
                        continue;
                    }
                    //else we wanna do the rest of the message handler thingamajig
                    else
                    {
                        //Parses the object with the JSON
                        obj = JObject.Parse(p);

                        if (!obj.ContainsKey("messageType"))
                            throw new Exception("DEBUG :: Bad JSON object, does not contain 'messageType' key !!");

                        string type = obj["messageType"].ToString();
                        switch (type)
                        {
                            case "cellUpdated":
                                ProcessCellUpdate(JsonConvert.DeserializeObject<CellUpdate>(p));
                                break;

                            case "cellSelected":
                                ProcessCellSelection(JsonConvert.DeserializeObject<CellSelected>(p));
                                break;

                            case "disconnected":
                                Disconnect disconnect = JsonConvert.DeserializeObject<Disconnect>(p);
                                if (clients.ContainsKey(disconnect.GetUserID()))
                                    clients.Remove(disconnect.GetUserID());

                                //update view
                                ClientDisconnect(disconnect.GetUserID());
                                break;

                            case "requestError":
                                InvalidRequest invalid = JsonConvert.DeserializeObject<InvalidRequest>(p);
                                //update view
                                Error(invalid.GetMessage());
                                break;

                            case "serverError":
                                ServerShutdown shutDown = JsonConvert.DeserializeObject<ServerShutdown>(p);
                                //update view
                                ServerError(shutDown.GetMessage());
                                Close();
                                return;

                            default:
                                throw new Exception($"DEBUG :: JSON 'messageType' was not of a known value ({type}) !!");
                        }
                    }
                }

            Networking.GetData(state);
        }


        /// Andrew can't do this via live share,
        /// This just handles the logic of updating the views cells based off of the most recent update from the server.
        private void ProcessCellUpdate(CellUpdate update)
        {
            string value = "";
            var updated = spreadsheet.SetContentsOfCell(update.GetCellName(), update.GetContents());
            foreach (var cell in updated)
            {
                object content = spreadsheet.GetCellContents(cell);
                if (Regex.IsMatch(update.GetCellName(), @"[A-Za-z]\d{1,2}"))
                    //checks if the content of the cell is a formula, if so '=' is added to the start of the string
                    if (content is SpreadsheetUtilities.Formula formula)
                        try
                        {
                            value = formula.Evaluate((string s) => { return (double)spreadsheet.GetCellValue(s); }).ToString(); //If exceptions occur this is probs is
                        }
                        catch (Exception ex)
                        {
                            value = "!ERROR :: Bad Formula Eval :: " + ex.Message;
                        }
                    else if (content is double d)
                        value = d.ToString();
                    else
                        value = content.ToString();
                CellUpdate(cell, value);
            }

        }

        /// Andrew still can't do XML comments via Live share 😭 
        ///  this abstracts away the logic about processing cell selection
        ///   like triggering the user's cell updated based off of the user id.
        private void ProcessCellSelection(CellSelected cellSelect)
        {
            if (!clients.ContainsKey(cellSelect.GetSelectorID()))
                clients.Add(cellSelect.GetSelectorID(), cellSelect.GetSelectorName());
            //update view
            CellSelect(cellSelect.GetSelectorID(), cellSelect.GetSelectorName(), cellSelect.GetCellName());
            if (cellSelect.GetSelectorID() == id)
                EditUpdate(cellSelect.GetCellName(), spreadsheet.GetCellContents(cellSelect.GetCellName()).ToString());
        }


        /// <summary>
        /// Closes the connection with the server
        /// </summary>
        public void Close()
        {
            theServer?.TheSocket.Close();
        }

        /// <summary>
        /// Creates a CellSelection object and sends it to the server
        /// </summary>
        public void RequestCellSelection(string cellName)
        {
            if (ConnectedCellClick)
            {
                CellSelection cellSelection = new CellSelection(cellName);
                Networking.Send(theServer.TheSocket, JsonConvert.SerializeObject(cellSelection) + "\n");
            }
        }

        /// <summary>
        /// Based on a cell's name and�� contents, create a RequestCellEdit object and send it to the server
        /// </summary>
        public void RequestCellEdit(string cellName, string contents)
        {
            if (ConnectedCellClick)
            {
                RequestCellEdit cellEdit = new RequestCellEdit(cellName, contents);
                Networking.Send(theServer.TheSocket, JsonConvert.SerializeObject(cellEdit) + "\n");
            }
        }

        /// <summary>
        /// Undoes the spreadsheet
        /// </summary>
        public void RequestUndo()
        {
            if (ConnectedCellClick)
            {
                Undo undo = new Undo();
                Networking.Send(theServer.TheSocket, JsonConvert.SerializeObject(undo) + "\n");
            }
        }

        /// <summary>
        /// Reverts the cell
        /// </summary>
        public void RequestRevert(string cellName)
        {
            if (ConnectedCellClick)
            {
                Revert r = new Revert(cellName);
                Networking.Send(theServer.TheSocket, JsonConvert.SerializeObject(r) + "\n");
            }
        }

        /// <summary>
        /// Part of the handshake process, sends the server the name of the spreadsheet that was selected
        /// </summary>
        /// <param name="spreadsheetName"></param>
        public void RequestSpreadsheet(string spreadsheetName)
        {
            
            Networking.Send(theServer.TheSocket, spreadsheetName + "\n");
        }

    }
}
