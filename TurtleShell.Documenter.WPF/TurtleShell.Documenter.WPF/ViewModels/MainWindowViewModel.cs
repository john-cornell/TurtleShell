using MarkdownSharp;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TurtleShell.Documenter.WPF.Commands;
using TurtleShell.Documenter.WPF.Documenter;
using TurtleShell.Documenter.WPF.Tests;

namespace TurtleShell.Documenter.WPF.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly IDocumentEngine _documentEngine;
        private string _solutionDirectory;
        private string _projectPath;
        private string _documentationContent;
        private string _rawMarkdownContent;
        private bool _isProcessing;
        private bool _isProcessingCompleted;

        public ICommand StartCommand { get; }
        public ICommand SelectSolutionDirectoryCommand { get; }
        public ICommand TestMarkdownCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand SaveCommand { get; }

        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                _projectPath = value;
                OnPropertyChanged(nameof(ProjectPath));
            }
        }

        public string DocumentationContent
        {
            get => _documentationContent;
            set
            {
                _documentationContent = value;
                OnPropertyChanged(nameof(DocumentationContent));
            }
        }

        public string RawMarkdownContent
        {
            get => _rawMarkdownContent;
            set
            {
                _rawMarkdownContent = value;
                OnPropertyChanged(nameof(RawMarkdownContent));
            }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                OnPropertyChanged(nameof(IsProcessing));
            }
        }

        public bool IsProcessingCompleted
        {
            get => _isProcessingCompleted;
            set
            {
                _isProcessingCompleted = value;
                OnPropertyChanged(nameof(IsProcessingCompleted));
            }
        }

        public MainWindowViewModel(IDocumentEngine documentEngine)
        {
            _documentEngine = documentEngine;
            _solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.Parent?.FullName ?? string.Empty;
            ProjectPath = Path.Combine(_solutionDirectory, "TurtleShell");
            IsProcessing = false; // Ensure initial state is set to false
            IsProcessingCompleted = false; // Ensure initial state is set to false
            StartCommand = new RelayCommand(async (parameter) => await ExecuteStartCommand(parameter));
            SelectSolutionDirectoryCommand = new RelayCommand(ExecuteSelectSolutionDirectoryCommand);
            TestMarkdownCommand = new RelayCommand(ExecuteTestMarkdownCommand);
            CopyCommand = new RelayCommand(ExecuteCopyCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
        }

        private async Task ExecuteStartCommand(object parameter)
        {
            IsProcessing = true;
            IsProcessingCompleted = false;
            try
            {
                var turtleShellProjectPath = Directory.GetDirectories(_solutionDirectory, "TurtleShell", SearchOption.TopDirectoryOnly).FirstOrDefault();

                if (turtleShellProjectPath != null)
                {
                    var overview = await _documentEngine.GenerateHighLevelOverview(turtleShellProjectPath);
                    SetMarkdown(overview);
                }
            }
            finally
            {
                IsProcessing = false;
                IsProcessingCompleted = true;
            }
        }

        private void ExecuteSelectSolutionDirectoryCommand(object parameter)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog();
            var result = dialog.ShowDialog();

            if (result == true)
            {
                _solutionDirectory = dialog.FolderName;
            }
        }

        private void ExecuteTestMarkdownCommand(object parameter)
        {
            SetMarkdown(Resources.HelloWorldDocumentation);
        }

        private void ExecuteCopyCommand(object parameter)
        {
            Clipboard.SetText(RawMarkdownContent);
        }

        private void ExecuteSaveCommand(object parameter)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "README.md",
                DefaultExt = ".md",
                Filter = "Markdown files (.md)|*.md",
                InitialDirectory = _solutionDirectory
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                File.WriteAllText(dialog.FileName, RawMarkdownContent);
            }
        }

        private void SetMarkdown(string content)
        {
            RawMarkdownContent = content;
            var markdown = new Markdown();
            DocumentationContent = markdown.Transform(content);
        }
    }
}
