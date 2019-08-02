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
		public static Map GetMap(int index)
			=> maps[index];

		public static void Generate()
			=> Generate(100, 35);

		public static void Generate(int width, int height)
		{
			var sprites = new List<MapSprite> {
					  new MapSprite(MapSprite.Type.Wall, '#'),
					  new MapSprite(MapSprite.Type.Floor, '.'),
					  new MapSprite(new MapSprite.Type[]{
										MapSprite.Type.Floor,
										MapSprite.Type.Room },
									',')
				  };
			var map = new Map(width, height, sprites);
			map.GenerateRoomRandom();
			for (int i = 0; i < 3; i++)
				map.GrowFloor();
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
		private const int MinimumRoomSize = 5;

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

		public IEnumerable<(int X, int Y)> GetRandomPoints(MapSprite.Type type)
		{
			List<int> ids = this.mapSprites.GetIDs(type);
			IEnumerable<(int item, int index)> maps = this.map.Indexed().Where(t => ids.Contains(t.item));
			maps = maps.OrderBy(a => Guid.NewGuid());
			foreach ((int mapID, int index) in maps)
			{
				yield return GetPosition(index);
			}
		}

		public IEnumerable<(int X, int Y)> GetRandomPoints(int id)
		{
			IEnumerable<(int mapID, int index)> maps = this.map.Indexed().Where(t => t.item == id);
			maps = maps.OrderBy(a => Guid.NewGuid());
			foreach ((int mapID, int index) in maps)
			{
				yield return GetPosition(index);
			}
		}

		public (int X, int Y) GetRandomPoint(MapSprite.Type type)
			=> GetRandomPoints(type).First();

		public (int X, int Y) GetRandomPoint(int id)
			=> GetRandomPoints(id).First();

		public (int X, int Y) GetRandomWall()
		{
			const int C = 8;
			foreach (var point in GetRandomPoints(MapSprite.Type.Wall))
			{
				var aroundSprites = GetAround(point.X, point.Y, C);
				for (int i = 0; i < C; i += C / 4)
				{
					if (aroundSprites[i].mapSprite.Is(MapSprite.Type.Floor) &&
						(aroundSprites[(i + 7) % C].mapSprite.Is(MapSprite.Type.Floor) ||
						aroundSprites[(i + 1) % C].mapSprite.Is(MapSprite.Type.Floor)))
					{
						return point;
					}
				}
			}
			return (-1, -1);
			//throw new KeyNotFoundException("Cannot found wall.");
		}

		public (MapSprite mapSprite, (int diffX, int diffY) position)[] GetAround(int left, int top, int count)
		{
			var list = new (MapSprite mapSprite, (int diffX, int diffY))[count];
			for (int i = 0; i < count; i++)
			{
				double d = count / 2d;
				int diffX = (int)Math.Round(Math.Sin(i / d * Math.PI), MidpointRounding.AwayFromZero);
				int diffY = (int)Math.Round(Math.Cos(i / d * Math.PI), MidpointRounding.AwayFromZero);
				list[i] = (this.mapSprites[this[left + diffX, top + diffY]], (diffX, diffY));
			}
			return list;
		}

		public (MapSprite mapSprite, (int diffX, int diffY) position)[] GetAround(int left, int top)
			=> GetAround(left, top, 4);

		public (MapSprite mapSprite, (int diffX, int diffY) position)[] GetAround((int x, int y) position, int count)
			=> GetAround(position.x, position.y, count);

		public (MapSprite mapSprite, (int diffX, int diffY) position)[] GetAround((int x, int y) position)
			=> GetAround(position.x, position.y);

		public (int diffX, int diffY) GetAngle(int left, int top, MapSprite.Type type)
		{
			(MapSprite mapSprite, (int diffX, int diffY) position)[] around = GetAround(left, top);
			foreach (var (mapSprite, position) in around)
			{
				if (mapSprite.Is(type))
				{
					return position;
				}
			}
			throw new KeyNotFoundException($"Cannot found {type.ToString()}.");
			//return (0, 0);
		}

		public (int diffX, int diffY) GetAngle((int X, int Y) position, MapSprite.Type type)
			=> GetAngle(position.X, position.Y, type);

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

		public void SetWall(int left, int top, int diffX, int diffY, int padding, MapSprite sprite, bool isReplace = true)
		{
			int x = (1 - Math.Abs(diffX)) * padding;
			int y = (1 - Math.Abs(diffY)) * padding;
			if (isReplace || this[left + x, top + y] == this.mapSprites.UnknownIndex)
				this[left + x, top + y] = this.mapSprites.GetID(sprite);
			if (isReplace || this[left - x, top - y] == this.mapSprites.UnknownIndex)
				this[left - x, top - y] = this.mapSprites.GetID(sprite);
		}

		public void SetWall(int left, int top, int diffX, int diffY, int padding)
			=> SetWall(left, top, diffX, diffY, padding, this.mapSprites[MapSprite.Type.Wall]);

		public void SetWall(int left, int top, int diffX, int diffY)
			=> SetWall(left, top, diffX, diffY, 1, this.mapSprites[MapSprite.Type.Wall]);

		public void GenerateOutline(int left, int top, int width, int height, MapSprite.Type type)
		{
			int sprite = this.mapSprites.GetID(type);
			for (int i = 0; i < width; i++)
			{
				this[left + i, top] = sprite;
				this[left + i, top + height - 1] = sprite;
			}
			for (int i = 0; i < height; i++)
			{
				this[left, top + i] = sprite;
				this[left + width - 1, top + i] = sprite;
			}
		}

		public void GenerateOutline(int left, int top, int width, int height, int padding, MapSprite.Type type)
			=> GenerateOutline(left - padding, top - padding, width + (2 * padding), height + (2 * padding), type);

		public void GenerateOutline(int left, int top, int width, int height, int padding, int borderWidth, MapSprite.Type type)
		{
			for (int i = padding; i < padding + borderWidth; i++)
			{
				GenerateOutline(left, top, width, height, i, type);
			}
		}

		public void GenerateRoom(int left, int top, int width, int height)
		{
			const int outlineWidth = 1;
			GenerateOutline(left, top, width, height, 1, outlineWidth, MapSprite.Type.AroundWall);
			GenerateOutline(left, top, width, height, MapSprite.Type.Wall);
			for (int y = 1; y < height - 1; y++)
			{
				for (int x = 1; x < width - 1; x++)
				{
					this[left + x, top + y] = this.mapSprites.GetID(new MapSprite.Type[] { MapSprite.Type.Floor, MapSprite.Type.Room });
				}
			}
		}

		public void GenerateRoom(int left, int top, int size)
			=> GenerateRoom(left, top, size, size);

		public void GenerateRoomRandom()
		{
			int x = GameManager.random.Next(Width - MinimumRoomSize);
			int y = GameManager.random.Next(Height - MinimumRoomSize);
			int size = GameManager.random.Next(MinimumRoomSize, Math.Min(Width - x + 1, Height - y + 1));
			GenerateRoom(x, y, size);
		}

		public void GrowFloor()
		{
			(int X, int Y) = GetRandomWall();
			this[X, Y] = this.mapSprites.GetID(MapSprite.Type.Floor);
			(int diffX, int diffY) = GetAngle(X, Y, MapSprite.Type.Floor);
			(int toX, int toY) = (X - diffX, Y - diffY);
			if (toX >= 0 && toX < Width && toY >= 0 && toY < Height)
			{
				for (int i = 0; ; i++)
				{
					this[toX, toY] = this.mapSprites.GetID(MapSprite.Type.Floor);
					SetWall(toX, toY, diffX, diffY);
					SetWall(toX, toY, diffX, diffY, 2, this.mapSprites[MapSprite.Type.AroundWall], false);
					toX -= diffX;
					toY -= diffY;
					if (toX < 0 || toX >= Width || toY < 0 || toY >= Height || this.mapSprites[this[toX, toY]].Is(MapSprite.Type.AroundWall))
						break;
					if (Math.Exp(-Math.Pow(i, 2) / Math.Pow(10, 2)) < GameManager.random.NextDouble())
						break;
				}
			}
			for (int i = 2; i >= 0; i--)
				SetWall(toX, toY, diffX, diffY, i, this.mapSprites[MapSprite.Type.AroundWall], false);
			toX += diffX;
			toY += diffY;
			this[toX, toY] = this.mapSprites.GetID(MapSprite.Type.Wall);
		}
	}
}
