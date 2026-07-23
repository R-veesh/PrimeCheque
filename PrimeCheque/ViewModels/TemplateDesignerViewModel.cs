using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
using PrimeCheque.Services;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class TemplateDesignerViewModel : ObservableObject
    {
        private readonly ITemplateService _templateService;
        private readonly IBankService _bankService;

        [ObservableProperty]
        private ObservableCollection<BankTemplate> _templates = new();

        [ObservableProperty]
        private ObservableCollection<Bank> _banks = new();

        [ObservableProperty]
        private BankTemplate? _selectedTemplate;

        [ObservableProperty]
        private Bank? _selectedBank;

        [ObservableProperty]
        private string _bankName = string.Empty;

        [ObservableProperty]
        private string _seriesName = string.Empty;

        [ObservableProperty]
        private double _chequeWidthMm = 200;

        [ObservableProperty]
        private double _chequeHeightMm = 88;

        [ObservableProperty]
        private string? _templateImagePath;

        [ObservableProperty]
        private string? _fullImagePath;

        // Field coordinates in mm
        [ObservableProperty] private double _dateDayX = 152;
        [ObservableProperty] private double _dateDayY = 12;
        [ObservableProperty] private double _dateMonthX = 164;
        [ObservableProperty] private double _dateMonthY = 12;
        [ObservableProperty] private double _dateYearX = 176;
        [ObservableProperty] private double _dateYearY = 12;

        [ObservableProperty] private double _payeeLine1X = 35;
        [ObservableProperty] private double _payeeLine1Y = 25;
        [ObservableProperty] private double _payeeLine1W = 150;

        [ObservableProperty] private double _amountWordsX = 12;
        [ObservableProperty] private double _amountWordsY = 42;
        [ObservableProperty] private double _amountWordsW = 165;

        [ObservableProperty] private double _amountFiguresX = 158;
        [ObservableProperty] private double _amountFiguresY = 42;
        [ObservableProperty] private double _amountFiguresW = 35;

        [ObservableProperty] private double _crossingX = 8;
        [ObservableProperty] private double _crossingY = 5;

        [ObservableProperty] private double _memoX = 12;
        [ObservableProperty] private double _memoY = 70;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        public TemplateDesignerViewModel(ITemplateService templateService, IBankService bankService)
        {
            _templateService = templateService;
            _bankService = bankService;
        }

        public async Task LoadDataAsync()
        {
            var bnks = await _bankService.GetAllBanksAsync();
            Banks.Clear();
            foreach (var b in bnks) Banks.Add(b);

            var tmpls = await _templateService.GetAllTemplatesAsync();
            Templates.Clear();
            foreach (var t in tmpls) Templates.Add(t);

            if (Templates.Count > 0)
            {
                SelectedTemplate = Templates[0];
            }
        }

        partial void OnSelectedTemplateChanged(BankTemplate? value)
        {
            if (value == null) return;

            BankName = value.BankName;
            SeriesName = value.SeriesName;
            ChequeWidthMm = (double)value.ChequeWidthMm;
            ChequeHeightMm = (double)value.ChequeHeightMm;
            TemplateImagePath = value.TemplateImagePath;

            if (!string.IsNullOrEmpty(TemplateImagePath))
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var absPath = System.IO.Path.Combine(baseDir, TemplateImagePath);
                if (System.IO.File.Exists(absPath))
                {
                    FullImagePath = absPath;
                }
                else
                {
                    FullImagePath = System.IO.Path.Combine(baseDir, "template_image", System.IO.Path.GetFileName(TemplateImagePath));
                }
            }
            else
            {
                FullImagePath = null;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(value.TemplateConfig))
                {
                    var cfg = JsonSerializer.Deserialize<TemplateConfigDto>(value.TemplateConfig);
                    if (cfg != null)
                    {
                        if (cfg.dateDay != null) { DateDayX = cfg.dateDay.x; DateDayY = cfg.dateDay.y; }
                        if (cfg.dateMonth != null) { DateMonthX = cfg.dateMonth.x; DateMonthY = cfg.dateMonth.y; }
                        if (cfg.dateYear != null) { DateYearX = cfg.dateYear.x; DateYearY = cfg.dateYear.y; }

                        if (cfg.payeeLine1 != null) { PayeeLine1X = cfg.payeeLine1.x; PayeeLine1Y = cfg.payeeLine1.y; PayeeLine1W = cfg.payeeLine1.width; }
                        if (cfg.amountWordsLine1 != null) { AmountWordsX = cfg.amountWordsLine1.x; AmountWordsY = cfg.amountWordsLine1.y; AmountWordsW = cfg.amountWordsLine1.width; }
                        if (cfg.amountFigures != null) { AmountFiguresX = cfg.amountFigures.x; AmountFiguresY = cfg.amountFigures.y; AmountFiguresW = cfg.amountFigures.width; }
                        if (cfg.crossingZone != null) { CrossingX = cfg.crossingZone.x; CrossingY = cfg.crossingZone.y; }
                        if (cfg.memoLine != null) { MemoX = cfg.memoLine.x; MemoY = cfg.memoLine.y; }
                    }
                }
            }
            catch
            {
                // Fallback
            }
        }

        [RelayCommand]
        private async Task SaveTemplateAsync()
        {
            if (string.IsNullOrWhiteSpace(BankName) || string.IsNullOrWhiteSpace(SeriesName))
                return;

            var dto = new TemplateConfigDto
            {
                dateDay = new FieldConfig { x = (float)DateDayX, y = (float)DateDayY, width = 12, height = 6, fontSize = 11 },
                dateMonth = new FieldConfig { x = (float)DateMonthX, y = (float)DateMonthY, width = 12, height = 6, fontSize = 11 },
                dateYear = new FieldConfig { x = (float)DateYearX, y = (float)DateYearY, width = 18, height = 6, fontSize = 11 },
                payeeLine1 = new FieldConfig { x = (float)PayeeLine1X, y = (float)PayeeLine1Y, width = (float)PayeeLine1W, height = 7, fontSize = 12 },
                amountWordsLine1 = new FieldConfig { x = (float)AmountWordsX, y = (float)AmountWordsY, width = (float)AmountWordsW, height = 7, fontSize = 11 },
                amountFigures = new FieldConfig { x = (float)AmountFiguresX, y = (float)AmountFiguresY, width = (float)AmountFiguresW, height = 8, fontSize = 12 },
                crossingZone = new FieldConfig { x = (float)CrossingX, y = (float)CrossingY, width = 30, height = 18 },
                memoLine = new FieldConfig { x = (float)MemoX, y = (float)MemoY, width = 100, height = 6, fontSize = 9 }
            };

            var jsonConfig = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });

            var tmpl = SelectedTemplate ?? new BankTemplate();
            tmpl.BankName = BankName;
            tmpl.SeriesName = SeriesName;
            tmpl.ChequeWidthMm = (decimal)ChequeWidthMm;
            tmpl.ChequeHeightMm = (decimal)ChequeHeightMm;
            tmpl.TemplateConfig = jsonConfig;
            tmpl.TemplateImagePath = TemplateImagePath;
            tmpl.BankId = SelectedBank?.Id;

            await _templateService.SaveTemplateAsync(tmpl);
            StatusMessage = "Template saved successfully!";

            await LoadDataAsync();
        }

        [RelayCommand]
        private void CreateNewTemplate()
        {
            SelectedTemplate = null;
            BankName = string.Empty;
            SeriesName = "New Cheque Series";
            ChequeWidthMm = 200;
            ChequeHeightMm = 88;
            StatusMessage = "Creating new template...";
        }
    }
}
