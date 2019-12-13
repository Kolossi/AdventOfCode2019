using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace Runner
{
    class Day12 : Day
    {
        public override string First(string input)
        {
            var lines = input.GetLines();
            var count = int.Parse(lines[0]);
            lines = lines.Skip(1).ToArray();
            var moons = Moon.GetMoons(lines);
            var result = Iterate(moons.Values.ToList(), count);
            return GetEnergy(result).ToString();
        }

        public override string Second(string input)
        {
            var lines = input.GetLines();
            var count = int.Parse(lines[0]);
            lines = lines.Skip(1).ToArray();
            var moons = Moon.GetMoons(lines);
            var moonMovements = GetMoonMovements(moons.Values.ToList(), count);

            var moonLoopLengths = new List<long>();


            LogEnabled = true;
            var velocityPeriods = new List<long>();
            velocityPeriods.Add(GetLoopLengthForAxis(moonMovements, xyz => xyz.X));
            velocityPeriods.Add(GetLoopLengthForAxis(moonMovements, xyz => xyz.Y));
            velocityPeriods.Add(GetLoopLengthForAxis(moonMovements, xyz => xyz.Z));

            LogLine("velocity periods: {0}", velocityPeriods);
            var loopResult = LowestCommonMultiple(velocityPeriods);

            return loopResult.ToString();
        }

        ////////////////////////////////////////////////////////

        public long GetLoopLengthForAxis(Dictionary<int, List<Moon>> moonMovements, Func<XYZ,long> AxisAccessor)
        {
            var movementCount = moonMovements.Values.First().Count();
            for (int i = 1; i < movementCount; i++)
            {
                bool match = true;
                foreach (var moonId in moonMovements.Keys)
                {
                    var moon = moonMovements[moonId][i];
                    var initialMoon = moonMovements[moonId].First();
                    if (AxisAccessor(moon.Velocity) != AxisAccessor(initialMoon.Velocity) || AxisAccessor(moon.Position) != AxisAccessor(initialMoon.Position))
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return i;
            }
            throw new InvalidOperationException();
        }
       
        public class Moon
        {
            public XYZ Position;
            public XYZ Velocity;
            public int Id;
            private static int NEXTID = 1;

            public Moon()
            {
                Id = NEXTID++;
            }

            public Moon(Moon other)
            {
                Id = other.Id;
                Position = new XYZ(other.Position);
                Velocity = new XYZ(other.Velocity);
            }
            public override string ToString()
            {
                return string.Format("{0}:pos={1}, vel={2}", Id, Position, Velocity);

            }

            public bool IsStationary()
            {
                return (Velocity.X == 0 && Velocity.Y == 0 && Velocity.Z == 0);
            }

            public void AddGravityToVelocity(List<Moon> moons)
            {
                Velocity = new XYZ()
                {
                    X = this.Velocity.X + moons.Where(m => m != this).Sum(m => GetGravity(m.Position.X, this.Position.X)),
                    Y = this.Velocity.Y + moons.Where(m => m != this).Sum(m => GetGravity(m.Position.Y, this.Position.Y)),
                    Z = this.Velocity.Z + moons.Where(m => m != this).Sum(m => GetGravity(m.Position.Z, this.Position.Z))
                };
            }

            private static long GetGravity(long left, long right)
            {
                return left > right ? 1 : left < right ? -1 : 0;
            }

            public void Move()
            {
                this.Position = new XYZ()
                {
                    X = this.Position.X + this.Velocity.X,
                    Y = this.Position.Y + this.Velocity.Y,
                    Z = this.Position.Z + this.Velocity.Z,
                };
            }

            public static Dictionary<int, Moon> GetMoons(string[] lines)
            {
                NEXTID = 0;
                var moons = new Dictionary<int, Moon>();
                foreach (var line in lines)
                {
                    var parts = line.GetParts("<>=xyz,");
                    var moon = new Moon()
                    {
                        Position = new XYZ()
                        {
                            X = long.Parse(parts[0]),
                            Y = long.Parse(parts[1]),
                            Z = long.Parse(parts[2])
                        },
                        Velocity = new XYZ() { X = 0, Y = 0, Z = 0 }
                    };
                    moons[moon.Id]=moon;
                }

                return moons;
            }
        }

        List<Moon> Iterate(List<Moon> moons, int count)
        {
            var result = new List<Moon>();
            foreach (var moon in moons)
            {
                result.Add(new Moon()
                {
                    Id = moon.Id,
                    Position = moon.Position,
                    Velocity = moon.Velocity
                });
            }
                
            for (int i = 0; i < count; i++)
            {
                IterateOne(result);
            }
            return result;
        }

        private static void IterateOne(List<Moon> moons)
        {
            foreach (var moon in moons)
            {
                moon.AddGravityToVelocity(moons);
            }
            foreach (var moon in moons)
            {
                moon.Move();
            }
        }

        Dictionary<int, List<Moon>> GetMoonMovements(List<Moon> moons, int count)
        {
            var result = new Dictionary<int, List<Moon>>();
            foreach (var moon in moons)
            {
                result[moon.Id] = new List<Moon>()
                {
                    new Moon()
                    {
                        Id = moon.Id,
                        Position = moon.Position,
                        Velocity = moon.Velocity
                    }
                };
            }
            for (int i = 0; i < count; i++)
            {
                IterateOne(moons);
                foreach (var moon in moons)
                {
                    result[moon.Id].Add(new Moon(moon));
                }
            }
            return result;
        }

        long GetEnergy(IEnumerable<Moon> moons)
        {
            return moons.Sum(m => GetEnergy(m));
        }

        private static long GetEnergy(Moon moon)
        {
            return (Math.Abs(moon.Position.X) + Math.Abs(moon.Position.Y) + Math.Abs(moon.Position.Z))
                    * (Math.Abs(moon.Velocity.X) + Math.Abs(moon.Velocity.Y) + Math.Abs(moon.Velocity.Z));
        }


        // thanks to t'internet
        static long GreatestCommonFactor(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static long LowestCommonMultiple(long a, long b)
        {
            return (a / GreatestCommonFactor(a, b)) * b;
        }

        static long LowestCommonMultiple(ICollection<long> items)
        {
            var itemsQueue = new Queue<long>(items);
            var result = itemsQueue.Dequeue();
            while (itemsQueue.Any())
            {
                result = LowestCommonMultiple(result, itemsQueue.Dequeue());
            }
            return result;
        }

        //// Some proper desperation: (didn't help)
        //private void Animate(Dictionary<int, List<Moon>> moonMovements)
        //{
        //    var minCoord = moonMovements.SelectMany(kv => kv.Value).Select(m => m.Position).GetMinCoord();
        //    var coordRange = moonMovements.SelectMany(kv => kv.Value).Select(m => m.Position).GetCoordRange();

        //    var numMovements = moonMovements.Values.First().Count();
        //    Console.CursorVisible = false;
        //    try
        //    {
        //        for (int frame = 0; frame < numMovements; frame++)
        //        {
        //            Console.WriteLine(string.Format("{0}/{1}", frame, numMovements));
        //            var frameData = new Dictionary<int, XYZ>();
        //            foreach (var id in moonMovements.Keys)
        //            {
        //                frameData[id] = moonMovements[id][frame].Position;
        //            }
        //            for (long y = minCoord.Y; y < coordRange.Y; y++)
        //            {
        //                for (long x = minCoord.X; x < coordRange.X; x++)
        //                {
        //                    char c = ' ';
        //                    foreach (var kv in frameData)
        //                    {
        //                        if (kv.Value.X == x && kv.Value.Y == y)
        //                        {
        //                            c = kv.Key.ToString()[0];
        //                        }
        //                    }
        //                    Console.Write(c);
        //                }
        //                Console.Write(" | ");
        //                for (long z = minCoord.Z; z < coordRange.Z; z++)
        //                {
        //                    char c = ' ';
        //                    foreach (var kv in frameData)
        //                    {
        //                        if (kv.Value.Z == z && kv.Value.Y == y)
        //                        {
        //                            c = kv.Key.ToString()[0];
        //                        }
        //                    }
        //                    Console.Write(c);
        //                }
        //                Console.WriteLine();
        //            }
        //            //System.Threading.Thread.Sleep(100);
        //            for (long y = minCoord.Y; y < coordRange.Y + 1; y++)
        //            {
        //                Console.CursorTop--;
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        Console.CursorVisible = true;
        //    }
        //}

    }
}
