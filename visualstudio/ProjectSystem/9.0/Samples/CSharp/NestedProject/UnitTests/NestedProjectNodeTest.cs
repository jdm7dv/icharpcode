/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using MSBuild = Microsoft.Build.BuildEngine;
using OleServiceProvider = Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider;

namespace Microsoft.VisualStudio.Project.Samples.NestedProject.UnitTests
{
	/// <summary>
	/// NestedProjectFactory fake object creates NestedProjectNode fake object
	/// </summary>
	public class NestedProjectFactoryFake : NestedProjectFactory
	{
		public NestedProjectFactoryFake(NestedProjectPackage package)
			: base(package)
		{
		}

		protected override ProjectNode CreateProject()
		{
			NesteProjectNodeFake project = new NesteProjectNodeFake();
			project.SetSite((IOleServiceProvider)((IServiceProvider)this.Package).GetService(typeof(IOleServiceProvider)));
			return project;
		}
	}

	/// <summary>
	/// This is a fake object for NestedPRojectNode so that we can skip certain method calls,
	/// e.g. ProcessRefrences involves a build and we would like to skip that
	/// </summary>
	public class NesteProjectNodeFake : NestedProjectNode
	{
		protected override void ProcessReferences()
		{
			return;
		}
	}

	/// <summary>
	///This is a test class for VisualStudio.Project.Samples.NestedProject.NestedProjectNode and is intended
	///to contain all VisualStudio.Project.Samples.NestedProject.NestedProjectNode Unit Tests
	///</summary>
	[TestClass()]
	public class NestedProjectNodeTest
	{
		#region Fields
		private TestContext testContextInstance;
		private GeneralPropertyPage generalPropertyPage;
		private Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider serviceProvider;

		private NestedProjectPackage projectPackage;
		private NestedProjectFactoryFake projectFactory;
		private NesteProjectNodeFake projectNode;

		private static string fullPathToClassTemplateFile = @"TemplateClass.cs";
		private static string fullPathToProjectFile = @"SampleProject.csproj";
		private static string fullPathToTargetFile = @"SampleClass.cs";

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

		[ClassInitialize]
		public static void TestClassInitialize(TestContext context)
		{
			fullPathToClassTemplateFile = Path.Combine(context.TestDeploymentDir, fullPathToClassTemplateFile);
			fullPathToProjectFile = Path.Combine(context.TestDeploymentDir, fullPathToProjectFile);
			fullPathToTargetFile = Path.Combine(context.TestDeploymentDir, fullPathToTargetFile);
		}

		/// <summary>
		/// Runs before the test to allocate and configure resources needed 
		/// by all tests in the test class.
		/// </summary>
		[TestInitialize()]
		public void Initialize()
		{
			generalPropertyPage = new GeneralPropertyPage();

			// Create a basic service provider
			serviceProvider = Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider.CreateOleServiceProviderWithBasicServices();
			AddBasicSiteSupport(serviceProvider);

			// Prepare the package
			projectPackage = new NestedProjectPackage();
			((IVsPackage)projectPackage).SetSite(serviceProvider);

			// prepare the factory
			projectFactory = new NestedProjectFactoryFake(projectPackage);

			//set the build engine and build project on the factory object
			FieldInfo buildEngine = typeof(ProjectFactory).GetField("buildEngine", BindingFlags.Instance | BindingFlags.NonPublic);
			buildEngine.SetValue(projectFactory, MSBuild.Engine.GlobalEngine);
			MSBuild.Project msbuildproject = MSBuild.Engine.GlobalEngine.CreateNewProject();
			FieldInfo buildProject = typeof(ProjectFactory).GetField("buildProject", BindingFlags.Instance | BindingFlags.NonPublic);
			buildProject.SetValue(projectFactory, msbuildproject);

			//Create the project object using the projectfactory and load the project
			int canCreate;
			if(VSConstants.S_OK == ((IVsProjectFactory)projectFactory).CanCreateProject(fullPathToProjectFile, 2, out canCreate))
			{
				MethodInfo preCreateForOuter = typeof(NestedProjectFactory).GetMethod("PreCreateForOuter", BindingFlags.Instance | BindingFlags.NonPublic);
				Assert.IsNotNull(preCreateForOuter, "failed to get the PreCreateForOuter method info object from NestedProjectFactory type");
				projectNode = (NesteProjectNodeFake)preCreateForOuter.Invoke(projectFactory, new object[] { IntPtr.Zero });
				Assert.IsNotNull(projectNode, "Failed to create the projectnode object");
				Guid iidProject = new Guid();
				int pfCanceled;
				projectNode.Load(fullPathToProjectFile, "", "", 2, ref iidProject, out pfCanceled);
			}
		}

		/// <summary>
		/// Runs after the test has run and to free resources obtained 
		/// by all the tests in the test class.
		/// </summary>
		//[TestCleanup()]
		public void Cleanup()
		{
			((IVsPackage)projectPackage).SetSite(null);
			serviceProvider.Dispose();

			generalPropertyPage = null;
		}

		#endregion Initialization && Cleanup

		#region Test Methods
		/// <summary>
		///A test for AddFileFromTemplate (string, string)
		///</summary>
		[TestMethod()]
		public void AddFileFromTemplateTest()
		{
			NestedProjectNode target = projectNode;
			target.AddFileFromTemplate(fullPathToClassTemplateFile, fullPathToTargetFile);
		}

