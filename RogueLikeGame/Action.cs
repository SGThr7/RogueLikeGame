using System;
using System.Linq;

namespace RogueLikeGame
{
	internal static class Action
	{
		public static (int X, int Y) PreMove(this ICharacter character, int distance, double angle)
		{
			int xDiff = -1 * (int)Math.Round(distance * Math.Sin(angle), MidpointRounding.AwayFromZero);
			int yDiff = -1 * (int)Math.Round(distance * Math.Cos(angle), MidpointRounding.AwayFromZero);

			(int x, int y) = (character.X, character.Y);
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
			}

			return (x, y);

		}

		public static void Move(this ICharacter character, int distance, double angle)
		{
			(int x, int y) = character.PreMove(distance, angle);

			bool isAttack = false;
			bool noCharacter(ICharacter c, int _x, int _y)
				=> c.X != _x || c.Y != _y;
			foreach (var c in GameManager.Characters)
			{
				if (c != character)
				{
					isAttack |= !noCharacter(c, x, y);
				}
				if (isAttack)
				{
					break;
				}
			}

			if (isAttack)
			{
				Attack(character, x, y);
			}
			else
			{
				character.X = x;
				character.Y = y;
			}
		}
		public static void MoveWait(this ICharacter character) { Move(character, 0, 0); }
		public static void MoveUp(this ICharacter character, int distance) { Move(character, distance, 0); }
		public static void MoveUp(this ICharacter character) { MoveUp(character, 1); }
		public static void MoveLeft(this ICharacter character, int distance) { Move(character, distance, 90 / 180d * Math.PI); }
		public static void MoveLeft(this ICharacter character) { MoveLeft(character, 1); }
		public static void MoveDown(this ICharacter character, int distance) { Move(character, distance, 180 / 180d * Math.PI); }
		public static void MoveDown(this ICharacter character) { MoveDown(character, 1); }
		public static void MoveRight(this ICharacter character, int distance) { Move(character, distance, 270 / 180d * Math.PI); }
		public static void MoveRight(this ICharacter character) { MoveRight(character, 1); }
		public static void MoveUpLeft(this ICharacter character, int distance) { Move(character, distance, 45 / 180d * Math.PI); }
		public static void MoveUpLeft(this ICharacter character) { MoveUpLeft(character, 1); }
		public static void MoveDownLeft(this ICharacter character, int distance) { Move(character, distance, 135 / 180d * Math.PI); }
		public static void MoveDownLeft(this ICharacter character) { MoveDownLeft(character, 1); }
		public static void MoveDownRight(this ICharacter character, int distance) { Move(character, distance, 225 / 180d * Math.PI); }
		public static void MoveDownRight(this ICharacter character) { MoveDownRight(character, 1); }
		public static void MoveUpRight(this ICharacter character, int distance) { Move(character, distance, 315 / 180d * Math.PI); }
		public static void MoveUpRight(this ICharacter character) { MoveUpRight(character, 1); }

		public static (int X, int Y) Teleport(this ICharacter character, (int x, int y) position)
		{
			character.X = position.x;
			character.Y = position.y;
			return position;
		}

		public static (int X, int Y) Teleport(this ICharacter character, int x, int y)
		{
			return Teleport(character, (x, y));
		}

		public static (int X, int Y) RandomTeleport(this ICharacter character)
		{
			(int X, int Y) position = MapManager.CurrentMap.GetRandomSpritePoint(MapSprite.Type.Room);
			return Teleport(character, position);
		}

		public static void Attack(this ICharacter character, int x, int y)
		{
			var c = GameManager.Characters
				.Where(a => a.X == x && a.Y == y)
				.First();
			c.TakeDamage(character.Attack);
		}
	}
}
