using System;
using System.Runtime.InteropServices;

namespace ManagedBass
{
    /// <summary>
    /// Blittable table of callback function pointers used with <see cref="Bass.CreateStream(StreamSystem,BassFlags,FileProcedurePointers,IntPtr)" />.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="FileProcedures"/>, no delegate is marshalled when the stream is created and nothing is copied back afterwards,
    /// which suits AOT runtimes such as IL2CPP. There, the callbacks must be static methods marked with MonoPInvokeCallbackAttribute.
    /// <para>
    /// BASS makes a copy of the table, so it does not have to persist beyond the call.
    /// The delegates the pointers were created from must stay alive (eg. in static fields) for as long as any stream using them exists:
    /// ManagedBass does not hold references to them.
    /// </para>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct FileProcedurePointers
    {
        /// <summary>
        /// Pointer to the <see cref="FileCloseProcedure"/> callback.
        /// </summary>
        public IntPtr Close;

        /// <summary>
        /// Pointer to the <see cref="FileLengthProcedure"/> callback.
        /// </summary>
        public IntPtr Length;

        /// <summary>
        /// Pointer to the <see cref="FileReadProcedure"/> callback.
        /// </summary>
        public IntPtr Read;

        /// <summary>
        /// Pointer to the <see cref="FileSeekProcedure"/> callback. Not used by buffered file streams.
        /// </summary>
        public IntPtr Seek;

        /// <summary>
        /// Creates a table from callback delegates.
        /// </summary>
        /// <param name="Close">Callback function to close the file.</param>
        /// <param name="Length">Callback function to get the file Length.</param>
        /// <param name="Read">Callback function to read from the file.</param>
        /// <param name="Seek">Callback function to seek in the file, or <see langword="null"/> for buffered file streams, which do not use it.</param>
        /// <remarks>The delegates must stay alive for as long as any stream created with this table exists.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="Close"/>, <paramref name="Length"/> or <paramref name="Read"/> is <see langword="null"/>.</exception>
        public FileProcedurePointers(FileCloseProcedure Close, FileLengthProcedure Length, FileReadProcedure Read, FileSeekProcedure Seek)
        {
            if (Close == null)
                throw new ArgumentNullException(nameof(Close));

            if (Length == null)
                throw new ArgumentNullException(nameof(Length));

            if (Read == null)
                throw new ArgumentNullException(nameof(Read));

            this.Close = Marshal.GetFunctionPointerForDelegate(Close);
            this.Length = Marshal.GetFunctionPointerForDelegate(Length);
            this.Read = Marshal.GetFunctionPointerForDelegate(Read);
            this.Seek = Seek == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(Seek);
        }
    }
}
