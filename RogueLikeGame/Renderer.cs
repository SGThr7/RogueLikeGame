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

			var text = sb.ToString();
			var diffIndexes = new List<int>();
			if (text.Length != buff.Length)
				Render(sb.ToString());
			for (int i = 0; i < text.Length; i++)
				if (text[i] != buff[i])
					diffIndexes.Add(i);
			(int w, int h) = MapManager.GetCurrentMapSize();
			foreach (int i in diffIndexes)
			{
				int y = i / w;
				int x = i - y * w;
				try
				{
					Console.SetCursorPosition(x, y);
					Console.Write(text[i]);
				}
				catch (ArgumentOutOfRangeException) { continue; }
			}
			buff = text;
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
			DrawPlayer(ref sb);
		}

		static void DrawPlayer(ref StringBuilder sb)
		{
			int index = (MapManager.GetCurrentMapSize().Width + 2) * GameManager.Player.Y + GameManager.Player.X;
			sb.Remove(index, 1).Insert(index, GameManager.Player.Symbol);
			//Console.SetCursorPosition(GameManager.Player.X, GameManager.Player.Y);
			//Console.Write(GameManager.Player.Symbol);
		}

		static void DrawMapFull(ref StringBuilder sb)
		{
			var map = MapManager.GetCurrentMap();
			(int width, int height) = map.Size;
			//var sb = new StringBuilder();
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
					sb.Append(map.GetMapSprite(x, y));
				sb.AppendLine();
			}
			//Console.SetCursorPosition(0, 0);
			//Console.WriteLine(sb.ToString());
		}
	}
}
