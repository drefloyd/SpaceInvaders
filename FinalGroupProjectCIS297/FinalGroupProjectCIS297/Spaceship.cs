using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalGroupProjectCIS297
{
    public class Spaceship : IDrawable, ICollidable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool TravelingLeftward { get; set; }
        public bool TravelingRightward { get; set; }

        //ship can shoot bullets
        public bool Shooting { get; set; }

        private CanvasBitmap image;

        public Spaceship(int x, int y, int width, int height, CanvasBitmap image)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            this.image = image;
            TravelingLeftward = false;
            TravelingRightward = false;
            Shooting = false;
        }
        public void Update()
        {
            if (TravelingRightward)
            {
                X += 1;

            }
            else if (TravelingLeftward)
            {
                X -= 1;

            }
        }

        public void Draw(CanvasDrawingSession canvas)
        {
            canvas.DrawImage(image, X, Y);
        }

        // Aliens will be able to collide with the ship from the top, left and right
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
    }
}
