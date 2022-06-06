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
        private string _fileName;
        private readonly InsertWorker _insertWorker;

        #endregion

        #region Properties

        public string PathFile { get => _pathFile; set => _pathFile = value; }
        public string FileName { get => _fileName; set => _fileName = value; }

        #endregion


        #region Methods

        //Сделать рефакторинг конверта. Разобраться с полями
        public Result<List<DTP>> Converter()
        {
            return _insertWorker.Parser(_pathFile, _fileName);
        }
        #endregion
    }
}