using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class PrintPreviewViewModel : ObservableObject
    {
        private readonly IChequeService _chequeService;
        private readonly ITemplateService _templateService;
        private readonly IPdfGenerationService _pdfGenerationService;
        private readonly IPrintService _printService;

        [ObservableProperty]
        private Cheque? _cheque;

        [ObservableProperty]
        private BankTemplate? _template;

        [ObservableProperty]
        private ObservableCollection<string> _printers = new();

        [ObservableProperty]
        private string? _selectedPrinter;

        [ObservableProperty]
        private double _horizontalOffsetMm;

        [ObservableProperty]
        private double _verticalOffsetMm;

        [ObservableProperty]
        private bool _printLandscape;

        [ObservableProperty]
        private string? _pdfFilePath;

        [ObservableProperty]
        private bool _isPrinted;

        public PrintPreviewViewModel(
            IChequeService chequeService,
            ITemplateService templateService,
            IPdfGenerationService pdfGenerationService,
            IPrintService printService)
        {
            _chequeService = chequeService;
            _templateService = templateService;
            _pdfGenerationService = pdfGenerationService;
            _printService = printService;
        }

        public async Task LoadChequeAsync(Guid chequeId)
        {
            Cheque = await _chequeService.GetChequeByIdAsync(chequeId);
            if (Cheque == null) return;

            var bankId = Cheque.ChequeBook?.BankId ?? Guid.Empty;
            Template = await _templateService.GetTemplateForBankAsync(bankId);

            if (Template == null)
            {
                var templates = await _templateService.GetAllTemplatesAsync();
                Template = templates.Count > 0 ? templates[0] : new BankTemplate();
            }

            Printers.Clear();
            var printerList = _printService.GetInstalledPrinters();
            foreach (var p in printerList) Printers.Add(p);

            if (Printers.Count > 0)
            {
                SelectedPrinter = Printers[0];
            }

            await GeneratePdfPreviewAsync();
        }

        partial void OnSelectedPrinterChanged(string? value)
        {
            if (value != null)
            {
                _ = LoadCalibrationAsync(value);
            }
        }

        private async Task LoadCalibrationAsync(string printerName)
        {
            var cal = await _printService.GetCalibrationAsync(printerName, Template?.Id);
            if (cal != null)
            {
                HorizontalOffsetMm = (double)cal.HorizontalOffsetMm;
                VerticalOffsetMm = (double)cal.VerticalOffsetMm;
                PrintLandscape = cal.PrintLandscape;
            }
            else
            {
                HorizontalOffsetMm = 0;
                VerticalOffsetMm = 0;
                PrintLandscape = false;
            }
        }

        [RelayCommand]
        private async Task GeneratePdfPreviewAsync()
        {
            if (Cheque == null || Template == null) return;

            var calibration = new PrinterCalibration
            {
                PrinterName = SelectedPrinter ?? "Default",
                HorizontalOffsetMm = (decimal)HorizontalOffsetMm,
                VerticalOffsetMm = (decimal)VerticalOffsetMm,
                PrintLandscape = PrintLandscape
            };

            PdfFilePath = await _pdfGenerationService.GenerateChequePdfAsync(Cheque, Template, calibration);
        }

        [RelayCommand]
        private async Task PrintChequeAsync()
        {
            if (Cheque == null || string.IsNullOrEmpty(PdfFilePath) || string.IsNullOrEmpty(SelectedPrinter))
                return;

            bool printed = await _printService.PrintPdfAsync(PdfFilePath, SelectedPrinter);
            if (printed)
            {
                await _chequeService.MarkAsPrintedAsync(Cheque.Id, "User", PdfFilePath);
                IsPrinted = true;

                // Save calibration settings
                await _printService.SaveCalibrationAsync(new PrinterCalibration
                {
                    PrinterName = SelectedPrinter,
                    HorizontalOffsetMm = (decimal)HorizontalOffsetMm,
                    VerticalOffsetMm = (decimal)VerticalOffsetMm,
                    PrintLandscape = PrintLandscape,
                    TemplateId = Template?.Id
                });
            }
        }
    }
}
