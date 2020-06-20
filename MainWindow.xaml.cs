using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MineSweeperGame.src;

using Point = MineSweeperGame.src.Point;

namespace MineSweeperGame
{

    //System.Diagnostics.Debug.WriteLine("");

    public partial class MainWindow : Window
    {
        private List<Field> fields = new List<Field> { };
        private List<Point> randomBombs = new List<Point> { };
        private readonly List<Result> results = new List<Result> { };
        private int dimensions = 5;
        private bool gameIsFinished = false;
        /// <summary>
        /// Count of clicked fields
        /// </summary>
        public int clickedFields;
        private string startTime;
        private int bombFlags = 0;
        private float mode = 1;


        /// <summary>
        /// Main window initialization
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initialize Grid with fields and start game
        /// </summary>
        public void InitGame()
        {
            Random randNum = new Random();
            int Min = 0;
            int Max = this.dimensions;

            this.clickedFields = 0;
            this.info.Visibility = Visibility.Hidden;
            this.time.Visibility = Visibility.Hidden;
            this.startTime = DateTime.Now.ToString("h:mm:ss tt");

            randomBombs = Point.GenerateRandomPoints(Min, Max, Convert.ToInt32(this.dimensions * this.mode));


            for (int x = 0; x < this.dimensions; x++)
            {

                this.mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                this.mainGrid.RowDefinitions.Add(new RowDefinition());


                for (int y = 0; y < this.dimensions; y++)
                {
                    Field field = Field.AddButton(x, y, this.mainGrid, fields.Count());

                    field.Button.Click += OnFieldClick;
                    field.Button.MouseDown += OnFieldRightClick;


                    fields.Add(field);
                }
            }

            foreach (Field field in fields)
            {
                Point buttonPoint = field.Point;
                Button button = field.Button;

                //@todo optimize
                foreach (Point randomBomb in randomBombs)
                {
                    bool found = randomBomb.X == buttonPoint.X && randomBomb.Y == buttonPoint.Y;

                    if (found)
                    {
                        field.State = new State(0, 0, true);
                    }
                }

                if (field.State == null)
                {
                    field.State = new State(0, 0, false);
                }
            }

            foreach (Field field in fields)
            {
                Point buttonPoint = field.Point;

                int bombsAround = Point.GetBombsAround(buttonPoint, randomBombs);

                if (bombsAround > 0)
                {
                    field.State.Number = bombsAround;
                }
                else
                {
                    field.State.Number = null;
                }

                field.Button.Foreground = Brushes.Black;
            }


            this.bombFlags = this.randomBombs.Count();
            ShowCurrentFlags();
        }

        /// <summary>
        /// Clears grid with fields and resets main game settings
        /// </summary>
        private void ResetGame()
        {
            this.mainGrid.Children.Clear();

            this.mainGrid.Height = 25 * this.dimensions;
            this.mainGrid.Width = 25 * this.dimensions;

            this.fields = new List<Field> { };

            this.mainGrid.ColumnDefinitions.Clear();
            this.mainGrid.RowDefinitions.Clear();

            this.randomBombs = new List<Point> { };
        }

        /// <summary>
        /// Clear fields with empty number and show closest fields with number around provided point
        /// </summary>
        /// <param name="buttonPoint">Point with x and y coordinate</param>
        public void ClearAllFieldsAround(Point buttonPoint)
        {
            
            List<Field> foundFields = new List<Field>
            {
                fields.Find(field => field.Point.X + 1 == buttonPoint.X && field.Point.Y == buttonPoint.Y),
                fields.Find(field => field.Point.X - 1 == buttonPoint.X && field.Point.Y == buttonPoint.Y),
                fields.Find(field => field.Point.X == buttonPoint.X && field.Point.Y + 1 == buttonPoint.Y),
                fields.Find(field => field.Point.X == buttonPoint.X && field.Point.Y - 1 == buttonPoint.Y),
                fields.Find(field => field.Point.X + 1 == buttonPoint.X && field.Point.Y + 1 == buttonPoint.Y),
                fields.Find(field => field.Point.X + 1 == buttonPoint.X && field.Point.Y - 1 == buttonPoint.Y),
                fields.Find(field => field.Point.X - 1 == buttonPoint.X && field.Point.Y + 1 == buttonPoint.Y),
                fields.Find(field => field.Point.X - 1 == buttonPoint.X && field.Point.Y - 1 == buttonPoint.Y)
            };

            List<Field> filtered = foundFields.FindAll(e => e != null);

            foreach (Field filteredField in filtered) {

                if (!filteredField.State.Clicked && filteredField.State.Id != 3) {
                    filteredField.Button.Background = Brushes.White;
                    filteredField.State.Clicked = true;

                    CountClick();

                    if (filteredField.State.Number == null)
                    {
                        ClearAllFieldsAround(filteredField.Point);
                    }
                    else 
                    {
                        filteredField.Button.Content = filteredField.State.Number;
                    }
                }
            }
        }

