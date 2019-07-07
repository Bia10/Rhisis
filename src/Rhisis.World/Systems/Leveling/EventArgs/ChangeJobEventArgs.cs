using Rhisis.Core.Data;
using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Leveling.EventArgs
{
    public class ChangeJobEventArgs : SystemEventArgs
    {
        public int JobId { get; }

        public bool Restat { get; }

        public ChangeJobEventArgs(int jobId, bool restat)
        {
            this.JobId = jobId;
            this.Restat = restat;
        }

        public override bool GetCheckArguments() => this.JobId > -1 && this.JobId < (int)DefineJob.Job.JOB_ALL;
    }
}
