namespace WindowsForm.View
{
    public class InfoAboutTheLevel
    {
        public readonly int Level;
        public bool Available { get; set; }
        public int Record { get; set; }
        public readonly int PossibleNumberOfPoints;
        public readonly int IntervalForTheAppearanceOfBots;
        public readonly int NumberOfBotsAtATime;
        public readonly int NumberOfMines;
        public readonly int DurationInSeconds;

        public InfoAboutTheLevel(params string[] data)
        {
            Level = int.Parse(data[0]);
            Available = bool.Parse(data[1]);
            Record = int.Parse(data[2]);
            PossibleNumberOfPoints = int.Parse(data[3]);
            IntervalForTheAppearanceOfBots = int.Parse(data[4]) * 1000;
            NumberOfBotsAtATime = int.Parse(data[5]);
            NumberOfMines = int.Parse(data[6]);
            DurationInSeconds = int.Parse(data[7]);
        }

        public override string ToString() => 
            $"{Level};{Available};{Record};{PossibleNumberOfPoints};{IntervalForTheAppearanceOfBots / 1000};{NumberOfBotsAtATime};{NumberOfMines};{DurationInSeconds}";
    }
}
