using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    public class XY : IEquatable<XY>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public XY(int x, int y)
        {
            X = x;
            Y = y;
        }

        public XY(XY xy) : this(xy.X, xy.Y)
        {
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", X, Y);
        }

        public override bool Equals(object obj)
        {
            if (obj as XY == null) return base.Equals(obj);
            XY objXY = (XY)obj;
            return Equals(objXY);
        }

        public bool Equals(XY objXY)
        {
            return (objXY.X == X && objXY.Y == Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        //       +1
        //        |
        // -1 ----0---- +1
        //        |
        //       -1
        public XY MoveN()
        {
            return new XY(X, Y - 1);
        }

        public XY MoveS()
        {
            return new XY(X, Y + 1);
        }

        public XY MoveE()
        {
            return new XY(X + 1, Y);
        }

        public XY MoveW()
        {
            return new XY(X - 1, Y);
        }

        public XY Move(Direction dir)
        {
            switch (dir)
            {
                case Direction.North:
                    return MoveN();
                case Direction.East:
                    return MoveE();
                case Direction.South:
                    return MoveS();
                case Direction.West:
                    return MoveW();
                default:
                    throw new ArgumentOutOfRangeException("Direction");
            }
        }

        public IEnumerable<XY> GetSurroundingCoords()
        {
            var xys = new List<XY>();
            for (int sy = -1; sy <= 1; sy++)
            {
                for (int sx = -1; sx <= 1; sx++)
                {
                    if (sx == 0 && sy == 0) continue;
                    xys.Add(new XY(X + sx, Y + sy));
                }
            }
            return xys;
        }

        public int GetManhattanDistanceTo(XY other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public static Dictionary<Direction, char> DirToChar = new Dictionary<Direction, char>()
        {
            { Direction.North,'^'},
            { Direction.East,'>' },
            { Direction.South,'v'},
            { Direction.West,'<' }
        };

        public static Dictionary<char, Direction> CharToDir = new Dictionary<char, Direction>()
        {
            { '^',Direction.North},
            { '>',Direction.East},
            { 'v',Direction.South},
            { '<',Direction.West },
            { 'N',Direction.North},
            { 'E',Direction.East},
            { 'S',Direction.South},
            { 'W',Direction.West },
            { 'n',Direction.North},
            { 'e',Direction.East},
            { 's',Direction.South},
            { 'w',Direction.West },
            { 'U',Direction.North},
            { 'R',Direction.East},
            { 'D',Direction.South},
            { 'L',Direction.West },
            { 'u',Direction.North},
            { 'r',Direction.East},
            { 'd',Direction.South},
            { 'l',Direction.West },
        };
    }

    public enum Direction
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }

    public static class DirectionExtensions
    {
        public static Direction TurnRight(this Direction direction)
        {
            return (Direction)(((int)direction + 1 + 4) % 4);
        }

        public static Direction TurnLeft(this Direction direction)
        {
            return (Direction)(((int)direction - 1 + 4) % 4);
        }

    }
}
