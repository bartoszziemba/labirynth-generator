using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace labirynth
{
    class Program
    {
        uint frameLimit = 20;
        bool renderFpsText = false;
        RenderWindow gameWindow;
        Text limiterText;
        Text fpsText;
        Font mainFont;
        Maze maze;
        MazeRenderer mazeRenderer;

        Vector2i mazeSize;
        int cellSize;

        Sprite labirynthSprite;
        Texture labirynthTexture;

        Thread mazeGeneratorThread;

        static void Main(string[] args)
        {
            var game = new Program();
            game.Start(args);
        }

        public static Text CreateText(string initial_string, Font font, Color color)
        {
            Text result = new Text(initial_string, font);
            result.CharacterSize = 38;
            result.OutlineThickness = 2;
            result.OutlineColor = Color.Black;
            result.FillColor = color;

            return result;
        }

        public static void CheckArgs(string[] args)
        {
            if (!(args.Length == 6 &&
            int.TryParse(args[1], out _) &&
            int.TryParse(args[3], out _) &&
            int.TryParse(args[5], out _)
            ))
            {
                Console.WriteLine("Usage: labirynth.exe -x width -y heigth -c cellSize");
                Environment.Exit(1);
            }
        }

        void ParseArgs(string[] args)
        {
            int mazex = int.Parse(args[1]);
            int mazey = int.Parse(args[3]);
            cellSize = int.Parse(args[5]);
            mazeSize = new Vector2i(mazex, mazey);
        }

        void EndGame()
        {
            gameWindow.Close();
            Environment.Exit(0);
        }

        void KeyPressEventHandler(object sender, EventArgs e)
        {
            if ((e as KeyEventArgs).Code == Keyboard.Key.Add)
            {
                frameLimit += 1;
            }
            if ((e as KeyEventArgs).Code == Keyboard.Key.Subtract)
            {
                frameLimit = frameLimit > 1 ? frameLimit - 1 : frameLimit;
            }
            if ((e as KeyEventArgs).Code == Keyboard.Key.F)
            {
                renderFpsText = !renderFpsText;
            }

            if ((e as KeyEventArgs).Code == Keyboard.Key.Escape)
            {
                EndGame();
            }

            limiterText.DisplayedString = "Limit: " + frameLimit.ToString();
            //(sender as Window).SetFramerateLimit(frame_limit);
            gameWindow.SetFramerateLimit(frameLimit);

        }

        void GenerateNewMaze()
        {
            maze = new Maze(mazeSize.X, mazeSize.Y);
            mazeRenderer = new MazeRenderer(maze, cellSize);
        }

        void Start(string[] args)
        {
            CheckArgs(args);
            ParseArgs(args);

            GenerateNewMaze();

            InitializeWindow();
        }

        void InitializeWindow()
        {
            mainFont = new Font("pixel.ttf");

            limiterText = CreateText("Limit: " + frameLimit.ToString(), mainFont, Color.Cyan);
            limiterText.Position += new Vector2f(0, 30);

            fpsText = CreateText("x", mainFont, Color.Magenta);

            labirynthTexture = mazeRenderer.GetTextureFromMaze();

            labirynthSprite = new Sprite();
            labirynthSprite.Texture = labirynthTexture;

            gameWindow = new RenderWindow(new VideoMode(labirynthTexture.Size.X, labirynthTexture.Size.Y), "Labirynth Generator");
            gameWindow.Closed += (sender, e) => { EndGame(); };
            gameWindow.KeyPressed += KeyPressEventHandler;

            gameWindow.SetFramerateLimit(frameLimit);


            mazeGeneratorThread = maze.StartIterationFillThread(maze.GetRandomCell());

            while (gameWindow.IsOpen)
            {
                Update();
            }
        }
        void UpdateLabirynthTexture()
        {
            labirynthTexture = mazeRenderer.GetTextureFromMaze();
            labirynthSprite.Texture = labirynthTexture;
        }
        void Update()
        {
            maze.GetRandomCell().GetNeighbours();
            Stopwatch sw = Stopwatch.StartNew();

            gameWindow.DispatchEvents();
            gameWindow.Clear();

            UpdateLabirynthTexture();

            gameWindow.Draw(labirynthSprite);
            if (renderFpsText)
            {
                gameWindow.Draw(fpsText);
                gameWindow.Draw(limiterText);
            }
            gameWindow.Display();

            if (!mazeGeneratorThread.IsAlive)
            {
                Console.WriteLine("Detected thread dead");
                Thread.Sleep(300);
                GenerateNewMaze();
                mazeGeneratorThread = maze.StartIterationFillThread(maze.GetRandomCell());
            }

            maze.Tick();

            sw.Stop();
            fpsText.DisplayedString = "FPS:  " + (1000 / sw.Elapsed.TotalMilliseconds).ToString("0");
        }

    }
}
