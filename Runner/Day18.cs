using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day18 : Day
    {
        // takes several minutes
        //public override string First(string input)
        //{
        //    var maze = new Maze(input);
        //    maze.CreateNodeLinks();
        //    //LogEnabled = true;
        //    LogEnabled = false;
        //    var route = maze.FindShortestRoute();
        //    //var test = maze.DetectRouteLoop(new List<char>("@a@a".ToCharArray()));

        //    return maze.ScoreRoute(route).ToString();
        //}

        public override string First(string input)
        {
            throw new NotImplementedException("First");
        }

        public override string Second(string input)
        {
            throw new NotImplementedException("Second");
        }

        // last test takes several minutes
        public override string FirstTest(string input)
        {
            throw new NotImplementedException("FirstTest");
        }

        public override string SecondTest(string input)
        {
            throw new NotImplementedException("SecondTest");
        }

        ////////////////////////////////////////////////////////

        public class MyPathState : ICloneable
        {
            public char StartNode;

            public object Clone()
            {
                return new MyPathState { StartNode = StartNode };
            }
        }

        public class Maze
        {
            public Map<char> Map;
            public List<XY> Keys;
            public List<XY> Doors;
            public XY Entrance;
            public Dictionary<char, Dictionary<char, long>> NodeLinks = new Dictionary<char, Dictionary<char, long>>();

            public Maze(string input)
            {
                Map = new Map<char>();
                Keys = new List<XY>();
                Doors = new List<XY>();
                var lines = input.GetLines();
                var xsize = lines[0].Length;
                var ysize = lines.Length;
                for (int y = 0; y < ysize; y++)
                {
                    for (int x = 0; x < xsize; x++)
                    {
                        char c = lines[y][x];
                        Map.Set(x, y, lines[y][x]);
                        if (char.IsLower(c)) Keys.Add(new XY(x, y));
                        else if (char.IsUpper(c)) Doors.Add(new XY(x, y));
                        else if (c == '@') Entrance = new XY(x, y);
                    }
                }
            }

            public void CreateNodeLinks()
            {
                RouteSolver<char>.FindSingleShortestPath(Entrance, Map, GetNextNodes, HaveFoundEnd, ScorePath);
                foreach (var key in Keys) RouteSolver<char>.FindSingleShortestPath(key, Map, GetNextNodes, HaveFoundEnd, ScorePath);
                foreach (var door in Doors) RouteSolver<char>.FindSingleShortestPath(door, Map, GetNextNodes, HaveFoundEnd, ScorePath);
            }

            private long ScorePath(MapPathState<char> mps)
            {
                var c = mps.Map.Get(mps.Path.XY);
                if (c == '.') return 1;
                MyPathState myState = ((MyPathState)mps.MyState);
                if (mps.Path.Length == 0)
                {
                    myState = new MyPathState() { StartNode = c };
                    mps.MyState = myState;
                    return 1;
                }
                StoreLinkAndIsShortest(myState.StartNode, c, mps.Path.Length);
                return long.MaxValue; // will exit this walk

                //if (!StoreLinkAndIsShortest(myState.StartNode, c, mps.Path.Length)) return long.MaxValue; // will exit this walk
                //return 1;
                
            }

            private bool HaveFoundEnd(MapPathState<char> mps)
            {
                //var c = mps.Map.Get(mps.Path.XY);
                //return c != '.';
                return false;
            }

            private IEnumerable<XY> GetNextNodes(MapPathState<char> mps)
            {
                return mps.Path.XY.GetAdjacentCoords().Where(c => mps.Map.Get(c) != '#');
            }

            private bool StoreLinkAndIsShortest(char from, char to, long length, bool addReverse = true)
            {
                bool result = false;
                if (!NodeLinks.TryGetValue(from, out var toDict))
                {
                    toDict = new Dictionary<char, long>();
                    NodeLinks[from] = toDict;
                    toDict[to] = length;
                }
                else
                {
                    if (!toDict.TryGetValue(to, out var existingLength) || existingLength > length)
                    {
                        toDict[to] = length;
                        result = true;
                    }
                }
                if (addReverse)
                {
                    result = StoreLinkAndIsShortest(to, from, length, addReverse: false) || result;
                }
                return result;
            }

            internal List<char> FindShortestRoute()
            {

                List<char> route = RouteSolver<char>.FindSingleShortestRoute(
                    '@',
                    GetNextNodes,
                    HaveFoundEnd,
                    ScoreRoute,
                    getGlobalVisitedHash:GetKeysFoundAndPosition,
                    routeRevisitsAllowed:true
                );
                return route;
            }

            private object GetKeysFoundAndPosition(RouteAttempt<char> routeAttempt)
            {
                var keysFound = routeAttempt.Route.Where(n => char.IsLower(n)).Distinct().OrderBy(n => n);
                return string.Format("{0}:{1}", routeAttempt.Node, string.Join("",keysFound));
            }

            public IEnumerable<char> GetNextNodes(RouteAttempt<char> routeAttempt)
            {
                var route = routeAttempt.Route;
                //if (char.IsUpper(routeAttempt.Node) && !route.Contains(char.ToLower(routeAttempt.Node))) return null; // hit door without key
                if (DetectRouteLoop(route)) return null;

                //var routeCount = route.Count();
                //if (routeCount >= 4 && route[routeCount -2] == route[routeCount - 4] && route[routeCount - 1] == route[routeCount - 3]) return null; // looping
                var next = NodeLinks[routeAttempt.Node].Keys.Where(k => !char.IsUpper(k) || route.Contains(char.ToLower(k)));
                next = next.OrderBy(c => routeAttempt.Visited.Contains(c) ? 0 : 1);
                return next;
            }

            public bool DetectRouteLoop(List<char> route)
            {
                var node = route.Last();
                //if (!char.IsLower(node)) return false;
                var routeCount = route.Count();
                if (routeCount <= 2) return false;
                
                var prevNodeIndex = route.LastIndexOf(node, routeCount - 2);
                if (prevNodeIndex == -1) return false;
                for (int i = prevNodeIndex + 1; i < routeCount-1; i++)
                {
                    var c = route[i];
                    if (!char.IsLower(c)) continue;
                    if (route.IndexOf(c, 0, prevNodeIndex) < 0)
                    {
                        return false;
                    }
                }
                LogLine("Loop:{0}", route);
                return true;
            }

            public bool HaveFoundEnd(RouteAttempt<char> routeAttempt)
            {
                IEnumerable<char> leftToFind = NodeLinks.Keys.Where(k=>char.IsLower(k)).Except(routeAttempt.Visited);
                if (LogEnabled) LogLine("{0}:{1}", string.Join("", routeAttempt.Route), string.Join("", leftToFind));
                return !leftToFind.Any();
            }

            public long ScoreRoute(RouteAttempt<char> routeAttempt)
            {
                return ScoreRoute(routeAttempt.Route);
            }

            public long ScoreRoute(List<char> route)
            {
                long score = 0;
                var prev = route.First();
                foreach (var node in route.Skip(1))
                {
                    score += NodeLinks[prev][node];
                    prev = node;
                }
                return score;
            }
        }
    }
}
