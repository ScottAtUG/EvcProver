using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Verification.PTZ
{
    public class PVerification : PTZBase
    {
        public enum PressureUnits
        {
            PSIG = 0,
            PSIA = 1,
            kPa = 2,
            mPa = 3,
            BAR = 4,
            mBAR = 5,
            KGcm2 = 6,
            inWC = 7,
            inHG = 8,
            mmHG = 9
        }

        public enum TransducerType
        {
            Gauge = 0,
            Absolute = 1
        }

        private int GAS_PRESSURE = 8;
        private int PRESSURE_FACTOR = 44;
        private int UNSQR_FACTOR = 47;
        private int PRESS_UNITS = 87;
        private int BASE_PRESS = 13;
        private int ATM_PRESS = 14;
        private int PRESS_RANGE = 137;
        private int TRANSDUCER_TYPE = 112;

        public PVerification(Instrument instrument, TestLevel level) : 
            base(instrument, level)
        {
            Items = Instrument.Items.CopyItemsByFilter(x => x.IsPressureTest == true);
            SetDefaultGauge(level);
        }

        public decimal? GasGauge { get; set; }
        public decimal? AtmosphericGauge { get; set; }

        public override decimal? ActualFactor
        {
            get
            {
                if (EvcBase == 0) return 0;

                decimal? result;

                switch (TransType)
                {
                    case TransducerType.Gauge:
                        result = (GasGauge + EvcAtmospheric) / EvcBase;
                        break;

                    case TransducerType.Absolute:
                        result = (GasGauge + AtmosphericGauge) / EvcBase;
                        break;
                    default:
                        result = 0;
                        break;
                }

                return decimal.Round(result.Value, 4);
            }
        }


        #region Not Mapped Properties
        [NotMapped]
        public decimal? EvcGasPressure
        {
            get
            {
                return Items.GetItem(GAS_PRESSURE).GetNumericValue();
            }
        }

        [NotMapped]
        public override decimal? EvcFactor
        {
            get
            {
                return Items.GetItem(PRESSURE_FACTOR).GetNumericValue();
            }
        }

        [NotMapped]
        public decimal? EvcUnsqrFactor
        {
            get
            {
                return Instrument.Items.GetItem(UNSQR_FACTOR).GetNumericValue();
            }
        }

        [NotMapped]
        public string Units
        {
            get { return Instrument.Items.GetItem(PRESS_UNITS).GetDescriptionValue(); }
        }

        [NotMapped]
        public decimal? EvcBase
        {
            get
            {
                return Items.GetItem(BASE_PRESS).GetNumericValue();
            }
        }

        [NotMapped]
        public decimal? EvcAtmospheric
        {
            get
            {
                return Items.GetItem(ATM_PRESS).GetNumericValue();
            }
        }

        [NotMapped]
        public decimal? EvcPressureRange
        {
            get
            {
                return Items.GetItem(PRESS_RANGE).GetNumericValue();
            }
        }

        [NotMapped]
        public TransducerType TransType
        {
            get
            {
                return (TransducerType)Items.GetItem(TRANSDUCER_TYPE).GetNumericValue();
            }
        }
        #endregion

        public void SetDefaultGauge(TestLevel level)
        {
            int percent;

            switch (level)
            {
                case TestLevel.One:
                    percent = 20;
                    break;
                case TestLevel.Two:
                    percent = 50;
                    break;
                case TestLevel.Three:
                    percent = 80;
                    break;
                default:
                    percent = 0;
                    break;
            }

            GasGauge = ((decimal)percent / 100) * EvcPressureRange;
        }
    }
}
