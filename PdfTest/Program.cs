using System;
using System.Drawing.Printing;
using System.Drawing;
using PdfiumViewer;

try 
{
    Console.WriteLine("Testing Printer without CreatePrintDocument()");
    
    string dummyPdf = "test.pdf";
    if (!System.IO.File.Exists(dummyPdf)) return;

    using (var document = PdfiumViewer.PdfDocument.Load(dummyPdf))
    {
        using (var printDoc = new PrintDocument())
        {
            printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
            printDoc.PrintController = new StandardPrintController();
            
            int currentPage = 0;
            printDoc.PrintPage += (s, e) => 
            {
                using (var image = document.Render(currentPage, (int)e.PageBounds.Width, (int)e.PageBounds.Height, e.Graphics.DpiX, e.Graphics.DpiY, PdfRenderFlags.ForPrinting))
                {
                    e.Graphics.DrawImage(image, e.PageBounds);
                }
                currentPage++;
                e.HasMorePages = currentPage < document.PageCount;
            };

            printDoc.Print();
            Console.WriteLine("Print successful!");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("ERROR: " + ex.GetType().Name + " - " + ex.Message);
}
