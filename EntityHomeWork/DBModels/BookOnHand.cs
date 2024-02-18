using System;
using System.Collections.Generic;

namespace EntityHomeWork.DBModels;

public partial class BookOnHand
{
    public int Id { get; set; }

    public int? BookId { get; set; }

    public string? TakenBy { get; set; }

    public DateTime? CheckoutDate { get; set; }

    public DateTime? DueDate { get; set; }

    public int? DaysLeft { get; set; }

    public DateTime? ReturnDate { get; set; }

    public virtual Book? Book { get; set; }

    public virtual Reader? TakenByNavigation { get; set; }
}
