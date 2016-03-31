using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Verification;

namespace Prover.GUI.ViewModels.InstrumentsList
{
    public class InstrumentTempTestViewModel : ReactiveScreen
    {
        public TVerification Test { get; set; }
        public bool IsSelected { get; set; }
        public InstrumentTempTestViewModel(TVerification test)
        {
            Test = test;
        }
    }
}
