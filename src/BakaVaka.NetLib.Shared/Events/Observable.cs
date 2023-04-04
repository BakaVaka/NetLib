namespace BakaVaka.NetLib.Shared.Events;
public class Observable<TEvent> {
    private List<Observer<TEvent>> _subscribers = new();
    private object _syncRoot = new();
    public void Publish(TEvent e) {
        // вот тут непонятно, надо ли локать чи не надо
        // типа вот диспоз вызвался, ждет пока обраотаем
        // с другой стороны, если мы таки просто обрабатываем - в результате можем пропустить кого-то кто подписался
        // СЛОЖНА!!!
        // Считаем, что если задиспозился - субскрибер должен сам это обрабатывать и не ебет
        DoSynchronized(() => {
            for(int i = 0; i < _subscribers.Count; i++) {
                var subscriber = _subscribers[i];
                subscriber.Signal(e);
            }
        });
    }

    public IDisposable Subscribe(Observer<TEvent> observer) {
        var unsubscriber = new Unsubscriber(
            () => {
                DoSynchronized(() => {
                    _subscribers.Remove(observer);
                });
            });

        DoSynchronized(() => {
            _subscribers.Add(observer);
        });


        return unsubscriber;
    }

    private void DoSynchronized(Action action) {
        lock( _syncRoot ) {
            action();
        }
    }

    private class Unsubscriber : IDisposable {
        private Action _unsub;
        public Unsubscriber(Action unsub) {
            _unsub = unsub;
        }

        public void Dispose() {
            _unsub?.Invoke();
        }
    }
}
