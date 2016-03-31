using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using NLog;
using Prover.Core.Extensions;

namespace Prover.Core.Models.Verification.Volume
{
    public abstract class RotaryVolumeVerification : VolumeVerification
    {
        const decimal METER_DIS_ERROR_THRESHOLD = 1m;

        public RotaryVolumeVerification(Instrument instrument, LevelVerification verificationSet) : base(instrument, verificationSet)
        {             
            MeterIndex = MeterIndexInfo.Get((int)Instrument.Items.GetItem(432).GetNumericValue());
        }        

        public int MaxUnCorrected()
        {
            if (UnCorrectedMultiplier == 10)
                return MeterIndex.UnCorPulsesX10;

            if (UnCorrectedMultiplier == 100)
                return MeterIndex.UnCorPulsesX100;

            return 10; //Low standard number if we can't find anything
        } 

        [NotMapped]
        public string MeterTypeDescription
        {
            get { return MeterIndex.Description; }
        }

        [NotMapped]
        public string MeterType
        {
            get { return Items.GetItem(432).GetDescriptionValue(); }
        }

        [NotMapped]
        public int MeterTypeId
        {
            get { return (int)Items.GetItem(432).GetNumericValue(); }
        }

        [NotMapped]
        public decimal? EvcMeterDisplacement
        {
            get { return Items.GetItem(439).GetNumericValue(); }
        }

        [NotMapped]
        public override decimal? UnCorrectedInputVolume => (MeterDisplacement * (decimal)AppliedInput);

        [NotMapped]
        public decimal MeterDisplacement
        {
            get
            {
                if (MeterIndex != null)
                    return MeterIndex.MeterDisplacement.Value;

                return 0;
            }
        } 

        [NotMapped]
        public decimal MeterDisplacementPercentError
        {
            get
            {
                if (MeterDisplacement != 0)
                {
                    return Math.Round((decimal)(((EvcMeterDisplacement - MeterDisplacement) / MeterDisplacement) * 100), 2);
                }
                return 0;
            }
        }

        [NotMapped]
        public bool MeterDisplacementHasPassed
        {
            get { return (MeterDisplacementPercentError.IsBetween(METER_DIS_ERROR_THRESHOLD)); }
        }

        [NotMapped]
        public override bool HasPassed
        {
            get 
            {
                return CorrectedHasPassed && UnCorrectedHasPassed && MeterDisplacementHasPassed;
            }
        }

        [NotMapped]
        public MeterIndexInfo MeterIndex { get; private set; }   
    }

    public class RotaryTemperatureOnlyVolume : RotaryVolumeVerification
    {
        public RotaryTemperatureOnlyVolume(Instrument instrument, TOnlyLevelVerification verificationSet) : base(instrument, verificationSet)
        {
            TemperatureVerificationSet = verificationSet;
        }

        public TOnlyLevelVerification TemperatureVerificationSet { get; private set; }

        public override decimal TrueCorrected
        {
            get
            {
                return (decimal)(TemperatureVerificationSet.TVerification.ActualFactor * UnCorrectedInputVolume);
            }
        }
    }

    public class RotaryPressureOnlyVolume : RotaryVolumeVerification
    {
        public RotaryPressureOnlyVolume(Instrument instrument, POnlyLevelVerification verificationSet) : base(instrument, verificationSet)
        {
            PressureVerificationSet = verificationSet;
        }

        public POnlyLevelVerification PressureVerificationSet { get; private set; }

        public override decimal TrueCorrected
        {
            get
            {
                return (decimal)(PressureVerificationSet.PVerification.ActualFactor * UnCorrectedInputVolume);
            }
        }
    }

    public class RotaryPressureTemperatureVolume : RotaryVolumeVerification
    {
        public RotaryPressureTemperatureVolume(Instrument instrument, PTZLevelVerification verificationSet) : base(instrument, verificationSet)
        {
            PTVerificationSet = verificationSet;
        }

        public PTZLevelVerification PTVerificationSet { get; private set; }

        public override decimal TrueCorrected
        {
            get
            {
                return (decimal)((decimal)PTVerificationSet.PVerification.ActualFactor * PTVerificationSet.TVerification.ActualFactor * PTVerificationSet.ZVerification.SuperFactorSquared * UnCorrectedInputVolume);
                
            }
        }
    }
}
