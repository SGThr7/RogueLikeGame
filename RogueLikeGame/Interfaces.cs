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
		Action MoveComponent { get; }
	}

	interface ISprite
	{
		char Symbol { get; }
	}

	interface IStatus
	{
		int HP { get; }

		int Damage(int damage);
	}
}
