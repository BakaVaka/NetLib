using System.Net;

namespace BakaVaka.NetLib.Abstractions;

public delegate Task ConnectionHandler(IConnection connection, CancellationToken cancellationToken = default);

/// <summary>
/// Представляет из себя слушателя входящих подключений
/// </summary>
public interface IListener {
    public event Action<IConnection> ConnectionAccepted;
    public EndPoint BindedEndPoint { get; }

    public void Bind();
    public void Unbind();
    public Task Run(CancellationToken cancellationToken = default);
}

public interface IListenerFactory {
    public IListener Create(EndPoint bindedEndPoint);
}