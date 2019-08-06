using System;

namespace RogueLikeGame
{
	class Player : IKeyInput, ICharacter
	{
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public char Symbol { get; } = '@';
		public Action Action { get; }
		public int HP { get; private set; }
		public int Attack { get; } = 2;

		public Player()
		{
			Action = new Action(this);
		}

		public override string ToString()
		{
			return Symbol.ToString();
		}

		public object Move(char keyChar)
		{
			switch (keyChar)
			{
				case 'k':
				case '8': return Action.MoveUp();
				case 'y':
				case '7': return Action.MoveUpLeft();
				case 'h':
				case '4': return Action.MoveLeft();
				case 'b':
				case '1': return Action.MoveDownLeft();
				case 'j':
				case '2': return Action.MoveDown();
				case 'n':
				case '3': return Action.MoveDownRight();
				case 'l':
				case '6': return Action.MoveRight();
				case 'u':
				case '9': return Action.MoveUpRight();
				case '.':
				case '5':
				default:
					return Action.MoveWait();
			}
		}

		public int TakeDamage(int damage)
			=> HP -= damage;
	}
}
