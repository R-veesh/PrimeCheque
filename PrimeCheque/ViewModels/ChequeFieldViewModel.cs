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

        public ChequeFieldViewModel(string fieldId, string displayName, string sampleText, FieldConfig model)
        {
            FieldId = fieldId;
            DisplayName = displayName;
            SampleText = sampleText;
            _model = model;
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

        public double PxX => X * ScaleFactor;
        public double PxY => Y * ScaleFactor;
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

        public void ApplyAngleDelta(double dPxX, double dPxY)
        {
            double centerPxX = PxCenterX;
            double centerPxY = PxCenterY;
            double currentAngle = System.Math.Atan2(dPxY, dPxX) * (180.0 / System.Math.PI);
            Angle = (float)currentAngle % 360;
        }
    }
}
