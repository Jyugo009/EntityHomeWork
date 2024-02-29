namespace EntityHomeWork.WebAPI.Dtos
{
    public class UsersDTO
    {
        public record ReaderDto
        {
            public string Login { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public record LibrarianDto
        {
            public string Login { get; init; }
            public string Email { get; init; }
        }
    }
}
