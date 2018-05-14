  

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
