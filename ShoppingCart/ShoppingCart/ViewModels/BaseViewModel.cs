﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace habahabamall.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Private

        private bool isBusy;

        #endregion

        #region Properties

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed == null)
            {
                return;
            }

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}