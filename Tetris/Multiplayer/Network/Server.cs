using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tetris.Other;

namespace Tetris.Multiplayer.Network
{
    internal class Server
    {
        private static Thread _serverThread;
        private static TcpListener _clientListener;
        private static TcpClient _connectedClient;
        private static bool _isClientConnected;
        private static bool _isRunningServer;
        private static NetworkStream _stream;

        public Server()
        {
            _serverThread = new Thread(RunServer);
            _serverThread.Start(); // begin running server thread
            _isRunningServer = true;
            Instance.GetGuiDebug().DebugMessage("Server thread starting...");
        }

        private void RunServer()
        {
            try
            {
                _clientListener =
                    new TcpListener(System.Net.IPAddress.Any, 25565); // listening for tcp connections on port 25565
                _clientListener.Start();

                while (_isRunningServer)
                {
                    //handle incoming connections
                    while (!_isClientConnected)
                    {
                        Thread.Sleep(100);
                        _connectedClient = _clientListener.AcceptTcpClient(); // attempt to accept incoming connection
                        _stream = _connectedClient.GetStream(); // grab stream of connected client
                        _isClientConnected = true;
                        Instance.GetMultiplayerHandler().ShowMultiplayer();
                        Instance.GetPacket().SendPacketFromName("con"); // let the player know we've connected
                        break;
                    }

                    //While client has successfully connected
                    while (_isClientConnected)
                    {
                        try
                        {
                            byte[] buffer = new byte[2048];
                            _stream.Read(buffer, 0, buffer.Length); // read buffer
                            int receivedByte = 0;
                            foreach (byte b in buffer) // loop through buffer for any data
                            {
                                if (b != 0) // if we've received anything
                                {
                                    receivedByte++;
                                }
                            }

                            if (receivedByte == 0) // if we've received no data, let the thread sleep for 1ms(Note, maybe make a server tick so server updates less frequently).
                            {
                                Thread.Sleep(1);
                                continue;
                            }

                            string request = Encoding.UTF8.GetString(buffer, 0, receivedByte); // make it readable

                            string packetName = "";
                            string packetData = "";

                            if (request.Contains(" ")) // if given data has a spaced, the packet has data along with it
                            {
                                packetName = request.Split(' ')[0];
                                packetData = request.Split(' ')[1].Split(';')[0];
                            }
                            else // if we don't see a space, we assume there is no further data provided
                            {
                                packetName = request.Split(';')[0];
                            }

                            //containsExtra is a fallback method for a small bug where if two packets are sent within a very, very small amount of time, packets will concatenate.
                            bool containsExtra = request.Split(';')[1] != "";

                            //Run the packets RunPacket method.
                            Instance.GetPacket().RunPacketFromName(packetName, packetData);

                            //we're repeating what we've just done, but after the first packets data.
                            if (containsExtra)
                            {
                                string extraPacket = request.Split(';')[1];

                                if (extraPacket.Contains(" "))
                                {
                                    packetName = extraPacket.Split(' ')[0];
                                    packetData = extraPacket.Split(' ')[1].Split(';')[0];
                                }
                                else
                                {
                                    packetName = extraPacket.Split(';')[0];
                                }

                                Instance.GetPacket().RunPacketFromName(packetName, packetData);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("forcibly closed"))
                            {
                                Instance.GetPacket().RunPacketFromName("dis");
                                break;
                            }

                            if (ex is ThreadAbortException)
                                break;
                            Instance.GetGuiDebug().DebugMessage("Client connection interrupted, exception thrown: " + ex.Message);
                            continue;
                        }
                    }
                }
            } catch (Exception) {}
        }

        public static bool ServerRunning()
        {
            return _isRunningServer;
        }

        public static bool ConnectionEstablished()
        {
            return _isClientConnected;
        }

        public static void CloseConnection()
        {
            if (!_isRunningServer)
                return;
            //stop listening for incoming connections
            _isRunningServer = false;
            _isClientConnected = false;
            _clientListener.Stop();

            if (_connectedClient != null)
            {
                //close client connections
                _connectedClient.GetStream().Close();
                _connectedClient.Close();
                _connectedClient = null;
                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }
            }

            Instance.InMultiplayer = false;
            Instance.GetGuiDebug().DebugMessage("Stopping server...");
            //stop server thread from running
            _serverThread = null;
        }

        public static void SendData(string data)
        {
            Task.Run(() =>
            {
                try
                {
                    int byteCount = Encoding.ASCII.GetByteCount(data + 1);
                    byte[] sendData = new byte[byteCount];
                    sendData = Encoding.ASCII.GetBytes(data);

                    _stream = _connectedClient.GetStream(); // get stream
                    _stream.Write(sendData, 0, sendData.Length); // write data to stream
                    _stream.Flush(); // let them know we finished sending our data
                }
                catch (Exception)
                {
                    //Debug.DebugMessage($"Unable to send data to client. Error: {ex.Message}", 3, true);
                }
            });
        }

    }
}
