using System;

namespace PrimeCheque.Helpers
{
    public static class PrintCalibrationHelper
    {
        private const double MmPerInch = 25.4;
        private const double PointsPerInch = 72.0;

        /// <summary>
        /// Converts millimetres to screen pixels based on standard DPI (default 96).
        /// </summary>
        public static double MmToPixels(double mm, double dpi = 96.0)
        {
            return (mm / MmPerInch) * dpi;
        }

        /// <summary>
        /// Converts pixels to millimetres based on standard DPI (default 96).
        /// </summary>
        public static double PixelsToMm(double pixels, double dpi = 96.0)
        {
            return (pixels / dpi) * MmPerInch;
        }

        /// <summary>
        /// Converts millimetres to PDF points (1/72 inch).
        /// </summary>
        public static double MmToPoints(double mm)
        {
            return (mm / MmPerInch) * PointsPerInch;
        }

        /// <summary>
        /// Converts PDF points (1/72 inch) to millimetres.
        /// </summary>
        public static double PointsToMm(double points)
        {
            return (points / PointsPerInch) * MmPerInch;
        }

        /// <summary>
        /// Applies printer calibration offset adjustments to base mm coordinates.
        /// </summary>
        public static (double CalibratedX, double CalibratedY) CalculateCalibratedCoordinates(
            double baseX, 
            double baseY, 
            decimal horizontalOffsetMm, 
            decimal verticalOffsetMm)
        {
            var calibratedX = baseX + (double)horizontalOffsetMm;
            var calibratedY = baseY + (double)verticalOffsetMm;

            // Ensure coordinates don't go negative
            calibratedX = Math.Max(0.0, calibratedX);
            calibratedY = Math.Max(0.0, calibratedY);

            return (calibratedX, calibratedY);
        }

        /// <summary>
        /// Validates that field bounding box fits within cheque dimensions.
        /// </summary>
        public static bool IsWithinBounds(
            double fieldX, 
            double fieldY, 
            double fieldWidth, 
            double fieldHeight, 
            double chequeWidthMm, 
            double chequeHeightMm)
        {
            if (fieldX < 0 || fieldY < 0) return false;
            if (fieldX + fieldWidth > chequeWidthMm) return false;
            if (fieldY + fieldHeight > chequeHeightMm) return false;
            return true;
        }
    }
}
