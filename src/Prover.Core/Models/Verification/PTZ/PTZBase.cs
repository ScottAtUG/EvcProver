using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Verification.PTZ
{
    public abstract class PTZBase : DomainTable
    {
        protected PTZBase(Instrument instrument, TestLevel level)
        {
            Instrument = instrument;
        }

        public virtual decimal? PercentError
        {
            get
            {
                if (EvcFactor == null) return null;
                return Math.Round((decimal)((EvcFactor - ActualFactor) / ActualFactor) * 100, 2);
            }
        }

        [NotMapped]
        public TestLevel TestLevel { get; set; }
        [NotMapped]
        public Instrument Instrument { get; set; }
        [NotMapped]
        public virtual bool HasPassed
        {
            get { return (PercentError < 1 && PercentError > -1); }
        }

        public abstract decimal? ActualFactor { get; }
        
        public abstract decimal? EvcFactor { get; }
    }
}
