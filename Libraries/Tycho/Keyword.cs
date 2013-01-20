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
#undef NET4
#define NET35
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
    public class Keyword : Word, IComparable<Keyword>
    {
        public bool RequiresEqualityCheck { get; set; }
        public Keyword(string input, bool requiresEqualityCheck = false)
            : base(input, input)
        {
            RequiresEqualityCheck = requiresEqualityCheck;
        }
        public override ShakeCondition<string> AsShakeCondition()
        {
            var fn = (RequiresEqualityCheck) ? 
              (LexicalExtensions.GenerateCond<string>(
                        (val, ind, len) => new Tuple<bool, Segment>(val.Equals(TargetWord), new Segment(TargetWord.Length, ind)))) : LexicalExtensions.GenerateMultiCharacterCond(TargetWord);
            return (x) => x.Value.Contains(TargetWord) ? fn(x) : null; //fuck it
        }
        public override TypedShakeCondition<string> AsTypedShakeCondition()
        {
            TypedShakeCondition<string> fn = null;
            if (RequiresEqualityCheck)
            {
                fn = LexicalExtensions.GenerateTypedCond<string>(
                (val, ind, len) =>
                {
                    bool condition = val.Equals(TargetWord);
                    TypedSegment seg = new TypedSegment(TargetWord.Length, WordType, ind);
                    return new Tuple<bool, TypedSegment>(condition, seg);
                } );
            }
            else
            {
                fn = LexicalExtensions.GenerateMultiCharacterTypedCond(TargetWord, TargetWord);
            }
            return (x) =>
                {
                    bool result = x.Value.Contains(TargetWord);
                    return result ? fn(x) : null;  
                };
            //return (x) => x.Value.Contains(TargetWord) ? fn(x) : null;
        }
        public override object Clone()
        {
            return new Keyword(TargetWord, RequiresEqualityCheck);
        }
        public override int GetHashCode()
        {
            return RequiresEqualityCheck.GetHashCode() + base.GetHashCode();
        }
        public virtual int CompareTo(Keyword other)
        {
            return RequiresEqualityCheck.CompareTo(other.RequiresEqualityCheck) 
                + base.CompareTo((Word)other);
        }
        
    }
}
