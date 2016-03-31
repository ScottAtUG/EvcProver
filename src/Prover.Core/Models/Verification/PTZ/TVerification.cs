using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Prover.Core.Models.Verification.PTZ
{
    public class TVerification : PTZBase
    {
        private const decimal TempCorrection = 459.67m;
        private const decimal MetericTempCorrection = 273.15m;

        const int TEMP_UNITS = 89;
        const int BASE_TEMP = 34;
        const int GAUGE_TEMP = 26;
        const int FACTOR_TEMP = 45;

        public TVerification(Instrument instrument, TestLevel level) : 
            base(instrument, level)
        {
            Items = instrument.Items.CopyItemsByFilter(x => x.IsTemperatureTest == true);
            SetDefaultGauge(level);
        }

        public double Gauge { get; set; }

        public override decimal? ActualFactor
        {
            get
            {
                var result = 0.0m;
                switch (Units)
                {
                    case "K":
                        throw new NotImplementedException("Kelvin Units aren't implemented yet.");
                    case "C":
                        result = (decimal) ((MetericTempCorrection + EvcBase) / ((decimal) Gauge + MetericTempCorrection));
                        break;
                    case "R":
                        throw new NotImplementedException("Rankin Units aren't implemented yet.");
                    case "F":
                        result = (decimal) ((TempCorrection + EvcBase) / ((decimal) Gauge + TempCorrection));
                        break;
                }

                return decimal.Round(result, 2);
            }
        }
        
        private void SetDefaultGauge(TestLevel level)
        {
            switch (level)
            {
                case TestLevel.One:
                    Gauge = 90;
                    break;
                case TestLevel.Two:
                    Gauge = 60;
                    break;
                case TestLevel.Three:
                    Gauge = 32;
                    break;
            }
        }


        #region Not Mapped Properties
        [NotMapped]
        public decimal? EvcReading => Items.GetItem(GAUGE_TEMP).GetNumericValue();

        [NotMapped]
        public override decimal? EvcFactor => Items.GetItem(FACTOR_TEMP).GetNumericValue();

        [NotMapped]
        public string Range
        {
            get { return string.Format("-40 - 150 {0}", Units); }
        }

        [NotMapped]
        public string Units => Instrument.Items.GetItem(TEMP_UNITS).GetDescriptionValue();

        [NotMapped]
        public decimal? EvcBase => Instrument.Items.GetItem(BASE_TEMP).GetNumericValue();
        #endregion
    }
}
