using ProgramLauncher.Common;
using ProgramLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.View
{
    public class FileDataView : BaseViewModel
    {

        private readonly FileData _fileData;
        private int _val;

        public FileDataView(FileData fileData)
        {
            this._fileData = fileData;
            this._val = -1000;
        }

        public FileData FileDataProperty
        {
            get { return this._fileData; }
        }

        public int val
        {
            get
            {
                return this._val;
            }

            set
            {
                // this.SetProperty(value, ref this._val);
                if (this._val != value)
                {
                    this._val = value;
                    this.OnPropertyChanged("val");
                }
            }
        }

        protected override void HandleInternalPropertyChanged(string property)
        {
            
        }
    }
}
