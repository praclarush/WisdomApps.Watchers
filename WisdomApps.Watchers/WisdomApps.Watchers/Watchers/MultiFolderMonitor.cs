using System;
using System.Collections.Generic;
using WisdomApps.Watchers.Extensions;

namespace WisdomApps.Watchers.Watchers
{
	public class MultiFolderMonitor
	{
		private readonly string _name;
		private Dictionary<string, FolderMonitor> _monitors;

		public event EventHandler<Args.PathAvailabilityEventArgs> PathAvailabilityChanged;

		public MultiFolderMonitor() {
			this._name = "Multi-Folder Monitor";
			this._monitors = new Dictionary<string, FolderMonitor>();
		}

		public MultiFolderMonitor(uint interval, params string[] paths) : this() {
			foreach(var path in paths) {
				this.AddNewMonitor(path, interval);
			}
		}

		public void AddNewMonitor(string path, uint interval = 60) {
			var monitor = new FolderMonitor(path, interval);
			AddNewMonitor(monitor);
		}

		public void AddNewMonitor(FolderMonitor monitor) {		
			if(!this._monitors.ContainsKey(monitor.DirectoryPath)) {
				this._monitors.Add(monitor.DirectoryPath, monitor);
				monitor.PathAvailabilityChanged += OnPathAvailabilityChanged;
			}			
		}

		public void RemoveMonitor(string directoryPath) {
			if(this._monitors.ContainsKey(directoryPath)) {
				var monitor = this._monitors[directoryPath];
				monitor.EnableRaisingEvents = false;
				monitor.PathAvailabilityChanged -= OnPathAvailabilityChanged;
				this._monitors.Remove(directoryPath);
				monitor.Dispose();
			}
		}

		public void Start() {
			foreach(var monitor in this._monitors) {
				monitor.Value.EnableRaisingEvents = true;
			}
		}

		public void Stop() {
			foreach(var monitor in this._monitors) {
				monitor.Value.EnableRaisingEvents = false;
			}
		}

		private void OnPathAvailabilityChanged(object sender, Args.PathAvailabilityEventArgs e) {
			this.PathAvailabilityChanged.Raise(sender, e);
		}

	}
}
