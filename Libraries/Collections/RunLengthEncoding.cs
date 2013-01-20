
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
using System.Runtime.Serialization;

namespace Libraries.Collections
{
	[Serializable]
		public class RunLengthEncodingEntry<T> : ISerializable
		where T : IComparable<T>
		{
			private ulong count; 
			private T value;
			public bool PartiallyTranslated { get; set; }
			public string TemporaryRepresentation { get; set; }
			public ulong Count { get { return count; } }
			public T Value { get { return value; } set { this.value = value; } }

			public RunLengthEncodingEntry(ulong count, T value) 
			{
				this.count = count; 
				this.value = value;
			}
			protected RunLengthEncodingEntry(SerializationInfo info, StreamingContext context)
			{
				count = info.GetUInt64("a");
				value = (T)info.GetValue("b", typeof(T));
			}
			public void CompleteTranslation(Func<string, T> conv)
			{
				PartiallyTranslated = false;
				value = conv(TemporaryRepresentation);
				TemporaryRepresentation = null;
			}
			public void IncrementCount() { count++; }
			public void DecrementCount() { count--; }
			public static RunLengthEncodingEntry<T> operator ++(RunLengthEncodingEntry<T> e)
			{
				e.IncrementCount();
				return e;
			}
			public static RunLengthEncodingEntry<T> operator --(RunLengthEncodingEntry<T> e)
			{
				e.DecrementCount();
				return e;
			}
			public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("a", count);
				info.AddValue("b", value, typeof(T));
			}
			public override string ToString()
			{
				return string.Format("{0},{1}", count, value);
			}
			public static RunLengthEncodingEntry<T> Parse(string input)
			{
				Regex r = new Regex(",");
				string[] s = r.Split(input);
				var q = new RunLengthEncodingEntry<T>(ulong.Parse(s[0]), default(T));
				q.TemporaryRepresentation = s[1];
				q.PartiallyTranslated = true;
				return q;
			}
		}
	[Serializable]
		public class RunLengthEncoding<T> : IEnumerable<T>, IEnumerable<RunLengthEncodingEntry<T>>, ISerializable
		where T : IComparable<T>
		{
			private ulong totalCount;
			public ulong TotalCount { get { return totalCount; } }
			private List<RunLengthEncodingEntry<T>> encoding = new List<RunLengthEncodingEntry<T>>();
			public static RunLengthEncoding<T> Encode(IEnumerable<T> enumeration)
			{
				RunLengthEncoding<T> enc = new RunLengthEncoding<T>();
				RunLengthEncodingEntry<T> curr = null; 
				foreach(var e in enumeration)
				{
					if(curr == null)
						curr = new RunLengthEncodingEntry<T>(1, e);
					else 
					{
						if(curr.Value.CompareTo(e) == 0)
							curr++;
						else
						{
							enc.encoding.Add(curr);
							curr = new RunLengthEncodingEntry<T>(1, e);
						}
					}
				}
				enc.encoding.Add(curr);
				return enc;
			}
			public static RunLengthEncoding<T> Encode(IEnumerable<RunLengthEncodingEntry<T>> enc)
			{
				RunLengthEncoding<T> e = new RunLengthEncoding<T>();
				foreach(var v in enc)
				{
					e.encoding.Add(v);
					e.totalCount += v.Count;
				}
				return e;
			}
			public static RunLengthEncoding<T> Encode(T[] array)
			{
				return Encode((IEnumerable<T>)array);
			}
			public static RunLengthEncoding<T> Encode(T[][] array)
			{
				RunLengthEncoding<T> enc = new RunLengthEncoding<T>();
				RunLengthEncodingEntry<T> curr = null; 
				enc.totalCount = (ulong)array.Length;
				for(int i = 0; i < array.Length; i++)
				{
					enc.totalCount += (ulong)array[i].Length;
					for(int j = 0; j < array[i].Length; j++)
					{
						var e = array[i][j];
						if(curr == null)
						{
							curr = new RunLengthEncodingEntry<T>(1, e);
							continue;
						}
						else if(curr.Value.CompareTo(e) != 0)
						{
							enc.encoding.Add(curr);
							curr = new RunLengthEncodingEntry<T>(1, e);
						}
						else 
							curr++;
					}
				}
				enc.encoding.Add(curr);
				return enc;
			}
			public RunLengthEncoding() 
			{ 
			}
			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("{0} ", totalCount);
				for(int i = 0; i < Count; i++)
					sb.AppendFormat("{0} ", this[i].ToString());
				return sb.ToString();
			}
			protected RunLengthEncoding(SerializationInfo info, StreamingContext cntxt)
			{
				string body = info.GetString("a");
				Regex r = new Regex(" ");
				string[] items = r.Split(body);
				totalCount = ulong.Parse(items[0]);
				for(int i = 1; i < items.Length; i++)
				{
					if(!items[i].Equals(string.Empty))
						encoding.Add(RunLengthEncodingEntry<T>.Parse(items[i]));
				}
				TranslationFinished = false;
			}
			public bool TranslationFinished { get; protected set; }
			public void CompleteTranslation(Func<string, T> translator)
			{
				TranslationFinished = true;
				for(int i = 0; i < Count; i++)
					this[i].CompleteTranslation(translator);
			}
			public int Count { get { return encoding.Count; } }
			public RunLengthEncodingEntry<T> this[int index] { get { return encoding[index]; } }
			public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("a", this.ToString());
			}
			public IEnumerator<RunLengthEncodingEntry<T>> GetEnumerator()
			{
				return encoding.GetEnumerator();
			}
			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return YieldContents().GetEnumerator();
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return (System.Collections.IEnumerator)GetEnumerator();
			}
			private IEnumerable<T> YieldContents()
			{
				foreach(var v in encoding)
				{
					ulong count = v.Count;
					for(ulong i = 0; i < count; i++)
						yield return v.Value;
				}
			}
			public IEnumerable<T> Expand()
			{
				return ((IEnumerable<T>)this);
			}
			public T[][] DestructiveExpand(int width, int height)
			{
				return GenericExpand(width, height, DestructiveExpand);
			}
			public T[][] Expand(int width, int height)
			{
				return GenericExpand(width, height, Expand);
			}

			private T[][] GenericExpand(int width, int height, ExpansionOperation<T> op)
			{
				T[][] elements = new T[width][];
				op(width, height, (x,y,v) => 
						{
						if(elements[x] == null) 
						elements[x] = new T[height];
						elements[x][y] = v;
						});
				return elements;
			}
			public void DestructiveExpand(int width, int height, Action<int,int,T> fn)
			{
				int q = 0;
				RunLengthEncodingEntry<T> curr = null;
				T val = default(T);
				for(int i = 0; i < width; i++)
				{
					for(int j = 0; j < height; j++)
					{
						if(curr == null || curr.Count == 0L)
						{
							curr = this[q++];
							val = curr.Value;
						}
						fn(i,j,val);
						curr--;
					}
				}
			}
			public void ExpandPartially(int width, int height,
					Func<int,int,T,bool> fn)
			{
				int q = 0;
				RunLengthEncodingEntry<T> curr = null;
				T val = default(T);
				ulong total = 0L; 
				for(int i = 0; i < width; i++)
				{
					for(int j = 0; j < height; j++)
					{
						if(total == 0L)
						{
							curr = this[q]; 
							val = curr.Value;
							total = curr.Count;
							q++;
						}
						if(!fn(i,j,val)) return;
						total--;
					}
				}
			}
			public void Expand(int width, int height, Action<int,int,T> fn)
			{
				int q = 0;
				RunLengthEncodingEntry<T> curr = null;
				T val = default(T);
				ulong total = 0L; 
				for(int i = 0; i < width; i++)
				{
					for(int j = 0; j < height; j++)
					{
						if(total == 0L)
						{
							curr = this[q]; 
							val = curr.Value;
							total = curr.Count;
							q++;
						}
						fn(i,j,val);
						total--;
					}
				}
			}
			private static Regex spaces = new Regex(" ");
			public static RunLengthEncoding<T> ParsePartially(string encoding)
			{
				RunLengthEncoding<T> enc = new RunLengthEncoding<T>();
				string[] elements = spaces.Split(encoding);
				enc.totalCount = ulong.Parse(elements[0]);
				for(int i = 1; i < elements.Length; i++)
				{
					if(!elements[i].Equals(string.Empty))

						enc.encoding.Add(RunLengthEncodingEntry<T>.Parse(elements[i]));    
				}
				return enc;
			}
			public static RunLengthEncoding<T> Parse(string encoding, Func<string, T> conv)
			{
				RunLengthEncoding<T> rle = ParsePartially(encoding);
				rle.CompleteTranslation(conv);
				return rle;
			}
			public T GetValueAtPosition(int width, int height, int x, int y)
			{
				//just do expand
				T result = default(T);
				ExpandPartially(width, height,
						(cX,cY, val) => 
						{
						bool cond = cX == x && cY == y;
						if(cond) result = val;
						return !cond; //if we succeded then return false, otherwise return true
						});
				return result;
			}
			public void Add(ulong count, T value)
			{
				encoding.Add(new RunLengthEncodingEntry<T>(count, value));
			}
		}
	public delegate void ExpansionOperation<T>(int width, int height, Action<int,int,T> fn);
}
