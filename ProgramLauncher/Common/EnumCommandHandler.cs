using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProgramLauncher.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEnum">Enumeration for command.</typeparam>
    public class EnumCommandHandler<TEnum> : ICommand
        where TEnum : struct
    {

        #region Fields

        private readonly Func<TEnum, bool> _canExecuteCallback;
        private readonly Action<TEnum> _executeCallback;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor to verify that this is an enumeration type.
        /// </summary>
        static EnumCommandHandler()
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an Enumeration type!");
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="canExecuteCallback"></param>
        /// <param name="executeCallback"></param>
        public EnumCommandHandler(Func<TEnum, bool> canExecuteCallback, Action<TEnum> executeCallback)
        {
            this._canExecuteCallback = canExecuteCallback;
            this._executeCallback = executeCallback;
        }

        #endregion

        #region Events

        /// <summary>
        /// Event to notify listeners of execution status changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion

        #region Public Methods

        /// <summary>
        /// Fires the CanExecute changed event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event parameters.</param>
        public void FireCanExecuteChanged(object sender, EventArgs args)
        {
            this.CanExecuteChanged(sender, args);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (parameter is TEnum)
            {
                canExecute = this._canExecuteCallback((TEnum)parameter);
            }
            else if (null != parameter)
            {
                Logger.LogError("Called on unknown parameter type " + parameter);
            }
            else
            {
                Logger.LogError("Called on null parameter.");
            }

            return canExecute;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (parameter is TEnum)
            {
                this._executeCallback((TEnum)parameter);
            }
            else if (null != parameter)
            {
                Logger.LogError("Called on unknown parameter type " + parameter);
            }
            else
            {
                Logger.LogError("Called on null parameter.");
            }
        }

        #endregion
    }
}
