using System;
using Cosmos;
using Cosmos.Network;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using ProtocolCore;
using Protocol;
using System.Collections.Generic;
using Lidgren.Network;
using System.Runtime.Serialization.Formatters.Binary;

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
            GameManager.External.GetModule<JWTManager>().SetHelper(new GateEncodeHelper());
            Task.Run(GameManagerAgent.Instance.OnRefresh);
            AssertCode();
            while (true) { }
        }
        static void AssertCode()
        {
            User userObj = new User() { Account = "ws123456", Password = "jieyougame", UUID = "ABCD" };
            var token = GameManager.External.GetModule<JWTManager>().EncodeToken(userObj);
            Utility.Debug.LogInfo(token);
            var desEnToken = Utility.Encryption.DESEncrypt(token, AppConst._TokenSecretKey, AppConst.KcpIV);
            var desDeToken = Utility.Encryption.DESDecrypt(desEnToken ,AppConst._TokenSecretKey, AppConst.KcpIV);
            Utility.Debug.LogInfo("对称加密 : "+desEnToken);
            Utility.Debug.LogInfo("对称解密 : "+desDeToken);
            var bitToken = Utility.Converter.GetBytes(desEnToken);
            string str="";
            for (int i = 0; i < bitToken.Length; i++)
            {
                str += "-"+bitToken[i].ToString("X2");
            }
            Utility.Debug.LogInfo(str);
            var deToken = GameManager.External.GetModule<JWTManager>().DecodeToken<User>(token);
            Utility.Debug.LogInfo(deToken.ToString());
        }
    }
}
