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
        private readonly IPrintService _printService;

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

        partial void OnChequeWidthMmChanged(double value)
        {
            OnPropertyChanged(nameof(ScaleFactor));
            OnPropertyChanged(nameof(CanvasHeight));
            UpdateFieldScales();
        }

        [ObservableProperty]
        private double _chequeHeightMm = 88;

        partial void OnChequeHeightMmChanged(double value)
        {
            OnPropertyChanged(nameof(ScaleFactor));
            OnPropertyChanged(nameof(CanvasHeight));
            UpdateFieldScales();
        }

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

        // Phase 1 Additional properties
        [ObservableProperty]
        private ObservableCollection<string> _sampleOverlays = new();

        [ObservableProperty]
        private string? _selectedSampleOverlay;

        // Phase 4 Calibration properties
        [ObservableProperty]
        private ObservableCollection<string> _installedPrinters = new();

        [ObservableProperty]
        private string? _selectedPrinter;

        [ObservableProperty]
        private bool _showCalibrationOffsets;

        [ObservableProperty]
        private bool _printLandscape;

        [ObservableProperty]
        private string _printPosition = "Top";

        public ObservableCollection<string> PrintPositions { get; } = new() { "Top", "Middle", "Bottom" };

        [ObservableProperty]
        private string _printPositionHorizontal = "Left";

        public ObservableCollection<string> PrintPositionHorizontals { get; } = new() { "Left", "Center", "Right" };

        [ObservableProperty]
        private double _calibrationHOffset = 0;
        partial void OnCalibrationHOffsetChanged(double value) => UpdateFieldScales();

        [ObservableProperty]
        private double _calibrationVOffset = 0;
        partial void OnCalibrationVOffsetChanged(double value) => UpdateFieldScales();
        
        partial void OnShowCalibrationOffsetsChanged(bool value) => UpdateFieldScales();

        public TemplateDesignerViewModel(ITemplateService templateService, IBankService bankService, IPrintService printService)
        {
            _templateService = templateService;
            _bankService = bankService;
            _printService = printService;
            InitializeDefaultFields();
        }

        private void InitializeDefaultFields()
        {
            Fields.Clear();
            Fields.Add(new ChequeFieldViewModel("dateD1", "Date D1", "D", new FieldConfig { x = 152, y = 12, width = 6, height = 6 }, this));
            Fields.Add(new ChequeFieldViewModel("dateD2", "Date D2", "D", new FieldConfig { x = 158, y = 12, width = 6, height = 6 }, this));
            Fields.Add(new ChequeFieldViewModel("dateM1", "Date M1", "M", new FieldConfig { x = 164, y = 12, width = 6, height = 6 }, this));
            Fields.Add(new ChequeFieldViewModel("dateM2", "Date M2", "M", new FieldConfig { x = 170, y = 12, width = 6, height = 6 }, this));
            Fields.Add(new ChequeFieldViewModel("dateY1", "Date Y1", "Y", new FieldConfig { x = 176, y = 12, width = 6, height = 6 }, this));
            Fields.Add(new ChequeFieldViewModel("dateY2", "Date Y2", "Y", new FieldConfig { x = 182, y = 12, width = 6, height = 6 }, this));
            Fields.Add(new ChequeFieldViewModel("dateY3", "Date Y3", "Y", new FieldConfig { x = 188, y = 12, width = 6, height = 6 }, this));
            Fields.Add(new ChequeFieldViewModel("dateY4", "Date Y4", "Y", new FieldConfig { x = 194, y = 12, width = 6, height = 6 }, this));
            Fields.Add(new ChequeFieldViewModel("payeeLine1", "Payee Line 1", "PAYEE NAME LINE 1", new FieldConfig { x = 35, y = 25, width = 150, height = 7 }, this));
            Fields.Add(new ChequeFieldViewModel("payeeLine2", "Payee Line 2", "PAYEE NAME LINE 2", new FieldConfig { x = 35, y = 32, width = 150, height = 7 }, this));
            Fields.Add(new ChequeFieldViewModel("amountWordsLine1", "Amount (Words) 1", "** Sri Lanka Rupees Twenty Six", new FieldConfig { x = 12, y = 42, width = 90, height = 7 }, this));
            Fields.Add(new ChequeFieldViewModel("amountWordsLine2", "Amount (Words) 2", "Million Seven Hundred and Ninety", new FieldConfig { x = 12, y = 50, width = 100, height = 7 }, this));
            Fields.Add(new ChequeFieldViewModel("amountWordsLine3", "Amount (Words) 3", "Five Thousand Only **", new FieldConfig { x = 12, y = 58, width = 100, height = 7 }, this));
            Fields.Add(new ChequeFieldViewModel("amountFigures", "Amount (Figures)", "**75,000.00**", new FieldConfig { x = 158, y = 42, width = 35, height = 8 }, this));
            Fields.Add(new ChequeFieldViewModel("crossingZone", "Crossing", "A/C PAYEE ONLY", new FieldConfig { x = 8, y = 5, width = 35, height = 18 }, this));
            Fields.Add(new ChequeFieldViewModel("memoLine", "Memo", "MEMO / NOTE", new FieldConfig { x = 12, y = 70, width = 100, height = 6 }, this));
            Fields.Add(new ChequeFieldViewModel("orBearerZone", "Or Bearer Strike", "XXXX", new FieldConfig { x = 160, y = 25, width = 25, height = 5 }, this));

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

                // Load Sample Overlays
                SampleOverlays.Clear();
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var sampleDir = System.IO.Path.Combine(baseDir, "outPutSamplePdf");
                if (System.IO.Directory.Exists(sampleDir))
                {
                    var files = System.IO.Directory.GetFiles(sampleDir, "*.*")
                        .Where(f => f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) || 
                                    f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                    f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                        .Select(f => System.IO.Path.GetFileName(f));
                    
                    foreach (var f in files)
                    {
                        SampleOverlays.Add(f);
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Error loading template data: " + ex.Message;
            }

            // Load installed printers
            try
            {
                InstalledPrinters.Clear();
                var printers = _printService.GetInstalledPrinters();
                foreach (var p in printers) InstalledPrinters.Add(p);
                if (InstalledPrinters.Count > 0)
                {
                    var defaultPrinter = new System.Drawing.Printing.PrinterSettings().PrinterName;
                    SelectedPrinter = InstalledPrinters.FirstOrDefault(p => p.Equals(defaultPrinter, StringComparison.OrdinalIgnoreCase))
                        ?? InstalledPrinters[0];
                }
            }
            catch
            {
                // Printer enumeration may fail on some systems
            }
        }

        partial void OnSelectedPrinterChanged(string? value)
        {
            if (value != null)
            {
                _ = LoadCalibrationForPrinterAsync(value);
            }
        }

        private async Task LoadCalibrationForPrinterAsync(string printerName)
        {
            var cal = await _printService.GetCalibrationAsync(printerName, SelectedTemplate?.Id);
            if (cal != null)
            {
                CalibrationHOffset = (double)cal.HorizontalOffsetMm;
                CalibrationVOffset = (double)cal.VerticalOffsetMm;
                PrintLandscape = cal.PrintLandscape;
                PrintPosition = string.IsNullOrEmpty(cal.PrintPosition) ? "Top" : cal.PrintPosition;
                PrintPositionHorizontal = string.IsNullOrEmpty(cal.PrintPositionHorizontal) ? "Left" : cal.PrintPositionHorizontal;
            }
            else
            {
                CalibrationHOffset = 0;
                CalibrationVOffset = 0;
                PrintLandscape = false;
                PrintPosition = "Top";
                PrintPositionHorizontal = "Left";
            }
        }

        [RelayCommand]
        private async Task SaveCalibrationAsync()
        {
            if (string.IsNullOrEmpty(SelectedPrinter))
            {
                StatusMessage = "Please select a printer first.";
                return;
            }

            var cal = new PrinterCalibration
            {
                PrinterName = SelectedPrinter,
                HorizontalOffsetMm = (decimal)CalibrationHOffset,
                VerticalOffsetMm = (decimal)CalibrationVOffset,
                PrintLandscape = PrintLandscape,
                PrintPosition = PrintPosition,
                PrintPositionHorizontal = PrintPositionHorizontal,
                TemplateId = SelectedTemplate?.Id
            };
            await _printService.SaveCalibrationAsync(cal);
            StatusMessage = "✅ Calibration saved for " + SelectedPrinter;
        }

        [RelayCommand]
        private async Task PrintTestPageAsync()
        {
            if (string.IsNullOrEmpty(SelectedPrinter) || SelectedTemplate == null)
            {
                StatusMessage = "Select a printer and template first.";
                return;
            }

            try
            {
                StatusMessage = "Generating test page...";

                // Create a sample cheque for the test print
                var testCheque = new Cheque
                {
                    ChequeNumber = 999999,
                    PayeeName = "TEST PRINT - Calibration Check",
                    Amount = 12345.67m,
                    AmountInWords = "** Test Twelve Thousand Three Hundred and Forty Five and Cents Sixty Seven Only **",
                    ChequeDate = DateOnly.FromDateTime(DateTime.Now),
                    Memo = "Calibration Test",
                    CrossingType = CrossingType.AccountPayeeOnly
                };

                var calibration = new PrinterCalibration
                {
                    PrinterName = SelectedPrinter,
                    HorizontalOffsetMm = (decimal)CalibrationHOffset,
                    VerticalOffsetMm = (decimal)CalibrationVOffset,
                    PrintLandscape = PrintLandscape,
                    PrintPosition = PrintPosition,
                    PrintPositionHorizontal = PrintPositionHorizontal,
                    TemplateId = SelectedTemplate.Id
                };

                var pdfService = App.GetService<IPdfGenerationService>();
                var pdfPath = await pdfService.GenerateChequePdfAsync(testCheque, SelectedTemplate, calibration, "TEST PRINT");
                var printed = await _printService.PrintPdfAsync(pdfPath, SelectedPrinter, calibration);

                StatusMessage = printed ? "✅ Test page sent to " + SelectedPrinter : "❌ Print failed.";
            }
            catch (Exception ex)
            {
                StatusMessage = "❌ Test print error: " + ex.Message;
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
                        UpdateFieldModel("dateD1", cfg.dateD1);
                        UpdateFieldModel("dateD2", cfg.dateD2);
                        UpdateFieldModel("dateM1", cfg.dateM1);
                        UpdateFieldModel("dateM2", cfg.dateM2);
                        UpdateFieldModel("dateY1", cfg.dateY1);
                        UpdateFieldModel("dateY2", cfg.dateY2);
                        UpdateFieldModel("dateY3", cfg.dateY3);
                        UpdateFieldModel("dateY4", cfg.dateY4);
                        UpdateFieldModel("payeeLine1", cfg.payeeLine1);
                        UpdateFieldModel("payeeLine2", cfg.payeeLine2);
                        UpdateFieldModel("amountWordsLine1", cfg.amountWordsLine1);
                        UpdateFieldModel("amountWordsLine2", cfg.amountWordsLine2);
                        UpdateFieldModel("amountWordsLine3", cfg.amountWordsLine3);
                        UpdateFieldModel("amountFigures", cfg.amountFigures);
                        UpdateFieldModel("crossingZone", cfg.crossingZone);
                        UpdateFieldModel("memoLine", cfg.memoLine);
                        UpdateFieldModel("orBearerZone", cfg.orBearerZone);
                    }
                }

                if (value.BankId.HasValue)
                {
                    SelectedBank = Banks.FirstOrDefault(b => b.Id == value.BankId.Value);
                }
                else
                {
                    SelectedBank = Banks.FirstOrDefault(b => b.Name.Equals(value.BankName, StringComparison.OrdinalIgnoreCase));
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
                dateD1 = Fields.FirstOrDefault(f => f.FieldId == "dateD1")?.GetModel(),
                dateD2 = Fields.FirstOrDefault(f => f.FieldId == "dateD2")?.GetModel(),
                dateM1 = Fields.FirstOrDefault(f => f.FieldId == "dateM1")?.GetModel(),
                dateM2 = Fields.FirstOrDefault(f => f.FieldId == "dateM2")?.GetModel(),
                dateY1 = Fields.FirstOrDefault(f => f.FieldId == "dateY1")?.GetModel(),
                dateY2 = Fields.FirstOrDefault(f => f.FieldId == "dateY2")?.GetModel(),
                dateY3 = Fields.FirstOrDefault(f => f.FieldId == "dateY3")?.GetModel(),
                dateY4 = Fields.FirstOrDefault(f => f.FieldId == "dateY4")?.GetModel(),
                payeeLine1 = Fields.FirstOrDefault(f => f.FieldId == "payeeLine1")?.GetModel(),
                payeeLine2 = Fields.FirstOrDefault(f => f.FieldId == "payeeLine2")?.GetModel(),
                amountWordsLine1 = Fields.FirstOrDefault(f => f.FieldId == "amountWordsLine1")?.GetModel(),
                amountWordsLine2 = Fields.FirstOrDefault(f => f.FieldId == "amountWordsLine2")?.GetModel(),
                amountWordsLine3 = Fields.FirstOrDefault(f => f.FieldId == "amountWordsLine3")?.GetModel(),
                amountFigures = Fields.FirstOrDefault(f => f.FieldId == "amountFigures")?.GetModel(),
                crossingZone = Fields.FirstOrDefault(f => f.FieldId == "crossingZone")?.GetModel(),
                memoLine = Fields.FirstOrDefault(f => f.FieldId == "memoLine")?.GetModel(),
                orBearerZone = Fields.FirstOrDefault(f => f.FieldId == "orBearerZone")?.GetModel()
            };

            var jsonConfig = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });

            var tmpl = SelectedTemplate ?? new BankTemplate();
            tmpl.BankName = BankName.Trim();
            tmpl.SeriesName = SeriesName.Trim();
            tmpl.ChequeWidthMm = (decimal)ChequeWidthMm;
            tmpl.ChequeHeightMm = (decimal)ChequeHeightMm;
            tmpl.TemplateConfig = jsonConfig;
            tmpl.TemplateImagePath = TemplateImagePath;

            var matchedBank = SelectedBank 
                ?? Banks.FirstOrDefault(b => b.Name.Equals(tmpl.BankName, StringComparison.OrdinalIgnoreCase))
                ?? Banks.FirstOrDefault(b => b.ShortName.Equals(tmpl.BankName, StringComparison.OrdinalIgnoreCase))
                ?? (SelectedTemplate?.BankId.HasValue == true ? Banks.FirstOrDefault(b => b.Id == SelectedTemplate.BankId.Value) : null);

            tmpl.BankId = matchedBank?.Id ?? SelectedTemplate?.BankId ?? tmpl.BankId;
            tmpl.IsDefault = SelectedTemplate?.IsDefault ?? true;

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

        partial void OnSelectedSampleOverlayChanged(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                OverlayImagePath = null;
                return;
            }

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = System.IO.Path.Combine(baseDir, "outPutSamplePdf", value);
            
            // Note: If it's a PDF, we'd theoretically need to render it to image first.
            // Since this is just path selection, the view will bind to OverlayImagePath. 
            // If it's a PDF and UWP Image can't render it directly, it won't show.
            // For full support, we should invoke a service to render PDF -> Image here.
            // But we will assume the selected sample could just be the path for now.
            // We can enhance it later if we add a background task to render it.
            OverlayImagePath = System.IO.File.Exists(path) ? new Uri(path).AbsoluteUri : null;
        }

        [RelayCommand]
        private async Task GeneratePrintPreviewAsync()
        {
            try
            {
                StatusMessage = "Generating preview...";
                
                // Create a temporary Cheque object for preview
                var dummyCheque = new Cheque
                {
                    ChequeNumber = 123456,
                    ChequeDate = DateOnly.FromDateTime(DateTime.Now),
                    PayeeName = "SAMPLE PAYEE NAME",
                    Amount = 75000.00m,
                    AmountInWords = "** Sri Lanka Rupees Seventy-Five Thousand Only **",
                    Memo = "PREVIEW MEMO",
                    CrossingType = PrimeCheque.Models.CrossingType.AccountPayeeOnly
                };

                var tmpl = SelectedTemplate ?? new BankTemplate();
                tmpl.ChequeWidthMm = (decimal)ChequeWidthMm;
                tmpl.ChequeHeightMm = (decimal)ChequeHeightMm;
                
                var dto = new TemplateConfigDto
                {
                    dateD1 = Fields.FirstOrDefault(f => f.FieldId == "dateD1")?.GetModel(),
                    dateD2 = Fields.FirstOrDefault(f => f.FieldId == "dateD2")?.GetModel(),
                    dateM1 = Fields.FirstOrDefault(f => f.FieldId == "dateM1")?.GetModel(),
                    dateM2 = Fields.FirstOrDefault(f => f.FieldId == "dateM2")?.GetModel(),
                    dateY1 = Fields.FirstOrDefault(f => f.FieldId == "dateY1")?.GetModel(),
                    dateY2 = Fields.FirstOrDefault(f => f.FieldId == "dateY2")?.GetModel(),
                    dateY3 = Fields.FirstOrDefault(f => f.FieldId == "dateY3")?.GetModel(),
                    dateY4 = Fields.FirstOrDefault(f => f.FieldId == "dateY4")?.GetModel(),
                    payeeLine1 = Fields.FirstOrDefault(f => f.FieldId == "payeeLine1")?.GetModel(),
                    amountWordsLine1 = Fields.FirstOrDefault(f => f.FieldId == "amountWordsLine1")?.GetModel(),
                    amountFigures = Fields.FirstOrDefault(f => f.FieldId == "amountFigures")?.GetModel(),
                    crossingZone = Fields.FirstOrDefault(f => f.FieldId == "crossingZone")?.GetModel(),
                    memoLine = Fields.FirstOrDefault(f => f.FieldId == "memoLine")?.GetModel(),
                    orBearerZone = Fields.FirstOrDefault(f => f.FieldId == "orBearerZone")?.GetModel()
                };
                tmpl.TemplateConfig = JsonSerializer.Serialize(dto);

                PrinterCalibration? calibration = null;
                if (ShowCalibrationOffsets)
                {
                    calibration = new PrinterCalibration
                    {
                        HorizontalOffsetMm = (decimal)CalibrationHOffset,
                        VerticalOffsetMm = (decimal)CalibrationVOffset,
                        PrintLandscape = PrintLandscape
                    };
                }

                // Since we don't have PdfGenerationService injected, we can resolve it
                var pdfService = App.GetService<IPdfGenerationService>();
                var pdfPath = await pdfService.GenerateChequePdfAsync(dummyCheque, tmpl, calibration, "PREVIEW");

                StatusMessage = "Preview generated successfully!";
                
                // Open the PDF
                if (System.IO.File.Exists(pdfPath))
                {
                    await Windows.System.Launcher.LaunchFileAsync(await Windows.Storage.StorageFile.GetFileFromPathAsync(pdfPath));
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Preview error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task TestSavePdfAsync()
        {
            try
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                
                // Get the window handle for the picker
                var window = App.MainWindow;
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("PDF Document", new System.Collections.Generic.List<string>() { ".pdf" });
                savePicker.SuggestedFileName = "Test_Print_Cheque.pdf";

                var file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    StatusMessage = "Generating PDF...";
                    
                    var dummyCheque = new Cheque
                    {
                        ChequeNumber = 123456,
                        ChequeDate = DateOnly.FromDateTime(DateTime.Now),
                        PayeeName = "SAMPLE PAYEE NAME",
                        Amount = 75000.00m,
                        AmountInWords = "** Sri Lanka Rupees Seventy-Five Thousand Only **",
                        Memo = "TEST SAVE MEMO",
                        CrossingType = PrimeCheque.Models.CrossingType.AccountPayeeOnly
                    };

                    var tmpl = SelectedTemplate ?? new BankTemplate();
                    tmpl.ChequeWidthMm = (decimal)ChequeWidthMm;
                    tmpl.ChequeHeightMm = (decimal)ChequeHeightMm;
                    
                    var dto = new TemplateConfigDto
                    {
                        dateD1 = Fields.FirstOrDefault(f => f.FieldId == "dateD1")?.GetModel(),
                        dateD2 = Fields.FirstOrDefault(f => f.FieldId == "dateD2")?.GetModel(),
                        dateM1 = Fields.FirstOrDefault(f => f.FieldId == "dateM1")?.GetModel(),
                        dateM2 = Fields.FirstOrDefault(f => f.FieldId == "dateM2")?.GetModel(),
                        dateY1 = Fields.FirstOrDefault(f => f.FieldId == "dateY1")?.GetModel(),
                        dateY2 = Fields.FirstOrDefault(f => f.FieldId == "dateY2")?.GetModel(),
                        dateY3 = Fields.FirstOrDefault(f => f.FieldId == "dateY3")?.GetModel(),
                        dateY4 = Fields.FirstOrDefault(f => f.FieldId == "dateY4")?.GetModel(),
                        payeeLine1 = Fields.FirstOrDefault(f => f.FieldId == "payeeLine1")?.GetModel(),
                        amountWordsLine1 = Fields.FirstOrDefault(f => f.FieldId == "amountWordsLine1")?.GetModel(),
                        amountFigures = Fields.FirstOrDefault(f => f.FieldId == "amountFigures")?.GetModel(),
                        crossingZone = Fields.FirstOrDefault(f => f.FieldId == "crossingZone")?.GetModel(),
                        memoLine = Fields.FirstOrDefault(f => f.FieldId == "memoLine")?.GetModel(),
                        orBearerZone = Fields.FirstOrDefault(f => f.FieldId == "orBearerZone")?.GetModel()
                    };
                    tmpl.TemplateConfig = JsonSerializer.Serialize(dto);

                    PrinterCalibration? calibration = null;
                    if (ShowCalibrationOffsets)
                    {
                        calibration = new PrinterCalibration
                        {
                            HorizontalOffsetMm = (decimal)CalibrationHOffset,
                            VerticalOffsetMm = (decimal)CalibrationVOffset,
                            PrintLandscape = PrintLandscape
                        };
                    }

                    var pdfService = App.GetService<IPdfGenerationService>();
                    var tempPdfPath = await pdfService.GenerateChequePdfAsync(dummyCheque, tmpl, calibration, "TEST PRINT");

                    // Copy the generated PDF to the chosen location
                    var tempFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(tempPdfPath);
                    
                    // Windows.Storage.NameCollisionOption.ReplaceExisting is 1
                    await tempFile.CopyAndReplaceAsync(file);

                    StatusMessage = $"Saved to: {file.Name}";
                    
                    // Optionally open the saved file
                    await Windows.System.Launcher.LaunchFileAsync(file);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save PDF error: {ex.Message}";
            }
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
