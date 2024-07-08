namespace WindowsForm.Model.GameEntities
{
    public class Stone : GameObjects
    {
        public Stone(Point location, string pathToTheFile = @"Images\Stone.jpg") : base(location, pathToTheFile, int.MaxValue, 0)
        {
            AngleInDegrees = 0;
        }

        public override bool DeadInConflict(GameObjects gameObjects) => gameObjects is Explosion;
    }
}
