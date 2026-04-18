namespace LoveCapsule.Api.DTOs
{
    public class CreateMessageDto
    {
        public Guid EventId { get; set; }

        public string Text { get; set; } = "";

        public string AuthorName { get; set; } = "";
    }
}
