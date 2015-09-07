using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WisdomApps.Watchers.Enums;
namespace WisdomApps.Watchers.Options
{
	public class QueuedFileWatcherOptions {
		public bool includeSubFolders { get; set; }		
		public bool ThrowIfWatchDirectoryNotAvaliable { get; set; }		
		public bool EnableDirectoryCleanupOnError { get; set; }
		public string WatchPath { get; set; }
		public string FileFilter { get; set; }
		public uint BufferBytes { get; set; }
		public QueuedFileNotifyFilters EventFilters { get; set; }
		public NotifyFilters NotifyFilter { get; set; }

		public QueuedFileWatcherOptions() {
			this.includeSubFolders = false;			
			this.ThrowIfWatchDirectoryNotAvaliable = true;
			this.WatchPath = String.Empty;
			this.FileFilter = String.Empty;			
			this.BufferBytes = 4;
			this.EventFilters = QueuedFileNotifyFilters.Created;
			this.NotifyFilter = NotifyFilters.FileName;		
		}
	}
}
