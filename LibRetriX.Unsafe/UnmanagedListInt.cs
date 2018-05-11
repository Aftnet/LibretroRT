using System;
using System.Collections;
using System.Collections.Generic;

namespace LibRetriX.RetroBindings.Unsafe
{
    public unsafe class UnmanagedListInt : IReadOnlyList<int>
    {
        private class InnerEnumerator : IEnumerator<int>
        {
            private int* StartPtr { get; }
            private int* EndPtr { get; }
            private int* CurrentPtr { get; set; } = null;

            public int Current => *CurrentPtr;
            object IEnumerator.Current => Current;

            public InnerEnumerator(int* startPtr, int* endPtr)
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

        private int* StartPtr { get; }

        public int Count { get; }
        public int this[int index] => StartPtr[index];

        public UnmanagedListInt(IntPtr startPtr, int count)
        {
            StartPtr = (int*)startPtr.ToPointer();
            Count = count;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return new InnerEnumerator(StartPtr, StartPtr + Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
