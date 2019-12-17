using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day17 :  Day
    {
        public override string First(string input)
        {
            long[] data = input.GetParts(",").Select(p => long.Parse(p)).ToArray();
            var map = GetMap(data);
            LogLine(map.GetStateString(SCAF_VALUE_MAP));
            return GetCalibrationCode(map).ToString();
        }

        public override string Second(string input)
        {
            throw new NotImplementedException("Second");
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
            var path = RouteSolver<ScafMap>.FindSingleShortestPath(vacuumPos, map, GetNextNodes, HaveFoundEnd);
            throw new NotImplementedException();    
        }

        private bool HaveFoundEnd(MapPathState<ScafMap> mps)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<XY> GetNextNodes(MapPathState<ScafMap> mps)
        {
            throw new NotImplementedException();
        }

    }
}
