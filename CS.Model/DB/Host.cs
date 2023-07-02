using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CS.Model.DB
{
    /// <summary>
    /// Target host information
    /// </summary>
    [Table(name: "Host", Schema = "public")]
    public class Host
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Cosumer IP-address
        /// </summary>
        public string? ConsumerIp { get; set; }

        /// <summary>
        /// Host IP
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// Host Type IP
        /// </summary>
        public string? TypeIp { get; set; }

        /// <summary>
        /// Host name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Host status
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Date create
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Port list
        /// </summary>
        [InverseProperty(nameof(Port.Host))]
        public virtual ICollection<Port> Ports { get; set; } = new List<Port>();

        /// <summary>
        /// Trace list
        /// </summary>
        [InverseProperty(nameof(Port.Host))]
        public virtual ICollection<Trace> Traces { get; set; } = new List<Trace>();
    }
}
