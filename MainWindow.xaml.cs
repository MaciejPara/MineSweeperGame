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

    //System.Diagnostics.Debug.WriteLine("");

    public partial class MainWindow : Window
    {
        private List<Field> fields = new List<Field> { };
        private List<Point> randomBombs = new List<Point> { };
        private int dimensions = 5;
        private bool gameIsFinished = false;
        private int clickedFields;
        private string startTime;
        public int bombFlags = 0;

        public MainWindow()
        {
            //System.Diagnostics.Debug.WriteLine("init");
            InitializeComponent();
        }

        public void clearAllFieldsAround(Point buttonPoint)
        {
            
            List<Field> foundFields = new List<Field>
            {
                fields.Find(field => field.point.x + 1 == buttonPoint.x && field.point.y == buttonPoint.y),
                fields.Find(field => field.point.x - 1 == buttonPoint.x && field.point.y == buttonPoint.y),
                fields.Find(field => field.point.x == buttonPoint.x && field.point.y + 1 == buttonPoint.y),
                fields.Find(field => field.point.x == buttonPoint.x && field.point.y - 1 == buttonPoint.y),
                fields.Find(field => field.point.x + 1 == buttonPoint.x && field.point.y + 1 == buttonPoint.y),
                fields.Find(field => field.point.x + 1 == buttonPoint.x && field.point.y - 1 == buttonPoint.y),
                fields.Find(field => field.point.x - 1 == buttonPoint.x && field.point.y + 1 == buttonPoint.y),
                fields.Find(field => field.point.x - 1 == buttonPoint.x && field.point.y - 1 == buttonPoint.y)
            };

            List<Field> filtered = foundFields.FindAll(e => e != null);

            foreach (Field filteredField in filtered) {

                if (!filteredField.state.clicked) {
                    filteredField.button.Background = Brushes.White;
                    filteredField.state.clicked = true;

                    countClick();

                    if (filteredField.state.number == null)
                    {
                        clearAllFieldsAround(filteredField.point);
                    }
                    else 
                    {
                        filteredField.button.Content = filteredField.state.number;
                    }
                }
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

        private void countClick() {
            clickedFields++;

            if (clickedFields == fields.Count() - randomBombs.Count()) {
                resetGame();
                showWinInfo();
            }
        }

        Field AddButton(int row, int column, Grid parent, int id)
        {

            Button button = new Button();
            button.Width = 25;
            button.Height = 25;
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
                Point point = fields[(Int32)btn.Tag].point;

                if (state.id != 3) {
                    if (state != null && state.isBomb)
                    {
                        this.showAllBombs();
                    }
                    else
                    {
                        btn.Background = Brushes.White;
                        state.clicked = true;

                        countClick();

                        if (state.number != null)
                        {
                            btn.Content = state.number;
                        }
                        else
                        {
                            this.clearAllFieldsAround(point);

                            btn.Content = "";
                        }

                    }
                }
            }
        }

        public void showWinInfo() {
            this.info.Visibility = Visibility.Visible;
            this.time.Visibility = Visibility.Visible;

            this.info.Content = "You won with time: ";
            this.time.Content = (DateTime.Now - Convert.ToDateTime(this.startTime)).ToString("c");
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

                if (e.ChangedButton == MouseButton.Right && !state.clicked && state.id != 3 && this.bombFlags > 0)
                {
                    state.id = 3;
                    btn.Background = Brushes.Yellow;
                    btn.Content = new Image
                    {
                        //Source = new BitmapImage(new Uri("/MineSweeperGame;component/assets/mask2.png", UriKind.RelativeOrAbsolute)),
                        Source = new BitmapImage(new Uri("C:\\Users\\cieni\\source\\repos\\MineSweeperGame\\assets\\Bitmap1.bmp", UriKind.RelativeOrAbsolute)),

                        VerticalAlignment = VerticalAlignment.Center,

                        Height = 15,
                        Width = 15
                    };
                    
                    this.showCurrentFlags(-1);
                }
                else if (state.id == 3)
                {
                    state.id = 0;
                    btn.Content = "";
                    btn.Background = Brushes.LightGray;
                    this.showCurrentFlags(1);
                }
            }
        }

        private void showCurrentFlags(int number = 0)
        {
            this.bombFlags += number;
            this.flags.Content = "Flags: " + this.bombFlags;
        }

        private void initGame()
        {
            Random randNum = new Random();
            int Min = 0;
            int Max = this.dimensions;
            
            

            this.clickedFields = 0;
            this.info.Visibility = Visibility.Hidden;
            this.time.Visibility = Visibility.Hidden;
            this.startTime = DateTime.Now.ToString("h:mm:ss tt");

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
                foreach (Point randomBomb in randomBombs)
                {
                    bool found = randomBomb.x == buttonPoint.x && randomBomb.y == buttonPoint.y;

                    if (found)
                    {
                        field.state = new State(0, 0, true);
                    }
                }

                if (field.state == null)
                {
                    field.state = new State(0, 0, false);
                }
            }

            foreach (Field field in fields)
            {
                Point buttonPoint = field.point;

                int bombsAround = getBombsAround(buttonPoint);

                if (bombsAround > 0) {
                    field.state.number = bombsAround;
                }
                else
                {
                    field.state.number = null;
                }

                field.button.Foreground = Brushes.Black;
            }


            this.bombFlags = this.randomBombs.Count();
            showCurrentFlags();
        }

        private void resetGame()
        {
            this.mainGrid.Children.Clear();

            this.mainGrid.Height = 25 * this.dimensions;
            this.mainGrid.Width = 25 * this.dimensions;

            this.fields = new List<Field> { };

            this.mainGrid.ColumnDefinitions.Clear();
            this.mainGrid.RowDefinitions.Clear();
             
            this.randomBombs = new List<Point> { };
        }

        private void StartGameButton(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            this.gameIsFinished = false;

            btn.Content = "Restart";

            this.resetGame();
            this.initGame();
        }

        private void onRadioCLick(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;

            this.dimensions = Convert.ToInt32(radio.Tag);
        }
    }

    public class State
    {
        private string[] colors = { "lightGray", "white", "red", "yellow" };
        public int id { get; set; }
        public string color { get; set; }
        public int? number { get; set; }
        public bool isBomb { get; set; }
        public bool clicked { get; set; }

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
