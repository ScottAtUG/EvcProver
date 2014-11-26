using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.MiSerialProtocol.Ports
{
    public interface IPort
    {
        bool IsOpen();
        void OpenPort();
        void ClosePort();
        void Dispose();
        int BytesToRead();
        string Read();
        string Write(string data);
        void DiscardInBuffer();
    }
}
