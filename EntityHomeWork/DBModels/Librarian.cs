using System;
using System.Collections.Generic;

namespace EntityHomeWork.DBModels;

public partial class Librarian
{
    public string Login { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }
}
