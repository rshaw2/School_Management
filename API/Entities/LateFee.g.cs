using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchoolManagement.Entities
{
#pragma warning disable
    /// <summary> 
    /// Represents a latefee entity with essential details
    /// </summary>
    public class LateFee
    {
        /// <summary>
        /// TenantId of the LateFee 
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Primary key for the LateFee 
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Name of the LateFee 
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Amount of the LateFee 
        /// </summary>
        public int? Amount { get; set; }
        /// <summary>
        /// Description of the LateFee 
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// CreatedOn of the LateFee 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// CreatedBy of the LateFee 
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// UpdatedOn of the LateFee 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        /// <summary>
        /// UpdatedBy of the LateFee 
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
}