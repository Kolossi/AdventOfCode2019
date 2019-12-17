using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day15 :  Day
    {
        private static Map<ShipMap> Map;
        public override string First(string input)
        {
            var data = input.GetParts(",").Select(p => long.Parse(p)).ToArray();
            var robot = new Robot(data);
            Map = robot.GetMap();
            LogLine(Map.GetStateString(SHIP_VALUE_MAP));
            LogLine("Got map, looking for shortest path");
            var shortestPath = RouteSolver<ShipMap>.FindSingleShortestPath(new XY(0, 0), Map, GetNextNodes, HaveFoundEnd);
            return shortestPath.Length.ToString();
        }

        public override string Second(string input)
        {
            // 447 too high
            if (Map == null)
            {
                var data = input.GetParts(",").Select(p => long.Parse(p)).ToArray();
                var robot = new Robot(data);
                Map = robot.GetMap();
            }
            var oxygenTime = FindOxygenFillTime(Map);
            return oxygenTime.ToString();
        }

        private long FindOxygenFillTime(Map<ShipMap> map)
        {
            long oxygenTime = 0;
            var newOxygens = new HashSet<XY>(map.GetAllCoords().Where(c => map.Get(c) == ShipMap.Oxygen));
            while (newOxygens.Any())
            {
                var oxygensToDiffuse = newOxygens;
                newOxygens = new HashSet<XY>();
                foreach (var oxygen in oxygensToDiffuse)
                {
                    foreach (var newOxygen in oxygen.GetAdjacentCoords().Where(p=>map.Get(p)==ShipMap.Free))
                    {
                        newOxygens.Add(newOxygen);
                    }
                }

                foreach (var newOxygen in newOxygens) map.Set(newOxygen, ShipMap.Oxygen);
                oxygenTime++;
            }
            oxygenTime--;
            return oxygenTime;
        }

        public override string FirstTest(string input)
        {
            throw new NotImplementedException("FirstTest");
        }

        public override string SecondTest(string input)
        {
            throw new NotImplementedException("SecondTest");
        }

        ////////////////////////////////////////////////////////

        public static Dictionary<long, Direction> RobotToMapDirection = new Dictionary<long, Direction>()
        {
            { 1, Direction.North },
            { 2, Direction.South },
            { 3, Direction.West },
            { 4, Direction.East }
        };

        public static Dictionary<Direction, long> MapToRobotDirection = new Dictionary<Direction, long>()
        {
            { Direction.North, 1 },
            { Direction.South, 2 },
            { Direction.West, 3 },
            { Direction.East, 4 }
        };

        public enum ShipMap
        {
            Unexplored = 0,
            Free = 1,
            Wall = 2,
            Oxygen = 3
        }

        public static Dictionary<ShipMap, char> SHIP_VALUE_MAP = new Dictionary<ShipMap, char>()
        {
            {ShipMap.Unexplored, ' ' },
            {ShipMap.Free, '.' },
            {ShipMap.Wall, 'W' },
            {ShipMap.Oxygen, 'O' }
        };


        public class Robot
        {
            public long[] Data;
            public Intcode Intcode;
            public Random rnd = new Random();
            public int MapTick = 0;
            public static int MAXTICK = int.MaxValue;

            public Robot(long[] data)
            {
                Data = data;
                Intcode = new Intcode(data);
            }

            private bool IsComplete(MapWalkState<ShipMap> mapWalkState)
            {
                if (MapTick++ > MAXTICK)
                {
                    MapTick = 0;
                    LogLine("{0} {1}", mapWalkState.Position, mapWalkState.Direction);
                    LogLine("map count: {0}", mapWalkState.Map.Count);
                    LogLine(mapWalkState.Map.GetStateString(SHIP_VALUE_MAP, mapWalkState.Position, mapWalkState.Direction));
                }
                return (mapWalkState.Map.TryGetValue(mapWalkState.Position, out ShipMap value) && value == ShipMap.Oxygen);
            }

            private MapMoveResponse<ShipMap> GetMoveResult(MapWalkState<ShipMap> mapWalkState)
            {
                var robotDir = MapToRobotDirection[mapWalkState.Direction];
                Intcode.InputQueue.Enqueue((long)robotDir);
                Intcode.Resume();
                var resultValue = Intcode.OutputQueue.Dequeue();
                if (resultValue == 0) return new MapMoveResponse<ShipMap>() { Value = ShipMap.Wall, CanMove = false };
                else if (resultValue == 1) return new MapMoveResponse<ShipMap>() { Value = ShipMap.Free, CanMove = true };
                else if (resultValue == 2) return new MapMoveResponse<ShipMap>() { Value = ShipMap.Oxygen, CanMove = true };
                throw new ArgumentOutOfRangeException("result");
            }

            private Direction GetNextDirection(MapWalkState<ShipMap> mapWalkState)
            {
                Direction[] directionsToTry = new Direction[]
                                            {
                                                mapWalkState.Direction,
                                                mapWalkState.Direction.TurnRight(),
                                                mapWalkState.Direction.TurnLeft(),
                                                mapWalkState.Direction.TurnRight().TurnRight()
                                            };

                foreach (var direction in directionsToTry)
                {
                    if (!mapWalkState.Map.Has(mapWalkState.Position.Move(direction))) return direction;
                }

                // swap straight/dominant turn to get out of standard walkright/left maze trap:
                // #######
                // #.....#
                // #.#.#.#
                // ###.###
                if (rnd.Next(100)>50) 
                {
                    var temp = directionsToTry[0];
                    directionsToTry[0] = directionsToTry[1];
                    directionsToTry[1] = temp;
                }

                foreach (var direction in directionsToTry)
                {

                    if (mapWalkState.Map.TryGetValue(mapWalkState.Position.Move(direction), out ShipMap value) && value != ShipMap.Wall) return direction;
                }

                throw new InvalidOperationException();
            }

            public Map<ShipMap> GetMap()
            {
                var mapState = new MapWalkState<ShipMap>()
                {
                    Position = new XY(0, 0),
                    Direction = Direction.North,
                    Map = new Map<ShipMap>()
                };

                mapState = RouteSolver<ShipMap>.WalkMap(mapState, GetNextDirection, GetMoveResult, IsComplete);
                // go again, to fill in blanks
                mapState.Position = new XY(0, 0);
                Intcode = new Intcode(Data);
                mapState = RouteSolver<ShipMap>.WalkMap(mapState, GetNextDirection, GetMoveResult, IsComplete);

                return mapState.Map;
            }
        }

        private IEnumerable<XY> GetNextNodes(MapPathState<ShipMap> mapPathState)
        {
            return mapPathState.Path.XY.GetAdjacentCoords()
                .Where(p => mapPathState.Map.TryGetValue(p, out ShipMap val) && (val == ShipMap.Free || val == ShipMap.Oxygen));
        }

        private bool HaveFoundEnd(MapPathState<ShipMap> mapPathState)
        {
            return (mapPathState.Map.TryGetValue(mapPathState.Path.XY, out ShipMap val) && val == ShipMap.Oxygen);
        }

    }
}
