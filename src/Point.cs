using System;
using System.Collections.Generic;
using System.Linq;

namespace MineSweeperGame.src
{
    /// <summary>
    /// Point - object with x and y coordinate
    /// </summary>
    public class Point
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Y coordinate
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Point initialization
        /// </summary>
        /// <param name="x">coordinate</param>
        /// <param name="y">coordinate</param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Generate random points - bombs destination
        /// </summary>
        /// <param name="min">minimum number</param>
        /// <param name="max">maximum number</param>
        /// <param name="count">count of point to generate</param>
        /// <returns>List of random points</returns>
        static public List<Point> GenerateRandomPoints(int min, int max, int count)
        {
            List<Point> points = new List<Point> { };

            for (int i = 0; i < count; i++)
            {
                Point point = GenerateValidRandomPoint(min, max, points);
                points.Add(point);
            }

            return points;
        }

        /// <summary>
        /// Generate random point which doesn't exists in provided list
        /// </summary>
        /// <param name="min">minimum number</param>
        /// <param name="max">maximum number</param>
        /// <param name="points">list of points</param>
        /// <returns>Random point</returns>
        static public Point GenerateValidRandomPoint(int min, int max, List<Point> points)
        {
            Point validPoint = GetRandomPoint(min, max);
            Point found = points.Find(point => point.X == validPoint.X && point.Y == validPoint.Y);

            if (found != null) return GenerateValidRandomPoint(min, max, points);
            else return validPoint;
        }

        /// <summary>
        /// Get random point - object with x and y coordinate
        /// </summary>
        /// <param name="min">minimum number</param>
        /// <param name="max">maximum number</param>
        /// <returns>Random point</returns>
        static public Point GetRandomPoint(int min, int max)
        {
            Random randNum = new Random();

            return new Point(randNum.Next(min, max), randNum.Next(min, max));
        }

        /// <summary>
        /// Count number of bombs around provided point
        /// </summary>
        /// <param name="buttonPoint">the point the user clicked</param>
        /// <param name="randomBombs">points where is are the bombs</param>
        /// <returns>Bombs count</returns>
        static public int GetBombsAround(Point buttonPoint, List<Point> randomBombs)
        {
            List<Point> points = new List<Point>
            {
                randomBombs.Find(point => point.X == buttonPoint.X + 1 && point.Y == buttonPoint.Y),
                randomBombs.Find(point => point.X == buttonPoint.X - 1 && point.Y == buttonPoint.Y),
                randomBombs.Find(point => point.X == buttonPoint.X && point.Y == buttonPoint.Y + 1),
                randomBombs.Find(point => point.X == buttonPoint.X && point.Y == buttonPoint.Y - 1),
                randomBombs.Find(point => point.X == buttonPoint.X + 1 && point.Y == buttonPoint.Y + 1),
                randomBombs.Find(point => point.X == buttonPoint.X - 1 && point.Y == buttonPoint.Y - 1),
                randomBombs.Find(point => point.X == buttonPoint.X - 1 && point.Y == buttonPoint.Y + 1),
                randomBombs.Find(point => point.X == buttonPoint.X + 1 && point.Y == buttonPoint.Y - 1)
            };

            List<Point> filtered = points.FindAll(e => e != null);

            return filtered.Count();
        }
    }
}
