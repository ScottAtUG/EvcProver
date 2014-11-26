using System;

namespace Prover.InstrumentCommunication.Ports
{
    public class SerialPort : IPort
    {
        public bool IsOpen()
        {
            throw new NotImplementedException();
        }

        public void OpenPort()
        {
            throw new NotImplementedException();
        }

        public void ClosePort()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int BytesToRead()
        {
            throw new NotImplementedException();
        }

        public string Read()
        {
            throw new NotImplementedException();
        }

        public string Write(string data)
        {
            throw new NotImplementedException();
        }

        public void DiscardInBuffer()
        {
            throw new NotImplementedException();
        }
    }
}
