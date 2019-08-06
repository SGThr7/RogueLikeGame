using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
	class Enemy : ICharacter
	{
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public char Symbol { get; } = 'g';
		public Action MoveComponent { get; }

		public Enemy()
		{
			MoveComponent = new Action(this);
		}

		public void Move()
		{
			Player player = GameManager.Player;
			double dist(int x, int y)
				=> Math.Pow(player.X - x, 2) + Math.Pow(player.Y - y, 2);
			if (Math.Sqrt(dist(X, Y)) < 5)
			{
				const int direction = 8;
				var around = new List<((int X, int Y) point, double angle)>();
				for (int i = 0; i < direction; i++)
				{
					double _angle = i / (direction / 2d) * Math.PI;
					around.Add((MoveComponent.Move(1, _angle, true), _angle));
				}
				(_, double angle) = around.OrderBy(a => dist(a.point.X, a.point.Y)).First();
				MoveComponent.Move(1, angle);
			}
		}
	}
}
