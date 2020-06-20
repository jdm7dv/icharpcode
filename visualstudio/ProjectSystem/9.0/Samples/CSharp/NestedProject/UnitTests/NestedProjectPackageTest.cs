/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project.Samples.NestedProject.UnitTests
{
	/// <summary>
	///This is a test class for VisualStudio.Project.Samples.NestedProject.NestedProjectPackage and is intended
	///to contain all VisualStudio.Project.Samples.NestedProject.NestedProjectPackage Unit Tests
	///</summary>
	[TestClass()]
	public class NestedProjectPackageTest
	{
		#region Fields
		private TestContext testContextInstance;
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

		#region Test Methods
		[TestMethod()]
		public void CreateInstanceTest()
		{
			NestedProjectPackage package = new NestedProjectPackage();
			Assert.IsNotNull(package, "Failed to initialize an instance of NestedProjectPackage type.");
		}

		/// <summary>
		///A test for Initialize ()
		///</summary>
		[DeploymentItem("NestedProject.dll")]
		[TestMethod()]
		public void InitializeTest()
		{
			NestedProjectPackage target = new NestedProjectPackage();

			// Create a basic service provider
			serviceProvider = Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider.CreateOleServiceProviderWithBasicServices();
			AddBasicSiteSupport(serviceProvider);

			// SetSite calls Initialize intrinsically
			((IVsPackage)target).SetSite(serviceProvider);
		}

		#endregion

		#region Service function
		//Add some basic service mock objects to the service provider
		public static void AddBasicSiteSupport(Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider serviceProvider)
		{
			if(serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}

			// Add solution Support
			BaseMock solution = MockServicesProvider.GetSolutionFactoryInstance();
			serviceProvider.AddService(typeof(SVsSolution), solution, false);

			//Add site support for ILocalRegistry
			BaseMock localRegistry = MockServicesProvider.GetLocalRegistryInstance();
			serviceProvider.AddService(typeof(SLocalRegistry), (ILocalRegistry)localRegistry, false);

			// Add site support for UI Shell
			BaseMock uiShell = MockServicesProvider.GetUiShellInstance0();
			serviceProvider.AddService(typeof(SVsUIShell), uiShell, false);
			serviceProvider.AddService(typeof(SVsUIShellOpenDocument), (IVsUIShellOpenDocument)uiShell, false);

			//Add site support for Track Selection
			BaseMock trackSel = MockServicesProvider.GetTrackSelectionInstance();
			serviceProvider.AddService(typeof(STrackSelection), trackSel, false);

			//Add site support for Running Document Table
			BaseMock runningDoc = MockServicesProvider.GetRunningDocTableInstance();
			serviceProvider.AddService(typeof(SVsRunningDocumentTable), runningDoc, false);

			//Add site support for Window Frame
			BaseMock windowFrame = MockServicesProvider.GetWindowFrameInstance();
			serviceProvider.AddService(typeof(SVsWindowFrame), windowFrame, false);

			//Add site support for IVsTextManager
			BaseMock queryEditQuerySave = MockServicesProvider.GetQueryEditQuerySaveInstance();
			serviceProvider.AddService(typeof(SVsQueryEditQuerySave), queryEditQuerySave, false);

			// Add site support for RegisterProjectTypes
			BaseMock mock = MockServicesProvider.GetRegisterProjectInstance();
			serviceProvider.AddService(typeof(SVsRegisterProjectTypes), mock, false);
		}
		#endregion

	}
}
