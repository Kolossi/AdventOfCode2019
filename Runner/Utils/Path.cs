using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    public class Path
    {
        public XY XY;
        public LinkedList<XY> Points = new LinkedList<XY>();
        public Map<bool> Visited = new Map<bool>();
        public int Length = -1;

        public Path()
        {
        }

        public Path(Path sourcePath)
        {
            XY = new XY(sourcePath.XY.X, sourcePath.XY.Y);
            Points = new LinkedList<XY>(sourcePath.Points);
            Visited = new Map<bool>(sourcePath.Visited);
            Length = sourcePath.Length;
        }

        public Path Move(XY xy)
        {
            XY = xy;
            Visited.Set(xy);
            Points.AddLast(xy);
            Length++;
            return this;
        }

        public Path Backup()
        {
            if (Length == 0) throw new InvalidOperationException();
            XY = Points.Last.Previous.Value;
            Visited.Remove(XY);
            Points.RemoveLast();
            Length--;
            return this;
        }

        public override string ToString()
        {
            return string.Format("({0}):{1}", Length, string.Join(",", Points));
        }
    }
}
