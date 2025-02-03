using easysave_project.View;
using easysave_project.Controller;

class Program {
    static void Main(String[] args) {
        var viewController = new MainViewController();
        var menuView = new MenuView(viewController);
        menuView.Run();
    }
}
