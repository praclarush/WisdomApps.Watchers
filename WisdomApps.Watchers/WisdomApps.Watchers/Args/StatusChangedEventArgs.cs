using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisdomApps.Watchers.Enums;

namespace WisdomApps.Watchers.Args {
	public class StatusChangedEventArgs : EventArgs{
		WatcherStatus CurrentStatus { get; private set; }
		WatcherStatus PreviousStatus { get; private set; }

		public StatusChangedEventArgs(WatcherStatus previousStatus, WatcherStatus currentStatus) {
			this.CurrentStatus = currentStatus;
			this.PreviousStatus = previousStatus;
		}
	}
}
