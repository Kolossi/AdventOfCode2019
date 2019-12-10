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
            return result.Seen.ToString();
        }

        public override string Second(string input)
        {
            var asteroids = GetAsteroids(GetLines(input));
            var result = GetMostSeen(asteroids);
            var vaporizeOrder = Vaporise(result);
            return string.Format("{0}{1:d2}", vaporizeOrder[199].X, vaporizeOrder[199].Y);
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
            public Map<AsteroidEnum> Map;

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
                result = GetStationResult(asteroids, result, station);
            }
            return result;
        }

        private StationResult GetStationResult(Map<AsteroidEnum> asteroids, XY station)
        {
            return GetStationResult(asteroids, new StationResult() { Seen = 0 }, station);
        }

        private StationResult GetStationResult(Map<AsteroidEnum> asteroids, StationResult result, XY station)
        {
            var asteroidsToCheck = new Map<AsteroidEnum>(asteroids);
            asteroidsToCheck.Set(station, AsteroidEnum.Station);
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
            if (seen > result.Seen) result = new StationResult()
            {
                XY = station,
                Seen = seen,
                Map = asteroidsToCheck

            };
            return result;
        }

        private void Obscure(Map<AsteroidEnum> asteroids, IOrderedEnumerable<XY> checkList, XY station, XY target)
        {
            // simple vertical case (avoids div by zero in general solution that follows)          
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
            
            foreach (var toCheck in checkList)
            {
                if (toCheck.Equals(target) || asteroids.Get(toCheck) != AsteroidEnum.Asteroid) continue;
                if ((targetIsRight && toCheck.X < target.X) || (!targetIsRight && toCheck.X > target.X)) continue;
                if (IsOnLine(toCheck, m, c)) asteroids.Set(toCheck, AsteroidEnum.ObscuredAsteroid);
            }
        }

        private XY[] Vaporise(StationResult result)
        {
            var vaporized = new List<XY>();
            double laserAngle = double.MaxValue;
            while (true)
            {
                var station = result.XY;
                var map = result.Map;
                var vaporizedThisTime = map
                    .GetAllCoords()
                    .Where(xy => map.Get(xy) == AsteroidEnum.Asteroid)
                    .OrderBy(xy => GetLaserOrder(station, xy, laserAngle));
                vaporized.AddRange(vaporizedThisTime);
                foreach (var asteroid in vaporizedThisTime)
                {
                    map.Remove(asteroid);
                }
                if (!map.GetAllValues().Any(v => v == AsteroidEnum.Asteroid)) return vaporized.ToArray();
                laserAngle = GetAngle(station, vaporizedThisTime.Last());
                foreach (var obscured in map.GetAllCoords().Where(xy => map.Get(xy) == AsteroidEnum.ObscuredAsteroid))
                    map.Set(obscured, AsteroidEnum.Asteroid);
                result = GetStationResult(map, station);
            }
        }

        private double GetLaserOrder(XY origin, XY xy, double laserAngle)
        {
            var angle = GetAngle(origin, xy);
            if (angle <= laserAngle) angle += 360;
            return angle;
        }

        private double GetAngle(XY origin, XY xy)
        {
            int deltaX = xy.X - origin.X;
            int deltaY = xy.Y - origin.Y;
            var angle = 90 - (Math.Atan2(-deltaY, deltaX) * 180 / Math.PI);
            if (angle < 0) angle += 360;
            return angle;
        }

        private bool IsOnLine(XY xy, decimal m, decimal c)
        {
            return Math.Abs(m * ((decimal)xy.X) + c - (decimal)xy.Y) < 0.001m; // need to cope with 3x 1/3 = 0.99999 etc
        }
    }
}
