//Copyright 2010 Joshua Scoggins. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are
//permitted provided that the following conditions are met:
//
//   1. Redistributions of source code must retain the above copyright notice, this list of
//      conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright notice, this list
//      of conditions and the following disclaimer in the documentation and/or other materials
//      provided with the distribution.
//
//THIS SOFTWARE IS PROVIDED BY Joshua Scoggins ``AS IS'' AND ANY EXPRESS OR IMPLIED
//WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Joshua Scoggins OR
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//The views and conclusions contained in the software and documentation are those of the
//authors and should not be interpreted as representing official policies, either expressed
//or implied, of Joshua Scoggins. 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace Libraries.Extensions
{
	public static partial class Extensions
	{
		public static IEnumerable<T> Append<T>(this T value, IEnumerable<T> elements)
		{
			yield return value;
			foreach(var v in elements)
				yield return v;
		}
		public static IEnumerable<T> Append<T>(this T value, params T[] elements)
		{
			return Append(value, (IEnumerable<T>)elements);
		}
		//allows code to executed inline of a statement without disrupting the statement
		public static T Then<T>(this T value, Action code)
		{
			code();
			return value;
		}
		public static T Then<T>(this T value, Action<T> code)
		{
			code(value);
			return value;
		}
		//Performs an operation on a value inline, this is useful for values that can't
		//be stored into a separate variable before hand
		public static T Then<T>(this T value, Func<T, T> modCode)
		{
			return modCode(value);
		}
		//this is an inline modification chain...be forewarned
		public static T Then<T>(this T value, params Action<T>[] actions)
		{
			for(int i =0 ; i < actions.Length; i++)
				actions[i](value);
			return value;
		}
		public static T Then<T>(this T value, params Action[] actions)
		{
			for(int i = 0; i < actions.Length; i++)
				actions[i]();
			return value;
		}
		public static T Then<T>(this T value, params Func<T,T>[] functions)
		{
			T output = value;
			for(int i = 0; i < functions.Length; i++)
				output = functions[i](output);
			return output;
		}

	}
}
