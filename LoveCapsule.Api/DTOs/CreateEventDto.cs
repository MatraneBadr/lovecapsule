namespace LoveCapsule.Api.DTOs
{
    public class CreateEventDto
    {
        public string Title { get; set; }
        public string HostName { get; set; }
        public string Email { get; set; }
        public DateTime EventDate { get; set; }
    }
}
