namespace InsertInto.ModelComponents
{
    internal class DTP
    {
        #region Constructor

        public DTP(string tableName, int id, string type, double longitude, double latitude)
        {
            _tableName = tableName;
            _id = id;
            _type = type;
            _longitude = longitude;
            _latitude = latitude;
        }

        #endregion

        #region Fields

        private string _tableName;
        private int _id;
        private string _type;
        private double _longitude;
        private double _latitude;

        #endregion

        #region Properties


        public string TableName { get => _tableName; }
        public int Id { get => _id; }
        public string Type { get => _type; }
        public double Longitude { get => _longitude; set => _longitude = value; }
        public double Latitude { get => _latitude; set => _latitude = value; }

        #endregion

        #region Methods

        public string InsertInto()
        {
            return $@"insert into {TableName} values('{Id}', '{Type}', '{Latitude}', '{Longitude}')";
        }

        #endregion
    }
}
