using System;

namespace RogueLikeGame
{
	internal class Player : BaseCharacter, IKeyInput, ICharacter
	{
		public Player() : base(0, 0, '@', 20, 2)
		{
		}

		public Player(int x, int y, char symbol, int maxHP, int attack) : base(x, y, symbol, maxHP, attack)
		{
		}

		public override string ToString()
		{
			return Symbol.ToString();
		}

		public void Move(char keyChar)
		{
			switch (keyChar)
			{
				case 'k':
				case '8': this.MoveUp(); break;
				case 'y':
				case '7': this.MoveUpLeft(); break;
				case 'h':
				case '4': this.MoveLeft(); break;
				case 'b':
				case '1': this.MoveDownLeft(); break;
				case 'j':
				case '2': this.MoveDown(); break;
				case 'n':
				case '3': this.MoveDownRight(); break;
				case 'l':
				case '6': this.MoveRight(); break;
				case 'u':
				case '9': this.MoveUpRight(); break;
				case '.':
				case '5':
				default:
					this.MoveWait(); break;
			}
		}
	}
}