		/// <summary>
		///A test for GetAutomationObject ()
		///</summary>
		[TestMethod()]
		public void GetAutomationObjectTest()
		{
			NestedProjectNode target = projectNode;

			object actual = target.GetAutomationObject();
			Assert.IsNotNull(actual, "Failed to initialize an AutomationObject for "
				+ "NestedProjectNode within GetAutomationObject method");
		}

		/// <summary>
		///A test for GetConfigurationDependentPropertyPages ()
		///</summary>
		[TestMethod()]
		public void GetConfigurationDependentPropertyPagesTest()
		{
			NestedProjectNode target = projectNode;
			VisualStudio_Project_Samples_NestedProjectNodeAccessor accessor =
				new VisualStudio_Project_Samples_NestedProjectNodeAccessor(target);

			Guid[] expected = new Guid[] { new Guid("C43AD3DC-7468-48e1-B4D2-AAC0C74A0109") };
			Guid[] actual;

			actual = accessor.GetConfigurationDependentPropertyPages();

			CollectionAssert.AreEqual(expected, actual, "Microsoft.VisualStudio.Project.Samples.NestedProject.NestedProjectNode.GetConfigurationDepe" +
					"ndentPropertyPages did not return the expected value.");
		}

		/// <summary>
		///A test for GetConfigurationIndependentPropertyPages ()
		///</summary>
		[TestMethod()]
		public void GetConfigurationIndependentPropertyPagesTest()
		{
			NestedProjectNode target = new NestedProjectNode();
			VisualStudio_Project_Samples_NestedProjectNodeAccessor accessor =
				new VisualStudio_Project_Samples_NestedProjectNodeAccessor(target);


			Guid[] actual;
			actual = accessor.GetConfigurationIndependentPropertyPages();

			Assert.IsTrue(actual != null && actual.Length > 0, "The result of GetConfigurationIndependentPropertyPages was unexpected.");
			Assert.IsTrue(actual[0].Equals(typeof(GeneralPropertyPage).GUID), "The value of collection returned by GetConfigurationIndependentPropertyPages is unexpected.");
		}

		/// <summary>
		///A test for GetFormatList (out string)
		///</summary>
		[TestMethod()]
		public void GetFormatListTest()
		{
			NestedProjectNode target = new NestedProjectNode();

			string ppszFormatList;

			int expected = VSConstants.S_OK;
			int actual;

			actual = target.GetFormatList(out ppszFormatList);

			Assert.IsFalse(String.IsNullOrEmpty(ppszFormatList), "[out] ppszFormatList in GetFormatList() method was not set correctly.");
			Assert.AreEqual(expected, actual, "Microsoft.VisualStudio.Project.Samples.NestedProject.NestedProjectNode.GetFormatList did no" +
					"t return the expected value.");
		}

		/// <summary>
		///A test for GetPriorityProjectDesignerPages ()
		///</summary>
		[TestMethod()]
		public void GetPriorityProjectDesignerPagesTest()
		{
			NestedProjectNode target = new NestedProjectNode();
			VisualStudio_Project_Samples_NestedProjectNodeAccessor accessor =
				new VisualStudio_Project_Samples_NestedProjectNodeAccessor(target);


			Guid[] actual;
			actual = accessor.GetPriorityProjectDesignerPages();

			Assert.IsTrue(actual != null && actual.Length > 0, "The result of GetConfigurationIndependentPropertyPages was unexpected.");
			Assert.IsTrue(actual[0].Equals(typeof(GeneralPropertyPage).GUID), "The value of collection returned by GetConfigurationIndependentPropertyPages is unexpected.");
		}

		/// <summary>
		///A test for NestedProjectNode ()
		///</summary>
		[TestMethod()]
		public void ConstructorTest()
		{
			NestedProjectNode target = new NestedProjectNode();
			Assert.IsNotNull(target, "Failed to initialize new instance of NestedProjectNode");
		}

		/// <summary>
		///A test for ProjectGuid
		///</summary>
		[TestMethod()]
		public void ProjectGuidTest()
		{
			NestedProjectNode target = projectNode;

			Guid val = new Guid(GuidStrings.GuidNestedProjectFactory);

			Assert.AreEqual(val, target.ProjectGuid, "NestedProjectNode.ProjectGuid was not set correctly.");
		}

		/// <summary>
		///A test for ProjectType
		///</summary>
		[TestMethod()]
		public void ProjectTypeTest()
		{
			NestedProjectNode target = new NestedProjectNode();

			string val = typeof(NestedProjectNode).Name;

			Assert.AreEqual(val, target.ProjectType, "Microsoft.VisualStudio.Project.Samples.NestedProject.NestedProjectNode.ProjectType was not " +
					"set correctly.");
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

			// Add site support for VsShell
			BaseMock vsShell = MockServicesProvider.GetVsShellInstance0();
			serviceProvider.AddService(typeof(SVsShell), vsShell, false);

			// Add site support for SolutionBuildManager service
			BaseMock solutionBuildManager = MockServicesProvider.GetSolutionBuildManagerInstance0();
			serviceProvider.AddService(typeof(SVsSolutionBuildManager), solutionBuildManager, false);

		}

		/// <summary>
		/// Initialize ProjectConfig and internal projectMgr objects.
		/// </summary>
		/// <remarks>Service function. Before calling this function projectNode must be 
		/// initialized by valid project data.</remarks>
		protected void PrepareProjectConfig()
		{
			object[] ppUnk = new object[2];
			ProjectConfig pjc = new ProjectConfig(projectNode, "manualSetConfigArgument");
			ppUnk[0] = pjc;
			generalPropertyPage.SetObjects(1, ppUnk);
		}
		#endregion Service functions
	}
}
