using Newtonsoft.Json;
using Prover.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Verification.Volume
{
    public abstract class VolumeVerification : DomainTable
    {
        const decimal COR_ERROR_THRESHOLD = 1.5m;
        const decimal UNCOR_ERROR_THRESHOLD = 0.1m;
        const int COR_VOLUME = 0;
        const int COR_VOLUME_HIGH_RES = 113;
        const int UNCOR_VOL = 2;
        const int UNCOR_VOL_HIGHRES = 892;
        const int PULSER_A = 93;
        const int PULSER_B = 94;

        public static VolumeVerification Create(Instrument instrument, LevelVerification levelVerification)
        {
            if (instrument.Items.GetItem(98).GetDescriptionValue() == "Rotary")
            {
                if (levelVerification is TOnlyLevelVerification)
                    return new RotaryTemperatureOnlyVolume(instrument, (TOnlyLevelVerification)levelVerification);

                if (levelVerification is POnlyLevelVerification)
                    return new RotaryPressureOnlyVolume(instrument, (POnlyLevelVerification)levelVerification);

                if (levelVerification is PTZLevelVerification)
                    return new RotaryPressureTemperatureVolume(instrument, (PTZLevelVerification)levelVerification);
            }

            throw new NotImplementedException("Only Rotary drives are supported.");
        }

        protected VolumeVerification(Instrument instrument, LevelVerification ptzLevelVerification)
        {
            Items = instrument.Items.CopyItemsByFilter(i => i.IsVolume == true);
            AfterTestItems = Instrument.Items.CopyItemsByFilter(x => x.IsVolumeTest == true);

            Instrument = instrument;
            InstrumentId = Instrument.Id;

            PTZLevelVerification = ptzLevelVerification;
        }

        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }

        public double AppliedInput { get; set; }

        public Guid InstrumentId { get; set; }
        [Required]
        public Instrument Instrument { get; set; }

        public LevelVerification PTZLevelVerification { get; set; }
        public string TestInstrumentData
        {
            get { return JsonConvert.SerializeObject(AfterTestItems.InstrumentValues); }
            set
            {
                Items.InstrumentValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(value);
            }
        }

        #region Not Mapped

        [NotMapped]
        public string DriveRateDescription => Items.GetItem(98).GetDescriptionValue();

        [NotMapped]
        public InstrumentItems AfterTestItems { get; set; }

        [NotMapped]
        public string PulseASelect => Items.GetItem(PULSER_A).GetDescriptionValue();

        [NotMapped]
        public string PulseBSelect => Items.GetItem(PULSER_B).GetDescriptionValue();

        [NotMapped]
        public int UncPulseCount
        {
            get
            {
                if (PulseASelect == "UncVol")
                    return PulseACount;

                return PulseBCount;
            }
        }

        [NotMapped]
        public int CorPulseCount
        {
            get
            {
                if (PulseASelect == "CorVol")
                    return PulseACount;

                return PulseBCount;
            }
        }

        [NotMapped]
        public decimal? EvcCorrected
        {
            get { return Math.Round((decimal)((EndCorrected - StartCorrected) * CorrectedMultiplier), 4); }
        }

        [NotMapped]
        public decimal? EvcUncorrected
        {
            get { return Math.Round((decimal)((EndUncorrected - StartUncorrected) * UnCorrectedMultiplier), 4); }
        }

        [NotMapped]
        public decimal? StartCorrected
        {
            get { return GetHighResolutionValue(Items, COR_VOLUME, COR_VOLUME_HIGH_RES); }
        }

        [NotMapped]
        public decimal? StartUncorrected
        {
            get
            {
                return GetHighResolutionValue(Items, UNCOR_VOL, UNCOR_VOL_HIGHRES);
            }
        }

        [NotMapped]
        public decimal? EndCorrected
        {
            get
            {
                if (AfterTestItems == null)
                    return StartCorrected;

                return GetHighResolutionValue(AfterTestItems, COR_VOLUME, COR_VOLUME_HIGH_RES);
            }
        }

        [NotMapped]
        public decimal? EndUncorrected
        {
            get
            {
                return GetHighResolutionValue(AfterTestItems, UNCOR_VOL, UNCOR_VOL_HIGHRES);
            }
        }

        [NotMapped]
        public decimal? UnCorrectedPercentError
        {
            get
            {
                if (UnCorrectedInputVolume != 0 && UnCorrectedInputVolume != null)
                {
                    return Math.Round((decimal)(((EvcUncorrected - UnCorrectedInputVolume) / UnCorrectedInputVolume) * 100), 2);
                }
                return null;
            }
        }

        [NotMapped]
        public decimal? CorrectedPercentError
        {
            get
            {
                if (TrueCorrected != 0)
                {
                    return Math.Round((decimal)(((EvcCorrected - TrueCorrected) / TrueCorrected) * 100), 2);
                }
                return null;
            }
        }

        [NotMapped]
        public decimal? CorrectedMultiplier
        {
            get { return Items.GetItem(90).GetNumericValue(); }
        }

        [NotMapped]
        public string CorrectedMultiplierDescription
        {
            get { return Items.GetItem(90).GetDescriptionValue(); }
        }

        [NotMapped]
        public decimal? UnCorrectedMultiplier
        {
            get { return Items.GetItem(92).GetNumericValue(); }
        }

        [NotMapped]
        public string UnCorrectedMultiplierDescription
        {
            get { return Items.GetItem(92).GetDescriptionValue(); }
        }

        [NotMapped]
        public bool CorrectedHasPassed
        {
            get { return CorrectedPercentError.IsBetween(COR_ERROR_THRESHOLD); }
        }

        [NotMapped]
        public bool UnCorrectedHasPassed
        {
            get { return (UnCorrectedPercentError.IsBetween(UNCOR_ERROR_THRESHOLD)); }
        }


        [NotMapped]
        public abstract decimal TrueCorrected { get; }

        [NotMapped]
        public abstract decimal? UnCorrectedInputVolume { get; }

        [NotMapped]
        public abstract bool HasPassed { get; }

        #endregion

        public static decimal GetHighResolutionValue(InstrumentItems items, int lowResItemNumber, int highResItemNumber)
        {
            return JoinLowResHighResReading(items.GetItem(lowResItemNumber).GetNumericValue(), items.GetItem(highResItemNumber).GetNumericValue());
        }

        public static decimal GetHighResFractionalValue(decimal highResValue)
        {
            if (highResValue == 0) return 0;

            var highResString = Convert.ToString(highResValue);
            var pointLocation = highResString.IndexOf(".", StringComparison.Ordinal);

            if (highResValue > 0 && pointLocation > -1)
            {
                var result = highResString.Substring(pointLocation, highResString.Length - pointLocation);

                return Convert.ToDecimal(result);
            }

            return 0;
        }

        public static decimal GetHighResolutionItemValue(int lowResValue, decimal highResValue)
        {
            var fractional = GetHighResFractionalValue(highResValue);
            return lowResValue + fractional;
        }

        public static decimal JoinLowResHighResReading(decimal? lowResValue, decimal? highResValue)
        {
            if (!lowResValue.HasValue || !highResValue.HasValue)
                throw new ArgumentNullException(nameof(lowResValue) + " & " + nameof(highResValue));

            return GetHighResolutionItemValue((int)lowResValue.Value, highResValue.Value);
        }
    }
}
