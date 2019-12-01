using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day01 :  Day
    {
        public override string First(string input)
        {
            return input.GetLines().Select(i => GetFuel(int.Parse(i))).Sum().ToString();
        }

        public override string Second(string input)
        {
            return input.GetLines().Select(i => GetTotalFuel(int.Parse(i))).Sum().ToString();
        }

        public override string FirstTest(string input)
        {
            return GetFuel(int.Parse(input)).ToString();
        }

        public override string SecondTest(string input)
        {
            return GetTotalFuel(int.Parse(input)).ToString();
        }

        int GetTotalFuel(int mass)
        {
            int total = 0;
            var fuel = GetFuel(mass);
            total += fuel;
            while (fuel>0)
            {
                fuel = GetFuel(fuel);
                total += fuel;
            }
            return total;
        }
        int GetFuel(int mass)
        {
            return Math.Max(0,(int)Math.Floor(((double)mass) / 3.0) - 2);
        }

        ////////////////////////////////////////////////////////
    }
}
