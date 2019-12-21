using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day20 : Day
    {
        public override string First(string input)
        {
            var Maze = new PlutoMaze(input);
            //LogLine("Portals:{0}", Maze.Portals);
            //LogLine(Maze.Map.GetStateString(new Dictionary<char, char> { { '\0', ' ' },{ '.', '.' }, { '#', '#' } });
            return Maze.FindShortestPath().Length.ToString();
        }

        public override string Second(string input)
        {
            var Maze = new PlutoMaze(input);
            Maze.CreateNodeLinks();
            return Maze.NodeLinks.ToString();
        }

        //public override string Second(string input)
        //{
        //    throw new NotImplementedException("Second");
        //}

        //public override string FirstTest(string input)
        //{
        //    throw new NotImplementedException("FirstTest");
        //}

        //public override string SecondTest(string input)
        //{
        //    throw new NotImplementedException("SecondTest");
        //}

        ////////////////////////////////////////////////////////

        public class Portal
        {
            public string Name;
            public XY Coord;
            public bool IsOuter;
            public string UniqueName
            {
                get
                {
                    return string.Format("{0}{1}", Name, IsOuter ? 'O' : 'I');
                }
            }


            public override string ToString()
            {
                return string.Format("{0}:{1}", UniqueName, Coord);
            }
        }

        public class MyPathState : ICloneable
        {
            public string StartNodeName;

            public object Clone()
            {
                return new MyPathState { StartNodeName = StartNodeName };
            }
        }

        public class PlutoMaze
        {
            public Map<char> Map;
            public List<Portal> Portals = new List<Portal>();
            public Dictionary<string, Dictionary<string, long>> NodeLinks = new Dictionary<string, Dictionary<string, long>>();

            public PlutoMaze(string input)
            {
                Map = new Map<char>();
                var data = input.GetLines();
                for (int x = 0; x < data[0].Length; x++)
                {
                    for (int y = 0; y < data.Length; y++)
                    {
                        char c = data[y][x];
                        if (c == ' ') continue;
                        if (c == '.' || c == '#')
                        {
                            Map.Set(x, y, c);
                            continue;
                        }
                        if (!char.IsLetter(c)) throw new InvalidOperationException();
                        if (y>0 && char.IsLetter(data[y - 1][x]) || x>0 && char.IsLetter(data[y][x - 1])) continue;
                        // top outer
                        if (y == 0)
                        {
                            Portals.Add(new Portal()
                            {
                                Coord = new XY(x, y + 2),
                                IsOuter = true,
                                Name = string.Format("{0}{1}", c, data[y + 1][x])
                            });
                            continue;
                        }

                        // bottom outer
                        if (y == data.Length - 2)
                        {
                            Portals.Add(new Portal()
                            {
                                Coord = new XY(x, y - 1),
                                IsOuter = true,
                                Name = string.Format("{0}{1}", c, data[y + 1][x])
                            });
                            continue;
                        }

                        // left outer
                        if (x == 0)
                        {
                            Portals.Add(new Portal()
                            {
                                Coord = new XY(x + 2, y),
                                IsOuter = true,
                                Name = string.Format("{0}{1}", c, data[y][x + 1])
                            });
                            continue;
                        }

                        //right outer
                        if (x == data[0].Length - 2)
                        {
                            Portals.Add(new Portal()
                            {
                                Coord = new XY(x - 1, y),
                                IsOuter = true,
                                Name = string.Format("{0}{1}", c, data[y][x + 1])
                            });
                            continue;
                        }
                        // top inner
                        if (data[y - 1][x] == '.')
                        {
                            Portals.Add(new Portal()
                            {
                                Coord = new XY(x, y - 1),
                                IsOuter = false,
                                Name = string.Format("{0}{1}", c, data[y + 1][x])
                            });
                            continue;
                        }
                        //bottom inner
                        if (y < data.Length - 2 && data[y + 2][x] == '.')
                        {
                            Portals.Add(new Portal()
                            {
                                Coord = new XY(x, y + 2),
                                IsOuter = false,
                                Name = string.Format("{0}{1}", c, data[y + 1][x])
                            });
                            continue;
                        }
                        // left inner
                        if (data[y][x - 1] == '.')
                        {
                            Portals.Add(new Portal()
                            {
                                Coord = new XY(x - 1, y),
                                IsOuter = false,
                                Name = string.Format("{0}{1}", c, data[y][x + 1])
                            });
                            continue;
                        }
                        // right inner
                        if (x<data[0].Length-2 && data[y][x + 2] == '.')
                        {
                            Portals.Add(new Portal()
                            {
                                Coord = new XY(x + 2, y),
                                IsOuter = false,
                                Name = string.Format("{0}{1}", c, data[y][x + 1])
                            });
                            continue;
                        }

                    }
                }
            }

            public void CreateNodeLinks()
            {
                foreach (var portal in Portals) RouteSolver<char>.FindSingleShortestPath(portal.Coord, Map, GetNextNodes, HaveNeverFoundEnd, ScorePath);
            }

            private long ScorePath(MapPathState<char> mps)
            {
                var c = mps.Map.Get(mps.Path.XY);
                var portalHere = Portals.FirstOrDefault(p => p.Coord.Equals(mps.Path.XY));
                if (portalHere== null) return 1;
                MyPathState myState = ((MyPathState)mps.MyState);
                if (mps.Path.Length == 0)
                {
                    myState = new MyPathState() { StartNodeName = portalHere.UniqueName };
                    mps.MyState = myState;
                    return 1;
                }
                if (!StoreLinkAndIsShortest(myState.StartNodeName, portalHere.UniqueName, mps.Path.Length)) return long.MaxValue; // will exit this walk
                return 1;
                
            }

            private bool HaveNeverFoundEnd(MapPathState<char> mps)
            {
                return false;
            }

            private IEnumerable<XY> GetNextNodes(MapPathState<char> mps)
            {
                return mps.Path.XY.GetAdjacentCoords().Where(a => mps.Map.TryGetValue(a, out char c) && c== '.');
            }

            private bool StoreLinkAndIsShortest(string from, string to, long length, bool addReverse = true)
            {
                bool result = false;
                if (!NodeLinks.TryGetValue(from, out var toDict))
                {
                    toDict = new Dictionary<string, long>();
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

            public Path FindShortestPath()
            {
                var startPos = Portals.First(p => p.Name == "AA").Coord;
                var endPos = Portals.First(p => p.Name == "ZZ").Coord;
                return RouteSolver<char>.FindSingleShortestPath(startPos, Map, GetNextPos, HaveFoundEnd);
            }

            private bool HaveFoundEnd(MapPathState<char> mps)
            {
                bool result = false;
                XY xy = mps.Path.XY;
                var portal = Portals.FirstOrDefault(p => p.Coord.Equals(xy));
                result = (portal != null && portal.Name == "ZZ");
                return result;
            }

            private IEnumerable<XY> GetNextPos(MapPathState<char> mps)
            {
                XY xy = mps.Path.XY;
                var nextNodes = xy.GetAdjacentCoords().Where(a => Map.TryGetValue(a, out char c) && c == '.');
                var portal = Portals.FirstOrDefault(p => p.Coord.Equals(xy));
                if (portal!=null && portal.Name!="ZZ")
                {
                    nextNodes = Portals.Where(p => p.Name == portal.Name && p.IsOuter != portal.IsOuter).Select(p=>p.Coord).Union(nextNodes);
                }
                return nextNodes;
            }
        }
    }
}
