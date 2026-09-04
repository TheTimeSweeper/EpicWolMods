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
    public class NetWizardingManager_Steam : MonoBehaviour
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

        public static NetWizardingManager_Steam Instance;

        // Client-to-server connection
        public NetworkClient hostClient;
        public NetworkClient clientClient;

        // steam state vars
        public SessionConnectionState lobbyConnectionState { get; private set; }
        //[HideInInspector]
        public CSteamID myHostSteamLobbyId;
        public CSteamID otherHostSteamLobbyId;

        // callbacks
        private Callback<LobbyEnter_t> m_LobbyEntered;
        private Callback<GameLobbyJoinRequested_t> m_GameLobbyJoinRequested;
        private Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;
        private CallResult<LobbyMatchList_t> m_LobbyMatchList;

        private bool openInviteOnLobbyCreated;

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

                m_P2PSessionRequested = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequested);
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

            if(!NetworkServer.active && !hostClient.IsReady())
            {
                return;
            }

            //originally checking 'myclient' which was the self client. do we need another way to check if the host server is active?
            //if (!NetWizardingManager_HLAPI_But_Steam.instance.IsClientReady())//todo bouncing too much
            //{
            //    return;
            //}

            uint packetSize;
            int channels = NetWizardingManager_HLAPI_But_Steam.GetChannelCount();

            // Read Steam packets
            for (int chan = 0; chan < channels; chan++)
            {
                while (SteamNetworking.IsP2PPacketAvailable(out packetSize, chan))
                {
                    byte[] data = new byte[packetSize];

                    CSteamID senderId;

                    if (SteamNetworking.ReadP2PPacket(data, packetSize, out packetSize, out senderId, chan))
                    {
                        Log.Message("received p2p packet");

                        NetworkConnection connection = NetWizardingManager_HLAPI_But_Steam.GetSteamNetworkClient(senderId);
                        
                        if (connection == null)
                        {
                            Log.Message("defaulting to host client");
                            connection = hostClient.connection;

                        }
                        if (connection != null)
                        {
                            Log.Message($"receiving packet on connection {NetWizardingManager_HLAPI_But_Steam.Debug_GetConnectionIndex(connection)}");
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

        void OnP2PSessionRequested(P2PSessionRequest_t pCallback)
        {
            Log.Warning("P2P session request received");

            CSteamID member = pCallback.m_steamIDRemote;

            if (NetworkServer.active && NetWizardingManager_Steam.Instance.IsMemberInSteamLobby(member))
            {
                // Accept the connection if this user is in the lobby
                Log.Info("Steam P2P connection accepted");
                SteamNetworking.AcceptP2PSessionWithUser(member);

                NetWizardingManager_HLAPI_But_Steam.instance.CreateP2PConnectionWithPeer(member);
            }
        }

        public bool IsMemberInSteamLobby(CSteamID steamUser)
        {
            if (SteamManager.Initialized)
            {
                int numMembers = SteamMatchmaking.GetNumLobbyMembers(myHostSteamLobbyId);

                for (int i = 0; i < numMembers; i++)
                {
                    var member = SteamMatchmaking.GetLobbyMemberByIndex(myHostSteamLobbyId, i);

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

        //let's just disconnect our client for now.
        public void ClientDisconnectFromHost()
        {
            Log.Message("ClientDisconnectFromHost");
            if (!clientClient.IsReady())
            {
                Log.Message("but we were already disconnected");
                return;
            }
            lobbyConnectionState = SessionConnectionState.DISCONNECTED;//todo dont quite understand but seems fine

            //todo is this for the local client? if so not needed. will test without
            //if not it was probably to close the lobby
            //if (SteamManager.Initialized)
            //{
            //    SteamMatchmaking.LeaveLobby(steamLobbyId);
            //}

            SteamMatchmaking.LeaveLobby(otherHostSteamLobbyId);
            otherHostSteamLobbyId.Clear();

            //this was definitely local client stuff but we can use it
            if (clientClient != null)
            {
                (clientClient as SteamNetworkClient).Disconnect();
                clientClient = null;
            }

            //UNETServerController.Disconnect();// i don't think I ever shut down my server so idk

        }

        void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback)
        {
            if (pCallback.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeLeft && pCallback.m_ulSteamIDLobby == myHostSteamLobbyId.m_SteamID)
            {
                Log.Message("A client has disconnected from the UNET server");

                // user left lobby
                var userIdToDisconnect = new CSteamID(pCallback.m_ulSteamIDUserChanged);
                if (NetworkServer.active)
                {
                    NetWizardingManager_HLAPI_But_Steam.instance.HostRemoveClientConnection(userIdToDisconnect);
                }

                SteamNetworking.CloseP2PSessionWithUser(userIdToDisconnect);
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


        public void InviteFriendOrCreateLobbyAndInvite()
        {
            if (lobbyConnectionState == SessionConnectionState.CONNECTING)
            {
                // Already trying to connect...
                Log.Warning("trying to invite friends but Already trying to connect");
                return;
            }

            if (lobbyConnectionState == SessionConnectionState.CONNECTED)
            {
                // Already in lobby. show dialog to Invite friends to current lobby
                InviteFriendsToLobby();
            }
            else
            {
                openInviteOnLobbyCreated = true;
                // No lobby yet. create one, and show dialog after.
                CreateLobby();
            }
        }

        public void InviteFriendsToLobby()
        {
            Log.Warning("Showing invite friend dialog");
            SteamFriends.ActivateGameOverlayInviteDialog(myHostSteamLobbyId);
        }
        public void CreateLobby()
        {
            if (!SteamManager.Initialized)
            {
                lobbyConnectionState = SessionConnectionState.FAILED;
                Log.Warning("Failed. SteamManager not initialized");
                return;
            }

            lobbyConnectionState = SessionConnectionState.CONNECTING;
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, MAX_USERS);
            // ...continued in OnLobbyEntered callback
        }

        //callback when joining someone's lobby, but also when requesting a lobby for self
        void OnLobbyEntered(LobbyEnter_t pCallback)
        {
            if (!SteamManager.Initialized)
            {
                lobbyConnectionState = SessionConnectionState.FAILED;
                return;
            }

            var receivedSteamLobbyId = new CSteamID(pCallback.m_ulSteamIDLobby);

            Log.Warning("OnLobbyEntered Callback");
            lobbyConnectionState = SessionConnectionState.CONNECTED;

            var hostUserId = SteamMatchmaking.GetLobbyOwner(receivedSteamLobbyId);
            var me = SteamUser.GetSteamID();
            if (hostUserId.m_SteamID == me.m_SteamID)
            {
                myHostSteamLobbyId = receivedSteamLobbyId;
                SteamMatchmaking.SetLobbyData(receivedSteamLobbyId, "game", GAME_ID);
                Log.Message("Lobby Created on host");
                NetWizardingManager_HLAPI_But_Steam.instance.HLAPI_StartServer();
                if (openInviteOnLobbyCreated)
                {
                    InviteFriendsToLobby();
                }
            }
            else
            {
                otherHostSteamLobbyId = receivedSteamLobbyId;
                // joined friend's lobby.
                Log.Message("Sending request to p2p connect with host");
                StartCoroutine(RequestP2PConnectionWithHost());
            }


        }

        IEnumerator RequestP2PConnectionWithHost()
        {
            var hostUserId = SteamMatchmaking.GetLobbyOwner(otherHostSteamLobbyId);

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
                    Log.Message("P2P connection established");

                    // packet was from host, assume it's notifying client that AcceptP2PSessionWithUser was called
                    P2PSessionState_t sessionState;
                    if (SteamNetworking.GetP2PSessionState(hostUserId, out sessionState))
                    {
                        // connect to the unet server
                        NetWizardingManager_HLAPI_But_Steam.instance.ConnectToUnetServerFromSteam(hostUserId);

                        yield break;
                    }

                }
            }

            Log.Message("Connection failed");
        }
    }
}
