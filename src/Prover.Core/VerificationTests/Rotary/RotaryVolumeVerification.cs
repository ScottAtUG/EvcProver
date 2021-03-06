﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Communication;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.Rotary
{
    public sealed class RotaryVolumeVerification : VolumeVerificationManager
    {
        private readonly IDInOutBoard _outputBoard;
        private readonly TachometerCommunicator _tachometerCommunicator;

        public RotaryVolumeVerification(IEventAggregator eventAggregator, VolumeTest volumeTest,
            EvcCommunicationClient instrumentComm, TachometerCommunicator tachComm)
            : base(eventAggregator, volumeTest, instrumentComm)
        {
            _tachometerCommunicator = tachComm;
            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
        }

        public override async Task StartVolumeTest()
        {
            if (!RunningTest)
            {
                await RunSyncTest();
                
                if (_tachometerCommunicator != null)
                    await _tachometerCommunicator?.ResetTach();

                await InstrumentCommunicator.Connect();
                VolumeTest.Items = await InstrumentCommunicator.GetItemValues(InstrumentCommunicator.ItemDetails.VolumeItems());
                await InstrumentCommunicator.Disconnect();

                await RunningVolumeTest();
            }
        }

        public override async Task RunningVolumeTest()
        {
            await Task.Run(async () =>
            {
                do
                {
                    //TODO: Raise events so the UI can respond
                    VolumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                    VolumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                } while (VolumeTest.UncPulseCount < VolumeTest.DriveType.MaxUnCorrected() && !RequestStopTest);

                _outputBoard?.StopMotor();
                await FinishVolumeTest();
            });
        }

        public override async Task FinishVolumeTest()
        {
            await Task.Run(async () =>
            {
                try
                {
                    Thread.Sleep(250);

                    VolumeTest.AfterTestItems = await InstrumentCommunicator.GetItemValues(InstrumentCommunicator.ItemDetails.VolumeItems());

                    await ZeroInstrumentVolumeItems();

                    try
                    {
                        if (_tachometerCommunicator != null)
                            VolumeTest.AppliedInput = await _tachometerCommunicator.ReadTach();
                        else
                            VolumeTest.AppliedInput = 0;

                        Log.Info($"Tachometer reading: {VolumeTest.AppliedInput}");
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"An error occured communication with the tachometer: {ex}");
                    }

                    Log.Info("Volume test finished!");
                }
                finally
                {
                    RunningTest = false;
                }
            });
        }

        private async Task RunSyncTest()
        {
            await Task.Run(async () =>
            {
                if (!RunningTest)
                {
                    Log.Info("Syncing Volume...");
                    await InstrumentCommunicator.Disconnect();

                    VolumeTest.PulseACount = 0;
                    VolumeTest.PulseBCount = 0;

                    _outputBoard.StartMotor();

                    do
                    {
                        VolumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                        VolumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                    } while (VolumeTest.UncPulseCount < 1);

                    _outputBoard.StopMotor();
                    Thread.Sleep(500);
                }
            });
        }

        protected override async Task ZeroInstrumentVolumeItems()
        {
            await InstrumentCommunicator.Connect();
            await InstrumentCommunicator.SetItemValue(264, "20140867");
            await InstrumentCommunicator.SetItemValue(434, "0");
            await InstrumentCommunicator.SetItemValue(113, "0");
            await InstrumentCommunicator.SetItemValue(892, "0");
            await base.ZeroInstrumentVolumeItems();
        }
    }
}