using System.ComponentModel.DataAnnotations;

namespace EntityHomeWork.WebAPI.Dtos
{
    public record RegisterReaderDTO(
        [Required][StringLength(maximumLength: 100, MinimumLength = 4)] 
        string Login,
        [Required][StringLength(maximumLength: 255, MinimumLength = 6)]
        string Password,
        [Required][EmailAddress]
        string? Email,
        string FirstName,
        string LastName,
        [Required]
        int? DocumentTypeId,
        string DocumentNumber);
 

    public record RegisterLibrarianDTO(
        [Required][StringLength(maximumLength: 100, MinimumLength = 4)]
        string Login,
        [Required][StringLength(maximumLength: 255, MinimumLength = 6)]
        string Password,
        [Required][EmailAddress]
        string Email);
}