        /// <summary>
        /// Count field click
        /// </summary>
        public void CountClick()
        {
            clickedFields++;

            if (clickedFields == fields.Count() - randomBombs.Count())
            {
                ResetGame();
                ShowWinInfo();
            }
        }

        /// <summary>
        /// Show info message after winning
        /// </summary>
        public void ShowWinInfo() {
            this.info.Visibility = Visibility.Visible;
            this.time.Visibility = Visibility.Visible;

            this.info.Content = "You won with time: ";
            this.time.Content = (DateTime.Now - Convert.ToDateTime(this.startTime)).ToString("c");

            results.Add(new Result(results.Count(), this.time.Content.ToString()));

            AddResults(results.Count(), this.time.Content.ToString());
        }

        
        /// <summary>
        /// Add game result to list view
        /// </summary>
        /// <param name="id">Index of the result</param>
        /// <param name="time">Game time</param>
        public void AddResults(int id, string time) {
            if (!resultsList.IsVisible) {
                resultsList.Visibility = Visibility.Visible;
                resultsList.Items.Add("Results:");
            }

            Label label = new Label
            {
                Content = "" + id + " --- " + time
            };

            resultsList.Items.Add(label);
        }

        /// <summary>
        /// Show all bombs after game over
        /// </summary>
        public void ShowAllBombs() {
            this.gameIsFinished = true;

            foreach (Field field in fields) {
                if (field.State != null && field.State.IsBomb) {
                    field.Button.Content = "B";
                    field.Button.Background = Brushes.Red;
                }
            }
        }

        /// <summary>
        /// Show current flag number
        /// </summary>
        /// <param name="number">Flag number</param>
        private void ShowCurrentFlags(int number = 0)
        {
            this.bombFlags += number;
            this.flags.Content = "Flags: " + this.bombFlags;
        }

        /// <summary>
        /// Handle field click
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">event</param>
        private void OnFieldClick(object sender, RoutedEventArgs e)
        {
            if (!this.gameIsFinished)
            {
                Button btn = sender as Button;
                State state = fields[(Int32)btn.Tag].State;
                Point point = fields[(Int32)btn.Tag].Point;

                if (state.Id != 3 && !state.Clicked)
                {
                    if (state != null && state.IsBomb)
                    {
                        this.ShowAllBombs();
                    }
                    else
                    {
                        btn.Background = Brushes.White;
                        state.Clicked = true;

                        CountClick();

                        if (state.Number != null)
                        {
                            btn.Content = state.Number;
                        }
                        else
                        {
                            this.ClearAllFieldsAround(point);

                            btn.Content = "";
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Handle field right click
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">event</param>
        private void OnFieldRightClick(object sender, MouseButtonEventArgs e)
        {
            if (!this.gameIsFinished)
            {
                Button btn = sender as Button;
                State state = fields[(Int32)btn.Tag].State;

                if (e.ChangedButton == MouseButton.Right && !state.Clicked && state.Id != 3 && this.bombFlags > 0)
                {
                    state.Id = 3;
                    btn.Background = Brushes.Yellow;

                    btn.Content = "?";

                    this.ShowCurrentFlags(-1);
                }
                else if (state.Id == 3)
                {
                    state.Id = 0;
                    btn.Content = "";
                    btn.Background = Brushes.LightGray;
                    this.ShowCurrentFlags(1);
                }
            }
        }

        /// <summary>
        /// Handle dimensions change
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">event</param>
        private void OnDimensionsCLick(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;

            this.dimensions = Convert.ToInt32(radio.Tag);
        }

        /// <summary>
        /// Handle mode change - difficulty of the game
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">event</param>
        private void OnModeClick(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;

            this.mode = float.Parse(radio.Tag.ToString(), CultureInfo.InvariantCulture.NumberFormat);

        }
        
        /// <summary>
        /// Handle game start button click
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">event</param>
        private void OnStartGameButtonClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            this.gameIsFinished = false;

            btn.Content = "Restart";

            this.ResetGame();
            this.InitGame();
        }
    }
}
