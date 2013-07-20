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
using Libraries.LexicalAnalysis;

namespace Libraries.Tycho
{
    public abstract class Word : IComparable<Word>, ICloneable, IComparable<string>
    {
        public string TargetWord { get; protected set; }
        public string WordType { get; protected set; }
        public Word(string input, string type)
        {
            TargetWord = input;
            WordType = type;
        }

        public Word(string input)
            : this(input, string.Empty)
        {
        }

        public abstract ShakeCondition<string> AsShakeCondition();
        public abstract TypedShakeCondition<string> AsTypedShakeCondition();
        /*
        public virtual TypedShakeCondition<string> AsTypedShakeCondition()
        {
            var fn = AsShakeCondition();
            return (x) => new TypedSegment(fn(x), WordType);         
        }
         */
        

        public virtual int CompareTo(Word other)
        {
            return TargetWord.CompareTo(other.TargetWord) + WordType.CompareTo(other.WordType);
        }

        public virtual int CompareTo(string other)
        {
            return TargetWord.CompareTo(other);
        }

        public abstract object Clone();

        public override int GetHashCode()
        {
            return TargetWord.GetHashCode() + WordType.GetHashCode();
        }

        public override bool Equals(object other)
        {
            Word w = (Word)other;
            return TargetWord.Equals(w.TargetWord) && WordType.Equals(w.WordType);
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", TargetWord, WordType);
        }

        public static implicit operator ShakeCondition<string>(Word k)
        {
            return k.AsShakeCondition();
        }
    }
}
