using InsertInto.ModelComponents;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using UltimateCore.AppManagement;
using UltimateCore.CN;
using UltimateCore.EventManagement;
using System.Linq;
using UltimateCore.LRI;
using System.Threading;
using System.Globalization;
using InsertInto.Contracts;

namespace InsertInto.MVVM
{
    public class InsertIntoViewModel : Notifier, IHandle<List<DTP>>, IHandle<DTP>, IHandle<ReturnDTPContract>
    {

        #region Constructor

        public InsertIntoViewModel(AppFactory appFactory, EventAggregator eventAggregator, ObservableCollection<DTP> dtps)
        {
            _appFactory = appFactory;
            _model = _appFactory.GetClass<InsertIntoModel>();
            _eventAggregator = eventAggregator;
            
            _dtps = dtps;
            _dtpsCoordinates = new ObservableCollection<DTP>();
        }

        #endregion

        #region Fields

        private readonly InsertIntoModel _model;
        private readonly AppFactory _appFactory;
        private ObservableCollection<DTP> _dtps;
        private ObservableCollection<DTP> _dtpsCoordinates;
        private readonly EventAggregator _eventAggregator;

        #endregion

        #region Properties

        public ObservableCollection<DTP> Dtps { get => _dtps; set { _dtps = value; OnPropertyChanged(() => Dtps); } }
        public ObservableCollection<DTP> DtpsCoordinates { get => _dtpsCoordinates; set { _dtpsCoordinates = value; OnPropertyChanged(() => DtpsCoordinates); } }

        #endregion

        #region Methods

        public Result<bool> Init() 
        {
            return new Result<bool>(() =>
            {
                _eventAggregator.Subscribe(this);

                return true;
            });
        }

        #endregion

        #region Command

        public Command OpenFileCommand
        { 
            get => new Command(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Файлы PDF (*.pdf)|*.pdf";
                openFileDialog.InitialDirectory = @"C:\Dowloads";
                if (openFileDialog.ShowDialog().Value)
                {
                    var res = _model.Converter(openFileDialog.FileName);
                    if (res.IsOk) 
                    {
                        Dtps.Clear();
                        foreach (var dtp in res.ResultObject) 
                        {
                            Dtps.Add(dtp);
                        }
                        OnPropertyChanged(() => Dtps);
                    }
                }
            });
        }
        public Command CopyCommand
        {
            get => new Command(() =>
            {
                string text = string.Empty;
                foreach (var dtps in DtpsCoordinates)
                {
                    text += dtps.InsertInto() + "\n";
                }
                if (string.IsNullOrEmpty(text))
                    return;
                Clipboard.SetText(text);
            });
        }

        public Command DownloadCommand 
        {
            get => new Command(() =>
            {
                if (DtpsCoordinates.Count == 0) 
                {
                    return;
                }
                List<DTP> dtps = DtpsCoordinates.ToList();
                var resBDWork = _model.BDWork(dtps);
            });
        }

        public bool Handled(List<DTP> data)
        {
            DtpsCoordinates.Clear();
            foreach (var dtp in data) 
            {
                DtpsCoordinates.Add(dtp);
            }

            OnPropertyChanged(() => DtpsCoordinates);
            return true;
        }

        public bool Handled(DTP data)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Dtps.Remove(data);
            DtpsCoordinates.Add(data);
            OnPropertyChanged(() => Dtps);
            OnPropertyChanged(() => DtpsCoordinates);
            return true;
        }

        public bool Handled(ReturnDTPContract data)
        {
            data.Result = new Result<bool>(() =>
            {
                var dtp = data.DTP;
                DtpsCoordinates.Remove(dtp);
                Dtps.Add(dtp);
                OnPropertyChanged(() => DtpsCoordinates);
                OnPropertyChanged(() => Dtps);
                return true;
            });

            return true;
        }

        #endregion
    }
}
