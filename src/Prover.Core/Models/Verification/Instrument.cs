using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json;
using Prover.Core.Communication;
using Prover.Core.Models.Certificates;
using Prover.SerialProtocol;
using Prover.Core.Models.Verification.Volume;

namespace Prover.Core.Models.Verification
{
    public enum InstrumentType
    {
        MiniMax = 4,
        MiniAt = 3,
        Ec300 = 7
    }

    public enum CorrectorType
    {
        TemperatureOnly,
        PressureOnly,
        PressureTemperature
    }

    public class Instrument : DomainTable
    {
        private int FIXED_PRESSURE_FACTOR = 109;
        private int FIXED_SUPER_FACTOR = 110;
        private int FIXED_TEMP_FACTOR = 111;
        private int SERIAL_NUMBER = 62;
        private InstrumentType _instrumentType;

        private Instrument()
        {
        }

        public Instrument(InstrumentType type)
        {
            Items = new InstrumentItems(type);
            Type = type;
        }

        public Instrument(InstrumentType type, InstrumentItems items) : base(items)
        {
            TestDateTime = DateTime.Now;
            Type = type;
            CertificateId = null;
            BuildCorrectorTypes();
        }

        public DateTime TestDateTime { get; set; }
        public InstrumentType Type
        {
            get
            {
                return _instrumentType;
            }
            set
            {
                if (Items == null || !Items.Items.Any())
                {
                    Items = new InstrumentItems(value);
                }
                _instrumentType = value;                                   
            }
        }
        public Guid? CertificateId { get; set; }
        public Certificate Certificate { get; set; }

        public List<LevelVerification> VerificationSets { get; set; } = new List<LevelVerification>();
        public virtual VolumeVerification Volume { get; set; }

        #region NotMapped Properties
        [NotMapped]
        public int SerialNumber
        {
            get { return (int)Items.GetItem(SERIAL_NUMBER).GetNumericValue(); }
        }

        [NotMapped]
        public string TypeString
        {
            get { return Type.ToString(); }
        }

        [NotMapped] 
        public CorrectorType CorrectorType
        {
            get
            {
                if (Items.GetItem(FIXED_PRESSURE_FACTOR).GetDescriptionValue().ToLower() == "live"
                  && Items.GetItem(FIXED_TEMP_FACTOR).GetDescriptionValue().ToLower() == "live")
                    return CorrectorType.PressureTemperature;

                if (Items.GetItem(FIXED_PRESSURE_FACTOR).GetDescriptionValue().ToLower() == "live")
                    return CorrectorType.PressureOnly;

                if (Items.GetItem(FIXED_TEMP_FACTOR).GetDescriptionValue().ToLower() == "live")
                    return CorrectorType.TemperatureOnly;

                return CorrectorType.TemperatureOnly;
            }
        }

        [NotMapped]
        public decimal FirmwareVersion
        {
            get
            {
                return Items.GetItem(122).GetNumericValue();
            }
        }

        [NotMapped]
        public decimal PulseAScaling
        {
            get { return Items.GetItem(56).GetNumericValue(); }
        }

        [NotMapped]
        public string PulseASelect
        {
            get { return Items.GetItem(93).GetDescriptionValue(); }
        }

        [NotMapped]
        public decimal PulseBScaling
        {
            get { return Items.GetItem(57).GetNumericValue(); }
        }

        [NotMapped]
        public string PulseBSelect
        {
            get { return Items.GetItem(94).GetDescriptionValue(); }
        }

        [NotMapped]
        public decimal SiteNumber1
        {
            get { return Items.GetItem(200).GetNumericValue(); }
        }

        [NotMapped]
        public decimal SiteNumber2
        {
            get { return Items.GetItem(201).GetNumericValue(); }
        }

        [NotMapped]
        public bool HasPassed
        {
            get
            {
                return false;
            }
        }
        #endregion      

        private void BuildCorrectorTypes()
        {
            VerificationSets = new List<LevelVerification>();

            VerificationSets.Add(LevelVerification.Create(TestLevel.One, this));
            VerificationSets.Add(LevelVerification.Create(TestLevel.Two, this));

            var volumeVerification = LevelVerification.Create(TestLevel.Three, this);
            VerificationSets.Add(volumeVerification);

            Volume = VolumeVerification.Create(this, volumeVerification);
        }
    }
}
