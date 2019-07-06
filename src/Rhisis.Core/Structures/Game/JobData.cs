using Rhisis.Core.Data;
using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Game
{
    [DataContract]
    public sealed class JobData
    {
        /// <summary>
        /// Gets or sets the job Id.
        /// </summary>
        [DataMember(Order = 0)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the job name.
        /// </summary>
        [IgnoreDataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the job type.
        /// </summary>
        [IgnoreDataMember]
        public DefineJob.JobType Type { get; set; }

        /// <summary>
        /// Gets or sets the job's base job.
        /// </summary>
        [IgnoreDataMember]
        public DefineJob.Job BaseJob { get; set; }

        /// <summary>
        /// Gets or sets the job attack speed bonus.
        /// </summary>
        [DataMember(Order = 1)]
        public float AttackSpeed { get; set; }

        /// <summary>
        /// Gets or sets the job's max HP factor.
        /// </summary>
        [DataMember(Order = 2)]
        public float MaxHpFactor { get; set; }

        /// <summary>
        /// Gets or sets the job's max MP factor.
        /// </summary>
        [DataMember(Order = 3)]
        public float MaxMpFactor { get; set; }

        /// <summary>
        /// Gets or sets the job's max FP factor.
        /// </summary>
        [DataMember(Order = 4)]
        public float MaxFpFactor { get; set; }

        /// <summary>
        /// Gets or sets the job's defense factor.
        /// </summary>
        [DataMember(Order = 5)]
        public float DefenseFactor { get; set; }

        /// <summary>
        /// Gets or sets the job's HP recovery factor.
        /// </summary>
        [DataMember(Order = 6)]
        public float HpRecoveryFactor { get; set; }

        /// <summary>
        /// Gets or sets the job's MP recovery factor.
        /// </summary>
        [DataMember(Order = 7)]
        public float MpRecoveryFactor { get; set; }

        /// <summary>
        /// Gets or sets the job's FP recovery factor.
        /// </summary>
        [DataMember(Order = 8)]
        public float FpRecoveryFactor { get; set; }

        /// <summary>
        /// Gets or sets the job's sword attack factor.
        /// </summary>
        [DataMember(Order = 9)]
        public float MeleeSword { get; set; }

        /// <summary>
        /// Gets or sets the job's axe attack factor.
        /// </summary>
        [DataMember(Order = 10)]
        public float MeleeAxe { get; set; }

        /// <summary>
        /// Gets or sets the job's staff attack factor.
        /// </summary>
        [DataMember(Order = 11)]
        public float MeleeStaff { get; set; }

        /// <summary>
        /// Gets or sets the job's stick attack factor.
        /// </summary>
        [DataMember(Order = 12)]
        public float MeleeStick { get; set; }

        /// <summary>
        /// Gets or sets the job's knuckle attack factor.
        /// </summary>
        [DataMember(Order = 13)]
        public float MeleeKnuckle { get; set; }

        /// <summary>
        /// Gets or sets the job's wand attack factor.
        /// </summary>
        [DataMember(Order = 14)]
        public float MagicWand { get; set; }

        /// <summary>
        /// Gets or sets the job's blocking attack factor.
        /// </summary>
        [DataMember(Order = 15)]
        public float Blocking { get; set; }

        /// <summary>
        /// Gets or sets the job's yo-yo attack factor.
        /// </summary>
        [DataMember(Order = 16)]
        public float MeleeYoyo { get; set; }

        /// <summary>
        /// Gets or sets the job's critical attack factor.
        /// </summary>
        [DataMember(Order = 17)]
        public float Critical { get; set; }

        /// <inheritdoc />
        public override string ToString() => this.Name;
    }
}
