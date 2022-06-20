using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using UltimateCore;
using UltimateCore.AppManagement;
using UltimateCore.LRI;

namespace InsertInto.ModelComponents
{
    internal sealed class BDWorker : Singleton<BDWorker>
    {
        #region Constructor

        private BDWorker()
        {

        }

        #endregion

        #region Fields

        private string _stringConnection = "Host=10.200.10.112;Username=cuba;Password=cuba;Database=smarts";

        #endregion

        #region Methods

        public Result<NpgsqlConnection> SQLConnected()
        {
            return new Result<NpgsqlConnection>(() =>
            {
                NpgsqlConnection npgsql = new NpgsqlConnection(_stringConnection);
                npgsql.Open();
                return npgsql;
            });
        }

        public Result<bool> CreateTable(MonthEnum tableName, NpgsqlConnection npgsql) 
        {
            return new Result<bool>(() =>
            {
                if (npgsql.FullState != ConnectionState.Broken || npgsql.FullState != ConnectionState.Closed)
                {
                    var resConnect = new Result<bool>(() => 
                    {
                        NpgsqlCommand createTable = new NpgsqlCommand($"CREATE TABLE {tableName.GetDescription()}(n varchar(100), d varchar(500), l1 varchar(100), l2 varchar(100))", npgsql);
                        int resCreate = createTable.ExecuteNonQuery();
                        return true;
                    });
                    if (!resConnect.IsOk && resConnect.ErrorMessage.StartsWith("42P07")) 
                    {
                        NpgsqlCommand dropTable = new NpgsqlCommand($"DROP TABLE public.{tableName.GetDescription()}", npgsql);
                        int resDrop = dropTable.ExecuteNonQuery();
                        NpgsqlCommand createTable = new NpgsqlCommand($"CREATE TABLE {tableName.GetDescription()}(n varchar(100), d varchar(500), l1 varchar(100), l2 varchar(100))", npgsql);
                        int resCreate = createTable.ExecuteNonQuery();
                    }


                }
                else
                {
                    MessageBox.Show("Соединение не доступно");
                }

                return true;
            });
        }

        public Result<bool> InsertInto(DTP dtp, NpgsqlConnection npgsql) 
        {
            return new Result<bool>(() =>
            {
                var resTemporaryTab = new Result<bool>(() =>
                {
                    NpgsqlCommand insertCommand = new NpgsqlCommand(dtp.Into, npgsql);
                    int resInsert = insertCommand.ExecuteNonQuery();
                    return true;
                });
                if (!resTemporaryTab.IsOk)
                    return false; // ошибка

            return true;
            });
        }

        public Result<bool> MapLayer(List<DateTime> firstDate, List<DateTime> lastDate, NpgsqlConnection npgsql) 
        {
            return new Result<bool>(() =>
            {
                var resMapLayer = new Result<bool>(() =>
                {
                    try
                    {
                        if (lastDate.Count == 0) 
                        {
                            NpgsqlCommand searchMapLayer = new NpgsqlCommand($"SELECT * FROM public.smarts_map_layer WHERE name ='с 07.06 по 08.09'", npgsql);
                            NpgsqlDataReader readerMapLayer = searchMapLayer.ExecuteReader();
                            if (readerMapLayer.HasRows)
                            {
                                while (readerMapLayer.Read())
                                {
                                    for (var i = 0; i < readerMapLayer.FieldCount; i++)
                                    {
                                        if (readerMapLayer.GetFieldType(i).ToString() == "System.Guid")
                                        {
                                            var res2 = readerMapLayer.GetGuid(i).ToString();
                                            ;
                                        }
                                    }
                                }
                            }
                            else 
                            {
                                return false;
                            }
                            
                        }
                        else
                        {
                            NpgsqlCommand searchMapLayer = new NpgsqlCommand($"SELECT * FROM public.smarts_map_layer WHERE name ='с {firstDate.First().ToString("dd.MM")} по {firstDate.Last().Date.ToString("dd.MM")}'", npgsql);
                            NpgsqlDataReader readerMapLayer = searchMapLayer.ExecuteReader();
                        }
                    }
                    catch (PostgresException code) 
                    {

                    }
                    return true;
                });



                return true;
            });
        }

        #endregion
    }
}
