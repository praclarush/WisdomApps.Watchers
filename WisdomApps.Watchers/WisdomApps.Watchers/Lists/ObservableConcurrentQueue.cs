using System;
using System.Collections.Concurrent;
using WisdomApps.Watchers.Enums;
using WisdomApps.Watchers.Args;
using WisdomApps.Watchers.Extensions;

namespace WisdomApps.Watchers.Lists {
	public class ObservableConcurrentQueue<T> : ConcurrentQueue<T> {
		public bool EnableRaisingEvents {get; set;}
				
		public event EventHandler<NotifyConcurrentQueueChangedEventArgs> NotifyConcurrentQueueChagned;

		public ObservableConcurrentQueue() : base() {
			//Note(Nathan): decide if Events should be enabled by default.
			//EnableRaisingEvents = true;
		}

		public new bool TryDequeue(out T result) {
			if(!base.TryDequeue(out result)) {
				return false;
			}

			OnQueueChanged(NotifyConcurrentQueueChangeTypes.Dequeued);

			if(this.IsEmpty) {
				OnQueueChanged(NotifyConcurrentQueueChangeTypes.Empty);
			}

			return true;
		}

		public new void Enqueue(T item) {
			base.Enqueue(item);

			OnQueueChanged(NotifyConcurrentQueueChangeTypes.Enqueued);
		}

		public new bool TryPeek(out T result) {
			if(!base.TryPeek(out result)) {
				return false;
			}

			OnQueueChanged(NotifyConcurrentQueueChangeTypes.Peeked);
			return true;
		}

		private void OnQueueChanged(NotifyConcurrentQueueChangeTypes changeType) {
			if(EnableRaisingEvents) {
				this.NotifyConcurrentQueueChagned.Raise(this, new NotifyConcurrentQueueChangedEventArgs(changeType));
			}			
		}
	}
}
