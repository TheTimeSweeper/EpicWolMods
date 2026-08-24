using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace NetWizarding
{
    public class NetworkTransportVersion : MonoBehaviour
    {
        private int myReiliableChannelId;
        private int myUnreliableChannelId;
        private int thisHostId;
        private HostTopology hostTopology;
        private int hostSelfConnectionId = -1;

        private int clientConnectionId = -1;

        private List<int> hostConnectedClientIds = new List<int>();

        private string IPToConnect;

        private float positionSyncTimer;
        private Vector3 previousNetworkPosition;

        void Awake()
        {
            BeginHost();
        }

        private void BeginHost()
        {
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            myReiliableChannelId = config.AddChannel(QosType.Reliable);
            //myUnreliableChannelId = config.AddChannel(QosType.Unreliable);
            hostTopology = new HostTopology(config, 2);

            thisHostId = NetworkTransport.AddHost(hostTopology, NetWizardingPlugin.config_hostPort);

            Log.Message($"HOST ID {thisHostId} on port {NetWizardingPlugin.config_hostPort}");

            //hostSelfConnectionId = NetworkTransport.Connect(thisHostId, "192.168.1.69", config_hostPort, 0, out var error);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                BeginHost();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (!Util.inputUI)
                {
                    Util.ToggleIPInput();
                }
                else
                {
                    IPToConnect = Util.ConsumeIPInput();
                    ClientConnectToHost(IPToConnect);
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
            if (Input.GetKeyDown(KeyCode.Y))
            {
                DisconnectAll();
            }
            positionSyncTimer -= Time.deltaTime;
            if (positionSyncTimer <= 0 && GameController.playerScripts != null && GameController.playerScripts.Length > 0 && clientConnectionId != -1)
            {
                positionSyncTimer = 0.01f;
                SendPosition(GameController.playerScripts[0].transform.position);
            }

            ReceiveMessage();
        }

        private void DisconnectAll()
        {
            for (int i = 0; i < hostConnectedClientIds.Count; i++)
            {
                NetworkTransport.Disconnect(0, hostConnectedClientIds[i], out _);
            }
            hostConnectedClientIds.Clear();
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

        private void ClientConnectToHost(string ip)
        {
            //connectionId = NetworkTransport.Connect(hostId, "192.16.7.21", 8888, 0, out error);
            //NetworkTransport.Disconnect(hostId, connectionId, out error);
            //NetworkTransport.Send(hostId, connectionId, myReiliableChannelId, buffer, bufferLength, out error);
            if (string.IsNullOrEmpty(ip))
            {
                ip = "127.0.0.1";
            }
            Log.Message($"Client attempting to connect to {ip}:{NetWizardingPlugin.config_clientPort}");
            clientConnectionId = NetworkTransport.Connect(0, ip, NetWizardingPlugin.config_clientPort, 0, out var error);
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
                    //Log.Warning($"{connectionId} {hostSelfConnectionId}");
                    if (receivedHostId == thisHostId &&
                        //connectionId != hostSelfConnectionIda &&
                        (NetworkError)error == NetworkError.Ok)
                    {
                        hostConnectedClientIds.Add(connectionId);
                        Log.Warning($"Connection request received with connectionId {connectionId}");
                        NetWizardingPlugin.Instance.Player2ClaimFakeInput();
                    }
                    break;
                case NetworkEventType.DataEvent:       //3
                    if (receivedHostId == thisHostId &&
                        //connectionId != hostSelfConnectionId &&
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

                        GameController.playerScripts[1].transform.position = decodedPosition;
                        NetWizardingPlugin.Instance.positionDifference = (decodedPosition - previousNetworkPosition).normalized;
                        previousNetworkPosition = decodedPosition;
                    }
                    break;
                case NetworkEventType.DisconnectEvent: //4

                    if (receivedHostId == thisHostId //&&
                        //connectionId != hostSelfConnectionId //&&
                        //(NetworkError)error == NetworkError.Ok
                        )
                    {
                        Log.Warning($"Disconnect request received with connectionId {connectionId}");
                    }
                    break;
            }
        }

    }
}
