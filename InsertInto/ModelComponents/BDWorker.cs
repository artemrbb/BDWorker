using Npgsql;
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

        private string _stringConnection = "Host=localhost;Username=postgres;Password=linkoln;Database=postgres";

        #endregion

        #region Methods

        public Result<bool> SQLConnected()
        {
            return new Result<bool>(() =>
            {
                NpgsqlConnection npgsql = new NpgsqlConnection(_stringConnection);
                npgsql.Open();
                var res = CreateTable("artem", npgsql);
                ;

                return true;
            });
        }

        private Result<bool> CreateTable(string tableName, NpgsqlConnection npgsql) 
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
                    //NpgsqlCommand command = new NpgsqlCommand("INSERT INTO test values('asds','123321');", npgsql); // параметры конструктора комманд, подставляются либо команда которая отображает таблицу либ ввода и изменения данных
                    //int a = command.ExecuteNonQuery(); // метод для вывода успешного или не успешного ввода или изменения данных
                    //NpgsqlDataReader reader = command.ExecuteReader(); // метод для вывода информации о таблице
                    //                                                   //if (reader.HasRows)                                 нужно дергать тот или иной метод в зависимости от нужного применения,
                    //                                                   //{                                                   если нужно создать новый тейбл, то дергать  ExecuteNonQuery()
                    //                                                   //    var res = reader.GetFieldType(0);               если псмотреть таблицу то ExecuteReader()
                    //                                                   //    string str = reader.GetString(0);
                    //                                                   //    ;
                    //                                                   //}
                    //                                                   //;
                }
                else
                {
                    MessageBox.Show("Соединение не доступно");
                }

                return true;
            });
        }


        #endregion
    }
}
