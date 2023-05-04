using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FinalGroupProjectCIS297
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameOverPage : Page
    {
        public GameOverPage()
        {
            this.InitializeComponent();
        }

        private void creditsMenuButton_Click(object sender, RoutedEventArgs e)
        {//takes us from where we are (credits/gameover) to the menu
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MenuPage));
        }

        private void menuCreditsButton_Click(object sender, RoutedEventArgs e)
        {//takes us from where we are to credits
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(CreditsPage));
        }
        //eric's video: https://www.youtube.com/watch?v=peo08A2NKoo&t=6409s used
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
          ganeOverScore.Text = "Score: " + e.Parameter;
        }
    }
}
