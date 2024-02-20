using System;
using System.Collections.Generic;

namespace EntityHomeWork.DBModels;

public partial class TypeOfPublishingCode
{
    public int PublishingCodeTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
