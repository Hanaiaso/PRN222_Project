namespace PRN_Project.Models
{
    public class OTPModel
    {
        public string Email { get; set; }
        public string OTPCode { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
