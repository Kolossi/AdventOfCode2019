using System;
using System.Collections.Generic;
using System.Text;

namespace Runner
{
    public class Intcode
    {
        public static int[] Rerun(int[] data, int noun, int verb)
        {
            int[] rerunData = new int[data.Length];
            Array.Copy(data, rerunData, data.Length);
            rerunData[1] = noun;
            rerunData[2] = verb;
            return Execute(rerunData);
        }

        public static int[] Execute(int[] data)
        {
            int ptr = 0;
            while (true)
            {
                int opcode = data[ptr];
                if (opcode == 99) break;
                if (opcode != 1 && opcode != 2) throw new InvalidOperationException("Unknown opcode");
                int a = data[data[ptr + 1]];
                int b = data[data[ptr + 2]];
                int storeAt = data[ptr + 3];
                ptr += 4;
                if (ptr >= data.Length) throw new IndexOutOfRangeException("ptr past end of data");
                if (opcode == 1)
                {
                    data[storeAt] = a + b;
                }
                else if (opcode == 2)
                {
                    data[storeAt] = a * b;
                }
            }
            return data;
        }
    }
}
