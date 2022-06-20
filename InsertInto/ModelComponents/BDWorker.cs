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

        public Result<bool> MapLayer(List<DateTime> firstDate, List<DateTime> lastDate, NpgsqlConnection npgsql) // ПЕРЕРАБОТАТЬ АРХИТЕКТУРУ ПОИСКА И СОЗДАНИИ СЛОЕВ
        {
            return new Result<bool>(() =>
            {
                var resMapLayer = new Result<bool>(() =>
                {
                    try
                    {
                        if (lastDate.Count == 0) 
                        {
                            NpgsqlCommand searchMapLayer = new NpgsqlCommand($"SELECT * FROM public.smarts_map_layer WHERE name ='с 07.08 по 08.09'", npgsql);
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
                                readerMapLayer.Dispose();
                                var res = LayerFolder("08", npgsql);
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

        private Result<bool> LayerFolder(string firstDate, NpgsqlConnection npgsql) 
        {
            return new Result<bool>(() =>
            {
                var resDate = string.Empty;
                switch (firstDate) 
                {
                    case "01": resDate = "Январь 2022"; break;
                    case "02": resDate = "Февраль 2022"; break;
                    case "03": resDate = "Март 2022"; break;
                    case "04": resDate = "Апрель 2022"; break;
                    case "05": resDate = "Май 2022"; break;
                    case "06": resDate = "Июнь 2022"; break;
                    case "07": resDate = "Июль 2022"; break;
                    case "08": resDate = "Август 2022"; break;
                    case "09": resDate = "Сентябрь 2022"; break;
                    case "10": resDate = "Октябрь 2022"; break;
                    case "11": resDate = "Ноябрь 2022"; break;
                    case "12": resDate = "Декабрь 2022"; break;
                }
                try
                {
                    NpgsqlCommand searchLayerFolder = new NpgsqlCommand($"SELECT * FROM public.smarts_layer_folder WHERE name ='{resDate}'", npgsql);
                    NpgsqlDataReader readerLayerFolder = searchLayerFolder.ExecuteReader();
                    if (readerLayerFolder.HasRows)
                    {
                        // делаем логику по считыванию айдишника папки
                    }
                    else
                    {
                        readerLayerFolder.Close();
                        NpgsqlCommand searchLayerParentFolder = new NpgsqlCommand($"SELECT * FROM public.smarts_layer_folder WHERE name ='2022'", npgsql);
                        NpgsqlDataReader readerLayerParentFolder = searchLayerParentFolder.ExecuteReader();
                        if (readerLayerParentFolder.HasRows)
                        {
                            while (readerLayerParentFolder.Read())
                            {
                                for (var i = 0; i < readerLayerParentFolder.FieldCount; i++)
                                {
                                    if (readerLayerParentFolder.GetFieldType(i).ToString() == "System.Guid")
                                    {
                                        var res2 = readerLayerParentFolder.GetGuid(i).ToString();
                                    }
                                }
                            }
                        }

                    }
                }
                catch (NullReferenceException code)
                {

                }
                return true;
            });
        }

        #endregion
    }
}
