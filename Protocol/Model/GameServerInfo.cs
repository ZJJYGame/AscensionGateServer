using System;
namespace Protocol
{
    [Serializable]
    public class GameServerInfo
    {
        /// <summary>
        /// 游戏逻辑服务器的IP；
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 游戏逻辑服务器的端口；
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 游戏逻辑服务器的序号；
        /// 可能有多台游戏逻辑服务器；
        /// </summary>
        public ushort Index { get; set; }
        /// <summary>
        /// 游戏逻辑服务器应用名；
        /// </summary>
        public string AppName { get; set; }
    }
}
