namespace WindowsForm.Model.GameEntities
{
    public class Wall : GameObjects
    {
        public Wall(Point location, string pathToTheFile = @"Images\Wall.jpg") : base(location, pathToTheFile, int.MaxValue, 3, 3)
        {
            AngleInDegrees = 0;
        }

        public override bool DeadInConflict(GameObjects gameObjects) => !(gameObjects is Wall);
    }
}
