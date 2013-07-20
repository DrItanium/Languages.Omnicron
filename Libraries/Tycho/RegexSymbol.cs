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
using Libraries.Extensions;
using Libraries.Collections;
using Libraries.LexicalAnalysis;

namespace Libraries.Tycho
{
	public class RegexSymbol : Word, IComparable<RegexSymbol>
	{
		public string Name { get; set; }
		//	public bool CachingIsAllowed { get; protected set; }
		public RegexSymbol(string input, string name, string type)
			: base(input, type)
		{
			Name = name;
		}
		public override ShakeCondition<string> AsShakeCondition()
		{
			Regex currentRegex = new Regex(TargetWord);
			return LexicalExtensions.GenerateRegexCond<string>(
					(x,y,z) => 
					{
					Match m = currentRegex.Match(x,y,z - y);	
					Segment output = m.Success ? new Segment(m.Length, m.Index + y) : null;
					return new Tuple<bool, Segment>(m.Success, output);
					});
		}
		public override TypedShakeCondition<string> AsTypedShakeCondition()
		{
			Regex currentRegex = new Regex(TargetWord);
			return LexicalExtensions.GenerateTypedRegexCond<string>(
					(x,y,z) => 
					{
					//						Console.WriteLine("x = {0}, y = {1}, z = {2}", x, y, z);
					Match m = currentRegex.Match(x,y,z - y);	
					//						Console.WriteLine("m.Length = {0},m.Index = {1}", m.Length, m.Index);
					TypedSegment output = m.Success ? new TypedSegment(m.Length, WordType, m.Index + y) : null;
					//						if(m.Success)
					//						  Console.WriteLine("output = {0}", output);
					return new Tuple<bool, TypedSegment>(m.Success, output);
					});
		}

		public override object Clone()
		{
			return new RegexSymbol(TargetWord, Name, TargetWord);
		}
		public virtual int CompareTo(RegexSymbol other)
		{
			return Name.CompareTo(other.Name) + base.CompareTo((Word)other);
		}

	}
}
