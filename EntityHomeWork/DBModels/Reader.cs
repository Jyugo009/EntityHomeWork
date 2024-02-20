using System;
using System.Collections.Generic;

namespace EntityHomeWork.DBModels;

public partial class Reader
{
    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int? DocumentTypeId { get; set; }

    public string DocumentNumber { get; set; } = null!;

    public virtual ICollection<BookOnHand> BookOnHands { get; set; } = new List<BookOnHand>();

    public virtual TypeOfDocument? DocumentType { get; set; }
}
