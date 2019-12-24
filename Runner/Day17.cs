using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day17 :  Day
    {
        public Map<ScafMap> Map;

        public override string First(string input)
        {
            long[] data = input.GetParts(",").Select(p => long.Parse(p)).ToArray();
            Map = GetMap(data);
            LogEnabled = true;
            LogLine(Map.GetStateString(SCAF_VALUE_MAP));
            return GetCalibrationCode(Map).ToString();
        }

        public override string Second(string input)
        {
//            solved by human(TM) - naive iterative algorithm:
//            L,10,R,12,R,12,R,6,R,10,L,10,L,10,R,12,R,12,R,10,L,10,L,12,R,6,R,6,R,10,L,10,R,10,L,10,L,12,R,6,R,6,R,10,L,10,R,10,L,10,L,12,R,6,L,10,R,12,R,12,R,10,L,10,L,12,R,6
//            {A},{B},{A},{C},{B},{C},{B},{C},{A},{C}
//            A = L,10,R,12,R,12
//            B = R,6,R,10,L,10
//            C = R,10,L,10,L,12,R,6

            var path = GetPath(Map);
            LogLine(string.Join(",", path.ToLogo()));
            string[] program = new string[]
            {
                "A,B,A,C,B,C,B,C,A,C",
                "L,10,R,12,R,12",
                "R,6,R,10,L,10",
                "R,10,L,10,L,12,R,6",
                "n",
                ""
            };
            long[] data = input.GetParts(",").Select(p => long.Parse(p)).ToArray();

            var intcode = new Intcode(data);
            intcode.Data[0] = 2;
            foreach (var c in string.Join("\n",program))
            {
                intcode.InputQueue.Enqueue(c);
            }
            intcode.Resume();
            return intcode.OutputQueue.Last().ToString();
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

        public enum ScafMap
        {
            Space = 46,
            Scaffolding = 35,
            Vacuum = 94
        }

        public static Dictionary<ScafMap, char> SCAF_VALUE_MAP = new Dictionary<ScafMap, char>()
        {
            { ScafMap.Space,'.' },
            {ScafMap.Scaffolding,'#' },
            {ScafMap.Vacuum,'^' }
        };
            

        public Map<ScafMap> GetMap(long[] data)
        {
            var intcode = new Intcode(data);
            intcode.Resume();
            int x = 0, y = 0;
            var map = new Map<ScafMap>();
            while (intcode.OutputQueue.Any())
            {
                var value = intcode.OutputQueue.Dequeue();
                if (value==10)
                {
                    x = 0;
                    y++;
                    continue;
                }
                map.Set(x, y, (ScafMap)value);
                x++;
            }
            return map;
        }

        public IEnumerable<XY> GetIntersections(Map<ScafMap> map)
        {
            var scafCoords = map.GetAllCoords().Where(c => map.Get(c) == ScafMap.Scaffolding);
            var intersections = scafCoords.Where(s => s.GetAdjacentCoords().All(a => map.TryGetValue(a, out ScafMap value) && value == ScafMap.Scaffolding));
            return intersections;
        }

        public int GetCalibrationCode(Map<ScafMap> map)
        {
            var intersections = GetIntersections(map);
            return intersections.Select(i => i.X * i.Y).Sum();
        }

        public Path GetPath(Map<ScafMap> map)
        {
            var vacuumPos = map.GetAllCoords().Where(c => map.Get(c) == ScafMap.Vacuum).First();
            var path = RouteSolver<ScafMap>.FindSingleShortestPath(vacuumPos, map, GetNextNodes, HaveFoundEnd, routeRevisitsAllowed: true);
            return path;
        }

        private bool HaveFoundEnd(MapPathState<ScafMap> mps)
        {
            if (mps.Map.Get(mps.Path.XY) == ScafMap.Vacuum) return false;
            IEnumerable<XY> adjacentNotSpace = mps.Path.XY.GetAdjacentCoords()
                .Where(c => mps.Map.TryGetValue(c, out var val) && val != ScafMap.Space);
            bool result = adjacentNotSpace.Count() == 1;
            return result;
        }

        private IEnumerable<XY> GetNextNodes(MapPathState<ScafMap> mps)
        {
            if (mps.Path.Length == 0) return mps.Path.XY.GetAdjacentCoords().Where(c => mps.Map.Get(c) == ScafMap.Scaffolding);
            var direction = mps.Path.XY.DirectionFrom(mps.Path.Points.Last.Previous.Value);
            var nextInDir = mps.Path.XY.Move(direction);
            if (mps.Map.TryGetValue(nextInDir, out var val) && val == ScafMap.Scaffolding) return new XY[] { nextInDir };
            var toLeft = mps.Path.XY.Move(direction.TurnLeft());
            if (mps.Map.TryGetValue(toLeft, out val) && val == ScafMap.Scaffolding) return new XY[] { toLeft };
            var toRight = mps.Path.XY.Move(direction.TurnRight());
            if (mps.Map.TryGetValue(toRight, out val) && val == ScafMap.Scaffolding) return new XY[] { toRight };
            return Enumerable.Empty<XY>();
        }

    }
}
