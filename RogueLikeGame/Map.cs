using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueLikeGame
{
	static class MapManager
	{
		private static readonly List<Map> maps = new List<Map>();
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
		public enum Axis
		{
			X, Y
		}

		private readonly int[] map;
		public readonly MapSprites mapSprites;
		public int Width { get; }
		public int Height { get; }
		public (int Width, int Height) Size => (Width, Height);
		private const int MinimumRoomSize = 6;
		public readonly MapVisible mapVisible;

		public Map(int width, int height, List<MapSprite> sprites)
		{
			this.map = new int[width * height];
			Width = width;
			Height = height;
			this.mapVisible = new MapVisible(width, height);
			this.mapSprites = new MapSprites(sprites);
			GenerateMap();
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
				{
					return this.map[(y * Width) + x];
				}
				return -1;
			}
			set
			{
				if ((0 <= x && x < Width && 0 <= y && y < Height))
				{
					this.map[(y * Width) + x] = value;
				}
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

		public MapSprite GetVisibleMapSprite(int left, int top)
			=> this.mapVisible[left, top] ? GetMapSprite(left, top) : this.mapSprites.UnknownSprite;

		public IEnumerable<(int X, int Y)> GetSpritePositions(Predicate<MapSprite> predicate)
		{
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (predicate(this.mapSprites[this[x, y]]))
					{
						yield return (x, y);
					}
				}
			}
		}

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
			foreach ((int X, int Y) point in GetRandomSpritePoints(MapSprite.Type.Wall))
			{
				(MapSprite mapSprite, (int diffX, int diffY) position)[] aroundSprites = GetAround(point.X, point.Y, C);
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
		public bool SetSprite(int left, int top, int value, bool forceReplace = true)
		{
			if (forceReplace || this.mapSprites[this[left, top]].CanReplace)
			{
				this[left, top] = value;
				return true;
			}
			return false;
		}

		public void FillSprite(IEnumerable<int> xs, IEnumerable<int> ys, int value, bool forceReplace = false)
		{
			foreach (int y in ys)
			{
				foreach (int x in xs)
				{
					SetSprite(x, y, value, forceReplace);
				}
			}
		}
		public void FillSprite(IEnumerable<int> xs, int y, int value, bool forceReplace)
		{
			foreach (int x in xs)
			{
				SetSprite(x, y, value, forceReplace);
			}
		}
		public void FillSprite(int x, IEnumerable<int> ys, int value, bool forceReplace)
		{
			foreach (int y in ys)
			{
				SetSprite(x, y, value, forceReplace);
			}
		}

		private void GenerateMap()
		{
			const int sectionCount = 4;
			(int X, int Y) pivot = GetRandomPoint(MinimumRoomSize + 1);
			//for (int i = 0; i < Width; i++)
			//{
			//	this[i, pivot.Y] = this.mapSprites.GetID(MapSprite.Type.Debug);
			//}
			//for (int i = 0; i < Height; i++)
			//{
			//	this[pivot.X, i] = this.mapSprites.GetID(MapSprite.Type.Debug);
			//}
			var rooms = new (List<(int X, int Y, int SpriteID)> walls, List<(int X, int Y)> roads)[4];
			bool isTop(int i) => (int)(i / Math.Sqrt(sectionCount)) == 0;
			bool isLeft(int i) => (int)(i % Math.Sqrt(sectionCount)) == 0;
			sbyte biasX(int i) => (sbyte)(isLeft(i) ? 1 : -1);
			sbyte biasY(int i) => (sbyte)(isTop(i) ? 1 : -1);
			for (int i = 0; i < sectionCount; i++)
			{
				int left = isLeft(i) ? 0 : pivot.X + 1;
				int right = isLeft(i) ? pivot.X - 1 : Width;
				int top = isTop(i) ? 0 : pivot.Y + 1;
				int bottom = isTop(i) ? pivot.Y - 1 : Height;

				List<(int X, int Y, int SpriteID)> sprites = GenerateRoomRandom(left, top, right, bottom);
				IEnumerable<(int X, int Y, int SpriteID)> walls = sprites
					.Where(a => this.mapSprites[a.SpriteID].Is(MapSprite.Type.Wall));
				int minX = walls.Min(a => a.X * biasX(i)) * biasX(i);
				int minY = walls.Min(a => a.Y * biasY(i)) * biasY(i);
				walls = walls
					.Where(a => a.X != minX)
					.Where(a => a.Y != minY)
					.OrderByDescending(a => (a.X * biasX(i)) + (a.Y * biasY(i)))
					.Skip(1);
				rooms[i].walls = walls.ToList();
			}
			/// Generate roads
			while (true)
			{
				var hasRoad = new (bool hori, bool vert)[sectionCount];
				for (int i = 0; i < sectionCount; i++)
				{
					rooms[i].roads = new List<(int X, int Y)>();
					int maxX = rooms[i].walls.Max(a => a.X * biasX(i)) * biasX(i);
					int maxY = rooms[i].walls.Max(a => a.Y * biasY(i)) * biasY(i);
					var vertWall = rooms[i].walls.Where(a => a.X == maxX);
					var horiWall = rooms[i].walls.Where(a => a.Y == maxY);
					if (GameManager.random.NextDouble() < .5)
					{
						(int X, int Y, _) = vertWall.Random();
						rooms[i].roads.Add((X, Y));
						hasRoad[i].hori = true;
					}
					if (!hasRoad[i].hori || GameManager.random.NextDouble() < .5)
					{
						(int X, int Y, _) = horiWall.Random();
						rooms[i].roads.Add((X, Y));
						hasRoad[i].vert = true;
					}
				}
				int topRoadCount = hasRoad.Indexed().Where(a => isTop(a.index))
					.Sum(a => Convert.ToInt32(a.item.vert));
				int bottomRoadCount = hasRoad.Indexed().Where(a => !isTop(a.index))
					.Sum(a => Convert.ToInt32(a.item.vert));
				int leftRoadCount = hasRoad.Indexed().Where(a => isLeft(a.index))
					.Sum(a => Convert.ToInt32(a.item.hori));
				int rightRoadCount = hasRoad.Indexed().Where(a => !isLeft(a.index))
					.Sum(a => Convert.ToInt32(a.item.hori));
				if (topRoadCount > 0 && bottomRoadCount > 0 &&
					(topRoadCount + bottomRoadCount) % 2 == 0 &&
					leftRoadCount > 0 && rightRoadCount > 0 &&
					(leftRoadCount + rightRoadCount) % 2 == 0)
				{
					for (int i = 0; i < sectionCount; i++)
					{
						foreach (var (X, Y) in rooms[i].roads)
						{
							Axis axis = rooms[i].walls.Max(a => a.X * biasX(i)) * biasX(i) == X ? Axis.X : Axis.Y;
							int length = axis == Axis.X ?
								pivot.X - X + biasX(i) :
								pivot.Y - Y + biasY(i);
							GrowFloor(X, Y, axis, length);
						}
					}
					break;
				}
			}

			/// Connect roads
			void ConnectRoads(Axis axis)
			{
				int wall = this.mapSprites.GetID(MapSprite.Type.Wall);
				int aroundWall = this.mapSprites.GetID(MapSprite.Type.AroundWall);
				bool roadMode = false;
				bool oldRoadMode = false;
				int length = axis == Axis.X ? Width : Height;
				for (int i = 0; i < length; i++)
				{
					int x = axis == Axis.X ? i : pivot.X;
					int y = axis == Axis.Y ? i : pivot.Y;
					int diffX = axis == Axis.Y ? 1 : 0;
					int diffY = axis == Axis.X ? 1 : 0;

					if (this.mapSprites[this[x + diffX, y + diffY]].CanWalk)
					{
						roadMode = !roadMode;
					}
					if (this.mapSprites[this[x - diffX, y - diffY]].CanWalk)
					{
						roadMode = !roadMode;
					}
					if (roadMode)
					{
						if (oldRoadMode != roadMode)
						{
							FillSprite(Enumerable.Range(x - 2, 5), Enumerable.Range(y - 2, 5), aroundWall, false);
							FillSprite(Enumerable.Range(x - 1, 3), Enumerable.Range(y - 1, 3), wall, false);
						}
						GenerateRoad(x, y, axis);
					}
					else if (oldRoadMode != roadMode)
					{
						FillSprite(Enumerable.Range(x - 2, 5), Enumerable.Range(y - 2, 5), aroundWall, false);
						FillSprite(Enumerable.Range(x - 1, 3), Enumerable.Range(y - 1, 3), wall, false);
					}
					oldRoadMode = roadMode;
				}
			}
			ConnectRoads(Axis.X);
			ConnectRoads(Axis.Y);
		}

		public void GenerateWall(int left, int top, int diffX, int diffY, int padding, MapSprite sprite, bool forceReplace = true)
		{
			int x = (1 - Math.Abs(diffX)) * padding;
			int y = (1 - Math.Abs(diffY)) * padding;
			int id = this.mapSprites.GetID(sprite);
			SetSprite(left + x, top + y, id, forceReplace);
			SetSprite(left - x, top - y, id, forceReplace);
			//this[left + x, top + y, forceReplace] = id;
			//this[left - x, top - y, forceReplace] = id;
		}

		public void GenerateWall(int left, int top, int diffX, int diffY, int padding)
			=> GenerateWall(left, top, diffX, diffY, padding, this.mapSprites[MapSprite.Type.Wall]);

		public void GenerateWall(int left, int top, int diffX, int diffY)
			=> GenerateWall(left, top, diffX, diffY, 1, this.mapSprites[MapSprite.Type.Wall]);

		public void GenerateWall(int left, int top, Axis axis, int padding, int spriteID, bool forceReplace = true)
		{
			int x = axis == Axis.Y ? padding : 0;
			int y = axis == Axis.X ? padding : 0;
			SetSprite(left + x, top + y, spriteID, forceReplace);
			SetSprite(left - x, top - y, spriteID, forceReplace);
			//this[left + x, top + y, forceReplace] = id;
			//this[left - x, top - y, forceReplace] = id;
		}

		public void GenerateWall(int left, int top, Axis axis, int padding, MapSprite sprite, bool forceReplace = true)
			=> GenerateWall(left, top, axis, padding, this.mapSprites.GetID(sprite), forceReplace);
		public void GenerateWall(int left, int top, Axis axis, MapSprite sprite, bool forceReplace = true)
			=> GenerateWall(left, top, axis, 1, sprite, forceReplace);
		public void GenerateWall(int left, int top, Axis axis, bool forceReplace = true)
			=> GenerateWall(left, top, axis, 1, this.mapSprites[MapSprite.Type.Wall], forceReplace);
		public void GenerateWall(int left, int top, Axis axis, int padding, bool forceReplace = true)
			=> GenerateWall(left, top, axis, padding, this.mapSprites[MapSprite.Type.Wall], forceReplace);

		public List<(int X, int Y, int SpriteID)> GenerateOutline(int left, int top, int width, int height, MapSprite.Type type, bool forceReplace = true)
		{
			int sprite = this.mapSprites.GetID(type);
			var setList = new List<(int, int, int)>();
			for (int i = 0; i < width; i++)
			{
				if (SetSprite(left + i, top, sprite, forceReplace))
				{
					setList.Add((left + i, top, sprite));
				}
				if (SetSprite(left + i, top + height - 1, sprite, forceReplace))
				{
					setList.Add((left + i, top + height - 1, sprite));
				}
			}
			for (int i = 1; i < height - 1; i++)
			{
				if (SetSprite(left, top + i, sprite, forceReplace))
				{
					setList.Add((left, top + i, sprite));
				}
				if (SetSprite(left + width - 1, top + i, sprite, forceReplace))
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
					int id = this.mapSprites.GetID(new MapSprite.Type[] { MapSprite.Type.Floor, MapSprite.Type.Room });
					this[left + x, top + y] = id;
					setList.Add((left + x, top + y, id));
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

		public void GenerateRoad(int left, int top, Axis axis)
		{
			this[left, top] = this.mapSprites.GetID(MapSprite.Type.Floor);
			GenerateWall(left, top, axis, false);
			GenerateWall(left, top, axis, 2, this.mapSprites[MapSprite.Type.AroundWall], false);
		}

		public void GrowFloor(int left, int top, Axis axis, int length)
		{
			sbyte direction = (sbyte)(length < 0 ? -1 : 1);
			for (int i = 0; i < length * direction; i++)
			{
				int x = left + (axis == Axis.X ? i * direction : 0);
				int y = top + (axis == Axis.Y ? i * direction : 0);
				GenerateRoad(x, y, axis);
			}
		}
	}
}
