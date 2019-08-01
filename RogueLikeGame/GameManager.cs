using System;

namespace RogueLikeGame
{
	static class GameManager
	{
		public static Random random = new Random();
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
			Player.RandomTeleport();
			Renderer.RenderFull();
		}

		public static void Start()
		{
			for (; ; )
			{
				var keyInfo = Console.ReadKey(true);
				var p = Player.Move(keyInfo.KeyChar);
				if (keyInfo.KeyChar == ';')
				{
					MapManager.Generate();
					Player.RandomTeleport();
				}
				Renderer.Render(true);
			}
		}
	}
}
