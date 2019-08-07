using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
	internal static class SystemMessage
	{
		public static void Send(string message)
		{
			Renderer.DrawMessage(message);
			Console.ReadKey(true);
		}
	}
}
