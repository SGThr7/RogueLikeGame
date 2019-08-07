using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLikeGame
{
	internal static class Renderer
	{
		private static string buff;
		private const int UI_LEFT_LINE = 3;
		private const int UI_STATUS_LINE = 39;
		private const int UI_MESSAGE_LINE = 37;

		public static void RenderFull()
		{
			Console.Clear();
			Render();
		}

		public static void Render(bool isDiffOnly = false)
		{
			var mapData = new StringBuilder();
			Draw(ref mapData);
			if (!isDiffOnly)
			{
				Render(mapData.ToString());
				return;
			}

			var image = mapData.ToString();
			var diffIndexes = new List<int>();
			if (image.Length != buff.Length)
			{
				Render(mapData.ToString());
			}

			for (int i = 0; i < image.Length; i++)
			{
				if (image[i] != buff[i])
				{
					diffIndexes.Add(i);
				}
			}

			(int w, _) = MapManager.GetCurrentMapSize();
			foreach (int i in diffIndexes)
			{
				try
				{
					Console.SetCursorPosition(i % (w + 2), i / (w + 2));
					Console.Write(image[i]);
				}
				catch (ArgumentOutOfRangeException) { continue; }
			}
			buff = image;
		}

		public static void Render(string image)
		{
			Console.SetCursorPosition(0, 0);
			Console.Write(image);
			buff = image;
		}

		private static void ClearLine()
		{
			int left = Console.CursorLeft;
			int top = Console.CursorTop;
			Console.Write(new string(' ', Console.WindowWidth));
			Console.SetCursorPosition(left, top);
			//Console.SetCursorPosition(0, Console.CursorTop - 1);
		}

		private static void Draw(ref StringBuilder mapData)
		{
			DrawMapFull(ref mapData);
			DrawEnemys(ref mapData);
			DrawPlayer(ref mapData);
			DrawStatus();
		}

		private static void DrawPlayer(ref StringBuilder mapData)
		{
			int index = (MapManager.GetCurrentMapSize().Width + 2) * GameManager.Player.Y + GameManager.Player.X;
			mapData.Remove(index, 1).Insert(index, GameManager.Player.Symbol);
		}

		private static void DrawStatus()
		{
			Player player = GameManager.Player;
			Console.SetCursorPosition(UI_LEFT_LINE, UI_STATUS_LINE);
			Console.Write($"HP: {player.HP,3}/{player.MaxHP,3}, 満腹度: {player.Hunger,3}/{player.MaxHunger,3}");
		}

		private static void DrawEnemys(ref StringBuilder mapData)
		{
			foreach (var enemy in GameManager.Enemies)
			{
				int index = (MapManager.GetCurrentMapSize().Width + 2) * enemy.Y + enemy.X;
				mapData.Remove(index, 1).Insert(index, enemy.Symbol);
			}
		}

		private static void DrawMapFull(ref StringBuilder mapData)
		{
			var map = MapManager.CurrentMap;
			(int width, int height) = map.Size;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					mapData.Append(map.GetMapSprite(x, y));
				}

				mapData.AppendLine();
			}
		}

		public static void DrawMessage(string message)
		{
			Console.SetCursorPosition(UI_LEFT_LINE, UI_MESSAGE_LINE);
			ClearLine();
			Console.Write(message);
		}
	}
}
