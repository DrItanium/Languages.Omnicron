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
//Define the MATH_FORMULA use flag
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libraries.Extensions
{
#if MATH_FORMULA
	public static partial class Extensions
	{
		public static decimal LinearFibonacci(this decimal value)
		{
			if(value == 0.0M)
				return 0.0M;
			if(value == 1.0M)
				return 1.0M;
			decimal v0 = 0.0M;
			decimal v1 = 1.0M;
			for(decimal factor = 2; factor <= value; factor++)
			{
				decimal result = v0 + v1;
				if((result + 1) > value)
					return result;
				else
				{
					v0 = v1;
					v1 = result;
				}
			}
			return v1;
		}
		public static double LinearFibonacci(this double value)
		{
			if(value == 0.0)
				return 0.0;
			if(value == 1.0)
				return 1.0;
			double v0 = 0.0;
			double v1 = 1.0;
			for(double factor = 2; factor <= value; factor++)
			{
				double result = v0 + v1;
				if((result + 1) > value)
					return result;
				else
				{
					v0 = v1;
					v1 = result;
				}
			}
			return v1;
		}
		public static float LinearFibonacci(this float value)
		{
			if(value == 0.0f)
				return 0.0f;
			if(value == 1.0)
				return 1.0f;
			float v0 = 0.0f;
			float v1 = 1.0f;
			for(float factor = 2; factor <= value; factor++)
			{
				float result = v0 + v1;
				if((result + 1) > value)
					return result;
				else
				{
					v0 = v1;
					v1 = result;
				}
			}
			return v1;
		}
		public static long LinearFibonacci(this long value)
		{
			if(value == 0L)
				return 0L;
			if(value == 1L)
				return 1L;
			long v0 = 0L;
			long v1 = 1L;
			for(long factor = 2; factor <= value; factor++)
			{
				long result = v0 + v1;
				if((result + 1) > value)
					return result;
				else
				{
					v0 = v1;
					v1 = result;
				}
			}
			return v1;
		}
		public static ulong LinearFibonacci(this ulong value)
		{
			if(value == 0L)
				return 0L;
			if(value == 1L)
				return 1L;
			ulong v0 = 0L;
			ulong v1 = 1L;
			for(ulong factor = 2; factor <= value; factor++)
			{
				ulong result = v0 + v1;
				if((result + 1) > value)
					return result;
				else
				{
					v0 = v1;
					v1 = result;
				}
			}
			return v1;
		}
		public static int LinearFibonacci(this int value)
		{
			if(value == 0)
				return 0;
			if(value == 1)
				return 1;
			int v0 = 0;
			int v1 = 1;
			for(int factor = 2; factor <= value; factor++)
			{
				int result = v0 + v1;
				if((result + 1) > value)
					return result;
				else
				{
					v0 = v1;
					v1 = result;
				}
			}
			return v1;
		}
		public static uint LinearFibonacci(this uint value)
		{
			if(value == 0)
				return 0;
			if(value == 1)
				return 1;
			uint v0 = 0;
			uint v1 = 1;
			for(uint factor = 2; factor <= value; factor++)
			{
				uint result = v0 + v1;
				if((result + 1) > value)
					return result;
				else
				{
					v0 = v1;
					v1 = result;
				}
			}
			return v1;
		}
		public static decimal LinearFactorial(this decimal value)
		{
			if(value == 1.0M)
				return value;
			decimal _base = 1.0M;
			decimal curr = _base;
			for(decimal factor = _base; factor <= value; 
					curr = (curr * factor), factor++);
			return curr;
		}
		public static double LinearFactorial(this double value)
		{
			if(value == 1.0)
				return value;
			double _base = 1.0;
			double curr = _base;
			for(double factor = _base; factor <= value; 
					curr = (curr * factor), factor++);
			return curr;
		}
		public static float LinearFactorial(this float value)
		{
			if(value == 1.0f)
				return value;
			float _base = 1.0f;
			float curr = _base;
			for(float factor = _base; factor <= value; 
					curr = (curr * factor), factor++);
			return curr;
		}
		public static long LinearFactorial(this long value)
		{
			if(value == 1L)
				return value;
			long _base = 1L;
			long curr = _base;
			for(long factor = _base; factor <= value; 
					curr = (curr * factor), factor++);
			return curr;
		}
		public static ulong LinearFactorial(this ulong value)
		{
			if(value == 1L)
				return value;
			ulong _base = 1L;
			ulong curr = _base;
			for(ulong factor = _base; factor <= value; 
					curr = (curr * factor), factor++);
			return curr;
		}
		public static int LinearFactorial(this int value)
		{
			if(value == 1)
				return value;
			int _base = 1;
			int curr = _base;
			for(int factor = _base; factor <= value; 
					curr = (curr * factor), factor++);
			return curr;
		}
		public static uint LinearFactorial(this uint value)
		{
			if(value == 1)
				return value;
			uint _base = 1;
			uint curr = _base;
			for(uint factor = _base; factor <= value; 
					curr = (curr * factor), factor++);
			return curr;
		}
		



		public static Func<decimal,decimal,decimal> FirstDerivative(this Func<decimal,decimal> fn)
		{
			return (x,h) => (1.0M / (2.0M * h)) * ((fn(x + h) - fn(x - h)));	
		}	
		public static Func<double,double,double> FirstDerivative(this Func<double,double> fn)
		{
			return (x,h) => (1.0 / (2.0 * h)) * ((fn(x + h) - fn(x - h)));	
		}	
		public static Func<float,float,float> FirstDerivative(this Func<float,float> fn)
		{
			return (x,h) => (1.0f / (2.0f * h)) * ((fn(x + h) - fn(x - h)));	
		}	
		public static Func<long,long,long> FirstDerivative(this Func<long,long> fn)
		{
			return (x,h) => (1L / (2L * h)) * ((fn(x + h) - fn(x - h)));	
		}	
		public static Func<ulong,ulong,ulong> FirstDerivative(this Func<ulong,ulong> fn)
		{
			return (x,h) => (1L / (2L * h)) * ((fn(x + h) - fn(x - h)));	
		}	
		public static Func<int,int,int> FirstDerivative(this Func<int,int> fn)
		{
			return (x,h) => (1 / (2 * h)) * ((fn(x + h) - fn(x - h)));	
		}	
		public static Func<uint,uint,uint> FirstDerivative(this Func<uint,uint> fn)
		{
			return (x,h) => (1 / (2 * h)) * ((fn(x + h) - fn(x - h)));	
		}	

		public static Func<decimal,decimal,decimal> SecondDerivative(this Func<decimal,decimal> fn)
		{
			return (x,h) => ((1.0M / (h * h)) * ((fn(x + h) - (2.0M * fn(x))) + fn(x - h)));
		}	
		public static Func<double,double,double> SecondDerivative(this Func<double,double> fn)
		{
			return (x,h) => ((1.0 / (h * h)) * ((fn(x + h) - (2.0 * fn(x))) + fn(x - h)));
		}	
		public static Func<float,float,float> SecondDerivative(this Func<float,float> fn)
		{
			return (x,h) => ((1.0f / (h * h)) * ((fn(x + h) - (2.0f * fn(x))) + fn(x - h)));
		}	
		public static Func<long,long,long> SecondDerivative(this Func<long,long> fn)
		{
			return (x,h) => ((1L / (h * h)) * ((fn(x + h) - (2L * fn(x))) + fn(x - h)));
		}	
		public static Func<ulong,ulong,ulong> SecondDerivative(this Func<ulong,ulong> fn)
		{
			return (x,h) => ((1L / (h * h)) * ((fn(x + h) - (2L * fn(x))) + fn(x - h)));
		}	
		public static Func<int,int,int> SecondDerivative(this Func<int,int> fn)
		{
			return (x,h) => ((1 / (h * h)) * ((fn(x + h) - (2 * fn(x))) + fn(x - h)));
		}	
		public static Func<uint,uint,uint> SecondDerivative(this Func<uint,uint> fn)
		{
			return (x,h) => ((1 / (h * h)) * ((fn(x + h) - (2 * fn(x))) + fn(x - h)));
		}	
	}
#endif
}
