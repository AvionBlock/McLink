using System;

namespace MCLink
{
    public abstract class McLinkPeer
    {
        public Guid Token { get; } = Guid.NewGuid();
    }
}