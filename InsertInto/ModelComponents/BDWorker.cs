using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Windows;
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

        public Result<bool> CreateTable(string tableName, NpgsqlConnection npgsql) 
        {
            return new Result<bool>(() =>
            {
                if (npgsql.FullState != ConnectionState.Broken || npgsql.FullState != ConnectionState.Closed)
                {
                    try
                    {
                        NpgsqlCommand createTable = new NpgsqlCommand($"CREATE TABLE {tableName}(n varchar(100), d varchar(500), l1 varchar(100), l2 varchar(100))", npgsql);
                        int resCreate = createTable.ExecuteNonQuery();
                    }
                    catch (PostgresException ex)
                    {
                        switch (ex.Code) 
                        {
                            case "42P07":
                                NpgsqlCommand dropTable = new NpgsqlCommand($"DROP TABLE public.{tableName}", npgsql);
                                int resDrop = dropTable.ExecuteNonQuery();
                                NpgsqlCommand createTable = new NpgsqlCommand($"CREATE TABLE {tableName}(n varchar(100), d varchar(500), l1 varchar(100), l2 varchar(100))", npgsql);
                                int resCreate = createTable.ExecuteNonQuery(); break;
                            default: MessageBox.Show("Ошибка в BDWorker'e. В switch'e не обработана ошибка"); break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Соединение не доступно");
                }

                return true;
            });
        }

        public Result<bool> InsertInto(string intoCommand, NpgsqlConnection npgsql) 
        {
            return new Result<bool>(() =>
            {
                NpgsqlCommand insertCommand = new NpgsqlCommand(intoCommand, npgsql);
                int resInsert = insertCommand.ExecuteNonQuery();
                return true;
            });
        }


        #endregion
    }
}
