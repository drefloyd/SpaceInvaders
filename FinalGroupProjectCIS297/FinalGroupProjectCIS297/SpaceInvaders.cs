using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.Gaming.Input;
using Windows.UI;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;
//used https://github.com/EricCharnesky/CIS297-Winter2023/tree/main/XAMLAnimatedCanvasPong as reference for multiple things in the proj
namespace FinalGroupProjectCIS297
{
    public interface IDrawable
    {
        void Draw(CanvasDrawingSession canvas);
    }

    // spaceship will only collide with the right and left edges
    // bullets shot that do not hit an alien will eventually collide with the top wall
    // bottom wall needed bc technically an alien can make it all the way to the bottom
    public interface ICollidable
    {
        bool CollidesLeftEdge(int x, int y);
        bool ColllidesRightEdge(int x, int y);
        bool CollidesTopEdge(int x, int y);
        bool CoolidesBottomEdge(int x, int y);
    }

    public class SpaceInvaders
    {
        public int Score;
        public double shootDelayInSeconds;

        public static int LEFT_EDGE = 10;
        public static int TOP_EDGE = 10;
        public static int RIGHT_EDGE = 790;
        public static int BOTTOM_EDGE = 450;
        public static int MIDDLE_EDGE = 400;    // added just to make the positioning easier

        public int aliensLeft = 11; // keeps track of how many aliens are still alive

        //this is a used to hold the bullet image to use to create a bullet with a controller
        private CanvasBitmap theBulletImage;

        private CanvasBitmap theBulletImageFlipped;

        //this is a used to hold the powerUp image to use to create a powerUp
        private CanvasBitmap thePowerUpImage;


        private Random random;

        private Spaceship spaceship;
        private Wall leftWall;//made them instance so it makes collisions easier
        private Wall rightWall;
        private Wall bottomWall;
        CanvasDrawingSession canvasRef;
        private const int ALIENNUM = 9;
        private List<Bullet> bulletList = new List<Bullet>();

        private Alien[] aliens = new Alien[ALIENNUM];  // chose to do 11 to start just so we can have 1 row
        private int[] aliensX = new int[ALIENNUM];
        private int[] aliensY = new int[ALIENNUM];

        bool shotHasBeenFired = false;  // tells when the first shot has been taken
        int numberOfBullets = 0;    // counts bullets for the array

        private List<IDrawable> drawables;
        private List<IDrawable> bulletsShot;    // lsit strictly for all the bullets that are shot
        private List<PowerUp> powerUps;//list to store powerUps
        private List<Bullet> alienBullets;

        public bool gameOver = false;

        private Gamepad controller;
        int ticksForBullet;//make it so we have the same wait before shooting a bullet when using a controller
        bool canShoot;//used for bullet restrictions
        int ticks;  // will count our time for the alien movement intervals
        int ticksForPowerUp;//make it so powerUp is not forever
        bool hasPowerUp;//used to make sure they pick only 1 not all
        int flip = 0;  // flips to change aliens direction

        public SpaceInvaders(CanvasBitmap spaceshipImage, CanvasBitmap alienImage, CanvasBitmap bulletImage, CanvasBitmap powerUPImage, CanvasBitmap bulletImageFlipped)
        {
            //setting ticks to 0 to start counting:
            ticks = 0;
            ticksForBullet = 0;
            ticksForPowerUp = 0;
            //make shootdelay the default .5 sec
            shootDelayInSeconds = 0.5;
            //make it so they can shoot instantly when they load
            canShoot = true;
            //setup powerUpstatus
            hasPowerUp = false;
            //setting the bullet image and powerUp image for use later
            theBulletImage = bulletImage;
            theBulletImageFlipped = bulletImageFlipped;
            thePowerUpImage = powerUPImage;
            Score = 0;

            drawables = new List<IDrawable>();
            bulletsShot = new List<IDrawable>();
            powerUps = new List<PowerUp>();
            alienBullets = new List<Bullet>();

            leftWall = new Wall(LEFT_EDGE, TOP_EDGE, LEFT_EDGE, BOTTOM_EDGE + 49, Colors.White);
            drawables.Add(leftWall);

            rightWall = new Wall(RIGHT_EDGE, TOP_EDGE, RIGHT_EDGE, BOTTOM_EDGE + 49, Colors.White);
            drawables.Add(rightWall);


            bottomWall = new Wall(LEFT_EDGE, BOTTOM_EDGE + 49, RIGHT_EDGE, BOTTOM_EDGE + 49, Colors.Red);
            drawables.Add(bottomWall);


            spaceship = new Spaceship(MIDDLE_EDGE, BOTTOM_EDGE, 66, 20, spaceshipImage);

            // alien 1 starts in the middle-top of the screen and the rest will be either to the 
            // right or left of it
            int offset = 0;
            bool hit = false;
            for (int i = 0; i < aliens.Length; i++)
            {
                if (offset > 200)
                {
                    offset = 50;
                    hit = true;
                }
                if (hit)
                {
                    aliens[i] = new Alien(MIDDLE_EDGE + offset, TOP_EDGE, 30, 26, alienImage);
                }
                else
                {
                    aliens[i] = new Alien(MIDDLE_EDGE - offset, TOP_EDGE, 30, 26, alienImage);
                }
                offset += 50;
            }
            offset = 0;
            hit = false;

            drawables.Add(spaceship);

            for (int i = 0; i < aliens.Length; i++)
            {
                aliensX[i] = aliens[i].X;
                aliensY[i] = aliens[i].Y; 
            }

            foreach (Alien a in aliens)
            {
                drawables.Add(a);
            }

            gameOver = false;

            random = new Random();
        }

