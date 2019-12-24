using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    public class XYZ
    {
        public long X;
        public long Y;
        public long Z;

        public XYZ()
        {
        }

        public XYZ(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public XYZ(XYZ right)
        {
            X = right.X;
            Y = right.Y;
            Z = right.Z;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2}]", X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var right = (XYZ)obj;

            return X == right.X && Y == right.Y && Z == right.Z;
        }

        public override int GetHashCode()
        {
            return (int)(X ^ Y ^ Z ^ 81);
        }
    }

    public static class XYZHelper
    {
        public static XYZ GetMinCoord(this IEnumerable<XYZ> coords)
        {
            return new XYZ()
            {
                X = coords.Min(c => c.X),
                Y = coords.Min(c => c.Y),
                Z = coords.Min(c => c.Z)
            };
        }

        public static XYZ GetMaxCoord(this IEnumerable<XYZ> coords)
        {
            return new XYZ()
            {
                X = coords.Max(c => c.X),
                Y = coords.Max(c => c.Y),
                Z = coords.Max(c => c.Z)
            };
        }

        public static XYZ GetCoordRange(this IEnumerable<XYZ> coords)
        {
            return new XYZ()
            {
                X = coords.Max(c => c.X) - coords.Min(c => c.X),
                Y = coords.Max(c => c.Y) - coords.Min(c => c.Y),
                Z = coords.Max(c => c.Z) - coords.Min(c => c.Z)
            };
        }

    }
}
