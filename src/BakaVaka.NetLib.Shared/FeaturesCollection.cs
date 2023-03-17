using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.NetLib.Shared;

/// <summary>
/// Класс для хранения фич соединения и предоставления к ним доступа
/// </summary>
public class FeatureCollection : IFeatureCollection {
    private Dictionary<Type, object> _features = new();

    public Boolean HasFeatuer<T>() => _features.ContainsKey(typeof(T));
    public void Set<T>(T feature) => _features[typeof(T)] = feature;
    public T? Get<T>() {
        if( _features.TryGetValue(typeof(T), out var feature) ) {
            return (T)feature;
        }
        return default;
    }
}