using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LoveCapsule.Api.Services
{
    public class PdfService
    {
        public byte[] GenerateBook(string title, string hostName, DateTime date, List<(string Text, string Author)> messages)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    page.Content().Column(col =>
                    {
                        col.Item().Text(title).FontSize(24).Bold();
                        col.Item().Text(hostName).FontSize(18);
                        col.Item().Text(date.ToShortDateString()).FontSize(14);

                        col.Item().PaddingVertical(10);

                        foreach (var msg in messages)
                        {
                            col.Item().Text($"💬 {msg.Text}");
                            col.Item().Text($"— {msg.Author}").Italic().FontSize(12);

                            col.Item().PaddingBottom(10);
                        }
                    });
                });
            }).GeneratePdf();
        }
    }
}
