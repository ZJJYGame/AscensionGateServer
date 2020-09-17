using System;
using System.Collections.Generic;

namespace Protocol
{
    [Serializable]
    public class GameServerInfo
    {
        public List<GameServer> GameServerList { get; set; }
        public void SetData(object data)
        {
            GameServerList = data as List<GameServer>;
        }
        [Serializable]
        public class GameServer
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
            /// 服务器名
            /// </summary>
            public string ServerName{ get; set; }
            /// <summary>
            /// 游戏逻辑服务器应用名；
            /// </summary>
            public string AppName { get; set; }
        }
    }
}
