using System;
using System.Collections.Generic;

namespace EntityHomeWork.DBModels;

public partial class Author
{
    public int AuthorId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? SecondName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
