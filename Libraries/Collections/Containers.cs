
//Copyright 2012 Joshua Scoggins. All rights reserved.
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
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Threading;
using Libraries.Extensions;

namespace Libraries.Collections
{
	public class Container<T>
	{
		public T Value1 { get; set; }
		public Container(T value)
		{
			Value1 = value;
		}
		public Container() : this(default(T))
		{

		}
		public override bool Equals(object other)
		{
			Container<T> si = (Container<T>)other;
			return si.Value1.Equals(Value1);
		}
		public override int GetHashCode()
		{
			return Value1.GetHashCode();
		}
		public override string ToString()
		{
			return Value1.ToString();
		}
	}
	public class Container<T1,T2> : Container<T1>
	{
		public T2 Value2 { get; set; }
		public Container(T1 value1, T2 value2)
			: base(value1)
		{
			Value2 = value2;
		}
		public Container() : base()
		{

		}
		public override bool Equals(object other)
		{
			Container<T1,T2> c = (Container<T1,T2>)other;
			return c.Value2.Equals(Value2) && c.Value1.Equals(Value1);
		}
		public override string ToString()
		{
			return string.Format("[{0},{1}]", Value1, Value2);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() + Value2.GetHashCode();
		}
	}
	public class Container<T1,T2,T3> : Container<T1,T2>
	{
		public T3 Value3 { get; set; }
		public Container(T1 v1, T2 v2, T3 v3)
			: base(v1,v2)
		{
			Value3 = v3;
		}
		public Container() : base() 
		{

		}
		public override int GetHashCode()
		{
			return base.GetHashCode() + Value3.GetHashCode();
		}
		public override bool Equals(object other)
		{
			Container<T1,T2,T3> c = (Container<T1,T2,T3>)other;
			return base.Equals(other) && c.Value3.Equals(Value3);
		}
		public override string ToString()
		{
			return string.Format("[{0},{1},{2}]", Value1, Value2, Value3);
		}
	}
}
