using System;

namespace DualScreenDemo
{    
    public static class Clsid
    {
        public static readonly Guid LAVSplitter = new Guid("171252A0-8820-4AFE-9DF8-5C92B2D66B04");
        public static readonly Guid LAVVideoDecoder = new Guid("EE30215D-164F-4A92-A4EB-9D4C13390F9F");
        public static readonly Guid LAVAudioDecoder = new Guid("E8E73B6B-4CB3-44A4-BE99-4F7BCB96E491");
        public static readonly Guid VideoMixingRenderer9 = new Guid("51B4ABF3-748F-4E3B-A276-C828330E926A");
        public static readonly Guid EnhancedVideoRenderer = new Guid("FA10746C-9B63-4B6C-BC49-FC300EA5F256");
        public static readonly Guid DirectSoundAudioRenderer = new Guid("79376820-07D0-11CF-A24D-0020AFD79767");
        public static readonly Guid FileSourceAsync = new Guid("E436EBB5-524F-11CE-9F53-0020AF0BA770");
        public static readonly Guid InfinitePinTee = new Guid("F8388A40-D5BB-11D0-BE5A-0080C706568E");
    }
}