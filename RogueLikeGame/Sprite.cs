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
			Unknown, Wall, Floor
		}

		public List<Type> Attributes { get; } = new List<Type>();
		public char Symbol { get; }

		public bool CanWalk => Is(Type.Floor);

		public MapSprite(Type type, char symbol)
		{
			Symbol = symbol;
			Attributes.Add(type);
		}

		public override string ToString()
		{
			return Symbol.ToString();
		}

		public bool Is(Type type)
		{
			return Attributes.Contains(type);
		}
	}

	class MapSprites
	{
		private readonly List<MapSprite> mapSprites = new List<MapSprite>();
		private int UnknownIndex => 0;
		private MapSprite UnknownSprite => this.mapSprites[UnknownIndex];

		public MapSprites(List<MapSprite> mapSprites)
		{
			this.mapSprites.Add(new MapSprite(MapSprite.Type.Unknown, ' '));
			Add(mapSprites);
			if (GetID(MapSprite.Type.Wall) == UnknownIndex)
			{
				Add(new MapSprite(MapSprite.Type.Wall, '#'));
			}

			if (GetID(MapSprite.Type.Floor) == UnknownIndex)
			{
				Add(new MapSprite(MapSprite.Type.Floor, '.'));
			}
		}

		public MapSprites() : this(new List<MapSprite>())
		{
		}

		public MapSprite this[int index]
		{
			get
			{
				return index < 0 || index >= this.mapSprites.Count ? UnknownSprite : this.mapSprites[index];
			}
		}

		public void Add(MapSprite mapSprite)
		{
			this.mapSprites.Add(mapSprite);
		}

		public void Add(MapSprite.Type type, char sprite)
		{
			Add(new MapSprite(type, sprite));
		}

		public void Add(List<MapSprite> mapSprites)
		{
			foreach (MapSprite sprite in mapSprites)
			{
				Add(sprite);
			}
		}

		public MapSprite GetSprite(MapSprite.Type type)
		{
			try { return this.mapSprites.Find(sprite => sprite.Is(type)); }
			catch (ArgumentNullException) { return UnknownSprite; }
		}

		public int GetID(MapSprite.Type type)
		{
			try { return this.mapSprites.FindIndex(sprite => sprite.Is(type)); }
			catch { return UnknownIndex; }
		}

		public void AddIfNotExist(MapSprite.Type type, char sprite)
		{
			if (GetID(type) == UnknownIndex)
			{
				Add(type, sprite);
			}
		}
	}
}
