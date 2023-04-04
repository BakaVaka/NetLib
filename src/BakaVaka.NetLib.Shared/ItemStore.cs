using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.NetLib.Shared;
public sealed class ItemStore : IItemStore {
    private Dictionary<string, Object> _items = new();
    public Boolean HasItem(String key) => _items.ContainsKey(key);
    public Boolean HasItemOf<T>(String key) => TryGetItemOf<T>(key, out _);
    public void SetItem(String key, Object item) => _items[key] = item;
    public void SetItem<T>(String key, T item) => _items[key] = item;
    public Boolean TryGetItem(String key, out Object item) => _items.TryGetValue(key, out item);
    public Boolean TryGetItemOf<T>(String key, out T item) {
        item = default;
        if(TryGetItem(key, out var it) && item is T t) {
            item = t;
            return true;
        }
        return false;
    }
}
