using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalGroupProjectCIS297
{
    internal class PowerUp : IDrawable, ICollidable
    {//used professor github (credit inside mainpage xaml)
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }

        public int Height { get; set; }

        private CanvasBitmap powerUpImage;

        public PowerUp(int x, int y, int width, int height, CanvasBitmap powerUpImage)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            this.powerUpImage = powerUpImage;
        }

        public bool CollidesLeftEdge(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }

        public bool ColllidesRightEdge(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }

        public bool CollidesTopEdge(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y;
        }
        public bool CoolidesBottomEdge(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }
        public void Update()
        {//move downward when spawned
            Y++;
        }

        public void Draw(CanvasDrawingSession canvas)
        {
            canvas.DrawImage(powerUpImage, X, Y);
        }
    }
}
