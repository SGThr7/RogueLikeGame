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

		public object Move(char key)
		{
			switch (key)
			{
				case '8': return this.move.MoveUp();
				case '7': return this.move.MoveUpLeft();
				case '4': return this.move.MoveLeft();
				case '1': return this.move.MoveDownLeft();
				case '2': return this.move.MoveDown();
				case '3': return this.move.MoveDownRight();
				case '6': return this.move.MoveRight();
				case '9': return this.move.MoveUpRight();
				case '5':
				default:
					return this.move.MoveWait();
			}
		}

	}
}
