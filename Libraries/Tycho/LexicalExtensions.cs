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
using Libraries.Extensions;

namespace Libraries.LexicalAnalysis
{
	public static partial class LexicalExtensions
	{
		public static string Substring(this string input, Segment seg) { return seg.Substring(input); }
		public static Tuple<Token<T>, Token<T>> DefaultTypedSelector<T>(TypedSegment seg, Token<T> hunk, Func<T, TypedSegment, Token<T>, T> transform)
		{
			int offset = hunk.Start;
			if (seg.Start == 0)
			{
				var value = new Tuple<Token<T>, Token<T>>(
						null,
						new Token<T>(transform(hunk.Value, seg, hunk), seg.Type, seg.Length, seg.Start + offset, true));
				return value;
			}
			else
			{

				var value = new Tuple<Token<T>, Token<T>>(
						new Token<T>(transform(hunk.Value, new TypedSegment(seg.Start, string.Empty), hunk), string.Empty, seg.Start, offset), //forward
						new Token<T>(transform(hunk.Value, seg, hunk), seg.Type, seg.Length, seg.Start + offset, true)); //match
				return value;
			}

		}
		public static Tuple<Hunk<T>, Hunk<T>> DefaultSelector<T>(Segment seg, Hunk<T> hunk, Func<T,Segment,Hunk<T>,T> transform)
		{
			int offset = hunk.Start;
			if(seg.Start == 0)
				return new Tuple<Hunk<T>, Hunk<T>>(
						null, //before
						new Hunk<T>(transform(hunk.Value, seg, hunk), seg.Length, seg.Start + offset, true));
			else
				return new Tuple<Hunk<T>, Hunk<T>>(
						new Hunk<T>(transform(hunk.Value,new Segment(seg.Start), hunk), seg.Start, offset), //forward hunk
						new Hunk<T>(transform(hunk.Value, seg, hunk), seg.Length, seg.Start + offset, true)); //match
		}
		public static Tuple<Hunk<string>, Hunk<string>> DefaultStringSelector(Segment seg, Hunk<string> hunk)
		{
			return DefaultSelector<string>(seg, hunk, (x,y,z) => x.Substring(y.Start, y.Length));
		}
		public static Tuple<Token<string>, Token<string>> DefaultTypedStringSelector(TypedSegment seg, Token<string> tok)
		{
			return DefaultTypedSelector<string>(seg, tok, (x, y, z) => x.Substring(y.Start, y.Length));
		}
		public static TypedShakeCondition<string> GenerateSingleTypedCharacterCond(char target, string type)
		{
			return GenerateTypedCond<string>((val, ind, len) => new Tuple<bool, TypedSegment>((val[ind] == target), new TypedSegment(1, type, ind)));
		}
		public static ShakeCondition<string> GenerateSingleCharacterCond(char target)
		{
			return GenerateCond<string>((val, ind, len) => new Tuple<bool, Segment>((val[ind] == target), new Segment(1, ind)));
		}	
		public static ShakeCondition<string> GenerateMultiCharacterCond(string lookingFor)
		{
			return GenerateCond<string>((val, ind, len) => 
					{
					int existence = val.IndexOf(lookingFor);
					bool result = existence != -1;
					Segment target = new Segment(lookingFor.Length, existence);
					return new Tuple<bool,Segment>(result,target);
					});
		}
		public static TypedShakeCondition<string> GenerateMultiCharacterTypedCond(string lookingFor, string type)
		{
			return GenerateTypedCond<string>((val, ind, len) =>
					{
					int v = val.IndexOf(lookingFor);
					bool result = v != -1;
					TypedSegment target = new TypedSegment(lookingFor.Length, type, v);
					return new Tuple<bool, TypedSegment>(result, target);
					});
		}
		public static TypedShakeCondition<T> GenerateTypedCond<T>(Func<T, int, int, Tuple<bool, TypedSegment>> actualCond)
		{
			return (x) => GenericTypedCond<T>(x, actualCond);
		}
		public static ShakeCondition<T> GenerateCond<T>(Func<T, int, int, Tuple<bool,Segment>> actualCond)
		{
			return (x) => GenericCond(x, actualCond);
		}
		public static TypedSegment GenericTypedCond<T>(Token<T> token, Func<T, int, int, Tuple<bool, TypedSegment>> actualCondition)
		{
			if(token != null)
			{
				T input = token.Value;
				for (int i = 0; i < token.Length; i++)
				{
					var result = actualCondition(input, i, token.Length);
					if (result.Item1)
						return result.Item2;
				}
			}
			return null;
		}
		public static Segment GenericCond<T>(Hunk<T> hunk, Func<T, int, int, Tuple<bool,Segment>> actualCondition)
		{
			if(hunk != null)
			{
				T input = hunk.Value;
				for(int i = 0; i < hunk.Length; i++)
				{
					var result = actualCondition(input,i,hunk.Length);	
					if(result.Item1)
						return result.Item2; 
				}
			}
			return null;
		}

		public static Segment GenericOffsetCond<T>(Hunk<T> hunk, Func<T, int, int, int, Tuple<bool, Segment>> aCond)
		{
			if(hunk != null)
			{
				var result = aCond(hunk.Value, 0, hunk.Length, hunk.Start);
				if(result.Item1)
					return result.Item2;
			}
			return null;
		}
		public static ShakeCondition<T> GenerateOffsetCond<T>(Func<T, int, int, int, Tuple<bool, Segment>> aCond)
		{
			return (x) => GenericOffsetCond(x, aCond);
		}
		public static Segment GenericRegexCond<T>(Hunk<T> hunk, Func<T, int, int, Tuple<bool, Segment>> aCond)
		{
			if(hunk != null)
			{
				var result = aCond(hunk.Value, 0, hunk.Length);
				if(result.Item1)
					return result.Item2;
			}
			return null;
		}
		public static TypedShakeCondition<T> GenerateTypedRegexCond<T>(Func<T, int, int, Tuple<bool, TypedSegment>> aCond)
		{
			return (x) => GenericTypedRegexCond(x, aCond);
		}
		public static TypedSegment GenericTypedRegexCond<T>(Token<T> tok, Func<T, int, int, Tuple<bool, TypedSegment>> aCond)
		{
			if(tok != null)
			{
				var result = aCond(tok.Value, 0, tok.Length);
				if (result.Item1)
					return result.Item2;
			}
			return null;
		}
		public static ShakeCondition<T> GenerateRegexCond<T>(Func<T, int, int, Tuple<bool, Segment>> aCond)
		{
			return (x) => GenericRegexCond(x, aCond);
		}
		public static IEnumerable<ShakeCondition<T>> GenerateConditions<T>(IEnumerable<Func<T, int, int, Tuple<bool, Segment>>> conds)
		{
			foreach(var cond in conds)
				yield return GenerateCond<T>(cond);
		}
		public static IEnumerable<ShakeCondition<string>> GenerateSingleCharacterConditions(params char[] elements)
		{
			foreach(var v in elements)
				yield return GenerateSingleCharacterCond(v);
		}
		public static IEnumerable<ShakeCondition<string>> GenerateMultiCharacterConditions(IEnumerable<string> elements)
		{
			foreach(var v in elements)
				yield return GenerateMultiCharacterCond(v);
		}
		public static IEnumerable<ShakeCondition<string>> GenerateMultiCharacterConditions(params string[] elements)
		{
			foreach(var v in elements)
				yield return GenerateMultiCharacterCond(v);
		}
	}
}
