namespace BakaVaka.NetLib.Abstractions;

public interface IItemStore {

    public bool HasItemOf<T>(string key);
    public bool HasItem(string key);
    public bool TryGetItem(string key, out object item);
    public bool TryGetItemOf<T>(string key, out T item);
    public void SetItem(string key, object item);
    public void SetItem<T>(string key, T item);
}