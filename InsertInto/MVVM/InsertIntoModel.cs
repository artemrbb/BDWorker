using InsertInto.ModelComponents;
using System;
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
                List<DateTime> allDate = new List<DateTime>();
                List<DateTime> firstDate = new List<DateTime>();
                List<DateTime> lastDate = new List<DateTime>();
                foreach (var dtp in dtpList)
                {
                    allDate.Add(dtp.Date);
                    if (namesTable.Contains(dtp.TableName))
                        continue;
                    namesTable.Add(dtp.TableName);
                }

                for (int i = 1; i < 13; i++)
                {
                    var months = allDate.Where(p => p.Month == i).ToList();
                    if (months.Count != 0) 
                    {
                        months.Sort();
                        var resFirstDay = months.First();
                        var resLastDay = months.Last();
                        if (namesTable.Count > 1)
                        {
                            if (firstDate.Count != 0)
                            {
                                lastDate.Add(resFirstDay);
                                lastDate.Add(resLastDay);
                                break;
                            }
                            firstDate.Add(resFirstDay);
                            firstDate.Add(resLastDay);
                        }
                        else 
                        {
                            firstDate.Add(resFirstDay);
                            firstDate.Add(resLastDay);
                            break;
                        }
                    }
                }
                //foreach (var tables in namesTable)
                //{
                //    var resCreate = _bdWorker.CreateTable(tables, resConnect.ResultObject);
                //}

                //foreach (var dtp in dtpList)
                //{
                //    var resInsert = _bdWorker.InsertInto(dtp, resConnect.ResultObject);
                //}

                var resMapLayer = _bdWorker.MapLayer(firstDate, lastDate, resConnect.ResultObject);



                return true;
            });
        }
        #endregion
    }
}