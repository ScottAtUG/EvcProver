using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prover.InstrumentCommunication.Tests
{
    [TestClass]
    public class Connection
    {
        [TestMethod]
        public void Connect()
        {
            var port = new Ports.SerialPort("COM3", BaudRate.b38400);
            var comm = new InstrumentCommunication.Communication(port, InstrumentTypeCode.MiniMax);
            comm.Connect();
        }
    }
}
