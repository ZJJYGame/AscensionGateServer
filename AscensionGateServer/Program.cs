using System;
using Cosmos;
using Cosmos.Network;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using ProtocolCore;
using Protocol;
using System.Collections.Generic;

namespace AscensionGateServer
{
    class Program
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
            Utility.Logger.SetHelper(new ConsoleLoggerHelper("AscensionGateServer"));
            Utility.Debug.SetHelper(new ConsoleDebugHelper());
            Utility.Json.SetHelper(new NewtonjsonHelper());
            Utility.Debug.LogInfo("Server Start Running !");
            GameManager.NetworkManager.Connect(ip, port, System.Net.Sockets.ProtocolType.Udp);
            GameManager.External.GetModule<JWTManager>().SetHelper(new GateTokenHelper());

            User userObj = new User() { Account = "ws123456", Password = "jieyougame", UUID = "ABCD" };
            var userJson = Utility.Json.ToJson(userObj);
            var token = GameManager.External.GetModule<JWTManager>().EncodeToken("User","jieyou");
            Utility.Debug.LogInfo(token);
            var deToken = GameManager.External.GetModule<JWTManager>().DecodeToken(token);
            //List<string> str = Utility.Json.ToObject<List<string>>(deToken);
            List<string> str = new List<string>() { "ws", "jck" };
            Utility.Debug.LogInfo(deToken);
            //Utility.Debug.LogInfo(str);
            Utility.Debug.LogInfo(Utility.Json.ToJson(str));
            Task.Run(GameManagerAgent.Instance.OnRefresh);
            while (true) { }
        }
    }
}
