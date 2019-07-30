using System;
using System.Collections.Generic;

namespace RogueLikeGame
{
	static class MapManager
	{
		private static List<Map> maps = new List<Map>();
		public static int CurrentMapNumber => maps.Count;
		public static Map GetCurrentMap()
		{
			if (CurrentMapNumber <= 0)
				Generate(100, 30);
			return maps[CurrentMapNumber - 1];
		}
		public static (int Width, int Height) GetCurrentMapSize() => GetCurrentMap().Size;
		public static Map GetMap(int index) => maps[index];

		public static void Generate(int width, int height)
		{
			var map = new Map(width, height);
			for (int y = 0; y < height; y++)
			{
				map.SetMapData(y, y, 1);
			}
			maps.Add(map);
		}
	}

	class Map
	{
		private static readonly char unknownSprite = '_';
		private readonly int[,] map;
		public char[] Sprites { get; private set; }

		public Map(int width, int height, char[] sprites)
		{
			this.map = new int[height, width];
			Sprites = sprites;
			(int w, int h) = Size;
			GenerateWall(0, 0, w - 1, h - 1);
		}

		public Map(int width, int height) : this(width, height, new char[] { ' ', '#', '.' })
		{
		}

		public Map(int size) : this(size, size)
		{
		}

		public Map(int size, char[] sprites) : this(size, size, sprites)
		{
		}

		public int Width => this.map.GetLength(1);
		public int Height => this.map.GetLength(0);
		public (int Width, int Height) Size => (Width, Height);
		public int GetMapData(int left, int top)
		{
			try { ValidPoint(left, top); }
			catch (Exception) { return -1; }
			return this.map[top, left];
		}

		public char GetMapSprite(int left, int top)
		{
			int data = GetMapData(left, top);
			if (data < 0 || Sprites.Length <= data)
				return unknownSprite;
			return Sprites[GetMapData(left, top)];
		}

		public void SetMapData(int left, int top, int data)
		{
			try { ValidPoint(left, top); }
			catch (Exception) { throw; }

			this.map[top, left] = data;
		}

		public void ValidPoint(int left, int top)
		{
			if (top < 0 || Height <= top || left < 0 || Width <= left)
				throw new Exception($"({left}, {top}) is out of map.");
		}

		public void GenerateWall(int left, int top, int width, int height)
		{
			for (int i = left; i <= width; i++)
			{
				SetMapData(i, top, 1);
				SetMapData(i, height, 1);
			}
			for (int i = top + 1; i <= height - 1; i++)
			{
				SetMapData(left, i, 1);
				SetMapData(width, i, 1);
			}
		}
	}
}
