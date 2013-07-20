
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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace Libraries.Collections
{
	/*
		 A Multicell is a cell that stores cells
		 this allows for techniques such as the principle of
		 locality and a segmented memory model to take place
		 as each cell can be located in different areas of memory
		 and the reference is stored in the given area.

		 The super type of a multi cell is cell and operating on the
		 super type will allow one to modify the cells instead of the
		 data they store. 

		 A multicell must shadow:
		 - indexer operator
		 - the length property
		 - the count property
		 - the IsFull property
		 - the Expand method 
		 - the GetEnumerator method to return IEnumerable<T>
		 instead of R

		 It must add the following as well
		 - A consolidate operator to covert the multiple cells into
		 a single large cell
		 - a new Add method for data elements of type t
		 - a new Remove method for data elements of type t
		 - the Exists method for data elements of type t
		 - the indexof method for data elements of type T
		 - a new contains method for data elements of type T

		 Prototype interface 
		 --------------------------
		 public interface IMultiCell<R, T> : ICell<R>, IEnumerable<T>
		 where R : ICell<T>
		 {
		 new T this[int index] { get; }	
		 new int Length { get; }
		 new int Count { get; }
		 new bool IsFull { get; }
		 new void Expand(int newSize);
		 new IEnumerator<T> GetEnumerator();
		 Cell<T> Consolidate();	
		 bool Add(T value);
		 bool Remove(T value);
		 int IndexOf(T value);
		 bool Exists(Predicate<T> predicate);
		 bool Contains(T value);
		 }
	 */
	public class MultiCell<R, T> : Cell<R> , IEnumerable<T>
	 	where R : Cell<T> 
		{
			protected Func<int, R> ctor;
			private int lastCell, defaultCellSize, numElements, capacity;
			public int DefaultCellSize { get { return defaultCellSize; } set { defaultCellSize = value; } }
			public int CurrentCell { get { return lastCell; } protected set { lastCell = value; } }
			public new int Length { get { return capacity; } protected set { capacity = value; } }
			public new int Count { get { return numElements; } protected set { numElements = value; } }
			public int CellCount { get { return base.Count; } protected set { base.Count = value; } }
			public int CellLength { get { return base.Length; } } 
			public new bool IsFull { get { return Count == Length; } }
			public Func<int, R> CellConstructor { get { return ctor; } protected set { ctor = value; } } 
			public MultiCell(Func<int, R> ctor, int size, int defaultCellSize)
				: base(size)
			{
				this.defaultCellSize = defaultCellSize;
				lastCell = 0;
				this.ctor = ctor;
				base.Add(ctor(defaultCellSize));
				Length = backingStore[0].Length;
			}
			public MultiCell(Func<int, R> ctor, int size)
				: this(ctor, size, DEFAULT_CAPACITY)
			{

			}
			public MultiCell(Func<int, R> ctor)
				: this(ctor, DEFAULT_CAPACITY)
			{

			}

			public MultiCell(MultiCell<R, T> cell)
				: base(cell.Length)
			{
				this.ctor = cell.ctor;
				this.defaultCellSize = cell.defaultCellSize;
				this.lastCell = cell.lastCell;
				for(int i = 0; i < cell.Length; i++)
					this.backingStore[i] = (R)cell.backingStore[i].Clone();
			}
			public bool AddRange(IEnumerable<T> elements)
			{
				foreach(T v in elements)
					if(!Add(v))
						return false;
				return true;
			}
			public bool Add(T value)
			{
				//this can fail horribly
				if(lastCell >= base.Count)
					return false;
				bool result = backingStore[lastCell].Add(value);
				if(result)
				{
					numElements++;
					return true;
				}
				else
				{
					lastCell++;
					return Add(value);
				}
			}
			public new bool Remove(R value)
			{
				bool result = base.Remove(value);
				if(result)
				{
					Length -= value.Length;
					Count -= value.Count;
				}
				return result;
			}
			public bool Remove(T value)
			{
				int index = IndexOf(value);
				bool result = (index == -1);
				if(!result)
				{
					R target = base[index];
					result = target.Remove(value);
					if(result)
						Count--;
					if(target.Count == 0 && lastCell > 0)
						lastCell--; //go to the previous one so that if the removal continues we can properly handle this
				}
				return result;
			}
			public int IndexOf(T value)
			{
				int index = 0;
				Predicate<T> subElement = (y) =>
				{
					bool result = y.Equals(value);
					if(!result)
						index++;
					return result;
				};
				Predicate<R> p = (x) => x.Exists(subElement);
				return Exists(p) ? index : -1;
			}
			protected object compressionLock = new object();
			public new T[] ToArray()
			{
				List<T> elements = new List<T>();
				for(int i = 0; i < base.Count; i++)
				{
					R value = base[i];
					for(int j = 0; j < value.Count; j++)
						elements.Add(value[j]);
				}
				return elements.ToArray();
			}
			public void PerformFullCompression()
			{
				lock(compressionLock)
				{
					T[] newElements = ToArray();
					Clear();
					AddRange(newElements);
				}
			}
			public new void Expand(int newSize)
			{
				if(newSize < Length)
					throw new ArgumentException("Given new cell count is smaller than current count");
				else if(newSize == Length)
					return;
				else
				{
					int increase = newSize - Length;
					R newCell = ctor(increase);
					bool result = Add(newCell);
					if(!result)
					{
						base.Expand(base.Length + 2);
						Add(newCell);
					}
					Length += increase;
				}
			}	
			public override object Clone()
			{
				return new MultiCell<R, T>(this);
			}
			public new void Clear()
			{
				CellCount = 0;
				lastCell = 0;
				Count = 0;
				Length = 0;
				for(int i = 0; i < CellLength; i++)
					base[i].Clear();
			}
			public void ExpandCells(int newCellCount)
			{
				if (newCellCount < base.Length)
					throw new ArgumentException("Given new size is smaller than current size");
				else if (newCellCount == base.Length)
					return;
				else
					base.Expand(newCellCount);
			}
			public bool Exists(Predicate<T> pred)
			{
				Predicate<R> pr = (y) => y.Exists(pred);
				return Exists(pr);
			}
			public bool Contains(T value)
			{
				Predicate<T> pred = (x) => x.Equals(value);
				return Exists(pred);
			}
			/*

				 When overloading this[int index] its important to convert the 
				 index so that it spans across the different cells automatically
			 */	 

			public new T this[int index] 
			{
				get
				{
					KeyValuePair<int, int> relativePos = TranslateIndex(index);
					if(relativePos.Key == -1 && relativePos.Value == -1)
						return default(T);
					else
						return backingStore[relativePos.Key][relativePos.Value];
				}
				set
				{
					KeyValuePair<int, int> relativePos = TranslateIndex(index);
					if(relativePos.Key != -1 && relativePos.Value != -1)
						backingStore[relativePos.Key][relativePos.Value] = value;
				}
			}
			public T this[int cellNumber, int index] 
			{
				get 
				{
					return base[cellNumber][index]; 
				}
				set 
				{
					base[cellNumber][index] = value; 
				} 
			}
			public KeyValuePair<int, int> TranslateIndex(int index)
			{
				//Alright here is what we do... we start at the current
				for(int i = 0; i < base.Count; i++)
				{
					R value = base[i];
					if(index < value.Count)
						return new KeyValuePair<int, int>(i, index);
					else
						index = index - value.Count;
				}
				return new KeyValuePair<int, int>(-1, -1);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			public new IEnumerator<T> GetEnumerator()
			{
				return new MultiCellEnumerator(this);
			}
			public class MultiCellEnumerator : IEnumerator<T>
			{
				private	R[] backingElements;
				private int count, bIndex;
				private IEnumerator<T> current;
				public MultiCellEnumerator(MultiCell<R, T> mc)
				{
					backingElements = mc.backingStore;
					count = mc.CellCount;
					bIndex = -1;
				}
				object IEnumerator.Current { get { return Current; } }
				public T Current { get { return current.Current; } }
				public bool MoveNext()
				{
					if(bIndex == -1)
					{
						bIndex++;
						if(bIndex < count)
							current = backingElements[bIndex].GetEnumerator();
						return current.MoveNext();
					}
					else
					{
						bool result = current.MoveNext();
						if(result)
							return result;
						else
						{
							bIndex++;
							bool result2 = bIndex < count;
							if(result2)
							{
								current = backingElements[bIndex].GetEnumerator();
								return current.MoveNext();
							}
							return result2;
						}
					}
				}	
				public void Reset()
				{
					bIndex = -1;
				}
				public void Dispose()
				{
					if(current != null)
						current.Dispose();
					backingElements = null;	
				}
			}		
		}
	public class MultiCell<T> : MultiCell<Cell<T>, T>
	{
		public MultiCell(int numCells, int defaultCellSize) : base((x) => new Cell<T>(x), numCells, defaultCellSize) { }
		public MultiCell(int numCells) : base((x) => new Cell<T>(x), numCells, DEFAULT_CAPACITY) { }
		public MultiCell() : base((x) => new Cell<T>(x), DEFAULT_CAPACITY) { }
		public MultiCell(MultiCell<T> cells) : base(cells) { }
		public override object Clone()
		{
			return new MultiCell<T>(this);
		}
	}
}
