namespace WindowsForm.Model.GameEntities
{
    public class FirstAid : GameObjects
    {
        public FirstAid(Point location, string pathToTheFile = @"Images\FirstAidKit.png", int health = 1, int priority = 0, int angleInDegrees = 0) : base(location, pathToTheFile, health, 1, priority, angleInDegrees)
        {
        }

        public override void DeductDamage() => Health--;
    }
}
