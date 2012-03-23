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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Collections;
using Libraries.Filter;
using Frameworks.Plugin;
using Libraries.LexicalAnalysis;
using Libraries.Extensions;
using Libraries.Parsing;
using Libraries.Starlight;
using Libraries.Tycho;
using Libraries.Collections;
using msg = Libraries.Messaging;
using System.Text.RegularExpressions;

namespace Libraries.Filter 
{
  public static class TransmissionEncoder
  {
    public static int LoadByte(byte value, Stream encoder)
    {
      //if it's false then a compression code was used
      if(value == (byte)0)
        return WriteCode((byte)10, encoder);
      else if(value == (byte)255)
        return WriteCode((byte)45, encoder);
      else if(value == (byte)128)
        return WriteCode((byte)46, encoder);
      else if(value == (byte)127)
        return WriteCode((byte)47, encoder);
      else if(value < 10)
        return WriteCode((byte)value, encoder);
      else
        return BasicEncode((byte)0, new byte[1] { value }, encoder);
    }
    public static int LoadInt(int value, Stream encoder)
    {
      if(value == 0)
        return WriteCode((byte)21, encoder);
      else if(value == 127)
        return WriteCode((byte)50, encoder);
      else if(value == 128)
        return WriteCode((byte)49, encoder);
      else if(value == 255)
        return WriteCode((byte)48, encoder);
      else if(value < 10)
        return WriteCode((byte)(value + 11), encoder);
      else
        return BasicEncode((byte)11, BitConverter.GetBytes(value), encoder);
    }
    public static int LoadLong(long value, Stream encoder)
    {
      if(value == 0L)
        return WriteCode((byte)32, encoder);
      else if(value < 10L)
        return WriteCode((byte)(value + 22), encoder);
      else
      {
        return BasicEncode((byte)22,
            BitConverter.GetBytes(value),
            encoder);
      }
    }
    public static int WriteCode(byte code, Stream encoder)
    {
      encoder.WriteByte(code);
      return 1;
    }
    public static int BasicEncode(byte code, byte[] values, Stream encoder)
    {
      encoder.WriteByte(code);
      encoder.Write(values, 0, values.Length);
      return 1 + values.Length;
    }
    public static int LoadChar(char value, Stream encoder)
    {
			switch(value)
			{
				case '0':
					return WriteCode((byte)60, encoder);
				case '1':
					return WriteCode((byte)61, encoder);
				case '2':
					return WriteCode((byte)62, encoder);
				case '3':
					return WriteCode((byte)63, encoder);
				case '4':
					return WriteCode((byte)64, encoder);
				case '5':
					return WriteCode((byte)65, encoder);
				case '6':
					return WriteCode((byte)66, encoder);
				case '7':
					return WriteCode((byte)67, encoder);
				case '8':
					return WriteCode((byte)68, encoder);
				case '9':
					return WriteCode((byte)69, encoder);
				default:
      		return BasicEncode((byte)33, BitConverter.GetBytes(value), encoder);
			}
    }
    public static int LoadString(string message, Stream encoder)
    {
      if(message.Equals("srcWidth"))
        return WriteCode((byte)51, encoder);
      else if(message.Equals("srcHeight"))
        return WriteCode((byte)52, encoder);
      else if(message.Equals("image"))
        return WriteCode((byte)53, encoder);
			else if(message.Equals("width"))
				return WriteCode((byte)54, encoder);
			else if(message.Equals("height"))
				return WriteCode((byte)55, encoder);
			else if(message.Equals("result"))
				return WriteCode((byte)56, encoder);
			else if(message.Equals("terminate"))
				return WriteCode((byte)57, encoder);
			else if(message.Equals("mask"))
				return WriteCode((byte)58, encoder);
			else if(message.Equals("encoding"))
				return WriteCode((byte)59, encoder);
      else
      {
        int total = 0;
        for(int i = message.Length - 1; i >= 0; i--)
        {
          int result = LoadChar(message[i], encoder); 
          total += result;
        }
        int length = LoadInt(message.Length, encoder);
        total += length;
        total += WriteCode((byte)37, encoder);
        return total; 
      }
    }
    public static int LoadGUID(Guid value, Stream encoder)
    {
      return BasicEncode((byte)36, value.ToByteArray(), encoder);
    }
    public static int LoadBool(bool value, Stream encoder)
    {
      return value ? LoadTrue(encoder) : LoadFalse(encoder);
    }
    public static int LoadFalse(Stream encoder)
    {
      return WriteCode((byte)35, encoder);
    }
    public static int LoadTrue(Stream encoder)
    {
      return WriteCode((byte)34, encoder);
    }
    public static int LoadIntCell(RunLengthEncodingEntry<int> entry, Stream encoder)
    {
       int total = 0;
       int length0 = LoadInt(entry.Value, encoder);
       total += length0;
       int length1 = LoadLong((long)entry.Count, encoder);
       total += length1;
       total += WriteCode((byte)38, encoder);
       return total;
    }
    public static int LoadByteCell(RunLengthEncodingEntry<byte> entry, Stream encoder)
    {
       int total = 0;
       int length0 = LoadByte(entry.Value, encoder);
       total += length0;
       int length1 = LoadLong((long)entry.Count, encoder);
       total += length1;
       total += WriteCode((byte)39, encoder);
       return total;
    }
    public static int LoadIntRunLengthEncoding(RunLengthEncoding<int> enc, Stream encoder)
    {
      int result = 0;
      int total = 0; 
      for(int i = enc.Count - 1; i >= 0; i--)
      {
        result = LoadIntCell(enc[i], encoder);
        total += result;
      }
      result = LoadInt(enc.Count, encoder);
      total += result;
       total += WriteCode((byte)40, encoder);
      return total;
    }
    public static int LoadByteRunLengthEncoding(RunLengthEncoding<byte> enc, Stream encoder)
    {
      int result = 0;
      int total = 0; 
      for(int i = enc.Count - 1; i >= 0; i--)
      {
        result = LoadByteCell(enc[i], encoder);
        total += result;
      }
      result = LoadInt(enc.Count, encoder);
      total += result;
       total += WriteCode((byte)41, encoder);
      return total;
    }
    public static int LoadSingle(float value, Stream encoder)
    {
      return BasicEncode((byte)42,
          BitConverter.GetBytes(value), 
          encoder);
    }
    public static int LoadDouble(double value, Stream encoder)
    {
      return BasicEncode((byte)43, 
          BitConverter.GetBytes(value),
          encoder);
    }
    public static int PutIntoHashtable<T>(string datum, Func<Stream, int> fn, Stream encoder)
    {
      int total = 0;
      int result = fn(encoder);
      total += result;
      result = LoadString(datum, encoder);
      total += result;
      total += WriteCode((byte)44, encoder);
      return total;
    }
    public static int PutIntoHashtable(string datum, byte value, Stream encoder)
    {
      return PutIntoHashtable<byte>(datum, (a) => LoadByte(value, a), encoder);
    }
    public static int PutIntoHashtable(string datum, int value, Stream encoder)
    {
      return PutIntoHashtable<int>(datum, (a) => LoadInt(value, a), encoder);
    }
    public static int PutIntoHashtable(string datum, long value, Stream encoder)
    {
      return PutIntoHashtable<long>(datum, (a) => LoadLong(value, a), encoder);
    }
    public static int PutIntoHashtable(string datum, float value, Stream encoder)
    {
      return PutIntoHashtable<float>(datum, (a) => LoadSingle(value, a), encoder);
    }
    public static int PutIntoHashtable(string datum, double value, Stream encoder)
    {
      return PutIntoHashtable<double>(datum, (a) => LoadDouble(value, a), encoder);
    }
    public static int PutIntoHashtable(string datum, decimal value, Stream encoder)
    {
      return PutIntoHashtable(datum, (double)value, encoder);
    }
    public static int PutIntoHashtable(string datum, bool value, Stream encoder)
    {
      return PutIntoHashtable<bool>(datum, (a) => LoadBool(value, a), encoder);
    }
    public static int PutIntoHashtable(string datum, RunLengthEncoding<int> rle, Stream encoder)
    {
      return PutIntoHashtable<RunLengthEncoding<int>>(datum, (a) => LoadIntRunLengthEncoding(rle, a), encoder);
    }
    public static int PutIntoHashtable(string datum, RunLengthEncoding<byte> rle, Stream encoder)
    {
      return PutIntoHashtable<RunLengthEncoding<byte>>(datum, (a) => LoadByteRunLengthEncoding(rle, a), 
          encoder);
    }
    public static int PutIntoHashtable(string datum, string value, Stream encoder)
    {
      return PutIntoHashtable<string>(datum, (a) => LoadString(value, a), encoder);
    }

  }
}
