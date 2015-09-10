using System;
namespace WisdomApps.Watchers.Args
{
	public class PathAvailabilityEventArgs : EventArgs
	{
		public bool PathIsAvailable { get; private set; }
		public string DirectoryPath { get; private set; }

		public PathAvailabilityEventArgs(string directoryPath, bool isAvailable) {
			this.PathIsAvailable = isAvailable;
			this.DirectoryPath = directoryPath;
		}
	}
}
