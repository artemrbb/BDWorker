using InsertInto.ModelComponents;
using System.Collections.Generic;
using System.Linq;
using UltimateCore.LRI;

namespace InsertInto.MVVM
{
    internal class InsertIntoModel
    {
        #region Constructor

        public InsertIntoModel(InsertWorker insertWorker, BDWorker bdWorker)
        {
            _insertWorker = insertWorker;
            _bdWorker = bdWorker;
        }
        #endregion

        #region Fields

        private string _pathFile;
        private readonly InsertWorker _insertWorker;
        private readonly BDWorker _bdWorker;

        #endregion

        #region Properties

        public string PathFile { get => _pathFile; set => _pathFile = value; }

        #endregion

        #region Methods

        public Result<List<DTP>> Converter()
        {
            return _insertWorker.Parser(_pathFile);
        }

        public Result<bool> BDWork(List<DTP> dtpList) 
        {
            return new Result<bool>(() =>
            {
                var resConnect = _bdWorker.SQLConnected();
                if (!resConnect.IsOk)
                {
                    // ошибка в подключении
                }
                List<string> namesTable = new List<string>();
                foreach (var dtp in dtpList) 
                {
                    if (namesTable.Contains(dtp.TableName))
                        continue;
                    namesTable.Add(dtp.TableName);
                }
                foreach (var tables in namesTable) 
                {
                    var resCreate = _bdWorker.CreateTable(tables, resConnect.ResultObject);
                }

                foreach (var dtp in dtpList) 
                {
                    var resInsert = _bdWorker.InsertInto(dtp.Into,resConnect.ResultObject);
                }

                return true;
            });
        }
        #endregion
    }
}