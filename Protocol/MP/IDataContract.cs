//***********************************************************
// 描述：
// 作者：Don  
// 创建时间：2020-11-13 10:44:59
// 版 本：1.0
//***********************************************************
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    /// <summary>
    /// 使用须知：
    /// C2S指从客户端发送到服务，一般数据类型表示为单元(Unit)；
    /// S2C指从服务器发送到客户端，一般指单元的集合(UnitSet)，由S2C数据包含C2S的一组集合；
    /// </summary>
    [Union(6, typeof(DataParameters))]
    public interface IDataContract { }
}


















