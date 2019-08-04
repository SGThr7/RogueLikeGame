using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
	static class AdvanceMethods
	{
		/// <summary>
		/// Return indexed items.
		/// </summary>
		public static IEnumerable<(T item, int index)> Indexed<T>(this IEnumerable<T> source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			IEnumerable<(T item, int index)> impl()
			{
				int i = 0;
				foreach (T item in source)
				{
					yield return (item, i++);
				}
			}
			return impl();
		}

		public static (int min, int max) MinMax(this (int num1, int num2) nums) 
			=> (Math.Min(nums.num1, nums.num2), Math.Max(nums.num1, nums.num2));
	}
}
