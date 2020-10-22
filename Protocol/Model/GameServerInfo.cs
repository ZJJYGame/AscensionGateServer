using System;
using System.Collections.Generic;
using MessagePack;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class GameServerInfo
    {
        [Key(0)]
        public List<GameServer> GameServerList { get; set; }
        public void SetData(object data)
        {
            GameServerList = data as List<GameServer>;
        }
        [Serializable]
        [MessagePackObject]
        public class GameServer
        {
            [Key(0)]
            /// <summary>
            /// 游戏逻辑服务器的IP；
            /// </summary>
            public string Address { get; set; }
            [Key(1)]
            /// <summary>
            /// 游戏逻辑服务器的端口；
            /// </summary>
            public int Port { get; set; }
            [Key(2)]
            /// <summary>
            /// 游戏逻辑服务器的序号；
            /// 可能有多台游戏逻辑服务器；
            /// </summary>
            public ushort Index { get; set; }
            [Key(3)]
            /// <summary>
            /// 服务器名
            /// </summary>
            public string ServerName{ get; set; }
            [Key(4)]
            /// <summary>
            /// 游戏逻辑服务器应用名；
            /// </summary>
            public string AppName { get; set; }
        }
    }
}
