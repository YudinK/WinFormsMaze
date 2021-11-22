using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp5
{
    public partial class Form1 : Form
    {
        int side = 20;
        State[,] Map;
        int width = 61;
        int height = 31;
        Point currentCell;
        Stack<Point> stack;
        Random rnd;
        bool builded = false;
        public enum State
        {
            Wall,
            Cell,
            Visited
        }
        public Form1()
        {
            InitializeComponent();
            panel1.Size = new Size(width * side, height * side);
            stack = new Stack<Point>();
            rnd = new Random(); 
            Map = new State[width, height];
            InitMap();            
            currentCell = new Point(1, 1);
        }
        private void InitMap()
        {
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    if (i % 2 != 0 && j % 2 != 0 &&         
                        i < width - 1 && j < height - 1)    
                        Map[i, j] = State.Cell;             
                    else Map[i, j] = State.Wall;            
                }
            }
        }       
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    switch (Map[x, y])
                    {
                        case State.Cell:
                            DrawBox(x, y, Brushes.White, e.Graphics);
                            break;
                        case State.Wall:
                            DrawBox(x, y, Brushes.Black, e.Graphics);
                            break;
                        case State.Visited:
                            DrawBox(x, y, Brushes.Red, e.Graphics);
                            break;
                    }
                }
        }
        void DrawBox(int x, int y, Brush br, Graphics gr)
        {
            gr.FillRectangle(br, x * side, y * side, side, side);
        }
        private void BuildMap()
        {
            while (true)
            {
                var neighbours = GetNeighbours(currentCell);
                if (neighbours.Length != 0)
                {
                    var randNum = rnd.Next(neighbours.Length);
                    var neighbourCell = neighbours[randNum];
                    if (neighbours.Length > 1)
                        stack.Push(currentCell); 
                    RemoveWall(currentCell, neighbourCell);
                    Map[currentCell.X, currentCell.Y] = State.Visited;
                    currentCell = neighbourCell;
                }
                else if (stack.Count > 0) 
                {
                    Map[currentCell.X, currentCell.Y] = State.Visited;
                    currentCell = stack.Pop();
                }
                else
                    break;
            }
        }
        private void PrepareAfterBuildMap()
        {
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    if (Map[i, j] == State.Visited)
                        Map[i, j] = State.Cell;
                }
            }
        }
        private void panel1_Click(object sender, EventArgs e)
        {
            if (builded)
            {
                InitMap();
                currentCell = new Point(1, 1);
            }
            BuildMap();
            PrepareAfterBuildMap();
            builded = true;
            panel1.Invalidate();
        }
        private void RemoveWall(Point first, Point second)
        {
            var xDiff = second.X - first.X;
            var yDiff = second.Y - first.Y;
            int addX, addY;
            var target = new Point();

            addX = (xDiff != 0) ? (xDiff / Math.Abs(xDiff)) : 0;
            addY = (yDiff != 0) ? (yDiff / Math.Abs(yDiff)) : 0;

            target.X = first.X + addX; 
            target.Y = first.Y + addY;

            Map[target.X, target.Y] = State.Visited;
        }
        private Point[] GetNeighbours(Point c)
        {
            const int distance = 2;
            var points = new List<Point>();
            var x = c.X;
            var y = c.Y;
            var up = new Point(x, y - distance);
            var rt = new Point(x + distance, y);
            var dw = new Point(x, y + distance);
            var lt = new Point(x - distance, y);
            var d = new Point[] { dw, rt, up, lt };
            foreach (var p in d)
            {
                if (p.X > 0 && p.X < width && p.Y > 0 && p.Y < height)
                {
                    if (Map[p.X, p.Y] != State.Wall && Map[p.X, p.Y] != State.Visited)
                        points.Add(p);
                }
            }
            return points.ToArray();
        }
    }
}
