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
using System.Linq;
using System.Text;
using Libraries.LexicalAnalysis;

namespace Libraries.Tycho
{
    public class Language : IEnumerable<TypedShakeCondition<string>>, IEnumerable<Word>
    {
        private List<Comment> comments;
        private List<Symbol> symbols;
        private List<RegexSymbol> regexSymbols;
        private StringOrganizer keywords;
        private List<Word> customActions;
				private IdSymbol id;
        public string Name { get; protected set;  }
        public string Version { get; protected set; }
        public string Description { get; protected set; }
        public Language(string name, string version, string idType,
						IEnumerable<Comment> comments, 
						IEnumerable<Symbol> symbols, 
						IEnumerable<RegexSymbol> regexSymbols, 
						IEnumerable<Keyword> keywords, 
						IEnumerable<Word> rest)
        {
            this.comments = new List<Comment>(comments);
            this.symbols = new List<Symbol>(symbols);
            this.regexSymbols = new List<RegexSymbol>(regexSymbols);
            this.keywords = new StringOrganizer(keywords);
            this.customActions = new List<Word>(rest);
						id = new IdSymbol(idType);
            Name = name;
            Version = version;
        }
        public Language(string name, string version, string idType, IEnumerable<Comment> comments, IEnumerable<Symbol> symbols, IEnumerable<RegexSymbol> regexSymbols, IEnumerable<Keyword> keywords)
            : this(name, version, idType, comments, symbols, regexSymbols, keywords, new Word[0])
        {

        }

        public void AddCustomAction(Word customAction)
        {
            customActions.Add(customAction);
        }
        private static IEnumerable<TypedShakeCondition<string>> Convert(IEnumerable<Word> words)
        {
            return (from x in words
                    select x.AsTypedShakeCondition());
        }

        public IEnumerator<TypedShakeCondition<string>> GetEnumerator()
        {
            var a = Convert(comments);
            var b = Convert(symbols);
            var c = Convert(regexSymbols);
            var d = keywords.GetTypedShakeConditions();
            var e = Convert(customActions);
						var idy = new[] { id.AsTypedShakeCondition() };
            return (a.Concat(c).Concat(b).Concat(d).Concat(e).Concat(idy)).GetEnumerator();
        }
        IEnumerator<Word> IEnumerable<Word>.GetEnumerator()
        {
            
            IEnumerable<Word> a = (comments);
            IEnumerable<Word> b = (symbols);
            IEnumerable<Word> c = (regexSymbols);
            IEnumerable<Word> d = keywords;
            IEnumerable<Word> e = (customActions);
            return a.Concat(c).Concat(b).Concat(d).Concat(e).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
