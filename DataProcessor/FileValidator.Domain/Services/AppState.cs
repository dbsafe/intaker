namespace FileValidator.Domain.Services
{
    public class AppState
    {
        public string AppName { get; } = "Intaker";
        public HomePageState HomePage { get; } = new HomePageState();
        public FileSpecificationsPageState FileSpecificationsPage { get; } = new FileSpecificationsPageState();
        public LoadedFilePageState LoadedFilePage { get; } = new LoadedFilePageState();
        public LoadedFileJsonPageState LoadedFileJsonPage { get; } = new LoadedFileJsonPageState();
        public LoadedFileGroupsJsonPageState LoadedFileGroupsJsonPage { get; } = new LoadedFileGroupsJsonPageState();
    }

    public class HomePageState
    {
        public string InputDataContent { get; set; }
        public int SelectedFileSpecId { get; set; }
        public int SelectedSampleFileId { get; set; }
        public CursorPosition CursorPosition { get; set; }
    }

    public class CursorPosition
    {
        public int Row { get; set; }
        public int Column { get; set; }
    }

    public class FileSpecificationsPageState
    {
        public FileSpecification SelectedFileSpecification { get; set; }
        public CursorPosition CursorPosition { get; set; }
    }

    public class LoadedFilePageState
    {
        public string FrameworkVersion { get; set; }
        public ParsedDataAndSpec10 ParsedDataAndSpec10 { get; set; }
        public ParsedDataAndSpec20 ParsedDataAndSpec20 { get; set; }
        public CursorPosition CursorPosition { get; set; }
    }

    public class LoadedFileJsonPageState
    {
        public CursorPosition CursorPosition { get; set; }
    }

    public class LoadedFileGroupsJsonPageState
    {
        public CursorPosition CursorPosition { get; set; }
    }
}
