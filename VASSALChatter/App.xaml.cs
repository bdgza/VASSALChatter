using VASSALChatter.ViewModel;
using Xamarin.Forms;

namespace VASSALChatter
{
	// icons:
	// https://www.iconfinder.com/icons/134216/hamburger_lines_menu_icon#size=128

	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new RootPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
