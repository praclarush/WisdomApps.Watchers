using System;
using WisdomApps.Watchers.Enums;

namespace WisdomApps.Watchers.Args {
	public class StatusChangedEventArgs : EventArgs{
		public WatcherStatus CurrentStatus { get; private set; }
		public WatcherStatus PreviousStatus { get; private set; }

		public StatusChangedEventArgs(WatcherStatus previousStatus, WatcherStatus currentStatus) {
			this.CurrentStatus = currentStatus;
			this.PreviousStatus = previousStatus;
		}
	}
}
