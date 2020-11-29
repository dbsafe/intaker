using System;

namespace FileValidator.Domain.Services
{
    public class MenuItemClickedEventArgs : EventArgs
    {
        public string Url { get; }

        public MenuItemClickedEventArgs(string url)
        {
            Url = url;
        }
    }

    public class ApplicationsEvents
    {
        public event EventHandler<MenuItemClickedEventArgs> MenuItemClicked;

        public void PublichItemClicked(string url)
        {
            MenuItemClicked?.Invoke(this, new MenuItemClickedEventArgs(url));
        }
    }
}
