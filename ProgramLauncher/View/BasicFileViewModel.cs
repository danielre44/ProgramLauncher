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

namespace ProgramLauncher.View
{
    public class BasicFileViewModel : BaseViewModel
    {

        #region Fields

        private readonly ObservableCollection<FileData> _fileDataList;
        private readonly ReadOnlyObservableCollection<FileData> _readOnlyFileDataList;

        private readonly ObservableCollection<FileData> _filteredFileDataList;
        private readonly ReadOnlyObservableCollection<FileData> _readOnlyfilteredFileDataList;

        private string _inputText;
        
        #endregion

        #region Constructors

        public BasicFileViewModel(FileModel fileModel)
        {
            this._fileDataList = new ObservableCollection<FileData>();
            this._readOnlyFileDataList = new ReadOnlyObservableCollection<FileData>(this._fileDataList);

            this._filteredFileDataList = new ObservableCollection<FileData>(this._fileDataList.ToList());
            this._readOnlyfilteredFileDataList = new ReadOnlyObservableCollection<FileData>(this._filteredFileDataList);

            this._inputText = string.Empty;

            fileModel.FileAdded += this.FileAddedHandler;
            fileModel.FileRemoved += this.FileRemovedHandler;
            
        }

        #endregion

        #region Properties

        public ReadOnlyObservableCollection<FileData> FileDataList
        {
            get
            {
                return this._readOnlyFileDataList;
            }
        }

        public ReadOnlyObservableCollection<FileData> FilteredFileDataList
        {
            get
            {
                return this._readOnlyfilteredFileDataList;
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
                this.ApplyFilter();
            }
            else if (property.Equals("Test"))
            {
                this.ApplyFilter();
            }


            //if (property.Equals("InputText"))
        }

        #endregion

        #region Private Methods

        private void ApplyFilter()
        {
            this._filteredFileDataList.Clear();
            
            if (string.IsNullOrEmpty(this.InputText))
            {
                this._fileDataList.FirstOrDefault(x => { this._filteredFileDataList.Add(x); return false; });
            }
            else
            { 
                this._fileDataList.Where(x => x.FileName.Contains(this.InputText)).FirstOrDefault(x => { this._filteredFileDataList.Add(x); return false; });
            }
        }

        /*
         * TODO: We should be on the GUI thread.. verify?
         */

        private void FileAddedHandler(FileData fileData)
        {
            this._fileDataList.Add(fileData);
            this._filteredFileDataList.Add(fileData);

            this.HandleInternalPropertyChanged("Test");
        }

        private void FileRemovedHandler(FileData fileData)
        {
            this._fileDataList.Remove(fileData);
            this._filteredFileDataList.Remove(fileData);
            
            this.HandleInternalPropertyChanged("Test");
        }

        #endregion


    }
}
