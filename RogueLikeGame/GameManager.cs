using System;

namespace RogueLikeGame
{
	static class GameManager
	{
		public static Player Player { get; private set; }

		static void Main(string[] args)
		{
			Initialize();
			Start();
		}

		public static void Initialize()
		{
			Console.CursorVisible = false;
			Player = new Player();
			Renderer.RenderFull();
		}

		public static void Start()
		{
			for (; ; )
			{
				var keyInfo = Console.ReadKey(true);
				var p = Player.Move(keyInfo.KeyChar);
				Renderer.Render();
			}
		}
	}
}
