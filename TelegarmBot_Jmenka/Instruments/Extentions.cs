using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegarmBot_Jmenka.Instruments
{
	public static class Extentions
	{
		public static TResult GetRandom<TResult>(this IEnumerable<TResult> results)
		{
			Random rnd = new Random();
			TResult[] arr = results.ToArray();
			return arr[rnd.Next(arr.Length)];
		}

		public static int GetIndex<TValue>(this IEnumerable<TValue> values, TValue value)
		{
			var array = values.ToList();
			for (int i = 0; i < array.Count; i++)
			{
				if ((object)array[i] == (object)value) return i;
			}	
			return -1;
		}

		public static void WriteArray<TValue>(this IEnumerable<TValue> values, string textBefor = "")
		{
            Console.Write(textBefor);
            foreach (TValue value in values)
			{
                Console.Write(value+" ");
            }
		}
	}
}
