using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DesignByContract2.Injectors;
using NMock2;
using NUnit.Framework;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.DesignByContract2.Tests
{
    [TestFixture]
    public class ContractInjectorTests : BaseFixture 
    {
        private string location = Path.GetDirectoryName(typeof(IContractLoader).Assembly.Location);
        private IContainer container;
        private Loader loader;
        protected override void OnInit()
        {
            container = new SimpleContainer();
            loader = new Loader(container);            
        }
        protected override void OnTerm()
        {
            container = null;
            loader = null;
        }
        [Test]
        public void ShouldGetContractInjectorInstance()
        {            
            loader.LoadDirectory(location, "LinFu.DesignByContract2.Injectors.dll");

            ITypeInjector injector = container.GetService<ITypeInjector>();
            Assert.IsNotNull(injector);
        }

        [Test]
        public void ShouldGetContractLoaderInstance()
        {            
            loader.LoadDirectory(location, "LinFu.DesignByContract2.Injectors.dll");
            
            IContractLoader contractLoader = container.GetService<IContractLoader>();
            Assert.IsNotNull(contractLoader);            
        }

        [Test]
        [ExpectedException(typeof(PreconditionViolationException))]
        public void ShouldAutoLoadExampleContract()
        {
            // Load the container itself
            string baseDirectory = Path.GetDirectoryName(typeof (ContractInjectorTests).Assembly.Location);
            loader.LoadDirectory(location, "LinFu.DesignByContract2.Injectors.dll");
            loader.LoadDirectory(baseDirectory, "*.dll");
            
            // Load the contract for the IDbConnection instance (IDbConnectionContract)
            IContractLoader contractLoader = container.GetService<IContractLoader>();
            Assert.IsNotNull(contractLoader);
            contractLoader.LoadDirectory(baseDirectory, "*.dll");
            contractLoader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            
            IDbConnection connection = new SqlConnection();            
            container.AddService(connection);
            
            // Grab the contract-injected instance of the connection
            IDbConnection checkedConnection = container.GetService<IDbConnection>();
            
            
            Stopwatch timer = Stopwatch.StartNew();
            try 
            {
                // This call should fail
                checkedConnection.Open();
            }
            catch(Exception ex)
            {
                timer.Stop();
                Console.WriteLine("Time elapsed: {0} ms", timer.ElapsedMilliseconds);
                throw ex;
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldNotLoadExampleContract()
        {
            // Load the container itself
            string baseDirectory = Path.GetDirectoryName(typeof(ContractInjectorTests).Assembly.Location);
            loader.LoadDirectory(location, "LinFu.DesignByContract2.Injectors.dll");
            //loader.LoadDirectory(baseDirectory, "*.dll");

            IDbConnection connection = new SqlConnection();
            container.AddService(connection);

            // Get the connection instance from the container
            // NOTE: This should be the same instance as the one that was provided
            // to the container
            IDbConnection checkedConnection = container.GetService<IDbConnection>();
            Assert.AreSame(connection, checkedConnection);
            
            checkedConnection.Open();
        }
    }
}
