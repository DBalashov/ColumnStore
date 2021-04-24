using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace ColumnStore
{
    static class StructExtenders
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void PackStructs<T>([NotNull] this T[]    source, int sourceOffsetInBytes,
                                            [NotNull]      byte[] dest,   int destOffsetInBytes = 0,
                                            int?                  countItems = null) where T : struct
        {
            using var p = new PinnedHandle(source);
            Marshal.Copy(IntPtr.Add(p.Address, sourceOffsetInBytes),
                         dest, destOffsetInBytes,
                         (countItems ?? source.Length) * Marshal.SizeOf(typeof(T)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        internal static T[] UnpackStructs<T>([NotNull] this byte[] source, int sourceOffsetInBytes, int count) where T : struct
        {
            var       dest = new T[count];
            using var p    = new PinnedHandle(dest);
            Marshal.Copy(source, sourceOffsetInBytes, p.Address, count * Marshal.SizeOf(typeof(T)));
            return dest;
        }
    }

    class PinnedHandle : IDisposable
    {
        readonly GCHandle      handle;
        bool                   isDisposed;
        public readonly IntPtr Address;

        public PinnedHandle(object dest)
        {
            handle     = GCHandle.Alloc(dest, GCHandleType.Pinned);
            Address    = handle.AddrOfPinnedObject();
            isDisposed = false;
        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;

            if (handle.IsAllocated)
                handle.Free();
        }
    }
}