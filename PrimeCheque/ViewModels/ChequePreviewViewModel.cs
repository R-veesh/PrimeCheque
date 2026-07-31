using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class ChequePreviewViewModel : ObservableObject
    {
        private readonly IChequeService _chequeService;
        private readonly ITemplateService _templateService;
        private readonly IPdfGenerationService _pdfGenerationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private Cheque? _cheque;

        [ObservableProperty]
        private string? _pdfFilePath;

        public ChequePreviewViewModel(
            IChequeService chequeService,
            ITemplateService templateService,
            IPdfGenerationService pdfGenerationService,
            INavigationService navigationService)
        {
            _chequeService = chequeService;
            _templateService = templateService;
            _pdfGenerationService = pdfGenerationService;
            _navigationService = navigationService;
        }

        public async Task LoadChequeAsync(Guid chequeId)
        {
            Cheque = await _chequeService.GetChequeByIdAsync(chequeId);
            if (Cheque == null) return;

            var bankId = Cheque.ChequeBook?.BankId ?? Guid.Empty;
            var template = await _templateService.GetTemplateForBankAsync(bankId);

            if (template == null)
            {
                var templates = await _templateService.GetAllTemplatesAsync();
                template = templates.Count > 0 ? templates[0] : new BankTemplate();
            }

            // Generate preview without offsets (calibration zeroed)
            var calibration = new PrinterCalibration
            {
                PrinterName = "Default",
                HorizontalOffsetMm = 0,
                VerticalOffsetMm = 0,
                PrintLandscape = false
            };

            PdfFilePath = await _pdfGenerationService.GenerateChequePdfAsync(Cheque, template, calibration);
        }

        [RelayCommand]
        private void GoBack()
        {
            _navigationService.GoBack();
        }
    }
}
