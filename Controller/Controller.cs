using WindowsForm.Model;
using Timer = System.Windows.Forms.Timer;

namespace WindowsForm.Controller
{
    public class Controller
    {
        public event Action<bool> PauseIsPressed;

        private readonly GameModel Model;

        private Timer MainTimer;
        private Timer GameTimeTimer;
        private Timer BotManagementTimer;
        private Timer BotCreationTimer;

        public Controller(GameModel model) => Model = model;

        public void ActivateTimers(int intervalForTheAppearanceOfBots)
        {
            GameTimeTimer = new() { Interval = 1000 };
            GameTimeTimer.Tick += (object sender, EventArgs args) => Model.AmountOfTimeUntilTheEndOfTheRound--;
            GameTimeTimer.Start();

            MainTimer = new();
            MainTimer.Tick += UpdateTheModel;
            MainTimer.Start();

            BotCreationTimer = new() { Interval = intervalForTheAppearanceOfBots };
            BotCreationTimer.Tick += (object sender, EventArgs args) => Model.CreateAFirstAidKit();
            BotCreationTimer.Tick += (object sender, EventArgs args) => Model.CreateBots();
            BotCreationTimer.Start();

            BotManagementTimer = new Timer() { Interval = 300 };
            BotManagementTimer.Tick += (object sender, EventArgs args) => Model.SetTheBotsInMotion(Model);
            BotManagementTimer.Start();
        }

        public void StopTimers()
        {
            MainTimer.Stop();
            GameTimeTimer.Stop();
            BotCreationTimer.Stop();
            BotManagementTimer.Stop();
            
            MainTimer.Dispose();
            GameTimeTimer.Dispose();
            BotCreationTimer.Dispose();
            BotManagementTimer.Dispose();
        }

        public void PutItOnPause(object sender, EventArgs e)
        {
            BotCreationTimer.Enabled = !BotCreationTimer.Enabled;
            BotManagementTimer.Enabled = !BotManagementTimer.Enabled;
            MainTimer.Enabled = !MainTimer.Enabled;
            GameTimeTimer.Enabled = !GameTimeTimer.Enabled;
            PauseIsPressed(GameTimeTimer.Enabled);
        }

        public void MakeAMove(object sender, KeyEventArgs e)
        {
            if (MainTimer.Enabled == false) return;
            switch (e.KeyCode)
            {
                case Keys.W: Model.Player.GoForwad(Model); break;
                case Keys.S: Model.Player.GoBack(Model); break;
                case Keys.D: Model.Player.GoRight(Model); break;
                case Keys.A: Model.Player.GoLeft(Model); break;
                case Keys.Enter: PutItOnPause(sender, e); break;
                default: break;
            }
        }

        public void ToShoot(object sender, EventArgs e)
        {
            if (MainTimer.Enabled == false) return;
            Model.Player.Shoot(Model);
        }

        public void RotateThePlayer(object sender, MouseEventArgs e)
        {
            if (MainTimer.Enabled == false) return;
            if (e.Delta >= 120) Model.Player.TurnRight();
            else if (e.Delta <= -120) Model.Player.TurnLeft();
        }

        void UpdateTheModel(object sender, EventArgs args) => Model.ExecuteTheCommandsOfTheHeroes();
    }
}
