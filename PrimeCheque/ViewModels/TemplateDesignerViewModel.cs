using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
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

        [ObservableProperty]
        private double _canvasWidth = 600;

        public double ScaleFactor => CanvasWidth / (ChequeWidthMm > 0 ? ChequeWidthMm : 200);
        public double CanvasHeight => ChequeHeightMm * ScaleFactor;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private ObservableCollection<ChequeFieldViewModel> _fields = new();

        [ObservableProperty]
        private ChequeFieldViewModel? _selectedField;

        // Overlay properties (Phase 1)
        [ObservableProperty]
        private string? _overlayImagePath;

        [ObservableProperty]
        private double _overlayOpacity = 0.5;

        [ObservableProperty]
        private double _backgroundOpacity = 0.85;

        public TemplateDesignerViewModel(ITemplateService templateService, IBankService bankService)
        {
            _templateService = templateService;
            _bankService = bankService;
            InitializeDefaultFields();
        }

        private void InitializeDefaultFields()
        {
            Fields.Clear();
            Fields.Add(new ChequeFieldViewModel("dateDay", "Date (Day)", "DD", new FieldConfig { x = 152, y = 12, width = 12, height = 6 }));
            Fields.Add(new ChequeFieldViewModel("dateMonth", "Date (Month)", "MM", new FieldConfig { x = 164, y = 12, width = 12, height = 6 }));
            Fields.Add(new ChequeFieldViewModel("dateYear", "Date (Year)", "YYYY", new FieldConfig { x = 176, y = 12, width = 18, height = 6 }));
            Fields.Add(new ChequeFieldViewModel("payeeLine1", "Payee Line 1", "PAYEE NAME LINE", new FieldConfig { x = 35, y = 25, width = 150, height = 7 }));
            Fields.Add(new ChequeFieldViewModel("amountWordsLine1", "Amount (Words)", "** Sri Lanka Rupees Seventy-Five Thousand Only **", new FieldConfig { x = 12, y = 42, width = 165, height = 7 }));
            Fields.Add(new ChequeFieldViewModel("amountFigures", "Amount (Figures)", "**75,000.00**", new FieldConfig { x = 158, y = 42, width = 35, height = 8 }));
            Fields.Add(new ChequeFieldViewModel("crossingZone", "Crossing", "// A/C PAYEE ONLY //", new FieldConfig { x = 8, y = 5, width = 35, height = 18 }));
            Fields.Add(new ChequeFieldViewModel("memoLine", "Memo", "MEMO / NOTE", new FieldConfig { x = 12, y = 70, width = 100, height = 6 }));

            UpdateFieldScales();
        }

        public void UpdateCanvasDimensions(double newWidth)
        {
            if (newWidth > 100)
            {
                CanvasWidth = newWidth;
                OnPropertyChanged(nameof(ScaleFactor));
                OnPropertyChanged(nameof(CanvasHeight));
                UpdateFieldScales();
            }
        }

        private void UpdateFieldScales()
        {
            foreach (var field in Fields)
            {
                field.ScaleFactor = ScaleFactor;
            }
        }

        public async Task LoadDataAsync()
        {
            try
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
            catch (Exception ex)
            {
                StatusMessage = "Error loading template data: " + ex.Message;
            }
        }

        partial void OnSelectedTemplateChanged(BankTemplate? value)
        {
            if (value == null) return;

            try
            {
                BankName = value.BankName ?? string.Empty;
                SeriesName = value.SeriesName ?? string.Empty;
                ChequeWidthMm = (double)value.ChequeWidthMm;
                ChequeHeightMm = (double)value.ChequeHeightMm;
                TemplateImagePath = value.TemplateImagePath;

                if (!string.IsNullOrEmpty(TemplateImagePath))
                {
                    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    var absPath = System.IO.Path.Combine(baseDir, TemplateImagePath);
                    if (!System.IO.File.Exists(absPath))
                    {
                        absPath = System.IO.Path.Combine(baseDir, "template_image", System.IO.Path.GetFileName(TemplateImagePath));
                    }

                    FullImagePath = System.IO.File.Exists(absPath) ? new Uri(absPath).AbsoluteUri : null;
                }
                else
                {
                    FullImagePath = null;
                }

                if (!string.IsNullOrWhiteSpace(value.TemplateConfig))
                {
                    var cfg = JsonSerializer.Deserialize<TemplateConfigDto>(value.TemplateConfig);
                    if (cfg != null)
                    {
                        UpdateFieldModel("dateDay", cfg.dateDay);
                        UpdateFieldModel("dateMonth", cfg.dateMonth);
                        UpdateFieldModel("dateYear", cfg.dateYear);
                        UpdateFieldModel("payeeLine1", cfg.payeeLine1);
                        UpdateFieldModel("amountWordsLine1", cfg.amountWordsLine1);
                        UpdateFieldModel("amountFigures", cfg.amountFigures);
                        UpdateFieldModel("crossingZone", cfg.crossingZone);
                        UpdateFieldModel("memoLine", cfg.memoLine);
                    }
                }

                OnPropertyChanged(nameof(ScaleFactor));
                OnPropertyChanged(nameof(CanvasHeight));
                UpdateFieldScales();
            }
            catch
            {
                // Fallback gracefully
            }
        }

        private void UpdateFieldModel(string fieldId, FieldConfig? config)
        {
            if (config == null) return;
            var field = Fields.FirstOrDefault(f => f.FieldId == fieldId);
            if (field != null)
            {
                field.X = config.x;
                field.Y = config.y;
                field.Width = config.width;
                field.Height = config.height;
                field.Angle = config.angle;
                field.FontSize = config.fontSize > 0 ? config.fontSize : 11;
                field.FontWeight = string.IsNullOrEmpty(config.fontWeight) ? "Bold" : config.fontWeight;
            }
        }

        [RelayCommand]
        private async Task SaveTemplateAsync()
        {
            if (string.IsNullOrWhiteSpace(BankName) || string.IsNullOrWhiteSpace(SeriesName))
                return;

            var dto = new TemplateConfigDto
            {
                dateDay = Fields.FirstOrDefault(f => f.FieldId == "dateDay")?.GetModel(),
                dateMonth = Fields.FirstOrDefault(f => f.FieldId == "dateMonth")?.GetModel(),
                dateYear = Fields.FirstOrDefault(f => f.FieldId == "dateYear")?.GetModel(),
                payeeLine1 = Fields.FirstOrDefault(f => f.FieldId == "payeeLine1")?.GetModel(),
                amountWordsLine1 = Fields.FirstOrDefault(f => f.FieldId == "amountWordsLine1")?.GetModel(),
                amountFigures = Fields.FirstOrDefault(f => f.FieldId == "amountFigures")?.GetModel(),
                crossingZone = Fields.FirstOrDefault(f => f.FieldId == "crossingZone")?.GetModel(),
                memoLine = Fields.FirstOrDefault(f => f.FieldId == "memoLine")?.GetModel()
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
            InitializeDefaultFields();
            StatusMessage = "Creating new template...";
        }

        [RelayCommand]
        private void SelectField(ChequeFieldViewModel field)
        {
            foreach (var f in Fields) f.IsSelected = false;
            if (field != null)
            {
                field.IsSelected = true;
                SelectedField = field;
            }
        }
    }
}
