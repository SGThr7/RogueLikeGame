using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
	internal abstract class BaseCharacter : ICharacter
	{
		public int X { get; set; }
		public int Y { get; set; }
		public string Name { get; }
		public char Symbol { get; }
		public int MaxHP { get; }
		private int hp;
		public int HP
		{
			get => this.hp;
			set
			{
				this.hp = value < MaxHP ? value : MaxHP;
				if (value <= 0)
				{
					Dead();
				}
			}
		}
		public int Power { get; }

		public BaseCharacter(int x, int y, string name, char symbol, int maxHP, int power)
		{
			X = x;
			Y = y;
			Name = name;
			Symbol = symbol;
			this.hp = MaxHP = maxHP;
			Power = power;
		}

		public void TakeDamage(int damage)
		{
			SystemMessage.Send($"{Name} に{damage}ダメージ");
			HP -= damage;
		}

		public void Attack(ICharacter character)
		{
			SystemMessage.Send($"{Name} の攻撃");
			character.TakeDamage(Power);
		}

		public abstract void Dead();
	}
}
