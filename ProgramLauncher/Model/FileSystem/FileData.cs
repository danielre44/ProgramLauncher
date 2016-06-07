using ProgramLauncher.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.Model.FileSystem
{
    /// <summary>
    /// A class containing File Data.
    /// </summary>
    public class FileData
    {
        
        #region Fields
        
        private readonly string _fileName;
        private readonly string _absoluteFilePath;
        private readonly string _fileExtension;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a FileData object.
        /// </summary>
        /// <param name="fileName">The Name associated with the file.</param>
        /// <param name="absoluteFilePath">The absolute path to the file.</param>
        /// <param name="fileExtension">The file extension associated with the file.</param>
        public FileData(string fileName, string absoluteFilePath, string fileExtension)
        {
            _fileName = fileName;
            _absoluteFilePath = absoluteFilePath;
            _fileExtension = fileExtension;

            Logger.LogTrace("Created File: FileName: " + _fileName + " AbsoluteFilePath: " + _absoluteFilePath + " FileExtension: " + _fileExtension);
        }

        #endregion

        #region Static Methods

        public static FileData TryCreateFromAbsolutePath(string absoluteFilePath)
        {
            FileData fileData = null;

            try
            {
                string fileName = Path.GetFileNameWithoutExtension(absoluteFilePath);
                string fileExtension = Path.GetExtension(absoluteFilePath);

                fileData = new FileData(fileName, absoluteFilePath, fileExtension);
            }
            catch (ArgumentException e)
            {
                Logger.LogException(e);
            }

            return fileData;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the FileName associated with this file.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Gets the absolute path to this file.
        /// </summary>
        public string AbsoluteFilePath
        {
            get { return _absoluteFilePath; }
        }

        /// <summary>
        /// Gets the File Extension associated with the file.
        /// </summary>
        public string FileExtension
        {
            get { return _fileExtension; }
        }

        #endregion

    }
}
