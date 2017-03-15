using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VASSALChatter
{
	public class ModuleItem : INotifyPropertyChanged
	{
		public string Name { get; set; }
		public int RoomCount => Rooms.Count;
		public int UserCount => Users.Count;
		public List<string> Rooms { get; set; } = new List<string>();
		public List<string> Users { get; set; } = new List<string>();
		public string ListSubLabel => $"Rooms: {RoomCount}, Users: {UserCount}";

		public ModuleItem()
		{
		}

		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			var changed = PropertyChanged;
			changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
