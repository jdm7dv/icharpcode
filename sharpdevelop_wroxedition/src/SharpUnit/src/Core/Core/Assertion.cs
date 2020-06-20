/*
 * Assertion.cs
 * Copyright (C) 2002 Mike Krueger, icsharpcode
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

namespace ICSharpCode.SharpUnit {

	/// <summary>
	/// A set of JUnit/NUnit compatible assertion methods
	/// </summary>
	public sealed class Assertion : MarshalByRefObject
	{
		/// <summary>
		/// Private constructor prohibits creation of instances from the Assertion class.
		/// </summary>
		Assertion()
		{
		}
		
		/// <summary>
		/// Asserts that a condition is true. 
		/// </summary>
		/// <param name="message">
		/// The message to display if the condition is false
		/// </param>
		/// <param name="condition">
		/// The evaluated condition
		/// </param>
		/// <exception name="AssertionFailedException">
		/// thrown if condition == false
		/// </exception>
		public static void Assert(string message, bool condition) 
		{
			if (!condition) {
				Assertion.Fail(message);
			}
		}
    
		/// <summary>
		/// Asserts that a condition is true. 
		/// </summary>
		/// <param name="condition">
		/// The evaluated condition
		/// </param>
		/// <exception name="AssertionFailedException">
		/// thrown if condition == false
		/// </exception>
		public static void Assert(bool condition) 
		{
			Assertion.Assert(String.Empty, condition);
		}
		
		/// <summary>
		/// Asserts that two doubles are equal concerning a delta. If the
		/// expected value is infinity then the delta value is ignored.
		/// </summary>
		/// <param name="expected">
		/// The expected value
		/// </param>
		/// <param name="current">
		/// The current value
		/// </param>
		/// <param name="delta">
		/// The maximum acceptable difference between the
		/// the expected and the current
		/// </param>
		/// <exception name="AssertionFailedException">
		/// thrown if |excepted - current| > delta
		/// </exception>
		public static void AssertEquals(double expected, double current, double delta) 
		{
			Assertion.AssertEquals(String.Empty, expected, current, delta);
		}
		
		/// <summary>
		/// Asserts that two doubles are equal concerning a delta.
		/// If the expected value is infinity then the delta value is ignored.
		/// </summary>
		/// <param name="message">
		/// The message to display if the two objects aren't equal.
		/// </param>
		/// <param name="expected">
		/// The expected value
		/// </param>
		/// <param name="current">
		/// The current value
		/// </param>
		/// <param name="delta">
		/// The maximum acceptable difference between the
		/// the expected and the current
		/// </param>
		/// <exception name="AssertionFailedException">
		/// thrown if |excepted - current| > delta
		/// </exception>
		public static void AssertEquals(string message, double expected, double current, double delta) 
		{
			if (Double.IsInfinity(expected))  {
				if (!(expected == current)) {
					Assertion.FailNotEquals(message, expected, current);
				}
			} else if (!(Math.Abs(expected - current) <= delta)) {
				Assertion.FailNotEquals(message, expected, current);
			}
		}
		
		/// <summary>
		/// Asserts that two singles are equal concerning a delta. If the
		/// expected value is infinity then the delta value is ignored.
		/// </summary>
		/// <param name="expected">
		/// The expected value
		/// </param>
		/// <param name="current">
		/// The current value
		/// </param>
		/// <param name="delta">
		/// The maximum acceptable difference between the
		/// the expected and the current
		/// </param>
		/// <exception name="AssertionFailedException">
		/// thrown if |excepted - current| > delta
		/// </exception>
		public static void AssertEquals(float expected, float current, float delta) 
		{
			Assertion.AssertEquals(String.Empty, expected, current, delta);
		}
		
		/// <summary>
		/// Asserts that two floats are equal concerning a delta.
		/// If the expected value is infinity then the delta value is ignored.
		/// </summary>
		/// <param name="message">
		/// The message to display if the two objects aren't equal.
		/// </param>
		/// <param name="expected">
		/// The expected value
		/// </param>
		/// <param name="current">
		/// The current value
		/// </param>
		/// <param name="delta">
		/// The maximum acceptable difference between the
		/// the expected and the current
		/// </param>
		/// <exception name="AssertionFailedException">
		/// thrown if |excepted - current| > delta
		/// </exception>
		public static void AssertEquals(string message, float expected, float current, float delta) 
		{
			if (Single.IsInfinity(expected)) {
				if (expected != current) {
					Assertion.FailNotEquals(message, expected, current);
				}
			} else if (Math.Abs(expected - current) > delta) {
				Assertion.FailNotEquals(message, expected, current);
			}
		}
		
		/// <summary>
		/// Asserts that two objects are equal. 
		/// </summary>
		/// <param name="expected">
		/// The expected value
		/// </param>
		/// <param name="current">
		/// The current value
		/// </param>
		/// <exception name="AssertionFailedException">
		/// if !excepted.Equals(current).
		/// </exception>
		public static void AssertEquals(object expected, object current) 
		{
			Assertion.AssertEquals(String.Empty, expected, current);
		}
		
		/// <summary>
		/// Asserts that two objects are equal. 
		/// </summary>
		/// <param name="message">
		/// The message to display if the two objects aren't equal.
		/// </param>
		/// <param name="expected">
		/// The expected value
		/// </param>
		/// <param name="current">
		/// The current value
		/// </param>
		/// <exception name="AssertionFailedException">
		/// thrown if !excepted.Equals(current)
		/// </exception>
		public static void AssertEquals(string message, object expected, object current)
		{
			if (expected == null && current == null) {
				return;
			}
			if (expected != null && expected.Equals(current)) {
				return;
			}
			Assertion.FailNotEquals(message, expected, current);
		}
    
		/// <summary>
		/// Asserts that an object isn't null.
		/// </summary>
		/// <exception name="AssertionFailedException">
		/// thrown if o == null
		/// </exception>
		public static void AssertNotNull(object o) 
		{
			Assertion.AssertNotNull(String.Empty, o);
		}
    
		/// <summary>
		/// Asserts that an object isn't null.
		/// </summary>
		/// <param name="message">
		/// The message to display if o == null
		/// </param>
		/// <exception name="AssertionFailedException">
		/// thrown if o == null
		/// </exception>
		public static void AssertNotNull(string message, object o) 
		{
			Assertion.Assert(String.Empty, o != null); 
		}
    
		/// <summary>
		/// Asserts that an object is null.
		/// </summary>
		/// <exception name="AssertionFailedException">
		/// thrown if o != null
		/// </exception>
		public static void AssertNull(object o) 
		{
			Assertion.AssertNull(String.Empty, o);
		}
    
		/// <summary>
		/// Asserts that an object is null.
		/// </summary>
		/// <param name="message">
		/// The message to display if o != null
		/// </param>
		/// <exception name="AssertionFailedException">
		/// thrown if o != null
		/// </exception>
		public static void AssertNull(string message, object o) 
		{
			Assertion.Assert(message, o == null); 
		}
    
		/// <summary>
		/// Asserts that two objects refer to the same object.
		/// </summary>
		/// <param name="expected">
		/// The expected value
		/// </param>
		/// <param name="current">
		/// The current value
		/// </param>
		/// <exception name="AssertionFailedException">
		/// thrown if excepted and current don't refer to the same object.
		/// </exception>
		public static void AssertSame(object expected, object current) 
		{
			Assertion.AssertSame(String.Empty, expected, current);
		}
    
		/// <summary>
		/// Asserts that two objects refer to the same object. 
		/// </summary>
		/// <param name="message">
		/// The message to display if the two objects don't refer to the same object
		/// </param>
		/// <param name="expected">
		/// The expected value
		/// </param>
		/// <param name="current">
		/// The current value
		/// </param>
		/// <exception name="AssertionFailedException">
		/// thrown if excepted and current don't refer to the same object.
		/// </exception>
		public static void AssertSame(string message, object expected, object current)
		{
			if (expected == current) {
				return;
			}
			Assertion.FailNotSame(message, expected, current);
		}
    
		/// <summary>
		/// Throw an AssertionFailedException.
		/// </summary>
		public static void Fail() 
		{
			Assertion.Fail(String.Empty);
		}
    
		/// <summary>
		/// Throw an AssertionFailedException.
		/// </summary>
		/// <param name="message">
		/// The message to display.
		/// </param>
		public static void Fail(string message) 
		{
			if (message == null) {
				message = String.Empty;
			}
			throw new AssertionFailedException(message);
		}
    
		static void FailNotEquals(string message, object expected, object current)
		{
			Assertion.Fail(message + " expected:<" + expected + "> but was:<" + current + ">");
		}
    
		static void FailNotSame(string message, object expected, object current) 
		{
			Assertion.Fail(message + " expected same");
		}
	}
}
