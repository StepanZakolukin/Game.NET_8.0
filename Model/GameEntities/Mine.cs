namespace WindowsForm.Model.GameEntities
{
    public class Mine : GameObjects
    {
        public Mine(Point location, string pathToTheFile = @"Images\Mine.png", int health = 1, int renderingPriority = 3, int priority = 0, int angleInDegrees = 90)
            : base(location, pathToTheFile, health, renderingPriority, priority, angleInDegrees)
        {
        }

        public override bool DeadInConflict(GameObjects gameObjects) => gameObjects is Bot || gameObjects is Player;

        public override void DeductDamage() => Health--;

        public void BlowUp(List<GameObjects> creatures) => creatures.Add(new Explosion(Location));
    }
}
