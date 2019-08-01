using System;

namespace RogueLikeGame
{
	class MoveComponent
	{
		private readonly IPosition position;

		public MoveComponent(IPosition position)
		{
			this.position = position;
		}

		public (int X, int Y) Move(int distance, double angle)
		{
			int xDiff = -1 * (int)Math.Round(distance * Math.Sin(angle), MidpointRounding.AwayFromZero);
			int yDiff = -1 * (int)Math.Round(distance * Math.Cos(angle), MidpointRounding.AwayFromZero);

			(int x, int y) = (this.position.X, this.position.Y);
			Map map = MapManager.CurrentMap;
			if (!map.GetMapSprite(x + xDiff, y).CanWalk)
			{
				xDiff = 0;
			}

			if (!map.GetMapSprite(x, y + yDiff).CanWalk)
			{
				yDiff = 0;
			}

			if (!map.GetMapSprite(x + xDiff, y + yDiff).CanWalk)
			{
				xDiff = yDiff = 0;
			}

			if (xDiff == 0 && yDiff == 0)
			{
			}
			else
			{
				this.position.X += xDiff;
				this.position.Y += yDiff;
			}
			return (this.position.X, this.position.Y);
		}
		public (int X, int Y) MoveWait() { return Move(0, 0); }
		public (int X, int Y) MoveUp(int distance) { return Move(distance, 0); }
		public (int X, int Y) MoveUp() { return MoveUp(1); }
		public (int X, int Y) MoveLeft(int distance) { return Move(distance, 90 / 180d * Math.PI); }
		public (int X, int Y) MoveLeft() { return MoveLeft(1); }
		public (int X, int Y) MoveDown(int distance) { return Move(distance, 180 / 180d * Math.PI); }
		public (int X, int Y) MoveDown() { return MoveDown(1); }
		public (int X, int Y) MoveRight(int distance) { return Move(distance, 270 / 180d * Math.PI); }
		public (int X, int Y) MoveRight() { return MoveRight(1); }
		public (int X, int Y) MoveUpLeft(int distance) { return Move(distance, 45 / 180d * Math.PI); }
		public (int X, int Y) MoveUpLeft() { return MoveUpLeft(1); }
		public (int X, int Y) MoveDownLeft(int distance) { return Move(distance, 135 / 180d * Math.PI); }
		public (int X, int Y) MoveDownLeft() { return MoveDownLeft(1); }
		public (int X, int Y) MoveDownRight(int distance) { return Move(distance, 225 / 180d * Math.PI); }
		public (int X, int Y) MoveDownRight() { return MoveDownRight(1); }
		public (int X, int Y) MoveUpRight(int distance) { return Move(distance, 315 / 180d * Math.PI); }
		public (int X, int Y) MoveUpRight() { return MoveUpRight(1); }
	}
}
