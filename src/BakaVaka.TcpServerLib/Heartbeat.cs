using System;
using System.Collections.Generic;
using System.Linq;

namespace BakaVaka.TcpServerLib;
/// <summary>
/// Класс реализующий "удар сердца" сервера
/// Т.е. раз в некоторый интервал времени - выполняет над соединениями какую-то логику.
/// В зависимости от логики сервера, это может быть отправка Ping-сообщения, закрытие соединения по неактивности, перевода в состояние "проставивает" и т.д.
/// </summary>
public sealed class Heartbeat
{
    /// <summary>
    /// Логика которая должна выполниться с соединением в рамках "удара сердца"
    /// </summary>
    public interface IOnHeartbeatHandler
    {
        public void OnHeartbeat(IConnectionContext connectionContext);
    }
    private class CompositeHeartbeatHandler : IOnHeartbeatHandler
    {
        private readonly Action<IConnectionContext> _handler;
        private readonly Action<IConnectionContext> _next;

        public CompositeHeartbeatHandler(Action<IConnectionContext> handler, Action<IConnectionContext> next)
        {
            _handler = handler;
            _next = next;
        }

        public void OnHeartbeat(IConnectionContext connectionContext)
        {
            _handler?.Invoke(connectionContext);
            _next?.Invoke(connectionContext);
        }
    }

    /// <summary>
    /// Настройки для "удара сердца"
    /// </summary>
    public class HeartbeatSettings
    {
        public TimeSpan HeartbeatInterval { get; set; }
    }
    /// <summary>
    /// Интервал для "удара сердца"
    /// </summary>
    private TimeSpan _interval = TimeSpan.FromSeconds(1);
    private Action<IConnectionContext> _onHeartbeat;
    public Heartbeat(IEnumerable<IOnHeartbeatHandler> onHeartbeatHandlers, HeartbeatSettings settings)
    {
        _onHeartbeat = onHeartbeatHandlers.Aggregate((acc, handler) =>
        {
            return new CompositeHeartbeatHandler(handler.OnHeartbeat, acc.OnHeartbeat);
        }).OnHeartbeat;
        
    }
}
