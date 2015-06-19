namespace Gearset.UserInterface.Wpf
{
    public interface IWindow
    {
        bool WasHiddenByGameMinimize { get; set; }

        bool IsVisible { get; }
        bool Topmost { get; set; }
        void Show();
        void Hide();
    }
}
