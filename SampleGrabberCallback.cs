using System;
using DirectShowLib;

namespace DualScreenDemo
{
    public class SampleGrabberCallback : ISampleGrabberCB
    {
        private VideoPlayerForm form;

        public SampleGrabberCallback(VideoPlayerForm form)
        {
            this.form = form;
        }

        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            // 不需要实现此方法
            return 0;
        }

        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            // 在此处处理回调，例如将样本数据传递到渲染器
            return 0;
        }
    }
}