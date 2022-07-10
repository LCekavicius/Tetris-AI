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
using TetrisThing.AI;

namespace TetrisThing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TestAlgo testAlgo;

        public static void test()
        {

        }

        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] pieceImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/IPiece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/JPiece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/LPiece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/OPiece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/SPiece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TPiece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/ZPiece.png", UriKind.Relative))
        };

        private readonly Image[,] imageControls;

        private GameController gameController = new GameController();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameController.Board);
        }


        private Image[,] SetupGameCanvas(Board board)
        {
            Image[,] imageControls = new Image[board.Rows, board.Columns];
            int cellSize = 25;
            for (int r = 0; r < board.Rows; r++)
            {
                for (int c = 0; c < board.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 50);
                    Canvas.SetLeft(imageControl, c * cellSize);

                    GameCanvas.Children.Add(imageControl);

                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawGrid(Board board)
        {
            for (int r = 0; r < board.Rows; r++)
            {
                for (int c = 0; c < board.Columns; c++)
                {
                    int id = board[r, c];
                    imageControls[r,c].Source = tileImages[id];
                }
            }
        }

        private void DrawPiece(Piece piece)
        {
            foreach (var item in piece.CellPositions())
            {
                imageControls[item.X, item.Y].Source = tileImages[(int)piece.Shape];
            }
        }

        private async Task GameLoop()
        {
            //Draw(gameController);
            if(testAlgo != null)
                Draw(testAlgo.GetController());
            else
                Draw(gameController);
            while (!gameController.IsDead)
            {
                await Task.Delay(100);
                //gameController.MoveDown();
                //Draw(gameController);
                if (testAlgo != null)
                    Draw(testAlgo.GetController());
                else
                    Draw(gameController);
            }

            GameOverMenu.Visibility = Visibility.Visible;
        }

        private void DrawNextPiece(Bag bag)
        {
            Piece next = bag.CurrentBag.First();
            NextImage.Source = pieceImages[(int)next.Shape];
        }

        private void DrawHeldPiece(Piece heldPiece)
        {
            if(heldPiece == null)
                HoldImage.Source = pieceImages[0];
            else
                HoldImage.Source= pieceImages[(int)heldPiece.Shape];

        }

        public void Draw(GameController controller)
        {
            DrawGrid(controller.Board);
            DrawPiece(controller.CurrentPiece);
            DrawNextPiece(controller.Bag);
            if(testAlgo != null)
            {
                GenerationText.Text = $"Gen {testAlgo.Generation}";
                Weight_holesText.Text = $"Holes W: {testAlgo.BestHolesW()}";
                Weight_bumpinessText.Text = $"Bumpiness W: {testAlgo.BestBumpinessW()}";
                Weight_relativeHeightText.Text = $"Relative height W: {testAlgo.BestBumpinessW()}";
                Weight_heightSumText.Text = $"HeightSum W: {testAlgo.BestHeightSumW()}";
                Weight_scoreText.Text = $"Score W: {testAlgo.BestScoreW()}";
            }
            ScoreText.Text = $"Score: {controller.Score}";
            ScoreText.Text = $"Score: {controller.Score}";
            ScoreText.Text = $"Score: {controller.Score}";
            ScoreText.Text = $"Score: {controller.Score}";
            DrawHeldPiece(controller.HeldPiece);
        }

        private async Task StartAlgorithm()
        {
            testAlgo = new TestAlgo(this);

            await testAlgo.Run();

        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if (gameController.IsDead)
                //return;

            switch (e.Key)
            {
                case Key.Left:
                    gameController.MoveLeft();
                    break;
                case Key.Right:
                    gameController.MoveRight();
                    break;
                case Key.Down:
                    gameController.MoveDown();
                    break;
                case Key.Up:
                    gameController.RotateClockWise();
                    break;
                case Key.X:
                    gameController.RotateCounterClockWise();
                    break;
                case Key.Z:
                    gameController.RotateClockWise();
                    break;
                case Key.C:
                    gameController.HoldPiece();
                    break;
                case Key.Space:
                    gameController.HardDrop();
                    break;
                case Key.Q:
                    gameController.SaveGameState();
                    break;
                case Key.W:
                    gameController.LoadGameState();
                    break;
                case Key.O:
                    await StartAlgorithm();
                    break;


                default:
                    return;
            }

            Draw(gameController);
        }
        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            //Draw(gameController);
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameController = new GameController();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }
    }
}
