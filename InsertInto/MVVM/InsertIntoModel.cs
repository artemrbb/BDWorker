using InsertInto.ModelComponents;
using System.Collections.Generic;
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

        private readonly InsertWorker _insertWorker;
        private readonly BDWorker _bdWorker;

        #endregion

        #region Properties


        #endregion

        #region Methods

        public Result<List<DTP>> Converter(string pathFile)
        {
            return _insertWorker.Parser(pathFile);
        }

        public Result<bool> BDWork(List<DTP> dtpList) 
        {
            return new Result<bool>(() =>
            {
                var resConnect = _bdWorker.SQLConnected();
                if (!resConnect.IsOk)
                {
                    return false;// пуш на ошибку или лог 
                }
                List<MonthEnum> namesTable = new List<MonthEnum>();
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
                    // перед тем как запушить, нужно отсортировать даты

                    var resInsert = _bdWorker.InsertInto(dtp, resConnect.ResultObject);
                }

                return true;
            });
        }
        #endregion
    }
}