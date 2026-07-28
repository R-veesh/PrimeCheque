using CommunityToolkit.Mvvm.ComponentModel;
using PrimeCheque.Models;

namespace PrimeCheque.ViewModels
{
    public partial class ChequeFieldViewModel : ObservableObject
    {
        private readonly FieldConfig _model;

        public string FieldId { get; }
        public string DisplayName { get; }
        public string SampleText { get; }

        private readonly TemplateDesignerViewModel? _parent;

        public ChequeFieldViewModel(string fieldId, string displayName, string sampleText, FieldConfig model, TemplateDesignerViewModel? parent = null)
        {
            FieldId = fieldId;
            DisplayName = displayName;
            SampleText = sampleText;
            _model = model;
            _parent = parent;
        }

        public float X
        {
            get => _model.x;
            set { _model.x = value; OnPropertyChanged(); OnPropertyChanged(nameof(PxX)); }
        }

        public float Y
        {
            get => _model.y;
            set { _model.y = value; OnPropertyChanged(); OnPropertyChanged(nameof(PxY)); }
        }

        public float Width
        {
            get => _model.width;
            set { _model.width = value; OnPropertyChanged(); OnPropertyChanged(nameof(PxWidth)); }
        }

        public float Height
        {
            get => _model.height;
            set { _model.height = value; OnPropertyChanged(); OnPropertyChanged(nameof(PxHeight)); }
        }

        public float Angle
        {
            get => _model.angle;
            set { _model.angle = value; OnPropertyChanged(); }
        }

        public float FontSize
        {
            get => _model.fontSize;
            set { _model.fontSize = value; OnPropertyChanged(); }
        }

        public string FontWeight
        {
            get => _model.fontWeight;
            set { _model.fontWeight = value; OnPropertyChanged(); }
        }

        private double _scaleFactor = 1.0;
        public double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (SetProperty(ref _scaleFactor, value))
                {
                    OnPropertyChanged(nameof(PxX));
                    OnPropertyChanged(nameof(PxY));
                    OnPropertyChanged(nameof(PxWidth));
                    OnPropertyChanged(nameof(PxHeight));
                    OnPropertyChanged(nameof(PxXPlusW));
                    OnPropertyChanged(nameof(PxYPlusH));
                    OnPropertyChanged(nameof(PxCenterX));
                    OnPropertyChanged(nameof(PxCenterY));
                    OnPropertyChanged(nameof(RotateHandleX));
                    OnPropertyChanged(nameof(RotateHandleY));
                }
            }
        }

        public double OffsetX => (_parent?.ShowCalibrationOffsets == true) ? _parent.CalibrationHOffset : 0;
        public double OffsetY => (_parent?.ShowCalibrationOffsets == true) ? _parent.CalibrationVOffset : 0;

        public double PxX => (X + OffsetX) * ScaleFactor;
        public double PxY => (Y + OffsetY) * ScaleFactor;
        public double PxWidth => Width * ScaleFactor;
        public double PxHeight => Height * ScaleFactor;
        public double PxXPlusW => PxX + PxWidth - 8;
        public double PxYPlusH => PxY + PxHeight - 8;
        public double PxCenterX => PxX + PxWidth / 2;
        public double PxCenterY => PxY + PxHeight / 2;
        public double RotateHandleX => PxCenterX - 8;
        public double RotateHandleY => PxY - 24;

        [ObservableProperty]
        private bool _isSelected;

        public FieldConfig GetModel() => _model;

        public void ApplyDelta(double dPxX, double dPxY)
        {
            X += (float)(dPxX / ScaleFactor);
            Y += (float)(dPxY / ScaleFactor);
        }

        public void ApplyResize(double dPxW, double dPxH)
        {
            Width = System.Math.Max(5, Width + (float)(dPxW / ScaleFactor));
            Height = System.Math.Max(3, Height + (float)(dPxH / ScaleFactor));
        }

        public void ApplyAngle(double pointerX, double pointerY)
        {
            double centerPxX = PxCenterX;
            double centerPxY = PxCenterY;
            
            // Calculate the angle between the vertical axis (up) and the pointer,
            // relative to the center of the field.
            double deltaX = pointerX - centerPxX;
            double deltaY = pointerY - centerPxY;
            
            // Atan2 takes (y, x). Note that screen coordinates have Y going down.
            // A point directly above the center has negative deltaY, deltaX = 0.
            // We want that to be 0 degrees.
            double currentAngle = (System.Math.Atan2(deltaY, deltaX) * (180.0 / System.Math.PI)) + 90.0;
            
            if (currentAngle < 0) currentAngle += 360.0;
            
            // Round to nearest degree for usability
            Angle = (float)System.Math.Round(currentAngle);
        }
    }
}
