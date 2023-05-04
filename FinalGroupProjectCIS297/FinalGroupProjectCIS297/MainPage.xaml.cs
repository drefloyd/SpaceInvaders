using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Threading;
using Windows.System;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
//we used royalty free pictures and song
namespace FinalGroupProjectCIS297
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SpaceInvaders SpaceInvaders;
        private int ticksForBullet;
        private bool canShoot;
        private CanvasBitmap shipImage;
        private CanvasBitmap alienImage;
        private CanvasBitmap bulletImage;
        private CanvasBitmap bulletImageFlipped;
        private CanvasBitmap powerUpImage;
        private MediaPlayer backgroundMusic;

        public MainPage()
        {
            this.InitializeComponent();

            Window.Current.CoreWindow.KeyDown += Canvas_KeyDown;
            Window.Current.CoreWindow.KeyUp += Canvas_KeyUp;
            ticksForBullet= 0;
            canShoot = true;
        }

        private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            SpaceInvaders.DrawGame(args.DrawingSession);
        }

        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            SpaceInvaders.Update();
            ticksForBullet++;
            if(ticksForBullet % (SpaceInvaders.shootDelayInSeconds * 60) == 0)//set to the shootdelay sec 
            {
                canShoot= true;
            }
            //used same chatgpt answer under this to display text
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ScoreText.Text = $"SCORE: {SpaceInvaders.Score}";

            });

            if (SpaceInvaders.gameOver)
            {
                backgroundMusic.Pause(); //restarts music once game is played again
                //asked chatgpt: is there a way to change pages in the update event that is not async in UWP xaml, using the dispatcher var
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Frame rootFrame = Window.Current.Content as Frame;
                    
                        rootFrame.Navigate(typeof(GameOverPage),SpaceInvaders.Score);
                        
                    
                });

               
            }
        }


        // when the button is pressed
        private void Canvas_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            if (e.VirtualKey == Windows.System.VirtualKey.Up)
            {if (canShoot)
                {
                    SpaceInvaders.shootABullet(bulletImage);
                    ticksForBullet = 0;//reset so we can start counting 0.5 sec from now
                    canShoot = false;
                }
            }

            if (e.VirtualKey == Windows.System.VirtualKey.Left)
            {
                SpaceInvaders.SetShipTravelingLeftward(true);
            }
            else if (e.VirtualKey == Windows.System.VirtualKey.Right)
            {
                SpaceInvaders.SetShipTravelingRightward(true);
            }

            
        }

        // when the button is released
        private void Canvas_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            if (e.VirtualKey == Windows.System.VirtualKey.Left)
            {
                SpaceInvaders.SetShipTravelingLeftward(false);
            }
            else if (e.VirtualKey == Windows.System.VirtualKey.Right)
            {
                SpaceInvaders.SetShipTravelingRightward(false);
            }
        }

        private void Canvas_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResources(sender).AsAsyncAction());
        }

        async Task CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender)
        {
            shipImage = await CanvasBitmap.LoadAsync(sender, "Assets/spaceshipImage.jpg");
            alienImage = await CanvasBitmap.LoadAsync(sender, "Assets/alien.jpg");
            bulletImage = await CanvasBitmap.LoadAsync(sender, "Assets/BulletButSmaller.jpg");
            powerUpImage = await CanvasBitmap.LoadAsync(sender, "Assets/powerUp.jpg");
            bulletImageFlipped = await CanvasBitmap.LoadAsync(sender, "Assets/bulletFlipped.jpg");
            backgroundMusic = new MediaPlayer();
            Windows.Storage.StorageFolder assets = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets");//retrieve the assets folder

            //Source Video on how to add music file in UWP: https://www.youtube.com/watch?v=hPxExtLCMK0&t=230s 
            Windows.Storage.StorageFile sound = await assets.GetFileAsync("SkyFire (Title Screen).ogg");//retrieve the sound from the folder
            backgroundMusic.AutoPlay = true;//start playing instantly
            backgroundMusic.IsLoopingEnabled = true;//loop
            backgroundMusic.Source = MediaSource.CreateFromStorageFile(sound);//added sound to the player
            backgroundMusic.Volume = 0.09; //adjusted volume
            SpaceInvaders = new SpaceInvaders(shipImage, alienImage, bulletImage, powerUpImage, bulletImageFlipped);
        }
    }
}
