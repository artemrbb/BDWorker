using InsertInto.ModelComponents;
using System.Collections.Generic;
using UltimateCore.LRI;

namespace InsertInto.MVVM
{
    internal class InsertIntoModel
    {
        #region Constructor

        public InsertIntoModel(InsertWorker insertWorker)
        {
            _insertWorker = insertWorker;
        }
        #endregion

        #region Fields

        private string _pathFile;
        private readonly InsertWorker _insertWorker;

        #endregion

        #region Properties

        public string PathFile { get => _pathFile; set => _pathFile = value; }

        #endregion


        #region Methods

        public Result<List<DTP>> Converter()
        {
            return _insertWorker.Parser(_pathFile);
        }
        #endregion
    }
}