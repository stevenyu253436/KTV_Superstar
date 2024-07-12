using System;

namespace DualScreenDemo
{
    public class RemoteControlEventArgs : EventArgs
    {
        public RemoteCommand Command { get; set; }
    }
}