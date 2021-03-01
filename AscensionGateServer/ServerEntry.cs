using Cosmos;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using RedisService;
using System.Threading;

namespace AscensionGateServer
{
    class ServerEntry
    {
        public delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        static ControlCtrlDelegate newDelegate = new ControlCtrlDelegate(HandlerRoutine);
        public static bool HandlerRoutine(int CtrlType)
        {
            Utility.Debug.LogInfo("Server Shutdown !");//按控制台关闭按钮关闭 
            return false;
        }
        static string ip = "127.0.0.1";
        static int port = 8511;
        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(newDelegate, true);
            CosmosEntry.LaunchHelpers();
            Utility.Debug.LogInfo("Server Start Running !");
            CosmosEntry.LaunchModules();
            CosmosEntry.NetworkManager.Connect(ip, port, System.Net.Sockets.ProtocolType.Udp);
            RedisManager.Instance.OnInitialization();
            CosmosEntry.Run();
        }
     public static IDataManager DataManager { get { return GameManager.GetModule<IDataManager>(); } }
     public static INetMessageManager  NetMessageManager{ get { return GameManager.GetModule<INetMessageManager>(); } }
    }
}
