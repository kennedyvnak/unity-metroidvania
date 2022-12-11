namespace Metroidvania.UI.Menus {
    public interface IMenuScreen {
        bool menuEnabled { get; }

        void ActiveMenu();
        void DesactiveMenu();
    }
}