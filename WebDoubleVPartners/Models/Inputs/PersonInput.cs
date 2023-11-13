namespace WebDoubleVPartners.Models.Inputs
{
    public class PersonInput
    {
        public string? Names { get; set; }

        public string? LastNames { get; set; }

        public long? IdentificationNumber { get; set; }

        public string? Email { get; set; }

        public string? IdentificationType { get; set; }

        public DateTime? CreationDate { get; set; }

        public PersonInput()
        {
            CreationDate = DateTime.UtcNow;
        }
    }
}
