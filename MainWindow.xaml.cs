using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MineSweeperGame
{

    public partial class MainWindow : Window
    {
        private List<Field> fields = new List<Field> { };
        private List<Point> randomBombs = new List<Point> { };
        private int dimensions = 5;
        private bool gameIsFinished = false;

        public MainWindow()
        {
            InitializeComponent();

            Random randNum = new Random();
            int Min = 0;
            int Max = 4;

            randomBombs = generateRandomPoints(Min, Max, this.dimensions);


            for (int x = 0; x < this.dimensions; x++)
            {

                this.mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                this.mainGrid.RowDefinitions.Add(new RowDefinition());


                for (int y = 0; y < this.dimensions; y++)
                {
                    fields.Add(AddButton(x, y, this.mainGrid, fields.Count())); 
                }
            }

            foreach (Field field in fields)
            {
                Point buttonPoint = field.point;
                Button button = field.button;

                //@todo optimize
                foreach(Point randomBomb in randomBombs)
                {
                    bool found = randomBomb.x == buttonPoint.x && randomBomb.y == buttonPoint.y;

                    if (found) {
                        field.state = new State(0, 0, true);
                    }
                }

                if (field.state == null) {
                    field.state = new State(0, 0, false);
                }
            }

            foreach (Field field in fields)
            {
                Point buttonPoint = field.point;

                field.state.number = getBombsAround(buttonPoint);
                field.button.Foreground = Brushes.Black;
            }
        }

        public int getBombsAround(Point buttonPoint) {
            List<Point> points = new List<Point>
            {
                randomBombs.Find(point => point.x == buttonPoint.x + 1 && point.y == buttonPoint.y),
                randomBombs.Find(point => point.x == buttonPoint.x - 1 && point.y == buttonPoint.y),
                randomBombs.Find(point => point.x == buttonPoint.x && point.y == buttonPoint.y + 1),
                randomBombs.Find(point => point.x == buttonPoint.x && point.y == buttonPoint.y - 1),
                randomBombs.Find(point => point.x == buttonPoint.x + 1 && point.y == buttonPoint.y + 1),
                randomBombs.Find(point => point.x == buttonPoint.x - 1 && point.y == buttonPoint.y - 1),
                randomBombs.Find(point => point.x == buttonPoint.x - 1 && point.y == buttonPoint.y + 1),
                randomBombs.Find(point => point.x == buttonPoint.x + 1 && point.y == buttonPoint.y - 1)
            };

            List<Point> filtered = points.FindAll(e => e != null);

            return filtered.Count();
        }

        public Point generateValidRandomPoint(int min, int max, List<Point>points)
        {
            Point validPoint = getRandomPoint(min, max);
            Point found = points.Find(point => point.x == validPoint.x && point.y == validPoint.y);

            if (found != null) return generateValidRandomPoint(min, max, points);
            else return validPoint;
        }


        public List<Point> generateRandomPoints(int min, int max, int count) {
            List<Point> points = new List<Point> { };

            for (int i = 0; i < count; i++) {
                Point point = generateValidRandomPoint(min, max, points);
                points.Add(point);
            }

            return points;
        }

        public Point getRandomPoint(int min, int max) {
            Random randNum = new Random();

            return new Point(randNum.Next(min, max), randNum.Next(min, max));
        }

        public class Point
        {
            public int x { get; set; }
            public int y { get; set; }

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public class Field {
            public Point point { get; set; }
            public State state { get; set; }

            public Button button { get; set; }

            public Field(Point point, Button button) {
                this.point = point;
                this.button = button;
            }
        }

        Field AddButton(int row, int column, Grid parent, int id)
        {

            Button button = new Button();
            button.Width = 125/ this.dimensions;
            button.Height = 125/ this.dimensions;
            button.Padding = new Thickness(5);
            button.Click += Button_Click;
            button.MouseDown += Button_Right_Click;
            button.Background = Brushes.LightGray;
            button.Tag = id;

            parent.Children.Add(button);

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);

            return new Field(new Point(row, column), button);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!this.gameIsFinished) {
                Button btn = sender as Button;
                State state = fields[(Int32)btn.Tag].state;

                if (state != null && state.isBomb)
                {
                    this.showAllBombs();
                }
                else
                {
                    btn.Background = Brushes.White;

                    if (state.number != null)
                    {
                        btn.Content = state.number;
                    }
                    else
                    {
                        btn.Content = "";
                    }
                    
                }
            }
        }

        public void showAllBombs() {
            this.gameIsFinished = true;

            foreach (Field field in fields) {
                if (field.state != null && field.state.isBomb) {
                    field.button.Content = "B";
                    field.button.Background = Brushes.Red;
                }
            }
        }

        private void Button_Right_Click(object sender, MouseButtonEventArgs e)
        {
            if (!this.gameIsFinished) {
                Button btn = sender as Button;
                State state = fields[(Int32)btn.Tag].state;



                //if (e.ChangedButton == MouseButton.Right && btn.Tag != "clicked")
                //{

                //    btn.Tag = "clicked";
                //    btn.Background = Brushes.Yellow;
                //    btn.Content = new Image
                //    {
                //        //Source = new BitmapImage(new Uri("/MineSweeperGame;component/assets/mask2.png", UriKind.RelativeOrAbsolute)),
                //        Source = new BitmapImage(new Uri("C:\\Users\\cieni\\source\\repos\\MineSweeperGame\\assets\\Bitmap1.bmp", UriKind.RelativeOrAbsolute)),

                //        VerticalAlignment = VerticalAlignment.Center,

                //        Height = 15,
                //        Width = 15
                //    };
                //}
                //else if (btn.Tag == "clicked")
                //{
                //    btn.Tag = "";
                //    btn.Content = "";
                //    btn.Background = Brushes.LightGray;
                //}
            }
        }
    }

    public class State
    {
        private string[] colors = { "lightGray", "white", "red", "yellow" };
        public int id { get; set; }
        public string color { get; set; }
        public int? number { get; set; }
        public bool isBomb { get; set; }

        public State(int id, int? number, bool isBomb) {
            this.id = id;
            this.color = this.getColor(id);
            this.number = number;
            this.isBomb = isBomb;
        }

        private string getColor(int id) {
            return this.colors[id];
        }
    }
}
