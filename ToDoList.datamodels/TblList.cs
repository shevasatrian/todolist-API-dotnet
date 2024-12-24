using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.datamodels;

[Table("TblList")]
public partial class TblList
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [Unicode(false)]
    public string? Note { get; set; }

    public bool? IsDelete { get; set; }

    public long? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    public long? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }

    public long? DeletedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedOn { get; set; }

    [StringLength(20)]
    public string? Color { get; set; }
}
