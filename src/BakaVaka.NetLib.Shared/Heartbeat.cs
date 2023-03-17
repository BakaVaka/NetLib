using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.TcpServerLib;
/// <summary>
/// Класс реализующий "удар сердца" сервера
/// Т.е. раз в некоторый интервал времени - выполняет над соединениями какую-то логику.
/// В зависимости от логики сервера, это может быть отправка Ping-сообщения, закрытие соединения по неактивности, перевода в состояние "проставивает" и т.д.
/// </summary>
public sealed class Heartbeat {
    public class HeartbeatSettings {
        public TimeSpan HeartbeatInterval { get; set; }
    }
    private TimeSpan _interval = TimeSpan.FromSeconds(1);
    private IClock _clock;
    private CancellationTokenSource? _cancellationTokenSource;
    private Thread? _heartbeatThread;
    public delegate void HeartbeatDelegate(DateTimeOffset now);
    public event HeartbeatDelegate Tick;
    private object _syncRoot = new();
    public Heartbeat(IClock clock, HeartbeatSettings settings) {
        _clock = clock;
        _interval = settings.HeartbeatInterval;
    }

    public void Start() {
        lock( _syncRoot ) {
            if( _heartbeatThread is not null ) {
                throw new InvalidOperationException("Heartbeat already started");
            }
            _cancellationTokenSource = new CancellationTokenSource();
            _heartbeatThread = new Thread(HeartbeatHandler) {
                IsBackground = true,
                Name = "Heartbeat"
            };
            _heartbeatThread.Start();
        }
    }

    public void Stop() {
        lock( _syncRoot ) {
            if( _heartbeatThread is null ) {
                throw new InvalidOperationException("Heartbeat not started yet");
            }
            _cancellationTokenSource.Cancel();
            _heartbeatThread.Join();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            _heartbeatThread = null;
        }
    }


    private void HeartbeatHandler() {
        var ct = _cancellationTokenSource.Token;
        while( !ct.IsCancellationRequested ) {
            Thread.Sleep(_interval);
            var tick = Tick;
            if( _heartbeatThread != null ) {
                tick(_clock.Now);
            }
        }
    }
}
