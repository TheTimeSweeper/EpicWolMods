using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace NetWizarding
{
    public class WizardPositionMessage : MessageBase
    {
        public Vector3 decodedPosition;
        public Vector2 moveInput;
        public Vector2 lookInput;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((Vector3)GameController.playerScripts[0].transform.position);
            writer.Write((Vector2)GameController.playerScripts[0].inputDevice.GetMoveVector());
            writer.Write((Vector2)GameController.playerScripts[0].inputDevice.GetAimVector());

        }
        public override void Deserialize(NetworkReader reader)
        {
            decodedPosition = reader.ReadVector3();
            moveInput = reader.ReadVector2();
            lookInput = reader.ReadVector2();
        }
    }

    public class WizardPVPDamageMessage : MessageBase
    {
        public AttackInfo attackInfo;

        #region reference

        ////public Entity entity;//decode for now
        ////public GameObject gameObject;//decode for now
        /////hopefully don't need
        ////public string skillCategory;
        ////public string skillID;
        ////public int skillLevel;
        ////public bool isUltimate;
        ////public string attackInfoKey;
        /////decode for now
        ////public int atkObjID;
        ////public string attacker;

        //public Vector2 inAttackerPosition;
        //public Vector2 inTargetPosition;
        //public ElementType inElementType;
        //public ElementType inSubElementType;
        //public bool inShowDamageNumber;
        //public bool inShowDamageEffect;
        //public bool inShakeCamera;
        ////targetobjid - using hard-coding p1 vs p2 values
        ////targetnames - doesn't seem to be referenced in health take damage step
        //public float inTime;
        //public float inSameTargetImmunityTime;
        //public float inSameAttackImmunityTime;
        ////public bool inCanHitStun;//oh god
        ////hitstunduration
        //public float inKnockbackMultiplier;
        //public bool inKnockbackOverwrite;
        //public Vector2 inKnockbackVector;
        //public Vector2 attackingVector;
        //public int damage;
        //public bool isCritical;
        //public bool isDamageOverride;
        ////public float critHitChance;
        //public float critDmgModifier;
        ////these will have to be networked separately
        ////public bool isStatusEffect;
        ////public float rootChance;
        ////public float rootDuration;
        ////public float chaosChance;
        ////public int chaosLevel;
        ////public float burnChance;
        ////public int burnLevel;
        ////public float slowChance;
        ////public int slowLevel;
        ////public float poisonChance;
        ////public int poisonLevel;
        ////public float shockChance;
        ////public int shockLevel;
        ////public float freezeChance;
        ////public float freezeDuration;

        ////not read in takedamage step
        ////public float odDmgModifier;
        ////public float odProgMultiplier;
        ////public bool odSingleIncrease;
        #endregion reference

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((Vector2)attackInfo.attackerPosition);
            writer.Write((Vector2)attackInfo.targetPosition);
            writer.Write((int)attackInfo.elementType);
            writer.Write((int)attackInfo.subElementType);
            writer.Write((bool)attackInfo.showDamageNumber);
            writer.Write((bool)attackInfo.showDamageEffect);
            writer.Write((bool)attackInfo.shakeCamera);
            writer.Write((float)attackInfo.time);
            writer.Write((float)attackInfo.sameTargetImmunityTime);
            writer.Write((float)attackInfo.sameAttackImmunityTime);
            writer.Write((float)attackInfo.knockbackMultiplier);
            writer.Write((bool)attackInfo.knockbackOverwrite);
            writer.Write((Vector2)attackInfo.knockbackVector);
            writer.Write((Vector2)attackInfo.attackingVector);
            writer.Write((int)attackInfo.damage);
            writer.Write((bool)attackInfo.isCritical);
            writer.Write((bool)attackInfo.isDamageOverride);
            writer.Write((float)attackInfo.critDmgModifier);
        }
        public override void Deserialize(NetworkReader reader)
        {
            attackInfo = new AttackInfo()
            {
                attackerPosition = reader.ReadVector2(),
                targetPosition = reader.ReadVector2(),
                elementType = (ElementType)reader.ReadInt32(),
                subElementType = (ElementType)reader.ReadInt32(),
                showDamageNumber = reader.ReadBoolean(),

                showDamageEffect = reader.ReadBoolean(),
                shakeCamera = reader.ReadBoolean(),
                time = reader.ReadSingle(),
                sameTargetImmunityTime = reader.ReadSingle(),
                sameAttackImmunityTime = reader.ReadSingle(),
                knockbackMultiplier = reader.ReadSingle(),
                knockbackOverwrite = reader.ReadBoolean(),
                knockbackVector = reader.ReadVector2(),
                attackingVector = reader.ReadVector2(),
                damage = reader.ReadInt32(),
                isCritical = reader.ReadBoolean(),
                isDamageOverride = reader.ReadBoolean(),
                critDmgModifier = reader.ReadSingle(),
            };
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
            NetWizardingPlugin.OnPlayer1Spawned += NetWizardingPlugin_OnPlayer1Spawned;
            NetWizardingPlugin.OnSceneExited += NetWizardingPlugin_OnSceneExited;

            NetWizardingPlugin.OnPlayer1StateChanged += NetWizardingPlugin_OnPlayer1StateChanged;
            NetWizardingPlugin.OnPlayer2TakeDamage += NetWizardingPlugin_OnPlayer2TakeDamage;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                SetupServer();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Util.ToggleIPInput();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (Util.inputUI)
                {
                    IpToConnect = Util.ConsumeIPInput();
                    if (string.IsNullOrEmpty(IpToConnect)) IpToConnect = "127.0.0.1";
                    SetupClient(IpToConnect);
                }
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                SendDisconnectMessage();
            }

            msgTimer -= Time.deltaTime;
            if (msgTimer <= 0)
            {
                msgTimer = 0.03f;
                SendPositionMessage();
            }
        }

        private void NetWizardingPlugin_OnSceneExited()
        {
            SendDisconnectMessage();
        }

        private void NetWizardingPlugin_OnPlayer1Spawned()
        {
            SetupServer();
        }

        private void SendDisconnectMessage()
        {
            if (!IsClientReady())
                return;

            myClient.Send(MsgType.Disconnect, new EmptyMessage());
            myClient = null;
        }

        private bool IsClientReady()
        {
            return myClient != null && myClient.connection != null && myClient.connection.isConnected;
        }

        private void SendPositionMessage()
        {
            if (!IsClientReady())
                return;

            myClient.Send((short)NetWizMessageType.position, new WizardPositionMessage());
        }

        private void NetWizardingPlugin_OnPlayer1StateChanged(int stateIndex)
        {
            if (!IsClientReady())
                return;
            
            Log.Warning($"sending state {stateIndex}");
            myClient.Send((short)NetWizMessageType.fsm_state, new IntegerMessage(stateIndex));
        }

        private void NetWizardingPlugin_OnPlayer2TakeDamage(AttackInfo givenAtkInfo, Entity attackEntity)
        {
            if (!IsClientReady())
                return;

            WizardPVPDamageMessage damageMessage = new WizardPVPDamageMessage()
            {
                attackInfo = givenAtkInfo
            };

            myClient.Send((short)NetWizMessageType.damage, damageMessage);
        }

        // Create a server and listen on a port
        public void SetupServer()
        {
            if (NetworkServer.active)
            {
                Log.Warning("Host already active");
                if(NetworkServer.connections.Count == 0)
                {
                    GameUI.BroadcastNoticeMessage("Created Host Server. Awaiting client connection (press C to connect).");
                }
                return;
            }
            NetworkServer.Listen(NetWizardingPlugin.config_hostPort);
            NetworkServer.RegisterHandler(MsgType.Connect, OnHostReceivedConnection);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnHostReceivedDisconnect);
            NetworkServer.RegisterHandler((short)NetWizMessageType.position, OnHostReceivedPosition);
            NetworkServer.RegisterHandler((short)NetWizMessageType.fsm_state, OnHostReceivedState);
            NetworkServer.RegisterHandler((short)NetWizMessageType.damage, OnHostReceivedPVPDamage);
            Log.Message($"started server at port {NetworkServer.listenPort}");
            GameUI.BroadcastNoticeMessage("Created Host Server. Awaiting client connection.");
        }

        private void OnHostReceivedPVPDamage(NetworkMessage netMsg)
        {
            WizardPVPDamageMessage damageMessage = netMsg.ReadMessage<WizardPVPDamageMessage>();
            AttackInfo decodedInfo = damageMessage.attackInfo;
            NetWizardingPlugin.Instance.ReceivePVPAttackInfo(decodedInfo);
        }

        private void OnHostReceivedDisconnect(NetworkMessage netMsg)
        {
            Log.Message("Client Disconnected from us. Disconnecting from our host as well.");
            GameUI.BroadcastNoticeMessage("Client disconnected from us. Disconnecting from our host as well.");
            SendDisconnectMessage();

            NetWizardingPlugin.Instance.ReceiveDisconnect();
        }

        // Create a client and connect to the server port
        public void SetupClient(string Ip)
        {
            if (IsClientReady())
            {
                Log.Message("trying to connect when already established");
                return;
            }
            myClient = new NetworkClient();
            myClient.RegisterHandler(MsgType.Connect, OnClientConnectedToHost);
            myClient.Connect(Ip, NetWizardingPlugin.config_clientPort);
        }

        private void OnHostReceivedConnection(NetworkMessage netMsg)
        {
            Log.Message("Client Connected to us");
            string message = "Client connection received.";
            if (!IsClientReady())
            {
                message += "\nConnect back to establish pair (press C)";
            }
            GameUI.BroadcastNoticeMessage("Client connection received.");

            NetWizardingPlugin.Instance.Player2ClaimFakeInput();
        }

        // client function
        public void OnClientConnectedToHost(NetworkMessage netMsg)
        {
            Log.Message("Connected to server");
            GameUI.BroadcastNoticeMessage("Connected to host.");
        }

        private void OnHostReceivedPosition(NetworkMessage netMsg)
        {
            WizardPositionMessage wizardPositionMessage = netMsg.ReadMessage<WizardPositionMessage>();
            Vector3 decodedPosition = wizardPositionMessage.decodedPosition;
            GameController.playerScripts[1].transform.position = decodedPosition;
            NetWizardingPlugin.Instance.networkPlayer2MoveInput = wizardPositionMessage.moveInput;
            NetWizardingPlugin.Instance.networkPlayer2LookInput = wizardPositionMessage.lookInput;
            previousNetworkPosition = decodedPosition;
        }

        private void OnHostReceivedState(NetworkMessage netMsg)
        {
            var stateIndex = netMsg.ReadMessage<IntegerMessage>().value;
            Log.Warning($"received state {stateIndex}");
            NetWizardingPlugin.Instance.RecieveState(stateIndex);
        }
    }
}