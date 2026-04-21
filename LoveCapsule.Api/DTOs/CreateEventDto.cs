namespace LoveCapsule.Api.DTOs
{
    public class CreateEventDto
    {
        public string Title { get; set; }
        public string HostName { get; set; }
        public DateTime EventDate { get; set; }
    }
}
