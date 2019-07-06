using Rhisis.Core.Data;
using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Leveling.EventArgs
{
    public class ChangeJobEventArgs : SystemEventArgs
    {
        public int JobId { get; }

        public ChangeJobEventArgs(int jobId)
        {
            this.JobId = jobId;
        }

        public override bool GetCheckArguments() => this.JobId > -1 && this.JobId < (int)DefineJob.Job.JOB_ALL;
    }
}
