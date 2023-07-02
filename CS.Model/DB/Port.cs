using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS.Model.DB
{
    /// <summary>
    /// Target port information
    /// </summary>
    [Table(name: "Port", Schema = "public")]
    public class Port
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
        /// Port number
        /// </summary>
        public ushort Number { get; set; }

        /// <summary>
        /// Port type
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Service name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Service product
        /// </summary>
        public string? Product { get; set; }

        /// <summary>
        /// Servic product version
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// External information
        /// </summary>
        public string? Extrainfo { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        [ForeignKey(nameof(HostId))]
        public virtual Host Host { get; set; } = new Host();
    }
}