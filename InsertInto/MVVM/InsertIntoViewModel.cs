using InsertInto.ModelComponents;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using UltimateCore.AppManagement;
using UltimateCore.CN;
using UltimateCore.EventManagement;

namespace InsertInto.MVVM
{
    public class InsertIntoViewModel : Notifier, IHandle<List<DTP>>, IHandle<DTP>
    {
        #region Constructor

        public InsertIntoViewModel(AppFactory appFactory, EventAggregator eventAggregator, ObservableCollection<DTP> dtps)
        {
            _appFactory = appFactory;
            _model = _appFactory.GetClass<InsertIntoModel>();
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            
            _isOpenFile = false;
            _dtps = dtps;
            _dtpsCoordinates = new ObservableCollection<DTP>();
        }

        #endregion

        #region Fields

        InsertIntoModel _model;
        private readonly AppFactory _appFactory;
        private ObservableCollection<DTP> _dtps;
        private ObservableCollection<DTP> _dtpsCoordinates;
        private EventAggregator _eventAggregator;
        private bool _isOpenFile;

        #endregion

        #region Properties

        public ObservableCollection<DTP> Dtps { get => _dtps; set { _dtps = value; OnPropertyChanged(() => Dtps); } }
        public ObservableCollection<DTP> DtpsCoordinates { get => _dtpsCoordinates; set { _dtpsCoordinates = value; OnPropertyChanged(() => DtpsCoordinates); } }

        #endregion

        #region Methods



        #endregion

        #region Command

        public Command NewFileCommand
        { 
            get => new Command(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Файлы PDF (*.pdf)|*.pdf";
                openFileDialog.InitialDirectory = @"C:\Dowloads";
                if (openFileDialog.ShowDialog() == true)
                {
                    _isOpenFile = true;
                    _model.PathFile = openFileDialog.FileName;
                    var res = _model.Converter();
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
                if (_isOpenFile == true) 
                {
                    string text = string.Empty;
                    foreach (var dtps in DtpsCoordinates)
                    {
                        text += dtps.InsertInto() + "\n";
                    }

                    Clipboard.SetText(text);
                }
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
            Dtps.Remove(data);
            DtpsCoordinates.Add(data);
            OnPropertyChanged(() => Dtps);
            OnPropertyChanged(() => DtpsCoordinates);
            return true;
        }

        #endregion
    }
}
