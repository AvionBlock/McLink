using System;
using System.Collections.Generic;

namespace MCLink
{
    public interface IMcLinkSerializable
    {
        public void Serialize(List<byte> data);
        
        public void Deserialize(Span<byte> data);
    }
}