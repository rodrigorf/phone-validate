namespace PhoneValidate.Application.Services.Dto
{
    public class RecipientsDto
    {
        public Guid Id { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}