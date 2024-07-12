using System;
using System.Runtime.InteropServices;

namespace DualScreenDemo
{
    public class ComInterop
    {
        [DllImport("ole32.dll")]
        public static extern int CoInitializeEx(IntPtr pvReserved, int dwCoInit);

        public const int COINIT_APARTMENTTHREADED = 0x2; // Single-threaded apartment
        public const int COINIT_MULTITHREADED = 0x0;    // Multi-threaded apartment
    }
}