using System;
using System.Collections.Generic;
using EntityHomeWork.DBModels;

namespace EntityHomeWork;

public partial class TypeOfPublishingCode
{
    public int PublishingCodeTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
