using System;
using System.Collections.Generic;

namespace EntityHomeWork.DBModels;

public partial class Book
{
    public int BookId { get; set; }

    public string? Title { get; set; }

    public string? PublishingCode { get; set; }

    public int? PublishingCodeTypeId { get; set; }

    public DateTime? Year { get; set; }

    public string? CountryOfPublishing { get; set; }

    public string? CityOfPublishHouse { get; set; }

    public virtual TypeOfPublishingCode? PublishingCodeType { get; set; }

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
}
