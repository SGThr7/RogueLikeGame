using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLikeGame
{
	internal static class Renderer
	{
		private static string buff;

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

		static void Draw(ref StringBuilder mapData)
		{
			DrawMapFull(ref mapData);
			DrawEnemys(ref mapData);
			DrawPlayer(ref mapData);
			DrawStatus();
		}

		static void DrawPlayer(ref StringBuilder mapData)
		{
			int index = (MapManager.GetCurrentMapSize().Width + 2) * GameManager.Player.Y + GameManager.Player.X;
			mapData.Remove(index, 1).Insert(index, GameManager.Player.Symbol);
		}

		static void DrawStatus()
		{
			Player player = GameManager.Player;
			Console.SetCursorPosition(3, 39);
			Console.Write($"HP {player.HP} / {player.MaxHP}");
		}

		static void DrawEnemys(ref StringBuilder mapData)
		{
			foreach (var enemy in GameManager.Enemies)
			{
				int index = (MapManager.GetCurrentMapSize().Width + 2) * enemy.Y + enemy.X;
				mapData.Remove(index, 1).Insert(index, enemy.Symbol);
			}
		}

		static void DrawMapFull(ref StringBuilder mapData)
		{
			var map = MapManager.CurrentMap;
			(int width, int height) = map.Size;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					//sb.Append(map.GetVisibleMapSprite(x, y));
					mapData.Append(map.GetMapSprite(x, y));
				}

				mapData.AppendLine();
			}
		}
	}
}
