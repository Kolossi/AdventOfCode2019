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
            throw new NotImplementedException("Second");
        }

        //public override string FirstTest(string input)
        //{
        //    throw new NotImplementedException("FirstTest");
        //}

        public override string SecondTest(string input)
        {
            LogLine("3 Result: {0}", DetectLoop(new long[] { 1, 5, 7, 3, 6, 8, 2, 9, 3, 6, 8 },3));
            LogLine("2 Result: {0}", DetectLoop(new long[] { 1, 5, 7, 3, 6, 8, 2, 9, 3, 6, 8 },2));
            var lines = input.GetLines();
            var count = int.Parse(lines[0]);
            lines = lines.Skip(1).ToArray();
            var moons = Moon.GetMoons(lines);
            var moonMovements = GetMoonMovements(moons.Values.ToList(), count);

            var moonLoopLengths = new List<long>();

            LogEnabled = true;
            var velocityPeriods = new List<long>();
            foreach (var moonId in moons.Keys)
            {
                //LogLine("Moon {0}:{1}",moon.Id,Enumerable.Range(0,count).Select(i => GetEnergy(moonMovements[moon.Id][i])));
                //var moonEnergies = Enumerable.Range(0, count).Select(i => GetEnergy(moonMovements[moon.Id][i])).ToArray();
                //var zeroEnergyIndexes = Enumerable.Range(0, count).Where(i => moonEnergies[i] == 0).ToArray();
                //moonLoopLengths.Add(zeroEnergyIndexes[1]);
                ////LogLine("moon {0}:{1}", moon.Id, moonEnergies);
                //LogLine("moon {0}: 0 energy indexes {1}", moon.Id, zeroEnergyIndexes);
                //LogLine("Moon {0}: Loop length: {1}", moon.Id, zeroEnergyIndexes[1]);
                //var moonEnergies = Enumerable.Range(0, count).Select(i => GetEnergy(moonMovements[moonId][i])).ToArray();
                //var moonStopIndexes = Enumerable.Range(0, count).Where(i => moonMovements[moonId][i].Equals(moons[moonId])).ToArray();
                //moonLoopLengths.Add(moonStopIndexes[1]);
                //LogLine("moon {0}:{1}", moon.Id, moonEnergies);
                //LogLine("moon {0}: 0 energy indexes {1}", moonId, moonStopIndexes);
                //LogLine("Moon {0}: Loop length: {1}", moonId, moonStopIndexes[1]);

                var xVelocityZeroPeriod = Enumerable.Range(0, count).Where(i => moonMovements[moonId][i].Velocity.X == 0).ToArray();
                var yVelocityZeroPeriod = Enumerable.Range(0, count).Where(i => moonMovements[moonId][i].Velocity.Y == 0).ToArray();
                var zVelocityZeroPeriod = Enumerable.Range(0, count).Where(i => moonMovements[moonId][i].Velocity.Z == 0).ToArray();

                var xVelocityPeriod = DetectLoop(moonMovements[moonId].Select(m => m.Velocity.X),2);
                var yVelocityPeriod = DetectLoop(moonMovements[moonId].Select(m => m.Velocity.Y),2);
                var zVelocityPeriod = DetectLoop(moonMovements[moonId].Select(m => m.Velocity.Z),2);

                //LogLine("Moon {0}: X velocity Period: {1}", moonId, xVelocityZeroPeriod);
                //LogLine("Moon {0}: Y velocity Period: {1}", moonId, yVelocityZeroPeriod);
                //LogLine("Moon {0}: Z velocity Period: {1}", moonId, zVelocityZeroPeriod);

                LogLine("Moon {0}: X velocity Period: {1}", moonId, xVelocityPeriod);
                LogLine("Moon {0}: Y velocity Period: {1}", moonId, yVelocityPeriod);
                LogLine("Moon {0}: Z velocity Period: {1}", moonId, zVelocityPeriod);
                velocityPeriods.Add(xVelocityPeriod.LoopLength);
                velocityPeriods.Add(yVelocityPeriod.LoopLength);
                velocityPeriods.Add(zVelocityPeriod.LoopLength);
            }

            //Log(Enumerable.Range(0,count).Select(i => GetEnergy(moonMovements.Values.Select(v => v[i]))));

            //Animate(moonMovements);
            //var lcmSoFar = moonLoopLengths[0];
            //for (int i = 1; i < moonLoopLengths.Count; i++)
            //{
            //    lcmSoFar = LowestCommonMultiple(lcmSoFar, moonLoopLengths[i]);
            //}
            //LogLine("moons loop length = {0}", lcmSoFar);
            //var loopResult = DetectLoop(moonMovements[moons[0].Id]);
            LogLine("velocity periods: {0}", velocityPeriods);
            var loopResult = LowestCommonMultiple(velocityPeriods);

            return loopResult.ToString();
        }

        ////////////////////////////////////////////////////////

        private void Animate(Dictionary<int, List<Moon>> moonMovements)
        {
            var minCoord = moonMovements.SelectMany(kv => kv.Value).Select(m => m.Position).GetMinCoord();
            var coordRange = moonMovements.SelectMany(kv => kv.Value).Select(m => m.Position).GetCoordRange();

            var numMovements = moonMovements.Values.First().Count();
            Console.CursorVisible = false;
            try
            {
                for (int frame = 0; frame < numMovements; frame++)
                {
                    Console.WriteLine(string.Format("{0}/{1}", frame, numMovements));
                    var frameData = new Dictionary<int, XYZ>();
                    foreach (var id in moonMovements.Keys)
                    {
                        frameData[id] = moonMovements[id][frame].Position;
                    }
                    for (long y = minCoord.Y; y < coordRange.Y; y++)
                    {
                        for (long x = minCoord.X; x < coordRange.X; x++)
                        {
                            char c = ' ';
                            foreach (var kv in frameData)
                            {
                                if (kv.Value.X == x && kv.Value.Y == y)
                                {
                                    c = kv.Key.ToString()[0];
                                }
                            }
                            Console.Write(c);
                        }
                        Console.Write(" | ");
                        for (long z = minCoord.Z; z < coordRange.Z; z++)
                        {
                            char c = ' ';
                            foreach (var kv in frameData)
                            {
                                if (kv.Value.Z == z && kv.Value.Y == y)
                                {
                                    c = kv.Key.ToString()[0];
                                }
                            }
                            Console.Write(c);
                        }
                        Console.WriteLine();
                    }
                    //System.Threading.Thread.Sleep(100);
                    for (long y = minCoord.Y; y < coordRange.Y + 1; y++)
                    {
                        Console.CursorTop--;
                    }
                }
            }
            finally
            {
                Console.CursorVisible = true;
            }
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

        static long GreatestCommonFactor(long a, long b)
        {
            long temp;
            if (a>b)
            {
                temp = a;
                a = b;
                b = temp;
            }
            if (b%a==0)
                {
                return a;
            }
            else
            {
                return GreatestCommonFactor(b % a, a);
            }
            //while (b != 0)
            //{
            //    long temp = b;
            //    b = a % b;
            //    a = temp;
            //}
            //return a;
        }

        static long LowestCommonMultiple(long a, long b)
        {
            return (a / GreatestCommonFactor(a, b)) * b;
        }

        static long LowestCommonMultiple(IList<long> items)
        {
            var lcm = items[0];
            for (int i = 1; i < items.Count; i++)
            {
                lcm = LowestCommonMultiple(lcm, items[i]);
            }

            return lcm;
        }
    }
}
