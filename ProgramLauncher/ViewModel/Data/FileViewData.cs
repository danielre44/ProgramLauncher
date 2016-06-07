using ProgramLauncher.Common;
using ProgramLauncher.Model;
using ProgramLauncher.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.ViewModel.Data
{
    public class FileViewData : BaseNotifyPropertyChanged
    {
        #region Fields

        private readonly FileData _fileData;
        private int _stringCompareValue;

        #endregion

        #region Constructor

        public FileViewData(FileData fileData)
        {
            this._fileData = fileData;
            _stringCompareValue = 25; // TODO
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the FileName associated with this file.
        /// </summary>
        public string FileName
        {
            get { return _fileData.FileName; }
        }

        /// <summary>
        /// Gets the absolute path to this file.
        /// </summary>
        public string AbsoluteFilePath
        {
            get { return _fileData.AbsoluteFilePath; }
        }

        /// <summary>
        /// Gets the File Extension associated with the file.
        /// </summary>
        public string FileExtension
        {
            get { return _fileData.FileExtension; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int StringCompareValue
        {
            get { return _stringCompareValue; }
            set
            {
                if (value != StringCompareValue)
                {
                    StringCompareValue = value;
                    this.OnPropertyChanged(GetMethodName.GetCallingName());
                }
            }
        }


        protected override void HandleInternalPropertyChanged(string property)
        {
            // Nothing to do.
        }


        #endregion

    }
}
