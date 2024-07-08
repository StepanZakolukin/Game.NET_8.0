namespace WindowsForm.Model.GameEntities
{
    public class Bullet : GameObjects
    {
        public Bullet(int angleInDegrees, Point location, string pathToTheFile = @"Images\Bullet.png", int health = 1)
            : base(location, pathToTheFile, health, 1, 2)
        {
            AngleInDegrees = angleInDegrees;
            Forward();
        }

        public override void DeductDamage() => Health--;

        public void Forward() => Delta = Walker.MovingForwad[AngleInDegrees % 360];

        public override bool DeadInConflict(GameObjects gameObjects)
            => !(gameObjects is Bullet || gameObjects is Stone || gameObjects is FirstAid || gameObjects is Mine);
    }
}
