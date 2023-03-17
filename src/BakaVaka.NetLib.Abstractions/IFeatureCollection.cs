using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BakaVaka.NetLib.Abstractions;
public interface IFeatureCollection {
    public Boolean HasFeatuer<T>();
    public void Set<T>(T feature);
    public T? Get<T>();
}
