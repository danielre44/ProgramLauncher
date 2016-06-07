using ProgramLauncher.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.Model
{
    public class ProgramLauncherModel
    {
        #region Fields

        private static ProgramLauncherModel _instance = new ProgramLauncherModel();

        private bool _initialized;
        private FileSystemModel _fileSystemModel;

        #endregion

        #region Constructors

        private ProgramLauncherModel()
        {
            this._initialized = false;
            this._fileSystemModel = null;
        }

        #endregion

        #region Properties

        public static ProgramLauncherModel Instance
        {
            get { return _instance; }
        }

        public FileSystemModel FileSystemModel
        {
            get { return this._fileSystemModel; }
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            // TODO: Make sure we're on the main thread...

            if (this._initialized)
            {
                // TODO
            }
            else
            {
                this._fileSystemModel = new FileSystemModel();

                this._initialized = true;
            }
        }

        public void Shutdown()
        {
            // TODO: make sure we're on the main thread...

            if (this._initialized)
            {
                this._fileSystemModel.Stop();
                this._fileSystemModel = null;

                this._initialized = false;
            }
            else
            {
                // TODO
            }
        }

        #endregion

    }
}
