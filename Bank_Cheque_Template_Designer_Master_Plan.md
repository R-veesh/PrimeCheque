# PrimeCheque: Professional Bank Cheque Template Designer Master Plan

## 1. Overview & Objectives

The goal is to upgrade the existing basic Template Designer in PrimeCheque into a **professional-grade, precision-focused Cheque Template Designer**. It will allow users to perfectly align cheque fields (position, size, rotation) to match physical cheques from Sri Lankan banks, using printed outputs from Chrysanth Cheque Writer (stored in `outPutSamplePdf`) as reference overlays.

### Key Objectives:
1. **Interactive Millimetre-Based Editing**: Replace the current static number-box driven UI with a fully interactive Drag-and-Drop canvas (resize, reposition, rotate).
2. **Overlay Comparison Engine**: Allow users to load an original blank cheque image and a reference printed cheque (e.g., from Chrysanth Cheque Writer) simultaneously, controlling their opacities to perfectly align the generated fields with the reference.
3. **Advanced Field Properties**: Introduce font size, font style, and **rotation/angle** parameters to handle skewed or angled cheque designs.
4. **Printer Calibration Integration**: Visually apply printer hardware offsets directly in the designer.
5. **Accurate Print Preview**: Provide a true-to-life PDF print preview before saving.

---

## 2. Core Features & Functional Requirements

### 2.1 Interactive Canvas (Drag, Drop, Resize, Rotate)
- **Selectable Elements**: Each cheque field (Date Day, Payee, Amount Words, etc.) should be a selectable visual element.
- **Drag & Drop**: Click and drag to move elements around the canvas. Coordinates translate back to millimetres (mm) in real-time.
- **Resize Handles**: Drag edges/corners to adjust the width and height bounds of text areas.
- **Rotation**: A rotation handle to angle the text (useful for crossing lines like `A/C PAYEE ONLY`).
- **Snapping & Guidelines**: Snap to grid or snap to other elements for precise alignment.

### 2.2 Overlay Comparison System
- **Layer 1: Blank Cheque Background** - The original physical cheque scan.
- **Layer 2: Reference Print Overlay** - A PDF/Image of a perfectly aligned printed cheque (from `outPutSamplePdf`).
- **Layer 3: Active Designer Fields** - The fields currently being configured in PrimeCheque.
- **Opacity Controls**: Sliders to adjust the transparency of Layer 1 and Layer 2 to easily spot misalignments between the PrimeCheque fields and the reference.

### 2.3 Enhanced Field Configuration (`TemplateConfigDto`)
Expand the existing JSON configuration to include:
- `Angle / Rotation` (degrees)
- `FontFamily`
- `FontSize`
- `FontWeight` (Bold/Normal)
- `LetterSpacing` (crucial for Date fields separated by boxes)

### 2.4 Printer Calibration
- Connect the `PrinterCalibration` model directly to the designer.
- Option to "Apply Calibration Offsets" visually in the designer to see how the print will physically shift on a specific printer.

---

## 3. Architecture & Code Changes

### 3.1 Model & Data Transfer Object Updates
**Location:** `PrimeCheque.Services.PdfGenerationService.cs` (or moved to a shared Models folder)
```csharp
public class FieldConfig
{
    public float x { get; set; }
    public float y { get; set; }
    public float width { get; set; }
    public float height { get; set; }
    public float fontSize { get; set; } = 11;
    public float angle { get; set; } = 0;           // NEW: Rotation in degrees
    public string fontWeight { get; set; } = "Bold"; // NEW: Bold, Normal, etc.
    public float letterSpacing { get; set; } = 0;   // NEW: For date boxes
}
```

