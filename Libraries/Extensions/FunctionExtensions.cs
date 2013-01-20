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

namespace Libraries.Extensions
{
	public static partial class Extensions
	{
		public static T Choice<T>(this bool value, T onTrue, T onFalse)
		{
			if(value)
				return onTrue;
			else
				return onFalse;
		}
		public static Func<R> FunctionChoice<R>(this bool value, Func<R> onTrue, Func<R> onFalse)
		{
			return Choice<Func<R>>(value, onTrue, onFalse);
		}
		public static Func<T0,R> FunctionChoice<T0,R>(this bool value, Func<T0,R> onTrue, Func<T0,R> onFalse)
		{
			return Choice<Func<T0,R>>(value, onTrue, onFalse);
		}
		public static Func<T0,T1,R> FunctionChoice<T0,T1,R>(this bool value, Func<T0,T1,R> onTrue, Func<T0,T1,R> onFalse)
		{
			return Choice<Func<T0,T1,R>>(value, onTrue, onFalse);
		}
		public static Func<T0,T1,T2,R> FunctionChoice<T0,T1,T2,R>(this bool value, Func<T0,T1,T2,R> onTrue, Func<T0,T1,T2,R> onFalse)
		{
			return Choice<Func<T0,T1,T2,R>>(value, onTrue, onFalse);
		}
		public static Func<T0,T1,T2,T3,R> FunctionChoice<T0,T1,T2,T3,R>(this bool value, Func<T0,T1,T2,T3,R> onTrue, Func<T0,T1,T2,T3,R> onFalse)
		{
			return Choice<Func<T0,T1,T2,T3,R>>(value, onTrue, onFalse);
		}
		public static Action ActionChoice(this bool value, Action onTrue, Action onFalse)
		{
			return Choice<Action>(value, onTrue, onFalse);
		}
		public static Action<T0> ActionChoice<T0>(this bool value, Action<T0> onTrue, Action<T0> onFalse)
		{
			return Choice<Action<T0>>(value, onTrue, onFalse);
		}
		public static Action<T0,T1> ActionChoice<T0,T1>(this bool value, Action<T0,T1> onTrue, Action<T0,T1> onFalse)
		{
			return Choice<Action<T0,T1>>(value, onTrue, onFalse);
		}
		public static Action<T0,T1,T2> ActionChoice<T0,T1,T2>(this bool value, Action<T0,T1,T2> onTrue, Action<T0,T1,T2> onFalse)
		{
			return Choice<Action<T0,T1,T2>>(value, onTrue, onFalse);
		}
		public static Action<T0,T1,T2,T3> ActionChoice<T0,T1,T2,T3>(this bool value, Action<T0,T1,T2,T3> onTrue, Action<T0,T1,T2,T3> onFalse)
		{
			return Choice<Action<T0,T1,T2,T3>>(value, onTrue, onFalse);
		}
		public static IEnumerable<T> Transform<T>(this IEnumerable<T> list, Func<T,T> transformer)
		{
			return Transform<T,T>(list, transformer);
		}
		public static IEnumerable<R> Transform<T,R>(this IEnumerable<T> list, Func<T,R> transformer)
		{
			foreach(var v in list)
				yield return transformer(v);
		}

		public static Func<T, T> Compose<T>(this Func<T, T> f, Func<T, T> g)
		{
			return (x) => g(f(x));
		}

		public static Func<T, R> Compose<T, R>(this Func<T, T> f, Func<T, R> g)
		{
			return (x) => g(f(x));
		}
		public static bool ContainsAtLeast<T>(this IEnumerable<T> collec, int atLeast, Func<T, bool> eval)
		{
			int amt = 0;
			foreach (var v in collec)
			{
				if (amt >= atLeast)
					return true;
				if (eval(v))
					amt++;
			}
			return false;
		}
		public static bool ContainsAtMost<T>(this IEnumerable<T> collec, int atMost, Func<T, bool> eval)
		{
			int amt = 0;
			foreach (var v in collec)
			{
				if (amt > atMost)
					return false;
				if (eval(v))
					amt++;
			}
			return amt <= atMost;
		}
		public static bool ContainsExactly<T>(this IEnumerable<T> collec, int exactly, Func<T, bool> eval)
		{
			int amt = 0;
			foreach (var v in collec)
			{
				if (amt > exactly)
					return false;
				if (eval(v))
					amt++;
			}
			return amt == exactly;
		}
		public static bool ContainsLessThan<T>(this IEnumerable<T> collec, int lessThan, Func<T, bool> eval)
		{
			int amt = 0;
			foreach (var v in collec)
			{
				if (amt >= lessThan)
					return false;
				if (eval(v))
					amt++;
			}
			return amt < lessThan;
		}

		public static bool ContainsGreaterThan<T>(this IEnumerable<T> collec, int greaterThan, Func<T, bool> eval)
		{
			int amt = 0;
			foreach (var v in collec)
			{
				if (amt > greaterThan)
					return true;
				if (eval(v))
					amt++;
			}
			return amt > greaterThan;
		}
		public static bool ContainsRoughly<T>(this IEnumerable<T> collec, int roughly, int uncertainty, Func<T, bool> eval)
		{
			int amt = 0;
			foreach (var v in collec)
			{
				if (amt.InRange(roughly, uncertainty))
					return true;
				if (eval(v))
					amt++;
			}
			return amt.InRange(roughly, uncertainty);
		}

		//TransformAcross has been replaced by operations in LinqExtensions
	}
}
