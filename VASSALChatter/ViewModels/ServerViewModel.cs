using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VASSALChatter
{
	public class ServerViewModel : BaseViewModel
	{
		public ServerViewModel(RootPage root)
			: base(root)
		{
			Title = "VASSAL Server";
			Icon = "slideout.png";
		}

		private ObservableCollection<ModuleItem> moduleItems = new ObservableCollection<ModuleItem>();

		/// <summary>
		/// gets or sets the feed items
		/// </summary>
		public ObservableCollection<ModuleItem> ModuleItems
		{
			get { return moduleItems; }
			set { moduleItems = value; OnPropertyChanged(); }
		}

		private Command loadItemsCommand;
		/// <summary>
		/// Command to load/refresh items
		/// </summary>
		public Command LoadItemsCommand
		{
			get { return loadItemsCommand ?? (loadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand())); }
		}

		private async Task ExecuteLoadItemsCommand()
		{
			if (IsBusy) return;

			//var lines = text.Split('\n');
			//foreach (var line in lines)
			//{
			//	var fields = line.Split('\t');
			//	foreach (var field in fields)
			//	{
			//		Debug.WriteLine(field);
			//	}
			//}

			IsBusy = true;

			var error = false;
			try
			{
				var responseString = string.Empty;

				var assembly = typeof(ServerViewModel).GetTypeInfo().Assembly;
				Stream stream = assembly.GetManifestResourceStream("VASSALChatter.getCurrentConnections.txt");
				using (var reader = new System.IO.StreamReader(stream))
				{
					responseString = reader.ReadToEnd();
				}

				//using (var httpClient = new HttpClient())
				//{
				//	var feed = "http://www.vassalengine.org/util/getCurrentConnections";
				//	responseString = await httpClient.GetStringAsync(feed);
				//}

				ModuleItems.Clear();
				List<ModuleItem> items = await ParseData(responseString);
				//foreach (var item in items)
				//{
				//	ModuleItems.Add(item);
				//}
				ModuleItems = new ObservableCollection<ModuleItem>(items);
			}
			catch
			{
				error = true;
			}

			if (error)
			{
				var page = new ContentPage();
				await page.DisplayAlert("Error", "Unable to load server list.", "OK");
			}

			IsBusy = false;
		}

		private async Task<List<ModuleItem>> ParseData(string responseString)
		{
			return await Task.Run(() =>
			{
				List<ModuleItem> modules = new List<ModuleItem>();

				var lines = responseString.Split('\n');
				ModuleItem module = null;
				foreach (var line in lines)
				{
					var fields = line.Split('\t');
					if (fields.Length != 3) continue;

					var moduleName = fields[0];
					var roomName = fields[1];
					var userName = fields[2];

					// find module
					if (module == null || module.Name != moduleName)
						module = modules.Find((obj) => { return obj.Name == moduleName; });
					if (module == null)
					{
						module = new ModuleItem()
						{
							Name = moduleName
						};
						modules.Add(module);
					}

					// check room
					if (!module.Rooms.Contains(roomName)) module.Rooms.Add(roomName);

					// check user
					if (!module.Users.Contains(userName)) module.Users.Add(userName);
				}

				Debug.WriteLine($"SORTING {modules.Count}");

				modules.Sort((ModuleItem x, ModuleItem y) =>
				{
					return (x.Name.CompareTo(y.Name));
				});

				Debug.WriteLine($"Done Loading {modules.Count}");

				return modules;
			});
		}
}
}
