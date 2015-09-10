using System;

namespace WisdomApps.Watchers.Args {
	public class NotifyConcurrentQueueChangedEventArgs : EventArgs{

		public Enums.NotifyConcurrentQueueChangeTypes ConcurrentQueueChangeType { get; private set; }
		
		public NotifyConcurrentQueueChangedEventArgs(Enums.NotifyConcurrentQueueChangeTypes notifyConcurrentQueueChangeTypes) {

			this.ConcurrentQueueChangeType = notifyConcurrentQueueChangeTypes;
		}
	}
}
