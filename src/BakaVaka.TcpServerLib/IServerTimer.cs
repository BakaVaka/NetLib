using System;

namespace BakaVaka.TcpServerLib
{
    public interface IClock
    {
        public DateTimeOffset Now { get; }
        public DateTimeOffset NowUTC { get; }
    }
}