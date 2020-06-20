/*
 * AssertionTest.cs
 * Copyright (C) 2002 Mike Krueger, icsharpcode
 * Portet over from NUnit code.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice, 
 *   this list of conditions and the following disclaimer. 
 * 
 * - Redistributions in binary form must reproduce the above copyright notice, 
 *   this list of conditions and the following disclaimer in the documentation 
 *   and/or other materials provided with the distribution. 
 * 
 * - Neither the name of icsharpcode nor the names of its contributors may 
 *   be used to endorse or promote products derived from this software without specific 
 *   prior written permission. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
 * SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
 * OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
//using ICSharpCode.SharpUnit;

namespace ICSharpCode.SharpUnit.Tests {
	
	[TestSuiteAttribute()]
	public class AssertionTest
	{
		[TestMethodAttribute()]
		public void TestAssertEquals()
		{
			object o = new object();
			Assertion.AssertEquals(o, o);
		}

		[TestMethodAttribute()]
		public void TestAssertEqualsNaNFails()
		{
			try  {
				Assertion.AssertEquals(3.141, Double.NaN, 0.0);
				Assertion.AssertEquals(3.141f, Single.NaN, 0.0);
			} catch (AssertionFailedException) {
				return;
			}
			Assertion.Fail();
		}
		
		[TestMethodAttribute()]
		public void TestAssertEqualsNull() 
		{
			Assertion.AssertEquals(null, null);
		}
		
		[TestMethodAttribute()]
		public void TestAssertNanEqualsFails() 
		{
			try  {
				Assertion.AssertEquals(Double.NaN, 3.141, 0.0);
				Assertion.AssertEquals(Single.NaN, 3.141f, 0.0);
			} catch (AssertionFailedException) {
				return;
			}
			Assertion.Fail();
		}     
		
		[TestMethodAttribute()]
		public void TestAssertNanEqualsNaNFails()
		{
			try {
				Assertion.AssertEquals(Double.NaN, Double.NaN, 0.0);
				Assertion.AssertEquals(Single.NaN, Single.NaN, 0.0);
			} catch (AssertionFailedException) {
				return;
			}
			Assertion.Fail();
		}     
		
		[TestMethodAttribute()]
		public void TestAssertNegInfinityEqualsInfinity() 
		{
			Assertion.AssertEquals(Double.NegativeInfinity, Double.NegativeInfinity, 0.0);
		}
		
		[TestMethodAttribute()]
		public void TestAssertPosInfinityEqualsInfinity() 
		{
			Assertion.AssertEquals(Double.PositiveInfinity, Double.PositiveInfinity, 0.0);
		}
		
		[TestMethodAttribute()]
		public void TestAssertPosInfinityNotEquals() 
		{
			try {
				Assertion.AssertEquals(Double.PositiveInfinity, 1.23, 0.0);
			} catch (AssertionFailedException) {
				return;
			}
			Assertion.Fail();
		}
		
		[TestMethodAttribute()]
		public void TestAssertPosInfinityNotEqualsNegInfinity() 
		{
			try {
				Assertion.AssertEquals(Double.PositiveInfinity, Double.NegativeInfinity, 0.0);
			} catch (AssertionFailedException) {
				return;
			}
			Assertion.Fail();
		}

		[TestMethodAttribute()]
		public void TestAssertSinglePosInfinityNotEqualsNegInfinity() 
		{
			try {
				Assertion.AssertEquals(float.PositiveInfinity, float.NegativeInfinity, (float)0.0);
			} catch (AssertionFailedException) {
				return;
			}
			Assertion.Fail();
		}
		
		[TestMethodAttribute()]
		public void TestAssertSingle() 
		{
			Assertion.AssertEquals((float)1.0, (float)1.0, (float)0.0);
		}
		
		[TestMethodAttribute()]
		public void TestAssertByte() 
		{
			Assertion.AssertEquals((byte)1, (byte)1);
		}
		
		[TestMethodAttribute()]
		public void TestAssertString() 
		{
			string s1 = "test";
			string s2 = new System.Text.StringBuilder(s1).ToString();
			Assertion.AssertEquals(s1,s2);
		}
		
		[TestMethodAttribute()]
		public void TestAssertShort() 
		{
			Assertion.AssertEquals((short)1,(short)1);
		}
		
		[TestMethodAttribute()]
		public void TestAssertNull() 
		{
			Assertion.AssertNull(null);
		}
		
		[TestMethodAttribute()]
		public void TestAssertNullNotEqualsNull() 
		{
			try {
				Assertion.AssertEquals(null, new Object());
			} catch (AssertionFailedException) {
				return;
			}
			Assertion.Fail();
		}
		
		[TestMethodAttribute()]
		public void TestAssertSame() 
		{
			object o = new object();
			Assertion.AssertSame(o, o);
		}
		
		[TestMethodAttribute()]
		public void TestAssertSameFails() 
		{
			try  {
				Assertion.AssertSame(1, 2);
			} catch (AssertionFailedException) {
				return;
			}
			Assertion.Fail();
		}
		
		[TestMethodAttribute()]
		public void TestFail() 
		{
			try {
				Assertion.Fail();
			} 
			catch (AssertionFailedException) {
				return;
			}
			throw new AssertionFailedException("fail"); // You can't call fail() here
		}
		
		[TestMethodAttribute()]
		public void TestFailAssertNotNull() 
		{
			try {
				Assertion.AssertNotNull(null);
			} catch (AssertionFailedException) {
				return;
			}
			Assertion.Fail();
		}
		
		[TestMethodAttribute()]
		public void TestSucceedAssertNotNull() 
		{
			Assertion.AssertNotNull(new Object());
		}
	}
}
