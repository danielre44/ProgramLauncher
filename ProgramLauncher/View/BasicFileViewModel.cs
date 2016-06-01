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

        private readonly ObservableCollection<FileData> _FIXED_fileDataList;

        private readonly ObservableCollection<FileData> _fileDataList;

        private ICollectionView _fileDataListView;

        //private CollectionV

        private string _inputText;
        
        #endregion

        #region Constructors

        public BasicFileViewModel(FileModel fileModel)
        {
            this._FIXED_fileDataList = new ObservableCollection<FileData>();
            this._fileDataList = new ObservableCollection<FileData>();
            this._fileDataListView = CollectionViewSource.GetDefaultView(this._fileDataList);

            this._fileDataListView.Filter = this.TestFilter;

            this._inputText = string.Empty;

            fileModel.FileAdded += this.FileAddedHandler;
            fileModel.FileRemoved += this.FileRemovedHandler;
            
        }

        #endregion

        #region Properties

        public ObservableCollection<FileData> FileDataList
        {
            get
            {
                return this._FIXED_fileDataList;
            }
        }

        public ICollectionView FilteredFileDataList
        {
            get
            {
                return this._fileDataListView;
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
                this._fileDataListView.Refresh();
            }



            //if (property.Equals("InputText"))
        }

        #endregion

        #region Private Methods

        private bool TestFilter(object obj)
        {
            FileData fileData = obj as FileData;

            if (null != fileData)
            {
                return fileData.FileName.Contains(this.InputText);
            }


            // True displays, false hides
            return true;
        }

        /*
         * TODO: We should be on the GUI thread.. verify?
         */

        private void FileAddedHandler(FileData fileData)
        {
            this._FIXED_fileDataList.Add(fileData);
            this._fileDataList.Add(fileData);
        }

        private void FileRemovedHandler(FileData fileData)
        {
            this._FIXED_fileDataList.Remove(fileData);
            this._fileDataList.Remove(fileData);
        }

        #endregion


    }
}
