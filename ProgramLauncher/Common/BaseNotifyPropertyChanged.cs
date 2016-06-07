using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.Common
{
    public abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Protected Abstract Methods

        protected abstract void HandleInternalPropertyChanged(string property);

        #endregion

        #region Protected Methods

        protected void SetProperty<T>(T newValue, ref T currentValue, [CallerMemberName] string callerName = "")
            where T : class
        {
            if (null != currentValue)
            {
                if (false == currentValue.Equals(newValue))
                {
                    currentValue = newValue;
                    this.OnPropertyChanged(callerName);
                }
            }
            else
            {
                if (null != newValue)
                {
                    currentValue = newValue;
                    this.OnPropertyChanged(callerName);
                }
            }
        }

        protected void SetProperty(bool newValue, ref bool currentValue, [CallerMemberName] string callerName = "")
        {
            if (newValue != currentValue)
            {
                currentValue = newValue;
                this.OnPropertyChanged(callerName);
            }
        }

        protected virtual void OnPropertyChanged(string property)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            this.HandleInternalPropertyChanged(property);
        }

        #endregion
    }
}
