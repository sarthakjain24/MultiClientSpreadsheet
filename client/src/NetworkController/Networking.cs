using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SS
{
    public static class Networking
    {
        /////////////////////////////////////////////////////////////////////////////////////////
        // Server-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Starts a TcpListener on the specified port and starts an event-loop to accept new clients.
        /// The event-loop is started with BeginAcceptSocket and uses AcceptNewClient as the callback.
        /// AcceptNewClient will continue the event-loop.
        /// </summary>
        /// <param name="toCall">The method to call when a new connection is made</param>
        /// <param name="port">The the port to listen on</param>
        public static TcpListener StartServer(Action<SocketState> toCall, int port)
        {
            //Starts a new listener and begins accepting new clients
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            try
            {
                listener.Start();
                Tuple<Action<SocketState>, TcpListener> tuple = new Tuple<Action<SocketState>, TcpListener>(toCall, listener);
                listener.BeginAcceptSocket(AcceptNewClient, tuple);
            }
            catch (Exception e)
            {

            }
            return listener;
        }

        /// <summary>
        /// To be used as the callback for accepting a new client that was initiated by StartServer, and 
        /// continues an event-loop to accept additional clients.
        ///
        /// Uses EndAcceptSocket to finalize the connection and create a new SocketState. The SocketState's
        /// OnNetworkAction should be set to the delegate that was passed to StartServer.
        /// Then invokes the OnNetworkAction delegate with the new SocketState so the user can take action. 
        /// 
        /// If anything goes wrong during the connection process (such as the server being stopped externally), 
        /// the OnNetworkAction delegate should be invoked with a new SocketState with its ErrorOccured flag set to true 
        /// and an appropriate message placed in its ErrorMessage field. The event-loop should not continue if
        /// an error occurs.
        ///
        /// If an error does not occur, after invoking OnNetworkAction with the new SocketState, an event-loop to accept 
        /// new clients should be continued by calling BeginAcceptSocket again with this method as the callback.
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginAcceptSocket. It must contain a tuple with 
        /// 1) a delegate so the user can take action (a SocketState Action), and 2) the TcpListener</param>
        private static void AcceptNewClient(IAsyncResult ar)
        {
            //initializes the variables used in this method
            SocketState socketState = null;
            Action<SocketState> action = null;
            TcpListener listener = null;
            Socket socket = null;

            try
            {
                //Creates a tuple from the given parameter and sets the listener and action to the given items
                Tuple<Action<SocketState>, TcpListener> tuplePassed = (Tuple<Action<SocketState>, TcpListener>)ar.AsyncState;
                action = tuplePassed.Item1;
                listener = tuplePassed.Item2;

                socket = listener.EndAcceptSocket(ar);
                //creates a new socketstate with the socketstate and the socket from the tuple
                socketState = new SocketState(action, socket);

            }
            //Catches any Exception Thrown and creates a new scoket state to nofify the user
            catch (Exception e)
            {
                socketState = new SocketState(action, socket);
                socketState.ErrorOccured = true;
                socketState.ErrorMessage = e.Message;
                action(socketState);
                return;

            }

            socketState.OnNetworkAction(socketState);
            try
            {
                //starts accepting new clients from the listener inside of the tuple
                Tuple<Action<SocketState>, TcpListener> tuple = new Tuple<Action<SocketState>, TcpListener>(action, listener);
                listener.BeginAcceptSocket(AcceptNewClient, tuple);
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Stops the given TcpListener.
        /// </summary>
        public static void StopServer(TcpListener listener)
        {
            try
            {
                //Stops the listener
                listener.Stop();
            }
            catch (Exception e)
            {

            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        // Client-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Begins the asynchronous process of connecting to a server via BeginConnect, 
        /// and using ConnectedCallback as the method to finalize the connection once it's made.
        /// 
        /// If anything goes wrong during the connection process, toCall should be invoked 
        /// with a new SocketState with its ErrorOccured flag set to true and an appropriate message 
        /// placed in its ErrorMessage field. Between this method and ConnectedCallback, toCall should 
        /// only be invoked once on error.
        ///
        /// This connection process should timeout and produce an error (as discussed above) 
        /// if a connection can't be established within 3 seconds of starting BeginConnect.
        /// 
        /// </summary>
        /// <param name="toCall">The action to take once the connection is open or an error occurs</param>
        /// <param name="hostName">The server to connect to</param>
        /// <param name="port">The port on which the server is listening</param>
        /// <Citation> https://stackoverflow.com/questions/1062035/how-to-configure-socket-connect-timeout </Citation>
        public static void ConnectToServer(Action<SocketState> toCall, string hostName, int port)
        {
            // Establish the remote endpoint for the socket.
            IPHostEntry ipHostInfo;
            IPAddress ipAddress = IPAddress.None;

            SocketState socketState = null;

            // Determine if the server address is a URL or an IP
            try
            {
                ipHostInfo = Dns.GetHostEntry(hostName);
                bool foundIPV4 = false;
                foreach (IPAddress addr in ipHostInfo.AddressList)
                    if (addr.AddressFamily != AddressFamily.InterNetworkV6)
                    {
                        foundIPV4 = true;
                        ipAddress = addr;
                        break;
                    }
                // Didn't find any IPV4 addresses
                if (!foundIPV4)
                {
                    //Creates a null socket to be passed into creating a new socketState, with it indicating errors 
                    Socket temp = null;
                    SocketState tempSocketState = new SocketState(toCall, temp);

                    tempSocketState.ErrorOccured = true;
                    tempSocketState.ErrorMessage = "Didn't find any IPV4 addresses";

                    //Invokes the toCall delegate from the socketState class onto the temporary socketState and returns accordingly
                    toCall(tempSocketState);
                    return;
                }
            }
            catch (Exception)
            {
                // see if host name is a valid ipaddress
                try
                {
                    ipAddress = IPAddress.Parse(hostName);
                }
                //If IP address is invalid
                catch (Exception)
                {
                    //Creates a null socket to be passed into creating a new socketState, with it indicating errors 
                    Socket temp = null;
                    SocketState tempSocketState = new SocketState(toCall, temp);

                    tempSocketState.ErrorOccured = true;
                    tempSocketState.ErrorMessage = "Host has an invalid IP address";

                    //Invokes the toCall delegate from the socketState class onto the temporary socketState and returns accordingly
                    toCall(tempSocketState);
                    return;
                }
            }

            // Create a TCP/IP socket.
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // This disables Nagle's algorithm (google if curious!)
            // Nagle's algorithm can cause problems for a latency-sensitive 
            // game like ours will be 
            socket.NoDelay = true;

            //Creates a new SocketState with the toCall as the delegate, and the socket representing the socket
            SocketState state = new SocketState(toCall, socket);
            try
            {
                //Begins connecting the socket associated with the socketState by using the ipAddress, port, ConnectedCallback
                //as the Callback, and SocketState as the object
                IAsyncResult timeoutCheck = state.TheSocket.BeginConnect(ipAddress, port, ConnectedCallback, state);

                //Tries to see if the socket can successfully connect in less than 3 seconds, and if it can't then it closed the socket
                bool success = timeoutCheck.AsyncWaitHandle.WaitOne(3000, true);
                if (!socket.Connected)
                {
                    state.TheSocket.Close();
                }
            }
            catch (Exception e)
            {

                //Creates a null socket to be passed into creating a new socketState, with it indicating errors 
                Socket temp = null;
                SocketState tempSocketState = new SocketState(toCall, temp);

                tempSocketState.ErrorOccured = true;
                tempSocketState.ErrorMessage = "Connect to server failed";

                //Invokes the toCall delegate from the socketState class onto the temporary socketState and returns accordingly
                toCall(tempSocketState);
                return;
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a connection process that was initiated by ConnectToServer.
        ///
        /// Uses EndConnect to finalize the connection.
        /// 
        /// As stated in the ConnectToServer documentation, if an error occurs during the connection process,
        /// either this method or ConnectToServer (not both) should indicate the error appropriately.
        /// 
        /// If a connection is successfully established, invokes the toCall Action that was provided to ConnectToServer (above)
        /// with a new SocketState representing the new connection.
        /// 
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginConnect</param>
        private static void ConnectedCallback(IAsyncResult ar)
        {
            //Initalizes the SocketState uses later in this method
            SocketState state = null;
            try
            {
                //sets the SocketState to the parameter, uses EndConnect to finalize the connection and then invokes the ToCall action
                state = (SocketState)ar.AsyncState;
                state.TheSocket.EndConnect(ar);
                state.OnNetworkAction(state);
            }
            //Catches any Exceptions thrown, notifies the user and then invokes the toCall Action
            catch (Exception e)
            {
                state.ErrorOccured = true;
                state.ErrorMessage = "Connect to server failed";
                state.OnNetworkAction(state);
                return;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////
        // Server and Client Common Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Begins the asynchronous process of receiving data via BeginReceive, using ReceiveCallback 
        /// as the callback to finalize the receive and store data once it has arrived.
        /// The object passed to ReceiveCallback via the AsyncResult should be the SocketState.
        /// 
        /// If anything goes wrong during the receive process, the SocketState's ErrorOccured flag should 
        /// be set to true, and an appropriate message placed in ErrorMessage, then the SocketState's
        /// OnNetworkAction should be invoked. Between this method and ReceiveCallback, OnNetworkAction should only be 
        /// invoked once on error.
        /// 
        /// </summary>
        /// <param name="state">The SocketState to begin receiving</param>
        public static void GetData(SocketState state)
        {
            //Calls the begin receive method
            try
            {
                state.TheSocket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, ReceiveCallback, state);

            }
            //Catches any Exceptions thrown, notifies the user and then invokes the toCall Action
            catch (Exception e)
            {
                state.ErrorOccured = true;
                state.ErrorMessage = "Error in GetData";
                state.OnNetworkAction(state);
                return;
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a receive operation that was initiated by GetData.
        /// 
        /// Uses EndReceive to finalize the receive.
        ///
        /// As stated in the GetData documentation, if an error occurs during the receive process,
        /// either this method or GetData (not both) should indicate the error appropriately.
        /// 
        /// If data is successfully received:
        ///  (1) Read the characters as UTF8 and put them in the SocketState's unprocessed data buffer (its string builder).
        ///      This must be done in a thread-safe manner with respect to the SocketState methods that access or modify its 
        ///      string builder.
        ///  (2) Call the saved delegate (OnNetworkAction) allowing the user to deal with this data.
        /// </summary>
        /// <param name="ar"> 
        /// This contains the SocketState that is stored with the callback when the initial BeginReceive is called.
        /// </param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            //Initializes a null socketState
            SocketState socketState = null;
            try
            {
                //Gets the socketState passed into the AsyncCallback
                socketState = (SocketState)ar.AsyncState;
                //Checks for the number of bytes given by ending the socket reception
                int numBytes = socketState.TheSocket.EndReceive(ar);

                //If numBytes is 0, then it sets the error occurred as true
                if (numBytes == 0)
                {
                    socketState.ErrorOccured = true;
                }

                //Encodes the message using the buffer in the socketState,and appends it using the socketState's
                //data as the key to the lock to ensure we don't get cross-threading
                string message = Encoding.UTF8.GetString(socketState.buffer, 0, numBytes);
                lock (socketState.data)
                {
                    socketState.data.Append(message);
                }
            }
            //Catches the expection
            catch (Exception e)
            {
                //Sets the error occured as true, message denoting the error, and invoking the delegate on the socketState, and returns
                socketState.ErrorOccured = true;
                socketState.ErrorMessage = e.Message;
                socketState.OnNetworkAction(socketState);
                return;
            }
            //Invokes the OnNetworkAction delegate on the socketState itself
            socketState.OnNetworkAction(socketState);
        }

        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendCallback to finalize the send process.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool Send(Socket socket, string data)
        {
            try
            {
                //If the socket is connected, then it gets the bytes of the data, and adds it to an array, and begins the process
                //of sending the data via BeginSend, whilst having SendCallback as the AsyncCallBack and the socket as the object
                if (socket.Connected)
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(data);
                    socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendCallback, socket);
                    return true;
                }
                //Returns false if socket was not connected
                else
                {
                    return false;
                }
            }
            //If there is any other possible exception, this closes the socket and returns false
            catch (Exception e)
            {
                socket.Close();
                return false;
            }

        }

        /// <summary>
        /// To be used as the callback for finalizing a send operation that was initiated by Send.
        ///
        /// Uses EndSend to finalize the send.
        /// 
        /// This method must not throw, even if an error occured during the Send operation.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                //Gets the socket that was passed into the AsyncCallBack
                Socket s = (Socket)ar.AsyncState;

                //Ends the send on that socket
                s.EndSend(ar);
            }
            //Catches any possible exceptions
            catch (Exception e)
            {

            }
        }


        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendAndCloseCallback to finalize the send process.
        /// This variant closes the socket in the callback once complete. This is useful for HTTP servers.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool SendAndClose(Socket socket, string data)
        {
            try
            {

                //If the socket is connected, then it gets the bytes of the data, and adds it to an array, and begins the process
                //of sending the data via BeginSend, whilst having SendCallback as the AsyncCallBack and the socket as the object
                //and return true after
                if (socket.Connected)
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(data);
                    socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendAndCloseCallback, socket);
                    return true;
                }
                //Returns false if socket is not connected
                else
                {
                    return false;
                }
            }
            //Catches any possible exceptions
            catch (Exception e)
            {
                //Closes the socket, and returns false
                socket.Close();
                return false;
            }

        }

        /// <summary>n./g00y
        /// To be used as the callback for finalizing a send operation that was initiated by SendAndClose.
        ///
        /// Uses EndSend to finalize the send, then closes the socket.
        /// 
        /// This method must not throw, even if an error occured during the Send operation.
        /// 
        /// This method ensures that the socket is closed before returning.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendAndCloseCallback(IAsyncResult ar)
        {
            try
            {

                //Gets the socket that was passed into the AsyncCallBack
                Socket s = (Socket)ar.AsyncState;

                //Ends the send on that socket
                s.EndSend(ar);

                //Closes the socket
                s.Close();
            }
            //Catches any possible exceptions
            catch (Exception e)
            {
                //Gets the socket that was passed into the AsyncCallBack
                Socket s = (Socket)ar.AsyncState;
                s.Close();
            }
        }

    }
}
