using InsertInto.MVVM;
using System.Windows;
using UltimateCore.AppManagement;

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
            _appFactory =  AppFactory.GetInstance();
            _view = _appFactory.GetClass<InsertIntoView>();
            _view.Show();
        }

        #endregion

        #region Fields

        AppFactory _appFactory;
        InsertIntoView _view;

        #endregion
    }
}
