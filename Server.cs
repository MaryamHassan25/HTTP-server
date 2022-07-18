using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {   
        Socket serverSocket;
        public Server(int portNumber, string redirectionMatrixPath)
        {   //LoadRedirection takes information written in file and load it in RedirectionRule Dictionary as key from user and value 'el haga eli hload 3liha' 
            this.LoadRedirectionRules(redirectionMatrixPath);
            //establish server
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Tcp because it is reliable as message not lost or duplicated 
            //IP which server will receive
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, portNumber);
            //Send IPs to start Bind
            this.serverSocket.Bind(endpoint);
        }

        
        public void StartServer()
        {      
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                // Using threadin to be able to work with more than one client at same time
                //TODO: accept connections and start thread for each accepted connection.
                Socket soket = serverSocket.Accept();//Accept Request and put it into socket
                Thread NewThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                NewThread.Start(soket);
            }
        }

        public void HandleConnection(object obj)
        { //this function takes request and send response
            Socket clientSocket = (Socket)obj;
            clientSocket.ReceiveTimeout = 0; //indicates an infinite time-out period.
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] ClientData = new byte[1024];
                    int ClientDataLength = clientSocket.Receive(ClientData);
                    // TODO: break the while loop if receivedLen==0
                    if (ClientDataLength == 0)
                        break;
                    // TODO: Create a Request object using received request string
                    Request NewRequest = new Request(Encoding.ASCII.GetString(ClientData));
                    // TODO: Call HandleRequest Method that returns the response
                    Response NewResponse = HandleRequest(NewRequest);
                    // TODO: Send Response back to client
                    clientSocket.Send(Encoding.ASCII.GetBytes(NewResponse.ResponseString));
                }//convert from bytearray to string

                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }

            clientSocket.Close();
        }

   
        Response HandleRequest(Request request)
        { //check request if bad request or error exist
            try
            {
                throw new NotImplementedException();
                //TODO: check for bad request 
                if (!request.ParseRequest())
                    return new Response(StatusCode.BadRequest, "text/html", LoadDefaultPage(Configuration.BadRequestDefaultPageName), "");
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string PhysicalPath = Path.Combine(Configuration.RootPath, request.relativeURI);
                //TODO: check for redirect
                string RedirectionPagePath = GetRedirectionPagePathIFExist(request.relativeURI);
                if (RedirectionPagePath != "") return new Response(StatusCode.Redirect, "text/html", LoadDefaultPage(Configuration.RedirectionDefaultPageName), RedirectionPagePath);
                //TODO: check file exists
                if (!File.Exists(PhysicalPath))
                {
                    return new Response(StatusCode.NotFound, "text/html", LoadDefaultPage(Configuration.NotFoundDefaultPageName), "");
                }
                //TODO: read the physical file
                StreamReader reader = new StreamReader(PhysicalPath);
                string file = reader.ReadToEnd();
                reader.Close();
                // Create OK response
                return new Response(StatusCode.OK, "text/html", file, "");
            }


            catch (Exception ex)
            {
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error.
                return new Response(StatusCode.InternalServerError, "text/html", LoadDefaultPage(Configuration.InternalErrorDefaultPageName), "");
            }
        }


       // check if relative path exist in configuration if exist get rules
        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }

            return string.Empty;
        }

        
        private string LoadDefaultPage(string defaultPageName)
        {
            
            //mean inpup file
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception("Default Page " + defaultPageName + " doesn't exist"));
                return string.Empty;
            }
            StreamReader reader = new StreamReader(filePath);
            string file = reader.ReadToEnd();
            reader.Close();
            return file;

        }

        
        private void LoadRedirectionRules(string filePath)
        {
            //LoadRedirection takes information written in file and load it in RedirectionRule Dictionary as key from user and value 'el haga eli hload 3liha' 
            try
            {
                StreamReader reader = new StreamReader(filePath);

                Configuration.RedirectionRules = new Dictionary<string, string>();
                while (!reader.EndOfStream)
                {
                    string temp = reader.ReadLine();
                    string[] key = temp.Split(',');
                    Configuration.RedirectionRules.Add(key[0], key[1]);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);

                Environment.Exit(1);
            }
        }


    }
}