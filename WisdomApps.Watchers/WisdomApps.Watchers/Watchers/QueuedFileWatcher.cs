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

namespace WisdomApps.Watchers {
	public class QueuedFileWatcher : Disposable {		
		private List<FileSystemWatcher> _watchers;
		private FolderMonitor _folderMonitor;
		private WatcherStatus _currentStatus;

		private string _name = "QueuedFileWatcher";		
		private bool _watchersWatching; 

		public ConcurrentQueue<QueuedFile> QueuedFiles { get; private set; }
		public QueuedFileWatcherOptions Options { get; private set; }
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

		public QueuedFileWatcher(QueuedFileWatcherOptions options) {
			if(options == null) { throw new ArgumentNullException("options", "options are required to initialize a QueuedFileWatcher"); }
			this.Options = options;
			Initialize();
		}

		private void Initialize() {
			this.QueuedFiles = new ConcurrentQueue<QueuedFile>();
			this._currentStatus = WatcherStatus.NotStarted;

			_folderMonitor = new FolderMonitor(this.Options.WatchPath, 60);
			_folderMonitor.PathAvailabilityChanged += OnPathAvailabilityChanged;			

			//Setup Watcher Events 
			_watchers.Clear();
			CreateWatcher(QueuedFileNotifyFilters.Attributes);
			CreateWatcher(QueuedFileNotifyFilters.Created);
			CreateWatcher(QueuedFileNotifyFilters.CreationTime);
			CreateWatcher(QueuedFileNotifyFilters.Deleted);
			CreateWatcher(QueuedFileNotifyFilters.DirectoryRenamed);			
			CreateWatcher(QueuedFileNotifyFilters.FileRenamed);
			CreateWatcher(QueuedFileNotifyFilters.LastAccess);
			CreateWatcher(QueuedFileNotifyFilters.LastWrite);
			CreateWatcher(QueuedFileNotifyFilters.Security);
			CreateWatcher(QueuedFileNotifyFilters.Size);

		}

		private void CreateWatcher(QueuedFileNotifyFilters filter) {

			if(this.Options.EventFilters.HasFlag(filter)) {
				int bufferSize = (int)this.Options.BufferBytes * 1024;

				if(!Directory.Exists(this.Options.WatchPath) && this.Options.ThrowIfWatchDirectoryNotAvailable) {
					throw new DirectoryNotFoundException("Watch directory was not found", new Exception(this.Options.WatchPath));
				}

				var watcher = new FileSystemWatcher();
				watcher.IncludeSubdirectories = this.Options.IncludeSubFolders;
				watcher.Path = this.Options.WatchPath;
				watcher.Filter = this.Options.FileFilter;
				watcher.NotifyFilter = this.Options.NotifyFilter;
				watcher.InternalBufferSize = bufferSize;

				if(this.Options.EventFilters.HasFlag(QueuedFileNotifyFilters.Created) ||
					this.Options.EventFilters.HasFlag(QueuedFileNotifyFilters.Deleted) ||
					this.Options.EventFilters.HasFlag(QueuedFileNotifyFilters.FileRenamed)) {

					if(this.Options.EventFilters.HasFlag(QueuedFileNotifyFilters.Created)) {
						watcher.Created += new FileSystemEventHandler(OnFileCreated);
					}

					if(this.Options.EventFilters.HasFlag(QueuedFileNotifyFilters.Deleted)) {
						watcher.Deleted += new FileSystemEventHandler(OnFileDeleted);
					}

					if(this.Options.EventFilters.HasFlag(QueuedFileNotifyFilters.FileRenamed)) {
						watcher.Renamed += new RenamedEventHandler(OnFileRenamed);
					}

				} else {
					switch(filter) {
						case QueuedFileNotifyFilters.Attributes:
							watcher.Changed += new FileSystemEventHandler(OnAttributeChanged);
							break;
						case QueuedFileNotifyFilters.CreationTime:
							watcher.Changed += new FileSystemEventHandler(OnCreationTimeChanged);
							break;
						case QueuedFileNotifyFilters.DirectoryRenamed:
							watcher.Changed += new FileSystemEventHandler(OnDirectoryNameChanged);
							break;
						case QueuedFileNotifyFilters.FileName:
							watcher.Changed += new FileSystemEventHandler(OnFileNameChanged);
							break;
						case QueuedFileNotifyFilters.LastAccess:
							watcher.Changed += new FileSystemEventHandler(OnLastAccessChagned);
							break;
						case QueuedFileNotifyFilters.LastWrite:
							watcher.Changed += new FileSystemEventHandler(OnLastWriteChanged);
							break;
						case QueuedFileNotifyFilters.Security:
							watcher.Changed += new FileSystemEventHandler(OnSecurityChanged);
							break;
						case QueuedFileNotifyFilters.Size:
							watcher.Changed += new FileSystemEventHandler(OnSizeChanged);
							break;
					}
				}

				watcher.Error += new ErrorEventHandler(OnError);

				_watchers.Add(watcher);
			}
		}

