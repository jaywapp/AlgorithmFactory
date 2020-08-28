using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Algorithms.Algorithms
{
    /// <summary>
    /// Convex Hull 생성을 위한 정적함수
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Convex_hull"/>
    public static class ConvexHull
    {
        /// <summary>
        /// 좌표를 갖는 대상에 대해 ConvexHull 정보를 생성합니다.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="items"></param>
        /// <param name="locationSelector"></param>
        /// <returns></returns>
        public static List<TItem> Create<TItem>(
            List<TItem> items, Func<TItem, Point> locationSelector, double maxEdgeLength = double.MaxValue)
        {
            if (items.Count <= 2)
                return items.ToList();

            var sorted = Sort(items, locationSelector);
            var stack = new Stack<TItem>(sorted.Count);

            stack.Push(sorted[0]);
            stack.Push(sorted[1]);

            int idx = 2;
            while (idx < sorted.Count)
            {
                var item1 = stack.Pop();
                var item2 = stack.Pop();
                var nextItem = sorted[idx];

                // * 정상 단계
                // pt1, pt2와 이루는 각도가 좌측 방향인 경우 
                // pt1, pt2, next를 모두 stack에 넣고 다음 단계 진행
                if (IsCCW(item2, item1, nextItem, locationSelector))
                {
                    stack.Push(item2);
                    stack.Push(item1);
                    stack.Push(nextItem);
                    idx++;
                }
                // * 비정상 단계
                // 좌측 방향이 아니며, Stack이 비어있지 않을 경우
                // pt1로부터 다음 pt2에 대한 단계 진행
                else if (stack.Any())
                {
                    stack.Push(item2);
                }
                // * 특별 케이스
                // sorted[0]와 sorted[1] 사이를 잇는 선 위에 next가 존재하는 경우
                // ex) sorted[0] : (0, 0), sorted[1] : (3, 0), next : (1, 0)
                // (Sort 함수 특성상 탐색 첫 단계에서만 해당 경우가 발생할 수 있기 때문에 Stack이 비어있음)
                else
                {
                    stack.Push(item2);
                    stack.Push(item1);
                    idx++;
                }
            }

            return stack.Reverse().ToList();
        }

        /// <summary>
        /// item1, item2, item3의 위치에 따라 세 대상이 이루는 각도가 CCW인지 확인합니다.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="item3"></param>
        /// <param name="locationSelector"></param>
        /// <returns></returns>
        private static bool IsCCW<TItem>(
            TItem item1, TItem item2, TItem item3,
            Func<TItem, Point> locationSelector)
        {
            var loc1 = locationSelector(item1);
            var loc2 = locationSelector(item2);
            var loc3 = locationSelector(item3);

            var vector1 = loc2 - loc1;
            var vector2 = loc3 - loc1;

            return (loc1.X * loc2.Y + loc2.X * loc3.Y + loc3.X * loc1.Y) - (loc2.X * loc1.Y + loc3.X * loc2.Y + loc1.X * loc3.Y) > 0;
        }

        /// <summary>
        /// ConvexHull을 생성하기 위해 좌하단 item을 기준으로 시계반대방향으로 목록을 정렬합니다.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="items"></param>
        /// <param name="locationSelector"></param>
        /// <returns></returns>
        private static List<TItem> Sort<TItem>(
            List<TItem> items, Func<TItem, Point> locationSelector)
        {
            var result = new List<TItem>();

            // 좌하단 기준점 수집
            var leftBottom = items
                .GetMinItem(item => locationSelector(item).Y);

            result.Add(leftBottom);

            var leftBottomPoint = locationSelector(leftBottom);

            // 기준점을 제외한 다른 점
            var otherItems = items
                .Where(i => leftBottomPoint != locationSelector(i))
                .ToList();

            while (otherItems.Any())
            {
                // 기준점과 이루는 각이 가장 작은 점 목록 수집
                var minDegreeItems = otherItems
                    .GetMinItems(o =>
                    {
                        var otherPoint = locationSelector(o);
                        var vector = otherPoint - leftBottomPoint;

                        return vector.GetDegree();
                    })
                    .ToList();

                // 기준점과 이루는 각이 가장 작은 점 목록이 여러 개 일 경우,
                // 가장 먼 점을 다음 점으로 판단
                var nextItem = minDegreeItems.Count == 1
                    ? minDegreeItems[0]
                    : minDegreeItems.GetMinItem(i => WPFGeomOperator.GetDistance(leftBottomPoint, locationSelector(i)));

                result.Add(nextItem);
                otherItems.Remove(nextItem);
            }

            return result;
        }
    }
}
