﻿using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Events;
using Prover.GUI.Events;
using System.Windows;
using System;
using Prover.Core.VerificationManagers;

namespace Prover.GUI.ViewModels.TemperatureViews
{
    public class LiveTemperatureReadViewModel : ReactiveScreen, IHandle<LiveReadEvent>, IHandle<InstrumentUpdateEvent>
    {
        private readonly IUnityContainer _container;
        private TestManager _instrumentManager;

        public LiveTemperatureReadViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public decimal LiveReadTemperature { get; set; }

        public async Task StartLiveReadCommand()
        {
            try
            {
                await _instrumentManager?.StartLiveRead(26);
            }
            catch(Exception ex)
            {  
                MessageBox.Show("An error occured communicating with the instrument." + Environment.NewLine 
                    + ex.Message, 
                    "Error",
                    MessageBoxButton.OK);
            }
        }

        public async Task StopLiveReadCommand()
        {
            await _instrumentManager.StopLiveRead();
        }

        public void Handle(LiveReadEvent message)
        {
            LiveReadTemperature = message.LiveReadValue;
            NotifyOfPropertyChange(() => LiveReadTemperature);
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            _instrumentManager = message.InstrumentManager;
        }
    }
}
