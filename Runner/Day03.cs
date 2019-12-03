using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day03 :  Day
    {
        public override string First(string input)
        {
            var wireParts = input.Split(";");
            var path1 = MakePath(wireParts[0]);
            var path2 = MakePath(wireParts[1]);
            return FindClosestIntersection(path1, path2).ToString();
        }

        public override string Second(string input)
        {
            var wireParts = input.Split(";");
            var path1 = MakePath(wireParts[0]);
            var path2 = MakePath(wireParts[1]);
            return FindClosestStepsIntersection(path1, path2).ToString();
        }

        ////////////////////////////////////////////////////////

        private int FindClosestStepsIntersection(Path path1, Path path2)
        {
            int nearest = int.MaxValue;
            foreach (var xy in path1.Points)
            {
                if (xy.X == 0 && xy.Y == 0) continue;
                if (path2.Visited.Has(xy))
                {
                    var dist = StepsTo(path1,xy)+StepsTo(path2,xy);
                    if (dist < nearest) nearest = dist;
                }

            }
            return nearest;
        }

        private int FindClosestIntersection(Path path1, Path path2)
        {
            int nearest = int.MaxValue;
            foreach (var xy in path1.Points)
            {
                if (xy.X == 0 && xy.Y == 0) continue;
                if (path2.Visited.Has(xy))
                {
                    var dist = Math.Abs(xy.X) + Math.Abs(xy.Y);
                    if (dist < nearest) nearest = dist;
                }

            }
            return nearest;
        }


        private int StepsTo(Path path, XY xy)
        {
            int i = -1;
            foreach (var pathXY in path.Points)
            {
                i++;
                if (pathXY.X == xy.X && pathXY.Y == xy.Y) return i;
            }
            throw new InvalidOperationException();
        }

        private Path MakePath(string wire)
        {
            var path = new Path();
            var pos = new XY(0, 0);
            path.Move(pos);

            foreach (var move in wire.GetParts(","))
            {
                var dir = move[0];
                var dist = int.Parse(move.Substring(1));
                for (int i = 0; i < dist; i++)
                {
                    pos = pos.Move(XY.CharToDir[dir]);
                    path.Move(pos);
                }
            }
            return path;
        }
    }
}
