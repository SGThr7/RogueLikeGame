using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
	static class RouteSearch
	{
		public static void Search((int X, int Y) from, (int X, int Y) to)
		{
			Map map = MapManager.CurrentMap;
			((int X, int Y) point, bool searched)[] floorList = map
				.GetSpritePositions(a => a.CanWalk)
				.Select(a => ((a.X, a.Y), false))
				.ToArray();
			var queue = new Queue<(int X, int Y)>();
			var tree = new List<List<int>>();
			int depth = 0;

			int rootIndex = floorList.Indexed()
				.Where(a => a.item.point.X == from.X && a.item.point.Y == from.Y)
				.First()
				.index;
			floorList[rootIndex].searched = true;
			tree[depth++].Add(rootIndex);
			queue.Enqueue(floorList[rootIndex].point);
			while (queue.Count > 0)
			{
				(int tX, int tY) = queue.Dequeue();
				IEnumerable<(((int X, int Y) point, bool searched) item, int index)> around =
					floorList
					.Indexed()
					.Where(a =>
						!a.item.searched &&
						tX - 1 <= a.item.point.X && a.item.point.X <= tX + 1 &&
						tY - 1 <= a.item.point.Y && a.item.point.Y <= tY + 1);
				foreach ((((int X, int Y) point, _), int index) in around)
				{
					queue.Enqueue(point);
					floorList[index].searched = true;
				}
			}
		}
	}
}
