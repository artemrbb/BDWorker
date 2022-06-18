using InsertInto.Contracts;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using UltimateCore;
using UltimateCore.CN;
using UltimateCore.EventManagement;

namespace InsertInto.ModelComponents
{
    public class DTP : Notifier
    {
        #region Constructor

        public DTP(MonthEnum tableName, int id, string type, double longitude, double latitude, string adress, DateTime date)
        {
            _tableName = tableName;
            _id = id;
            _type = type;
            _longitude = longitude.ToString();
            _latitude = latitude.ToString();
            _adress = adress;
            _date = date;

        }

        #endregion

        #region Fields

        private MonthEnum _tableName;
        private int _id;
        private string _type;
        private string _longitude;
        private string _latitude;
        private string _adress;
        private DateTime _date;

        #endregion

        #region Properties


        public MonthEnum TableName { get => _tableName; }
        public int Id { get => _id; }
        public string Type { get => _type; }
        public string Longitude { get => _longitude; set { _longitude = value; OnPropertyChanged(() => Longitude); } }
        public string Latitude { get => _latitude; set {_latitude = value; OnPropertyChanged(() => Latitude); } }
        public string Adress { get => _adress; set => _adress = value; }
        public string Into { get => InsertInto(); }
        public DateTime Date { get => _date; set => _date = value; }


        #endregion

        #region Methods

        public string InsertInto()
        {
            return $@"insert into {TableName.GetDescription()} values('{Id}','{Type}','{Latitude.Replace(',','.')}','{Longitude.Replace(',','.')}');";
        }

        public void Dowload() 
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            if (double.TryParse(Longitude, out double lon) && double.TryParse(Latitude, out double lan) && lon != 0 && lan != 0)
            {
                EventAggregator.GetInstance().Push(this);
            }
            else 
            {
                MessageBox.Show("Вы не верно ввели ширину или долготу");
            }
        }

        public Command ChangeCoordinatesCommand 
        {
            get => new Command(() =>
            {
                Dowload();

            });
        }

        public Command ReturnChangeCommand 
        {
            get => new Command(() =>
            {
                EventAggregator.GetInstance().Push(new ReturnDTPContract(this));
            });
        }

        #endregion
    }
}
