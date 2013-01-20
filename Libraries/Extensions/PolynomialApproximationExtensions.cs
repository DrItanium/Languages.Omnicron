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
//
//POLYNOMIAL_APPROXIMATION is a use flag
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Libraries.Extensions
{
#if POLYNOMIAL_APPROXIMATION
	public static partial class Extensions
	{
		public static Func<decimal,decimal> LagrangePolynomialApproximation(
				this IEnumerable<Tuple<decimal,decimal>> points, int degree)
		{
			return new LagrangePolynomialApproximator(points, degree).MakeFunction();
		}
		public static IEnumerable<decimal> MakeChebyshevNodes(this IEnumerable<decimal> range, decimal n, bool exact)
		{
			Func<decimal,decimal,decimal> fn = null;
			if(exact)
				fn = MakeExactChebyshevNode;
			else
				fn = MakeApproximateChebyshevNode;	
			foreach(decimal d in range)
				yield return fn(d,n);
		}	
		public static decimal MakeApproximateChebyshevNode(this decimal i, decimal n)
		{
			if(decimal.Zero <= i && i <= n)
			{
				decimal val = i;
				val *= (decimal)Math.PI;
				val /= n;
				return (decimal)Math.Cos((double)val);
			}
			else
				throw new ArgumentException(string.Format("Chebyshev nodes can only be generated for values of i in the range [0,{0}]",n));
		}
		public static decimal MakeExactChebyshevNode(this decimal i, decimal n)
		{
			if(decimal.Zero <= i && i <= n)
			{
				decimal val = (2.0M * i + 1) / (2.0M * n + 2);
				val *= (decimal)Math.PI;
				return (decimal)Math.Cos((double)val);
			}
			else
				throw new ArgumentException(string.Format("Chebyshev nodes can only be generated for values of i in the range [0,{0}]",n));
		}
		public static decimal DividedDifference(this decimal x0, decimal y0, decimal x1, decimal y1)
		{
			return (y1 - y0) / (x0 - x1);
		}
		public static decimal DividedDifference(this KeyValuePair<decimal,decimal> a, KeyValuePair<decimal,decimal> b)
		{
			return DividedDifference(a.Key,a.Value,b.Key,b.Value);
		}
		public static decimal DividedDifference(this Tuple<decimal,decimal> a, Tuple<decimal,decimal> b)
		{
			return DividedDifference(a.Item1,a.Item2,b.Item1,b.Item2);
		}
	}
#endif
}
