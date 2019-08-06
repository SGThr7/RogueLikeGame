namespace RogueLikeGame
{
	interface IKeyInput
	{
		object Move(char keyInput);
	}

	interface IPosition
	{
		int X { get; set; }
		int Y { get; set; }
	}

	interface ICharacter : IPosition, ISprite, IStatus
	{
		Action Action { get; }
	}

	interface ISprite
	{
		char Symbol { get; }
	}

	interface IStatus
	{
		int HP { get; }
		int Attack { get; }

		int TakeDamage(int damage);
	}
}
