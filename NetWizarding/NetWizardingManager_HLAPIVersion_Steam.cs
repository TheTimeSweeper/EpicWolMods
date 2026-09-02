using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NetWizarding
{
    public class NetWizardingManager_HLAPIVersion_Steam : NetWizardingManager_HLAPIVersion
    {
        public static NetWizardingManager_HLAPIVersion_Steam instance;

        public override NetworkClient myClient { get => NetWizardingManager_SteamVersion.Instance.myClient; set => NetWizardingManager_SteamVersion.Instance.myClient = value; }

        // UNET vars
        private List<NetworkConnection> connectedClients = new List<NetworkConnection>();

        // Steamworks callbacks
        private Callback<P2PSessionRequest_t> m_P2PSessionRequested;

        public HostTopology hostTopology;

        public static int GetChannelCount()
        {
            return instance.hostTopology.DefaultConfig.Channels.Count;
        }

        protected override void Awake()
        {
            instance = this;
            gameObject.AddComponent<NetWizardingManager_SteamVersion>();
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
        }
        protected override void InputsNStuff()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                NetWizardingManager_SteamVersion.Instance.InviteFriendsToLobby();
            }
        }

        public void Init()
        {
            m_P2PSessionRequested = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequested);
        }

        public void RemoveConnection(CSteamID steamId)
        {
            var conn = GetClient(steamId);
            var steamConn = conn as SteamNetworkConnection;

            if (conn != null)
            {
                conn.InvokeHandlerNoData(MsgType.Disconnect);

                if (steamConn != null)
                {
                    steamConn.CloseP2PSession();
                }

                connectedClients.Remove(conn);

                conn.hostId = -1;
                conn.Disconnect();
                conn.Dispose();
                conn = null;
            }

        }

        public static NetworkConnection GetClient(CSteamID steamId)
        {
            if (steamId.m_SteamID == SteamUser.GetSteamID().m_SteamID)
            {
                // get the local client
                if (NetworkServer.active && NetworkServer.connections.Count > 0)
                {
                    return NetworkServer.connections[0];
                }
            }

            // find remote client
            for (int i = 0; i < instance.connectedClients.Count; i++)
            {
                var steamConn = instance.connectedClients[i] as SteamNetworkConnection;
                if (steamConn != null && steamConn.steamId.m_SteamID == steamId.m_SteamID)
                {
                    return steamConn;
                }
            }

            Log.Warning("Client not found\n" + Environment.StackTrace);
            return null;
        }

        void OnP2PSessionRequested(P2PSessionRequest_t pCallback)
        {
            Log.Info("P2P session request received");

            CSteamID member = pCallback.m_steamIDRemote;

            if (NetworkServer.active && NetWizardingManager_SteamVersion.Instance.IsMemberInSteamLobby(member))
            {
                // Accept the connection if this user is in the lobby
                Log.Info("P2P connection accepted");
                SteamNetworking.AcceptP2PSessionWithUser(member);

                CreateP2PConnectionWithPeer(member);
            }
        }

        public void CreateP2PConnectionWithPeer(CSteamID peer)
        {
            Debug.Log("Sending P2P acceptance message and creating remote client reference for UNET server");
            SteamNetworking.SendP2PPacket(peer, null, 0, EP2PSend.k_EP2PSendReliable);

            // create new connnection for this client and connect them to server
            var newConn = new SteamNetworkConnection(peer);
            newConn.ForceInitialize();

            NetworkServer.AddExternalConnection(newConn);
            connectedClients.Add(newConn);
        }

        protected override void StartNetworkServer()
        {
            //SteamMatchmaking.SetLobbyData(NetWizardingManager_SteamVersion.Instance.steamLobbyId, "game", NetWizardingManager_SteamVersion.GAME_ID);

            ConnectionConfig config = new ConnectionConfig();
            config.AddChannel(QosType.ReliableSequenced);
            config.AddChannel(QosType.Unreliable);
            hostTopology = new HostTopology(config, 3);

            NetworkServer.Configure(hostTopology);
            NetworkServer.dontListen = true;
            NetworkServer.Listen(0);

            //todo myClient connected to self is not how I am doing things
            //// Create a local client-to-server connection to the "server"
            //// Connect to localhost to trick UNET's ConnectState state to "Connected", which allows data to pass through TransportSend
            //myClient = ClientScene.ConnectLocalServer();
            //myClient.Configure(hostTopology);
            //myClient.Connect("localhost", 0);
            //myClient.connection.ForceInitialize();
            // Add local client to our list of connections. Here we get the connection from the NetworkServer because it represents the server-to-client connection
            //var serverToClientConn = NetworkServer.connections[0];
            //connectedClients.Add(serverToClientConn);
        }
    }
}
