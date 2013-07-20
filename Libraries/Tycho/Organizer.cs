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
using Libraries.Collections;
using Libraries.LexicalAnalysis;

namespace Libraries.Tycho
{
	public class StringOrganizer : List<Keyword>
	{
		private List<ShakeCondition<string>> shakedConditions; 
		private List<TypedShakeCondition<string>> typedShakedConditions;
		public StringOrganizer(IEnumerable<Keyword> input)
		{
			shakedConditions = new List<ShakeCondition<string>>();
			typedShakedConditions = new List<TypedShakeCondition<string>>();
			//select all elements that are contained within the
			//current element
			//What this does is make functions that are used as 
			//shake selectors
			//To this end its important to find those segments that are 
			Dictionary<string, Keyword> keys = new Dictionary<string, Keyword>();
			foreach(var v in input)
				keys.Add(v.TargetWord, v);
			var selection = from x in input
				let y = (from z in input
						where !z.TargetWord.Equals(x.TargetWord) && z.TargetWord.Contains(x.TargetWord)
						select z.TargetWord)
				group y by x.TargetWord into element
				select element;

			Dictionary<string,int> frequencyTable = new Dictionary<string,int>();
			foreach(var v in input)
				frequencyTable.Add(v.TargetWord,0);
			foreach(var v in selection)
			{
				foreach(var q in v)
				{
					frequencyTable[v.Key] += q.Count();
				}
			}
			var result = from zz in frequencyTable
				orderby zz.Value ascending, zz.Key.Length descending
				select new
				{
					Key = zz.Key,
							Frequency = zz.Value,
							NeedsEqualityCheck = (zz.Value > 0 && zz.Key.Length > 1)
				};

			foreach(var v in result)
			{
				if (v.NeedsEqualityCheck)
				{
					//Console.WriteLine("v.Key = {0}", v.Key);
					keys[v.Key].RequiresEqualityCheck = v.NeedsEqualityCheck;
				}
				Add(keys[v.Key]);
			}
			// Console.WriteLine("------");
			//foreach (var v in this)
			//   Console.WriteLine(v.TargetWord);
			//Console.WriteLine("------");
			foreach(var v in this)
				shakedConditions.Add(v.AsShakeCondition());
			foreach(var v in this)
				typedShakedConditions.Add(v.AsTypedShakeCondition());
		}
		public IEnumerable<ShakeCondition<string>> GetShakeConditions()
		{
			return shakedConditions;
		}
		public IEnumerable<TypedShakeCondition<string>> GetTypedShakeConditions()
		{
			return typedShakedConditions;
		}
	}
}
