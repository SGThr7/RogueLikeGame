using System;

namespace RogueLikeGame
{
	static class Renderer
	{
		public static void Render(GameManager game)
		{
			Console.Clear();
			DrawPlayer(game);
		}

		static void DrawPlayer(GameManager game)
		{
			Console.SetCursorPosition(game.Player.X, game.Player.Y);
			Console.Write(game.Player.Simbol);
		}
	}
}
