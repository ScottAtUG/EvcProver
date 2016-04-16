﻿using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Events;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews
{
    public class PTVerificationSetViewModel : ReactiveScreen
    {
        private IUnityContainer _container;
        private RotaryTestManager _testManager;

        public PTVerificationSetViewModel(IUnityContainer container, Instrument.VerificationTest verificationTest)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            _testManager = _container.Resolve<RotaryTestManager>();
            VerificationTest = verificationTest;
            CreateViews();
        }

        private void CreateViews()
        {
            if (_testManager.Instrument.CorrectorType == CorrectorType.PressureTemperature)
            {
                TemperatureTestViewModel = new TemperatureTestViewModel(_container, VerificationTest.TemperatureTest);
                PressureTestViewModel = new PressureTestViewModel(_container, VerificationTest.PressureTest);
                SuperFactorTestViewModel = new SuperTestViewModel(_container, VerificationTest.SuperTest);
            }                

            if (_testManager.Instrument.CorrectorType == CorrectorType.TemperatureOnly)
            {
                TemperatureTestViewModel = new TemperatureTestViewModel(_container, VerificationTest.TemperatureTest);
            }

            if (_testManager.Instrument.CorrectorType == CorrectorType.PressureOnly)
            {
                PressureTestViewModel = new PressureTestViewModel(_container, VerificationTest.PressureTest);
            }
        }

        public string Level => string.Format("Level {0}", VerificationTest.TestNumber + 1);

        public TemperatureTestViewModel TemperatureTestViewModel { get; private set; }
        public PressureTestViewModel PressureTestViewModel { get; private set; }
        public SuperTestViewModel SuperFactorTestViewModel { get; private set; }
        public Instrument.VerificationTest VerificationTest { get; private set; }

        public async Task DownloadItems()
        {
            await _testManager.DownloadVerificationTestItems(VerificationTest.TestNumber);
            _container.Resolve<IEventAggregator>().PublishOnUIThread(VerificationTestEvent.Raise());
        }
    }
}