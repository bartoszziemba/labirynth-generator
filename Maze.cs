using System;
using System.Collections.Generic;
using System.Threading;

namespace labirynth
{
    class Maze
    {

        public Random random = new Random();
        public List<List<Cell>> cells = new List<List<Cell>>();
        public Maze(int sizex, int sizey)
        {
            Cell.created_cells = 0;
            for (int i = 0; i < sizex; i += 1)
            {
                cells.Add(new List<Cell>());
                for (int j = 0; j < sizey; j += 1)
                {
                    cells[i].Add(new Cell(this,i,j));
                }
            }
            cells.ForEach( row => {row.ForEach( c => {c.FillNeighbours();});});
            
            
            
        }

        public Cell GetRandomCell()
        {
            int x = random.Next(0,cells.Count);
            int y = random.Next(0,cells[0].Count);

            return cells[x][y];
        }

        bool HasUnvisitedCells(out Cell c)
        {
            foreach(var row in cells)
            {
                foreach(var cell in row)
                {
                    if (!cell.visited)
                    {
                        c = cell;
                        return true;
                    }
                }
            }
            c = null;
            return false;
        }

        public void RecursiveFill(Cell c)
        {
            c.visit();
            while(c.HasUnvisitedNeighbours(this))
            {
                Cell cell = c.GetRandomUnvisitedNeighbour();
                Cell.DestroyNeighbourWall(this,c,cell);
                RecursiveFill(cell);
            }

        }
        public AutoResetEvent tickEvent = new AutoResetEvent(false);
        public Thread StartIterationFillThread(Cell c)
        {
            Thread t = new Thread(() => IterationFill(c));
            t.Start();
            return t;
        }

        public void Tick()
        {
            tickEvent.Set();
        }
        public void IterationFillNoWait(Cell c)
        {
            Stack<Cell> stack = new Stack<Cell>();

            c.visit();
            stack.Push(c);

            while(stack.Count != 0)
            {
                Cell cell = stack.Pop();
                if(cell.HasUnvisitedNeighbours(this))
                {
                    stack.Push(cell);
                    var cell2 = cell.GetRandomUnvisitedNeighbour();
                    Cell.DestroyNeighbourWall(this,cell,cell2);
                    cell2.visit();
                    stack.Push(cell2);
                }
            }
        }
        public void IterationFill(Cell c)
        {
            Stack<Cell> stack = new Stack<Cell>();

            c.visit();
            stack.Push(c);

            while(stack.Count != 0)
            {
                Cell cell = stack.Pop();
                cell.current = true;
                cell.undraw();
                if(cell.HasUnvisitedNeighbours(this))
                {
                    stack.Push(cell);
                    var cell2 = cell.GetRandomUnvisitedNeighbour();
                    Cell.DestroyNeighbourWall(this,cell,cell2);
                    cell2.visit();
                    cell2.current = true;
                    cell.current = false;
                    cell.undraw();
                    cell2.undraw();
                    stack.Push(cell2);     
                }
                tickEvent.WaitOne();
                cell.current = false;
                cell.undraw();

            }
        }
    }

  
}