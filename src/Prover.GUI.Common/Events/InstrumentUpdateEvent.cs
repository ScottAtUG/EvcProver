using Prover.Core.VerificationTests.Rotary;

namespace Prover.Client.Events
{
    public class InstrumentUpdateEvent
    {
        public InstrumentUpdateEvent(RotaryTestManager instrumentManager)
        {
            InstrumentManager = instrumentManager;
        }

        public RotaryTestManager InstrumentManager { get; set; }
    }
}