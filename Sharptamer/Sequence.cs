using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharptamer.Utils;

namespace Sharptamer
{
    class Sequence : IEnumerable<byte>, IComparable<Sequence>, ICloneable
    {
        //Global Structural Constants
        public const byte A = 0;
        public const byte C = 1;
        public const byte G = 2;
        public const byte T = 3;

        private static readonly int lengthIdx = 32;

        //Instance data
        private readonly ulong[] data;
        private readonly uint length;

        private int hashcode = 0;

        public Sequence(string seq)
        {
            if (seq.Length < 1)
            {
                throw new ArgumentException("Sequence must be at least one base long. Is: \"" + seq + "\"");
            }
            this.length = (uint)seq.Length;
            int dataLength = (int)Math.Ceiling(length / 4.0);
            data = new ulong[dataLength];
            int outer = 0;
            int inner = 0;
            for (int i = 0; i < seq.Length; i++)
            {
                ulong sym = toUint(seq[i]);
                sym <<= ((lengthIdx * 2 - 2) - (inner * 2));
                data[outer] |= sym;
                inner++;
                if (inner >= lengthIdx)
                {
                    inner = 0;
                    outer++;
                }
            }
        }


        public Sequence(FileStream file, byte[] b, ref int read, ref int pos, int maxRead)
        {

            this.length = 0;
            int dataLength = 10;
            data = new ulong[dataLength];
            int outer = 0;
            int inner = 0;
            bool finished = false;

            while (!finished && read > 0)
            {
                while (pos < read && !(finished = (b[pos] == '\n')))
                {
                    ulong sym = toUint((char)b[pos++]);
                    sym <<= ((lengthIdx * 2 - 2) - (inner * 2));
                    data[outer] |= sym;
                    inner++;
                    if (inner >= lengthIdx)
                    {
                        inner = 0;
                        outer++;
                        if (outer >= dataLength)
                        {
                            throw new System.Exception("Aptamer too long");
                        }
                    }
                }
                if (!finished)
                {
                    read = file.Read(b, 0, maxRead);
                    pos = 0;
                }
            }
            pos++;
        }

        private uint toUint(char c)
        {
            uint sym;
            switch (c)
            {
                case 'A':
                    sym = A;
                    break;
                case 'C':
                    sym = C;
                    break;
                case 'T':
                    sym = T;
                    break;
                case 'G':
                    sym = G;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Sequence contained a character not in ACTG: " + c);
            }
            return sym;
        }

        private Sequence(Sequence other)
        {
            this.data = new ulong[other.data.Length];
            this.length = other.length;
            for (int i = 0; i < this.data.Length; i++)
            {
                this.data[i] = other.data[i];
            }
        }

        public uint Length()
        {
            return this.length;
        }

        public IEnumerator<Byte> GetEnumerator()
        {
            return new SequenceEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public byte this[int i]
        {
            get
            {
                int outer = i / Sequence.lengthIdx;
                int inner = i % Sequence.lengthIdx;
                int offset = lengthIdx * 2 - 2 - inner * 2;
                return (byte)((data[outer] & (3ul << offset)) >> offset);
            }
        }

        public Object Clone()
        {
            return new Sequence(this);
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (byte b in this)
            {
                char c;
                switch (b)
                {
                    case A:
                        c = 'A';
                        break;
                    case C:
                        c = 'C';
                        break;
                    case G:
                        c = 'G';
                        break;
                    case T:
                        c = 'T';
                        break;
                    default:
                        throw new Exception("Unknown element in sequence");
                }
                s.Append(c);
            }
            return s.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Sequence)
            {
                return this.CompareTo((Sequence)obj) == 0;
            }
            return false;
        }

        public int CompareTo(Sequence other)
        {
            for (int i = 0; i < Math.Min(other.data.Length, this.data.Length); i++)
            {
                if (this.data[i] > other.data[i])
                {
                    return 1;
                }
                else if (this.data[i] < other.data[i])
                {
                    return -1;
                }
            }
            if (this.data.Length < other.data.Length)
            {
                return -1;
            }
            return 0;
        }

        public override int GetHashCode()
        {
            if (hashcode == 0)
            {
                this.hashcode = Hash.hashcode(data);
            }
            return this.hashcode;
        }
    }
}
