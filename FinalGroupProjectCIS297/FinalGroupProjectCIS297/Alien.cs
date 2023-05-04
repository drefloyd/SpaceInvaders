using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalGroupProjectCIS297
{
    public class Alien : IDrawable, ICollidable
    {
        public int X { get; set; }
        public int Y { get; set; }

        private int origX;
        private int origY;
        public int Width { get; set; }
        public int Height { get; set; }

        public bool TravelingDownward { get; set; }
        public bool TravelingUpward { get; set; }
        public bool TravelingLeftward { get; set; }
        public bool TravelingRightward { get; set; }


        private CanvasBitmap alienImage;

        public Alien(int x, int y, int width, int height, CanvasBitmap alienImage)
        {
            X = x;
            Y = y;
            origX = x;
            origY = y;
            Width = width;
            Height = height;
            this.alienImage = alienImage;
            TravelingDownward = false;
            TravelingLeftward = false;
            TravelingRightward = false;
        }

        public void setBack()
        {
            X = origX;
            Y = origY;
        }

        public void Update()
        {
            if (TravelingDownward)
            {
                Y += 1;
            }
            else if (TravelingUpward)
            {
                Y -= 1;
            }
            else if (TravelingRightward)
            {
                X += 3;
            }
            else if (TravelingLeftward)
            {
                X -= 3;
            }
        }

        public void Draw(CanvasDrawingSession canvas)
        {
            canvas.DrawImage(alienImage, X, Y);
        }

        // Aliens will be able to collide from the top, left, and right.
        // They'll also be damaged from the bottom (since the ship shoots from under it)
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
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }
        public bool CoolidesBottomEdge(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }
    }
}
