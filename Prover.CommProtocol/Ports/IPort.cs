namespace Prover.InstrumentCommunication.Ports
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
