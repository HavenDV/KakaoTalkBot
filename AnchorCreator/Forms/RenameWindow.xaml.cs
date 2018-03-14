using System.ComponentModel;

namespace AnchorsCreator.Forms
{
    public partial class RenameWindow : INotifyPropertyChanged
    {
        private string _oldName;
        private string _newName;

        public string OldName
        {
            get => _oldName;
            set
            {
                _oldName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OldName)));
            }
        }

        public string NewName
        {
            get => _newName;
            set
            {
                _newName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewName)));
            }
        }

        public RenameWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
