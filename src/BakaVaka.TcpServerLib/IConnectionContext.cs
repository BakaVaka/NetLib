using System;

namespace BakaVaka.TcpServerLib
{
    public interface IConnectionContext : IDisposable
    {
        //Владелец контекста
        public object Owner { get; }
        public void Bind(object connection);
        public T Get<T>();
    }
}