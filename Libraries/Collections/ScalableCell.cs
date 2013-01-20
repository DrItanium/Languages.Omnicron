
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
	public class ScalableCell<R, T> : MultiCell<R, T> 
		where R : Cell<T>
	{
		public ScalableCell(Func<int, R> ctor, int size, int defaultCellSize) : base(ctor, size, defaultCellSize) { }
		public ScalableCell(Func<int, R> ctor, int size) : this(ctor, size, DEFAULT_CAPACITY) { }
		public ScalableCell(Func<int, R> ctor) : this(ctor, DEFAULT_CAPACITY) { }
		public ScalableCell(ScalableCell<R, T> cell) : base(cell) { }

		public new bool Add(T value)
		{
			bool result = base.Add(value);
			if(!result && (CurrentCell >= CellCount) && (CellCount < CellLength))
			{
				R newCell = ctor(DefaultCellSize);		
				Add(newCell);
				return Add(value);
			}
			return result;
		}
		public new bool AddRange(IEnumerable<T> values)
		{
			foreach(T v in values)
			{
				if(!Add(v))
					return false;	
			}
			return true;
		}
		public new void PerformFullCompression()
		{
			lock(compressionLock)
			{
				T[] newElements = ToArray();
				Clear();
				AddRange(newElements);
			}
		}
		public override object Clone()
		{
			return new ScalableCell<R, T>(this);
		}
	}
	public class ScalableCell<T> : ScalableCell<Cell<T>, T>
	{
		public ScalableCell(int numCells, int defaultCellSize)
			: base((x) => new Cell<T>(x), numCells, defaultCellSize)
		{

		}
		public ScalableCell(int numCells) : this(numCells, DEFAULT_CAPACITY) { }
		public ScalableCell() : this(DEFAULT_CAPACITY) { }

		public ScalableCell(ScalableCell<T> other) : base(other) { }

		public override object Clone() 
		{
			return new ScalableCell<T>(this); 
		}
	}
}
