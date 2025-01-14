﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Configuration
{
    /// <summary>
    /// Represents the World Server configuration structure.
    /// </summary>
    [DataContract]
    public class WorldConfiguration : BaseConfiguration
    {
        public const string DefaultLanguage = "en";
        
        /// <summary>
        /// Gets or sets the world's id.
        /// </summary>
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent cluster server id.
        /// </summary>
        [DataMember(Name = "clusterId")]
        public int ClusterId { get; set; }

        /// <summary>
        /// Gets or sets the world's name.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the enabled or disabled systems of the world server.
        /// </summary>
        [DataMember(Name = "systems")]
        public IDictionary<string, bool> Systems { get; set; }

        /// <summary>
        /// Gets or sets the maps of the world server.
        /// </summary>
        [DataMember(Name = "maps")]
        public IEnumerable<string> Maps { get; set; }

        /// <summary>
        /// Gets or sets the world server's language.
        /// </summary>
        [DataMember(Name = "language")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the world server's rates.
        /// </summary>
        [DataMember(Name = "rates")]
        public WorldRates Rates { get; set; } = new WorldRates();

        /// <summary>
        /// Gets or sets the world drops configuration.
        /// </summary>
        [DataMember(Name = "drops")]
        public WorldDrops Drops { get; set; } = new WorldDrops();

        /// <summary>
        /// Gets or sets the IPC configuration.
        /// </summary>
        [DataMember(Name = "isc")]
        public ISCConfiguration ISC { get; set; } = new ISCConfiguration();

        /// <summary>
        /// Gets or sets the mails configuration.
        /// </summary>
        [DataMember(Name = "mails")]
        public MailConfiguration Mails { get; set; } = new MailConfiguration();

        /// <summary>
        /// Gets or sets the Style Customization settings.
        /// </summary>
        [DataMember(Name = "customization")]
        public StyleCustomization Customization { get; set; } = new StyleCustomization();

        /// <summary>
        /// Gets or sets the Party Configuration settings.
        /// </summary>
        [DataMember(Name = "party")]
        public PartyConfiguration PartyConfiguration { get; set; } = new PartyConfiguration();

        /// <summary>
        /// Gets or sets the death configuration settings.
        /// </summary>
        [DataMember(Name = "death")]
        public DeathConfiguration Death { get; set; } = new DeathConfiguration();

        /// <summary>
        /// Creates a new <see cref="WorldConfiguration"/> instance.
        /// </summary>
        public WorldConfiguration()
        {
            this.Language = DefaultLanguage;
            this.Systems = new Dictionary<string, bool>();
        }
    }
}
