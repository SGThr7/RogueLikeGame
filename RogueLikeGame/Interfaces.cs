namespace RogueLikeGame
{
	internal interface IKeyInput
	{
		void Move(char keyInput);
	}

	internal interface IPosition
	{
		int X { get; set; }
		int Y { get; set; }
	}

	internal interface ICharacter : IPosition, ISprite, IStatus
	{
	}

	internal interface ISprite
	{
		char Symbol { get; }
	}

	internal interface IStatus
	{
		int HP { get; }
		int Attack { get; }

		int TakeDamage(int damage);
	}
}
