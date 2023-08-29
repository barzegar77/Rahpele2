using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class Permission
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Permission? Parent { get; set; }

        public ICollection<RolePermission>? RolePermissions { get; set; }
    }
}
