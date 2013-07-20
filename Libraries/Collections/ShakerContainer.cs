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
	///<summary>
	/// A ShakeSelector is a data structure that
	/// allows a programmer to use a "sieve" to 
	/// filter out elements in a very unique way.
	/// Imagine that your data is like a bunch of
	/// debris that you have to search through
	/// to find something. 
	///</summary>
	public class ShakerContainer<T>
	{
		public ShakeSelector<T> Selector { get; set; }
		public ShakerContainer(ShakeSelector<T> selector)
		{
			Selector = selector;
		}
		public IEnumerable<Hunk<T>> Shake(Hunk<T> target, ShakeCondition<T> cond)
		{
			return Shake(target, cond, Selector);
		}

		public IEnumerable<Hunk<T>> Shake(Hunk<T> target, ShakeCondition<T> cond, ShakeSelector<T> selector)
		{
			Hunk<T> prev = target;
			Tuple<Hunk<T>, Hunk<T>, Hunk<T>> curr = new Tuple<Hunk<T>, Hunk<T>, Hunk<T>>(null, null, target);
			do
			{
				//need a way to just say "if you don't find a single thing
				// then just kick back"
				curr = BasicShakerFunction(curr.Item3, cond, selector);
				if (curr.Item1 != null)
					yield return curr.Item1;
				if (curr.Item2 != null)
					yield return curr.Item2;
				if (curr.Item3.Equals(prev))
				{
					if (curr.Item3.Length > 0)
						yield return curr.Item3;
					yield break;
				}
				prev = curr.Item3;
			} while (true);
		}
		public IEnumerable<Hunk<T>> Shake(Hunk<T> target, ShakeCondition<T> a, ShakeCondition<T> b)
		{
			//go through each cond and when one spits back we need to check if its a big element or not
			//we need to do something similar to how I did the abstract clone function in java for functions
			//as first class objects

			//Its important to note that we need to flatten the elements first
			foreach (var v in Shake(target, a))
			{
				if (v.IsBig)
					foreach (var q in Shake(v, b))
						yield return q;
				else
					yield return v;
			}
		}
		private bool ContainsAtLeastOne<Y>(IEnumerable<Y> collection)
		{
			return !collection.FirstOrDefault().Equals(default(Y));
		}
		private IEnumerable<Hunk<T>> ShakeInternal(IEnumerable<Hunk<T>> outer, ShakeCondition<T> cond)
		{
			if (cond == null)
			{
				foreach (var v in outer)
					yield return v;
			}
			else
			{
				foreach (var v in outer)
				{
					if (v.IsBig)
					{
						foreach (var q in Shake(v, cond))
							yield return q;
					}
					else
						yield return v;
				}
			}
		}
		private IEnumerable<Hunk<T>> ShakeSingle(Hunk<T> hunk) { yield return hunk; } //makes a more consistent solution

		public IEnumerable<Hunk<T>> Shake(Hunk<T> target, IEnumerable<ShakeCondition<T>> conds)
		{
			IEnumerable<Hunk<T>> initial = Shake(target, conds.First()).ToArray();
			foreach (var cond in conds.Skip(1)) //this goes through the entire list of hunks for each condition, lets go through each hunk for all conditions
				initial = ShakeInternal(initial, cond);
			return ShakeInternal(initial, null);
		}
		public Tuple<Hunk<T>, Hunk<T>, Hunk<T>> BasicShakerFunction(
				Hunk<T> target, ShakeCondition<T> cond)
		{
			return BasicShakerFunction(target, cond, Selector);
		}
		protected Tuple<Hunk<T>, Hunk<T>, Hunk<T>> BasicShakerFunction(
				Hunk<T> target,
				ShakeCondition<T> cond,
				ShakeSelector<T> selector)
		{
#if DEBUG
			Console.WriteLine("target = [{0}, {1}], {2}", target.Start, target.Length, target.Value);
#endif
			Func<Segment, Hunk<T>, int> getRest = (seg, hunk) => (hunk.Length - (seg.Start + seg.Length));
#if DEBUG
			Func<Segment, Hunk<T>, int> getComputedHunkStart = (seg, hunk) => seg.Start + hunk.Start;
#endif
			Func<Segment, int> getComputedStart = (seg) => seg.Start + seg.Length;
			Segment result = cond(target);
			if (result == null)
			{
#if DEBUG
				Console.WriteLine("Done Null");
#endif
				return new Tuple<Hunk<T>, Hunk<T>, Hunk<T>>(null, null, target);
			}
			else
			{
#if DEBUG
				Console.WriteLine("Target = {0}", target);
				Console.WriteLine("Target.Length = {0}", target.Length);
				Console.WriteLine("Result = {0}", result);
				Console.WriteLine("Result Substring = {0}", result.Substring(target.Value.ToString()));
				Console.WriteLine("Result = {0}", result);
#endif
				Segment restSection = new Segment(getRest(result, target), getComputedStart(result));
#if DEBUG
				Console.WriteLine("restSection = {0}", restSection);
				Console.WriteLine("restSection = {0}", restSection);
				Console.WriteLine("Calling selector");
#endif
				var matchHunks = selector(result, target);
#if DEBUG
				Console.WriteLine("Finished Calling Selector");
#endif
				var before = matchHunks.Item1;
				var match = matchHunks.Item2;
				match.IsBig = false; //make sure this is preserved
				var restHunks = selector(restSection, target);
				var rest = restHunks.Item2;
				rest.IsBig = true;
				return new Tuple<Hunk<T>, Hunk<T>, Hunk<T>>(before, match, rest);
			}
		}
	}
	public delegate Segment ShakeCondition<T>(Hunk<T> hunk);
	public delegate Tuple<Hunk<T>, Hunk<T>> ShakeSelector<T>(Segment seg, Hunk<T> target);
	public delegate Tuple<Hunk<T>, Hunk<T>, Hunk<T>> Shaker<T>(Hunk<T> target,
			ShakeCondition<T> condition, ShakeSelector<T> selector);
}
