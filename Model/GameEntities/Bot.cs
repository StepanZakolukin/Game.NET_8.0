using WindowsForm.Model.Map;

namespace WindowsForm.Model.GameEntities
{
    public class Bot : Characters
    {
        public Bot(Point location, int angleInDegrees, string pathToTheFile = @"Images\Bot.png", int health = 1)
            : base(location, pathToTheFile, health, angleInDegrees)
        {
        }

        public override bool DeadInConflict(GameObjects gameObjects) => 
            !(gameObjects is Stone || gameObjects == this);

        public void MakeAMove(GameModel model)
        {
            if (!model.Map[model.Player.Location].Contains(model.Player)) return;

            if (TryToExecuteAShotOrTurnAroundForAShot(model)) return;

            foreach(var followingLocation in FindAWay(model.Map, model.Player.Location, Walker.OfSets
                .Select(ofset => Location + ofset)
                .ToHashSet()))
            {
                if (CheckIfThePositionIsAvailable(followingLocation.Value, model))
                {
                    Delta = followingLocation.Value - Location;
                    break;
                }
            }
        }

        public IEnumerable<SinglyLinkedList<Point>> FindAWay(Playground map, Point finish, HashSet<Point> startingPositions)
        {
            var queue = new Queue<SinglyLinkedList<Point>>();
            queue.Enqueue(new SinglyLinkedList<Point>(finish));
            var visited = new HashSet<Point>() { finish };

            while (queue.Count > 0)
            {
                var point = queue.Dequeue();

                if (!map.InBounds(point.Value) || !map[point.Value].All(creature => creature is Mine || creature is Explosion || !creature.DeadInConflict(this)))
                    continue;

                if (startingPositions.Contains(point.Value)) yield return point;

                foreach (var ofset in Walker.OfSets)
                {
                    var nextPoint = point.Value + ofset;

                    if (visited.Contains(nextPoint)) continue;

                    visited.Add(nextPoint);
                    queue.Enqueue(new SinglyLinkedList<Point>(nextPoint, point));
                }
            }

            yield break;
        }

        private bool CheckIfThePositionIsAvailable(Point location, GameModel model)
        {
            if (location == model.Player.Location + model.Player.Delta) return false;

            foreach (var bot in model.ArmyOfBots)
            {
                if (bot == this) break;
                if (bot.Location + bot.Delta == location) return false;
            }

            return true;
        }

        bool TryToExecuteAShotOrTurnAroundForAShot(GameModel model)
        {
            (var actionIsCompleted, var distance) = (false, Location - model.Player.Location);
            if (distance.Y < 0 && distance.X == 0 && Enumerable.Range(Location.Y + 1,
                Math.Abs(distance.Y) - 1).All(y => model.Map[Location.X, y].All(creature => !(creature is Wall) && !(creature is Bot))))
            {
                if (AngleInDegrees == 0 || AngleInDegrees == 270) AngleInDegrees += 90;
                else if (AngleInDegrees == 180) AngleInDegrees += 270;
                else Shoot(model);
                actionIsCompleted = true;
            }
            else if (distance.Y > 0 && distance.X == 0 && Enumerable.Range(model.Player.Location.Y + 1,
                distance.Y - 1).All(y => model.Map[Location.X, y].All(creature => !(creature is Wall) && !(creature is Bot))))
            {
                if (AngleInDegrees == 0) AngleInDegrees += 270;
                else if (AngleInDegrees == 90 || AngleInDegrees == 180) AngleInDegrees += 90;
                else Shoot(model);
                actionIsCompleted = true;
            }
            else if (distance.X < 0 && distance.Y == 0 && Enumerable.Range(Location.X + 1,
                Math.Abs(distance.X) - 1).All(x => model.Map[x, Location.Y].All(creature => !(creature is Wall) && !(creature is Bot))))
            {
                if (AngleInDegrees == 90) AngleInDegrees += 270;
                else if (AngleInDegrees == 180 || AngleInDegrees == 270) AngleInDegrees += 90;
                else Shoot(model);
                actionIsCompleted = true;
            }
            else if (distance.X > 0 && distance.Y == 0 && Enumerable.Range(model.Player.Location.X + 1,
                distance.X - 1).All(x => model.Map[x, Location.Y].All(creature => !(creature is Wall) && !(creature is Bot))))
            {
                if (AngleInDegrees == 0 || AngleInDegrees == 90) AngleInDegrees += 90;
                else if (AngleInDegrees == 270) AngleInDegrees += 270;
                else Shoot(model);
                actionIsCompleted = true;
            }
            AngleInDegrees %= 360;
            return actionIsCompleted;
        }
    }
}
