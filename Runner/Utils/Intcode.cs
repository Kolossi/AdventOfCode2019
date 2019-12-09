using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    public class Intcode
    {
        // days 2,5,7,9

        public Queue<long> InputQueue = new Queue<long>();
        public Queue<long> OutputQueue = new Queue<long>();
        public long? Noun;
        public long? Verb;
        public bool Halt = false;
        public long Ptr;
        public long[] Data;
        public Dictionary<long, long> ExtendedRam = new Dictionary<long, long>();
        public long RelativeBase = 0;
        public bool AwaitingInput = false;

        private const int MAXPARAMS = 3;

        public Intcode()
        {
        }

        public Intcode(long initialInput, int[] data)
        {
            Data = data.Select(i=>(long)i).ToArray();
            InputQueue.Enqueue(initialInput);
        }

        public Intcode(long initialInput, long[] data)
        {
            Data = CloneData(data);
            InputQueue.Enqueue(initialInput);
        }

        public static int[] CloneData(int[] data)
        {
            int[] clonedData = new int[data.Length];
            Array.Copy(data, clonedData, data.Length);
            return clonedData;
        }

        public static long[] CloneData(long[] data)
        {
            long[] clonedData = new long[data.Length];
            Array.Copy(data, clonedData, data.Length);
            return clonedData;
        }

        public long[] Execute(long[] data)
        {
            if (Noun.HasValue) data[1] = Noun.Value;
            if (Verb.HasValue) data[2] = Verb.Value;
            ExtendedRam = new Dictionary<long, long>();
            Data = data;
            Ptr = 0;
            return Resume();
        }

        public int[] Execute(int[] data)
        {
            var result = Execute(data.Select(i => (long)i).ToArray());
            return result.Select(i => (int)i).ToArray();
        }

        public long[] Resume()
        {
            AwaitingInput = false;
            while (true)
            {
                var instructionBlock = GetInstructionBlock(Data, Ptr);
                Day.Log(Ptr);
                Day.Log(":");
                Day.LogLine(instructionBlock);
                Day.LogLine(Data);
                Data = instructionBlock.OpDef.Function(this, Data, instructionBlock.Parameters);
                if (instructionBlock.OpDef.AutoUpdatePtr) Ptr += instructionBlock.OpDef.NumParams + 1;
                if (Ptr >= Data.Length) throw new IndexOutOfRangeException("ptr past end of Data");
                if (this.Halt|| this.AwaitingInput) break;
            }
            return Data;
        }

        //public void EnqueueInput(long input)
        //{
        //    InputQueue.Enqueue(input);
        //}

        //public long DequeueOutput()
        //{
        //    return OutputQueue.Dequeue();
        //}

        //public long[] Execute(long[] data)
        //{
        //    if (Noun.HasValue) data[1] = Noun.Value;
        //    if (Verb.HasValue) data[2] = Verb.Value;
        //    this.Data = data;
        //    Ptr = 0;
        //    while (true)
        //    {
        //        var instructionBlock = GetInstructionBlock(data, Ptr);
        //        Day.Log(Ptr);
        //        Day.Log(":");
        //        Day.LogLine(instructionBlock);
        //        Day.LogLine(data);
        //        data = instructionBlock.OpDef.Function(this, data, instructionBlock.Parameters);
        //        if (instructionBlock.OpDef.AutoUpdatePtr) Ptr += instructionBlock.OpDef.NumParams + 1;
        //        if (Ptr >= data.Length) throw new IndexOutOfRangeException("ptr past end of data");
        //        if (this.Halt) break;
        //    }
        //    return data;
        //}

        private static InstructionBlock GetInstructionBlock(long[] data, long ptr)
        {
            string command = data[ptr].ToString().PadLeft(MAXPARAMS + 2, '0');
            var opcode = int.Parse(command.Substring(command.Length - 2, 2));
            var opdef = OpDef.OPDEFS[opcode];
            var parameters = new Parameter[opdef.NumParams];
            for (int i = 0; i < opdef.NumParams; i++)
            {
                var parameter = new Parameter()
                {
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

        public long ReadAddress(long address)
        {
            if (address < 0) throw new InvalidOperationException("negative address");
            if (address < Data.LongLength) return Data[address];
            if (ExtendedRam.TryGetValue(address, out long value)) return value;
            return 0;
        }

        public void Store(long[] data, long value, Parameter location)
        {
            long address = location.GetPositionalAddress(this);
            if (address < 0) throw new InvalidOperationException("negative address");
            if (address < Data.LongLength) Data[address] = value;
            ExtendedRam[address] = value;
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
            public Func<Intcode, long[], Parameter[], long[]> Function;
            public bool AutoUpdatePtr = true;

            public override string ToString()
            {
                return string.Format("{0}({1})={2}:{3}", Opcode, NumParams, Function.Method.Name, AutoUpdatePtr);
            }

            public static long[] Add(Intcode intcode, long[] data, Parameter[] parameters)
            {
                long result = parameters[0].GetValue(intcode) + parameters[1].GetValue(intcode);
                intcode.Store(data, result, parameters[2]);
                return data;
            }

            public static long[] Multiply(Intcode intcode, long[] data, Parameter[] parameters)
            {
                long result = parameters[0].GetValue(intcode) * parameters[1].GetValue(intcode);
                intcode.Store(data, result, parameters[2]);
                return data;
            }

            public static long[] StoreInput(Intcode intcode, long[] data, Parameter[] parameters)
            {
                if (!intcode.InputQueue.Any())
                {
                    intcode.AwaitingInput = true;
                    return data;
                }
                long value = intcode.InputQueue.Dequeue();
                intcode.Store(data, value, parameters[0]);
                intcode.Ptr += 2;
                return data;
            }

            public static long[] Output(Intcode intcode, long[] data, Parameter[] parameters)
            {
                intcode.OutputQueue.Enqueue(parameters[0].GetValue(intcode));
                return data;
            }

            public static long[] Halt(Intcode intcode, long[] data, Parameter[] parameters)
            {
                intcode.Halt = true;
                return data;
            }

            public static long[] JumpIfTrue(Intcode intcode, long[] data, Parameter[] parameters)
            {
                if (parameters[0].GetValue(intcode) != 0)
                {
                    intcode.Ptr = parameters[1].GetValue(intcode);
                }
                else
                {
                    intcode.Ptr += 3;
                }
                return data;
            }

            public static long[] JumpIfFalse(Intcode intcode, long[] data, Parameter[] parameters)
            {
                if (parameters[0].GetValue(intcode) == 0)
                {
                    intcode.Ptr = parameters[1].GetValue(intcode);
                }
                else
                {
                    intcode.Ptr += 3;
                }
                return data;
            }

            public static long[] LessThan(Intcode intcode, long[] data, Parameter[] parameters)
            {
                intcode.Store(data,
                    (parameters[0].GetValue(intcode) < parameters[1].GetValue(intcode)) ? 1 : 0,
                    parameters[2]);
                return data;
            }

            public static long[] Equals(Intcode intcode, long[] data, Parameter[] parameters)
            {
                intcode.Store(data,
                    (parameters[0].GetValue(intcode) == parameters[1].GetValue(intcode)) ? 1 : 0,
                    parameters[2]);
                return data;
            }

            public static long[] AdjustRelativeOffset(Intcode intcode, long[] data, Parameter[] parameters)
            {
                intcode.RelativeBase += parameters[0].GetValue(intcode);
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
                     Function = OpDef.StoreInput,
                     AutoUpdatePtr = false
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
            { 9, new OpDef() {
                     Opcode = 9,
                     NumParams = 1,
                     Function = OpDef.AdjustRelativeOffset
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
            Immediate = 1,
            Relative = 2

        }

        public class Parameter
        {
            public long Token;
            public ParamMode Mode;

            public long GetValue(Intcode intcode)
            {
                if (Mode == ParamMode.Immediate) return Token;
                return intcode.ReadAddress(GetPositionalAddress(intcode));
            }

            public long GetPositionalAddress(Intcode intcode)
            {
                if (Mode == ParamMode.Immediate) throw new InvalidOperationException();
                if (Mode == ParamMode.Relative) return Token + intcode.RelativeBase;
                return Token;
            }

            public override string ToString()
            {
                return string.Format("{0}{1}", Mode.ToString("G")[0], Token);
            }
        }
    }
}
