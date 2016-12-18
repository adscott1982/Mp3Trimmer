using System;
using AndyTools.Wpf;

namespace AndyTools.Utilities
{
    public class ProgressManager
    {
        private ILogger logger;
        private Action<int> externalSetProgressAction;

        public IProgress<string> ProgressString { get; }
        public IProgress<int> ProgressPercentage { get; }

        public ProgressManager(ILogger logger, Action<int> setProgressValueAction)
        {
            this.logger = logger;
            this.externalSetProgressAction = setProgressValueAction;

            ProgressString = new Progress<string>(Log);
            ProgressPercentage = new Progress<int>(SetProgressValue);

            ProgressPercentage.Report(0);
        }

        private void Log(string message)
        {
            logger.Add(message);
        }

        private void SetProgressValue(int value)
        {
            value = value.Clamp(0, 100);
            externalSetProgressAction(value);
        }
    }
}
