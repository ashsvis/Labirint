using System.Drawing;

namespace Labirint
{
    internal static class DrawHelper
    {
        /// <summary>
        /// Рисуем ячейку лабиринта на поверхности рисования
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="br"></param>
        /// <param name="gr"></param>
        internal static void DrawBox(this Graphics gr, Rectangle rect, Brush br)
        {
            gr.FillRectangle(br, rect);
        }
    }
}
