﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Messaging;

namespace Prover.CommProtocol.Common
{
    public abstract class EvcCommunicationClient : IDisposable
    {
        private const int ConnectionRetryDelayMs = 3000;
        private const int MaxConnectionAttempts = 10;
        private readonly IDisposable _receivedObservable;
        private readonly IDisposable _sentObservable;
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     A client to communicate with a wide range of EVCs
        /// </summary>
        /// <param name="commPort">Communcations interface to the device</param>
        protected EvcCommunicationClient(CommPort commPort)
        {
            CommPort = commPort;

            _receivedObservable = ResponseProcessors.MessageProcessor.ResponseObservable(CommPort.DataReceivedObservable)
                .Subscribe(msg => { Log.Debug($"[{CommPort.Name}][IN] << {ControlCharacters.Prettify(msg)}"); });

            _sentObservable =
                CommPort.DataSentObservable.Subscribe(
                    msg => { Log.Debug($"[{CommPort.Name}][OUT] >> {ControlCharacters.Prettify(msg)}"); });
        }

        protected CommPort CommPort { get; set; }

        /// <summary>
        ///     Contains all the item numbers and meta data for a specific instrument type
        /// </summary>
        public virtual IEnumerable<ItemMetadata> ItemDetails { get; } = new List<ItemMetadata>();

        /// <summary>
        ///     Is this client already connected to an instrument
        /// </summary>
        public abstract bool IsConnected { get; protected set; }

        public CancellationTokenSource CancellationTokenSource { get; private set; }

        public virtual void Dispose()
        {
            Disconnect().Wait();

            _receivedObservable.Dispose();
            _sentObservable.Dispose();
            CommPort.Dispose();
        }

        private async Task ExecuteCommand(string command)
        {
            await Task.Run(() => CommPort.Send(command));
        }

        protected virtual async Task<T> ExecuteCommand<T>(CommandDefinition<T> command) where T : ResponseMessage
        {
            if (command.ResponseProcessor == null)
            {
                await ExecuteCommand(command.Command);
                return default(T);
            }

            var response = command.ResponseProcessor.ResponseObservable(CommPort.DataReceivedObservable)
                .Timeout(TimeSpan.FromSeconds(3))
                .FirstAsync()
                .PublishLast();

            using (response.Connect())
            {
                await CommPort.Send(command.Command);

                var result = await response;
                return result;
            }
        }

        /// <summary>
        ///     Establish a link with an instrument
        ///     Handles retries for failed connections
        /// </summary>
        /// <param name="retryAttempts"></param>
        /// <returns></returns>
        public async Task Connect(int retryAttempts = MaxConnectionAttempts)
        {
            var connectionAttempts = 0;

            CancellationTokenSource = new CancellationTokenSource();
            var ct = CancellationTokenSource.Token;
            
            var result = Task.Run(async () =>
            {
                while (!IsConnected)
                {
                    connectionAttempts++;
                    Log.Info($"[{CommPort.Name}] Connecting to Instrument... Attempt {connectionAttempts} of {MaxConnectionAttempts}");

                    try
                    {
                        if (!CommPort.IsOpen())
                            await CommPort.Open();

                        await ConnectToInstrument();
                    }
                    catch (Exception ex)
                    {
                        Log.Warn(ex.Message);
                    }

                    if (!IsConnected)
                    {
                        if (connectionAttempts < retryAttempts)
                        {
                            Thread.Sleep(ConnectionRetryDelayMs);
                        }
                        else
                        {
                            throw new Exception($"{CommPort.Name} Could not connect to instrument.");
                        }
                    }
                }
            }, ct);

            try
            {
                await result;
            }
            catch (AggregateException e)
            {
                foreach (var v in e.InnerExceptions)
                    Log.Warn(e.Message + " " + v.Message);
            }
            finally
            {
                CancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        ///     Establish a link with an instrument
        ///     Handles retries for failed connections
        /// </summary>
        /// <returns></returns>
        protected abstract Task ConnectToInstrument();

        /// <summary>
        ///     Disconnect the current link with the EVC, if one exists
        /// </summary>
        public abstract Task Disconnect();

        /// <summary>
        ///     Read item value from instrument
        /// </summary>
        /// <param name="itemNumber">Item number for the value to request</param>
        /// <returns></returns>
        public abstract Task<ItemValue> GetItemValue(int itemNumber);

        /// <summary>
        ///     Read a group of items from instrument
        /// </summary>
        /// <param name="itemNumbers">Item numbers for the values to request</param>
        /// <returns></returns>
        public abstract Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<int> itemNumbers);

        /// <summary>
        ///     Read a group of items from instrument
        /// </summary>
        /// <param name="itemNumbers">Item numbers for the values to request</param>
        /// <returns></returns>
        public abstract Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        public abstract Task<bool> SetItemValue(int itemNumber, string value);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        public abstract Task<bool> SetItemValue(int itemNumber, decimal value);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        public abstract Task<bool> SetItemValue(int itemNumber, long value);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract Task<bool> SetItemValue(string itemCode, long value);

        /// <summary>
        ///     Live read item values
        ///     Gas Temp / Gas Pressure
        /// </summary>
        /// <param name="itemNumber">Item number to live read</param>
        /// <returns></returns>
        public abstract Task<ItemValue> LiveReadItemValue(int itemNumber);
    }
}