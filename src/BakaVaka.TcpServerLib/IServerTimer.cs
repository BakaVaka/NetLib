using System;

namespace BakaVaka.TcpServerLib
{
    public interface IServerTimer
    {
        public DateTime ServerTime { get; }
    }
}