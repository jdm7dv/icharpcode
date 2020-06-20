using System.Threading;

using ICSharpCode.SharpUnit;

namespace ICSharpCode.SharpUnit.Examples {
	
	[TestSuiteAttribute()]
	public class TestTest2
	{
		[TestMethodAttribute()]
		public void BlaMethod1()
		{
			Thread.Sleep(600);
			Assertion.Assert("This must fail", false);
		}
		
		
		[TestMethodAttribute()]
		void BlubMethod2()
		{
			Thread.Sleep(600);
			Assertion.AssertEquals("test if 1 == 2", 1, 2);
		}
		
		[TestMethodAttribute("Fail this method")]
		protected void fwefewMethod3()
		{
			Thread.Sleep(600);
		}
	}
}
