namespace Shared
{
    using BakaVaka.TcpServerLib;
    using System;

    public class EmptyContext : IConnectionContext
    {
        private object _owner;
        public object Owner => _owner;

        public void Bind(object connection)
        {
            if (_owner is null)
            {
                _owner = connection;
                return;
            }
            throw new InvalidOperationException("Already binded");
        }

        public void Dispose() { }

        public T Get<T>() { throw new NotImplementedException(); }
    }

}
