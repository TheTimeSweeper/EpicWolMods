using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000B40 RID: 2880
public class NetBoot : MonoBehaviour
{
    // Token: 0x060042FF RID: 17151 RVA: 0x0004FFCA File Offset: 0x0004E1CA
    private void Start()
    {
        Debug.Log("NetBoot: Ready. Press H to host, ` to join.");
        //if (UnityEngine.Object.FindObjectOfType<MainThreadDispatcher>() == null)
        //{
        //    GameObject gameObject = new GameObject("Dispatcher");
        //    gameObject.AddComponent<MainThreadDispatcher>();
        //    UnityEngine.Object.DontDestroyOnLoad(gameObject);
        //}
    }

    // Token: 0x06004301 RID: 17153 RVA: 0x0023C050 File Offset: 0x0023A250
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && !this.isHost && !this.isClient)
        {
            this.StartHost();
        }
        if (Input.GetKeyDown(KeyCode.BackQuote) && !this.isHost && !this.isClient)
        {
            this.ToggleIPInput();
        }
        if (this.inputUI != null && Input.GetKeyDown(KeyCode.Return))
        {
            string text = this.ipInputField.text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                this.StartClient(text);
                UnityEngine.Object.Destroy(this.inputUI);
                this.inputUI = null;
            }
        }
        //if (this.isHost && this.client == null && this.listener != null && this.listener.Pending())
        //{
        //    this.AcceptClient();
        //}
        this.sendTimer += Time.deltaTime;
        if (this.sendTimer >= 0.05f)
        {
            this.sendTimer = 0f;
            if (this.isClient)
            {
                this.SendInputToHost();
            }
            if (this.isHost && this.writer != null)
            {
                this.SendHostInputToClient();
            }
        }
        if (this.isHost && this.reader != null)
        {
            this.PollHostRead();
        }
        if (this.isClient && this.reader != null)
        {
            this.PollClientRead();
        }
    }

    // Token: 0x06004302 RID: 17154 RVA: 0x00050017 File Offset: 0x0004E217
    private void StartHost()
    {
        this.isHost = true;
        //this.listener = new TcpListener(IPAddress.Any, 7778);
        //this.listener.Start();
        Debug.Log("NetBoot: Host listening on port 7778.");
    }

    // Token: 0x06004303 RID: 17155 RVA: 0x0023C18C File Offset: 0x0023A38C
    private void SendInputToHost()
    {
        if (this.writer == null)  
        {
            return;
        }
        Vector2 zero = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            zero.y += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            zero.y -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            zero.x += 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            zero.x -= 1f;
        }
        NetBoot.PlayerInputData obj = new NetBoot.PlayerInputData
        {
            x = zero.x,
            y = zero.y
        };
        this.writer.WriteLine("input:" + JsonUtility.ToJson(obj));
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
        this.ipInputField.placeholder = this.CreateText(gameObject3.transform, "Enter IP...", TextAnchor.MiddleLeft, new Color(0.6f, 0.6f, 0.6f), 24);
        this.ipInputField.ActivateInputField();
        this.ipInputField.Select();
    }

    // Token: 0x06004305 RID: 17157 RVA: 0x0023C4A0 File Offset: 0x0023A6A0
    private void AcceptClient()
    {
        //this.client = this.listener.AcceptTcpClient();
        //Debug.Log(string.Format("NetBoot: Client connected from {0}", this.client.Client.RemoteEndPoint));
        //NetworkStream stream = this.client.GetStream();
        //this.reader = new StreamReader(stream, Encoding.UTF8);
        //this.writer = new StreamWriter(stream, Encoding.UTF8)
        //{
        //    AutoFlush = true
        //};
    }

    // Token: 0x06004306 RID: 17158 RVA: 0x0023C514 File Offset: 0x0023A714
    private void PollHostRead()
    {
        //while (this.client.Available > 0)
        //{
        //    string text = this.reader.ReadLine();
        //    Debug.Log("NetBoot: Host got: " + text);
        //    if (text.StartsWith("playerdata:"))
        //    {
        //        this.TrySpawnClientDummy(text.Substring("playerdata:".Length));
        //    }
        //    else if (text.StartsWith("input:"))
        //    {
        //        NetBoot.PlayerInputData playerInputData = JsonUtility.FromJson<NetBoot.PlayerInputData>(text.Substring("input:".Length));
        //        if (this.remotePlayerObject != null)
        //        {
        //            RemoteChaosInputDevice remoteChaosInputDevice = this.remotePlayerObject.GetComponent<Player>().inputDevice as RemoteChaosInputDevice;
        //            if (remoteChaosInputDevice != null)
        //            {
        //                Vector2 vector = new Vector2(playerInputData.x, playerInputData.y);
        //                Debug.Log(string.Format("NetBoot: Applying client movement → {0}", vector));
        //                remoteChaosInputDevice.SetMoveInput(vector);
        //                if (playerInputData.dash)
        //                {
        //                    Debug.Log("NetBoot: Triggering client DASH");
        //                    remoteChaosInputDevice.TriggerDash();
        //                }
        //                if (playerInputData.skillNo != 0)
        //                {
        //                    Debug.Log(string.Format("NetBoot: Triggering client SKILL #{0}", playerInputData.skillNo));
        //                    remoteChaosInputDevice.TriggerSkill(playerInputData.skillNo);
        //                }
        //            }
        //        }
        //    }
        //}
    }

    // Token: 0x06004307 RID: 17159 RVA: 0x0023C640 File Offset: 0x0023A840
    private void StartClient(string ip)
    {
        //this.isClient = true;
        //this.client = new TcpClient();
        //this.client.Connect(ip, 7778);
        //Debug.Log("NetBoot: Connected to host.");
        //NetworkStream stream = this.client.GetStream();
        //this.reader = new StreamReader(stream, Encoding.UTF8);
        //this.writer = new StreamWriter(stream, Encoding.UTF8)
        //{
        //    AutoFlush = true
        //};
        //this.writer.WriteLine("playerdata:" + this.SerializeSave());
        //this.TrySpawnHostDummy();
    }

    // Token: 0x06004308 RID: 17160 RVA: 0x0023C6D0 File Offset: 0x0023A8D0
    private void PollClientRead()
    {
        //while (this.client.Available > 0)
        //{
        //    string text = this.reader.ReadLine();
        //    Debug.Log("NetBoot: Client got: " + text);
        //    if (text.StartsWith("hostinput:"))
        //    {
        //        NetBoot.PlayerInputData playerInputData = JsonUtility.FromJson<NetBoot.PlayerInputData>(text.Substring("hostinput:".Length));
        //        this.TrySpawnHostDummy();
        //        if (this.hostDummyObject != null)
        //        {
        //            RemoteChaosInputDevice remoteChaosInputDevice = this.hostDummyObject.GetComponent<Player>().inputDevice as RemoteChaosInputDevice;
        //            if (remoteChaosInputDevice != null)
        //            {
        //                Vector2 vector = new Vector2(playerInputData.x, playerInputData.y);
        //                Debug.Log(string.Format("NetBoot: Applying host movement → {0}", vector));
        //                remoteChaosInputDevice.SetMoveInput(vector);
        //                if (playerInputData.dash)
        //                {
        //                    Debug.Log("NetBoot: Triggering host DASH on client");
        //                    remoteChaosInputDevice.TriggerDash();
        //                }
        //                if (playerInputData.skillNo != 0)
        //                {
        //                    Debug.Log(string.Format("NetBoot: Triggering host SKILL #{0} on client", playerInputData.skillNo));
        //                    remoteChaosInputDevice.TriggerSkill(playerInputData.skillNo);
        //                }
        //            }
        //        }
        //    }
        //}
    }

    // Token: 0x06004309 RID: 17161 RVA: 0x0023C7DC File Offset: 0x0023A9DC
    private void TrySpawnDummy(string json)
    {
        if (this.hostDummyObject != null)
        {
            return;
        }
        Debug.Log("NetBoot: Spawning host dummy from -> " + json);
        try
        {
            JsonUtility.FromJson<NetBoot.DummySave>(json);
        }
        catch (Exception arg)
        {
            Debug.LogError("NetBoot: Save parse error — " + arg);
            return;
        }
        //foreach (MonoBehaviour monoBehaviour in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>())
        //{
        //    MethodInfo method = monoBehaviour.GetType().GetMethod("SpawnPlayer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //    if (method != null && method.GetParameters().Length == 0)
        //    {
        //        FieldInfo field = monoBehaviour.GetType().GetField("playerInputDev", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //        if (field != null)
        //        {
        //            field.SetValue(monoBehaviour, new RemoteChaosInputDevice());
        //        }
        //        method.Invoke(monoBehaviour, null);
        //        break;
        //    }
        //}
        //foreach (Player player in UnityEngine.Object.FindObjectsOfType<Player>())
        //{
        //    if (player.inputDevice is RemoteChaosInputDevice)
        //    {
        //        this.hostDummyObject = player.gameObject;
        //        Debug.Log("NetBoot: Client spawned host dummy '" + this.hostDummyObject.name + "'");
        //        return;
        //    }
        //}
        //Debug.LogWarning("NetBoot: Failed to find spawned host dummy!");
    }

    // Token: 0x0600430A RID: 17162 RVA: 0x0005004A File Offset: 0x0004E24A
    private string SerializeSave()
    {
        return JsonUtility.ToJson(new NetBoot.DummySave
        {
            exampleField = 42
        });
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

    // Token: 0x0600430C RID: 17164 RVA: 0x0023C98C File Offset: 0x0023AB8C
    private void TrySpawnClientDummy(string json)
    {
        if (this.remotePlayerObject != null)
        {
            return;
        }
        //Debug.Log("NetBoot: Spawning client dummy from -> " + json);
        //JsonUtility.FromJson<NetBoot.DummySave>(json);
        //foreach (MonoBehaviour monoBehaviour in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>())
        //{
        //    MethodInfo method = monoBehaviour.GetType().GetMethod("SpawnPlayer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //    if (method != null && method.GetParameters().Length == 0)
        //    {
        //        FieldInfo field = monoBehaviour.GetType().GetField("playerInputDev", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //        if (field != null)
        //        {
        //            field.SetValue(monoBehaviour, new RemoteChaosInputDevice());
        //        }
        //        method.Invoke(monoBehaviour, null);
        //        break;
        //    }
        //}
        //foreach (Player player in UnityEngine.Object.FindObjectsOfType<Player>())
        //{
        //    if (player.inputDevice is RemoteChaosInputDevice)
        //    {
        //        this.remotePlayerObject = player.gameObject;
        //        Debug.Log("NetBoot: Host assigned client dummy '" + this.remotePlayerObject.name + "'");
        //        return;
        //    }
        //}
        //Debug.LogWarning("NetBoot: Host could not find spawned client dummy!");
    }

    // Token: 0x0600430D RID: 17165 RVA: 0x0023CA88 File Offset: 0x0023AC88
    private void TrySpawnHostDummy()
    {
        if (this.hostDummyObject != null)
        {
            return;
        }
        //Debug.Log("NetBoot: Spawning host dummy…");
        //List<Player> second = UnityEngine.Object.FindObjectsOfType<Player>().ToList<Player>();
        //foreach (MonoBehaviour monoBehaviour in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>())
        //{
        //    MethodInfo method = monoBehaviour.GetType().GetMethod("SpawnPlayer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //    if (method != null && method.GetParameters().Length == 0)
        //    {
        //        foreach (FieldInfo fieldInfo in monoBehaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        //        {
        //            if (typeof(RemoteChaosInputDevice).IsAssignableFrom(fieldInfo.FieldType) || fieldInfo.FieldType.Name.ToLower().Contains("inputdev"))
        //            {
        //                fieldInfo.SetValue(monoBehaviour, new RemoteChaosInputDevice());
        //                Debug.Log("NetBoot: Injected RemoteChaosInputDevice into " + monoBehaviour.GetType().Name + "." + fieldInfo.Name);
        //            }
        //        }
        //        method.Invoke(monoBehaviour, null);
        //        break;
        //    }
        //}
        //List<Player> list = UnityEngine.Object.FindObjectsOfType<Player>().Except(second).ToList<Player>();
        //if (list.Count == 0)
        //{
        //    Debug.LogWarning("NetBoot: Could not find newly spawned host dummy!");
        //    return;
        //}
        //Player player = list[0];
        //this.hostDummyObject = player.gameObject;
        //Debug.Log("NetBoot: Client assigned host dummy '" + this.hostDummyObject.name + "'");
    }

    // Token: 0x0600430E RID: 17166 RVA: 0x0023CC00 File Offset: 0x0023AE00
    private void SendHostInputToClient()
    {
        Vector2 zero = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            zero.y += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            zero.y -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            zero.x += 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            zero.x -= 1f;
        }
        bool queuedDash = this._queuedDash;
        int queuedSkill = this._queuedSkill;
        NetBoot.PlayerInputData obj = new NetBoot.PlayerInputData
        {
            x = zero.x,
            y = zero.y,
            dash = queuedDash,
            skillNo = queuedSkill
        };
        this.writer.WriteLine("hostinput:" + JsonUtility.ToJson(obj));
        Debug.Log("NetBoot: Sent hostinput:" + JsonUtility.ToJson(obj));
        this._queuedDash = false;
        this._queuedSkill = 0;
    }

    // Token: 0x170009BE RID: 2494
    // (get) Token: 0x06004310 RID: 17168 RVA: 0x0005005E File Offset: 0x0004E25E
    // (set) Token: 0x06004311 RID: 17169 RVA: 0x00050065 File Offset: 0x0004E265
    public static NetBoot Instance { get; private set; }

    // Token: 0x04004B61 RID: 19297
    //private TcpListener listener;

    // Token: 0x04004B62 RID: 19298
    private float sendTimer;

    // Token: 0x04004B63 RID: 19299
    private Vector2 latestReceivedMovement;

    // Token: 0x04004B64 RID: 19300
    private GameObject remotePlayerObject;

    // Token: 0x04004B65 RID: 19301
    private bool isClient;

    // Token: 0x04004B66 RID: 19302
    private bool isHost;

    // Token: 0x04004B67 RID: 19303
    private GameObject inputUI;

    // Token: 0x04004B68 RID: 19304
    private InputField ipInputField;

    // Token: 0x04004B69 RID: 19305
    private bool hasSpawnedDummy;

    // Token: 0x04004B6A RID: 19306
    //private TcpClient client;

    // Token: 0x04004B6B RID: 19307
    private StreamReader reader;

    // Token: 0x04004B6C RID: 19308
    private StreamWriter writer;

    // Token: 0x04004B6D RID: 19309
    private GameObject hostDummyObject;

    // Token: 0x04004B6E RID: 19310
    private Vector2 latestHostMovement = Vector2.zero;

    // Token: 0x04004B6F RID: 19311
    private Vector2 latestClientMovement = Vector2.zero;

    // Token: 0x04004B70 RID: 19312
    private bool _queuedDash;

    // Token: 0x04004B71 RID: 19313
    private int _queuedSkill;

    // Token: 0x02000B41 RID: 2881
    [Serializable]
    private class PlayerInputData
    {
        // Token: 0x04004B73 RID: 19315
        public float x;

        // Token: 0x04004B74 RID: 19316
        public float y;

        // Token: 0x04004B75 RID: 19317
        public bool dash;

        // Token: 0x04004B76 RID: 19318
        public int skillNo;
    }

    // Token: 0x02000B42 RID: 2882
    [Serializable]
    private class DummySave
    {
        // Token: 0x04004B77 RID: 19319
        public int exampleField;
    }
}