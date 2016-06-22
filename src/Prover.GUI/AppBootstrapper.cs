using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Client;
using Prover.GUI.Screens.Shell;
using System.Windows;

namespace Prover.GUI
{
    public class AppBootstrapper : AppBootstrapperBase
    {
        protected override void RegisterTypes(IUnityContainer container)
        {
            //Register Types with Unity
            container.RegisterType<IWindowManager, WindowManager>(new ContainerControlledLifetimeManager());
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}