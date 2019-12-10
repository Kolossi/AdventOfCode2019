using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day10 :  Day
    {
        public override string First(string input)
        {
            LogEnabled = false;
            var asteroids = GetAsteroids(GetLines(input));
            var result = GetMostSeen(asteroids);
            return result.ToString();
        }

        public override string Second(string input)
        {
            throw new NotImplementedException("Second");
        }

        //public override string FirstTest(string input)
        //{
        //    throw new NotImplementedException("FirstTest");
        //}

        public override string SecondTest(string input)
        {
            throw new NotImplementedException("SecondTest");
        }

        ////////////////////////////////////////////////////////

        Dictionary<AsteroidEnum, char> ASTEROID_VALUE_MAP = new Dictionary<AsteroidEnum, char>() {
            { AsteroidEnum.None, '.' }, 
            { AsteroidEnum.Asteroid, '#' }, 
            { AsteroidEnum.ObscuredAsteroid, 'o' },
            { AsteroidEnum.Station, 'S' },
            { AsteroidEnum.Target, 'T' }
        };

        public enum AsteroidEnum
        {
            None = 0,
            Asteroid = 1,
            ObscuredAsteroid = 2,
            Station = 3,
            Target = 4
        };

        public class StationResult
        {
            public XY XY;
            public int Seen;
            public override string ToString()
            {
                return string.Format("{0}: seen={1}", XY, Seen);
            }
        }

        private Map<AsteroidEnum> GetAsteroids(string[] inputLines)
        {
            var asteroids = new Map<AsteroidEnum>();
            for (int x = 0; x < inputLines[0].Length; x++)
            {
                for (int y = 0; y < inputLines.Length; y++)
                {
                    if (inputLines[y][x]=='#')
                    {
                        asteroids.Set(x, y, AsteroidEnum.Asteroid);
                    }
                }

            }

            return asteroids;
        }

        private StationResult GetMostSeen(Map<AsteroidEnum> asteroids)
        {
            StationResult result = new StationResult() { Seen = 0 };
            foreach (var station in asteroids.GetAllCoords())
            {
                var asteroidsToCheck = new Map<AsteroidEnum>(asteroids);
                asteroidsToCheck.Set(station, AsteroidEnum.Station);
                //LogLine("Before processing {0}", station);
                //LogLine(asteroidsToCheck.GetStateString(ASTEROID_VALUE_MAP));
                var checkList = asteroidsToCheck
                    .GetAllCoords()
                    .Where(xy => !xy.Equals(station))
                    .OrderBy(xy => xy.GetManhattanDistanceTo(station));
                foreach (var toCheck in checkList)
                {
                    if (asteroidsToCheck.Get(toCheck) != AsteroidEnum.Asteroid) continue;
                    Obscure(asteroidsToCheck, checkList, station, toCheck);
                }

                int seen = asteroidsToCheck.GetAllValues().Count(a => a == AsteroidEnum.Asteroid);
                if (station.Equals(new XY(11, 13)))
                {
                    LogEnabled = true;
                    LogLine("{0} : total seen = {1}", station, seen);
                    LogLine(asteroidsToCheck.GetStateString(ASTEROID_VALUE_MAP));
                }
                if (seen > result.Seen) result = new StationResult() { XY=station, Seen = seen };
            }
            return result;
        }

        private void Obscure(Map<AsteroidEnum> asteroids, IOrderedEnumerable<XY> checkList, XY station, XY target)
        {
            // simple vertical case (avoids div by zero in general solution
            
            if (station.X==target.X)
            {
                var isAbove = target.Y > station.Y;
                var obscured = checkList
                    .Where(cl => !cl.Equals(target) && cl.X == target.X && ((isAbove && cl.Y > target.Y) || (!isAbove && cl.Y < target.Y)));
                foreach (var o in obscured) asteroids.Set(o, AsteroidEnum.ObscuredAsteroid);
                return;
            }

            bool targetIsRight = (station.X < target.X);

            // line y=mx+c
            decimal m = (decimal)(station.Y - target.Y) / (decimal)(station.X - target.X);
            decimal c = (decimal)station.Y - m * (decimal)station.X;
            
            //var debugMap = new Map<AsteroidEnum>(asteroids);
            //debugMap.Set(target, AsteroidEnum.Target);
            //LogLine("Before checks");
            //LogLine(debugMap.GetStateString(ASTEROID_VALUE_MAP));

            foreach (var toCheck in checkList)
            {
                //debugMap = new Map<AsteroidEnum>(asteroids);
                //debugMap.Set(target, AsteroidEnum.Target);
                //LogLine("Checking {0}", toCheck);
                //LogLine(debugMap.GetStateString(ASTEROID_VALUE_MAP));

                if (toCheck.Equals(target) || asteroids.Get(toCheck) != AsteroidEnum.Asteroid) continue;
                if ((targetIsRight && toCheck.X < target.X) || (!targetIsRight && toCheck.X > target.X)) continue;
                if (IsOnLine(toCheck, m, c)) asteroids.Set(toCheck, AsteroidEnum.ObscuredAsteroid);

                //debugMap = new Map<AsteroidEnum>(asteroids);
                //debugMap.Set(target, AsteroidEnum.Target);
                //LogLine("Checked {0}", toCheck);
                //LogLine(debugMap.GetStateString(ASTEROID_VALUE_MAP));

            }
        }

        private bool IsOnLine(XY xy, decimal m, decimal c)
        {
            return Math.Abs(m * ((decimal)xy.X) + c - (decimal)xy.Y) < 0.0000000000001m;
        }
    }
}
