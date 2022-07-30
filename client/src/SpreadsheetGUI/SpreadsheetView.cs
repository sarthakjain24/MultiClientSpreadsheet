using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SpreadsheetUtilities;
using SS;
namespace SpreadsheetGUI
{
    /// <summary>
    /// A Spreadsheet Form
    /// </summary>
    /// <team>1265</team>
    /// <authors> Sarthak Jain, Christopher Beatty, Nick Sarka, Jerry Zhang, Andrew Osterhout, Vijai Siddharth Vachaspathi Tunukoju Venkatapuram</authors>
    public partial class SpreadsheetForm : Form
    {

        //A private instance variable for making the spreadsheet being in dark mode
        private bool darkMode;
        private SpreadsheetController controller;


        /// <summary>
        /// Initializes the spreadsheet and creates eventhandlers for multiple diffrent events
        /// Also creates the backing spreadsheet normilized with all uppercase letters and with the version "ps6"
        /// Sets the default cell name text box to "A1"
        /// </summary>
        public SpreadsheetForm()
        {
            InitializeComponent();

         //   this.MinimumSize.Height = spreadsheetPanel.Height + spreadsheetPanel.Top + 16;

            controller = new SpreadsheetController();

            //this just shows dialogue box 
            controller.Connected += HandleConnected;
            //what is this naming ????? this method shows a list of spreadsheets
            controller.namesRecieved += spreadsheetNamesRecieved;
            //those corresponds to the view events needed to happend just like their names
            controller.CellUpdate += spreadsheetPanel.SetValue;
            controller.CellSelect += spreadsheetPanel.SetClientSelection;
            controller.EditUpdate += UpdateEdit;
            controller.Error += RequestError;
            controller.ServerError += ServerError;
            controller.ClientDisconnect += spreadsheetPanel.RemoveClient;
            //for the users who makes local edits
            spreadsheetPanel.SelectionChanged += OnUsersSelectionChanged;
            cellContentsTextBox.KeyDown += new KeyEventHandler(cellContentsTextBox_KeyDown);
           // this.OnSizeChanged += spreadsheetPanel.;
            cellNameTextBox.Text = "";
            darkMode = false;
            lightModeButton.Checked = true;
            
            SpreadsheetForm_SizeChanged(this, new EventArgs());
            spreadsheetPanel.Height += 1;
        }


        /// <summary>
        /// When a contents text box changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contents_textbox_changed(object sender, EventArgs e)
        {
            //Puts the focus on the cell contents box, meaning that arrow keys can't be used to toggle the button
            cellContentsTextBox.Focus();
        }

        /// <summary>
        /// Request error
        /// </summary>
        /// <param name="msg"></param>
        private void RequestError(string msg)
        {
            this.Invoke(new MethodInvoker(() => { MessageBox.Show("Invalid Change", msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }));
        }

        /// <summary>
        /// Server error
        /// </summary>
        /// <param name="msg"></param>
        private void ServerError(string msg)
        {
            this.Invoke(new MethodInvoker(() => { MessageBox.Show("Server Error (shuting down)", msg, MessageBoxButtons.OK, MessageBoxIcon.Error); }));
        }

        /// <summary>
        /// Handler for the controller's Connected event
        /// </summary>
        private void HandleConnected()
        {
            // Shows a message box saying that the user was able to connect to the server
            this.Invoke(new MethodInvoker(
              () => MessageBox.Show("Connected to server" + Environment.NewLine)));
        }

        /// <summary>
        /// Triggered by the Spreadsheet panel when the user selects a cell.
        /// Should have the controller send a Sends a selectCell message to the server.
        /// </summary>
        /// <param name="ssp">the calling SpreadsheetPanel</param>
        private void OnUsersSelectionChanged(SpreadsheetPanel ssp)
        {

            //gets the current selected cell and sets col and row equal to that of the cell
            spreadsheetPanel.GetSelection(out int col, out int row);

            //The rowNum variable that is used to determine the cell name. this variable is offset by one to compensate for the zero based grid
            int rowNum = row + 1;
            //the column variable that stores the letter version of the col number
            char column = (char)('A' + Convert.ToChar(col));

            this.Invoke(new MethodInvoker
                (() =>
                {
                    if (spreadsheetPanel.GetValue(col, row, out string v)) { cellValueTextBox.Text = v; }
                }));

            // controller.SetSelectCell(true);
            controller.RequestCellSelection("" + column.ToString() + rowNum.ToString());
        }

        /// <summary>
        /// Handler for the controller's nameReceived event
        /// </summary>
        /// <param name="spreadsheetList"></param>
        private void spreadsheetNamesRecieved(List<string> spreadsheetList)
        {
            // Display the names to the user using some kind of clickable pop-up window 

            this.Invoke(new MethodInvoker(
                () =>
                {
                    foreach (string name in spreadsheetList)
                    {
                        spreadsheetComboBox.Items.Add(name);
                    }

                }));

        }

        ///<summary>
        /// When user click on one of the spreadsheet combo_box option, send a spreadsheetRequest to the server
        ///</summary>
        private void spreadsheetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            controller.RequestSpreadsheet(spreadsheetComboBox.Text);

            this.Invoke(new MethodInvoker(
              () =>
              {
                  spreadsheetComboBox.Enabled = false;
                  new_spreadsheet_box.Enabled = false;
                  spreadsheetButton.Enabled = false;
              }));
        }

