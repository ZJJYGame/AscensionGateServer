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
            Utility.Debug.SetHelper(new ConsoleDebugHelper());
            Utility.Json.SetHelper(new NewtonjsonHelper());
            Utility.MessagePack.SetHelper(new ImplMessagePackHelper());
            Utility.Debug.LogInfo("Server Start Running !");
            GameManager.NetworkManager.Connect(ip, port, System.Net.Sockets.ProtocolType.Udp);
            GameManager.InitCustomeModule(typeof(ServerEntry).Assembly);
            RedisManager.Instance.OnInitialization();
            GameManagerAgent.Instance.Start();
        }
        static void RedisAssert()
        {
        }
    }
}
