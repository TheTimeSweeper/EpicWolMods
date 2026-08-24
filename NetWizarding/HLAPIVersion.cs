using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace NetWizarding
{
    public class WizardPositionMessage : MessageBase
    {
        public Vector3 decodedPosition;
        public Vector2 input;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((Vector3)GameController.playerScripts[0].transform.position);
            writer.Write((Vector2)GameController.playerScripts[0].inputDevice.GetMoveVector());

        }
        public override void Deserialize(NetworkReader reader)
        {
            decodedPosition = reader.ReadVector3();
            input = reader.ReadVector2();
        }
    }

    public class HLAPIVersion : MonoBehaviour
    {
        private string IpToConnect;
        private NetworkClient myClient;

        private float msgTimer;

        private Vector3 previousNetworkPosition;


        public enum NetWizMessageType : short
        {
            position = 100,
            fsm_state = 200,
            damage = 300,
        }

        void Awake()
        {
            NetWizardingPlugin.OnPlayer1StateChanged += NetWizardingPlugin_OnPlayer1StateChanged;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                SetupServer();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (!Util.inputUI)
                {
                    Util.ToggleIPInput();
                }
                else
                {
                    IpToConnect = Util.ConsumeIPInput();
                    if (string.IsNullOrEmpty(IpToConnect)) IpToConnect = "127.0.0.1";
                    SetupClient(IpToConnect);
                }
            }

            msgTimer -= Time.deltaTime;
            if (msgTimer <= 0 && IsClientReady())
            {
                msgTimer = 0.03f;
                myClient.Send((short)NetWizMessageType.position, new WizardPositionMessage());
            }
        }

        private bool IsClientReady()
        {
            return myClient != null && myClient.connection != null && myClient.connection.isConnected;
        }

        private void NetWizardingPlugin_OnPlayer1StateChanged(int stateIndex)
        {
            if (IsClientReady())
            {
                Log.Warning($"sending state {stateIndex}");
                myClient.Send((short)NetWizMessageType.fsm_state, new IntegerMessage(stateIndex));
            }
        }

        // Create a server and listen on a port
        public void SetupServer()
        {
            NetworkServer.Listen(NetWizardingPlugin.config_hostPort);
            NetworkServer.RegisterHandler(MsgType.Connect, OnHostReceivedConnection);
            NetworkServer.RegisterHandler((short)NetWizMessageType.position, OnHostReceivedPosition);
            NetworkServer.RegisterHandler((short)NetWizMessageType.fsm_state, OnHostReceivedState);
            Log.Warning($"starter server at port {NetworkServer.listenPort}");
        }

        // Create a client and connect to the server port
        public void SetupClient(string Ip)
        {
            myClient = new NetworkClient();
            myClient.RegisterHandler(MsgType.Connect, OnClientConnectedToHost);
            myClient.Connect(Ip, NetWizardingPlugin.config_clientPort);
        }

        private void OnHostReceivedConnection(NetworkMessage netMsg)
        {
            Log.Warning("Client Connected to us");
            NetWizardingPlugin.Instance.Player2ClaimFakeInput();
        }

        // client function
        public void OnClientConnectedToHost(NetworkMessage netMsg)
        {
            Log.Warning("Connected to server");
        }

        private void OnHostReceivedPosition(NetworkMessage netMsg)
        {
            WizardPositionMessage wizardPositionMessage = netMsg.ReadMessage<WizardPositionMessage>();
            Vector3 decodedPosition = wizardPositionMessage.decodedPosition;
            GameController.playerScripts[1].transform.position = decodedPosition;
            NetWizardingPlugin.Instance.positionDifference = wizardPositionMessage.input;
            previousNetworkPosition = decodedPosition;
        }

        private void OnHostReceivedState(NetworkMessage netMsg)
        {
            var stateIndex = netMsg.ReadMessage<IntegerMessage>().value;
            Log.Message($"received state {stateIndex}");
            NetWizardingPlugin.Instance.RecieveState(stateIndex);
        }
    }
}