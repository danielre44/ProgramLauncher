using ProgramLauncher.Common;
using ProgramLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Data;

namespace ProgramLauncher.View
{
    public class BasicFileViewModel : BaseViewModel
    {

        #region Fields

        private readonly Dictionary<FileData, FileViewData> _fileDataViewMap;
        private readonly ObservableCollection<FileViewData> _fileDataList;
        
        private string _inputText;
        
        #endregion

        #region Constructors

        public BasicFileViewModel(FileModel fileModel)
        {
            this._fileDataViewMap = new Dictionary<FileData, FileViewData>();
            this._fileDataList = new ObservableCollection<FileViewData>();
            this._inputText = string.Empty;

            fileModel.FileAdded += this.FileAddedHandler;
            fileModel.FileRemoved += this.FileRemovedHandler;
        }

        #endregion

        #region Properties

        public ObservableCollection<FileViewData> FileDataList
        {
            get
            {
                return this._fileDataList;
            }
        }

        public string InputText
        {
            get
            {
                return _inputText;
            }

            set
            {
                this.SetProperty(value, ref this._inputText);
            }
        }


        #endregion

        #region Protected BaseViewModel Methods

        protected override void HandleInternalPropertyChanged(string property)
        {
            Logger.LogTrace("Property changed: " + property);

            if (property.Equals(nameof(this.InputText)))
            {
                
                /*
                 * TODO
                 */
            }
        }

        #endregion

        #region Private Methods

        private void FileAddedHandler(FileData fileData)
        {
            FileViewData fileDataView = new FileViewData(fileData);

            this._fileDataViewMap.Add(fileData, fileDataView);
            this._fileDataList.Add(fileDataView);
        }

        private void FileRemovedHandler(FileData fileData)
        {
            if (this._fileDataViewMap.ContainsKey(fileData))
            {
                this._fileDataList.Remove(this._fileDataViewMap[fileData]);
                this._fileDataViewMap.Remove(fileData);
            }
        }

        #endregion


    }
}
