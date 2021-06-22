  using System.Collections.Generic;
  

  namespace labirynth{
  
  class Cell
    {
        public static int created_cells = 0;
        public bool current = false;
        public bool drawn = false;
        public bool visited = false;
        public bool ifPath = false;
        public bool topWall = true;
        public bool botWall = true;
        public bool leftWall = true;
        public bool rightWall = true;

        public int x;
        public int y;

        public List<Cell> neighbours = new List<Cell>();

        public Stack<Cell> cellsToRender;
        Maze maze;
        public Cell(Maze m, int x, int y)
        {
            this.x = x;
            this.y = y;
            maze = m;
            created_cells += 1;
        }

        public void FillNeighbours()
        {
            if(x>0)
            {
                neighbours.Add(maze.cells[x-1][y]);
            }
            if(x < maze.cells.Count-1)
            {
                neighbours.Add(maze.cells[x+1][y]);
            }
            if(y>0)
            {
                neighbours.Add(maze.cells[x][y-1]);                
            }
            if(y < maze.cells[x].Count-1)
            {    
                neighbours.Add(maze.cells[x][y+1]);
            }
        }

        public List<Cell> GetNeighbours()
        {
            return neighbours;
        }

        public static void DestroyNeighbourWall(Maze maze,Cell cell,Cell neighbour)
        {
                if(neighbour.x < cell.x)
                {
                    neighbour.rightWall = false;
                    cell.leftWall = false;
                }

                if(neighbour.x >  cell.x)
                {
                    neighbour.leftWall = false;
                    cell.rightWall = false;
                }
                if(neighbour.y > cell.y)
                {
                    neighbour.topWall = false;
                    cell.botWall = false;
                }

                if(neighbour.y <  cell.y)
                {
                    neighbour.botWall = false;
                    cell.topWall = false;
                }
                cell.undraw();
                neighbour.undraw();
        }

        public void visit()
        {
            visited = true;
            undraw();
        }

        public void undraw()
        {
            drawn = false;
            if(cellsToRender != null)
                lock(cellsToRender)
                {
                    cellsToRender.Push(maze.cells[this.x][this.y]);
                }
        }

        public bool HasUnvisitedNeighbours(Maze m)
        {
            foreach(Cell neighbour in neighbours)
            {
                if(!neighbour.visited)
                    return true;
            }
            return false;
        }

        public Cell GetRandomUnvisitedNeighbour()
        {
            List<int> indexes = new List<int>();

            for(int i=0;i<neighbours.Count;i++)
            {
                indexes.Add(i);
            }

            int index = maze.random.Next(indexes.Count);
            index = indexes[index];
                
            while(neighbours[index].visited)
            {
                indexes.Remove(index);
                index = maze.random.Next(indexes.Count);
                index = indexes[index];
            }
            
            return neighbours[index];
        }

        

    }
  }