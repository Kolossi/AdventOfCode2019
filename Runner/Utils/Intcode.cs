using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    public class Intcode
    {
        // days 2,5

        public Queue<int> InputQueue = new Queue<int>();
        public Queue<int> OutputQueue = new Queue<int>();
        public int? Noun;
        public int? Verb;
        public bool Halt = false;
        public int Ptr;

        private const int MAXPARAMS = 3;

        public static int[] CloneData(int[] data)
        {
            int[] clonedData = new int[data.Length];
            Array.Copy(data, clonedData, data.Length);
            return clonedData;
        }

        public int[] Execute(int[] data)
        {
            if (Noun.HasValue) data[1] = Noun.Value;
            if (Verb.HasValue) data[2] = Verb.Value;
            Ptr = 0;
            while (true)
            {
                var instructionBlock = GetInstructionBlock(data, Ptr);
                Day.Log(Ptr);
                Day.Log(":");
                Day.LogLine(instructionBlock);
                Day.LogLine(data);
                data = instructionBlock.OpDef.Function(this, data, instructionBlock.Parameters);
                if (instructionBlock.OpDef.AutoUpdatePtr) Ptr += instructionBlock.OpDef.NumParams + 1;
                if (Ptr >= data.Length) throw new IndexOutOfRangeException("ptr past end of data");
                if (this.Halt) break;
            }
            return data;
        }

        private static InstructionBlock GetInstructionBlock(int[] data, int ptr)
        {
            string command = data[ptr].ToString().PadLeft(MAXPARAMS + 2, '0');
            var opcode = int.Parse(command.Substring(command.Length - 2, 2));
            var opdef = OpDef.OPDEFS[opcode];
            var parameters = new Parameter[opdef.NumParams];
            for (int i = 0; i < opdef.NumParams; i++)
            {
                var parameter = new Parameter() {
                    Token = data[ptr + 1 + i],
                    Mode = (ParamMode)int.Parse(command.Substring(command.Length - 3 - i, 1))
                };
                parameters[i] = parameter;
            }
            return new InstructionBlock()
            {
                OpDef = opdef,
                Parameters = parameters
            };
        }

        public static int[] Store(int[] data, int value, Parameter location)
        {
            data[location.GetPositionalAddress()] = value;
            return data;
        }
    }


    public class InstructionBlock
    {
        public OpDef OpDef;
        public Parameter[] Parameters;
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}|", OpDef);
            sb.AppendJoin(":", Parameters.Select(p => p.ToString()));
            return sb.ToString();
        }
    }

    public class OpDef
    {
        public int Opcode;
        public int NumParams;
        public Func<Intcode, int[], Parameter[], int[]> Function;
        public bool AutoUpdatePtr = true;

        public override string ToString()
        {
            return string.Format("{0}({1})={2}:{3}", Opcode, NumParams, Function.Method.Name, AutoUpdatePtr);
        }

        public static int[] Add(Intcode intcode, int[] data, Parameter[] parameters)
        {
            int result = parameters[0].GetValue(data) + parameters[1].GetValue(data);
            Intcode.Store(data, result, parameters[2]);
            return data;
        }

        public static int[] Multiply(Intcode intcode, int[] data, Parameter[] parameters)
        {
            int result = parameters[0].GetValue(data) * parameters[1].GetValue(data);
            Intcode.Store(data, result, parameters[2]);
            return data;
        }

        public static int[] StoreInput(Intcode intcode, int[] data, Parameter[] parameters)
        {
            int value = intcode.InputQueue.Dequeue();
            Intcode.Store(data, value, parameters[0]);
            return data;
        }

        public static int[] Output(Intcode intcode, int[] data, Parameter[] parameters)
        {
            intcode.OutputQueue.Enqueue(parameters[0].GetValue(data));
            return data;
        }

        public static int[] Halt(Intcode intcode, int[] data, Parameter[] parameters)
        {
            intcode.Halt = true;
            return data;
        }

        public static int[] JumpIfTrue(Intcode intcode, int[] data, Parameter[] parameters)
        {
            if (parameters[0].GetValue(data) != 0)
            {
                intcode.Ptr = parameters[1].GetValue(data);
            }
            else
            {
                intcode.Ptr += 3;
            }
            return data;
        }

        public static int[] JumpIfFalse(Intcode intcode, int[] data, Parameter[] parameters)
        {
            if (parameters[0].GetValue(data) == 0)
            {
                intcode.Ptr = parameters[1].GetValue(data);
            }
            else
            {
                intcode.Ptr += 3;
            }
            return data;
        }

        public static int[] LessThan(Intcode intcode, int[] data, Parameter[] parameters)
        {
            data[parameters[2].GetPositionalAddress()] = (parameters[0].GetValue(data) < parameters[1].GetValue(data)) ? 1 : 0;
            return data;
        }

        public static int[] Equals(Intcode intcode, int[] data, Parameter[] parameters)
        {
            data[parameters[2].GetPositionalAddress()] = (parameters[0].GetValue(data) == parameters[1].GetValue(data)) ? 1 : 0;
            return data;
        }

        public static Dictionary<int, OpDef> OPDEFS = new Dictionary<int, OpDef>()
        {
            { 1, new OpDef() {
                     Opcode=1,
                     NumParams =3,
                     Function = OpDef.Add
                 }
            },
            { 2, new OpDef() {
                     Opcode = 2,
                     NumParams = 3,
                     Function = OpDef.Multiply
                 }
            },
            { 3, new OpDef() {
                     Opcode = 3,
                     NumParams = 1,
                     Function = OpDef.StoreInput
                 }
            },
            { 4, new OpDef() {
                     Opcode = 4,
                     NumParams = 1,
                     Function = OpDef.Output
                 }
            },
            { 5, new OpDef() {
                     Opcode = 5,
                     NumParams = 2,
                     Function = OpDef.JumpIfTrue,
                     AutoUpdatePtr = false
                 }
            },
            { 6, new OpDef() {
                     Opcode = 6,
                     NumParams = 2,
                     Function = OpDef.JumpIfFalse,
                     AutoUpdatePtr = false
                 }
            },
            { 7, new OpDef() {
                     Opcode = 7,
                     NumParams = 3,
                     Function = OpDef.LessThan
                 }
            },
            { 8, new OpDef() {
                     Opcode = 8,
                     NumParams = 3,
                     Function = OpDef.Equals
                 }
            },
            { 99, new OpDef() {
                     Opcode = 99,
                     NumParams=0,
                     Function = OpDef.Halt,
                     AutoUpdatePtr = false
                 }
            }
        };
    }

    public enum ParamMode
    {
        Position = 0,
        Immediate = 1
    }

    public class Parameter
    {
        public int Token;
        public ParamMode Mode;

        public int GetValue(int[] data)
        {
            if (Mode == ParamMode.Immediate) return Token;
            return data[Token];
        }

        public int GetPositionalAddress()
        {
            if (Mode == ParamMode.Immediate) throw new InvalidOperationException();
            return Token;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", Mode == ParamMode.Immediate ? "I" : "P", Token);
        }
    }
}
