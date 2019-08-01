using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueLikeGame
{
	static class MapManager
	{
		private static List<Map> maps = new List<Map>();
		public static int CurrentMapNumber => maps.Count;
		public static Map CurrentMap
		{
			get
			{
				if (CurrentMapNumber <= 0)
				{
					Generate();
				}
				return maps[CurrentMapNumber - 1];
			}
		}

		public static (int Width, int Height) GetCurrentMapSize() => CurrentMap.Size;
		public static Map GetMap(int index) => maps[index];

		public static void Generate()
		{
			Generate(100, 30);
		}

		public static void Generate(int width, int height)
		{
			var sprites = new List<MapSprite> {
					  new MapSprite(MapSprite.Type.Wall, '#'),
					  new MapSprite(MapSprite.Type.Floor, '.'),
					  new MapSprite(MapSprite.Type.Floor, '.')
				  };
			var map = new Map(width, height, sprites);
			map.GenerateRoomRandom();
			for (int i = 0; i < 1; i++)
			{
				//(int X, int Y) pos = map.GetRandomPoint(map.mapSprites.GetSpriteID(MapSprite.Type.Wall));
				(int X, int Y) pos = map.GetRandomWall();
				map[pos] = 3;
			}
			maps.Add(map);
		}
	}

	class Map
	{
		private readonly int[] map;
		public readonly MapSprites mapSprites = new MapSprites();
		public int Width { get; }
		public int Height { get; }
		public (int Width, int Height) Size => (Width, Height);
		private const int MinimumRoomSize = 4;

		public Map(int width, int height, List<MapSprite> sprites)
		{
			this.map = new int[width * height];
			Width = width;
			Height = height;
			this.mapSprites.Add(sprites);
		}
		public Map(int width, int height)
			: this(width,
				  height,
				  new List<MapSprite> {
					  new MapSprite(MapSprite.Type.Wall, '#'),
					  new MapSprite(MapSprite.Type.Floor, '.')
				  })
		{
		}
		public Map(int size) : this(size, size)
		{
		}
		public Map(int size, List<MapSprite> sprites) : this(size, size, sprites)
		{
		}
		public int this[int x, int y]
		{
			get
			{
				if (0 <= x && x < Width && 0 <= y && y < Height)
					return this.map[(y * Width) + x];
				return -1;
			}
			set
			{
				if (0 <= x && x < Width && 0 <= y && y < Height)
					this.map[(y * Width) + x] = value;
			}
		}
		public int this[(int x, int y) position]
		{
			get { return this[position.x, position.y]; }
			set { this[position.x, position.y] = value; }
		}

		public (int X, int Y) GetPosition(int index) =>
			(index % Width, index / Width);

		public MapSprite GetMapSprite(int left, int top) =>
			this.mapSprites[this[left, top]];

		public (int X, int Y) GetRandomPoint(MapSprite.Type type)
		{
			List<int> ids = this.mapSprites.GetSpriteIDs(type);
			var indexes = this.map.Indexed().Where(t => ids.Contains(t.item));
			int random = GameManager.random.Next(indexes.Count());
			return GetPosition(indexes.ElementAt(random).index);
		}

		public (int X, int Y) GetRandomPoint(int id)
		{
			var indexes = this.map.Indexed().Where(t => t.item == id);
			if (indexes.Count() < 1) return (-1, -1);
			int random = GameManager.random.Next(indexes.Count());
			return GetPosition(indexes.ElementAt(random).index);
		}

		public (int X, int Y) GetRandomWall()
		{
			while (true)
			{
				var point = GetRandomPoint(MapSprite.Type.Wall);
				bool isCorner = false;
				for (int i = 0; i < 4; i++)
				{
					int x1 = point.X + (int)Math.Round(Math.Sin(i / 2d * Math.PI), MidpointRounding.AwayFromZero);
					int y1 = point.Y + (int)Math.Round(Math.Cos(i / 2d * Math.PI), MidpointRounding.AwayFromZero);
					int x2 = point.X + (int)Math.Round(Math.Sin((i + 1) / 2d * Math.PI), MidpointRounding.AwayFromZero);
					int y2 = point.Y + (int)Math.Round(Math.Cos((i + 1) / 2d * Math.PI), MidpointRounding.AwayFromZero);
					if (this.mapSprites[this[x1, y1]].Is(MapSprite.Type.Wall) && this.mapSprites[this[x2, y2]].Is(MapSprite.Type.Wall))
						isCorner = true;
					if (isCorner) break;
				}
				if (!isCorner) return point;
			}
		}

		public void ReplaceMapData(int source, int target)
		{
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (this[x, y] == source)
					{
						this[x, y] = target;
					}
				}
			}
		}

		public void GenerateWall(int left, int top, int width, int height)
		{
			int wall = this.mapSprites.GetID(MapSprite.Type.Wall);
			for (int i = 0; i < width; i++)
			{
				this[left + i, top] = wall;
				this[left + i, top + height - 1] = wall;
			}
			for (int i = 0; i < height; i++)
			{
				this[left, top + i] = wall;
				this[left + width - 1, top + i] = wall;
			}
		}

		public void GenerateRoom(int left, int top, int width, int height)
		{
			GenerateWall(left, top, width, height);
			for (int y = 1; y < height - 1; y++)
			{
				for (int x = 1; x < width - 1; x++)
				{
					this[left + x, top + y] = this.mapSprites.GetID(MapSprite.Type.Floor);
				}
			}
		}

		public void GenerateRoom(int left, int top, int size)
		{
			GenerateRoom(left, top, size, size);
		}

		public void GenerateRoomRandom()
		{
			int x = GameManager.random.Next(Width - MinimumRoomSize);
			int y = GameManager.random.Next(Height - MinimumRoomSize);
			int size = GameManager.random.Next(MinimumRoomSize, Math.Min(Width - x + 1, Height - y + 1));
			GenerateRoom(x, y, size);
		}
	}
}
