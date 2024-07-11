using WindowsForm.Model;
using WindowsForm.Controller;
using WindowsForm.View;
using Point = System.Drawing.Point;


namespace Window
{
    public partial class MyForm : Form
    {
        private GameModel model;
        private Controller controller;

        private float sizeOfTheGridCell;
        private PointF initialCoordinate;

        private readonly InfoAboutTheLevel[][] infoAboutTheLevels;

        private Button yesButton;
        private Button noButton;
        private Button pauseButton;
        private Button playButton;
        private Button exitButton;
        private Button buttonToGoToTheLevelSelection;
        private Button startOverButton;
        private Button gameInformationButton;
        private Button backButton;

        private readonly Button[,] LevelButtons;
        private readonly EventHandler[,] LevelTriggerFunctions;

        private readonly Dictionary<bool, Image> pauseImages;

        public MyForm(GameModel model)
        {
            this.model = model;
            LevelButtons = new Button[3, 6];
            infoAboutTheLevels = ReadTheSavedData();
            LevelTriggerFunctions = new EventHandler[LevelButtons.GetLength(0), LevelButtons.GetLength(1)];
            FillInTheMatrixWithFunctions();

            pauseImages = new Dictionary<bool, Image>()
            {
                [true] = Image.FromFile(@"Images\PauseTurnOn.png"),
                [false] = Image.FromFile(@"Images\PauseTurnOff.png")
            };

            //Ì‡ÒÚÓÈÍ‡ WinForm
            KeyPreview = true;
            DoubleBuffered = true;
            Size = new Size() { Height = 450, Width = 800 };

            BackColor = Color.Black;
            WindowState = FormWindowState.Maximized;

            OpenTheMainMenu();

            Load += (sender, args) => OnSizeChanged(EventArgs.Empty);
            FormClosing += SaveTheGameResults;
            InitializeComponent();
        }

        #region √À¿¬ÕŒ≈ Ã≈Õﬁ
        void OpenTheMainMenu()
        {
            BackgroundImageLayout = ImageLayout.Zoom;
            BackgroundImage = Image.FromFile(@"Images\MainMenu.jpg");

            playButton = new Button()
            {
                Text = "»„‡Ú¸",
                ForeColor = Color.DarkRed,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            playButton.FlatAppearance.BorderColor = Color.DarkRed;
            playButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            playButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 0, 0, 0);

            Controls.Add(playButton);

            SizeChanged += UpdateFieldValues;
            SizeChanged += RecalculateTheDimensionsInTheMainMenu;
            RecalculateTheDimensionsInTheMainMenu(null, EventArgs.Empty);

            playButton.Click += LaunchTheLevelSelectionWindow;
        }

        void CloseTheMainMenu()
        {
            Controls.Clear();
            SizeChanged -= RecalculateTheDimensionsInTheMainMenu;
            playButton.Click -= OpenTheLevelSelectionWindow;
            BackgroundImage = null;
        }

        void LaunchTheLevelSelectionWindow(object sender, EventArgs e)
        {
            CloseTheMainMenu();
            OpenTheLevelSelectionWindow(sender, e);
        }

        void RecalculateTheDimensionsInTheMainMenu(object sender, EventArgs e)
        {
            playButton.Location = new Point((int)(initialCoordinate.X + sizeOfTheGridCell * 16.3),
                (int)(initialCoordinate.Y + sizeOfTheGridCell * 11));
            playButton.Size = new Size((int)(6 * sizeOfTheGridCell), (int)(2 * sizeOfTheGridCell));
            playButton.FlatAppearance.BorderSize = (int)sizeOfTheGridCell / 7;

            playButton.Font = new Font(new FontFamily("Courier New"), Math.Max(sizeOfTheGridCell * 0.75f, 1), FontStyle.Bold);
        }
        #endregion

