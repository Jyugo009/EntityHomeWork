using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntityHomeWork.DBModels;

namespace EntityHomeWork;

public partial class Reader
{
    [Required]
    public string Login { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string? Email { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public int? DocumentTypeId { get; set; }

    [Required]
    public string DocumentNumber { get; set; } = null!;

    public virtual TypeOfDocument? DocumentType { get; set; }
}
