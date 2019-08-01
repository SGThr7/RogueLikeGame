using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
	static class AdvanceMethods
	{
		public static T Index<T>(this IEnumerable<T> self, int index)
		{
			try { return self.Skip(index).Take(1).SingleOrDefault(); }
			catch (Exception) { throw; }
		}
	}
}
