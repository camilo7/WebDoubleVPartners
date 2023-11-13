namespace WebDoubleVPartners.Models.Inputs
{
    public class UserInput
    {
        public string? UserName { get; set; }

        public string? Pass { get; set; }

        public DateTime CreationDate { get; set; }

        public UserInput()
        {
            CreationDate = DateTime.UtcNow;
        }
    }
}
