using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RogueLikeGame
{
	internal static class GameManager
	{
		[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
		public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint wFlags);
		public static uint SWP_NOSIZE = 1;
		public static uint SWP_NOZORDER = 4;

		public static Random random = new Random();
		public static Player Player { get; private set; }
		public static List<Enemy> Enemies { get; private set; } = new List<Enemy>();
		public static IEnumerable<ICharacter> Characters
		{
			get
			{
				yield return Player;
				foreach (Enemy enemy in Enemies)
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
			Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			SetWindowPos(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle,
				0, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
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
				bool isAction = Player.Move(keyInfo.KeyChar);
				Renderer.Render(true);
				if (isAction)
				{ 
					foreach (var e in Enemies)
					{
						e.Move();
					}
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
