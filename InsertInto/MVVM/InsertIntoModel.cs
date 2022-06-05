using InsertInto.ModelComponents;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;
using System.Text;
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
        private List<string> _parseString;
        private readonly InsertWorker _insertWorker;

        public string PathFile { get => _pathFile; set => _pathFile = value; }


        #endregion

        #region Properties



        #endregion


        #region Methods

        //Сделать рефакторинг конверта. Разобраться с полями
        public Result<List<DTP>> Converter()
        {
            return _insertWorker.Parser(_pathFile);
        }
        #endregion
    }
}