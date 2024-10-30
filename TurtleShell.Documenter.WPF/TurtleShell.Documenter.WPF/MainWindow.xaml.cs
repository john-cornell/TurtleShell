using System.Windows;
using TurtleShell.Documenter.WPF.ViewModels;

namespace TurtleShell.Documenter.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContextChanged += MainWindow_DataContextChanged;
        }

        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.DocumentationContent))
                {
                    MarkdownWebBrowser.NavigateToString(viewModel.DocumentationContent);
                }
            };
        }
    }
}
