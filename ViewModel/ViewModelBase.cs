﻿using MusikPlayer.Enum;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace MusikPlayer.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public const string DEFAULT_STRING = "";
        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Dictionary<string, object> Storage { get; set; } = new Dictionary<string, object>();

        public T GetProperty<T>(string propertyName, T fallback = default)
        {
            T result = fallback;
            if (Storage.ContainsKey(propertyName))
                result = (T)Storage[propertyName];
            else
                SetProperty(propertyName, fallback);

            return result;
        }

        public void SetProperty<T>(string propertyName, T value)
        {

            if (!Storage.ContainsKey(propertyName))
                Storage.Add(propertyName, value);
            else
                Storage[propertyName] = value;

            OnPropertyChanged(propertyName);
        }
    }
}
