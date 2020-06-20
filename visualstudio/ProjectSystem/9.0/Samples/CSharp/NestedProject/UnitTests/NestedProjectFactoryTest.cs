/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;

namespace Microsoft.VisualStudio.Project.Samples.NestedProject.UnitTests
{
	/// <summary>
	///This is a test class for VisualStudio.Project.Samples.NestedProject.NestedProjectFactory and is intended
	///to contain all VisualStudio.Project.Samples.NestedProject.NestedProjectFactory Unit Tests
	///</summary>
	[TestClass()]
	public class NestedProjectFactoryTest
	{
		#region Fields
		private TestContext testContextInstance;
		private NestedProjectPackage projectPackage;
		private Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider serviceProvider;
		#endregion

		#region Properties
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#endregion

		#region Initialization && Cleanup
		//
		//Use TestInitialize to run code before running each test
		//
		[TestInitialize()]
		public void TestInitialize()
		{
			// Create a basic service provider with basic site support
			serviceProvider = Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider.CreateOleServiceProviderWithBasicServices();
			AddBasicSiteSupport(serviceProvider);

			// initialize new NestedProjectPackage and SetSite for it.
			projectPackage = new NestedProjectPackage();
			((IVsPackage)projectPackage).SetSite(serviceProvider);
		}

		/// <summary>
		/// Runs after the test has run and to free resources obtained 
		/// by all the tests in the test class.
		/// </summary>
		//[TestCleanup()]
		public void TestCleanup()
		{
			((IVsPackage)projectPackage).SetSite(null);
			serviceProvider.Dispose();
		}
		#endregion

		#region Test Methods
		/// <summary>
		///A test for CreateProject ()
		///</summary>
		[TestMethod()]
		public void CreateProjectTest()
		{
			NestedProjectFactory target = new NestedProjectFactory(projectPackage);

			VisualStudio_Project_Samples_NestedProjectFactoryAccessor accessor = new VisualStudio_Project_Samples_NestedProjectFactoryAccessor(target);

			ProjectNode actual = accessor.CreateProject();
			Assert.IsNotNull(actual, "CreateProject did not return the expected value.");
		}

		/// <summary>
		///A test for NestedProjectFactory (NestedProjectPackage)
		///</summary>
		[TestMethod()]
		public void ConstructorTest()
		{
			NestedProjectFactory target = new NestedProjectFactory(projectPackage);
			Assert.IsNotNull(target, "New instance if NestedProjectFactory  type was not correctly initialized.");
		}

		#endregion

		#region Service functions
		public static void AddBasicSiteSupport(Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider serviceProvider)
		{
			if(serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}

			// Add solution Support
			BaseMock solution = MockServicesProvider.GetSolutionFactoryInstance();
			serviceProvider.AddService(typeof(IVsSolution), solution, false);

			//Add site support for ILocalRegistry
			BaseMock localRegistry = MockServicesProvider.GetLocalRegistryInstance();
			serviceProvider.AddService(typeof(SLocalRegistry), (ILocalRegistry)localRegistry, false);

			// Add site support for UI Shell
			BaseMock uiShell = MockServicesProvider.GetUiShellInstance0();
			serviceProvider.AddService(typeof(SVsUIShell), uiShell, false);
			serviceProvider.AddService(typeof(SVsUIShellOpenDocument), (IVsUIShellOpenDocument)uiShell, false);

			// Add site support for RegisterProjectTypes
			BaseMock mock = MockServicesProvider.GetRegisterProjectInstance();
			serviceProvider.AddService(typeof(SVsRegisterProjectTypes), mock, false);
		}

		#endregion Service functions
	}
}
