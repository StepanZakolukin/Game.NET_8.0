namespace WindowsForm.Model.GameEntities
{
    public class Explosion : GameObjects
    {
        public static readonly Image[] Pictures = [
            Image.FromFile(@"Images\Explosion2.png"),
            Image.FromFile(@"Images\Explosion3.png"),
            Image.FromFile(@"Images\Explosion4.png"),
            Image.FromFile(@"Images\Explosion5.png"),
            Image.FromFile(@"Images\Explosion6.png"),
            Image.FromFile(@"Images\Explosion7.png"),
            Image.FromFile(@"Images\Explosion8.png")
        ];

        public Explosion(Point location, string pathToTheFile = @"Images\Explosion1.png", int health = 8, int renderingPriority = 3, int priority = 0, int angleInDegrees = 0) : base(location, pathToTheFile, health, renderingPriority, priority, angleInDegrees)
        {
        }

        public override void CommandAreExecuted(int x, int y)
        {
            Health--;
            if (Health != 0) Picture = Pictures[7 - Health];
            base.CommandAreExecuted(x, y);
        }
    }
}
