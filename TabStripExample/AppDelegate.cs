using Foundation;
using UIKit;

namespace TabStripExample
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UINavigationController navigationController;

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            // If you have defined a root view controller, set it here:
            //ViewController controller = new ViewController();
            //Window.RootViewController = myViewController;

            navigationController = new UINavigationController();
            navigationController.NavigationBar.TitleTextAttributes =
                new UIStringAttributes() { ForegroundColor = UIColor.White };
            navigationController.NavigationBar.BarTintColor = UIColor.White;
            navigationController.NavigationBar.TintColor = UIColor.White;
            navigationController.NavigationBar.BarStyle = UIBarStyle.Black;
            navigationController.NavigationBar.Translucent = false;
            navigationController.NavigationBar.ShadowImage = new UIImage();
            navigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);

            Window.RootViewController = navigationController;

            ViewController tabController = new ViewController();
            tabController.Title = "Awesome app";
            //tabController.NavigationItem.Title = "Back";
            tabController.NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);

            navigationController.PushViewController(tabController, false);

            UIColor crimson = UIColor.FromRGB(165, 16, 129);
            UINavigationBar.Appearance.BarTintColor = crimson;
            //UIApplication.SharedApplication.KeyWindow.TintColor = crimson;

            // make the window visible
            Window.MakeKeyAndVisible();

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}


