using InsertInto.MVVM;
using System.Windows;
using UltimateCore.AppManagement;
using UltimateCore.LRI;

namespace InsertInto
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region Consrtuctor

        public App()
        {
            Logger.StartProgrammLog();
            App.Current.Exit += CurrentExit;
            _appFactory = AppFactory.GetInstance();
            _view = _appFactory.GetClass<InsertIntoView>();
            _view.Show();
        }

        

        #endregion

        #region Fields

        private readonly AppFactory _appFactory;
        private readonly InsertIntoView _view;

        #endregion

        #region Handlers

        private void CurrentExit(object sender, ExitEventArgs e)
        {
            App.Current.Exit -= CurrentExit;
            Logger.StopProgrammLog();
        }

        #endregion
    }
}
