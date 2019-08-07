using System;

namespace RogueLikeGame
{
	internal class Player : BaseCharacter, IKeyInput, IPlayer
	{
		public int MaxHunger { get; }
		private int hunger;
		public int Hunger
		{
			get => this.hunger;
			private set
			{
				if (value < 0) { this.hunger = 0; }
				else if (value > MaxHunger) { this.hunger = MaxHunger; }
				else { this.hunger = value; }
			}
		}

		public Player() : base(0, 0, "Hero", '@', 20, 2)
		{
			Hunger = MaxHunger = 690;
		}

		public Player(int x, int y, string name, char symbol, int maxHP, int maxHunger, int attack) : base(x, y, name, symbol, maxHP, attack)
		{
			Hunger = MaxHunger = maxHunger;
		}

		public override string ToString()
		{
			return Symbol.ToString();
		}

		public bool Move(char keyChar)
		{
			bool isMove = false;
			switch (keyChar)
			{
				case 'k':
				case '8': isMove = this.MoveUp(); break;
				case 'y':
				case '7': isMove = this.MoveUpLeft(); break;
				case 'h':
				case '4': isMove = this.MoveLeft(); break;
				case 'b':
				case '1': isMove = this.MoveDownLeft(); break;
				case 'j':
				case '2': isMove = this.MoveDown(); break;
				case 'n':
				case '3': isMove = this.MoveDownRight(); break;
				case 'l':
				case '6': isMove = this.MoveRight(); break;
				case 'u':
				case '9': isMove = this.MoveUpRight(); break;
				case '.':
				case '5': isMove = this.MoveWait(); break;
				default:
					break;
			}
			if (isMove)
			{
				Hunger--;
			}
			return isMove;
		}

		public override void Dead()
		{

		}
	}
}
