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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Libraries.Extensions
{
#if STRING_EXTENSIONS
	public static partial class Extensions
	{
		public static bool TryAnyEquals(this string value, out int index,
				StringComparison comparison, params string[] against)
		{
			for (int i = 0; i < against.Length; i++)
			{
				if (value.Equals(against[i], comparison))
				{
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}
		public static bool AnyEquals(this string value, StringComparison comparison, params string[] against)
		{
			int i;
			return TryAnyEquals(value, out i, comparison, against);
		}

		public static bool EqualsIgnoreCase(this string value, string other)
		{
			return value.Equals(other, StringComparison.OrdinalIgnoreCase);
		}
		public static bool AnyEqualsIgnoreCase(this string value, params string[] other)
		{
			return AnyEquals(value, StringComparison.OrdinalIgnoreCase, other);
		}
		public static bool TryAnyEqualsIgnoreCase(this string value, out int index, params string[] others)
		{
			return TryAnyEquals(value, out index, StringComparison.OrdinalIgnoreCase, others);
		}
	}
#endif
}
