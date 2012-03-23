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
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using Frameworks.Plugin;
using Libraries.Messaging;
using Libraries.Collections;
using System.IO;

namespace Libraries.Filter
{
	public class FilterStreamInterpreter 
	{
		private Stream communications;
		private Action[] invocationTable;
		public FilterStreamInterpreter(Stream communications)
		{
			this.communications = communications;
			invocationTable = new Action[]
			{
				LoadByte,
					() => LoadSpecificByte(1), 
					() => LoadSpecificByte(2),
					() => LoadSpecificByte(3), 
					() => LoadSpecificByte(4),
					() => LoadSpecificByte(5), 
					() => LoadSpecificByte(6),
					() => LoadSpecificByte(7), 
					() => LoadSpecificByte(8),
					() => LoadSpecificByte(9), 
					() => LoadSpecificByte(0),
					LoadInt,
					() => LoadSpecificInt(1), 
					() => LoadSpecificInt(2),
					() => LoadSpecificInt(3),
					() => LoadSpecificInt(4),
					() => LoadSpecificInt(5), 
					() => LoadSpecificInt(6),
					() => LoadSpecificInt(7), 
					() => LoadSpecificInt(8),
					() => LoadSpecificInt(9), 
					() => LoadSpecificInt(0),
					LoadLong,
					() => LoadSpecificLong(1L), 
					() => LoadSpecificLong(2L),
					() => LoadSpecificLong(3L), 
					() => LoadSpecificLong(4L),
					() => LoadSpecificLong(5L), 
					() => LoadSpecificLong(6L),
					() => LoadSpecificLong(7L), 
					() => LoadSpecificLong(8L),
					() => LoadSpecificLong(9L), 
					() => LoadSpecificLong(0L),
					LoadCharacter, 
					LoadTrue, 
					LoadFalse, 
					LoadGUID, 
					LoadString, 
					NewIntCell, 
					NewByteCell, 
					NewIntRunLengthEncoding, 
					NewByteRunLengthEncoding, 
					LoadFloat32,
					LoadFloat64, 
					() => PutIntoHashtable(h),
					() => LoadSpecificByte(255),  // 45
					() => LoadSpecificByte(128), //46
					() => LoadSpecificByte(127),  //47
					() => LoadSpecificInt(255), //48
					() => LoadSpecificInt(128), //49
					() => LoadSpecificInt(127), //50
					() => LoadSpecificString("srcWidth"), //51
					() => LoadSpecificString("srcHeight"), //52
					() => LoadSpecificString("image"), //53
					() => LoadSpecificString("width"), //54
					() => LoadSpecificString("height"), //55
					() => LoadSpecificString("result"), //56
					() => LoadSpecificString("terminate"), //57
					() => LoadSpecificString("mask"), //58
					() => LoadSpecificString("encoding"), //59
					() => LoadSpecificCharacter('0'),//60 
					() => LoadSpecificCharacter('1'),//61
					() => LoadSpecificCharacter('2'),//62
					() => LoadSpecificCharacter('3'),//63
					() => LoadSpecificCharacter('4'),//64
					() => LoadSpecificCharacter('5'),//65
					() => LoadSpecificCharacter('6'),//66
					() => LoadSpecificCharacter('7'),//67
					() => LoadSpecificCharacter('8'),//68
					() => LoadSpecificCharacter('9'),//69

			};
		}
		private Hashtable h = new Hashtable();
		private Stack<object> dataStack = new Stack<object>();
		///<summary>
		/// Reads the memory stream and decodes the given input
		/// values using the custom encoding I have defined
		///</summary>
		public Hashtable CreateData()
		{
			h.Clear();
			dataStack.Clear();
			//see EncodingScheme for description
			int value = communications.ReadByte();
			while(value != 255 && value != -1)
			{
				if(value > invocationTable.Length)
					Console.WriteLine("value {0} is out of range...skipping", value);
				else
				{
				invocationTable[value]();
				value = communications.ReadByte();
				}
			}
			return h;
		}
		public void LoadTrue()
		{
			dataStack.Push(true);
		}
		public void LoadFalse()
		{
			dataStack.Push(false);
		}
		public void LoadByte()
		{
			communications.Read(temp, 0, 1);
			dataStack.Push((byte)temp[0]);
		}
		byte[] temp = new byte[16];
		public void LoadFloat32()
		{
			communications.Read(temp, 0, 4);
			dataStack.Push(BitConverter.ToSingle(temp, 0));
		}
		public void LoadFloat64()
		{
			communications.Read(temp, 0, 8);
			dataStack.Push(BitConverter.ToDouble(temp, 0));
		}
		public void LoadInt()
		{
			communications.Read(temp, 0, 4); 
			dataStack.Push(BitConverter.ToInt32(temp,0));
		}
		public void LoadLong()
		{
			communications.Read(temp, 0, 8);
			dataStack.Push(BitConverter.ToInt64(temp, 0));
		}
		public void LoadCharacter()
		{
			communications.Read(temp, 0, 2);
			dataStack.Push(BitConverter.ToChar(temp, 0));
		}
		public void LoadGUID()
		{
			communications.Read(temp, 0, 16);
			dataStack.Push(new Guid(temp));
		}
		public void NewIntCell()
		{
			//pull a ulong and int off the stack
			ulong count = (ulong)(long)dataStack.Pop();
			int value = (int)dataStack.Pop();
			dataStack.Push(new RunLengthEncodingEntry<int>(count, value));
		}
		public void PutIntoHashtable(Hashtable target)
		{
			string name = (string)dataStack.Pop();
			object value = dataStack.Pop();
			target[name] = value;
		}
		public void NewByteCell()
		{
			ulong count = (ulong)(long)dataStack.Pop(); //double layer cast :)
			byte value = (byte)dataStack.Pop();
			var rle = new RunLengthEncodingEntry<byte>(count, value);
			dataStack.Push(rle);
		}
		private IEnumerable<RunLengthEncodingEntry<T>> YieldValues<T>(int length)
			where T : IComparable<T>
			{
				//it will be put in backwards so it shouldn't be too hard to get this correct
				for(int i = 0; i < length; i++)
				{
					var rlet = (RunLengthEncodingEntry<T>)dataStack.Pop();
					yield return rlet;
				}
			}
		public void NewIntRunLengthEncoding()
		{
			int length = (int)dataStack.Pop();
			RunLengthEncoding<int> enc = RunLengthEncoding<int>.Encode(YieldValues<int>(length));
			dataStack.Push(enc);
		}
		public void NewByteRunLengthEncoding()
		{
			int length = (int)dataStack.Pop();
			RunLengthEncoding<byte> enc = RunLengthEncoding<byte>.Encode(YieldValues<byte>(length));
			dataStack.Push(enc);
		}
		public void LoadString()
		{
			StringBuilder sb = new StringBuilder();
			int length = (int)dataStack.Pop();
			//assume that it's in backwards
			for(int i = 0; i < length; i++)
				sb.Append((char)dataStack.Pop());
			dataStack.Push(sb.ToString());
		}
		public void LoadSpecificString(string value) { dataStack.Push(value); }
		public void LoadSpecificByte(byte value) { dataStack.Push(value); }
		public void LoadSpecificInt(int value) { dataStack.Push(value); } 
		public void LoadSpecificLong(long value) { dataStack.Push(value); }
		public void LoadSpecificCharacter(char value) { dataStack.Push(value); }
	}
}
