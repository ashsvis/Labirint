using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Labirint
{
    public partial class MainForm : Form
    {
        readonly int side = 20;
        readonly State[,] Map;
        readonly int width = 61;
        readonly int height = 31;
        Point currentCell;
        readonly Stack<Point> stack;
        readonly Random rnd;
        bool builded = false;

        public MainForm()
        {
            InitializeComponent();
            panel1.Size = new Size(width * side, height * side);
            stack = new Stack<Point>();
            rnd = new Random(); // new Random(DateTime.Now.Millisecond);
            Map = new State[width, height];
            InitMap();
            // выбираем начальную точку стартовой
            currentCell = new Point(1, 1);

            
        }

        /// <summary>
        /// Создание начальной сетки ячеек
        /// </summary>
        private void InitMap()
        {
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    if (i % 2 != 0 && j % 2 != 0 &&         // если ячейка нечетная по x и y, 
                        i < width - 1 && j < height - 1)    // и при этом находится в пределах стен лабиринта
                        Map[i, j] = State.Cell;             // то это КЛЕТКА
                    else Map[i, j] = State.Wall;            // в остальных случаях это СТЕНА.
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

        /// <summary>
        /// Рисуем ячейку лабиринта на поверхности рисования
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="br"></param>
        /// <param name="gr"></param>
        void DrawBox(int x, int y, Brush br, Graphics gr)
        {
            gr.FillRectangle(br, x * side, y * side, side, side);
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            if (builded)
            {
                InitMap();
                // выбираем начальную точку стартовой
                currentCell = new Point(1, 1);
            }
            BuildMap();
            PrepareAfterBuildMap();
            builded = true;
            panel1.Invalidate();
        }

        /// <summary>
        /// Очистка посещённых ячеек после построения лабиринта
        /// </summary>
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

        /// <summary>
        /// Построение лабиринта
        /// </summary>
        private void BuildMap()
        {
            while (true)
            {
                // перемещаемся к случайному не посещённому соседу, пока таковые есть.
                var neighbours = GetNeighbours(currentCell);
                if (neighbours.Length != 0)
                {
                    var randNum = rnd.Next(neighbours.Length);
                    // выбираем случайного соседа
                    var neighbourCell = neighbours[randNum];
                    // если соседей больше чем один
                    if (neighbours.Length > 1)
                        stack.Push(currentCell); // то запоминаем ячейку для возврата
                                                 // убираем стену между текущей и соседней точками
                    RemoveWall(currentCell, neighbourCell);
                    // помечаем текущую ячейку как посещённую
                    Map[currentCell.X, currentCell.Y] = State.Visited;
                    // делаем соседнюю ячейку текущей и отмечаем ее посещённой
                    currentCell = neighbourCell;
                }
                else if (stack.Count > 0) // если нет соседей, возвращаемся на предыдущую запомненную ячейку
                {
                    Map[currentCell.X, currentCell.Y] = State.Visited;
                    currentCell = stack.Pop();
                }
                else
                    break;
            }
        }
        /// <summary>
        /// Функция RemoveWall убирает стенку между двумя клетками
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        private void RemoveWall(Point first, Point second)
        {
            var xDiff = second.X - first.X;
            var yDiff = second.Y - first.Y;
            int addX, addY;
            var target = new Point();

            addX = (xDiff != 0) ? (xDiff / Math.Abs(xDiff)) : 0;
            addY = (yDiff != 0) ? (yDiff / Math.Abs(yDiff)) : 0;

            target.X = first.X + addX; // координаты стенки
            target.Y = first.Y + addY;

            Map[target.X, target.Y] = State.Visited;
        }

        /// <summary>
        /// Функция GetNeighbours возвращает массив не посещённых соседей клетки
        /// </summary> 
        /// <param name="c"></param>
        /// <returns></returns>
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
                // если не выходит за границы лабиринта
                if (p.X > 0 && p.X < width && p.Y > 0 && p.Y < height)
                {
                    // и не посещена\является стеной
                    if (Map[p.X, p.Y] != State.Wall && Map[p.X, p.Y] != State.Visited)
                        points.Add(p); // записать в массив
                }
            }
            return points.ToArray();
        }

        /// <summary>
        /// Первоначальная загрузка главной формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // генерируем лабиринт
            panel1_Click(panel1, new EventArgs());
            // подстраиваем размер формы под размер панели с лабиринтом
            ClientSize = panel1.Bounds.Size;
            // показываем форму по центру экрана
            CenterToScreen();
        }
    }

    public enum State
    {
        Wall,
        Cell,
        Visited
    }
}
