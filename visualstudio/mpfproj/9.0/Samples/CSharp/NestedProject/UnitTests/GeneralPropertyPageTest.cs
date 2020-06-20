/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;
using MSBuild = Microsoft.Build.BuildEngine;
using OleServiceProvider = Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider;

namespace Microsoft.VisualStudio.Project.Samples.NestedProject.UnitTests
{
	/// <summary>
	///This is a test class for VisualStudio.Project.Samples.NestedProject.GeneralPropertyPage and is intended
	///to contain all VisualStudio.Project.Samples.NestedProject.GeneralPropertyPage Unit Tests
	///</summary>
	[TestClass()]
	public class GeneralPropertyPageTest
	{
		#region Fields

		private GeneralPropertyPage generalPropertyPage;
		private NestedProjectPackage projectPackage;
		private NestedProjectFactoryFake projectFactory;
		private NesteProjectNodeFake projectNode;
		private Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider serviceProvider;
		private TestContext testContextInstance;

		private string testString;
		VisualStudio_Project_Samples_GeneralPropertyPageAccessor gppAccessor;

		private static string fullPathToProjectFile = "SampleProject.csproj";

		#endregion Fields

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
		#endregion Properties

		[ClassInitialize]
		public static void TestClassInitialize(TestContext context)
		{
			fullPathToProjectFile = Path.Combine(context.TestDeploymentDir, fullPathToProjectFile);
		}

		#region Initialization && Cleanup

