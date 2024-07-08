using System.Collections.Generic;

namespace WindowsForm.Model
{
    public static class Walker
    {
        public static Dictionary<int, Point> MovingForwad = new Dictionary<int, Point>()
        {
            [0] = new Point(1, 0),
            [90] = new Point(0, 1),
            [180] = new Point(-1, 0),
            [270] = new Point(0, -1)
        };

        public static Point[] OfSets = new Point[] { new Point(0, 1), new Point(0, -1), new Point(-1, 0), new Point(1, 0) };
    }
}
