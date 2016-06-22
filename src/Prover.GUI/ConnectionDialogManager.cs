using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Client;
using Prover.CommProtocol.Common.Events;
using Prover.GUI.Dialogs;
using PubSub;
using System.Threading;

namespace Prover.GUI
{
    public class ConnectionDialogManager
    {
        private readonly IUnityContainer _container;
        private readonly ConnectionViewModel _viewModel;
        private bool _isDialogOpen;

        public ConnectionDialogManager(IUnityContainer container)
        {
            _container = container;
                
            _viewModel = new ConnectionViewModel(_container);
        }

        public void HandleEvent(ConnectionStatusEvent message)
        {
            OpenConnectDialog();

            if (message.ConnectionStatus == ConnectionStatusEvent.Status.Disconnected)
            {
                Thread.Sleep(1000);
                _viewModel.TryClose();
                _isDialogOpen = false;
            }
        }

        private void OpenConnectDialog()
        {
            if (!_isDialogOpen)
            {
                ScreenManager.Show(_container, _viewModel);
                _isDialogOpen = true;
            }
        }
    }
}