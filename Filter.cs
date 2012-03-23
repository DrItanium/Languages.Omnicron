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
using System.Text.RegularExpressions;
using Frameworks.Plugin;
using Libraries.Messaging;
using Libraries.Collections;

namespace Libraries.Filter
{
  public abstract class Filter : Plugin, IFilter
  {
    public abstract string InputForm { get; } 
    protected Filter(string name) : base(name) { }
    public Hashtable Transform(Hashtable source)
    {
      if(source != null)
      {
        if(source["encoding"] is RunLengthEncoding<byte>)
          return TransformGreyscale(source);
        else if(source["encoding"] is RunLengthEncoding<int>)
          return TransformColor(source);
      }
      return null;
    }
    protected abstract Hashtable TransformColor(Hashtable source);
    protected abstract Hashtable TransformGreyscale(Hashtable source);
    public void TranslateData(Hashtable input)
    {
      //reencode this stuff 
      if(input["image"] is RunLengthEncoding<byte>)
      {
        input["encoding"] = input["image"];
        TranslateDataGreyscale(input);
      }
      else if(input["image"] is RunLengthEncoding<int>)
      {
        input["encoding"] = input["image"];
        TranslateDataColor(input);
      }
      else
        throw new Exception("Given image is not of the proper encoding");
    }
    protected virtual void TranslateDataColor(Hashtable source) { }
    protected virtual void TranslateDataGreyscale(Hashtable source) { }
    //TODO: Introduce code that just returns an encoded string
    //Since all that will be returned is the string itself and the
    //width and height; and an error code (potentially) why not just set it up as 
    //such. I bet that the encoding of the hashtable isn't that efficient.
    public override Message Invoke(Message input)
    {
      Hashtable table = (Hashtable)input.Value;
      TranslateData(table);
      return new Message(Guid.NewGuid(), ObjectID, input.Sender, 
          MessageOperationType.Return,
          Transform(table)); 
    }
  }
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FilterAttribute : PluginAttribute
  {
    public string PartOf { get; set; }
    public FilterAttribute(string name) 
      : base(name)
    {
    }
  }
}
