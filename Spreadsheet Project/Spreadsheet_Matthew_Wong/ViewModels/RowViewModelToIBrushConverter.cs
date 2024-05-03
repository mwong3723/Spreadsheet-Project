using System;
using System.Dynamic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using SpreadsheetEngine;

namespace HW4_Spreadsheet_Application.ViewModels
{
    public class RowViewModelToIBrushConverter : IValueConverter
    {
        private RowViewModel currentRow;
        private int currentRowIndex;

        /// <summary>
        /// Converts RowViewModel to SolidColorBrush
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush;

            if (value is not RowViewModel row)
            {
                return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
            }
            
            if (this.currentRow != row)
            {
                this.currentRow = row;
                this.currentRowIndex = 0;
            }
            
            if (this.currentRow.Cells[this.currentRowIndex].IsSelected) brush = new SolidColorBrush(0xff3393df);
            else brush = new SolidColorBrush(this.currentRow.Cells[this.currentRowIndex].BackgroundColor);
            
            this.currentRowIndex++;
            if (this.currentRowIndex >= this.currentRow.Cells.Count) this.currentRow = null;

            return brush;
        }

        /// <summary>
        /// Converts SolidBrushColor to RowViewModel
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        
    }
}

