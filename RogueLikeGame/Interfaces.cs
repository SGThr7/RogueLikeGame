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

	interface ICharacter : IPosition
	{
		char Simbol { get; }
	}
}