        /// <summary>
        /// Called when new spreadsheet botton is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spreadsheetbotton_clicked(object sender, EventArgs e)
        {
            controller.RequestSpreadsheet(new_spreadsheet_box.Text);

            this.Invoke(new MethodInvoker(
              () =>
              {
                  spreadsheetComboBox.Enabled = false;
                  new_spreadsheet_box.Enabled = false;
                  spreadsheetButton.Enabled = false;
              }));
        }

        /// <summary>
        /// this method is called when Editupdate happens, change the contents box when THIS user selection happens 
        /// </summary>
        private void UpdateEdit(string cell, string contents)
        {
            this.Invoke(new MethodInvoker(
                  () =>
                  {
                      cellContentsTextBox.Text = contents;
                      cellNameTextBox.Text = cell;
                      cellContentsTextBox.SelectionStart = cellContentsTextBox.Text.Length;
                  }));
        }

        /// <summary>
        /// Closes the spreadsheet using the Close button in the file drop down menu
        /// </summary>
        private void closeSpreadsheet_Click(object sender, EventArgs e)
        {
            CloseSpreadsheet();
        }

        /// <summary>
        /// A helper method to close the spreadsheet
        /// </summary>
        private void CloseSpreadsheet()
        {
            //close this.form
            Close();
        }



        /// <summary>
        /// Creates a new SpreadSheet when the New button is clicked from the file drop down menu.
        /// </summary>
        private void newSpreadsheet_Click(object sender, EventArgs e)
        {
            //opens a new window 
            SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetForm());
        }


        /// <summary>
        /// A method that creates key bindings
        /// </summary>
        /// <Citation> https://www.daniweb.com/programming/software-development/threads/261384/disabling-arrow-keys-other-beginner-questions </Citation>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //If the user enters Alt + F4, closes the spreadsheet 
            if (keyData == (Keys.Alt | Keys.F4))
            {
                CloseSpreadsheet();
                return true;
            }

