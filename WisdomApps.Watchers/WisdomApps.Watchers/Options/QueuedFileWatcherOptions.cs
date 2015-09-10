using System;
using System.IO;
using WisdomApps.Watchers.Enums;
namespace WisdomApps.Watchers.Options
{
	public class QueuedFileWatcherOptions {
		public bool IncludeSubFolders { get; set; }
		public bool ThrowIfWatchDirectoryNotAvailable { get; set; }		
		public bool EnableDirectoryCleanupOnError { get; set; }
		public string WatchPath { get; set; }
		public string FileFilter { get; set; }
		public uint BufferBytes { get; set; }
		public QueuedFileNotifyFilters EventFilters { get; set; }
		public NotifyFilters NotifyFilter { get; set; }

		public QueuedFileWatcherOptions() {
			this.IncludeSubFolders = false;			
			this.ThrowIfWatchDirectoryNotAvailable = true;
			this.WatchPath = String.Empty;
			this.FileFilter = String.Empty;			
			this.BufferBytes = 4;
			this.EventFilters = QueuedFileNotifyFilters.Created;
			this.NotifyFilter = NotifyFilters.FileName;		
		}
	}
}
