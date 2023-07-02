using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS.Model.DB
{
    /// <summary>
    /// Trace to target host
    /// </summary>
    [Table(name: "Trace", Schema = "public")]
    public class Trace
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Link to host identifier
        /// </summary>
        public int HostId { get; set; }

        /// <summary>
        /// Hop
        /// </summary>
        public int TTL { get; set; }

        /// <summary>
        /// Intermediate host ip
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// Intermediate host name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        [ForeignKey(nameof(HostId))]
        public virtual Host Host { get; set; } = new Host();
    }
}