            //If the user enters Ctrl + N, creates a new spreadsheet window
            if (keyData == (Keys.Control | Keys.N))
            {
                SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetForm());
                return true;
            }

            //If the user enters Ctrl + Z, undoes the spreadsheet
            if (keyData == (Keys.Control | Keys.Z))
            {
                controller.RequestUndo();
                return true;
            }

            //If the user enters Ctrl + Y, reverts the spreadsheet
            if (keyData == (Keys.Control | Keys.Y))
            {
                spreadsheetPanel.GetSelection(out int col, out int row);
                row++;
                char column = (char)('A' + Convert.ToChar(col));
                string select = column.ToString() + row.ToString() + "";
                controller.RequestRevert(select);
                return true;
            }

            //Returns true if a key was pressed
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// The method that detects whenever the user is using the text box and also uses a key.
        /// This method detects when the enter key is pressed down and then updates the cell value, name, and contents
        /// according to what was typed in the text box.
        /// </summary>
        private void cellContentsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Checks to see if the key pressed was the enter key
            if (e.KeyCode == Keys.Enter)
            {
                //gets rid of the "ding" sound
                e.SuppressKeyPress = true;

                //the variable used for find the row
                int rowNum = 0;

                //the variable used for find the column after converting to a character
                char column = 'A';

                //finds what cell in the grid the user has selected
                spreadsheetPanel.GetSelection(out int col, out int row);

                //offests to account for the zero based grid 
                rowNum = row + 1;

                //converts the column integer to a char
                column = (char)('A' + Convert.ToChar(col));

                controller.RequestCellEdit(column.ToString() + rowNum.ToString(), cellContentsTextBox.Text);

                //Checks to see if the selected row is the last row in the grid
                if (rowNum < 98)
                {
                    //Adds 1 to row number
                    rowNum++;
                }

                controller.RequestCellSelection("" + column.ToString() + rowNum.ToString());

            }
        }

        /// <summary>
        /// Changes the mode between dark and light depending on which button is clicked on
        /// </summary>
        private void darkModeButton_CheckedChanged(object sender, EventArgs e)
        {
            //If the user selects the darkMode button, goes in here

            if (darkModeButton.Checked)
            {
                //Sets the panel to be in darkMode and changes the color
                darkMode = true;
                spreadsheetPanel.setDarkMode(darkMode);
                changeColors(true, Color.Orange, Color.Black, BorderStyle.FixedSingle);
            }
            //If the user selects lightMode, then goes in here
            if (!darkModeButton.Checked)
            {
                //Sets the panel to not be in darkMode and changes the color to reflect light mode
                darkMode = false;
                spreadsheetPanel.setDarkMode(darkMode);

                changeColors(false, Color.Black, Color.White, BorderStyle.Fixed3D);

            }

        }
        /// <summary>
        /// Changes the mode between dark and light depending on which button is clicked on
        /// </summary>
        private void lightModeButton_CheckedChanged(object sender, EventArgs e)
        {
            //If the darkMode button is selected, sets darkMode as true, and sets the panel to have darkMode
            if (darkModeButton.Checked)
            {
                darkMode = true;
                spreadsheetPanel.setDarkMode(darkMode);
            }
            //If the darkMode button is not selected, sets darkMode as false, and sets the panel to have lightMode
            if (!darkModeButton.Checked)
            {
                darkMode = false;
                spreadsheetPanel.setDarkMode(darkMode);
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {

            //If the server box is empty, then it asks the user to enter a valid server address
            if (serverNameTextBox.Text == "")
            {
                this.Invoke(new MethodInvoker(() => { MessageBox.Show("Please enter a Server Address"); }));
                return;
            }

            //If the name box is empty, then it asks the user to enter a valid name
            if (userNameTextBox.Text == "")
            {
                this.Invoke(new MethodInvoker(() => { MessageBox.Show("Please enter a Server Address"); }));
                return;
            }

            // Disables the controls and try to connect
            this.Invoke(new MethodInvoker(
                () =>
                {
                    connectButton.Enabled = false;
                    serverNameTextBox.Enabled = false;
                    userNameTextBox.Enabled = false;
                }
                ));


            //Sets keyPreview to true in order for the user
            KeyPreview = true;

            //Calls the controller's connect method
            controller.Connect(serverNameTextBox.Text, userNameTextBox.Text);
        }

        /// <summary>
        /// when undo buttton gets clicked
        /// </summary>
        private void undoSpreadsheet_Click(object sender, EventArgs e)
        {
            controller.RequestUndo();
        }

        /// <summary>
        /// when undo buttton gets clicked
        /// </summary>
        private void revertCellSelected_Click(object sender, EventArgs e)
        {
            spreadsheetPanel.GetSelection(out int col, out int row);
            row++;
            char column = (char)('A' + Convert.ToChar(col));
            string select = column.ToString() + row.ToString() + "";
            controller.RequestRevert(select);
        }

        /// <summary>
        /// A helper method to change the UI of the form, by switching the colors
        /// </summary>
        /// <Citation> https://stackoverflow.com/questions/276179/how-to-change-the-font-color-of-a-disabled-textbox </Citation>
        /// <Citation> https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-set-the-background-of-a-windows-forms-panel?view=netframeworkdesktop-4.8 </Citation>
        private void changeColors(bool b, Color foreColor, Color backColor, BorderStyle border)
        {
           
            //Changes the back color of the spreadsheet form and the menu strip
            this.BackColor = backColor;
            menuStrip.BackColor = backColor;

            //Changes the fore color and back color for the file tool strip and drop down options there
            fileToolStripMenuItem.ForeColor = foreColor;
            fileToolStripMenuItem.BackColor = backColor;
            newSpreadsheet.ForeColor = foreColor;
            newSpreadsheet.BackColor = backColor;
            revertCellSelected.ForeColor = foreColor;
            revertCellSelected.BackColor = backColor;

            closeSpreadsheet.ForeColor = foreColor;
            closeSpreadsheet.BackColor = backColor;
            undoSpreadsheet.ForeColor = foreColor;
            undoSpreadsheet.BackColor = backColor;
            connectButton.ForeColor = foreColor;
            connectButton.BackColor = backColor;
            userNameLabel.BackColor = backColor;
            userNameLabel.ForeColor = foreColor;
            serverNameLabel.ForeColor = foreColor;
            serverNameLabel.BackColor = backColor;
            spreadsheetButton.ForeColor = foreColor;
            spreadsheetButton.BackColor = backColor;
            cellNameLabel.ForeColor = foreColor;
            cellNameLabel.BackColor = backColor;

            cellContentsLabel.ForeColor = foreColor;
            cellContentsLabel.BackColor = backColor;

            cellValueLabel.ForeColor = foreColor;
            cellValueLabel.BackColor = backColor;

            newSpreadsheetLabel.ForeColor = foreColor;
            newSpreadsheetLabel.BackColor = backColor;


            existingSpreadsheetLabel.ForeColor = foreColor;
            existingSpreadsheetLabel.BackColor = backColor;

            userNameTextBox.BackColor = backColor;
            userNameTextBox.ForeColor = foreColor;
            userNameTextBox.BorderStyle = border;

            serverNameTextBox.BackColor = backColor;
            serverNameTextBox.ForeColor = foreColor;
            serverNameTextBox.BorderStyle = border;

            if (darkMode)
            {
                connectButton.FlatStyle = FlatStyle.Flat;
                connectButton.FlatAppearance.BorderColor = backColor;

                spreadsheetButton.FlatStyle = FlatStyle.Flat;
                spreadsheetButton.FlatAppearance.BorderColor = backColor;
            }

            spreadsheetComboBox.BackColor = backColor;
            spreadsheetComboBox.ForeColor = foreColor;

            //Changes the back color of the spreadsheet panel
            spreadsheetPanel.BackColor = backColor;

            //Changes the color of the text of the dark and light mode button
            darkModeButton.ForeColor = foreColor;
            lightModeButton.ForeColor = foreColor;

            //Changes the foreground and background color of the cellValue box as well
            //as adds or removes a border depending on the mode. Also enables or disables
            //the textbox to show the dark mode colors
            cellValueTextBox.Enabled = b;
            cellValueTextBox.ForeColor = foreColor;
            cellValueTextBox.BackColor = backColor;
            cellValueTextBox.BorderStyle = border;


            new_spreadsheet_box.Enabled = b;
            new_spreadsheet_box.ForeColor = foreColor;
            new_spreadsheet_box.BackColor = backColor;
            new_spreadsheet_box.BorderStyle = border;

            //Changes the foreground and background color of the cellName box as well
            //as adds or removes a border depending on the mode. Also enables or disables
            //the textbox to show the dark mode colors
            cellNameTextBox.Enabled = b;
            cellNameTextBox.BackColor = backColor;
            cellNameTextBox.ForeColor = foreColor;
            cellNameTextBox.BorderStyle = border;

            //Changes the foreground and background color of the cellContents box as well
            //as adds or removes a border depending on the mode
            cellContentsTextBox.BackColor = backColor;
            cellContentsTextBox.ForeColor = foreColor;
            cellContentsTextBox.BorderStyle = border;
        }

        private void spreadsheetPanel_Load(object sender, EventArgs e)
        {

        }

        private void SpreadsheetForm_SizeChanged(object sender, EventArgs e)
        {
            spreadsheetPanel.Height = statusStrip1.Location.Y - spreadsheetPanel.Location.Y;
            
        }
    }
}