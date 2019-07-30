using System;

namespace RogueLikeGame
{
	class GameManager
	{
		public Player Player { get; } = new Player();

		static void Main(string[] args)
		{
			var game = new GameManager();
		}

		public GameManager()
		{
			Console.CursorVisible = false;
			Renderer.Render(this);
			for (; ; )
			{
				var keyInfo = Console.ReadKey(true);
				var p = Player.Move(keyInfo.KeyChar);
				Renderer.Render(this);
			}
		}
	}
}
