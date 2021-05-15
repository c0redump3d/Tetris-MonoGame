using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tetris.GameDebug;
using Tetris.Other;

namespace Tetris.Multiplayer.Network
{
    internal class Client
    {
        
        private static Thread _clientThread;
        private static string _ipAddress = "127.0.0.1";

        private static NetworkStream _stream;
        private static TcpClient _client;

        private static bool _connectionEstablished = false;

        public Client(string ipAddress)
        {
            _ipAddress = ipAddress; // set to given ip.

            _clientThread = new Thread(RunClient);
            _clientThread.Start(); // start running the client thread

            Debug.DebugMessage($"Client attempting to connect to: {ipAddress}", 4);
        }

        private void RunClient()
        {
            int tries = 0;
            
        connection:
            try
            {
                _client = new TcpClient();
                var result = _client.BeginConnect(_ipAddress, 25565,null, null); // attempt connection asynchronously(to timeout connections every 1.5 seconds). 

                bool connected = result.AsyncWaitHandle.WaitOne(1500); // returns false if connection is not made within 1.5 seconds.

                if (!connected)
                {
                    tries++; // add to tries and throw exception to get out.
                    throw new Exception("Timeout");
                }

                Instance.GetMultiplayerHandler().ShowMultiplayer(); // show multiplayer screen once connected

                _stream = _client.GetStream();
                _connectionEstablished = true;

                Instance.GetPacket().SendPacketFromName("con");
                Instance.GetGuiMultiplayer().Connected();

                while (_connectionEstablished)
                {
                    try
                    {
                        byte[] buffer = new byte[2048];
                        _stream.Read(buffer, 0, buffer.Length);
                        int receivedByte = 0;
                        foreach (byte b in buffer)
                        {
                            if (b != 0)
                            {
                                receivedByte++;
                            }
                        }

                        if (receivedByte == 0)
                        {
                            Thread.Sleep(1);
                            continue;
                        }

                        string request = Encoding.UTF8.GetString(buffer, 0, receivedByte);

                        string packetName = "";
                        string packetData = "";

                        if (request.Contains(" "))
                        {
                            packetName = request.Split(' ')[0];
                            packetData = request.Split(' ')[1].Split(';')[0];
                        }
                        else
                        {
                            packetName = request.Split(';')[0];
                        }

                        bool containsExtra = request.Split(';')[1] != "";
                        
                        Instance.GetPacket().RunPacketFromName(packetName, packetData);

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
                    catch(Exception ex)
                    {
                        if (ex.Message.Contains("forcibly closed"))
                        {
                            Instance.GetPacket().RunPacketFromName("dis");
                            break;
                        }

                        if (!(ex is ThreadAbortException))
                            Debug.DebugMessage($"Client received information from server but did not understand it! Error: {ex.Message}", 4, true);
                        continue;
                    }
                }
            }
            catch (Exception)
            {
                if (tries == 0)
                {
                    Instance.GetPacket().RunPacketFromName("dis");
                    return;
                }
                
                Debug.DebugMessage($"Pinging {_ipAddress} ({tries})...", 4, false);
                
                if(tries < 10)
                    goto connection;
                
                Debug.DebugMessage($"Client was unable to connect to {_ipAddress} after 10 tries.", 4, false);
                _ipAddress = "127.0.0.1";
                Instance.GetGuiMultiplayer().FailedConnect = true;
                Instance.GetGuiMultiplayer().IsConnecting = false;
                Instance.GetGuiMultiplayer().EnableServerButton();
                CloseConnection();
            }
        }

        public static bool IsConnected()
        {
            return _connectionEstablished;
        }

        public static void SendData(string data)
        {
            if (!_connectionEstablished)
                return;

            Task.Run(() =>
            {
                try
                {
                    int byteCount = Encoding.ASCII.GetByteCount(data + 1);
                    byte[] sendData = new byte[byteCount];
                    sendData = Encoding.ASCII.GetBytes(data);

                    _stream = _client.GetStream();
                    _stream.Write(sendData, 0, sendData.Length);
                    _stream.Flush();
                }
                catch (Exception ex)
                {
                    Debug.DebugMessage("Unable to send data to server. Error: " + ex.Message, 3, true);
                }
            });
        }

        public static void CloseConnection()
        {
            if (!_connectionEstablished)
                return;
            _connectionEstablished = false;

            if (_client != null)
            {
                _client.GetStream().Close();
                _client.Close();
                _client = null;
                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }
            }
            
            Instance.InMultiplayer = false;
            Debug.DebugMessage("Closing connection...", 4, true);
            _clientThread = null;
        }

    }
}
