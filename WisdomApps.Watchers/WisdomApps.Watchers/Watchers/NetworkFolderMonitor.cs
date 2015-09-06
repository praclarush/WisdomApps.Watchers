using System.IO;
using System.Threading;
using WisdomApps.Watchers.Helpers;

namespace WisdomApps.Watchers {
	public class NetworkFolderMonitor : Disposable	{
		private readonly string _Name;
		private readonly int _interval;
		private readonly Thread _thread;
		private bool _running;

		public string DirectoryPath {get; private set;}
		public bool IsAvailable {get; private set;}

		public event Delegates.PathAvailabilityChanged PathAvailabilityChanged = delegate { };

		public NetworkFolderMonitor(string directoryPath, uint Interval) {
			this._Name = "Network Folder Monitor";
			this._interval = ((int)Interval * 1000);
			this._thread = new Thread(PathMonitor) {
				 IsBackground = true,
				 Name = this._Name,				 
			};			
		}

		private void PathMonitor() {
			while(this._running) {
				var isPathAvailable = Directory.Exists(this.DirectoryPath);

				if(this.IsAvailable != isPathAvailable) {
					this.IsAvailable = isPathAvailable;
					OnPathAvailabilityChange();
				}

			}
		}

		private void OnPathAvailabilityChange() {
			PathAvailabilityChanged(this, new Args.PathAvailabilityEventArgs(this.DirectoryPath, this.IsAvailable));
		}

		private void Start() {
			this._running = true;
			this._thread.Start();
		}

		private void Stop() {
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
