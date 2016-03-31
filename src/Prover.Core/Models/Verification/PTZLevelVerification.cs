using Prover.Core.Models.Verification.PTZ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Verification
{
    public enum TestLevel
    {
        One,
        Two,
        Three
    }

    public abstract class LevelVerification : DomainTable
    {
        protected LevelVerification(TestLevel level, Instrument instrument)
        {
            Level = level;
            Instrument = instrument;
            InstrumentId = Instrument.Id;
        }

        public Guid InstrumentId { get; private set; }
        public Instrument Instrument { get; private set; }
        public TestLevel Level { get; private set; }

        public static LevelVerification Create(TestLevel level, Instrument instrument)
        {
            switch (instrument.CorrectorType)
            {
                case CorrectorType.PressureOnly:
                    return new POnlyLevelVerification(level, instrument, new PVerification(instrument, level));
                case CorrectorType.PressureTemperature:
                    return new PTZLevelVerification(level, instrument, new TVerification(instrument, level), new PVerification(instrument, level));
                default: //TemperatureOnly is default
                    return new TOnlyLevelVerification(level, instrument, new TVerification(instrument, level));
            }
        }
    }

    public class TOnlyLevelVerification : LevelVerification
    {
        public TOnlyLevelVerification(TestLevel level, Instrument instrument, TVerification temperature) : base(level, instrument)
        {
            TVerification = temperature;
        }

        public TVerification TVerification { get; private set; }
    }

    public class POnlyLevelVerification : LevelVerification
    {
        public POnlyLevelVerification(TestLevel level, Instrument instrument, PVerification pressure) : base(level, instrument)
        {
            PVerification = pressure;
        }

        public PVerification PVerification { get; private set; }
    }

    public class PTZLevelVerification : LevelVerification
    {
        public PTZLevelVerification(TestLevel level, Instrument instrument, TVerification temperature, PVerification pressure) : base(level, instrument)
        {
            TVerification = temperature;
            PVerification = pressure;
            ZVerification = new ZVerification(instrument, level, TVerification, PVerification);
        }
        public PVerification PVerification { get; private set; }
        public ZVerification ZVerification { get; private set; }
        public TVerification TVerification { get; private set; }
    }
}
