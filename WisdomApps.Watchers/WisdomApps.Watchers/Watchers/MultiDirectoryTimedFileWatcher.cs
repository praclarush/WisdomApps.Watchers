using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using WisdomApps.Watchers.Enums;
using WisdomApps.Watchers.Args;
using WisdomApps.Watchers.Extensions;
using WisdomApps.Watchers.Helpers;
using WisdomApps.Watchers.Lists;
using WisdomApps.Watchers.Options;
using WisdomApps.Watchers.IO;

namespace WisdomApps.Watchers.Watchers
{
	public class MultiDirectoryTimedFileWatcher
	{
		private readonly string _name;
		private Dictionary<string, TimedFileWatcher> _watchers;
		
		public event EventHandler<ErrorEventArgs> Error;
		public event EventHandler<PathAvailabilityEventArgs> DiectoryAvalibilityChanged;
		public event EventHandler<EventArgs> FileQueued;
		public event EventHandler<StatusChangedEventArgs> WatcherStatusChanged;

		public MultiDirectoryTimedFileWatcher() {
			this._name = "Multi Directory Timed File Watcher";
			_watchers = new Dictionary<string, TimedFileWatcher>();
		}

		public MultiDirectoryTimedFileWatcher(TimedFileWatcherOptions options, params string[] paths) : this() {
			foreach(var path in paths) {
				options.WatchPath = path;
				this.AddNewWatcher(options);
			}
		}

		public void AddNewWatcher(TimedFileWatcherOptions options) {
			var watcher = new TimedFileWatcher(options);
			this.AddNewWatcher(watcher);
		}

		public void AddNewWatcher(TimedFileWatcher watcher) {
			if(!this._watchers.ContainsKey(watcher.Options.WatchPath)) {
				this._watchers.Add(watcher.Options.WatchPath, watcher);
				watcher.Error += OnError;
				watcher.DiectoryAvalibilityChanged += OnDiectoryAvalibilityChanged;
				watcher.FileQueued += OnFileQueued;
				watcher.WatcherStatusChanged += OnWatcherStatusChanged;
			}
		}

		public void RemoveWatcher(string DirectoryPath) {
			if(this._watchers.ContainsKey(DirectoryPath)) {
				var watcher = this._watchers[DirectoryPath];
				watcher.Stop();
				watcher.Error -= OnError;
				watcher.DiectoryAvalibilityChanged -= OnDiectoryAvalibilityChanged;
				watcher.FileQueued -= OnFileQueued;
				watcher.WatcherStatusChanged -= OnWatcherStatusChanged;
				this._watchers.Remove(DirectoryPath);
				watcher.Dispose();
			}
		}

		public void Start() {
			foreach(var watcher in this._watchers) {
				watcher.Value.Start();
			}
		}

		public void Stop() {
			foreach(var watcher in this._watchers) {
				watcher.Value.Stop();
			}
		}

		private void OnError(object sender, ErrorEventArgs e) {
			this.Error.Raise(sender, e);
		}

		private void OnDiectoryAvalibilityChanged(object sender, PathAvailabilityEventArgs e) {
			this.DiectoryAvalibilityChanged.Raise(sender, e);
		}

		//Note(Nathan): Only way at this point to get file from the correct watcher is to typecast sender to a TimedFileWatcher
		private void OnFileQueued(object sender, EventArgs e) {
			this.FileQueued.Raise(sender, e);
		}

		private void OnWatcherStatusChanged(object sender, StatusChangedEventArgs e) {
			this.WatcherStatusChanged.Raise(sender, e);
		}
	}

}
