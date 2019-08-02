using System;

namespace RogueLikeGame
{
	class Player : IKeyInput, ICharacter
	{
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public char Symbol { get; } = '@';
		private readonly MoveComponent move;

		public Player()
		{
			this.move = new MoveComponent(this);
		}

		public override string ToString()
		{
			return Symbol.ToString();
		}

		public object Move(char key)
		{
			switch (key)
			{
				case 'k':
				case '8': return this.move.MoveUp();
				case 'y':
				case '7': return this.move.MoveUpLeft();
				case 'h':
				case '4': return this.move.MoveLeft();
				case 'b':
				case '1': return this.move.MoveDownLeft();
				case 'j':
				case '2': return this.move.MoveDown();
				case 'n':
				case '3': return this.move.MoveDownRight();
				case 'l':
				case '6': return this.move.MoveRight();
				case 'u':
				case '9': return this.move.MoveUpRight();
				case '.':
				case '5':
				default:
					return this.move.MoveWait();
			}
		}

		public (int X, int Y) Teleport((int x, int y) position)
		{
			X = position.x;
			Y = position.y;
			return position;
		}

		public (int X, int Y) Teleport(int x, int y)
		{
			return Teleport(x, y);
		}

		public (int X, int Y) RandomTeleport()
		{
			(int X, int Y) position= MapManager.CurrentMap.GetRandomPoint(MapSprite.Type.Room);
			return Teleport(position);
		}
	}
}
