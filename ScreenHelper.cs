using System;
using System.Linq; // Add this for LINQ methods like FirstOrDefault
using System.Windows.Forms; // Add this for Screen

namespace DualScreenDemo
{
    
    public static class ScreenHelper
    {
        public static Screen GetSecondMonitor()
        {
            return Screen.AllScreens.FirstOrDefault(s => !s.Primary) ?? Screen.PrimaryScreen;
        }
    }
}