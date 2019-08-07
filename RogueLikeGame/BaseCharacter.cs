using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
	internal abstract class BaseCharacter : ICharacter
	{
		public int X { get; set; }
		public int Y { get; set; }
		public char Symbol { get; }
		public int MaxHP { get; }
		private int hp;
		public int HP
		{
			get => this.hp;
			set => this.hp = value < MaxHP ? value : MaxHP;
		}
		public int Attack { get; }

		public BaseCharacter(int x, int y, char symbol, int maxHP, int attack)
		{
			X = x;
			Y = y;
			Symbol = symbol;
			this.hp = MaxHP = maxHP;
			Attack = attack;
		}

		public int TakeDamage(int damage)
		{
			return HP -= damage;
		}
	}
}
