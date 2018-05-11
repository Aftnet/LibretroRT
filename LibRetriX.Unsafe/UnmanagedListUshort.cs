using System;
using System.Collections;
using System.Collections.Generic;

namespace LibRetriX.Unsafe
{
    public unsafe class UnmanagedListUShort : IReadOnlyList<ushort>
    {
        private class InnerEnumerator : IEnumerator<ushort>
        {
            private ushort* StartPtr { get; }
            private ushort* EndPtr { get; }
            private ushort* CurrentPtr { get; set; } = null;

            public ushort Current => *CurrentPtr;
            object IEnumerator.Current => Current;

            public InnerEnumerator(ushort* startPtr, ushort* endPtr)
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

        private ushort* StartPtr { get; }

        public int Count { get; }
        public ushort this[int index] => StartPtr[index];

        public UnmanagedListUShort(IntPtr startPtr, int count)
        {
            StartPtr = (ushort*)startPtr.ToPointer();
            Count = count;
        }

        public IEnumerator<ushort> GetEnumerator()
        {
            return new InnerEnumerator(StartPtr, StartPtr + Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
