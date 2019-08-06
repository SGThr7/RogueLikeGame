using System;
using System.Linq;

namespace RogueLikeGame
{
	class Action
	{
		private readonly ICharacter character;

		public Action(ICharacter character)
		{
			this.character = character;
		}

		public (int X, int Y) Move(int distance, double angle, bool isSimurate = false)
		{
			int xDiff = -1 * (int)Math.Round(distance * Math.Sin(angle), MidpointRounding.AwayFromZero);
			int yDiff = -1 * (int)Math.Round(distance * Math.Cos(angle), MidpointRounding.AwayFromZero);

			(int x, int y) = (this.character.X, this.character.Y);
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

			if (xDiff != 0 || yDiff != 0)
			{
				x += xDiff;
				y += yDiff;

				bool isAttack = true;
				bool noCharacter(ICharacter character)
					=> character.X != x || character.Y != y;
				foreach (var character in GameManager.Characters)
				{
					if (character != this.character)
					{
						isAttack &= noCharacter(character);
					}
				}

				if (!isSimurate)
				{
					if (isAttack)
					{
						this.character.X = x;
						this.character.Y = y;
					}
					else
					{
						Attack(x, y);
					}
				}
			}

			return (x, y);
		}
		public (int X, int Y) MoveWait(bool isSimulate = false) { return Move(0, 0, isSimulate); }
		public (int X, int Y) MoveUp(int distance, bool isSimulate = false) { return Move(distance, 0, isSimulate); }
		public (int X, int Y) MoveUp(bool isSimulate = false) { return MoveUp(1, isSimulate); }
		public (int X, int Y) MoveLeft(int distance, bool isSimulate = false) { return Move(distance, 90 / 180d * Math.PI, isSimulate); }
		public (int X, int Y) MoveLeft(bool isSimulate = false) { return MoveLeft(1, isSimulate); }
		public (int X, int Y) MoveDown(int distance, bool isSimulate = false) { return Move(distance, 180 / 180d * Math.PI, isSimulate); }
		public (int X, int Y) MoveDown(bool isSimulate = false) { return MoveDown(1, isSimulate); }
		public (int X, int Y) MoveRight(int distance, bool isSimulate = false) { return Move(distance, 270 / 180d * Math.PI, isSimulate); }
		public (int X, int Y) MoveRight(bool isSimulate = false) { return MoveRight(1, isSimulate); }
		public (int X, int Y) MoveUpLeft(int distance, bool isSimulate = false) { return Move(distance, 45 / 180d * Math.PI, isSimulate); }
		public (int X, int Y) MoveUpLeft(bool isSimulate = false) { return MoveUpLeft(1, isSimulate); }
		public (int X, int Y) MoveDownLeft(int distance, bool isSimulate = false) { return Move(distance, 135 / 180d * Math.PI, isSimulate); }
		public (int X, int Y) MoveDownLeft(bool isSimulate = false) { return MoveDownLeft(1, isSimulate); }
		public (int X, int Y) MoveDownRight(int distance, bool isSimulate = false) { return Move(distance, 225 / 180d * Math.PI, isSimulate); }
		public (int X, int Y) MoveDownRight(bool isSimulate = false) { return MoveDownRight(1, isSimulate); }
		public (int X, int Y) MoveUpRight(int distance, bool isSimulate = false) { return Move(distance, 315 / 180d * Math.PI, isSimulate); }
		public (int X, int Y) MoveUpRight(bool isSimulate = false) { return MoveUpRight(1, isSimulate); }

		public (int X, int Y) Teleport((int x, int y) position)
		{
			this.character.X = position.x;
			this.character.Y = position.y;
			return position;
		}

		public (int X, int Y) Teleport(int x, int y)
		{
			return Teleport(x, y);
		}

		public (int X, int Y) RandomTeleport()
		{
			(int X, int Y) position = MapManager.CurrentMap.GetRandomSpritePoint(MapSprite.Type.Room);
			return Teleport(position);
		}

		public void Attack(int x, int y)
		{
			var c = GameManager.Characters
				.Where(a => a.X == x && a.Y == y)
				.First();
			c.TakeDamage(this.character.Attack);
			System.Diagnostics.Debug.WriteLine("Attacked");
		}
	}
}
