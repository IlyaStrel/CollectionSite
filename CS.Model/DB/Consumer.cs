using System;
using System.ComponentModel.DataAnnotations;

namespace CS.Model.DB
{
    public class Consumer
    {
        /// <summary>
        /// Unicue identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Request date
        /// </summary>
        public DateTimeOffset Received { get; set; }

        /// <summary>
        /// User agent
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// OS platform
        /// </summary>
        public string? Platform { get; set; }

        /// <summary>
        /// Real IP
        /// </summary>
        public string? RealIp { get; set; }
    }
}