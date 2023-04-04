namespace BakaVaka.NetLib.Shared.Events;
public class Observer<TEvent> {

    private Action<TEvent> _eventHandler;
    public Observer(Action<TEvent> eventHandler) {
        _eventHandler = eventHandler;
    }
    public void Signal(TEvent e) => _eventHandler?.Invoke(e);
}
