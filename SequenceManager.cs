namespace DualScreenDemo
{
    
    public class SequenceManager
    {
        private ClickSequenceState currentState = ClickSequenceState.Initial;

        public void ProcessClick(string position)
        {
            switch (currentState)
            {
                case ClickSequenceState.Initial:
                    if (position == "中間")
                    {
                        currentState = ClickSequenceState.FirstClicked;
                    }
                    break;
                case ClickSequenceState.FirstClicked:
                    if (position == "右上")
                    {
                        currentState = ClickSequenceState.SecondClicked;
                    }
                    else
                    {
                        ResetState();
                    }
                    break;
                case ClickSequenceState.SecondClicked:
                    if (position == "左上")
                    {
                        currentState = ClickSequenceState.ThirdClicked;
                    }
                    else
                    {
                        ResetState();
                    }
                    break;
                case ClickSequenceState.ThirdClicked:
                    if (position == "謝謝")
                    {
                        currentState = ClickSequenceState.Completed;
                        PerformShutdown();
                    }
                    else
                    {
                        ResetState();
                    }
                    break;
            }
        }

        private void ResetState()
        {
            currentState = ClickSequenceState.Initial;
        }

        private void PerformShutdown()
        {
            // 执行关机代码
            System.Diagnostics.Process.Start("shutdown", "/s /t 0");
        }
    }
}