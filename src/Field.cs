using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MineSweeperGame.src
{
    /// <summary>
    /// Smalest piece of grid which contains data about bomb, number of bombs around and clicable button
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Point instance
        /// </summary>
        public Point Point { get; set; }
        /// <summary>
        /// State instance
        /// </summary>
        public State State { get; set; }

        /// <summary>
        /// Button instance
        /// </summary>
        public Button Button { get; set; }

        /// <summary>
        /// Initialize field with button and point
        /// </summary>
        /// <param name="point">point instance</param>
        /// <param name="button">button instance</param>
        public Field(Point point, Button button)
        {
            this.Point = point;
            this.Button = button;
        }

        /// <summary>
        /// Add new button to Grid and set as new field
        /// </summary>
        /// <param name="row">row position</param>
        /// <param name="column">column position</param>
        /// <param name="parent">parent node</param>
        /// <param name="id">button id</param>
        /// <returns>New Field with button attached to grid</returns>
        static public Field AddButton(int row, int column, Grid parent, int id)
        {

            Button button = new Button
            {
                Width = 25,
                Height = 25,
                Padding = new Thickness(5),
                Background = Brushes.LightGray,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Tag = id
            };

            parent.Children.Add(button);

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);

            return new Field(new Point(row, column), button);
        }
    }
}
