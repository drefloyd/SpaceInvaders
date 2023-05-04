using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Xaml;

namespace FinalGroupProjectCIS297
{
    public class Bullet : IDrawable
    {
        public bool Fired { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Speed { get; set; }
        public bool inUse { get; set; }
        public int Width { get; set; } 
        public int Radius { get; set; }
        public bool TravelingUpward { get; set; }
        public bool destroyed { get; set; }

        private CanvasBitmap bulletImage;

        private bool alienBullet;
        public Bullet(int x, int y, int speed, CanvasBitmap bulletImage, bool alienBullet = false,bool isFired = false)
        {
            Fired = isFired;
            X = x;
            Y = y;
            Speed = speed;
            Width = (int)bulletImage.Size.Width;
            Radius = (int)bulletImage.Size.Height;
            this.bulletImage = bulletImage;
            destroyed = false;
            this.alienBullet = alienBullet;
        }

        // bullets will only be shot from ship so they will only travel upward 
        public void Update(int x)
        {
            if (Fired)
            {
                TravelingUpward = true;
                if (alienBullet)
                {
                    Y += Speed;
                }
                else
                {
                    Y -= Speed;
                }
            }
        }
        public bool CollidesLeftEdge(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Radius;
        }

        public bool ColllidesRightEdge(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Radius;
        }

        public bool CollidesTopEdge(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Radius;
        }
        public bool CoolidesBottomEdge(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Radius;
        }

        public void Clear(CanvasDrawingSession canvas)
        {
            canvas.FillRectangle(new Rect(X, Y, bulletImage.Size.Width, bulletImage.Size.Height), Colors.Transparent);
        }

        public void Draw(CanvasDrawingSession canvas)
        {
            if (!destroyed)
            {
                canvas.DrawImage(bulletImage, X, Y);
            }
        }
    }
}
