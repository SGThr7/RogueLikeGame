using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
	internal class MapVisible
	{
		private readonly bool[] visibleMap;
		public int Width { get; }
		public int Height { get; }
		private const int VisibleRange= 2;

		public MapVisible(int width, int height)
		{
			this.visibleMap = new bool[width * height];
			Width = width;
			Height = height;
		}

		public bool this[int x, int y]
		{
			get => this.visibleMap[x + (y * Width)];
			private set
			{
				this.visibleMap[x + (y * Width)] = value;
			}
		}

		public void SetVisible(Player player)
		{
			int firstX = Math.Max(0, player.X - VisibleRange);
			int endX = Math.Min(player.X + VisibleRange, Width);
			int firstY = Math.Max(0, player.Y - VisibleRange);
			int endY = Math.Min(player.Y + VisibleRange, Height);

			for (int y = firstY; y <= endY; y++)
				for (int x = firstX; x <= endX; x++)
					this[x, y] = true;
		}
	}
}
