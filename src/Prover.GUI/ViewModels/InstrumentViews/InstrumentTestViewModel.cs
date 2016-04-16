﻿using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews;
using System.Collections.ObjectModel;
using System.Linq;

namespace Prover.GUI.ViewModels.InstrumentViews
{
    public class InstrumentTestViewModel : ReactiveScreen
    {
        protected IUnityContainer _container;

        public InstrumentTestViewModel(IUnityContainer container, Instrument instrument)
        {
            _container = container;
            Instrument = instrument;

            Instrument.VerificationTests.OrderBy(v => v.TestNumber).ForEach(x =>
                TestViews.Add(new PTVerificationSetViewModel(_container, x)));
        }

        public Instrument Instrument { get; private set; }

        #region Views
        public InstrumentInfoViewModel SiteInformationItem { get; set; }
        public ObservableCollection<PTVerificationSetViewModel> TestViews { get; set; } = new ObservableCollection<PTVerificationSetViewModel>();
        public VolumeVerificationViewModel VolumeInformationItem { get; set; }
        #endregion
    }
}