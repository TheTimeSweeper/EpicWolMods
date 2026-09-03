using UnityEngine.Networking;

namespace NetWizarding
{
    public static class UNETExtensions
    {

        private static int nextConnectionId = -1;

        /// Because we fake the UNET connection, connection initialization is not handled by UNET internally. 
        /// Connections must be manually initialized with this function.
        public static void ForceInitialize(this NetworkConnection conn)
        {
            int id = ++nextConnectionId;
            conn.Initialize("localhost", id, id, NetWizardingManager_HLAPI_But_Steam.instance.hostTopology);
        }
        public static bool IsReady(this NetworkClient client)
        {
            return client != null && client.connection != null && client.connection.isConnected;
        }
    }
}
