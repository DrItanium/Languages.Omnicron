//Copyright 2011 Joshua Scoggins. All rights reserved.
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
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Libraries.Extensions
{
#if POLYNOMIAL_APPROXIMATION
	public class LagrangePolynomialApproximator : List<Tuple<decimal,decimal>>
	{
		private List<Tuple<Func<int, decimal, decimal>, decimal>> precompute;
		private decimal[] xc, yc;
		private int degree;
		public int Degree { get { return degree; } protected set { degree = value; } }
		public LagrangePolynomialApproximator(IEnumerable<Tuple<decimal, decimal>> points, int degree)
		{
			if(degree >= (points.Count()))
				throw new ArgumentException(string.Format("Insufficient number of points given for degree {0}, have {1} points, maximum degree function that can be generated is {2}", degree, Count, degree - 1));
			else
			{
				this.degree = degree;
				AddRange(points);
				var q = points.Invert();
				xc = q.Item1.ToArray();
				yc = q.Item2.ToArray();
				precompute = new List<Tuple<Func<int, decimal, decimal>, decimal>>();
				for(int i = 0; i < (degree + 1); i++)
					precompute[i] = new Tuple<Func<int,decimal,decimal>,decimal>(LagrangePolynomialApproximation0(i), yc[i]);
			}
		}
		public Func<decimal, decimal> MakeFunction()
		{
			return LagrangePolynomialApproximation1(degree);	
		}
		private decimal LagrangePolynomialApproximation2(int j, decimal x, decimal xcI)
		{
			var xcJ = xc[j];
			return (x - xcJ) / (xcI - xcJ);
		}
		private Func<int, decimal, decimal> LagrangePolynomialApproximation0(int i)
		{
			Func<int, decimal, decimal, decimal> fn = LagrangePolynomialApproximation2;
      return (x,y) => fn(x,y,xc[i]);
		}
		private decimal LagrangePolynomialApproximation3(decimal x, int n)
		{
				decimal resultOuter = 0.0M;
				decimal resultInner = 1.0M;
				for(int i = 0; i < (n + 1); i++)
				{
					var pc = precompute[i];
					var fn = pc.Item1;
					var ycElementAt = pc.Item2;
					//if this is zero then there's no
					//point in going any further...this
					//will eventually result in zero.
					if(ycElementAt != 0.0M) 
					{
						resultInner = 1.0M;	
						for(int j = 0; j < (n + 1); j++)
						{
							if(j != i)
								resultInner *= fn(j,x);
						}	
						resultOuter += (resultInner * ycElementAt);
					}
				}
				return resultOuter;
		}
		private Func<decimal, decimal> LagrangePolynomialApproximation1(int n)
		{
			Func<decimal, int, decimal> fn = LagrangePolynomialApproximation3;
      return (x) => fn(x,n);
		}
	}
#endif
}
