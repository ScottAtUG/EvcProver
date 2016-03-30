using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.VerificationTests
{
    abstract class VerificationBase
    {
        protected InstrumentCommunicator _instrumentCommunicator;
        protected Instrument _instrument;

        protected VerificationBase(Instrument instrument, InstrumentCommunicator instrumentComm)
        {
            _instrument = instrument;
            _instrumentCommunicator = instrumentComm;
        }
    }

    public class VerificationTest
    {
        public VerificationTest(int level, Instrument instrument, TemperatureTest temperature, PressureTest pressure)
        {
            TestNumber = level;
            TemperatureTest = temperature;
            PressureTest = pressure;
            SuperTest = new SuperFactor(instrument, TemperatureTest, PressureTest);
        }

        public PressureTest PressureTest { get; private set; }
        public SuperFactor SuperTest { get; private set; }
        public TemperatureTest TemperatureTest { get; private set; }
        public int TestNumber { get; private set; }
    }
}
