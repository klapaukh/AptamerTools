using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharptamer
{
    namespace Utils
    {
        class SequenceEnumerator : IEnumerator<byte>
        {
            private int idx;
            private readonly Sequence s;

            public SequenceEnumerator(Sequence s)
            {
                this.s = s;
                this.Reset();
            }
            public byte Current
            {
                get
                {
                    if (this.idx >= s.Length())
                    {
                        throw new IndexOutOfRangeException("Trying to access base past the end: " + idx + " / " + s.Length());
                    }
                    return s[idx];
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                this.idx++;
                return this.idx >= 0 && this.idx < s.Length();
            }

            public void Reset()
            {
                this.idx = -1;
            }

            public void Dispose() { }
        }
    }
}