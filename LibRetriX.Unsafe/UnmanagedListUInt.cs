using System;
using System.Collections;
using System.Collections.Generic;

namespace LibRetriX.RetroBindings.Unsafe
{
    public unsafe class UnmanagedListUInt : IReadOnlyList<uint>
    {
        private class InnerEnumerator : IEnumerator<uint>
        {
            private uint* StartPtr { get; }
            private uint* EndPtr { get; }
            private uint* CurrentPtr { get; set; } = null;

            public uint Current => *CurrentPtr;
            object IEnumerator.Current => Current;

            public InnerEnumerator(uint* startPtr, uint* endPtr)
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

        private uint* StartPtr { get; }

        public int Count { get; }
        public uint this[int index] => StartPtr[index];

        public UnmanagedListUInt(IntPtr startPtr, int count)
        {
            StartPtr = (uint*)startPtr.ToPointer();
            Count = count;
        }

        public IEnumerator<uint> GetEnumerator()
        {
            return new InnerEnumerator(StartPtr, StartPtr + Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
