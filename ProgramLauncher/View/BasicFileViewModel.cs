using ProgramLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.View
{
    public class BasicFileViewModel
    {

        #region Fields

        private readonly ObservableCollection<FileData> _fileDataList;
        private readonly ReadOnlyObservableCollection<FileData> _readOnlyFileDataList;

        #endregion

        #region Constructors

        public BasicFileViewModel(FileModel fileModel)
        {
            this._fileDataList = new ObservableCollection<FileData>();
            this._readOnlyFileDataList = new ReadOnlyObservableCollection<FileData>(this._fileDataList);

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

        #endregion

        #region Private Methods


        /*
         * TODO: We should be on the GUI thread.. verify?
         */

        private void FileAddedHandler(FileData fileData)
        {
            _fileDataList.Add(fileData);
        }

        private void FileRemovedHandler(FileData fileData)
        {
            _fileDataList.Remove(fileData);
        }

        #endregion


    }
}
