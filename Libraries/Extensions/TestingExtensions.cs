using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Libraries.Extensions
{
#if TESTING_EXTENSIONS
	public static partial class TestingExtensions 
	{
		public static long GetMemorySize(bool garbageCollectFirst)
		{
			return GC.GetTotalMemory(garbageCollectFirst);
		}
		public static long GetMemorySize()
		{
			return GetMemorySize(true);
		}
		public static long GetMemorySize(long initialSize, bool garbageCollectFirst)
		{
			return GetMemorySize(garbageCollectFirst) - initialSize;
		}	
		public static long GetMemorySize(long initialSize)
		{
			return GetMemorySize(initialSize, true);
		}
		public static long GetMemorySize(IEnumerable<long> subtractAgainst, bool garbageCollectFirst)
		{
			long value = GetMemorySize(garbageCollectFirst);
			foreach(var sa in subtractAgainst)
				value -= sa;
			return value;
		}
		public static long GetMemorySize(IEnumerable<long> subtractAgainst)
		{
			return GetMemorySize(subtractAgainst, true);
		}


		public static long GetInittedSize<T>(out T result, bool garbageCollectFirst, Func<T> allocator)
		{
			long starting = GetMemorySize(garbageCollectFirst);
			result = allocator();
			long final = GetMemorySize(starting, garbageCollectFirst);
			return final;
		}
		public static long GetInittedSize<T>(out T result, bool garbageCollectFirst)
			where T : new()
		{
			return GetInittedSize<T>(out result, garbageCollectFirst, () => new T());
		}
		public static long GetInittedSize<T>(out T result)
			where T : new()
		{
			return GetInittedSize<T>(out result, true);
		}
		public static TimeSpan GetTimeDifference(this DateTime start)
		{
			return DateTime.Now - start;
		}
	}
#endif
}
