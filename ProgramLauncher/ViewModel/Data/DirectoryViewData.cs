using ProgramLauncher.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.ViewModel.Data
{
    public class DirectoryViewData : BaseNotifyPropertyChanged
    {
        #region Fields

        private readonly string _originalAbsolutePath;
        private string _absolutePath;


        /*
         * TODO: Add enable/disable feature?
         */

        #endregion

        #region Constructors

        public DirectoryViewData(string absolutePath)
        {
            this._originalAbsolutePath = absolutePath;
            this._absolutePath = absolutePath;
        }

        #endregion

        #region Properties

        public string AbsolutePath
        {
            get { return this._absolutePath; }
            set { this.SetProperty(value, ref this._absolutePath); }
        }

        public bool PathValid
        {
            get { return Directory.Exists(this._absolutePath); }
        }

        /*
         * TODO: Don't do this here...
         */
        public System.Windows.Media.Brush TextColor
        {
            get
            {
                if (this.PathValid)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else
                {
                    return System.Windows.Media.Brushes.Red;
                }
            }
        }

        public bool Modified
        {
            get
            {
                return
                    string.IsNullOrEmpty(this._originalAbsolutePath) ?
                        true :
                        !this._originalAbsolutePath.Equals(this._absolutePath);
            }
        }

        #endregion

        #region Protected Overridden Methods

        protected override void HandleInternalPropertyChanged(string property)
        {
            // Notify that our path changed.
            /*
             * TODO: Can we get rid of this magic string????
             */
            if (property == "AbsolutePath")
            {
                this.OnPropertyChanged("PathValid");
                this.OnPropertyChanged("Modified");
            }

            if (property == "PathValid")
            {
                this.OnPropertyChanged("TextColor");
            }
        }

        #endregion
        
    }
}
