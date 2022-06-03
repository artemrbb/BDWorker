using Microsoft.Win32;
using System.Windows;

namespace InsertInto.MVVM
{
    /// <summary>
    /// Логика взаимодействия для ViewInsterInto.xaml
    /// </summary>
    public partial class InsertIntoView : Window
    {

        #region Constructor

        public InsertIntoView()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields

        private string _pathFile;

        #endregion

        #region Commands

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы PDF (*.pdf)|*.pdf";
            openFileDialog.InitialDirectory = @"C:\Dowloads";
            if (openFileDialog.ShowDialog() == true) 
            {
                _pathFile = openFileDialog.FileName;
            }
        }

        #endregion
    }
}
