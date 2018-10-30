using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RTSPRedirect
{
    internal class RtspRedirect
    {
        /// <summary>
        /// Calls the RTSP commands needed to obtain the redirect location for the given RTSP endpoint.
        /// Please note that due to loading, the redirect can change over time.
        /// </summary>
        /// <param name="urlFromDataSource">The RTSP URL obtained from the data source.</param>
        /// <param name="doPlayback">True to return the redirect URL for playback, False for live.</param>
        /// <param name="urlRedirected">The redirect URL (if successful).</param>
        /// <param name="errorMessage">The error message (if unsuccessful).</param>
        /// <returns>True if the redirect was successful, otherwise False.</returns>
        public static bool ObtainRedirect(string urlFromDataSource, bool doPlayback, ref string urlRedirected, ref string errorMessage)
        {
            // Validate the RTSP URL
            if (!urlFromDataSource.StartsWith("rtsp://"))
            {
                errorMessage = "Error: Expecting a URL in form of \"rtsp://....\"";
                return false;
            }

            // Check if the RTSP URL is for a multicast stream
            var isMulticast = urlFromDataSource.Contains("multicast=true");

            // Create the communication socket
            var socket = CreateSocket(urlFromDataSource, ref errorMessage);
            if (socket == null)
                return false;

            // Set the RTP/RTCP port pair that will be used for the client_port parameter in the SETUP command. This parameter provides
            // the RTP/RTCP port pair on which the client has chosen to receive the media data and control information. The server
            // will send this data to the client after a successful PLAY command.
            // Note: This example does not receive any media data so the ports defined here are just for the example. In a real scenario
            // these should be set to the ports your RTSP client is set up to receive data on.
            var clientPort = ((IPEndPoint) socket.LocalEndPoint).Port + 1;

            // RTP wants an even socket to receive data, so bump the port number if it's odd.
            // Note: The RTCP port will be set to the next sequential port number, which is performed in the SendSetup method.
            if (clientPort % 2 == 1)
                clientPort += 1;

            ///////////////////////////////////////////////////////////////////////////
            //
            //  Begin RTSP Commands 
            //
            ///////////////////////////////////////////////////////////////////////////

            // The PLAY command needs the session ID, which is obtained from the SETUP command.
            // Issue OPTIONS, then DESCRIBE, then SETUP.
            var sequenceNum = 0;
            var controlUri = string.Empty;
            var rtspReturnCode = SendOptions(socket, urlFromDataSource, sequenceNum++, ref errorMessage);
            if (rtspReturnCode != 200)
                return false;

            rtspReturnCode = SendDescribe(socket, urlFromDataSource, sequenceNum++, ref controlUri, ref errorMessage);
            if (rtspReturnCode != 200)
                return false;

            var sessionId= string.Empty;
            var setupUri = string.IsNullOrEmpty(controlUri) ? urlFromDataSource : controlUri;
            rtspReturnCode = SendSetup(socket, setupUri, sequenceNum++, ref sessionId, clientPort, isMulticast, ref errorMessage);
            if (rtspReturnCode != 200)
                return false;

            var startUrl = urlFromDataSource;
            // You can have many redirects depending on the SETUP request.  Limiting to 20 
            for (var i = 0; i < 20; i++)
            {
                rtspReturnCode = SendPlay(socket, startUrl, sequenceNum++, sessionId, doPlayback, ref urlRedirected, ref errorMessage);
                if (rtspReturnCode == 200)
                {
                    urlRedirected = startUrl;
                    break;
                }

                if (rtspReturnCode == 302)
                {
                    // Call TEARDOWN first since you will get new session ID
                    rtspReturnCode = SendTeardown(socket, startUrl, sequenceNum++, sessionId, ref errorMessage);
                    if (rtspReturnCode != 200)
                        return false;

                    // Have to start over with new RTSP address (OPTIONS, DESCRIBE, SETUP, PLAY)
                    rtspReturnCode = SendOptions(socket, urlRedirected, sequenceNum++, ref errorMessage);
                    if (rtspReturnCode != 200)
                        return false;

                    rtspReturnCode = SendDescribe(socket, urlRedirected, sequenceNum++, ref controlUri, ref errorMessage);
                    if (rtspReturnCode != 200)
                        return false;

                    setupUri = string.IsNullOrEmpty(controlUri) ? urlRedirected : controlUri;
                    rtspReturnCode = SendSetup(socket, setupUri, sequenceNum++, ref sessionId, clientPort, isMulticast, ref errorMessage);
                    if (rtspReturnCode != 200)
                        return false;

                    // In case there is another redirect, and for final TEARDOWN call
                    startUrl = urlRedirected;
                }
                else 
                {
                    socket.Close();
                    return false;
                }
            }

            // Add a sleep if you want to watch some streaming on wireshark.
            //System.Threading.Thread.Sleep(1000);

            // Call TEARDOWN the stream
            SendTeardown(socket, startUrl, sequenceNum, sessionId, ref errorMessage);
            if (rtspReturnCode != 200)
                return false;

            // Close the socket
            socket.Close();

            return true;
        }

        /// <summary>
        /// Sends the RTSP OPTIONS command.
        /// </summary>
        /// <param name="socket">The communication socket.</param>
        /// <param name="url">The RTSP URL.</param>
        /// <param name="sequenceNum">The sequence number for the RTSP request-response pair.</param>
        /// <param name="errorMessage">The error message, if any.</param>
        /// <returns>The status code of the response.</returns>
        private static int SendOptions(Socket socket, string url, int sequenceNum, ref string errorMessage)
        {
            var receivedString = string.Empty;
            var sendString = "OPTIONS " + url + " RTSP/1.0\r\n" +
                             "CSeq: " + sequenceNum + "\r\n" +
                             "User-Agent: Pelco Redirect Example\r\n\r\n";

            var rtspReturnCode = SocketSendAndReceive(socket, sendString, ref receivedString);

            // Can have a redirect here, but not handling right now
            if (rtspReturnCode == 301 || rtspReturnCode == 302)
                return rtspReturnCode;

            if (rtspReturnCode != 200)
            {
                errorMessage = receivedString;
                socket.Close();
            }

            return rtspReturnCode;
        }

        /// <summary>
        /// Sends the RTSP DESCRIBE command.
        /// </summary>
        /// <param name="socket">The communication socket.</param>
        /// <param name="url">The RTSP URL.</param>
        /// <param name="sequenceNum">The sequence number for the RTSP request-response pair.</param>
        /// <param name="controlUri">The URL to be used for controlling the media stream.</param>
        /// <param name="errorMessage">The error message, if any.</param>
        /// <returns>The status code of the response.</returns>
        private static int SendDescribe(Socket socket, string url, int sequenceNum, ref string controlUri, ref string errorMessage)
        {
            var receivedString = string.Empty;
            var sendString = "DESCRIBE " + url + " RTSP/1.0\r\n" +
                             "CSeq: " + sequenceNum + "\r\n" +
                             "User-Agent: Pelco Redirect Example\r\n" +
                             "Accept: application/sdp\r\n\r\n";

            var rtspReturnCode = SocketSendAndReceive(socket, sendString, ref receivedString);
            if (rtspReturnCode != 200)
            {
                errorMessage = receivedString;
                socket.Close();
                return rtspReturnCode;
            }

            // Get the control URI if there is one (which needs to be used in SETUP). It will start with "a=control:". The control
            // can contain the entire address, or part of the address that needs to be added to the "Content-Base". Find "m=vIDeo"
            // first, then find "a=control".
            controlUri = string.Empty;
            if (!receivedString.Contains("m=video"))
                return rtspReturnCode;

            controlUri = receivedString.Substring(receivedString.IndexOf("m=video", StringComparison.Ordinal) + 7);
            if (controlUri.Contains("a=control:"))
            {
                controlUri = controlUri.Substring(controlUri.IndexOf("a=control:", StringComparison.Ordinal) + 10);
                controlUri = controlUri.Substring(0, controlUri.IndexOf("\r\n", StringComparison.Ordinal));
                if (controlUri.Contains("//"))
                    return rtspReturnCode;

                if (!receivedString.Contains("Content-Base"))
                {
                    errorMessage = "Error: Cannot find Content-Base in Describe";
                    return -1;
                }

                var contentBase = receivedString.Substring(receivedString.IndexOf("Content-Base:", StringComparison.Ordinal) + 13);
                contentBase = contentBase.TrimStart(' ');
                contentBase = contentBase.Substring(0, contentBase.IndexOf("\r\n", StringComparison.Ordinal));
                controlUri = contentBase + (contentBase[contentBase.Length - 1] == '/' ? string.Empty : "/") + controlUri;
            }
            else
                controlUri = string.Empty;

            return rtspReturnCode;
        }

        /// <summary>
        /// Sends the RTSP SETUP command.
        /// </summary>
        /// <param name="socket">The communication socket.</param>
        /// <param name="url">The RTSP URL.</param>
        /// <param name="sequenceNum">The sequence number for the RTSP request-response pair.</param>
        /// <param name="sessionID">The RTSP session identifier.</param>
        /// <param name="port">The client port.</param>
        /// <param name="useMulticast">True to specify mutlicast in the Transport header, False for unicast.</param>
        /// <param name="errorMessage">The error message, if any.</param>
        /// <returns>The status code of the response.</returns>
        private static int SendSetup(Socket socket, string url, int sequenceNum, ref string sessionID, int port, bool useMulticast, ref string errorMessage)
        {
            var receivedString = string.Empty;
            var sendString = "SETUP " + url + " RTSP/1.0\r\n" +
                             "CSeq: " + sequenceNum + "\r\n" +
                             "User-Agent: Pelco Redirect Example\r\n" +
                             "Transport: RTP/AVP;" + (useMulticast ? "multicast" : "unicast") + ";client_port=" + port + "-" + (port + 1) + "\r\n\r\n";

            var rtspReturnCode = SocketSendAndReceive(socket, sendString, ref receivedString);
            if (rtspReturnCode != 200) 
            {
                errorMessage = receivedString;
                socket.Close();
                return rtspReturnCode;
            }

            // Get the session ID
            sessionID = receivedString.ToLower();
            if (!sessionID.Contains("session: "))
            {
                errorMessage = "Error: Unable to get session id\r\n" + receivedString;
                return -1;
            }

            sessionID = sessionID.Substring(sessionID.IndexOf("session: ", StringComparison.Ordinal) + 9);

            // This should be safe
            sessionID = sessionID.Substring(0, sessionID.IndexOf('\r'));

            // Check for semicolon before getting the session ID
            if (sessionID.Contains(";"))
                sessionID = sessionID.Substring(0, sessionID.IndexOf(';'));

            return rtspReturnCode;
        }

        // This will handle finding redirect too
        /// <summary>
        /// Sends the RTSP PLAY command.
        /// </summary>
        /// <param name="socket">The communication socket.</param>
        /// <param name="url">The RTSP URL.</param>
        /// <param name="sequenceNum">The sequence number for the RTSP request-response pair.</param>
        /// <param name="sessionID">The RTSP session identifier.</param>
        /// <param name="doPlayback">True to send a playback request, False for live.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="errorMessage">The error message, if any.</param>
        /// <returns>The status code of the response.</returns>
        private static int SendPlay(Socket socket, string url, int sequenceNum, string sessionID, bool doPlayback, ref string redirectUrl, ref string errorMessage)
        {
            var receivedString = string.Empty;
            var sendString = "PLAY " + url + " RTSP/1.0\r\n" +
                             "CSeq: " + sequenceNum + "\r\n" +
                             "User-Agent: Pelco Redirect Example\r\n";

            // If playback is requested, add the Scale header with a negative value. This will set the playback speed to -1x, i.e. reverse from live.
            // Note: Playback can also be initiated using the Range header with a value containing the playback start time in the format: clock=YYYYMMDDTHHMMSS.fffZ-
            if (doPlayback)
                sendString += "Scale: -1.000000\r\n";

            sendString += "Session: " + sessionID + "\r\n\r\n";

            var rtspReturnCode = SocketSendAndReceive(socket, sendString, ref receivedString);
            // Can have a redirect here (temporary)
            if (rtspReturnCode == 302)
            {
                // In this string should be the redirect information we need
                redirectUrl = receivedString;
                if (!redirectUrl.Contains("Location:") || !redirectUrl.Contains("RTSP/1.0"))
                {
                    errorMessage = "Error: No redirect address and one was expected\r\n" + receivedString;
                    socket.Close();
                    return -1;
                }

                redirectUrl = redirectUrl.Substring(redirectUrl.IndexOf("Location:", StringComparison.Ordinal) + 9);
                redirectUrl = redirectUrl.TrimStart(' ');
                redirectUrl = redirectUrl.Substring(0, redirectUrl.IndexOf("\r\n", StringComparison.Ordinal));
                return rtspReturnCode;
            }

            if (rtspReturnCode != 200)
            {
                errorMessage = receivedString;
                socket.Close();
            }

            return rtspReturnCode;
        }

        /// <summary>
        /// Sends the RTSP TEARDOWN command.
        /// </summary>
        /// <param name="socket">The communication socket.</param>
        /// <param name="url">The RTSP URL.</param>
        /// <param name="sequenceNum">The sequence number for the RTSP request-response pair.</param>
        /// <param name="sessionID">The RTSP session identifier.</param>
        /// <param name="errorMessage">The error message, if any.</param>
        /// <returns>The status code of the response.</returns>
        private static int SendTeardown(Socket socket, string url, int sequenceNum, string sessionID, ref string errorMessage)
        {
            var receivedString = string.Empty;
            var sendString = "TEARDOWN " + url + " RTSP/1.0\r\n" +
                             "CSeq: " + sequenceNum + "\r\n" +
                             "User-Agent: Pelco Redirect Example\r\n" +
                             "Session: " + sessionID + "\r\n\r\n";

            var rtspReturnCode = SocketSendAndReceive(socket, sendString, ref receivedString);
            if (rtspReturnCode != 200)
            {
                errorMessage = receivedString;
                socket.Close();
            }

            return rtspReturnCode;
        }

        /// <summary>
        /// Sends a message over the socket conneciton and returns the received response.
        /// </summary>
        /// <param name="socket">The communication socket.</param>
        /// <param name="request">The request to send.</param>
        /// <param name="response">The received response.</param>
        /// <returns></returns>
        private static int SocketSendAndReceive(Socket socket, string request, ref string response)
        {
            // Initialize the response string if needed
            if (response == null)
                response = string.Empty;

            // Encode the request into a byte sequence
            var requestBytes = Encoding.ASCII.GetBytes(request);

            try
            {
                socket.Send(requestBytes, requestBytes.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                response = "Socket Error: " + e.Message;
                return -1;
            }

            int bytes;
            var bytesReceived = new byte[5000];
            try
            {
                bytes = socket.Receive(bytesReceived, bytesReceived.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                response = "Socket Error: " + e.Message;
                return -1;
            }

            if (bytes == 5000)
            {
                response = "Socket Error: You need to make a bigger byte buffer for 'bytesReceived' in socket communications";
                return -1;
            }

            // Decode the response bytes
            response = Encoding.Default.GetString(bytesReceived);
            if (!response.Contains("RTSP/1.0"))
                return -1;

            var rtspReturnCode = Convert.ToInt32(response.Substring(9, 3));
            return rtspReturnCode;
        }

        /// <summary>
        /// Validates an RTSP URL.
        /// </summary>
        /// <param name="url">The RTSP URL.</param>
        /// <param name="errorMessage">The error message, if any.</param>
        /// <returns>True if the RTSP URL is valid, otherwise False.</returns>
        private static Socket CreateSocket(string url, ref string errorMessage)
        {
            // Create the server end point for the socket connection
            IPEndPoint serverEndPoint;
            try
            {
                // Get the IP address and port from the RTSP URL
                var uri = new Uri(url);
                var port = uri.Port;
                if (port == -1)
                    port = 554;

                serverEndPoint = new IPEndPoint(IPAddress.Parse(uri.Host), port);
            }
            catch (Exception e)
            {
                errorMessage = "IPEndPoint Error: " + e.Message;
                return null;
            }

            // Create the communication socket
            Socket socket;
            try
            {
                socket = new Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception e)
            {
                errorMessage = "Socket Error: " + e.Message;
                return null;
            }

            // Connect the socket to the server endpoint
            try
            {
                socket.Connect(serverEndPoint);
            }
            catch (Exception e)
            {
                socket.Close();
                errorMessage = "Socket Error: " + e.Message;
                return null;
            }

            return socket;
        }
    }
}
