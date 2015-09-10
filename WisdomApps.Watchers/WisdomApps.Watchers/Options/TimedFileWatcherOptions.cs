namespace WisdomApps.Watchers.Options
{
	public class TimedFileWatcherOptions
	{
		public bool IncludeSubFolders { get; set; }
		public bool ThrowIfWatchDirectoryNotAvailable { get; set; }
		public string WatchPath { get; set; }
		public string FileFilter { get; set; }
		public uint PollingInterval { get; set; }

		public TimedFileWatcherOptions() {
			this.IncludeSubFolders = false;
			this.ThrowIfWatchDirectoryNotAvailable = true;
			this.WatchPath = string.Empty;
			this.FileFilter = string.Empty;
			this.PollingInterval = 30;
		}

	}
}
