using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace ColumnStore
{
    public static class StructExtenders
    {
        #region MapStructs

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static byte[] PackStructs<T>([NotNull] this T[] source, int offsetInBytes = 0, int? count = null) where T : struct
        {
            using var p    = new PinnedHandle(source);
            var       dest = new byte[(count ?? source.Length) * Marshal.SizeOf(typeof(T))];
            Marshal.Copy(IntPtr.Add(p.Address, offsetInBytes), dest, 0, dest.Length);
            return dest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackStructs<T>([NotNull] this T[] source, [NotNull] byte[] dest, int offsetInBytes) where T : struct
        {
            using var p = new PinnedHandle(source);
            Marshal.Copy(p.Address, dest, offsetInBytes, source.Length * Marshal.SizeOf(typeof(T)));
        }

        #endregion

        #region UnmapStructs

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [NotNull]
        // public static T[] UnpackStructs<T>([NotNull] this byte[] source) where T : struct
        // {
        //     var dest = new T[source.Length / Marshal.SizeOf(typeof(T))];
        //
        //     using var p = new PinnedHandle(dest);
        //     Marshal.Copy(source, 0, p.Address, source.Length);
        //
        //     return dest;
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static T[] UnpackStructs<T>([NotNull] this byte[] source, int offsetInBytes, int count) where T : struct
        {
            var       dest = new T[count];
            using var p    = new PinnedHandle(dest);
            Marshal.Copy(source, offsetInBytes, p.Address, count * Marshal.SizeOf(typeof(T)));
            return dest;
        }

        #endregion
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