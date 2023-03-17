using System.IO.Pipelines;

namespace BakaVaka.NetLib.Abstractions;

/// <summary>
/// Представляет транспорт
/// </summary>
public interface ITransport {
    public PipeReader In { get; }
    public PipeWriter Out { get; }
}
public class DefaultTransport : ITransport {
    public DefaultTransport(PipeReader @in, PipeWriter @out) {
        In = @in;
        Out = @out;
    }

    public PipeReader In { get; }
    public PipeWriter Out { get; }
}

public interface ITransportPair {
    public ITransport In { get; }
    public ITransport Out { get; }
}
//пара пайп, для реализации двунаправленной линии связи(от соединения к приложению и от приложения к соедниению)
public class DefaultTransportPair : ITransportPair {
    public DefaultTransportPair(Pipe @in, Pipe @out) {
        In = new DefaultTransport(@in.Reader, @out.Writer);
        Out = new DefaultTransport(@out.Reader, @in.Writer);
    }

    public ITransport In { get; }
    public ITransport Out { get; }
}