using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomApps.Watchers.Args {
	public class NotifyConcurrentQueueChangedEventArgs {

		public Enums.NotifyConcurrentQueueChangeTypes ConcurrentQueueChangeType { get; private set; }
		
		public NotifyConcurrentQueueChangedEventArgs(Enums.NotifyConcurrentQueueChangeTypes notifyConcurrentQueueChangeTypes) {

			this.ConcurrentQueueChangeType = notifyConcurrentQueueChangeTypes;
		}
	}
}
