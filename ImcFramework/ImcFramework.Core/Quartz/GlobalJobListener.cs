﻿using ImcFramework.LogPool;
using Quartz;

namespace ImcFramework.Core.Quartz
{
    /// <summary>
    /// Global job listener.
    /// </summary>
    public class GlobalJobListener : IJobListener
    {
        private ILoggerPool loggerPool;
        public GlobalJobListener(ILoggerPool loggerPool)
        {
            this.loggerPool = loggerPool;
        }

        public virtual void JobExecutionVetoed(IJobExecutionContext context)
        {

        }

        public virtual void JobToBeExecuted(IJobExecutionContext context)
        {

        }

        /// <summary>
        /// After job was Executed
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="jobException">The exception.</param>
        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (jobException != null)
            {
                loggerPool.Log(Name, new LogContentEntity()
                {
                    Level = "Error",
                    Message = string.Format("[{0}]:[{1}] Has An Exceptions：", context.JobDetail.Key.Name, context.JobDetail.Key.Group) + jobException.Message + jobException.StackTrace
                });
            }
        }

        public virtual string Name
        {
            get { return "MainJobListener"; }
        }
    }
}
