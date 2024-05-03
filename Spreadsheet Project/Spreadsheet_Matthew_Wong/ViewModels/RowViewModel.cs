using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using ReactiveUI;
namespace HW4_Spreadsheet_Application.ViewModels
{
    public class RowViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="cells">new list of cells</param>
        public RowViewModel(List<CellViewModel> cells)
        {
            Cells = cells;
            foreach (var cell in Cells)
            {
                cell.PropertyChanged += CellOnPropertyChanged;
            }

            SelfReference = this;
        }

        /// <summary>
        /// gets the CellViewModel based on index
        /// </summary>
        /// <param name="index"></param>
        public CellViewModel this[int index] => Cells[index];

        /// <summary>
        /// When a CellViewModel in a RowViewModel style is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CellOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            var styleImpactingProperties = new List<string>
            {
                nameof(CellViewModel.IsSelected),
                nameof(CellViewModel.CanEdit),
                nameof(CellViewModel.BackgroundColor),
            };
            if (styleImpactingProperties.Contains(args.PropertyName))
            {
                FireChangedEvent();
            }
        }


        //Properties
        public List<CellViewModel> Cells { get; set; }
        public RowViewModel SelfReference { get; private set; }

        /// <summary>
        /// Fires an event to let other know a property has changed
        /// </summary>
        public void FireChangedEvent()
        {
            this.RaisePropertyChanged(nameof(SelfReference));
        }
    }    
}

