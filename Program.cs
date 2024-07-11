using WindowsForm.Model;
using WindowsForm.Model.Map;


namespace Window
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MyForm(
                new GameModel(new Playground(0),
                new WindowsForm.View.InfoAboutTheLevel([ "0", "false", "1", "1", "1", "1", "1", "1" ]))));
        }
    }
}