        public void SetShipTravelingLeftward(bool travelingLeftward)
        {
            spaceship.TravelingLeftward = travelingLeftward;
        }

        public void SetShipTravelingRightward(bool travelingRightward)
        {
            spaceship.TravelingRightward = travelingRightward;
        }
        public void shootABullet(CanvasBitmap bulletImage)
        {
            Bullet bullet = new Bullet(spaceship.X + 25, spaceship.Y + 25, 10, bulletImage); //place holder
            bulletList.Add(bullet);

            drawables.Add(bullet);
            bulletsShot.Add(bullet);
            bullet.Fired = true;
            shotHasBeenFired = true;
            numberOfBullets++;

        }


        public void moveAliensDown(Alien[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i].TravelingDownward = true;
                a[i].Update();
                a[i].TravelingDownward = false;
            }
        }

        public void moveAliensRight(Alien[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i].TravelingRightward = true;
                a[i].Update();
                a[i].TravelingRightward = false;
            }
        }

        public void moveAliensLeft(Alien[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i].TravelingLeftward = true;
                a[i].Update();
                a[i].TravelingLeftward = false;
            }
        }

        public void alienShoot(CanvasBitmap bulletImageFlipped)
        {
            foreach(Alien a in aliens)
            {
                if (random.Next(1, 1001) == 1)
                {// shoot rate (1 in 1000)
                    alienBullets.Add(new Bullet(a.X + 25, a.Y + 25, 2, bulletImageFlipped, true,true));//add alien bullet
                }
            }
            
        }
        public void checkBulletStatus()
        {
            for(int j = 0; j < bulletList.Count; j++) 
            { 

                Bullet curBullet = bulletList[j];   

                if (curBullet == null)
                {
                    return;
                }


                //Check if bullet hit alien
                bool bulletHitAlien = false;
                for (int i = 0; i < aliens.Length; i++)
                {
                    if (aliens[i].CollidesTopEdge(curBullet.X, curBullet.Y))
                    {
                        bulletHitAlien = true;
                        aliens[i].X = aliensX[i];
                        aliens[i].Y = aliensY[i];
                        Score += 10;
                        //here we add a power up from where the alien is killed
                        if (random.Next(1, 8) == 1)
                        {//14.3% spawn rate (1 in 7)
                            powerUps.Add(new PowerUp(aliens[i].X, aliens[i].Y, 27, 35, thePowerUpImage));
                        }
                        break;
                    }
                }

            // Dequeue the bullet if it hit an alien or the top edge
                if (bulletHitAlien || curBullet.Y <= 0)
                {

                    curBullet.X = -15;
                    bulletHitAlien = true;
                    curBullet.Y = -15;
                    curBullet.Speed = 0;
                    bulletList.Remove(curBullet);
                }

            }
        }

        public void Update()    // what is called to refresh the game every x seconds
        {
            ticks++;    // increase with the time, 60x per sec
            ticksForBullet++;
            ticksForPowerUp++;
            if (ticksForBullet % (shootDelayInSeconds * 60) == 0)//set to shoot delay in seconds 
            {
                canShoot = true;
            }
            if (ticksForPowerUp % (3 * 60) == 0)//set the powerUp time limit, 3 sec
            {
                hasPowerUp = false;
            }
            //bool bounced = false;

            // Game logic needs to be added/fixed here 

            if (!gameOver)
            {
                List<IDrawable> aliensToKill = new List<IDrawable>();

                //get controller readings
                if (Gamepad.Gamepads.Count > 0)//moved this in here 
                {
                    controller = Gamepad.Gamepads.First();
                    var reading = controller.GetCurrentReading();

                    //see if they pressed the button to shoot a bullet
                    if (reading.Buttons == GamepadButtons.A)//or use right trigger: reading.RightTrigger > 0
                    {//shot a bullet 
                        if (canShoot)
                        {
                            shootABullet(theBulletImage);//call this function to create the bullet
                            canShoot = false;
                            ticksForBullet = 0;
                        }
                    }
                    if (reading.LeftThumbstickX > 0.20)
                    {//moving right
                        spaceship.TravelingRightward = true;
                        spaceship.TravelingLeftward = false;
                    }
                    else if (reading.LeftThumbstickX < -0.20)
                    {//moving left
                        spaceship.TravelingRightward = false;
                        spaceship.TravelingLeftward = true;
                    }
                    else
                    {
                        //not moving
                        spaceship.TravelingRightward = false;
                        spaceship.TravelingLeftward = false;
                    }
                }

                //ship-wall collision logic
                if (leftWall.ColllidesRightEdge(spaceship.X, spaceship.Y)) //y isnt needed 
                {//checking if the ship has collided with the wall
                    //checking right edge because its the left wall
                    spaceship.TravelingLeftward = false;//cannot travel anymore to the left
                }
                else if (rightWall.CollidesLeftEdge(spaceship.X + spaceship.Width, spaceship.Y)) //y not really needed
                {//checking if the ship has collided with the wall
                    //checking left edge because its the right wall
                    spaceship.TravelingRightward = false; //cannot travel anymore to the right 
                }
                spaceship.Update();

                if (shotHasBeenFired == true) // will not enter until the first bullet has been shot
                {
                    for (int i = 0; i < numberOfBullets; i++)
                    {
                        Bullet bulletJustShot = (Bullet)bulletsShot[i];

                        bulletJustShot.Update(spaceship.X);

                    }
                }

                if (ticks % 3 * 60 == 0)  // every 3 seconds move down (*60 is because each tick is 1/60 of a sec)
                {
                    moveAliensDown(aliens);
                }

                if (flip == 0)  // so right and left can take turns going
                {
                    if (ticks % 7 * 60 == 0)  // every 7 seconds move right
                    {
                        moveAliensRight(aliens);
                    }
                    flip = 1;
                }
                else
                {
                    flip = 0;
                    if (ticks % 7 * 60 == 0)  // every 7 seconds move left
                    {
                        moveAliensLeft(aliens);
                    }
                }

                //Check bullet queue to see if any bullets in it have collided with an alien. HERE
            
                for (int i = 0; i < aliens.Length; i++)
                {
                    if (bottomWall.CollidesTopEdge(aliens[i].X, aliens[i].Y + aliens[i].Height))
                    {
                        gameOver = true;
                    }
                    else if (spaceship.CollidesTopEdge(aliens[i].X, aliens[i].Y + aliens[i].Height))
                    {
                        gameOver = true;
                    }
                }

                checkBulletStatus();
                //after checking bullet status update powerUps
                List<PowerUp> powerUpsToDestroy = new List<PowerUp>();//powerUps we want to get rid of (collected or hit ground)
                foreach (PowerUp powerUp in powerUps)
                {
                    powerUp.Update();
                    //check if collision with ship (collect)
                    if (spaceship.CollidesTopEdge(powerUp.X, powerUp.Y+powerUp.Height) ||
                        spaceship.CollidesLeftEdge(powerUp.X+powerUp.Width, powerUp.Y) ||
                        spaceship.ColllidesRightEdge(powerUp.X, powerUp.Y) 
                        )//we can collect it any way (side or direct)
                    { 
                        powerUpsToDestroy.Add(powerUp);
                        //power up functionality, make shoot delay atleast .1 (less is too fast)
                        if (!hasPowerUp)//if he doesnt have a powerUp then we can pick this up
                        {
                            shootDelayInSeconds = 0.15;
                            hasPowerUp = true;
                            ticksForPowerUp = 0;
                        }
                       
                    }

                    //check if power up is passed the redline (delete it)
                    else if(bottomWall.CollidesTopEdge(powerUp.X, powerUp.Y + powerUp.Height))
                    {
                        powerUpsToDestroy.Add(powerUp);
                    }
                    
                }
                foreach (PowerUp powerUpToDestroy in powerUpsToDestroy)
                {
                    powerUps.Remove(powerUpToDestroy);
                }

                if (!hasPowerUp)//after powerup is out, turn off boost
                {
                    shootDelayInSeconds = 0.5;
                }

                //make it so every 1 sec theres a possibility to sshoot an alien bullet
                if (ticks % 1 * 60 == 0)  // every 1 second (*60 is because each tick is 1/60 of a sec)
                {
                    alienShoot(theBulletImageFlipped);
                }
                
                //check alien bullet collisions
                List<Bullet> aBulletsToDestroy = new List<Bullet>();//bullets we want to get rid of (hit ground or ship)
                foreach (Bullet aBullet in alienBullets)
                {
                    aBullet.Update(0);//int not used here
                    //check if collision with ship 
                    if (spaceship.CollidesTopEdge(aBullet.X, aBullet.Y + aBullet.Radius))//we can get hit in any way (side(logic inside topedge already) or direct)
                    {
                        aBulletsToDestroy.Add(aBullet);
                        gameOver = true;//end game

                    }

                    //check if it is passed the redline (delete it)
                    else if (bottomWall.CollidesTopEdge(aBullet.X, aBullet.Y + 18))
                    {
                        aBulletsToDestroy.Add(aBullet);
                    }
                }
                foreach (Bullet abullet in aBulletsToDestroy)
                {
                    alienBullets.Remove(abullet);
                }
            }
        }

        public void DrawGame(CanvasDrawingSession canvas)
        {
            canvasRef = canvas;
            for (int index = 0; index < drawables.Count; index++)
            {
                drawables[index].Draw(canvas);
            }
            for (int index = 0; index < powerUps.Count; index++)
            {
                powerUps[index].Draw(canvas);
            }
            for (int index = 0; index < alienBullets.Count; index++)
            {
                alienBullets[index].Draw(canvas);
            }
        }
    }
}


  
