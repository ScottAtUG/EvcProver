using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.Startup;

namespace Prover.Client
{
    public abstract class AppBootstrapperBase : BootstrapperBase
    {
        protected IUnityContainer Container;

        protected AppBootstrapperBase()
        {
            var coreBootstrap = new CoreBootstrapper();
            Container = coreBootstrap.Container;

            Initialize();
        }

        protected abstract void RegisterTypes(IUnityContainer container);

        protected override void Configure()
        {
            base.Configure();
            
            RegisterTypes(Container);
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service != null)
            {
                return Container.Resolve(service);
            }

            if (!string.IsNullOrEmpty(key))
            {
                return Container.Resolve(Type.GetType(key));
            }

            return null;
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = new List<Assembly>();
            assemblies.AddRange(base.SelectAssemblies());
            
            var fileEntries = Directory.GetFiles(Directory.GetCurrentDirectory());
            foreach (var fileName in fileEntries)
            {
                if (fileName.EndsWith("UnionGas.MASA.dll"))
                {
                    var ass = Assembly.LoadFrom(fileName);
                    assemblies.Add(ass);

                    var type = ass.GetType("UnionGas.MASA.Startup");
                    type.GetMethod("Initialize").Invoke(null, new object[] { Container });
                }
            }

            return assemblies;
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.ResolveAll(service);
        }

        protected override void BuildUp(object instance)
        {
            Container.BuildUp(instance);
        }
    }
}
