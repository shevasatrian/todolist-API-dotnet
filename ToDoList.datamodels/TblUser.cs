using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.datamodels;

[Table("TblUser")]
public partial class TblUser
{
    [Key]
    public long Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? LastName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? Role { get; set; }

    public bool? IsDelete { get; set; }

    public byte[]? ProfilePicture { get; set; }
}
