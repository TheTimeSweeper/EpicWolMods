using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace NetWizarding
{
    public class NetWizardingManager_HLAPI_But_Steam : NetWizardingManager_HLAPI
    {
        public static NetWizardingManager_HLAPI_But_Steam instance;

        public override NetworkClient myClient { get => NetWizardingManager_Steam.Instance.myClient; set => NetWizardingManager_Steam.Instance.myClient = value; }

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
            gameObject.AddComponent<NetWizardingManager_Steam>(); //todo should this add that or should that add this?
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
        }
        protected override void InputsNStuff()
        {
            if (Input.GetKeyDown(KeyCode.H)) //todo responsibility of NetWizardingManager_Steam?
            {
                NetWizardingManager_Steam.Instance.InviteFriendOrCreateLobbyAndInvite();
            }
        }

        public void SubscribeToSteamMessages()
        {
            m_P2PSessionRequested = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequested);
        }

        #region host initialization
        //NetWizardingManager_Steam handles initial call to start server

        //todo fuck the backward compatibility and simplify the confusing-ass inheritence
        //or ditch the inheritence with backward compatibility and do a component pattern instead
        protected override void StartUNETServer()
        {
            ConnectionConfig config = new ConnectionConfig();
            config.AddChannel(QosType.ReliableSequenced);
            config.AddChannel(QosType.Unreliable);
            hostTopology = new HostTopology(config, 3);

            NetworkServer.Configure(hostTopology);
            NetworkServer.dontListen = true;
            NetworkServer.Listen(0);
        }
        #endregion host initialization
        protected override void NetWizardingPlugin_OnPlayer1Spawned()
        {
            NetWizardingManager_Steam.Instance.CreateLobby();
            //sends a message to steam to create a lobby
            //in OnLobbyEntered, creates the UNET server
        }

        #region client initialization
        #endregion client initialization

        #region host connection
        void OnP2PSessionRequested(P2PSessionRequest_t pCallback)
        {
            Log.Warning("P2P session request received");

            CSteamID member = pCallback.m_steamIDRemote;

            if (NetworkServer.active && NetWizardingManager_Steam.Instance.IsMemberInSteamLobby(member))
            {
                // Accept the connection if this user is in the lobby
                Log.Info("Steam P2P connection accepted");
                SteamNetworking.AcceptP2PSessionWithUser(member);

                CreateP2PConnectionWithPeer(member);
            }
        }

        public void CreateP2PConnectionWithPeer(CSteamID peer)
        {
            Log.Warning("Sending P2P acceptance message and creating remote client reference for UNET server");
            SteamNetworking.SendP2PPacket(peer, null, 0, EP2PSend.k_EP2PSendReliable);

            // create new connnection for this client and connect them to server
            SteamNetworkConnection newConn = new SteamNetworkConnection(peer);
            newConn.ForceInitialize();

            NetworkServer.AddExternalConnection(newConn);
            connectedClients.Add(newConn);
        }
        #endregion host connection

        #region client connection
        public void ConnectToUnetServerFromSteam(CSteamID hostSteamId)
        {
            Log.Warning("Connecting to UNET server from steam P2P session");

            // Create connection to host player's steam ID
            var conn = new SteamNetworkConnection(hostSteamId);
            var mySteamClient = new SteamNetworkClient(conn);
            myClient = mySteamClient;

            // Setup and connect
            mySteamClient.RegisterHandler(MsgType.Connect, base.OnClientConnectedToHost);
            mySteamClient.SetNetworkConnectionClass<SteamNetworkConnection>();
            mySteamClient.Configure(hostTopology);
            mySteamClient.Connect();
        }
        #endregion client connection

        #region host recieve disconnect from client
        #endregion host recieve disconnect from client

        #region host disconnect client from us
        //called from steam api OnLobbyChatUpdate
        public void HostRemoveClientConnection(CSteamID steamIdToDisconnect)
        {
            var unetClientConnection = GetClient(steamIdToDisconnect);
            var steamClientConnection = unetClientConnection as SteamNetworkConnection;

            if (unetClientConnection != null)
            {
                unetClientConnection.InvokeHandlerNoData(MsgType.Disconnect);

                if (steamClientConnection != null)
                {
                    steamClientConnection.CloseP2PSession();
                }

                connectedClients.Remove(unetClientConnection);

                unetClientConnection.hostId = -1;
                unetClientConnection.Disconnect();
                unetClientConnection.Dispose();
                unetClientConnection = null;
            }
        }
        #endregion host disconnect client from us

        #region client disconnect from host
        //when we leave scene
        //todoo maybe hold on to connection and automatically spawn wizard etc but that's a later later thing
        protected override void ClientSendDisconnectMessage()
        {
            NetWizardingManager_Steam.Instance.ClientDisconnectFromHost();
        }
        #endregion client disconnect from host


        public static NetworkConnection GetClient(CSteamID steamId)
        {
            //todo if id is us... I think that means we fucked up somewhere
            if (steamId.m_SteamID == SteamUser.GetSteamID().m_SteamID)
            {
                Log.Error("Attempting to reach self local client. did we have a connection to self? how? what? who? faster? stronger?");
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
    }
}
