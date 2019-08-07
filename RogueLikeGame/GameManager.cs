using System;
using System.Collections.Generic;

namespace RogueLikeGame
{
	internal static class GameManager
	{
		public static Random random = new Random();
		public static Player Player { get; private set; }
		public static List<Enemy> Enemies { get; private set; } = new List<Enemy>();
		public static IEnumerable<ICharacter> Characters
		{
			get
			{
				yield return Player;
				foreach(Enemy enemy in Enemies)
				{
					yield return enemy;
				}
			}
		}

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
				Player.Move(keyInfo.KeyChar);
				foreach (var e in Enemies)
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
			Player.RandomTeleport();
			Enemies = new List<Enemy>();
			SpawnEnemy();
			//MapManager.CurrentMap.mapVisible.SetVisible(Player);
		}

		private static void SpawnEnemy()
		{
			if (Enemies == null)
			{
				Enemies = new List<Enemy>();
			}
			var enemy = new Enemy();
			enemy.RandomTeleport();
			Enemies.Add(enemy);
		}
	}
}
