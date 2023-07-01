using System.IO.Pipelines;
using System.Text;
using System.Xml;

using BakaVaka.NetLib.Abstractions;

namespace Shared;
internal class SimpleXMLProtocol : ICodec<SimpleXMLProtocolMessage> {
    private const int _headerLenght = sizeof(long);
    private const ulong _maxMessageSize = 1024 * 10;
    public ValueTask<SimpleXMLProtocolMessage> Decode(PipeReader reader, CancellationToken cancellationToken = default) {
        var stream = reader.AsStream();
        var memoryStream = new BinaryReader(stream);

        var len = memoryStream.ReadUInt64();
        if( len > _maxMessageSize ) {
            throw new Exception("Invalid message lenght");
        }
        var buffer = memoryStream.ReadBytes((int)len);
        string xmlString = Encoding.UTF8.GetString(buffer);
        var xmlReader = XmlReader.Create(xmlString);
        var message = xmlReader.ReadContentAs(typeof(SimpleXMLProtocol), null) as SimpleXMLProtocolMessage;
        return ValueTask.FromResult(message);
    }

    public ValueTask Encode(PipeWriter writer, SimpleXMLProtocolMessage message, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
}

//типа мой простой протокол
public class SimpleXMLProtocolMessage {
    public string MessageType { get; set; }
    public string Payload { get; set; }
}