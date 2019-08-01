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
					  new MapSprite(MapSprite.Type.Floor, '8')
				  };
			var map = new Map(width, height, sprites);
			map.GenerateRoomRandom();
			for (int i = 0; i < 10; i++)
			{
				(int X, int Y) pos = map.GetRandomPoint(MapSprite.Type.Floor);
				//map.SetMapData(pos, 3);
				map[pos] = 3;
			}
			maps.Add(map);
		}
	}

	class Map
	{
		private readonly int[] map;
		private readonly MapSprites mapSprites = new MapSprites();
		public int Width { get; }
		public int Height { get; }
		public (int Width, int Height) Size => (Width, Height);

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
				if (x < 0 || x >= Width || y < 0 || y >= Height)
					return -1;
				//throw new IndexOutOfRangeException();
				return this.map[(y * Width) + x];
			}
			set
			{
				if (x < 0 || x >= Width || y < 0 || y >= Height)
					throw new IndexOutOfRangeException();
				this.map[y * Width + x] = value;
			}
		}
		public int this[(int x, int y) position]
		{
			get { return this[position.x, position.y]; }
			set { this[position.x, position.y] = value; }
		}

		public MapSprite GetMapSprite(int left, int top) =>
			this.mapSprites[this[left, top]];

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

		public void ValidPoint(int left, int top)
		{
			if (top < 0 || Height <= top || left < 0 || Width <= left)
			{
				throw new Exception($"({left}, {top}) is out of map.");
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
			int x = GameManager.random.Next(0, Width - 3);
			int y = GameManager.random.Next(Height - 3);
			int size = GameManager.random.Next(3, Math.Min(Width - x + 1, Height - y + 1));
			GenerateRoom(x, y, size);
		}

		public IEnumerable<(int X, int Y)> FindSprite(MapSprite.Type type)
		{
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (this.mapSprites[this[x,y]].Is(type))
					{
						yield return (x, y);
					}
				}
			}
		}

		public (int X, int Y) GetRandomPoint(MapSprite.Type type)
		{
			IEnumerable<(int X, int Y)> spritePositions = FindSprite(type);
			int random = GameManager.random.Next(spritePositions.Count());
			return spritePositions.Index(random);
		}
	}
}
