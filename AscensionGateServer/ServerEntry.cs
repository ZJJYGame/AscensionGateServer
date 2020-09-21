using System;
using Cosmos;
using Cosmos.Network;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Protocol;
using System.Collections.Generic;
using Lidgren.Network;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using RedisService;

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
            Utility.Debug.LogInfo("Server Start Running !");
            GameManager.NetworkManager.Connect(ip, port, System.Net.Sockets.ProtocolType.Udp);
            GameManager.InitCustomeModule(typeof(ServerEntry).Assembly);
            Task.Run(GameManagerAgent.Instance.OnRefresh);
            RedisManager.Instance.OnInitialization();
            while (true) { }
        }
        static void RedisAssert()
        {
        }
    }
}
