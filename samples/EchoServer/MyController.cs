using BakaVaka.NetLib.Shared;

namespace EchoServer;
public class AlwaysMatch : IRouteMatcher<object, object> {
    public bool IsMatch(object a, object b) => true;
}
internal class MyController : Controller<object, object> {
    [Route<object, object>(typeof(AlwaysMatch))]
    public void Lol(IInterface a, IMyService myService) => myService.Print();
}