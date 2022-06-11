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

        private string _stringConnection = "User ID=root;Host=10.200.10.112;Port=5432;Database=smarts;Pooling=true;Connection Lifetime=0;";

        #endregion

        #region Methods

        //public Result<bool> SQLConnected() 
        //{
        //    //return new Result<bool>(() =>
        //    //{
        //    //    NpgsqlConnection npgsql = new NpgsqlConnection(_stringConnection);
        //    //    ;
        //    //    npgsql.Open();

        //    //    return true;
        //    //});
            
        //    //if (npgsql.FullState != ConnectionState.Broken || npgsql.FullState != ConnectionState.Closed)
        //    //{
        //    //    //d
        //    //    NpgsqlCommand command = new NpgsqlCommand("INSERT INTO test values('asds','123321');", npgsql); // параметры конструктора комманд, подставляются либо команда которая отображает таблицу либ ввода и изменения данных
        //    //    int a = command.ExecuteNonQuery(); // метод для вывода успешного или не успешного ввода или изменения данных
        //    //    NpgsqlDataReader reader = command.ExecuteReader(); // метод для вывода информации о таблице
        //    //    //if (reader.HasRows)                                 нужно дергать тот или иной метод в зависимости от нужного применения,
        //    //    //{                                                   если нужно создать новый тейбл, то дергать  ExecuteNonQuery()
        //    //    //    var res = reader.GetFieldType(0);               если псмотреть таблицу то ExecuteReader()
        //    //    //    string str = reader.GetString(0);
        //    //    //    ;
        //    //    //}
        //    //    //;
        //    //}
        //    //else
        //    //{
        //    //    MessageBox.Show("Соединение не доступно");
        //    //}

        //}


        #endregion
    }
}
