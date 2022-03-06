using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Tetris.GUI;
using Tetris.GUI.DebugMenu;
using Tetris.Util;

namespace Tetris.Multiplayer.Network
{
    /// <summary>
    /// Manages the multiplayer network(connecting, disconnecting, packets, etc.), uses LiteNetLib for networking.
    /// </summary>
    public class NetworkManager
    {
        private bool runningClient = false;
        public bool Connected = false;
        private bool runningServer = false;
        private Packet packets;
        public bool IsServer() => runningServer;
        public int GetPing() => Peer.Ping;

        private float UpdateTime = 15f;

        private NetManager Client;
        private NetManager Server;
        private NetPeer Peer;

        private NetworkManager()
        {
            packets = new Packet();
        }

        public void SendPacket(int id)
        {
            packets.SendPacketFromID(id);
        }

        private void RunPacketFromID(int id, NetPacketReader reader)
        {
            packets.RunPacketFromID(id, reader);
        }
        
        public void Connect(string ip, int port, string password)
        {
            runningClient = true;
            DebugConsole.Instance.AddMessage($"Connecting to {ip}:{port}, with password:{password}");
            
            EventBasedNetListener listener = new EventBasedNetListener();
            Client = new NetManager(listener);
            Client.Start();
            Client.MaxConnectAttempts = 5; // ping server 5 times before giving up.
            Client.Connect(ip, port, password);
            //Event is raised if the client has successfully connected to the server.
            listener.PeerConnectedEvent += peer =>
            {
                DebugConsole.Instance.AddMessage($"Connected to {peer.EndPoint}");
                Peer = peer;
                DebugConsole.Instance.AddMessage("Player has successfully connected!");
                Gui.Instance.MultiplayerMessage = "Connected to host";
                Connected = true;
            };
            listener.NetworkReceiveEvent += ReceiveInformation;
            listener.PeerDisconnectedEvent += Disconnect;
        }

        public void StopServer()
        {
            DebugConsole.Instance.AddMessage($"Stopping server.");
            Connected = false;
            runningServer = false;
            Server.Stop();
            Server = null;
        }
        
        public void Disconnect()
        {
            Disconnect(Peer, new DisconnectInfo());
        }
        
        private void Disconnect(NetPeer peer, DisconnectInfo info)
        {
            runningClient = false;
            Gui.Instance.MultiplayerMessage = $"Disconnected: {info.Reason}";
            if (runningServer)
            {
                Connected = false;
                runningServer = false;
                Server.Stop();
                Server = null;
            }
            else
            {
                if (Connected)
                {
                    Connected = false;
                    Client.Stop();
                    Client = null;
                }
            }
        }
        
        private void UpdateClient(GameTime gameTime)
        {
            UpdateTime -= gameTime.ElapsedGameTime.Milliseconds;

            //Delay update time to every 15ms, any faster really isn't needed and is easier on network/CPU usage.
            if (UpdateTime < 0f)
            {
                Client.PollEvents();
                UpdateTime = 15f;
            }
        }

        private void UpdateServer(GameTime gameTime)
        {
            UpdateTime -= gameTime.ElapsedGameTime.Milliseconds;

            //Again server is only updated every 15ms.
            if (UpdateTime < 0f)
            {
                Server.PollEvents();
                UpdateTime = 15f;
            }
        }

        public void UpdateNetwork(GameTime gameTime)
        {
            if (runningServer)
            {
                UpdateServer(gameTime);
            }
            else
            {
                if(runningClient)
                    UpdateClient(gameTime);
            }
        }

        public void StartServer(int port, string password)
        {
            runningServer = true;
            EventBasedNetListener listener = new EventBasedNetListener();
            Server = new NetManager(listener);
            Server.Start(port); // starts server with user specified port.
            listener.NetworkReceiveEvent += ReceiveInformation;
            //Event is raised when a client is attempting to connect on port.
            listener.ConnectionRequestEvent += request =>
            {
                if(Server.ConnectedPeersCount < 1)
                    request.AcceptIfKey(password);
                else
                    request.Reject();
            };

            listener.PeerConnectedEvent += peer =>
            {
                DebugConsole.Instance.AddMessage($"Connected to {peer.EndPoint}");
                Peer = peer;
                DebugConsole.Instance.AddMessage("Player has successfully connected!");
                Connected = true;
            };
            listener.PeerDisconnectedEvent += Disconnect;
        }
        
        public void SendInformation(NetDataWriter writer, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            //Make sure we are connected first, then send packet data.
            if(Connected)
                Peer.Send(writer, deliveryMethod);
        }

        private void ReceiveInformation(NetPeer peer, NetPacketReader dataReader, DeliveryMethod deliveryMethod)
        {
            //When we receive a packet, we first pass the packets id so that the correct packet is selected.
            RunPacketFromID(dataReader.GetInt(), dataReader);
            dataReader.Recycle(); // the data is no longer needed, remove it from memory.
        }

        private static NetworkManager _instance;
        public static NetworkManager Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new NetworkManager();
                }

                return result;
            }
        }
        
    }
}