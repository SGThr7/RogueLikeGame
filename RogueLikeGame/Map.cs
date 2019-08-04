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
									','),
					  new MapSprite(MapSprite.Type.Debug,'=')
				  };
			var map = new Map(width, height, sprites);
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
		private const int MinimumRoomSize = 6;

		public Map(int width, int height, List<MapSprite> sprites)
		{
			this.map = new int[width * height];
			Width = width;
			Height = height;
			this.mapSprites.Add(sprites);
			this.GenerateMap();
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

		public IEnumerable<(int X, int Y)> GetRandomSpritePoints(MapSprite.Type type)
		{
			List<int> ids = this.mapSprites.GetIDs(type);
			IEnumerable<(int item, int index)> maps = this.map.Indexed().Where(t => ids.Contains(t.item));
			maps = maps.OrderBy(a => Guid.NewGuid());
			foreach ((int mapID, int index) in maps)
			{
				yield return GetPosition(index);
			}
		}

		public IEnumerable<(int X, int Y)> GetRandomSpritePoints(int id)
		{
			IEnumerable<(int mapID, int index)> maps = this.map.Indexed().Where(t => t.item == id);
			maps = maps.OrderBy(a => Guid.NewGuid());
			foreach ((int mapID, int index) in maps)
			{
				yield return GetPosition(index);
			}
		}

		public (int X, int Y) GetRandomSpritePoint(MapSprite.Type type)
			=> GetRandomSpritePoints(type).First();

		public (int X, int Y) GetRandomSpritePoint(int id)
			=> GetRandomSpritePoints(id).First();

		public (int X, int Y) GetRandomPoint()
			=> (GameManager.random.Next(Width), GameManager.random.Next(Height));

		public (int X, int Y) GetRandomPoint(int minimum)
			=> (GameManager.random.Next(minimum, Width - minimum), GameManager.random.Next(minimum, Height - minimum));

		public (int X, int Y) GetRandomPoint(int left, int top, int right, int bottom)
		{
			(int fromX, int toX) = (left, right).MinMax();
			(int fromY, int toY) = (top, bottom).MinMax();
			return (GameManager.random.Next(fromX, toX), GameManager.random.Next(fromY, toY));
		}

		public (int X, int Y) GetRandomWall()
		{
			const int C = 8;
			foreach (var point in GetRandomSpritePoints(MapSprite.Type.Wall))
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

		/// <summary>
		/// Set sprite if isReplace == true or old sprite is UnknownSprite
		/// </summary>
		/// <returns>If set sprite, return true.</returns>
		public bool SetSprite(int left, int top, int value, bool isReplace)
		{
			if (isReplace || this[left, top] == this.mapSprites.UnknownIndex)
			{
				this[left, top] = value;
				return true;
			}
			return false;
		}

		private void GenerateMap()
		{
			(int X, int Y) = GetRandomPoint(MinimumRoomSize + 1);
			for (int i = 0; i < Width; i++)
				this[i, Y] = this.mapSprites.GetID(MapSprite.Type.Debug);
			for (int i = 0; i < Height; i++)
				this[X, i] = this.mapSprites.GetID(MapSprite.Type.Debug);
			//GenerateRoomRandom(0, 0, X - 1, Y - 1);
			//GenerateRoomRandom(X + 1, 0, Width, Y - 1);
			//GenerateRoomRandom(0, Y + 1, X - 1, Height);
			//GenerateRoomRandom(X + 1, Y + 1, Width, Height);
			for (int i = 0; i < 4; i++)
			{
				int left = i % 2 == 0 ? 0 : X + 1;
				int right = i % 2 == 0 ? X - 1 : Width;
				int top = i / 2 == 0 ? 0 : Y + 1;
				int bottom = i / 2 == 0 ? Y - 1 : Height;
				System.Diagnostics.Debug.WriteLine($"{X}, {Y}, {left}-{right}, {top}-{bottom}");
				GenerateRoomRandom(left, top, right, bottom);
			}
		}

		public void GenerateWall(int left, int top, int diffX, int diffY, int padding, MapSprite sprite, bool isReplace = true)
		{
			int x = (1 - Math.Abs(diffX)) * padding;
			int y = (1 - Math.Abs(diffY)) * padding;
			SetSprite(left + x, top + y, this.mapSprites.GetID(sprite), isReplace);
			SetSprite(left - x, top - y, this.mapSprites.GetID(sprite), isReplace);
		}

		public void GenerateWall(int left, int top, int diffX, int diffY, int padding)
			=> GenerateWall(left, top, diffX, diffY, padding, this.mapSprites[MapSprite.Type.Wall]);

		public void GenerateWall(int left, int top, int diffX, int diffY)
			=> GenerateWall(left, top, diffX, diffY, 1, this.mapSprites[MapSprite.Type.Wall]);

		public List<(int X, int Y, int SpriteID)> GenerateOutline(int left, int top, int width, int height, MapSprite.Type type, bool isReplace = true)
		{
			int sprite = this.mapSprites.GetID(type);
			var setList = new List<(int, int, int)>();
			for (int i = 0; i < width; i++)
			{
				if (SetSprite(left + i, top, sprite, isReplace))
				{
					setList.Add((left + i, top, sprite));
				}
				if (SetSprite(left + i, top + height - 1, sprite, isReplace))
				{
					setList.Add((left + i, top + height - 1, sprite));
				}
			}
			for (int i = 0; i < height; i++)
			{
				if (SetSprite(left, top + i, sprite, isReplace))
				{
					setList.Add((left, top + i, sprite));
				}
				if (SetSprite(left + width - 1, top + i, sprite, isReplace))
				{
					setList.Add((left + width - 1, top + i, sprite));
				}
			}
			return setList;
		}

		public List<(int X, int Y, int SpriteID)> GenerateOutline(int left, int top, int width, int height, int padding, MapSprite.Type type, bool isReplace = true)
			=> GenerateOutline(left - padding, top - padding, width + (2 * padding), height + (2 * padding), type, isReplace);

		public List<(int X, int Y, int SpriteID)> GenerateOutline(int left, int top, int width, int height, int padding, int borderWidth, MapSprite.Type type, bool isReplace = true)
		{
			var setList = new List<(int, int, int)>();
			for (int i = padding; i < padding + borderWidth; i++)
			{
				setList.AddRange(GenerateOutline(left, top, width, height, i, type, isReplace));
			}
			return setList;
		}

		public List<(int X, int Y, int SpriteID)> GenerateRoom(int left, int top, int width, int height)
		{
			var setList = new List<(int, int, int)>();
			const int outlineWidth = 1;
			setList.AddRange(GenerateOutline(left, top, width, height, 1, outlineWidth, MapSprite.Type.AroundWall, false));
			setList.AddRange(GenerateOutline(left, top, width, height, MapSprite.Type.Wall));
			for (int y = 1; y < height - 1; y++)
			{
				for (int x = 1; x < width - 1; x++)
				{
					this[left + x, top + y] = this.mapSprites.GetID(new MapSprite.Type[] { MapSprite.Type.Floor, MapSprite.Type.Room });
				}
			}
			return setList;
		}

		public List<(int X, int Y, int SpriteID)> GenerateRoom(int left, int top, int size)
			=> GenerateRoom(left, top, size, size);

		public List<(int X, int Y, int SpriteID)> GenerateRoomRandom()
		{
			int x = GameManager.random.Next(Width - MinimumRoomSize);
			int y = GameManager.random.Next(Height - MinimumRoomSize);
			int size = GameManager.random.Next(MinimumRoomSize, Math.Min(Width - x + 1, Height - y + 1));
			return GenerateRoom(x, y, size);
		}

		public List<(int X, int Y, int SpriteID)> GenerateRoomRandom(int left, int top, int right, int bottom)
		{
			(int minX, int maxX) = (left, right).MinMax();
			(int minY, int maxY) = (top, bottom).MinMax();
			int x = GameManager.random.Next(minX, maxX - MinimumRoomSize);
			int y = GameManager.random.Next(minY, maxY - MinimumRoomSize);
			int width = GameManager.random.Next(MinimumRoomSize, maxX - x);
			int height = GameManager.random.Next(MinimumRoomSize, maxY - y);
			return GenerateRoom(x, y, width, height);
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
					GenerateWall(toX, toY, diffX, diffY);
					GenerateWall(toX, toY, diffX, diffY, 2, this.mapSprites[MapSprite.Type.AroundWall], false);
					toX -= diffX;
					toY -= diffY;
					if (toX < 0 || toX >= Width || toY < 0 || toY >= Height || this.mapSprites[this[toX, toY]].Is(MapSprite.Type.AroundWall))
						break;
					if (Math.Exp(-Math.Pow(i, 2) / Math.Pow(10, 2)) < GameManager.random.NextDouble())
						break;
				}
			}
			for (int i = 2; i >= 0; i--)
				GenerateWall(toX, toY, diffX, diffY, i, this.mapSprites[MapSprite.Type.AroundWall], false);
			toX += diffX;
			toY += diffY;
			this[toX, toY] = this.mapSprites.GetID(MapSprite.Type.Wall);
		}
	}
}
