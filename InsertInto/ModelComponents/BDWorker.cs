using Npgsql;
using System.Data;
using System.Windows;
using UltimateCore.AppManagement;

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

        public void SQLConnected() 
        {
            NpgsqlConnection npgsql = new NpgsqlConnection(_stringConnection);
            npgsql.Open();
            if (npgsql.FullState != ConnectionState.Broken || npgsql.FullState != ConnectionState.Closed)
            {
                NpgsqlCommand command = new NpgsqlCommand("INSERT INTO test values('asds','123321');", npgsql); // параметры конструктора комманд, подставляются либо команда которая отображает таблицу либ ввода и изменения данных
                int a = command.ExecuteNonQuery(); // метод для вывода успешного или не успешного ввода или изменения данных
                NpgsqlDataReader reader = command.ExecuteReader(); // метод для вывода информации о таблице
                //if (reader.HasRows)                                 нужно дергать тот или иной метод в зависимости от нужного применения,
                //{                                                   если нужно создать новый тейбл, то дергать  ExecuteNonQuery()
                //    var res = reader.GetFieldType(0);               если псмотреть таблицу то ExecuteReader()
                //    string str = reader.GetString(0);
                //    ;
                //}
                //;
            }
            else 
            {
                MessageBox.Show("Соединение не доступно");
            }

        }


        #endregion
    }
}
