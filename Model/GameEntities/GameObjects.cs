using System.Drawing;

namespace WindowsForm.Model.GameEntities
{
    public class GameObjects
    {
        public readonly int Priority;
        public Image Picture { get; protected set; }
        public int AngleInDegrees { get; protected set; }
        public Point Location { get; protected set; }
        public Point Delta { get; protected set; }
        public int Health { get; protected set; }
        public readonly int RenderingPriority;
        public GameObjects(Point location, string pathToTheFile, int health, int renderingPriority, int priority = 0, int angleInDegrees = 90)
        {
            Picture = Image.FromFile(pathToTheFile);
            Location = location;
            Delta = new Point(0, 0);
            AngleInDegrees = angleInDegrees;
            Health = health;
            Priority = priority;
            RenderingPriority = renderingPriority;
        }

        public virtual void DeductDamage()
        {
        }

        public virtual void CommandAreExecuted(int x, int y) => Location = new Point(x, y);

        public virtual bool DeadInConflict(GameObjects gameObjects) => false;
    }
}
