namespace EntityHomeWork.WebAPI.Dtos
{
    public class UpdateLibrarianDTO
    {
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class UpdateReaderDTO
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }

        public int? DocumentTypeId { get; set; }
    }
}
