using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day24 :  Day
    {
        public override string First(string input)
        {
            return GetBiodiversityRating(input);
        }

        public override string Second(string input)
        {
            return GetRecursiveBugCount(input, 200);
        }

        //public override string FirstTest(string input)
        //{
        //    throw new NotImplementedException("FirstTest");
        //}

        public override string SecondTest(string input)
        {
            return GetRecursiveBugCount(input, 10);
        }


        ////////////////////////////////////////////////////////

        private string GetRecursiveBugCount(string input, int time)
        {
            var data = input.GetLines();
            var bugs = new HashSet<XYZ>();
            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[0].Length; x++)
                {
                    if (data[y][x] == '#') bugs.Add(new XYZ(x, y, 0));
                }
            }

            for (int t = 0; t < time; t++)
            {
                bugs = IterateBugs(bugs);
            }

            return bugs.Count().ToString();
        }

        private HashSet<XYZ> IterateBugs(HashSet<XYZ> bugs)
        {
            var adjacentToBugs = bugs.SelectMany(b => GetAdjacent(b));
            var toCheck = new HashSet<XYZ>(bugs.AsEnumerable());
            toCheck.UnionWith(adjacentToBugs.ToList());
            var toAdd = new List<XYZ>();
            var toRemove = new List<XYZ>();
            foreach (var tile in toCheck)
            {
                bool isBug = bugs.Contains(tile);
                var adjacent = GetAdjacent(tile);
                var tileAdjacentBugs = adjacent.Where(a => bugs.Contains(a));
                var adjacentBugCount = tileAdjacentBugs.Count();
                if (isBug && adjacentBugCount != 1) toRemove.Add(tile);
                if (!isBug && (adjacentBugCount == 1 || adjacentBugCount == 2)) toAdd.Add(tile);
            }
            return new HashSet<XYZ>(bugs.Except(toRemove).Union(toAdd));
        }

        private IEnumerable<XYZ> GetAdjacent(XYZ b)
        {
            IEnumerable<XYZ> adjacent =
                new XY((int)b.X, (int)b.Y)
                .GetAdjacentCoords()
                .Where(a => a.X >= 0 && a.X < 5 && a.Y >= 0 && a.Y < 5 && (a.X != 2 || a.Y != 2))
                .Select(xy => new XYZ(xy.X, xy.Y, b.Z));

            if (b.X == 4) adjacent = adjacent.Union(new XYZ[] { new XYZ(3, 2, b.Z - 1) });
            else if (b.X == 3 && b.Y == 2) adjacent = adjacent.Union(Enumerable.Range(0, 5).Select(i => new XYZ(4, i, b.Z + 1)));
            else if (b.X == 1 && b.Y == 2) adjacent = adjacent.Union(Enumerable.Range(0, 5).Select(i => new XYZ(0, i, b.Z + 1)));
            else if (b.X == 0) adjacent = adjacent.Union(new XYZ[] { new XYZ(1, 2, b.Z - 1) });

            if (b.Y == 4) adjacent = adjacent.Union(new XYZ[] { new XYZ(2, 3, b.Z - 1) });
            else if (b.Y == 3 && b.X == 2) adjacent = adjacent.Union(Enumerable.Range(0, 5).Select(i => new XYZ(i, 4, b.Z + 1)));
            else if (b.Y == 1 && b.X == 2) adjacent = adjacent.Union(Enumerable.Range(0, 5).Select(i => new XYZ(i, 0, b.Z + 1)));
            else if (b.Y == 0) adjacent = adjacent.Union(new XYZ[] { new XYZ(2, 1, b.Z - 1) });

            return adjacent;
        }

        private void LogState(HashSet<XYZ> bugs)
        {

        }

        private string GetBiodiversityRating(string input)
        {
            var state = string.Join("", input.GetLines());
            var states = new HashSet<string>();
            while (!states.Contains(state))
            {
                states.Add(state);
                state = string.Join("",state.Select((c,i) => GetNewState(c, i, state)));
            }
            long stateVal = StringToBin(state);
            return stateVal.ToString();
        }

        private static char GetNewState(char c, int i, string state)
        {
            int sum = 0;
            if (i % 5 > 0 && state[i - 1] == '#') sum++;
            if (i % 5 < 4 && state[i + 1] == '#') sum++;
            if (i >= 5 && state[i - 5] == '#') sum++;
            if (i < state.Length - 5 && state[i + 5] == '#') sum++;
            var newState = c;
            if (c=='#' && sum!=1) newState = '.';
            if (c == '.' && (sum == 1 || sum == 2)) newState = '#';
            return newState;
        }

        private static long StringToBin(string state)
        {
            long stateVal = 0;
            for (int i = 0; i < state.Length; i++)
            {
                if (state[i] == '#') stateVal += (long)Math.Pow(2, i);
            }

            return stateVal;
        }
    }
}
