//Copyright 2012-2015 Joshua Scoggins. All rights reserved.
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

namespace Libraries.Collections
{
	public class Segment : ICloneable
	{
		private int start, length, computedStart;
		public int Start
		{
			get
			{
				return start;
			}
			set
			{
				if(start != value)
				  computedStart = value + length;
				start = value;

			}
		}
		public int Length
		{
			get
			{
				return length;
			}
			set
			{
				if(value != length)
					computedStart = value + start;
				length = value;
			}
		}
		public int ComputedStart { get { return computedStart; } }

		public Segment(int length, int start)
		{
			this.start = start;
			this.length = length;
		}
		public Segment(int length) : this(length, 0) { }
		public Segment(Segment other) : this(other.length, other.start) { }
		public virtual object Clone()
		{
			return new Segment(this);
		}
		public override bool Equals(object other)
		{
			Segment ot = (Segment)other;
			return ot.length == length && ot.start == start;
		}
		public override int GetHashCode()
		{
			return length.GetHashCode() + start.GetHashCode();
		}
		public override string ToString()
		{
			return string.Format("({0}=>{1})", start, length);
		}
		public string Substring(string input)
		{
			return input.Substring(start, length);
		}
	}
}
