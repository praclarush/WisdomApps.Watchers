using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using WisdomApps.Watchers.Helpers;
using WisdomApps.Watchers.Extensions;

namespace WisdomApps.Watchers {
	public class FolderMonitor : Disposable	{
		private readonly string _Name;
		private readonly int _interval;
		private readonly Thread _thread;
		private bool _running;
		
		public string DirectoryPath {get; private set;}
		public bool IsAvailable {get; private set;}
		public bool EnableRaisingEvents { get; set; }

		public event EventHandler<Args.PathAvailabilityEventArgs> PathAvailabilityChanged;

		public FolderMonitor(string directoryPath, uint interval) {
			this._Name = "Network Folder Monitor";
            this.DirectoryPath = directoryPath;
			this._interval = ((int)interval * 1000);
			this._thread = new Thread(PathMonitor) {
				 IsBackground = true,
				 Name = this._Name,				 
			};			

			//Note(Nathan): decide if Events should be enabled by default.
			//EnableRaisingEvents = true;
		}

		private void PathMonitor() {
			while(this._running) {
				var isPathAvailable = Directory.Exists(this.DirectoryPath);

				if(this.IsAvailable != isPathAvailable) {
					this.IsAvailable = isPathAvailable;
					OnPathAvailabilityChange();
				}

                Thread.Sleep(this._interval);
			}
		}

		private void OnPathAvailabilityChange() {
			if(EnableRaisingEvents) {
				PathAvailabilityChanged.Raise(this, new Args.PathAvailabilityEventArgs(this.DirectoryPath, this.IsAvailable));		
			}			
		}

		public void Start() {
			this._running = true;
			this._thread.Start();
		}

		public void Stop() {
			this._running = false;
			this._thread.Join();
		}

		//TODO(Nathan): verify that this is all that needs to be done in Dispose.
		protected override void Dispose(bool disposing) {
			this._running = false;
			this._thread.Join();
			base.Dispose(disposing);
		}
	}
}
