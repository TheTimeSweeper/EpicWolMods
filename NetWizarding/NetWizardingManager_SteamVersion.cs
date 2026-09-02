using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace NetWizarding
{
    //credit to https://news.clobber.net/gamedev/2017-10-28-unity-unet-hlapi-and-steam-p2p-networking/
    public class NetWizardingManager_SteamVersion : MonoBehaviour
    {
        public const int MAX_USERS = 4;
        public const string GAME_ID = "wizard-of-legend-net-wizarding"; // Unique identifier for matchmaking so we don't match up with other Spacewar games

        public enum SessionConnectionState
        {
            UNDEFINED,
            CONNECTING,
            CANCELLED,
            CONNECTED,
            FAILED,
            DISCONNECTING,
            DISCONNECTED
        }

        public static NetWizardingManager_SteamVersion Instance;

        // Client-to-server connection
        public NetworkClient myClient;

        // steam state vars
        public SessionConnectionState lobbyConnectionState { get; private set; }
        [HideInInspector]
        public CSteamID steamLobbyId;

        // callbacks
        private Callback<LobbyEnter_t> m_LobbyEntered;
        private Callback<GameLobbyJoinRequested_t> m_GameLobbyJoinRequested;
        private Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;
        private CallResult<LobbyMatchList_t> m_LobbyMatchList;

        void Start()
        {
            // init
            Instance = this;

            LogFilter.currentLogLevel = Log.DebugLogs ? LogFilter.Debug : LogFilter.Info;

            if (SteamManager.Initialized)
            {
                m_LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
                m_GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
                m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
                //m_LobbyMatchList = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);

                NetWizardingManager_HLAPIVersion_Steam.instance.Init();
            } else
            {
                Log.Warning("SteamManager not initializd");
            }

            //todo when do
            //JoinGameStartupLobby();
        }

        private void JoinGameStartupLobby()
        {

            // check if game started via friend invitation
            string[] args = System.Environment.GetCommandLineArgs();
            string input = "";
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "+connect_lobby" && args.Length > i + 1)
                {
                    input = args[i + 1];
                }
            }

            if (!string.IsNullOrEmpty(input))
            {
                // Invite accepted, launched game. Join friend's game
                ulong lobbyId = 0;

                if (ulong.TryParse(input, out lobbyId))
                {
                    JoinLobby(new CSteamID(lobbyId));
                }

            }
        }

        void Update()
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            if (!IsConnectedToUNETServer())
            {
                return;
            }

            uint packetSize;
            int channels = NetWizardingManager_HLAPIVersion_Steam.GetChannelCount();

            // Read Steam packets
            for (int chan = 0; chan < channels; chan++)
            {
                while (SteamNetworking.IsP2PPacketAvailable(out packetSize, chan))
                {
                    byte[] data = new byte[packetSize];

                    CSteamID senderId;

                    if (SteamNetworking.ReadP2PPacket(data, packetSize, out packetSize, out senderId, chan))
                    {
                        NetworkConnection connection;

                        // We are the server, one of our clients will handle this packet
                        connection = NetWizardingManager_HLAPIVersion_Steam.GetClient(senderId);

                        if (connection == null)
                        {
                            // In some cases the p2p connection can persist, resulting in UNETServerController.OnP2PSessionRequested not being called. This happens usually when testing in editor.
                            // If the peers have already established a connection, reset it.
                            P2PSessionState_t sessionState;
                            if (SteamNetworking.GetP2PSessionState(senderId, out sessionState) && Convert.ToBoolean(sessionState.m_bConnectionActive))
                            {
                                Log.Message("P2P connection is still established. Resetting.");
                                SteamNetworking.CloseP2PSessionWithUser(senderId);
                                NetWizardingManager_HLAPIVersion_Steam.instance.CreateP2PConnectionWithPeer(senderId);
                                connection = NetWizardingManager_HLAPIVersion_Steam.GetClient(senderId);
                            }
                        }

                        if (connection != null)
                        {
                            // Handle Steam packet through UNET
                            connection.TransportReceive(data, Convert.ToInt32(packetSize), chan);
                        }

                    }
                }
            }

            //todo disconnect
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    Disconnect();
            //}
        }

        public bool IsMemberInSteamLobby(CSteamID steamUser)
        {
            if (SteamManager.Initialized)
            {
                int numMembers = SteamMatchmaking.GetNumLobbyMembers(steamLobbyId);

                for (int i = 0; i < numMembers; i++)
                {
                    var member = SteamMatchmaking.GetLobbyMemberByIndex(steamLobbyId, i);

                    if (member.m_SteamID == steamUser.m_SteamID)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //todo not used?
        //public CSteamID GetSteamIDForConnection(NetworkConnection conn)
        //{
        //    if (UNETServerController.IsHostingServer())
        //    {
        //        return UNETServerController.GetSteamIDForConnection(conn);
        //    }
        //    else
        //    {
        //        // clients only have the client-to-server connection
        //        var steamConn = myClient as SteamNetworkClient;
        //        if (steamConn != null)
        //        {
        //            return steamConn.steamConnection.steamId;
        //        }
        //    }

        //    Debug.LogError("Could not find Steam ID");
        //    return CSteamID.Nil;
        //}

        public bool IsConnectedToUNETServer()
        {
            return myClient != null && myClient.connection != null && myClient.connection.isConnected;
        }

        //todo disconnect
        //public void Disconnect()
        //{
        //    lobbyConnectionState = SessionConnectionState.DISCONNECTED;

        //    if (SteamManager.Initialized)
        //    {
        //        SteamMatchmaking.LeaveLobby(steamLobbyId);
        //    }

        //    if (myClient != null)
        //    {
        //        myClient.Disconnect();
        //        myClient = null;
        //    }

        //    UNETServerController.Disconnect();
        //    //NetworkClient.ShutdownAll();//todo uh

        //    steamLobbyId.Clear();
        //}


        void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback)
        {
            if (pCallback.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeLeft && pCallback.m_ulSteamIDLobby == steamLobbyId.m_SteamID)
            {
                Log.Message("A client has disconnected from the UNET server");

                // user left lobby
                var userId = new CSteamID(pCallback.m_ulSteamIDUserChanged);
                if (NetworkServer.active)
                {
                    NetWizardingManager_HLAPIVersion_Steam.instance.RemoveConnection(userId);
                }

                SteamNetworking.CloseP2PSessionWithUser(userId);
            }
        }


        void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t pCallback)
        {
            // Invite accepted, game is already running
            JoinLobby(pCallback.m_steamIDLobby);
        }

        public void JoinLobby(CSteamID lobbyId)
        {
            if (!SteamManager.Initialized)
            {
                lobbyConnectionState = SessionConnectionState.FAILED;
                return;
            }

            lobbyConnectionState = SessionConnectionState.CONNECTING;
            SteamMatchmaking.JoinLobby(lobbyId);
            // ...continued in OnLobbyEntered callback
        }

        //public void FindMatch()
        //{
        //    if (!SteamManager.Initialized)
        //    {
        //        lobbyConnectionState = SessionConnectionState.FAILED;
        //        return;
        //    }

        //    lobbyConnectionState = SessionConnectionState.CONNECTING;

        //    //Note: call SteamMatchmaking.AddRequestLobbyList* before RequestLobbyList to filter results by some criteria
        //    SteamMatchmaking.AddRequestLobbyListStringFilter("game", GAME_ID, ELobbyComparison.k_ELobbyComparisonEqual);
        //    var call = SteamMatchmaking.RequestLobbyList();
        //    m_LobbyMatchList.Set(call, OnLobbyMatchList);
        //}

        //void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure)
        //{
        //    uint numLobbies = pCallback.m_nLobbiesMatching;

        //    if (numLobbies <= 0)
        //    {
        //        // no lobbies found. create one
        //        Debug.Log("Creating lobby");

        //        UNETServerController.inviteFriendOnStart = false;
        //        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, MAX_USERS);
        //        // ...continued in OnLobbyEntered callback
        //    }
        //    else
        //    {
        //        // If multiple lobbies are returned we can iterate over them with SteamMatchmaking.GetLobbyByIndex and choose the "best" one
        //        // In this case we are just joining the first one
        //        Debug.Log("Joining lobby");
        //        var lobby = SteamMatchmaking.GetLobbyByIndex(0);
        //        JoinLobby(lobby);
        //    }


        //}


        public void InviteFriendsToLobby()
        {
            if (lobbyConnectionState == SessionConnectionState.CONNECTING)
            {
                // Already trying to connect...
                Log.Warning("trying to invite friends but Already trying to connect");
                return;
            }

            if (lobbyConnectionState != SessionConnectionState.CONNECTED)
            {
                // No lobby yet
                CreateLobbyAndInviteFriend();
            }
            else
            {
                // Already in lobby. Invite friends to current lobby
                InviteFriendsToLobbye();
            }
        }

        public void InviteFriendsToLobbye()
        {
            Debug.Log("Showing invite friend dialog");
            SteamFriends.ActivateGameOverlayInviteDialog(steamLobbyId);
        }

        public void CreateLobbyAndInviteFriend()
        {
            if (!SteamManager.Initialized)
            {
                lobbyConnectionState = SessionConnectionState.FAILED;
                Log.Warning("failed not initialized");
                return;
            }

            //UNETServerController.inviteFriendOnStart = true;
            lobbyConnectionState = SessionConnectionState.CONNECTING;
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, MAX_USERS);
            // ...continued in OnLobbyEntered callback
        }

        void OnLobbyEntered(LobbyEnter_t pCallback)
        {
            if (!SteamManager.Initialized)
            {
                lobbyConnectionState = SessionConnectionState.FAILED;
                return;
            }

            steamLobbyId = new CSteamID(pCallback.m_ulSteamIDLobby);

            Log.Warning("Connected to Steam lobby");
            lobbyConnectionState = SessionConnectionState.CONNECTED;

            var hostUserId = SteamMatchmaking.GetLobbyOwner(steamLobbyId);
            var me = SteamUser.GetSteamID();
            //todo we're not connecting to self. need to start unetserver ourselves
            if (hostUserId.m_SteamID == me.m_SteamID)
            {
                SteamMatchmaking.SetLobbyData(steamLobbyId, "game", GAME_ID);
                Log.Warning("Lobby Created on host");
                NetWizardingManager_HLAPIVersion_Steam.instance.SetupServer();
                //UNETServerController.StartUNETServer();
            }
            else
            {
                // joined friend's lobby.
                Log.Warning("Sending request to p2p connect with host");
                StartCoroutine(RequestP2PConnectionWithHost());
            }


        }

        IEnumerator RequestP2PConnectionWithHost()
        {
            var hostUserId = SteamMatchmaking.GetLobbyOwner(steamLobbyId);

            //send packet to request connection to host via Steam's NAT punch or relay servers
            Log.Warning("Sending packet to request P2P connection");
            SteamNetworking.SendP2PPacket(hostUserId, null, 0, EP2PSend.k_EP2PSendReliable);

            Log.Warning("Waiting for P2P acceptance message");
            uint packetSize;
            while (!SteamNetworking.IsP2PPacketAvailable(out packetSize))
            {
                yield return null;
            }

            byte[] data = new byte[packetSize];

            CSteamID senderId;

            if (SteamNetworking.ReadP2PPacket(data, packetSize, out packetSize, out senderId))
            {
                if (senderId.m_SteamID == hostUserId.m_SteamID)
                {
                    Debug.Log("P2P connection established");

                    // packet was from host, assume it's notifying client that AcceptP2PSessionWithUser was called
                    P2PSessionState_t sessionState;
                    if (SteamNetworking.GetP2PSessionState(hostUserId, out sessionState))
                    {
                        // connect to the unet server
                        ConnectToUnetServerForSteam(hostUserId);

                        yield break;
                    }

                }
            }

            Debug.LogError("Connection failed");
        }


        void ConnectToUnetServerForSteam(CSteamID hostSteamId)
        {
            Debug.Log("Connecting to UNET server");

            // Create connection to host player's steam ID
            var conn = new SteamNetworkConnection(hostSteamId);
            var mySteamClient = new SteamNetworkClient(conn);
            myClient = mySteamClient;

            // Setup and connect
            mySteamClient.RegisterHandler(MsgType.Connect, OnConnect);
            mySteamClient.SetNetworkConnectionClass<SteamNetworkConnection>();
            mySteamClient.Configure(NetWizardingManager_HLAPIVersion_Steam.instance.hostTopology);
            mySteamClient.Connect();

        }

        void OnConnect(NetworkMessage msg)
        {
            // Set to ready and spawn player
            Log.Message("Connected to UNET server.");
            myClient.UnregisterHandler(MsgType.Connect);//todo uh

            //var conn = myClient.connection;
            //if (conn != null)
            //{
            //    ClientScene.Ready(conn);
            //    Debug.Log("Requesting spawn");
            //    myClient.Send(NetworkMessages.SpawnRequestMsg, new StringMessage(SteamUser.GetSteamID().m_SteamID.ToString()));
            //}

        }
    }

    //credit to https://news.clobber.net/gamedev/2017-10-28-unity-unet-hlapi-and-steam-p2p-networking/
    public class SteamNetworkConnection : NetworkConnection
    {
        public CSteamID steamId;

        public SteamNetworkConnection() : base()
        {
        }

        public SteamNetworkConnection(CSteamID steamId)
        {
            this.steamId = steamId;
        }

        public override bool TransportSend(byte[] bytes, int numBytes, int channelId, out byte error)
        {
            if (steamId.m_SteamID == SteamUser.GetSteamID().m_SteamID)
            {
                // sending to self. short circuit
                TransportReceive(bytes, numBytes, channelId);
                error = 0;
                return true;
            }

            EP2PSend eP2PSendType = EP2PSend.k_EP2PSendReliable;

            QosType qos = /*SteamNetworkManager*/NetworkServer.hostTopology.DefaultConfig.Channels[channelId].QOS;
            if (qos == QosType.Unreliable || qos == QosType.UnreliableFragmented || qos == QosType.UnreliableSequenced)
            {
                eP2PSendType = EP2PSend.k_EP2PSendUnreliable;
            }

            // Send packet to peer through Steam
            if (SteamNetworking.SendP2PPacket(steamId, bytes, (uint)numBytes, eP2PSendType))
            {
                error = 0;
                return true;
            }
            else
            {
                error = 1;
                return false;
            }
        }

        public void CloseP2PSession()
        {
            SteamNetworking.CloseP2PSessionWithUser(steamId);
            steamId = CSteamID.Nil;
        }
    }

    public class SteamNetworkClient : NetworkClient
    {

        public SteamNetworkConnection steamConnection
        {
            get
            {
                return connection as SteamNetworkConnection;
            }

        }

        public string status { get { return m_AsyncConnect.ToString(); } }

        public void Connect()
        {
            // Connect to localhost and trick UNET by setting ConnectState state to "Connected", which triggers some initialization and allows data to pass through TransportSend
            Connect("localhost", 0);
            m_AsyncConnect = ConnectState.Connected;

            // manually init connection
            connection.ForceInitialize();

            // send Connected message
            connection.InvokeHandlerNoData(MsgType.Connect);
        }

        public SteamNetworkClient(NetworkConnection conn) : base(conn)
        {
        }

        public override void Disconnect()
        {
            m_AsyncConnect = ConnectState.Disconnected;

            if (m_Connection != null & m_Connection.isConnected)
            {
                m_Connection.InvokeHandlerNoData(MsgType.Disconnect);

                steamConnection.CloseP2PSession();
                m_Connection.hostId = -1;
                m_Connection.Disconnect();
                m_Connection.Dispose();
                m_Connection = null;

            }

        }
    }

    public static class UNETExtensions
    {

        private static int nextConnectionId = -1;

        /// Because we fake the UNET connection, connection initialization is not handled by UNET internally. 
        /// Connections must be manually initialized with this function.
        public static void ForceInitialize(this NetworkConnection conn)
        {
            int id = ++nextConnectionId;
            conn.Initialize("localhost", id, id, NetWizardingManager_HLAPIVersion_Steam.instance.hostTopology);
        }
    }
}
