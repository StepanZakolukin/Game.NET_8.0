using WindowsForm.Model.GameEntities;
using WindowsForm.Model.Map;
using WindowsForm.View;

namespace WindowsForm.Model
{
    public class GameModel
    {
        public event Action StateChanged;
        public event Action TheGameIsOver;
        public List<Bot> ArmyOfBots { get; private set; }
        public Playground Map { get; private set; }
        public Player Player { get; private set; }
        public int AmountOfTimeUntilTheEndOfTheRound { get; set; }
        private int numberOfBotsInTheGame;
        public int NumberOfBotsDestroyed { get; private set; }
        public readonly InfoAboutTheLevel InfoAboutTheLevel;

        public GameModel(Playground map, InfoAboutTheLevel info)
        {
            InfoAboutTheLevel = info;
            Map = map;
            var playerLocation = FindAPositionToCreateAnOject();
            Map[playerLocation].Add(new Player(new Random().Next(1, 5) * 90, playerLocation));
            Player = (Player)Map[playerLocation].Last();
            ArmyOfBots = new List<Bot>();
            CreateBots();
            var firstAidKit = new FirstAid(FindAPositionToCreateAnOject());
            Map[firstAidKit.Location].Add(firstAidKit);

            MineTheMap(info.NumberOfMines);
            AmountOfTimeUntilTheEndOfTheRound = info.DurationInSeconds;
        }

        private void MineTheMap(int quantity)
        {
            for (var i = 0; i < quantity; i++)
            {
                var location = FindAPositionToCreateAnOject();
                Map[location].Add(new Mine(location));
            }
        }

        public List<GameObjects>[,] GetCandidatesPerLocation()
        {
            var creatures = new List<GameObjects>[Map.Width, Map.Height];

            for (var x = 0; x < Map.Width; x++)
                for (var y = 0; y < Map.Height; y++)
                    creatures[x, y] = new();

            for (var x = 0; x < Map.Width; x++)
                for (var y = 0; y < Map.Height; y++)
                    foreach(var creature in Map[x, y])
                    {
                        if (creature.Health < 1) continue;
                        var targetLogicalLocation = creature.Location + creature.Delta;
                        creatures[targetLogicalLocation.X, targetLogicalLocation.Y].Add(creature);
                    }

            return creatures;
        }

        public void ExecuteTheCommandsOfTheHeroes()
        {
            var creaturesPerLocation = GetCandidatesPerLocation();

            for (var x = 0; x < Map.Width; x++)
                for (var y = 0; y < Map.Height; y++)
                {
                    Map[x, y] = SelectWinnerCandidatePerLocation(creaturesPerLocation, x, y);

                    foreach(var creature in Map[x, y])
                        creature.CommandAreExecuted(x, y);
                }

            StateChanged();
            if (AmountOfTimeUntilTheEndOfTheRound < 1 || !Map[Player.Location].Contains(Player)
                || NumberOfBotsDestroyed == InfoAboutTheLevel.PossibleNumberOfPoints) TheGameIsOver();
        }

        private List<GameObjects> SelectWinnerCandidatePerLocation(List<GameObjects>[,] creatures, int x, int y)
        {
            var sortedСreatures = creatures[x, y].OrderBy(creature => creature.Priority).ToList();

            if (Player.Location + Player.Delta == new Point(x, y) && sortedСreatures.Any(creature => creature is FirstAid))
                Player.Treat();

            if (sortedСreatures.Any(creature => creature is Characters) && 
                sortedСreatures
                .Where(creature => creature is Mine)
                .FirstOrDefault() is Mine mine) mine.BlowUp(sortedСreatures);

            for (var j = 1; j < sortedСreatures.Count; j++)
                for (var i = 0; i < sortedСreatures.Count - j; i++)
                    if (sortedСreatures[sortedСreatures.Count - j].DeadInConflict(sortedСreatures[i]))
                    {
                        sortedСreatures[i].DeductDamage();
                        if (sortedСreatures[i].DeadInConflict(sortedСreatures[sortedСreatures.Count - j]))
                            sortedСreatures[sortedСreatures.Count - j].DeductDamage();
                    }

            return sortedСreatures.OrderBy(creature => creature.RenderingPriority).ToList();
        }

        public void CreateBots()
        {
            if (InfoAboutTheLevel.PossibleNumberOfPoints == numberOfBotsInTheGame) return;

            var random = new Random();

            for (var i = 0; i < InfoAboutTheLevel.NumberOfBotsAtATime; i++)
            {
                var location = FindAPositionToCreateAnOject();
                Map[location].Add(new Bot(location, random.Next(1, 5) * 90));
                ArmyOfBots.Add((Bot)Map[location].Last());
                numberOfBotsInTheGame++;
            }
        }

        public void CreateAFirstAidKit()
        {
            var location = FindAPositionToCreateAnOject();

            Map[location].Add(new FirstAid(location));
        }

        public void SetTheBotsInMotion(GameModel model)
        {
            ArmyOfBots = ArmyOfBots
                .Where(bot => Map[bot.Location].Contains(bot))
                .ToList();

            NumberOfBotsDestroyed = numberOfBotsInTheGame - ArmyOfBots.Count;

            foreach (var bot in ArmyOfBots)
                bot.MakeAMove(model);
        }

        private Point FindAPositionToCreateAnOject()
        {
            var random = new Random();
            (var x, var y) = (int.MaxValue, int.MaxValue);

            while (!Map.InBounds(new Point(x, y)) || !Map[x, y].All(creature => creature is Stone) 
                || Player != null && (Math.Abs(Player.Location.X - x) < 4 || Math.Abs(Player.Location.Y - y) < 4))
            {
                (x, y) = (random.Next(1, Map.Width - 1), random.Next(1, Map.Height - 1));
            }

            return new Point(x, y);
        }
    }
}