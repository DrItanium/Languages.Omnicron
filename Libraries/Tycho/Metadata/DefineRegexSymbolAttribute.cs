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
using Libraries.LexicalAnalysis;
using Libraries.Extensions;
using Libraries.Collections;

namespace Libraries.Tycho.Metadata
{
	public class DefineRegexSymbolAttribute : DefineWordAttribute
	{
		public DefineRegexSymbolAttribute(string name, string type, string expression)
			: base(expression, type)
		{
			Name = name;
            
		}	
		public override Word DefineWord()
		{
			return new RegexSymbol(TargetWord, Name, TargetWord);
		}
	}
	public abstract class DefineGenericRegexAttribute : DefineRegexSymbolAttribute
	{
		public string Addendum { get; private set; }
		public DefineGenericRegexAttribute(string name, string type, string addendum)
			: base(name, type, string.Empty)
		{
			Addendum = addendum;
			Group = "Literals";
		}
		public DefineGenericRegexAttribute(string name, string type)
			: this(name, type, string.Empty)
		{
		}
		public sealed override Word DefineWord()
		{
			return DefineWord_Impl();
		}
		protected abstract Word DefineWord_Impl();
	}

	public class DefineGenericFloatingPointNumberAttribute : DefineGenericRegexAttribute 
	{
		public DefineGenericFloatingPointNumberAttribute(string name, string type, string addendum) : base(name, type, addendum) { }
		public DefineGenericFloatingPointNumberAttribute(string name, string type) : base(name, type) { }
		protected override Word DefineWord_Impl()
		{
			return new GenericFloatingPointNumber(Name, WordType, Addendum);
		}
	}
	public class DefineGenericIntegerAttribute : DefineGenericRegexAttribute 
	{
		public DefineGenericIntegerAttribute(string name, string type, string addendum) : base(name, type, addendum) { }
		public DefineGenericIntegerAttribute(string name, string type) : base(name, type) { }

		protected override Word DefineWord_Impl()
		{
			return new GenericInteger(Name, WordType, Addendum);
		}
	}
	public class DefineStringLiteralAttribute : DefineRegexSymbolAttribute 
	{
		public string Before { get; private set; }
		public DefineStringLiteralAttribute(string name, string type, string expression, string before)
			: base(name, type, expression)
		{
			Before = before;
		//	Expression = string.Format("{0}{1}", before, expression);
		}
		public DefineStringLiteralAttribute(string name, string type, string expression)
			: this(name, type, expression, string.Empty)
		{
		}
		public DefineStringLiteralAttribute(string name, string type)
			: this(name, type, string.Empty)
		{
		}
		public DefineStringLiteralAttribute(string name)
			: this(name, "string-literal")
		{
		}
		public DefineStringLiteralAttribute()
			: this("String Literal")
		{
		}
		public override Word DefineWord()
		{
			if(TargetWord.Equals(string.Empty))
				TargetWord = ModifiableStringLiteral.MODIFIABLE_IDENTIFIER;
			return new ModifiableStringLiteral(TargetWord, Name, WordType, Before);
		}
	}
	public class DefineCharacterLiteralAttribute : DefineRegexSymbolAttribute 
	{
		public DefineCharacterLiteralAttribute(string name, 
				string type, string expression)
			: base(name, type, expression)
		{
		}
		public DefineCharacterLiteralAttribute(string name,
				string type) 
			: this(name, type, string.Empty)
		{
		}
		public DefineCharacterLiteralAttribute(string name)
			: this(name, "char-literal")
		{
		}
		public DefineCharacterLiteralAttribute()
			: this("Character Literal")
		{

		}

		public override Word DefineWord()
		{
			if(TargetWord.Equals(string.Empty))
				TargetWord = CharacterSymbol.DEFAULT_EXPRESSION;
			return new CharacterSymbol(TargetWord, Name, WordType);
		}
	}

}
