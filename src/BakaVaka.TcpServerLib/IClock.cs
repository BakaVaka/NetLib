using System;

namespace BakaVaka.TcpServerLib
{
    /// <summary>
    /// clock abstraction for testing
    /// </summary>
    public interface IClock
    {
        public DateTime Now { get; }
    }
}