using System;

namespace DualScreenDemo
{
    public static class MediaTypes
    {
        public static readonly Guid Video = new Guid("73646976-0000-0010-8000-00AA00389B71"); // MEDIATYPE_Video, 'vids'
        public static readonly Guid Audio = new Guid("73647561-0000-0010-8000-00AA00389B71"); // MEDIATYPE_Audio, 'auds'
    }
}