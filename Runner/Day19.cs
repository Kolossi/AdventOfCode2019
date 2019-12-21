using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day19 : Day
    {
        public override string First(string input)
        {
            long[] data = input.GetParts(",").Select(p => long.Parse(p)).ToArray();
            var map = GetMap(data, 50, 50);
            var result = map.GetAllValues().Count(c => c == '#');
            LogEnabled = true;
            LogLine(map.GetStateString(VALUEMAP));
            return result.ToString();
        }

        public override string Second(string input)
        {
            long[] data = input.GetParts(",").Select(p => long.Parse(p)).ToArray();
            long initialY = (50 / 7) * 90;
            var xy = FindXY(data, initialY);
            return (xy.X*10000+xy.Y).ToString();
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

        Dictionary<char, char> VALUEMAP = new Dictionary<char, char>()
        {
            {'.','.' },
            {'#','#' }
        };

        public Map<char> GetMap(long[] data, long xsize, long ysize)
        {
            var intcode = new Intcode(data);
            var map = new Map<char>();
            for (int y = 0; y < ysize; y++)
            {
                for (int x = 0; x < xsize; x++)
                {
                    //map.Set(x, y, IsBeam(intcode, x, y) ? '#': '.');
                    map.Set(x, y, IsBeam(data, x, y) ? '#': '.');
                }
            }
            return map;
        }

        private XY FindXY(long[] data, long initialY)
        {
            var resulty = initialY;
            long resultx = 0;
            bool found = false;
            var topEndx = resulty;
            do
            {
                topEndx = FindBeam(data, topEndx, resulty, -1);
                resultx = topEndx - 99;
                if (IsBeam(data, resultx, resulty) && IsBeam(data, topEndx, resulty) && IsBeam(data, resultx, resulty + 99)) break;
                resulty++;
                topEndx += 2;
            } while (!found);
            return new XY((int)resultx, (int)resulty);
        }

        private long FindBeam(long[]data, long x, long y, long increment)
        {
            bool isBeam = false;
            do
            {
                x += increment;
                isBeam = IsBeam(data, x, y);
            } while (!isBeam);
            return x;
        }

        public bool IsBeam(long[] data, long x, long y)
        {
            var intcode = new Intcode(data);
            intcode.InputQueue.Enqueue(x);
            intcode.InputQueue.Enqueue(y);
            intcode.Resume();
            return intcode.OutputQueue.Dequeue() == 1;
        }
    }
}
