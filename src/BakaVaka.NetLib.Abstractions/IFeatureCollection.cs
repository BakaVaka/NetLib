namespace BakaVaka.NetLib.Abstractions;
public interface IFeatureCollection {
    public Boolean HasFeatuer<T>();
    public void Set<T>(T feature);
    public T? Get<T>();
}
