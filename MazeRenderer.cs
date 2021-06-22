using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Threading;

namespace labirynth
{
    class MazeRenderer
    {
        int cellSize = 10;
        int linethicc = 1;

        public Color[,] pixels;
        Maze maze;

        static Color WallColor = Color.White;
        static Color CurrentColor = Color.Yellow;
        static Color VisitedColor = new Color(44,168,209);
        static Color EmptyColor = Color.Black;

        Stack<Cell> cellsToRender = new Stack<Cell>();
        public MazeRenderer(Maze m,int cellSize)
        {
            this.cellSize = cellSize;
            linethicc = cellSize/10 < 1?1:cellSize/10;
            maze = m;
            pixels = new Color[m.cells.Count * cellSize, m.cells[0].Count * cellSize];
            for (int i = 0; i < m.cells.Count; i++)
            {
                for (int j = 0; j < m.cells[i].Count; j++)
                {
                    m.cells[i][j].cellsToRender = cellsToRender;
                    m.cells[i][j].undraw();
                }
            }

        }

        public Texture GetTextureFromMaze()
        {

            //DrawerThread(pixels, 0, maze.cells.Count, maze);
            
            DrawCellStack(pixels,cellsToRender);
            Image image = new Image(pixels);

            Texture texture = new Texture(image);
            return texture;
        }

        void DrawCellStack(Color[,] img,Stack<Cell> cells)
        {
            int i=0;
            while(cells.Count > 0)
            {
                Cell cell;
                lock(cells)
                {
                cell = cells.Pop();
                }
                if(cell == null)
                    i+=1;
                else
                    DrawCellOnImage(cell,img,maze);
            }
            if(i>0)
                Console.WriteLine($"Null values detected: {i}");
        }
        void DrawerThread(Color[,] img, int startx, int stopx, Maze m)
        {
            for (int i = startx; i < stopx; i++)
            {
                for (int j = 0; j < m.cells[i].Count; j++)
                {
                    DrawCellOnImage(m.cells[i][j], img, m);
                }
            }
        }
        static void SetPixel(Color[,] img, int x_length, int x, int y, Color c)
        {
            img[x, y] = c;
        }
        void DrawCellOnImage(Cell c, Color[,] img, Maze m)
        {
            if (c.drawn)
            {
               return;
            }
            c.drawn = true;


            //top wall
            for (int i = c.x * cellSize + linethicc; i < (c.x + 1) * cellSize - linethicc; i++)
            {
                for (int j = c.y * cellSize ; j < c.y * cellSize + linethicc; j++)
                {
                    if (c.topWall)
                        SetPixel(img, m.cells[0].Count * cellSize, i, j, WallColor);
                    else
                        SetPixel(img, m.cells[0].Count * cellSize, i, j, VisitedColor);
                }
            }

            //bot wall

            for (int i = c.x * cellSize+ linethicc; i < (c.x + 1) * cellSize - linethicc; i++)
            {
                for (int j = (c.y + 1) * cellSize - linethicc; j < (c.y + 1) * cellSize; j++)
                {
                    if (c.botWall)
                        SetPixel(img, m.cells[0].Count * cellSize, i, j, WallColor);
                    else
                        SetPixel(img, m.cells[0].Count * cellSize, i, j, VisitedColor);
                }
            }


            //left wall

            for (int i = c.x * cellSize; i < c.x * cellSize + linethicc; i++)
            {
                for (int j = c.y * cellSize + linethicc; j < (c.y + 1) * cellSize - linethicc; j++)
                {
                    if (c.leftWall)
                        SetPixel(img, m.cells[0].Count * cellSize, i, j, WallColor);
                    else
                        SetPixel(img, m.cells[0].Count * cellSize, i, j, VisitedColor);
                }
            }

            

            //right wall

            for (int i = (c.x + 1) * cellSize - linethicc; i < (c.x + 1) * cellSize; i++)
            {
                for (int j = c.y * cellSize + linethicc; j < (c.y + 1) * cellSize - linethicc; j++)
                {
                    if (c.rightWall)
                        SetPixel(img, m.cells[0].Count * cellSize, i, j, WallColor);
                    else
                        SetPixel(img, m.cells[0].Count * cellSize, i, j, VisitedColor);
                }
            }

            
            //visited
            for (int i = c.x * cellSize + linethicc; i < (c.x + 1) * cellSize - linethicc; i++)
            {
                for (int j = c.y * cellSize + linethicc; j < (c.y + 1) * cellSize - linethicc; j++)
                {
                    if (c.visited && !c.current)
                        SetPixel(img, m.cells[0].Count * cellSize, i, j, VisitedColor);
                    else if(c.current)
                        SetPixel(img, m.cells[0].Count * cellSize, i, j, CurrentColor);
                    else
                        SetPixel(img, m.cells[0].Count * cellSize, i, j, EmptyColor);
                }
            }



            //top left corner
            for(int i = c.x * cellSize; i< c.x * cellSize + linethicc;i++)
            {
                for (int j = c.y * cellSize; j < c.y * cellSize + linethicc; j++)
                {
                    SetPixel(img, m.cells[0].Count * cellSize, i, j, WallColor);
                }
            }

            //top right corner
            for(int i = (c.x+1) * cellSize - linethicc; i< (c.x+1) * cellSize;i++)
            {
                for (int j = c.y * cellSize; j < c.y * cellSize + linethicc; j++)
                {
                    SetPixel(img, m.cells[0].Count * cellSize, i, j, WallColor);
                }
            }

            //bot left corner
            for(int i = c.x * cellSize; i< c.x * cellSize + linethicc;i++)
            {
                for (int j = (c.y+1) * cellSize - linethicc; j < (c.y+1) * cellSize; j++)
                {
                    SetPixel(img, m.cells[0].Count * cellSize, i, j, WallColor);
                }
            }

            //bot right corner
            for(int i = (c.x+1) * cellSize - linethicc; i< (c.x+1) * cellSize;i++)
            {
                for (int j = (c.y+1) * cellSize - linethicc; j < (c.y+1) * cellSize; j++)
                {
                    SetPixel(img, m.cells[0].Count * cellSize, i, j, WallColor);
                }
            }


        }
    }
}