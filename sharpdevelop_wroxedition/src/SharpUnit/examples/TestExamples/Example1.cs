using System.Threading;

using ICSharpCode.SharpUnit;

namespace ICSharpCode.SharpUnit.Examples {
	
	[TestSuiteAttribute("This is a test testsuite")]
	public class TestTest
	{
		[TestMethodAttribute("Bla blub method 1")]
		public void Method1()
		{
			Thread.Sleep(600);
		}
		
		[TestMethodAttribute("Bla blub method 2")]
		void Method2()
		{
			Thread.Sleep(600);
		}
		
		[TestMethodAttribute("Fail this method")]
		protected void Method3()
		{
			Thread.Sleep(600);
			Assertion.Assert(false);
		}
	}
}
