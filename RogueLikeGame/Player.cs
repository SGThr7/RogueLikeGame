using System;

namespace RogueLikeGame
{
	class Player : IKeyInput, ICharacter
	{
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public char Symbol { get; } = '@';
		public Action MoveComponent { get; }

		public Player()
		{
			MoveComponent = new Action(this);
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
				case '8': return MoveComponent.MoveUp();
				case 'y':
				case '7': return MoveComponent.MoveUpLeft();
				case 'h':
				case '4': return MoveComponent.MoveLeft();
				case 'b':
				case '1': return MoveComponent.MoveDownLeft();
				case 'j':
				case '2': return MoveComponent.MoveDown();
				case 'n':
				case '3': return MoveComponent.MoveDownRight();
				case 'l':
				case '6': return MoveComponent.MoveRight();
				case 'u':
				case '9': return MoveComponent.MoveUpRight();
				case '.':
				case '5':
				default:
					return MoveComponent.MoveWait();
			}
		}
	}
}
