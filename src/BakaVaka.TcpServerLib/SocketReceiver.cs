/**
 * LicenceName : BakaVaka Licence
 * Author : BakaVaka
 * email : BakaVaka@examle.com
 * 
 * Лицензионное соглашение : 
 * Весь код принадлежит его автору
 * В случае нахождения дефектов в коде - вы обязуетесь сообщить о дефекте, создав issue на странице проекта на github
 * В случае, если вы хотите использовать код - вы имеете на это право, указав авторство и клацнув звездочку на github
 * В случае, если вы хотите использовать код для своих проектов - все риски вы берете на себя.
 * Автор кода - не несет ответственности, в случае если в ходе изучения кода у вас вытекут глаза или заболит живот.
 * Все пулл реквесты должны начинаться со строк: "О, Мудрейшая, взгляни что я хочу предложить"; в противном случае - вы будете проигнорированы.
 */

using System.IO.Pipelines;
using System.Net.Sockets;

using BakaVaka.TcpServerLib.Utils;

namespace BakaVaka.TcpServerLib;
internal sealed class SocketReceiver {
    private readonly Socket _socket;
    private readonly PipeWriter _writer;


    public SocketReceiver(
        Socket socket,
        PipeWriter writer,
        ) {
        _socket = socket;
        _writer = writer;
    }
    public async Task Receive(CancellationToken cancellationToken) {
        while( !cancellationToken.IsCancellationRequested ) {
            try {
                var buffer = _writer.GetMemory(1024);
                var receivedBytes = await _socket.ReceiveAsync(buffer, cancellationToken);
                if( receivedBytes == 0 ) {
                    break;
                }
                var receivedData = buffer.Slice(0, receivedBytes);
                await _writer.WriteAsync(buffer, cancellationToken);
            }
            catch( Exception ex ) {
                break;
            }
        }
    }
}
