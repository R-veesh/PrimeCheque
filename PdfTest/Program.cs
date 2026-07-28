using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

try 
{
    float widthMm = 150f;
    float heightMm = 75f;
    string watermarkText = "TEST PRINT";

    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(new PageSize(widthMm, heightMm, Unit.Millimetre));
            page.Margin(0, Unit.Millimetre);
            page.PageColor(Colors.White);

            page.Content().Layers(layers =>
            {
                layers.PrimaryLayer();
                
                layers.Layer()
                    .Unconstrained()
                    .OffsetX(10, Unit.Millimetre)
                    .OffsetY(10, Unit.Millimetre)
                    .Width(50, Unit.Millimetre)
                    .Text(txt =>
                    {
                        var span = txt.Span("Testing").FontSize(11).FontColor(Colors.Black);
                    });

                if (!string.IsNullOrEmpty(watermarkText))
                {
                    layers.Layer()
                        .OffsetX(widthMm / 4, Unit.Millimetre)
                        .OffsetY(heightMm / 3, Unit.Millimetre)
                        .Text(txt =>
                        {
                            txt.Span(watermarkText).FontSize(24).FontColor(Colors.Grey.Lighten2).Bold();
                        });
                }
            });
        });
    });

    document.GeneratePdf("test.pdf");
    Console.WriteLine("Success");
}
catch (Exception ex)
{
    Console.WriteLine("ERROR: " + ex.Message);
}
