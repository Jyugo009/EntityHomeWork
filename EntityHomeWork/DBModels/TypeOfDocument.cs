using System;
using System.Collections.Generic;

namespace EntityHomeWork.DBModels;

public partial class TypeOfDocument
{
    public int DocumentTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Reader> Readers { get; set; } = new List<Reader>();
}
