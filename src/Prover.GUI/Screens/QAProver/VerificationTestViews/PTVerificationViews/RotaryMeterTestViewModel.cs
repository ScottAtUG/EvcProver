﻿using Caliburn.Micro.ReactiveUI;
using Prover.Core.DriveTypes;
using Prover.Core.Extensions;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews
{
    public class RotaryMeterTestViewModel : ReactiveScreen
    {
        public RotaryMeterTestViewModel(RotaryDrive rotaryDrive)
        {
            RotaryDriveType = rotaryDrive;
        }

        //Meter properties
        public string DriveRateDescription => RotaryDriveType.Instrument.DriveRateDescription();
        public string MeterTypeDescription => RotaryDriveType.Meter.MeterTypeDescription;
        public decimal? MeterDisplacement => RotaryDriveType.Meter.MeterDisplacement;
        public decimal? EvcMeterDisplacement => RotaryDriveType.Meter.EvcMeterDisplacement;
        public decimal? MeterDisplacementPercentError => RotaryDriveType.Meter.MeterDisplacementPercentError;
        public bool MeterDisplacementHasPassed => RotaryDriveType.HasPassed;

        public RotaryDrive RotaryDriveType { get; }
    }
}