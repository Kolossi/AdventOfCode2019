using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    public class Map<T>
    {
        public Dictionary<int, Dictionary<int, T>> Data;
        public int Count;

        public Map()
        {
            Count = 0;
            Data = new Dictionary<int, Dictionary<int, T>>();
        }

        public Map(IEnumerable<XY> coords)
        {
            Count = 0;
            Data = new Dictionary<int, Dictionary<int, T>>();
            foreach (var coord in coords)
            {
                Set(coord);
            }
        }

        public Map(Map<T> sourceMap)
        {
            Count = sourceMap.Count;
            Data = new Dictionary<int, Dictionary<int, T>>();
            foreach (var key in sourceMap.Data.Keys)
            {
                Data[key] = new Dictionary<int, T>(sourceMap.Data[key]);
            }
        }

        public IEnumerable<XY> GetAllCoords()
        {
            if (!Data.Any()) return Enumerable.Empty<XY>();
            return Data.Keys.SelectMany(y => Data[y].Keys.Select(x => new XY(x, y)));
        }

        public int GetMinX()
        {
            return Data.Keys.SelectMany(y => Data[y].Keys).Min();
        }

        public int GetMaxX()
        {
            return Data.Keys.SelectMany(y => Data[y].Keys).Max();
        }

        public int GetMinY()
        {
            return Data.Keys.Min();
        }

        public int GetMaxY()
        {
            return Data.Keys.Max();
        }

        public XY GetMinPos()
        {
            return new XY(GetMinX(), GetMinY());
        }

        public XY GetMaxPos()
        {
            return new XY(GetMaxX(), GetMaxY());
        }

        public IEnumerable<T> GetAllValues()
        {
            return Data.SelectMany(i => i.Value).Select(i => i.Value);
        }

        public bool Has(XY xy)
        {
            return Has(xy.X, xy.Y);
        }

        public bool Has(int x, int y)
        {
            Dictionary<int, T> yDict;
            if (!Data.TryGetValue(y, out yDict)) return false;
            return yDict.ContainsKey(x);
        }

        public T Get(XY xy)
        {
            return Get(xy.X, xy.Y);
        }

        public T Get(int x, int y)
        {
            Dictionary<int, T> yDict;
            if (!Data.TryGetValue(y, out yDict)) return default(T);
            return yDict[x];
        }

        public bool TryGetValue(XY xy, out T value)
        {
            return TryGetValue(xy.X, xy.Y, out value);
        }

        public bool TryGetValue(int x, int y, out T value)
        {
            Dictionary<int, T> yDict;
            if (!Data.TryGetValue(y, out yDict))
            {
                value = default(T);
                return false;
            }
            return yDict.TryGetValue(x, out value);
        }

        public bool Remove(XY xy)
        {
            Dictionary<int, T> yDict;
            if (!Data.TryGetValue(xy.Y, out yDict))
            {
                return false;
            }
            if (yDict.Remove(xy.X))
            {
                Count--;
                return true;
            }
            return false;
        }

        public void Set(XY xy)
        {
            Set(xy.X, xy.Y);
        }

        public void Set(int x, int y)
        {
            Set(x, y, default(T));
        }

        public void Set(XY xy, T value)
        {
            Set(xy.X, xy.Y, value);
        }

        public void Set(int x, int y, T value)
        {
            Dictionary<int, T> yDict;
            if (!Data.TryGetValue(y, out yDict))
            {
                yDict = new Dictionary<int, T>();
                Data[y] = yDict;
            }
            if (!yDict.ContainsKey(x)) Count++;
            yDict[x] = value;
        }

        public string GetStateString(Dictionary<T, char> valueMap)
        {
            var sb = new StringBuilder();
            var minPos = GetMinPos();
            var maxPos = GetMaxPos();
            sb.AppendFormat("{0}->{1}", minPos, maxPos).AppendLine();
            for (int y = minPos.Y; y <= maxPos.Y; y++)
            {
                for (int x = minPos.X; x <= maxPos.X; x++)
                {
                    T value;
                    TryGetValue(x, y, out value);
                    sb.Append(valueMap[value]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

    }
}
