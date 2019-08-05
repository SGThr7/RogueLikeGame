using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
	class MapSprite : ISprite
	{
		public enum Type
		{
			Unknown, Wall, Floor, AroundWall, Room, Debug
		}

		public List<Type> Attributes { get; } = new List<Type>();
		public char Symbol { get; }

		public bool CanWalk => Is(Type.Floor);
		public bool CanReplace => IsOr(Type.Unknown, Type.AroundWall, Type.Debug);

		public MapSprite(Type type, char symbol)
		{
			Symbol = symbol;
			Attributes.Add(type);
		}

		public MapSprite(IEnumerable<Type> types, char symbol)
		{
			Symbol = symbol;
			foreach (Type type in types)
			{
				Attributes.Add(type);
			}
		}

		public override string ToString()
			=> Symbol.ToString();

		public bool Is(Type type)
			=> Attributes.Contains(type);

		public bool Is(params Type[] types)
		{
			bool ret = true;
			foreach (Type type in types)
			{
				ret &= Is(type);
			}

			return ret;
		}

		public bool IsOr(params Type[] types)
		{
			foreach (var type in types)
			{
				if (Is(type))
				{
					return true;
				}
			}
			return false;
		}
	}

	class MapSprites
	{
		private readonly List<MapSprite> mapSprites = new List<MapSprite>();
		public int UnknownIndex => 0;
		public MapSprite UnknownSprite => this.mapSprites[UnknownIndex];

		public MapSprites(List<MapSprite> mapSprites)
		{
			this.mapSprites.Add(new MapSprite(MapSprite.Type.Unknown, ' '));
			Add(mapSprites);
			AddIfNotExist(MapSprite.Type.Wall, '#');
			AddIfNotExist(MapSprite.Type.Floor, '.');
			AddIfNotExist(MapSprite.Type.AroundWall, '+');
		}

		public MapSprites() : this(new List<MapSprite>())
		{
		}

		public MapSprite this[int index]
		{
			get { return index < 0 || index >= this.mapSprites.Count ? UnknownSprite : this.mapSprites[index]; }
		}
		public MapSprite this[MapSprite.Type type]
		{
			get
			{
				try { return this.mapSprites.Find(sprite => sprite.Is(type)); }
				catch { return UnknownSprite; }
			}
		}

		public bool IsExist(MapSprite.Type type)
		{
			MapSprite sprite = this[type];
			if (sprite == null) return false;
			return sprite.Is(type);
		}

		public void Add(MapSprite mapSprite)
		{
			if (mapSprite == null)
			{
				throw new ArgumentNullException(nameof(mapSprite));
			}

			this.mapSprites.Add(mapSprite);
		}

		public void Add(MapSprite.Type type, char sprite)
			=> Add(new MapSprite(type, sprite));

		public void Add(List<MapSprite> mapSprites)
		{
			foreach (MapSprite sprite in mapSprites)
			{
				Add(sprite);
			}
		}

		public void AddIfNotExist(MapSprite.Type type, char sprite)
		{
			if (!IsExist(type))
			{
				Add(type, sprite);
			}
		}

		public List<int> GetIDs(MapSprite.Type type)
		{
			try { return this.mapSprites.Indexed().Where(t => t.item.Is(type)).Select(t => t.index).ToList(); }
			catch { return new List<int>(); }
		}

		public int GetID(Predicate<MapSprite> match)
		{
			try { return this.mapSprites.FindIndex(match); }
			catch { return UnknownIndex; }
		}

		public int GetID(MapSprite.Type type)
			=> GetID(s => s.Is(type));

		public int GetID(MapSprite mapSprite)
			=> GetID(s => s == mapSprite);

		public int GetID(params MapSprite.Type[] types)
			=> GetID(s => s.Is(types));
	}
}
