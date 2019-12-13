using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day13 :  Day
    {
        public override string First(string input)
        {
            var map = RunGame(input);
            return map.GetAllValues().Count(t => t == (int)TileType.Block).ToString();
        }

        public override string Second(string input)
        {
            var score = PlayGame(input);
            return score.ToString();
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
        
        public enum TileType
        {
            Empty = 0,
            Wall = 1,
            Block = 2,
            HorizontalPaddle = 3,
            Ball = 4
        }

        public static Dictionary<int, char> TILE_VALUE_MAP = new Dictionary<int, char>()
        {
            {0, ' ' },
            {1, 'W' },
            {2, 'B' },
            {3, '=' },
            {4, 'O' }
        };

        private static Map<int> RunGame(string input)
        {
            var intcode = new Intcode(input.GetParts(",").Select(p => long.Parse(p)).ToArray());

            var map = new Map<int>();

            while (!intcode.Halt)
            {
                intcode.Resume();
                while (intcode.OutputQueue.Count >= 3)
                {
                    var x = intcode.OutputQueue.Dequeue();
                    var y = intcode.OutputQueue.Dequeue();
                    var tile = intcode.OutputQueue.Dequeue();
                    map.Set((int)x, (int)y, (int)tile);
                }
            }
            return map;
        }

        private static long PlayGame(string input)
        {
            var intcode = new Intcode(input.GetParts(",").Select(p => long.Parse(p)).ToArray());
            intcode.Data[0] = 2;

            var map = new Map<int>();
            long score=-1;
            XY paddle = null;
            XY ball = null;

            while (!intcode.Halt)
            {
                intcode.Resume();
                while (intcode.OutputQueue.Count >= 3)
                {
                    var x = intcode.OutputQueue.Dequeue();
                    var y = intcode.OutputQueue.Dequeue();
                    var tile = intcode.OutputQueue.Dequeue();
                    if (x == -1 && y == 0)
                    {
                        score = tile;
                    }
                    else
                    {
                        map.Set((int)x, (int)y, (int)tile);
                        if (tile == (int)TileType.Ball) ball = new XY((int)x, (int)y);
                        if (tile == (int)TileType.HorizontalPaddle) paddle = new XY((int)x, (int)y);
                    }
                }

                if (ball != null && paddle != null)
                {
                    intcode.InputQueue.Clear();
                    if (ball.X > paddle.X) intcode.InputQueue.Enqueue(1);
                    else if (ball.X < paddle.X) intcode.InputQueue.Enqueue(-1);
                    else intcode.InputQueue.Enqueue(0);
                }

                LogLine("Score:{0}  Input:{1}", score, intcode.InputQueue.Any()?intcode.InputQueue.Peek().ToString():"-none-");
                LogLine(map.GetStateString(TILE_VALUE_MAP));
                if (LogEnabled) for (int i = 0; i < map.GetMaxY()-map.GetMinY()+4; i++) Console.CursorTop--;
            }
            return score;
        }
    }
}
