using easysave_project.View;
using easysave_project.Controllers;
using easysave_project.Models;

class Program {
    static void Main(String[] args) {
        var viewModel = new MainViewController();
        var menuView = new MenuView(viewModel);
        menuView.Run();
    }
}
