using System;
using System.Collections.Generic;

namespace RogueLikeGame
{
	static class GameManager
	{
		public static Random random = new Random();
		public static Player Player { get; private set; }
		public static List<Enemy> Enemys { get; private set; }

		static void Main(string[] args)
		{
			Initialize();
			Start();
		}

		public static void Initialize()
		{
			Console.CursorVisible = false;
			Player = new Player();
			MakeMap();
			Renderer.RenderFull();
		}

		public static void Start()
		{
			for (; ; )
			{
				var keyInfo = Console.ReadKey(true);
				if (keyInfo.KeyChar == ';')
				{
					MakeMap();
				}
				var p = Player.Move(keyInfo.KeyChar);
				foreach(var e in Enemys)
				{
					e.Move();
				}
				//if (random.NextDouble() < 1)
				//{
				//	SpawnEnemy();
				//}
				//MapManager.CurrentMap.mapVisible.SetVisible(Player);
				Renderer.Render(true);
			}
		}

		private static void MakeMap()
		{
			MapManager.Generate();
			Player.MoveComponent.RandomTeleport();
			Enemys = new List<Enemy>();
			SpawnEnemy();
			//MapManager.CurrentMap.mapVisible.SetVisible(Player);
		}

		private static void SpawnEnemy()
		{
			if (Enemys == null)
			{
				Enemys = new List<Enemy>();
			}
			var enemy = new Enemy();
			enemy.MoveComponent.RandomTeleport();
			Enemys.Add(enemy);
		}
	}
}
