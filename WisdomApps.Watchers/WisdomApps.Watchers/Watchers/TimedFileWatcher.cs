using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using WisdomApps.Watchers.Enums;
using WisdomApps.Watchers.Args;
using WisdomApps.Watchers.Extensions;
using WisdomApps.Watchers.Helpers;
using WisdomApps.Watchers.Options;
using WisdomApps.Watchers.IO;

namespace WisdomApps.Watchers
{
	public class TimedFileWatcher : Disposable
	{			
		private Thread _mainThread;
		private FolderMonitor _folderMonitor;
		private WatcherStatus _currentStatus;

		private string _name = "Timed File Watcher";
		private volatile bool _running;

		public ConcurrentQueue<QueuedFile> QueuedFiles { get; private set; }
		public TimedFileWatcherOptions Options { get; private set; }
		public WatcherStatus CurrentStatus {
			get {
				return _currentStatus;
			}
			private set {
				if(_currentStatus.Equals(value)) {
					var prevStatus = this.CurrentStatus;
					_currentStatus = value;

					WatcherStatusChanged.Raise(this, new StatusChangedEventArgs(prevStatus, this.CurrentStatus));
				}
			}
		}

		public event EventHandler<ErrorEventArgs> Error;
		public event EventHandler<PathAvailabilityEventArgs> DiectoryAvalibilityChanged;
		public event EventHandler<EventArgs> FileQueued;
		public event EventHandler<StatusChangedEventArgs> WatcherStatusChanged;

		public TimedFileWatcher(TimedFileWatcherOptions options) {
			if(options == null) { throw new ArgumentNullException("options", "Options are required to initialize a TimedFileWatcher"); }
			this.Options = options;
			Initialize();			
		}

		private void Initialize() {
			this.QueuedFiles = new ConcurrentQueue<QueuedFile>();
			this._currentStatus = WatcherStatus.NotStarted;

			_folderMonitor = new FolderMonitor(this.Options.WatchPath, 60);
			_folderMonitor.PathAvailabilityChanged += OnPathAvailabilityChanged;

			_mainThread = new Thread(PollDirectory) {
				IsBackground = true,
				Name = this._name
			};
		}

		private void OnPathAvailabilityChanged(object sender, PathAvailabilityEventArgs e) {
			if(!e.PathIsAvailable) {
				if(this.Options.ThrowIfWatchDirectoryNotAvailable) {
					throw new DirectoryNotFoundException("Watch Directory is No longer Available", new Exception(e.DirectoryPath));
				}
				if(_running) { Stop(); }
			} else {
				if(!_running) { Start(); }
			}

			this.DiectoryAvalibilityChanged.Raise(sender, e);
		}

		private void PollDirectory() {
			while(_running) {
				var files = Directory.EnumerateFiles(this.Options.WatchPath, this.Options.FileFilter, this.Options.IncludeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

				foreach(var file in files) {
					if(!this.QueuedFiles.All(x => x.FullPath == file && x.ChangeType == QueuedFileChangeType.Created)) {
						QueueFile(this, new QueuedFile(Path.GetFileName(file), this.Options.WatchPath, file, QueuedFileChangeType.Created));
					}
				}
			}
		}			
	
		private void OnError(object sender, Exception e){
			Error.Raise(sender, new ErrorEventArgs(e));
			//Log
		}

		private void QueueFile(object sender, QueuedFile file) {
			this.QueuedFiles.Enqueue(file);
			FileQueued.Raise(sender, new EventArgs());
		}

		public void Start() {
			_running = true;
			_mainThread.Start();
			_folderMonitor.EnableRaisingEvents = true;
			CurrentStatus = WatcherStatus.Started;
		}

		public void Stop() {
			_running = false;
			_mainThread.Join();
			_folderMonitor.EnableRaisingEvents = false;
			CurrentStatus = WatcherStatus.Stoped;
		}

		public void Restart() {
			Stop();
			CurrentStatus = WatcherStatus.Restarted;
			Initialize();
		}

		protected override void Dispose(bool disposing) {
			_running = false;
			if(_mainThread != null) {
				_mainThread.Join();				
			}

            if (_folderMonitor != null) {
                _folderMonitor.Dispose();
            }
			
			base.Dispose(disposing);
		}
	}
}
