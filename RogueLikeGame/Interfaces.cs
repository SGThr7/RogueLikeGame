namespace RogueLikeGame
{
	internal interface IKeyInput
	{
		bool Move(char keyInput);
	}

	internal interface IPosition
	{
		int X { get; set; }
		int Y { get; set; }
	}

	internal interface ISprite
	{
		char Symbol { get; }
	}

	internal interface IStatus
	{
		int MaxHP { get; }
		int HP { get; }
		int Power { get; }

		void TakeDamage(int damage);
		void Attack(ICharacter character);
		void Dead();
	}

	internal interface ICharacter : IPosition, ISprite, IStatus
	{
		string Name { get; }
	}

	internal interface IEnemy: ICharacter
	{ 
	}

	internal interface IPlayer: ICharacter
	{
		int Hunger { get; }
	}
}