        #region “¿¡À»÷¿ ”–Œ¬Õ≈…
        void OpenTheLevelSelectionWindow(object sender, EventArgs e)
        {
            gameInformationButton = new Button()
            {
                Text = "i",
                TextAlign = ContentAlignment.BottomCenter,
                ForeColor = Color.LightGoldenrodYellow,
                BackgroundImage = Image.FromFile(@"Images\Frame.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            gameInformationButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            gameInformationButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            gameInformationButton.FlatAppearance.BorderColor = Color.Black;
            Controls.Add(gameInformationButton);

            for (var row = 0; row < LevelButtons.GetLength(0); row++)
                for (var column = 0; column < LevelButtons.GetLength(1); column++)
                    ConfigureTheLevelLaunchButton(row, column);

            SizeChanged += RecalculateTheValuesOfTheLevelButtons;
            RecalculateTheValuesOfTheLevelButtons(sender, e);

            gameInformationButton.Click += OpenTheGameInformationWindow;
        }

        void ConfigureTheLevelLaunchButton(int row, int column)
        {
            LevelButtons[row, column] = new Button()
            {
                TextAlign = ContentAlignment.MiddleLeft,
                BackgroundImageLayout = ImageLayout.Zoom,
                FlatStyle = FlatStyle.Flat,
            };

            if (infoAboutTheLevels[row][column].Available)
            {
                LevelButtons[row, column].Text = string.Format("{0}\n\n{1, 5}", infoAboutTheLevels[row][column].Level,
                    $"{infoAboutTheLevels[row][column].Record}/{infoAboutTheLevels[row][column].PossibleNumberOfPoints}");
                LevelButtons[row, column].BackgroundImage = Image.FromFile(@"Images\LevelIsOpen.png");
                LevelButtons[row, column].Click += LevelTriggerFunctions[row, column];
            }
            else LevelButtons[row, column].BackgroundImage = Image.FromFile(@"Images\LevelIsClosed.png");

            LevelButtons[row, column].FlatAppearance.MouseDownBackColor = Color.Black;
            Controls.Add(LevelButtons[row, column]);
        }

        void FillInTheMatrixWithFunctions()
        {
            for (var row = 0; row < LevelTriggerFunctions.GetLength(0); row++)
                for (var column = 0; column < LevelTriggerFunctions.GetLength(1); column++)
                {
                    (var i, var j) = (row, column);

                    LevelTriggerFunctions[i, j] = (object sender, EventArgs e) =>
                    {
                        CloseTheLevelSelectionWindow();
                        OpenTheGame(infoAboutTheLevels[i][j].Level);
                    };
                }
        }

        void CloseTheLevelSelectionWindow()
        {
            Controls.Clear();

            for (var i = 0; i < LevelButtons.GetLength(0); i++)
                for (var j = 0; j < LevelButtons.GetLength(1); j++)
                    LevelButtons[i, j].Click -= LevelTriggerFunctions[i, j];

            SizeChanged -= RecalculateTheValuesOfTheLevelButtons;

            gameInformationButton.Click -= OpenTheGameInformationWindow;
        }

        void RecalculateTheValuesOfTheLevelButtons(object sender, EventArgs e)
        {
            (var numberOfRows, var numberOfColumns) = (LevelButtons.GetLength(0), LevelButtons.GetLength(1));
            var buttonSize = new Size((int)(sizeOfTheGridCell * 4), (int)(sizeOfTheGridCell * 4));
            var distanceBetweenTheButtons = (int)(sizeOfTheGridCell / 2);
            var startingPoint = new PointF(
                (ClientSize.Width - buttonSize.Width * numberOfColumns - ((numberOfColumns - 1) * distanceBetweenTheButtons)) / 2f,
                (ClientSize.Height - buttonSize.Height * numberOfRows - ((numberOfRows - 1) * distanceBetweenTheButtons)) / 2f);

            for (var row = 0; row < numberOfRows; row++)
                for (var column = 0; column < numberOfColumns; column++)
                {
                    LevelButtons[row, column].Location = new Point(
                        (int)(startingPoint.X + buttonSize.Width * column + column * distanceBetweenTheButtons),
                        (int)(startingPoint.Y + buttonSize.Height * row + row * distanceBetweenTheButtons));
                    LevelButtons[row, column].Size = buttonSize;
                    LevelButtons[row, column].Font = new Font(new FontFamily("Courier New"), Math.Max(sizeOfTheGridCell / 1.9f, 1), FontStyle.Bold);
                }

            gameInformationButton.Font = new Font(new FontFamily("Courier New"), Math.Max(sizeOfTheGridCell / 1.7f, 1), FontStyle.Bold);
            gameInformationButton.Location = new Point((int)(ClientSize.Width - 1.2 * sizeOfTheGridCell), 0);
            gameInformationButton.Size = new Size((int)(1.2 * sizeOfTheGridCell), (int)(1.2 * sizeOfTheGridCell));
        }
        #endregion

        #region Œ ÕŒ »Õ‘Œ–Ã¿÷»» Œ¡ »√–≈
        void OpenTheGameInformationWindow(object sender, EventArgs e)
        {
            CloseTheLevelSelectionWindow();

            BackgroundImage = Image.FromFile(@"Images\info.png");

            backButton = new Button()
            {
                BackgroundImage = Image.FromFile(@"Images\BackButton.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            backButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            backButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            backButton.FlatAppearance.BorderColor = Color.Black;
            Controls.Add(backButton);

            SizeChanged += RecalculateTheCoordinatesOfTheGameInformationWindow;
            RecalculateTheCoordinatesOfTheGameInformationWindow(sender, e);

            backButton.Click += ReturnToTheLevelSelectionWindow;
        }

        void ReturnToTheLevelSelectionWindow(object sender, EventArgs e)
        {
            CloseTheGameInformationWindow();
            OpenTheLevelSelectionWindow(sender, e);
        }

        void CloseTheGameInformationWindow()
        {
            Controls.Clear();
            BackgroundImage = null;
            SizeChanged -= RecalculateTheCoordinatesOfTheGameInformationWindow;
            backButton.Click -= ReturnToTheLevelSelectionWindow;
        }

        void RecalculateTheCoordinatesOfTheGameInformationWindow(object sender, EventArgs e)
        {
            backButton.Location = new Point(0, 0);
            backButton.Size = new Size((int)(1.2 * sizeOfTheGridCell), (int)(1.2 * sizeOfTheGridCell));
        }
        #endregion

        #region »√–¿
        public void OpenTheGame(int level)
        {
            CreateGamePanelButtons();
            pauseButton.Enabled = false;
            exitButton.Enabled = false;
            RecalculateTheValuesOfTheGameButtons(null, EventArgs.Empty);
            SizeChanged += RecalculateTheValuesOfTheGameButtons;

            model = new GameModel(new WindowsForm.Model.Map.Playground(level),
                infoAboutTheLevels[(level - 1) / LevelButtons.GetLength(1)][(level - 1) % LevelButtons.GetLength(1)]);
            model.StateChanged += Invalidate;
            model.TheGameIsOver += StartTheNextLevel;

            controller = new Controller(model);
            controller.PauseIsPressed += ChangeThePausePicture;

            Paint += DrawingTheModel;
            Paint += DrawAGamePanel;
            Paint += DrawTheStartOfTheLevel;
            Invalidate();

            Click += StartTheGame;
        }

        void StartTheGame(object sender, EventArgs e)
        {
            pauseButton.Enabled = true;
            exitButton.Enabled = true;
            Click -= StartTheGame;
            Paint -= DrawTheStartOfTheLevel;
            controller.ActivateTimers(model.InfoAboutTheLevel.IntervalForTheAppearanceOfBots);
            ActivateGameManagement();
        }

        void ActivateGameManagement()
        {
            Click += controller.ToShoot;
            pauseButton.Click += controller.PutItOnPause;
            exitButton.Click += OpenAConfirmationWindow;

            KeyDown += controller.MakeAMove;
            MouseWheel += controller.RotateThePlayer;
        }

        void DeactivateGameManagement()
        {
            Click -= controller.ToShoot;
            pauseButton.Click -= controller.PutItOnPause;
            exitButton.Click -= OpenAConfirmationWindow;

            KeyDown -= controller.MakeAMove;
            MouseWheel -= controller.RotateThePlayer;
        }

        void CreateGamePanelButtons()
        {
            pauseButton = new Button()
            {
                BackColor = Color.FromArgb(0, 0, 0, 0),
                BackgroundImage = pauseImages[true],
                BackgroundImageLayout = ImageLayout.Zoom,
                FlatStyle = FlatStyle.Flat
            };
            pauseButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            pauseButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            Controls.Add(pauseButton);

            exitButton = new Button()
            {
                BackColor = Color.FromArgb(0, 0, 0, 0),
                BackgroundImage = Image.FromFile(@"Images\Exit.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                FlatStyle = FlatStyle.Flat
            };
            exitButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            exitButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            Controls.Add(exitButton);
        }

        void EraseThePlayingField()
        {
            Controls.Clear();

            Paint -= DrawingTheModel;
            Paint -= DrawAGamePanel;

            SizeChanged -= RecalculateTheValuesOfTheGameButtons;

            model.StateChanged -= Invalidate;
            model.TheGameIsOver -= StartTheNextLevel;

            controller.StopTimers();
        }

        public void StartTheNextLevel()
        {
            if (!model.Map[model.Player.Location].Contains(model.Player) || model.InfoAboutTheLevel.Level == 18)
                OpenTheResultsWindow();
            else
            {
                RecordTheResults();
                EraseThePlayingField();
                DeactivateGameManagement();
                OpenTheGame(model.InfoAboutTheLevel.Level + 1);
            }
        }

        void RestartTheGame(object sender, EventArgs e)
        {
            RecordTheResults();
            EraseThePlayingField();
            DeactivateGameManagement();
            OpenTheGame(model.InfoAboutTheLevel.Level);
        }

        void DrawingTheModel(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (var x = 0; x < model.Map.Width; x++)
                for (var y = 0; y < model.Map.Height; y++)
                    foreach (var creture in model.Map[x, y])
                    {
                        var image = creture.Picture;
                        var coordinatesOnTheForm = RecalculateTheCoordinatesOnTheForm(new Point(x, y));
                        e.Graphics.DrawImage(image, RotateAnArrayOfPoints(coordinatesOnTheForm, creture.AngleInDegrees * Math.PI / 180));
                    }
        }

        void DrawTheStartOfTheLevel(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            DrawAWindow(graphics,
                new Point((int)(initialCoordinate.X + 9f * sizeOfTheGridCell), (int)(initialCoordinate.Y + sizeOfTheGridCell * 5.3f)),
                new Size((int)(14 * sizeOfTheGridCell), (int)(sizeOfTheGridCell * 3.5f)));

            DrawTheText(graphics, $"”Ó‚ÂÌ¸ {model.InfoAboutTheLevel.Level}", new RectangleF(
                new PointF(initialCoordinate.X + 9f * sizeOfTheGridCell, initialCoordinate.Y + sizeOfTheGridCell * 6f),
                new SizeF(14 * sizeOfTheGridCell, sizeOfTheGridCell * 2.5f)), 1.2f * sizeOfTheGridCell, StringAlignment.Center, FontStyle.Bold);

            DrawTheText(graphics, " ÎËÍÌËÚÂ ÔÓ ˝Í‡ÌÛ, ˜ÚÓ·˚ Ì‡˜‡Ú¸ Ë„Û.", new RectangleF(
                new PointF(initialCoordinate.X + 6f * sizeOfTheGridCell, initialCoordinate.Y + sizeOfTheGridCell * 15f),
                new SizeF(20 * sizeOfTheGridCell, sizeOfTheGridCell * 2.5f)), 0.8f * sizeOfTheGridCell / 2, StringAlignment.Center);
        }

        void DrawAGamePanel(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            DisplayTheTextOnTheGamePanel(graphics);

            DrawAPicture(@"Images\star.png", new PointF(initialCoordinate.X + 18.9f * sizeOfTheGridCell,
                initialCoordinate.Y - sizeOfTheGridCell * 0.7f), new SizeF(sizeOfTheGridCell * 0.7f, sizeOfTheGridCell * 0.7f), graphics);

            for (var i = 0; i < model.Player.Health; i++)
                DrawAPicture(@"Images\heart.png", new PointF(initialCoordinate.X + i * sizeOfTheGridCell,
                    initialCoordinate.Y - sizeOfTheGridCell * 0.7f), new SizeF(sizeOfTheGridCell * 0.7f, sizeOfTheGridCell * 0.7f), graphics);

            DrawAPicture(@"Images\Timer.png", new PointF(initialCoordinate.X + 8.1f * sizeOfTheGridCell,
                initialCoordinate.Y - sizeOfTheGridCell * 0.7f), new SizeF(sizeOfTheGridCell * 0.7f, sizeOfTheGridCell * 0.7f), graphics);
        }

        void DisplayTheTextOnTheGamePanel(Graphics graphics)
        {
            var strings = new string[] { "", "0" };

            DrawTheText(graphics, $"0{model.AmountOfTimeUntilTheEndOfTheRound / 60}:" + strings[2 - (model.AmountOfTimeUntilTheEndOfTheRound % 60).ToString().Length] + $"{model.AmountOfTimeUntilTheEndOfTheRound % 60}",
                new RectangleF(new PointF(initialCoordinate.X + 8.8f * sizeOfTheGridCell, initialCoordinate.Y - sizeOfTheGridCell / 2 * 1.34f),
                    new SizeF(2.5f * sizeOfTheGridCell, sizeOfTheGridCell)), sizeOfTheGridCell / 2.5f);

            DrawTheText(graphics, $"”Ó‚ÂÌ¸: {model.InfoAboutTheLevel.Level}",
                new RectangleF(new PointF(initialCoordinate.X + sizeOfTheGridCell * 24f, initialCoordinate.Y - sizeOfTheGridCell / 2 * 1.34f),
                    new SizeF(5f * sizeOfTheGridCell, sizeOfTheGridCell)), sizeOfTheGridCell / 2.5f,
                StringAlignment.Center);

            DrawTheText(graphics, $"—˜∏Ú: {model.NumberOfBotsDestroyed}",
                new RectangleF(new PointF(initialCoordinate.X + 14.9f * sizeOfTheGridCell, initialCoordinate.Y - sizeOfTheGridCell / 2 * 1.34f),
                    new SizeF(4f * sizeOfTheGridCell, sizeOfTheGridCell)), sizeOfTheGridCell / 2.5f,
                StringAlignment.Far);
        }

        void ChangeThePausePicture(bool gameIsRunning) => pauseButton.BackgroundImage = pauseImages[gameIsRunning];

        void RecalculateTheValuesOfTheGameButtons(object sender, EventArgs e)
        {
            pauseButton.Location = new Point((int)(initialCoordinate.X + sizeOfTheGridCell * (model.Map.Width - 2)),
                (int)(initialCoordinate.Y - sizeOfTheGridCell));
            pauseButton.Size = new Size((int)sizeOfTheGridCell, (int)sizeOfTheGridCell);

            exitButton.Location = new Point((int)(initialCoordinate.X + sizeOfTheGridCell * (model.Map.Width - 1)), (int)(initialCoordinate.Y - sizeOfTheGridCell));
            exitButton.Size = pauseButton.Size;
        }
        #endregion

        #region Œ ÕŒ œŒƒ“¬≈–∆ƒ≈Õ»ﬂ
        void OpenAConfirmationWindow(object sender, EventArgs e)
        {
            exitButton.Enabled = false;
            pauseButton.Enabled = false;
            Paint += DrawAConfirmationWindow;

            if (pauseButton.BackgroundImage == pauseImages[true])
                controller.PutItOnPause(sender, e);

            InitializeConfirmationWindowButton();
            noButton.Click += ContinueTheGame;
            yesButton.Click += SumUpTheGame;

            RecalculateTheCoordinatesOfTheConfirmationWindow(sender, e);
            SizeChanged += RecalculateTheCoordinatesOfTheConfirmationWindow;

            Invalidate();
        }

        void CloseAConfirmationWindow()
        {
            if (yesButton == null) return;

            Controls.Remove(yesButton);
            Controls.Remove(noButton);
            noButton.Click -= ContinueTheGame;
            yesButton.Click -= SumUpTheGame;

            Paint -= DrawAConfirmationWindow;
            SizeChanged -= RecalculateTheCoordinatesOfTheConfirmationWindow;
        }

        void InitializeConfirmationWindowButton()
        {
            yesButton = new Button()
            {
                Text = "ƒ‡",
                ForeColor = Color.LightGoldenrodYellow,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(yesButton);
            yesButton.FlatAppearance.BorderColor = Color.LightGoldenrodYellow;
            yesButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);

            noButton = new Button()
            {
                Text = "ÕÂÚ",
                ForeColor = Color.LightGoldenrodYellow,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat,
            };
            Controls.Add(noButton);
            noButton.FlatAppearance.BorderColor = Color.LightGoldenrodYellow;
            noButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
        }

        void SumUpTheGame(object sender, EventArgs e)
        {
            OpenTheResultsWindow();
            Invalidate();
        }

        void ContinueTheGame(object sender, EventArgs e)
        {
            CloseAConfirmationWindow();

            pauseButton.Enabled = true;
            exitButton.Enabled = true;
            controller.PutItOnPause(sender, e);
        }

        void DrawAConfirmationWindow(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            DrawAWindow(graphics,
                new Point((int)(initialCoordinate.X + 9.5f * sizeOfTheGridCell), (int)(initialCoordinate.Y + 4.5f * sizeOfTheGridCell)),
                new Size((int)(13 * sizeOfTheGridCell), (int)(8f * sizeOfTheGridCell)));

            DrawTheText(graphics, "¬˚ ‰ÂÈÒÚ‚ËÚÂÎ¸ÌÓ ıÓÚËÚÂ Á‡‚Â¯ËÚ¸ Ë„Û?",
                new RectangleF(new PointF(initialCoordinate.X + 9.5f * sizeOfTheGridCell, initialCoordinate.Y + sizeOfTheGridCell * 5.5f),
                new SizeF(13 * sizeOfTheGridCell, sizeOfTheGridCell * 4f)), sizeOfTheGridCell / 1.5f, StringAlignment.Center);
        }

        void RecalculateTheCoordinatesOfTheConfirmationWindow(object sender, EventArgs e)
        {
            yesButton.Location = new Point((int)(initialCoordinate.X + sizeOfTheGridCell * 10.4f),
                (int)(initialCoordinate.Y + sizeOfTheGridCell * 9.8));
            yesButton.Size = new Size() { Width = (int)(5 * sizeOfTheGridCell), Height = (int)(2 * sizeOfTheGridCell) };

            noButton.Size = yesButton.Size;
            noButton.Location = new Point((int)(initialCoordinate.X + sizeOfTheGridCell * 16.6f),
                (int)(initialCoordinate.Y + sizeOfTheGridCell * 9.8));

            noButton.FlatAppearance.BorderSize = (int)sizeOfTheGridCell / 8;
            yesButton.FlatAppearance.BorderSize = (int)sizeOfTheGridCell / 8;

            yesButton.Font = new Font(new FontFamily("Courier New"), Math.Max(sizeOfTheGridCell / 1.8f, 1), FontStyle.Bold);
            noButton.Font = new Font(new FontFamily("Courier New"), Math.Max(sizeOfTheGridCell / 1.8f, 1), FontStyle.Bold);
        }
        #endregion

        #region Œ ÕŒ –≈«”À‹“¿“Œ¬
        void OpenTheResultsWindow()
        {
            RecordTheResults();

            CloseAConfirmationWindow();
            pauseButton.Enabled = false;
            exitButton.Enabled = false;

            controller.StopTimers();
            DeactivateGameManagement();

            InitializeTheButtonsInTheResultsMenu();

            RecalculateTheCoordinatesOfTheButtonsOfTheResetWindow(null, EventArgs.Empty);
            SizeChanged += RecalculateTheCoordinatesOfTheButtonsOfTheResetWindow;

            Paint += DrawTheResultsWindow;

            startOverButton.Click += StartOver;
            buttonToGoToTheLevelSelection.Click += GoToTheLevelSelectionWindow;
            Invalidate();

        }
        void CloseTheResultsWindow()
        {
            Controls.Clear();

            Paint -= DrawTheResultsWindow;
            SizeChanged -= RecalculateTheCoordinatesOfTheButtonsOfTheResetWindow;

            startOverButton.Click -= StartOver;
            buttonToGoToTheLevelSelection.Click -= GoToTheLevelSelectionWindow;
        }

        void GoToTheLevelSelectionWindow(object sender, EventArgs e)
        {
            EraseThePlayingField();
            CloseTheResultsWindow();
            Invalidate();
            OpenTheLevelSelectionWindow(sender, e);
        }

        void InitializeTheButtonsInTheResultsMenu()
        {
            buttonToGoToTheLevelSelection = new Button()
            {
                Text = "¬˚·‡Ú¸ ÛÓ‚ÂÌ¸",
                ForeColor = Color.LightGoldenrodYellow,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(buttonToGoToTheLevelSelection);
            buttonToGoToTheLevelSelection.FlatAppearance.BorderColor = Color.LightGoldenrodYellow;
            buttonToGoToTheLevelSelection.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);

            startOverButton = new Button()
            {
                Text = "Õ‡˜‡Ú¸ Á‡ÌÓ‚Ó",
                ForeColor = Color.LightGoldenrodYellow,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(startOverButton);
            startOverButton.FlatAppearance.BorderColor = Color.LightGoldenrodYellow;
            startOverButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
        }

        void StartOver(object sender, EventArgs e)
        {
            EraseThePlayingField();
            CloseTheResultsWindow();
            OpenTheGame(model.InfoAboutTheLevel.Level);
        }

        void DrawTheResultsWindow(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            DrawAWindow(graphics, new Point((int)(initialCoordinate.X + 9.5 * sizeOfTheGridCell), (int)(initialCoordinate.Y + 5f * sizeOfTheGridCell)), new Size((int)(13 * sizeOfTheGridCell), (int)(7 * sizeOfTheGridCell)));

            DrawTheText(graphics, "»„‡ Á‡‚Â¯ÂÌ‡!",
                new RectangleF(new PointF(initialCoordinate.X + 9 * sizeOfTheGridCell, initialCoordinate.Y + sizeOfTheGridCell * 5.5f),
                new SizeF(14 * sizeOfTheGridCell, sizeOfTheGridCell * 1.5f)), sizeOfTheGridCell / 1.5f,  StringAlignment.Center, FontStyle.Bold, Brushes.Red);

            DrawTheText(graphics, String.Format("{0, -7} {1, 9}", "—˜∏Ú", model.NumberOfBotsDestroyed).Replace(' ', '.'),
                new RectangleF(new PointF(initialCoordinate.X + sizeOfTheGridCell * 10.5f, initialCoordinate.Y + sizeOfTheGridCell * 7f),
                new SizeF(14 * sizeOfTheGridCell, sizeOfTheGridCell * 1.3f)), sizeOfTheGridCell / 1.8f);

            DrawAPicture(@"Images\star.png", new PointF(initialCoordinate.X + sizeOfTheGridCell * 20.6f, initialCoordinate.Y + 7.13f * sizeOfTheGridCell), new SizeF(sizeOfTheGridCell * 0.7f, sizeOfTheGridCell * 0.7f), graphics);
        }

        void RecalculateTheCoordinatesOfTheButtonsOfTheResetWindow(object sender, EventArgs e)
        {
            buttonToGoToTheLevelSelection.Location = new Point((int)(initialCoordinate.X + sizeOfTheGridCell * 10.4f),
                (int)(initialCoordinate.Y + sizeOfTheGridCell * 8.5));
            buttonToGoToTheLevelSelection.Size = new Size() { Width = (int)(5.3 * sizeOfTheGridCell), Height = (int)(3 * sizeOfTheGridCell) };

            startOverButton.Size = buttonToGoToTheLevelSelection.Size;
            startOverButton.Location = new Point((int)(initialCoordinate.X + sizeOfTheGridCell * 16.3f),
                (int)(initialCoordinate.Y + sizeOfTheGridCell * 8.5));

            startOverButton.FlatAppearance.BorderSize = (int)sizeOfTheGridCell / 8;
            buttonToGoToTheLevelSelection.FlatAppearance.BorderSize = (int)sizeOfTheGridCell / 8; ;

            buttonToGoToTheLevelSelection.Font = new Font(new FontFamily("Courier New"), Math.Max(sizeOfTheGridCell / 1.8f, 1), FontStyle.Bold);
            startOverButton.Font = new Font(new FontFamily("Courier New"), Math.Max(sizeOfTheGridCell / 1.8f, 1), FontStyle.Bold);
        }
        #endregion

        #region œ–Œ◊≈≈

        void DrawAWindow(Graphics graphics, Point startingPoint, Size size)
        {
            DrawAPicture(@"Images\haze.png", new PointF(0, 0), ClientSize, graphics);

            graphics.DrawRectangle(new Pen(Color.LightGoldenrodYellow, sizeOfTheGridCell / 5), new Rectangle(startingPoint, size));
        }

        InfoAboutTheLevel[][] ReadTheSavedData()
        {
            return File.ReadAllText(@"LevelData.txt").Split('\n')
                .Take(LevelButtons.GetLength(0))
                .Select(line => line.Split('\t').Select(str => str.Split(';')))
                .Select(line => line.Select(array => new InfoAboutTheLevel(array)).ToArray())
                .ToArray();
        }

        void RecordTheResults()
        {
            if (model.AmountOfTimeUntilTheEndOfTheRound == 0 && model.InfoAboutTheLevel.Level != 18)
            {
                infoAboutTheLevels[model.InfoAboutTheLevel.Level / LevelButtons.GetLength(1)][model.InfoAboutTheLevel.Level % LevelButtons.GetLength(1)].Available = true;
            }

            if (!model.InfoAboutTheLevel.Available ||
                model.InfoAboutTheLevel.Record < model.NumberOfBotsDestroyed)
            {
                model.InfoAboutTheLevel.Available = true;
                model.InfoAboutTheLevel.Record = model.NumberOfBotsDestroyed;
            }
        }

        void SaveTheGameResults(object sender, FormClosingEventArgs e)
        {
            RecordTheResults();

            File.WriteAllLines(@"LevelData.txt", infoAboutTheLevels
                    .Select(line => string.Join("\t", line.Select(info => info.ToString()))));
        }

        static void DrawTheText(Graphics graphics, string text, RectangleF location, float fontSize, StringAlignment alignment = StringAlignment.Near, FontStyle outline = FontStyle.Regular, Brush brushes = null)
        {
            graphics.DrawString(text,
                new Font("Courier New", Math.Max(fontSize, 1), outline),
                brushes ?? Brushes.LightGoldenrodYellow,
                location,
                new StringFormat() { Alignment = alignment });
        }

        static void DrawAPicture(string pathToTheFile, PointF location, SizeF size, Graphics graphics)
        {
            graphics.DrawImage(Image.FromFile(pathToTheFile), 
                [location, new(location.X + size.Width, location.Y), new(location.X, location.Y + size.Height)]);
        }

        void UpdateFieldValues(object sender, EventArgs e)
        {
            sizeOfTheGridCell = Math.Min(ClientSize.Height / (model.Map.Height + 1), ClientSize.Width / model.Map.Width);

            initialCoordinate = new PointF((ClientSize.Width - sizeOfTheGridCell * model.Map.Width) / 2,
                (ClientSize.Height - sizeOfTheGridCell * (model.Map.Height + 1)) / 2 + sizeOfTheGridCell);
        }

        PointF[] RecalculateTheCoordinatesOnTheForm(Point positionOnTheMap)
        {
            return [ new(positionOnTheMap.X * sizeOfTheGridCell + initialCoordinate.X, positionOnTheMap.Y * sizeOfTheGridCell + initialCoordinate.Y),
                new(positionOnTheMap.X * sizeOfTheGridCell + initialCoordinate.X + sizeOfTheGridCell, positionOnTheMap.Y * sizeOfTheGridCell + initialCoordinate.Y),
                new(positionOnTheMap.X * sizeOfTheGridCell + initialCoordinate.X, positionOnTheMap.Y * sizeOfTheGridCell + initialCoordinate.Y + sizeOfTheGridCell) ];
        }

        PointF[] RotateAnArrayOfPoints(PointF[] points, double turn)
        {
            var centre = new PointF(points[0].X + (points[1].X - points[0].X) / 2, points[0].Y + (points[2].Y - points[0].Y) / 2);

            var point1 = RotateAPoint(new PointF(points[0].X - centre.X, points[0].Y - centre.Y), turn);
            var point2 = RotateAPoint(new PointF(points[1].X - centre.X, points[1].Y - centre.Y), turn);
            var point3 = RotateAPoint(new PointF(points[2].X - centre.X, points[2].Y - centre.Y), turn);

            return [ new(centre.X + point1.X,centre.Y + point1.Y), new(centre.X + point2.X, centre.Y + point2.Y), new PointF(centre.X + point3.X, centre.Y + point3.Y) ];
        }

        static PointF RotateAPoint(PointF point, double angleInRadians)
        {
            var d = Math.Sqrt(point.X * point.X + point.Y * point.Y);
            var angle = Math.Atan2(point.Y, point.X) + angleInRadians;

            return new PointF((float)(Math.Cos(angle) * d), (float)(Math.Sin(angle) * d));
        }
        #endregion
    }
}
