using System;
using System.Collections;
using System.Collections.Generic;

namespace LibRetriX.RetroBindings.Unsafe
{
    public unsafe class UnmanagedListShort : IReadOnlyList<short>
    {
        private class InnerEnumerator : IEnumerator<short>
        {
            private short* StartPtr { get; }
            private short* EndPtr { get; }
            private short* CurrentPtr { get; set; } = null;

            public short Current => *CurrentPtr;
            object IEnumerator.Current => Current;

            public InnerEnumerator(short* startPtr, short* endPtr)
            {
                StartPtr = startPtr;
                EndPtr = endPtr;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                CurrentPtr++;
                return CurrentPtr < EndPtr;
            }

            public void Reset()
            {
                CurrentPtr = StartPtr;
            }
        }

        private short* StartPtr { get; }

        public int Count { get; }
        public short this[int index] => StartPtr[index];

        public UnmanagedListShort(IntPtr startPtr, int count)
        {
            StartPtr = (short*)startPtr.ToPointer();
            Count = count;
        }

        public IEnumerator<short> GetEnumerator()
        {
            return new InnerEnumerator(StartPtr, StartPtr + Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
