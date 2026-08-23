using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Rewired;
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Networking
{
    [BepInPlugin("TheTimesweeper.NetWizarding", "NetWizarding", "0.1.0")]
    public class NetWizardingPlugin : BaseUnityPlugin
    {
        private int config_hostPort;
        private int config_clientPort;
        private int myReiliableChannelId;
        private int myUnreliableChannelId;
        private int thisHostId;
        private HostTopology hostTopology;
        private int hostSelfConnectionId = -1;

        private int clientConnectionId = -1;

        // Token: 0x04004B67 RID: 19303
        private GameObject inputUI;

        // Token: 0x04004B68 RID: 19304
        private InputField ipInputField;

        public static BepInEx.Logging.ManualLogSource Log;

        public static NetWizardingPlugin Instance;

        private Rewired.Data.UserData userDataButEpic;
        private CustomController netWizardFakeController;
        
        public enum NetWizMessageType : byte
        {
            position,
            fsm_state,
            damage
        }

        void Awake()
        {
            Instance = this; 
            Harmony.CreateAndPatchAll(typeof(NetWizardingPlugin));
             Log = Logger;
            Debug.Log("I belive in you c:");

            LoadFromBundle(); 

            On.Health.TakeDamage += Health_TakeDamage;
            On.Rewired.InputManager_Base.Initialize += InputManager_Base_Initialize;
            //On.Attack.CheckCollision += Attack_CheckCollision;
            config_hostPort = base.Config.Bind("uh", "hostport", 6969).Value;
            config_clientPort = base.Config.Bind("uh", "clientport", 9696).Value;
            BeginHost();
        }

        [HarmonyPatch(typeof(ChaosInputDevice), "controllerCount", MethodType.Getter)]
        [HarmonyPostfix]
        static void Postfix(ref int __result, ChaosInputDevice __instance)
        {
            __result += __instance.rewiredPlayer.controllers.customControllerCount;
        }

        private void LoadFromBundle()
        {
            var bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), Path.Combine("Assets", "netwizarding")));
            userDataButEpic = bundle.LoadAsset<GameObject>("RewiredInputButEpic").GetComponent<InputManager>().userData;
        }

        private void InputManager_Base_Initialize(On.Rewired.InputManager_Base.orig_Initialize orig, InputManager_Base self)
        {
            self._userData.customControllers = userDataButEpic.customControllers;
            self._userData.customControllerMaps = userDataButEpic.customControllerMaps;
            self._userData.players = userDataButEpic.players;
            orig(self);
            netWizardFakeController = ReInput.ControllerHelper.Instance.CreateCustomController(0);
            ReInput.InputSourceUpdateEvent += ReInput_InputSourceUpdateEvent;
        }

        private void ReInput_InputSourceUpdateEvent()
        {
            netWizardFakeController.SetAxisValue(0, diff.y);
            netWizardFakeController.SetAxisValue(1, diff.x);
        }

        private void BeginHost()
        {
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            myReiliableChannelId = config.AddChannel(QosType.Reliable);
            //myUnreliableChannelId = config.AddChannel(QosType.Unreliable);
            hostTopology = new HostTopology(config, 2);

            thisHostId = NetworkTransport.AddHost(hostTopology, config_hostPort);

            Logger.LogMessage($"HOST ID {thisHostId} on port {config_hostPort}");

            hostSelfConnectionId = NetworkTransport.Connect(thisHostId, "192.168.1.69", config_hostPort, 0, out var error);
        }
        float timer;
        private Vector3 diff;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H)){
                BeginHost(); 
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleIPInput();
            }
            if (this.inputUI != null && Input.GetKeyDown(KeyCode.Return))
            {
                string text = this.ipInputField.text.Trim();
                if (!string.IsNullOrEmpty(text))
                {

                    ClientConnectToHost(int.Parse(text));
                    UnityEngine.Object.Destroy(this.inputUI);
                    this.inputUI = null;
                }
            }
            //if (Input.GetKeyDown(KeyCode.M))
            //{
            //    SendTestMessage();
            //}
            if (Input.GetKeyDown(KeyCode.X))
            {
                SendDisconnectMessage();
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Player2ClaimFakeInput();
            }
            timer -= Time.deltaTime;
            if(timer <= 0 && GameController.playerScripts != null && GameController.playerScripts.Length > 0 && clientConnectionId != -1)
            {
                timer = 0.01f;
                SendPosition(GameController.playerScripts[0].transform.position);
            }

            ReceiveMessage();
        }

        private void SendPosition(Vector3 position)
        {
            byte[] array = new byte[12];

            var xBytes = BitConverter.GetBytes(position.x);
            var yBytes = BitConverter.GetBytes(position.y);
            var zBytes = BitConverter.GetBytes(position.z);

            int arrayIndex = 0;
            for (int i = 0; i < xBytes.Length; i++)
            {
                array[arrayIndex] = xBytes[i];
                arrayIndex++;
            }
            for (int i = 0; i < yBytes.Length; i++)
            {
                array[arrayIndex] = yBytes[i];
                arrayIndex++;
            }
            for (int i = 0; i < zBytes.Length; i++)
            {
                array[arrayIndex] = zBytes[i];
                arrayIndex++;
            }

            NetworkTransport.Send(0, clientConnectionId, myReiliableChannelId, array, array.Length, out var error);

        }

        private void SendDisconnectMessage()
        {
            NetworkTransport.Disconnect(0, clientConnectionId, out var error);
            clientConnectionId = -1;
        }
        
        private void SendTestMessage()
        {
            string message = "your a nipple";
            byte[] array = Encoding.ASCII.GetBytes(message);
            NetworkTransport.Send(0, clientConnectionId, myReiliableChannelId, array, array.Length, out var error);
        }

        private void ClientConnectToHost(int hostIdToConnect)
        {
            //connectionId = NetworkTransport.Connect(hostId, "192.16.7.21", 8888, 0, out error);
            //NetworkTransport.Disconnect(hostId, connectionId, out error);
            //NetworkTransport.Send(hostId, connectionId, myReiliableChannelId, buffer, bufferLength, out error);

            Log.LogMessage($"Client attempting to connect to 192.168.1.69:{config_clientPort}");
            clientConnectionId = NetworkTransport.Connect(hostIdToConnect, "192.168.1.69", config_clientPort, 0, out var error);   
        }

        private void ReceiveMessage()
        {
            int receivedHostId;
            int connectionId;
            int channelId;
            byte[] receivedBuffer = new byte[1024];
            int recievedBufferSize = 1024;
            int receivedDataSize;
            byte error;
            NetworkEventType receievedEventType = NetworkTransport.Receive(out receivedHostId, out connectionId, out channelId, receivedBuffer, recievedBufferSize, out receivedDataSize, out error);

            switch (receievedEventType)
            {
                case NetworkEventType.Nothing:         //1
                    break;
                case NetworkEventType.ConnectEvent:    //2
                    Log.LogWarning($"{connectionId} {hostSelfConnectionId}");
                    if (receivedHostId == thisHostId &&
                        connectionId != hostSelfConnectionId &&
                        (NetworkError)error == NetworkError.Ok)
                    {
                        Log.LogWarning($"Connection request received with connectionId {connectionId}");
                        Player2ClaimFakeInput();
                    }
                    break;
                case NetworkEventType.DataEvent:       //3
                    if (receivedHostId == thisHostId &&
                        connectionId != hostSelfConnectionId &&
                        (NetworkError)error == NetworkError.Ok)
                    {
                        if (GameController.playerScripts.Length <= 1 || !GameController.playerScripts[1])
                        {
                            //no player 2 to drive
                            break;
                        }

                        Vector3 decodedPosition = new Vector3();

                        decodedPosition.x = BitConverter.ToSingle(receivedBuffer, 0);
                        decodedPosition.y = BitConverter.ToSingle(receivedBuffer, 4);
                        decodedPosition.z = BitConverter.ToSingle(receivedBuffer, 8);

                        Vector3 prev = GameController.playerScripts[1].transform.position;
                        GameController.playerScripts[1].transform.position = decodedPosition;
                        diff = (decodedPosition - prev);
                        if(diff.sqrMagnitude > 1)
                        {
                            diff = diff.normalized;
                        }

                    }
                    break;
                case NetworkEventType.DisconnectEvent: //4

                    if (receivedHostId == thisHostId &&
                        connectionId != hostSelfConnectionId //&&
                        //(NetworkError)error == NetworkError.Ok
                        )
                    {
                        Log.LogWarning($"Disconnect request received with connectionId {connectionId}");
                    }
                    break;
            }
        }

        private void Player2ClaimFakeInput()
        {
            CharacterSelectUI.P2CharacterSelectUI.currentController = netWizardFakeController;
            CharacterSelectUI.P2CharacterSelectUI.ClaimInputDevice();
        }

        // Token: 0x06004304 RID: 17156 RVA: 0x0023C248 File Offset: 0x0023A448
        private void ToggleIPInput()
        {
            Debug.Log("NetBoot: ToggleIPInput called; inputUI is " + ((this.inputUI == null) ? "null" : "exists"));
            if (this.inputUI != null)
            {
                UnityEngine.Object.Destroy(this.inputUI);
                this.inputUI = null;
                return;
            }
            this.inputUI = new GameObject("IPInputUI");
            UnityEngine.Object.DontDestroyOnLoad(this.inputUI);
            this.inputUI.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            this.inputUI.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            this.inputUI.AddComponent<GraphicRaycaster>();
            if (UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
            {
                GameObject gameObject = new GameObject("EventSystem");
                gameObject.AddComponent<EventSystem>();
                gameObject.AddComponent<StandaloneInputModule>();
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
            }
            GameObject gameObject2 = new GameObject("Panel");
            gameObject2.transform.SetParent(this.inputUI.transform, false);
            gameObject2.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
            RectTransform component = gameObject2.GetComponent<RectTransform>();
            component.sizeDelta = new Vector2(500f, 100f);
            Vector2 vector = new Vector2(0.5f, 0.5f);
            component.anchorMax = vector;
            component.anchorMin = vector;
            component.pivot = new Vector2(0.5f, 0.5f);
            component.anchoredPosition = Vector2.zero;
            GameObject gameObject3 = new GameObject("IPInputField");
            gameObject3.transform.SetParent(gameObject2.transform, false);
            RectTransform rectTransform = gameObject3.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(450f, 60f);
            vector = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = vector;
            rectTransform.anchorMin = vector;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            this.ipInputField = gameObject3.AddComponent<InputField>();
            this.ipInputField.textComponent = this.CreateText(gameObject3.transform, "", TextAnchor.MiddleLeft, Color.white, 24);
            this.ipInputField.placeholder = this.CreateText(gameObject3.transform, "Enter Host ID...", TextAnchor.MiddleLeft, new Color(0.6f, 0.6f, 0.6f), 24);
            this.ipInputField.ActivateInputField();
            this.ipInputField.Select();
        }

        // Token: 0x0600430B RID: 17163 RVA: 0x0023C904 File Offset: 0x0023AB04
        private Text CreateText(Transform parent, string content, TextAnchor align, Color color, int size)
        {
            GameObject gameObject = new GameObject("Text");
            gameObject.transform.SetParent(parent, false);
            Text text = gameObject.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = align;
            text.color = color;
            RectTransform component = gameObject.GetComponent<RectTransform>();
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            component.offsetMin = (component.offsetMax = Vector2.zero);
            return text;
        }

        private bool Health_TakeDamage(On.Health.orig_TakeDamage orig, Health self, AttackInfo givenAttackInfo, Entity attackEntity, bool critPreCalculated)
        {
            if(givenAttackInfo.entity == GameController.playerScripts[1])
            {
                return false;
            }
            return orig(self,givenAttackInfo,attackEntity,critPreCalculated);
        }

        //no. attacks need to know when they hit for effects to go through.
        //private bool Attack_CheckCollision(On.Attack.orig_CheckCollision orig, Attack self, Collider2D col, int targetObjID)
        //{
        //    if(self.atkInfo.entity == GameController.playerScripts[1])
        //    {
        //        return false;
        //    }
        //    return orig(self, col, targetObjID);
        //}
    }
}