		private void OnPathAvailabilityChanged(object sender, PathAvailabilityEventArgs e) {
			if (!e.PathIsAvailable){
				if(this.Options.ThrowIfWatchDirectoryNotAvailable) {
					throw new DirectoryNotFoundException("Watch Directory is No longer Available", new Exception(e.DirectoryPath));
				}
				if(_watchersWatching) { Stop();}
			} else {
				if(!_watchersWatching) { Start(); }
			}

			this.DiectoryAvalibilityChanged.Raise(sender, e);
		}

		private void QueueFile(object sender, QueuedFile file){
			this.QueuedFiles.Enqueue(file);
			FileQueued.Raise(sender, new EventArgs());
		}

		private void OnAttributeChanged(object sender, FileSystemEventArgs e) {
			QueueFile(sender, new QueuedFile(e.Name, Options.WatchPath, e.FullPath, QueuedFileChangeType.Attributes));			
		}

		private void OnCreationTimeChanged(object sender, FileSystemEventArgs e) {
			QueueFile(sender, new QueuedFile(e.Name, Options.WatchPath, e.FullPath, QueuedFileChangeType.CreationTime));
		}

		private void OnDirectoryNameChanged(object sender, FileSystemEventArgs e) {
			QueueFile(sender, new QueuedFile(e.Name, Options.WatchPath, e.FullPath, QueuedFileChangeType.Created));
		}

		private void OnFileNameChanged(object sender, FileSystemEventArgs e) {
			QueueFile(sender, new QueuedFile(e.Name, Options.WatchPath, e.FullPath, QueuedFileChangeType.FileName));
		}

		private void OnLastAccessChagned(object sender, FileSystemEventArgs e) {
			QueueFile(sender, new QueuedFile(e.Name, Options.WatchPath, e.FullPath, QueuedFileChangeType.LastAccess));
		}

		private void OnLastWriteChanged(object sender, FileSystemEventArgs e) {
			QueueFile(sender, new QueuedFile(e.Name, Options.WatchPath, e.FullPath, QueuedFileChangeType.LastWrite));
		}

		private void OnSecurityChanged(object sender, FileSystemEventArgs e) {
			QueueFile(sender, new QueuedFile(e.Name, Options.WatchPath, e.FullPath, QueuedFileChangeType.Security));
		}

		private void OnSizeChanged(object sender, FileSystemEventArgs e) {
			QueueFile(sender, new QueuedFile(e.Name, Options.WatchPath, e.FullPath, QueuedFileChangeType.Size));
		}

		private void OnFileCreated(object sender, FileSystemEventArgs e) {
			QueueFile(sender, new QueuedFile(e.Name, Options.WatchPath, e.FullPath, QueuedFileChangeType.Created));
		}

		private void OnFileDeleted(object sender, FileSystemEventArgs e) {
			QueueFile(sender, new QueuedFile(e.Name, Options.WatchPath, e.FullPath, QueuedFileChangeType.Deleted));
		}

		private void OnFileRenamed(object sender, RenamedEventArgs e) {
			QueueFile(sender, new QueuedFile(e.Name, Options.WatchPath, e.FullPath, QueuedFileChangeType.FileRenamed));
		}

		private void OnError(object sender, ErrorEventArgs e) {			
			if(this.Options.EventFilters.HasFlag(QueuedFileNotifyFilters.Error)) {
				Error.Raise(sender, e);
				//Note(Nathan): do we stop the watcher and set its status to errored and force the consumer to restart the watcher or just handle things internaly.
			}

			//TODO: Get the exception for buffer overflow
			var bufferOverflow = true;

			if(bufferOverflow && this.Options.EnableDirectoryCleanupOnError) {
				CleanDirectory();
			}
		}

		private void CleanDirectory() {
			var files = System.IO.Directory.EnumerateFiles(this.Options.WatchPath, this.Options.FileFilter, this.Options.IncludeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

			foreach(var file in files) {
				if(!this.QueuedFiles.All(x => x.FullPath == file && x.ChangeType == QueuedFileChangeType.Created)) {
					OnFileCreated(this, new FileSystemEventArgs(WatcherChangeTypes.Created, this.Options.WatchPath, Path.GetFileName(file)));
				}
			}
		}

		public void Start() {
			_watchers.ForEach(x => x.EnableRaisingEvents = true);
			_watchersWatching = true;
			_folderMonitor.EnableRaisingEvents = true;
			CurrentStatus = WatcherStatus.Started;
		}

		public void Stop() {
			_watchers.ForEach(x => x.EnableRaisingEvents = false);
			_watchersWatching = false;
			_folderMonitor.EnableRaisingEvents = false;
			CurrentStatus = WatcherStatus.Stoped;				
		}

		/// <summary>
		/// reloads the watcher using the Options.
		/// </summary>
		public void Reset() {
			Stop();
			CurrentStatus = WatcherStatus.Restarted;
			Initialize();
		}

		protected override void Dispose(bool disposing) {

			if(_watchers != null) {
				_watchers.ForEach(x => x.Dispose());
				_watchers.Clear();
			}

			if(_folderMonitor != null) {
				_folderMonitor.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}