		/// <summary>
		/// Runs before the test to allocate and configure resources needed 
		/// by all tests in the test class.
		/// </summary>
		[TestInitialize()]
		public void GeneralPropertyPageTestInitialize()
		{
			testString = "This is a test string";

			// Create a basic service provider
			serviceProvider = Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider.CreateOleServiceProviderWithBasicServices();
			AddBasicSiteSupport(serviceProvider);

			// Initialize GeneralPropertyPage instance
			generalPropertyPage = new GeneralPropertyPage();
			gppAccessor = new VisualStudio_Project_Samples_GeneralPropertyPageAccessor(generalPropertyPage);

			// Initialize ProjectPackage context
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
		public void GeneralPropertyPageTestCleanup()
		{
			((IVsPackage)projectPackage).SetSite(null);
			serviceProvider.Dispose();

			testString = null;
			generalPropertyPage = null;
		}

		#endregion Initialization && Cleanup


		#region Test methods
		/// <summary>
		/// The test for ApplicationIcon.
		/// AppIcon must be internally assigned and isDirty flag switched on.
		///</summary>
		[TestMethod()]
		public void ApplicationIconTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			target.ApplicationIcon = testString;

			Assert.AreEqual(testString, gppAccessor.applicationIcon, target.ApplicationIcon,
				"ApplicationIcon value was not initialized by expected value.");
			Assert.IsTrue((VSConstants.S_OK == target.IsPageDirty()),
				"IsDirty status was unexpected after changing of the property of the tested object.");
		}
		/// <summary>
		/// The test for ApplyChanges() in scenario when ProjectMgr is uninitialized.
		///</summary>
		[TestMethod()]
		public void ApplyChangesNullableProjectMgrTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			// sets indirectly projectMgr to null
			target.SetObjects(0, null);
			int actual = gppAccessor.ApplyChanges();
			Assert.IsNull(target.ProjectMgr, "ProjectMgr instance was not initialized to null as it expected.");
			Assert.AreEqual(VSConstants.E_INVALIDARG, actual, "Method ApplyChanges() was returned unexpected value in case of uninitialized project instance.");
		}
		/// <summary>
		/// The test for AssemblyName property.
		///</summary>
		[TestMethod()]
		public void AssemblyNameTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			target.AssemblyName = testString;
			Assert.AreEqual(testString, gppAccessor.assemblyName, target.ApplicationIcon,
				"AssemblyName property value was not initialized by expected value.");
			Assert.IsTrue((VSConstants.S_OK == target.IsPageDirty()), "IsDirty status was unexpected after changing of the property of the tested object.");
		}
		/// <summary>
		/// The test for GeneralPropertyPage default constructor.
		///</summary>
		[TestMethod()]
		public void ConstructorTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			target.Name = testString;
			Assert.AreEqual(testString, target.Name, target.ApplicationIcon,
				"Name property value was not initialized by expected value in GeneralPropertyPage() constructor.");
		}
		/// <summary>
		/// The test for DefaultNamespace property.
		///</summary>
		[TestMethod()]
		public void DefaultNamespaceTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			target.DefaultNamespace = testString;
			Assert.AreEqual(testString, target.DefaultNamespace, "DefaultNamespace property value was not initialized by expected value;");
			Assert.IsTrue((VSConstants.S_OK == target.IsPageDirty()), "IsDirty status was unexpected after changing of the property of the tested object.");
		}
		/// <summary>
		/// The test for GetClassName()  method.
		///</summary>
		[TestMethod()]
		public void GetClassNameTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			string expectedClassName = "Microsoft.VisualStudio.Project.Samples.NestedProject.GeneralPropertyPage";
			string actualClassName = target.GetClassName();

			Assert.AreEqual(expectedClassName, actualClassName,
				"GetClassName() method was returned unexpected Type FullName value.");
		}
		/// <summary>
		/// The test for OutputFile in case of OutputType.Exe file type.
		///</summary>
		[TestMethod()]
		public void OutputFileWithExeTypeTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			gppAccessor.outputType = OutputType.Exe;
			string expectedValue = target.AssemblyName + ".exe";

			Assert.AreEqual(expectedValue, target.OutputFile,
				"OutputFile name was initialized by unexpected value for EXE OutputType.");
		}
		/// <summary>
		/// The test for OutputFile property in case of using of OutputType.WinExe file type.
		///</summary>
		[TestMethod()]
		public void OutputFileWithWinExeTypeTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			gppAccessor.outputType = OutputType.WinExe;
			string expectedValue = target.AssemblyName + ".exe";

			Assert.AreEqual(expectedValue, target.OutputFile,
				"OutputFile name was initialized by unexpected value for WINEXE OutputType.");
		}
		/// <summary>
		/// The test for OutputFile in case of using of OutputType.Library file type.
		///</summary>
		[TestMethod()]
		public void OutputFileWithLibraryTypeTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			gppAccessor.outputType = OutputType.Library;
			string expectedValue = target.AssemblyName + ".dll";

			Assert.AreEqual(expectedValue, target.OutputFile,
				"OutputFile name was initialized by unexpected value for Library OutputType.");
		}
		/// <summary>
		/// The test for OutputType property.
		///</summary>
		[TestMethod()]
		public void OutputTypeTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			OutputType expectedOutType = OutputType.Library;
			target.OutputType = expectedOutType;

			Assert.AreEqual(expectedOutType, target.OutputType,
				"OutputType property value was initialized by unexpected value.");
			Assert.IsTrue((VSConstants.S_OK == target.IsPageDirty()),
				"IsDirty status was unexpected after changing of the property of the tested object.");
		}
		/// <summary>
		/// The test for StartupObject property.
		///</summary>
		[TestMethod()]
		public void StartupObjectTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			target.StartupObject = testString;
			Assert.AreEqual(testString, gppAccessor.startupObject, target.StartupObject,
				"StartupObject property value was not initialized by expected value.");
			Assert.IsTrue((VSConstants.S_OK == target.IsPageDirty()),
				"IsDirty status was unexpected after changing of the property of the tested object.");
		}
		/// <summary>
		/// The test for TargetPlatform property.
		///</summary>
		[TestMethod()]
		public void TargetPlatformTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			PlatformType expectedPlatformType = PlatformType.v2;
			target.TargetPlatform = expectedPlatformType;

			Assert.AreEqual(expectedPlatformType, target.TargetPlatform,
				"TargetPlatform property value was not initialized by expected value.");
			Assert.IsTrue((VSConstants.S_OK == target.IsPageDirty()),
				"IsDirty status was unexpected after changing of the property of the tested object.");
		}

		/// <summary>
		/// The test for TargetPlatformLocation property.
		///</summary>
		[TestMethod()]
		public void TargetPlatformLocationTest()
		{
			GeneralPropertyPage target = generalPropertyPage;
			target.TargetPlatformLocation = testString;
			Assert.AreEqual(testString, target.TargetPlatformLocation,
				"TargetPlatformLocation property value was not initialized by expected value.");
			Assert.IsTrue((VSConstants.S_OK == target.IsPageDirty()),
				"IsDirty status was unexpected after changing of the property of the tested object.");
		}
		/// <summary>
		/// The test for BindProperties() method.
		///</summary>
		[TestMethod()]
		public void BindPropertiesTest()
		{
			PrepareProjectConfig();
			gppAccessor.targetPlatformLocation = null;
			gppAccessor.defaultNamespace = null;
			gppAccessor.startupObject = null;
			gppAccessor.applicationIcon = null;
			gppAccessor.assemblyName = null;
			gppAccessor.targetPlatform = PlatformType.notSpecified;

			// NOTE: For the best test result we must tests all shown below fields:
			// For this we must Load project with specified property values.
			//gppAccessor.targetPlatform
			//gppAccessor.targetPlatformLocation
			//gppAccessor.defaultNamespace
			//gppAccessor.startupObject
			//gppAccessor.applicationIcon

			gppAccessor.BindProperties();
			Assert.IsNotNull(gppAccessor.assemblyName, "The AssemblyName was not properly initialized.");
		}
		/// <summary>
		/// The test for BindProperties() method in scenario when ProjectMgr is not initialized.
		///</summary>

		[TestMethod()]
		public void BindPropertiesWithNullableProjectMgrTest()
		{
			gppAccessor.BindProperties();

			Assert.IsNull(gppAccessor.assemblyName,
				"The AssemblyName was initialized in scenario when ProjectMgr is invalid.");
		}
		/// <summary>
		/// The test for ProjectFile property.
		///</summary>
		[TestMethod()]
		public void ProjectFileTest()
		{
			PrepareProjectConfig();
			GeneralPropertyPage target = generalPropertyPage;

			// Project File Name must be equivalent with name of the currently loaded project
			Assert.AreEqual(Path.GetFileName(fullPathToProjectFile), target.ProjectFile,
				"ProjectFile property value was initialized by unexpected path value.");
		}
		/// <summary>
		///The test for ProjectFolder property.
		///</summary>
		[TestMethod()]
		public void ProjectFolderTest()
		{
			PrepareProjectConfig();
			GeneralPropertyPage target = generalPropertyPage;

			string expectedProjectFolderPath = Path.GetDirectoryName(fullPathToProjectFile);
			expectedProjectFolderPath = Path.GetDirectoryName(expectedProjectFolderPath);

			// Project Folder path must be equivalent with path of the currently loaded project
			Assert.AreEqual(expectedProjectFolderPath, target.ProjectFolder,
				"ProjectFolder property value was initialized by unexpected path value.");

		}
		/// <summary>
		/// The test for ApplyChanges() in scenario when ProjectMgr is initialized.
		///</summary>
		[TestMethod()]
		public void ApplyChangesTest()
		{
			PrepareProjectConfig();
			int actual = gppAccessor.ApplyChanges();

			Assert.AreEqual(VSConstants.S_OK, actual,
				"Method ApplyChanges() was returned unexpected value in case of initialized project instance.");
		}
		#endregion Completed test methods

		#region Service functions
		/// <summary>
		/// Add some basic service mock objects to the service provider.
		/// </summary>
		/// <param name="serviceProvider">Instance of ServiceProvider which will be 
		/// configured with the mocks.</param>
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