### 3.2 View / UI Upgrades (`TemplateDesignerPage.xaml`)
- **Canvas Framework**: Upgrade from a standard `Canvas` to using Win2D (`CanvasControl`) or UWP/WinUI `UIElement.ManipulationMode` for smooth drag, scale, and rotate gestures.
- **Layers**:
  - `Image` (Reference Background) - Binding to Selected Bank Template Image.
  - `Image` (Reference Print Overlay) - Selectable from `outPutSamplePdf` folder.
  - `ItemsControl` (Editable Fields) - Bound to an `ObservableCollection<ChequeFieldViewModel>`.
- **Property Panel**: Move coordinate textboxes to a contextual property panel that updates based on the currently selected field on the canvas.

### 3.3 ViewModel Upgrades (`TemplateDesignerViewModel.cs`)
- Introduce a `ChequeFieldViewModel` to wrap each `FieldConfig`.
  - Properties: `X`, `Y`, `Width`, `Height`, `Angle`, `Name`, `SampleText`, `IsSelected`.
- Methods:
  - `LoadReferencePdf(string pdfPath)`: Converts reference PDF to image for overlay.
  - `OnFieldDragged(double deltaX, double deltaY)`
  - `OnFieldRotated(double deltaAngle)`
- Add `ReferenceOverlayImagePath` and `ReferenceOverlayOpacity` properties.

### 3.4 PDF Engine Upgrades (`PdfGenerationService.cs`)
QuestPDF supports rotation. Update the drawing logic:
```csharp
layers.Layer()
    .OffsetX(posX, Unit.Millimetre)
    .OffsetY(posY, Unit.Millimetre)
    .Width(fieldW, Unit.Millimetre)
    .Height(fieldH, Unit.Millimetre)
    .Rotate(cfg.angle) // Apply rotation
    .Text(txt =>
    {
        var span = txt.Span(text).FontSize(fontSz).FontColor(Colors.Black);
        if (cfg.fontWeight == "Bold") span.Bold();
        // Add Letter spacing if applicable
    });
```

---

## 4. Implementation Steps (Phases)

### Phase 1: Reference Overlay Integration
- [ ] Add a file picker to load reference PDFs from `outPutSamplePdf`.
- [ ] Convert the selected PDF page to an image (using Windows.Data.Pdf or similar).
- [ ] Add the reference image as an overlay layer in the XAML canvas.
- [ ] Add an opacity slider binding to the overlay layer's opacity.

### Phase 2: Interactive Drag & Drop Canvas
- [ ] Create `ChequeFieldViewModel` to manage individual field state (X, Y, W, H).
- [ ] Replace hardcoded XAML borders with an `ItemsControl` bound to the collection of fields.
- [ ] Implement `ManipulationDelta` events on the field templates to support dragging elements with the mouse/touch.
- [ ] Bind the real-time pixel coordinates back to the millimetre properties in the ViewModel.

### Phase 3: Resize and Rotation Handles
- [ ] Add resize adorner boxes to the edges of the selected field.
- [ ] Add a rotation handle above the field.
- [ ] Implement math to translate pixel rotation/scaling back to the `FieldConfig` mm constraints.

### Phase 4: PDF Engine & Calibration Integration
- [ ] Update `TemplateConfigDto` and `FieldConfig` classes.
- [ ] Update `PdfGenerationService.cs` to apply rotation and font weights.
- [ ] Add a toggle button in the designer: "Show Calibration Offsets". When enabled, add the selected printer's H/V offsets to the visual coordinates so the user can see the exact physical print position.
- [ ] Add a "Generate Print Preview" button that creates a temp PDF and displays it.

---

## 5. Bank Coverage Map
We will calibrate templates against the following outputs provided in `outPutSamplePdf`:
- Amana Bank
- Bank of Ceylon
- Cargills Bank
- Citi bank
- Commercial Bank of Ceylon
- DFCC Bank
- Hatton National Bank (HNB)
- HSBC Advance
- Nations Trust Bank (NTB)
- NDB
- Peoples Bank
- Public Bank
- Sampath Bank
- Seylan Bank
- Standard Chartered
- Union Bank

