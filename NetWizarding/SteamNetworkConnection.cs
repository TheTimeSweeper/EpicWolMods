using Steamworks;
using UnityEngine.Networking;

namespace NetWizarding
{
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
}
