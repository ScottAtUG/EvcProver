using System;
using System.Collections.Generic;
using Prover.CommProtocol;
using Prover.InstrumentCommunication.Ports;
using System.Reactive;

namespace Prover.InstrumentCommunication
{
    public class Communication : IDisposable
    {
        private const string AccessCode = "33333";
        private readonly IPort _port;
        private readonly Enums.InstrumentTypeCode _instrumentType;

        
        public Communication(IPort port, Enums.InstrumentTypeCode instrumentType)
        {
            _port = port;
            _instrumentType = instrumentType;
        }

        //Comm State holds the higher level functions that are currently occuring
        //Typically more important to other libraries
        public Enums.CommStateEnum CommState { get; private set; }
        
        //MessageState holds the lower level functions that are currently occuring
        //i.e. Data Packets that are being recieved and if a message is in the midst of being sent
        private Enums.MessageStateEnum MessageState { get; set; }

        public void Connect()
        {
            
        }

        public void Disconnect()
        {
            
        }

        public Enums.InstrumentErrorsEnum Write(int itemNumber, string itemValue)
        {
            
            return Enums.InstrumentErrorsEnum.NoError;
        }

        public T Read<T>(int itemNumber) where T : struct
        {
            var val = 0;
            return (T)System.Convert.ChangeType(val, Type.GetTypeCode(typeof(T)));
        }

        public Dictionary<int, string> Read(List<int> itemNumbers)
        {
            return new Dictionary<int, string>();
        }

        public Enums.InstrumentErrorsEnum ResetAlarms(List<int> alarmItems)
        {
            return Enums.InstrumentErrorsEnum.NoError;
        }

        public void LiveRead(int itemNumber, IObserver<int> liveStream)
        {
            
        }

        public void Dispose()
        {
            _port.ClosePort();
            _port.Dispose();
        }
    }
}
