using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
	internal class Enemy : BaseCharacter, IEnemy
	{
		enum Names
		{
			Jackal = 'j',
			Goblin = 'g'
		}

		public Enemy() : base(0, 0, Names.Jackal.ToString(), (char)Names.Jackal, 10, 1)
		{
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
					around.Add((this.PreMove(1, _angle), _angle));
				}
				(_, double angle) = around.OrderBy(a => dist(a.point.X, a.point.Y)).First();
				this.Move(1, angle);
			}
		}

		public override void Dead()
		{
			SystemMessage.Send($"{Name} を倒した");
			GameManager.Enemies.Remove(this);
		}
	}
}
