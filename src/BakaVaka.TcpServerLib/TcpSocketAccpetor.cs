using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib;
public class TcpSocketAccpetor : IAcceptor
{
    public Task<IConnection> Accept()
    {
        throw new NotImplementedException();
    }
}
