using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLikeGame
{
	static class Renderer
	{
		private static string buff;

		public static void RenderFull()
		{
			Console.Clear();
			Render();
		}

		public static void Render(bool isDiffOnly = false)
		{
			var sb = new StringBuilder();
			Draw(ref sb);
			if (!isDiffOnly)
			{
				Render(sb.ToString());
				return;
			}

			var image = sb.ToString();
			var diffIndexes = new List<int>();
			if (image.Length != buff.Length)
			{
				Render(sb.ToString());
			}

			for (int i = 0; i < image.Length; i++)
			{
				if (image[i] != buff[i])
				{
					diffIndexes.Add(i);
				}
			}

			(int w, int h) = MapManager.GetCurrentMapSize();
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

		static void Draw(ref StringBuilder sb)
		{
			DrawMapFull(ref sb);
			DrawEnemys(ref sb);
			DrawPlayer(ref sb);
		}

		static void DrawPlayer(ref StringBuilder sb)
		{
			int index = (MapManager.GetCurrentMapSize().Width + 2) * GameManager.Player.Y + GameManager.Player.X;
			sb.Remove(index, 1).Insert(index, GameManager.Player.Symbol);
		}

		static void DrawEnemys(ref StringBuilder sb)
		{
			foreach (var enemy in GameManager.Enemys)
			{
				int index = (MapManager.GetCurrentMapSize().Width + 2) * enemy.Y + enemy.X;
				sb.Remove(index, 1).Insert(index, enemy.Symbol);
			}
		}

		static void DrawMapFull(ref StringBuilder sb)
		{
			var map = MapManager.CurrentMap;
			(int width, int height) = map.Size;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					//sb.Append(map.GetVisibleMapSprite(x, y));
					sb.Append(map.GetMapSprite(x, y));
				}

				sb.AppendLine();
			}
		}
	}
}
