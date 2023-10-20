using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Formation.WPF
{
    public class CommandBase<T> :INotifyPropertyChanged
    {
        #region "INotifyPropertyChanged members"

        public event PropertyChangedEventHandler PropertyChanged;
        //This routine is called each time a property value has been set. 
        //This will //cause an event to notify WPF via data-binding that a change has occurred. 
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private ObservableCollection<T> collection;
        public ObservableCollection<T> Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = new ObservableCollection<T>();
                }
                return collection;
            }
            set { collection = value; OnPropertyChanged("Collection");}
        }

      
        private ICommand getCommand;
        private ICommand saveCommand;
        private ICommand removeCommand;
    
        public ICommand GetCommand
        {
            get
            {
                return getCommand ?? (getCommand = new RelayCommand(param => Get(),param=>CanGet()));
            }
        }
        protected virtual bool CanGet()
        {
            return true;
        }

        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new RelayCommand(param => Save()));
            }
        }
        protected virtual bool CanSave()
        {
            return true;
        }

        public ICommand DeleteCommand
        {
            get
            {
                return removeCommand ?? (removeCommand = new RelayCommand(param=>Delete()));
            }
        }
        protected virtual bool CanDelete()
        {
            return true;
        }

        protected virtual async Task Get()
        {
            throw new NotImplementedException();
        }
        protected virtual void Save()
        {
            throw new NotImplementedException();
        }
        protected virtual void Delete()
        {
            throw new NotImplementedException();
        }

     
    }
}
