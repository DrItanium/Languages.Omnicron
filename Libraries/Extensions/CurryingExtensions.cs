//Copyright 2010 Joshua Scoggins. All rights reserved.
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
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Libraries.Extensions
{
#if CURRYING
  public static partial class Extensions
  {
    public static Func<R> Curry<T,R>(this Func<T,R> f, T value) 
    {
      return () => f(value); 
    }
    public static Func<T1, Func<T2, R>> Curry<T1, T2, R>(this Func<T1,T2,R> f)
    {
      return x => y => f(x,y);
    }
    public static Func<T1,Func<T2, Func<T3, R>>> Curry<T1, T2, T3, R>(this Func<T1,T2,T3,R> f)
    {
      return x => y => z => f(x,y,z);
    }
    public static Func<T1,Func<T2, Func<T3, Func<T4,R>>>> Curry<T1,T2,T3,T4,R>(this Func<T1,T2,T3,T4,R> f)
    {
      return x => y => z => w => f(x,y,z,w);
    }

    //---------Begin .NET 4.0 Currying Functions----
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, R>>>>> Curry<T1,T2,T3,T4,T5,R>(this Func<T1,T2,T3,T4,T5,R> f)
    {
      return x => y => z => w => h => f(x,y,z,w,h);
    }

    //---------Begin Action Currying Functions----

    public static Action Curry<T>(this Action<T> f, T value) 
    {
      return () => f(value); 
    }
    public static Func<T1, Action<T2>> Curry<T1,T2>(this Action<T1,T2> f)
    {
      return x => y => f(x,y);
    }
    public static Func<T1, Func<T2, Action<T3>>> Curry<T1,T2,T3>(this Action<T1,T2,T3> f)
    {
      return x => y => z => f(x,y,z);
    }
    public static Func<T1, Func<T2, Func<T3, Action<T4>>>> Curry<T1,T2,T3,T4>(this Action<T1,T2,T3,T4> f)
    {
      return x => y => z => w => f(x,y,z,w);
    }

    public static Func<T1, Func<T2, Func<T3, Func<T4, Action<T5>>>>> Curry<T1,T2,T3,T4,T5>(this Action<T1,T2,T3,T4,T5> f)
    {
      return x => y => z => w => h => f(x,y,z,w,h);
    }

  }
#endif
}
