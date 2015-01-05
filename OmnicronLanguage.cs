//Copyright 2012-2015 Joshua Scoggins. All rights reserved.
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
using System.Text;
using System.Runtime;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Libraries.LexicalAnalysis;
using Libraries.Extensions;
using Libraries.Parsing;
using Libraries.Starlight;
using Libraries.Tycho;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Languages.Omnicron 
{
	public class OmnicronLanguage 
	{
		private static object[] EmptyArray = new object[0];
		private Stack<object> dataStack;  
		private object Push(object input)
		{
			dataStack.Push(input);
			return EmptyArray;
		}
		private EnhancedLR1ParsableLanguage language;
		public OmnicronLanguage()
		{
			dataStack = new Stack<object>(); 
			EnhancedGrammar grammar = new EnhancedGrammar
			{
				new Rule("S'")
				{
					new Production { "Block" },
				},
					new Rule("Block")
					{
						new Production { "Rest", "return" },
					},
					new Rule("Rest")
					{
						new Production { "BlockStatements", },
					},
					new Rule("BlockStatements" )
					{
						new Production{ "BlockStatements", "BlockStatement" }, 
						new Production{ "BlockStatement" },
					},
					new Rule("BlockStatement")
					{
						new Production { "Word" },
					},
					new Rule("Word")
					{
						new Production { "Action" },
						new Production { "Type" }, 
					},
					new Rule("Type")
					{
						new Production((x) => { Push(int.Parse(x[0].ToString())); return EmptyArray; }) { "int" },
						new Production((x) => { Push(long.Parse(x[0].ToString())); return EmptyArray; }) { "long" },
						new Production((x) => { Push(float.Parse(x[0].ToString())); return EmptyArray; }) { "float" },
						new Production((x) => { Push(double.Parse(x[0].ToString())); return EmptyArray; }) { "double" },
						new Production((x) => { Push(x[0].ToString().Replace("\"",string.Empty)); return EmptyArray; }) { "string-literal" },
						new Production((x) => { Push(x[0].ToString().Replace("\"",string.Empty)); return EmptyArray; }) { "id" },
						new Production((x) => { Push(true); return EmptyArray; }) { "true" }, 
						new Production((x) => { Push(false); return EmptyArray; }) { "false" }, 
					},
					new Rule("Action")
					{
						new Production(NewItem) { "new" },
						new Production(ImbueItem) { "imbue" },
						new Production(NewPoint) { "point" },
						new Production(NewSize) { "size" },
					},

			};
			language = new EnhancedLR1ParsableLanguage(
					"Dynamic Form Constructor Language",
					"1.0.0.0",
					IdSymbol.DEFAULT_IDENTIFIER,
					new Comment[]
					{
					new Comment("single-line-comment", "--", "\n", "\""),
					},
					new Symbol[]
					{
					new Symbol('\n', "Newline", "<new-line>"),
					new Symbol(' ', "Space", "<space>" ),
					new Symbol('\t', "Tab", "<tab>"),
					new Symbol('\r', "Carriage Return", "<cr>"),
					},
					new RegexSymbol[]
					{
					new StringLiteral(),
					new CharacterSymbol(),
					new GenericFloatingPointNumber("Single Precision Floating Point Number", "[fF]", "single"),
					new GenericFloatingPointNumber("Double Precision Floating Point Number", "double"),
					new GenericInteger("Long", "[Ll]", "long"),
					new GenericInteger("Int", "int"),
					},
					new Keyword[]
					{
						new Keyword("new"),
						new Keyword("imbue"),
						new Keyword("point"),
						new Keyword("size"),
						new Keyword("extract"),
						new Keyword("true"),
						new Keyword("false"),
						new Keyword("return"),
					},
					LexicalExtensions.DefaultTypedStringSelector,
					grammar,
					"$",
					(x) => 
					{
						object result = dataStack.Pop();
						if(result is DynamicForm)
						{
							DynamicForm d = (DynamicForm)result;
							d.ResumeLayout(false);
							return d;
						}
						else
						{
							throw new ArgumentException("A form should always be returned from any of these scripts!");
						}
					},
					true,
					IsValid);
		}
		public DynamicForm ConstructForm(string input)
		{
			dataStack = new Stack<object>();
			return (DynamicForm)language.Parse(input);
		}
		private object NewPoint(object[] input)
		{
			int y = (int)dataStack.Pop();
			int x = (int)dataStack.Pop();
			dataStack.Push(new Point(x,y));
			return EmptyArray;
		}
		private object NewSize(object[] input)
		{
			int y = (int)dataStack.Pop();
			int x = (int)dataStack.Pop();
			dataStack.Push(new Size(x,y));
			return EmptyArray;
		}
		private object ImbueItem(object[] input)
		{
			//takes in an object and a corresponding item to imbue it into
			string name = (string)dataStack.Pop();
			object data = dataStack.Pop();
			Control target = (Control)dataStack.Pop();
			switch(name.Replace("\"",string.Empty))
			{
				case "Location":
					try
					{
						target.Location = (Point)data;
						break;
					}
					catch(InvalidCastException)
					{
						//find out some information
						throw new OmnicronException(
								string.Format("ERROR: expected a Point type for Location field for {0} of type {1}, got a {1} instead",
									target.Name, target.GetType().Name, data.GetType().Name));
					}
				case "Size":
					try
					{
						target.Size = (Size)data;
						break;
					}
					catch(InvalidCastException)
					{
						//find out some information
						throw new OmnicronException(
								string.Format("ERROR: expected a Size type for Size field for {0} of type {1}, got a {1} instead",
									target.Name, target.GetType().Name, data.GetType().Name));
					}
				case "Name":
					try
					{
						target.Name = (string)data;
						break;
					}
					catch(InvalidCastException)
					{
						//find out some information
						throw new OmnicronException(
								string.Format("ERROR: expected a string type for Name field for {0} of type {1}, got a {1} instead",
									target.Name, target.GetType().Name, data.GetType().Name));
					}
				case "Visible":
					try
					{
						target.Visible = (bool)data;
						break;
					}
					catch(InvalidCastException)
					{
						//find out some information
						throw new OmnicronException(
								string.Format("ERROR: expected a bool type for Visible field for {0} of type {1}, got a {1} instead",
									target.Name, target.GetType().Name, data.GetType().Name));
					}
				case "Text":
					try
					{
						target.Text = (string)data;
						break;
					}
					catch(InvalidCastException)
					{
						//find out some information
						throw new OmnicronException(
								string.Format("ERROR: expected a string type for Text field for {0} of type {1}, got a {1} instead",
									target.Name, target.GetType().Name, data.GetType().Name));
					}
				case "Controls.Add":
					Control c = (Control)data;
					if(c is Button)
					{
						DynamicForm t = (DynamicForm)target;
						t.ButtonOpensFileDialog((Button)c, true);   
					}
					else
					{
						target.Controls.Add(c);
					}
					if(!(c is Label) && target is DynamicForm)
						((DynamicForm)target).StorageCells[c.Name] = null;
					break;
				case "Items.Add":
					ComboBox cmbo = (ComboBox)target;
					cmbo.Items.Add(data.ToString());
					break;
				default:
					throw new OmnicronException(
							string.Format("Unknown Data Element: {0}", name));
			}
			dataStack.Push(target);
			return EmptyArray;
		}
		private object NewItem(object[] input)
		{
			//ignore the contents 
			//this should be an id or string 
			string str = dataStack.Pop().ToString();
			switch(str)
			{
				case "form":
					DynamicForm d = new DynamicForm();
					d.SuspendLayout();
					dataStack.Push(d);
					break;
				case "label":
					dataStack.Push(new Label());
					break;
				case "combobox":
					dataStack.Push(new ComboBox());
					break;
				case "textbox":
					dataStack.Push(new TextBox());
					break;
				case "button":
					dataStack.Push(new Button());
					break;
				case "checkbox":
					dataStack.Push(new CheckBox());
					break;
				case "richtextbox":
					dataStack.Push(new RichTextBox());
					break;
				default:
					throw new ArgumentException("Unknown type given");
			}
			return EmptyArray;
		}
		public bool IsValid(Token<string> input)
		{
			switch(input.TokenType)
			{
				case "<cr>":
				case "<space>":
				case "<new-line>":
				case "<tab>":
				case "multi-line-comment":
				case "single-line-comment":
					return false;
				default: 
					return true;
			}
		}
		public static bool Equals(Dictionary<string, LR1ParsingTableCell> a, 
				Dictionary<string, LR1ParsingTableCell> b)
		{
			bool result = true;
			foreach(var pairA in a)
			{
				var aCell = pairA.Value;
				var bCell = b[pairA.Key];
				result &= (aCell.Equals(bCell));
			}
			return result;
		}
	}
}
