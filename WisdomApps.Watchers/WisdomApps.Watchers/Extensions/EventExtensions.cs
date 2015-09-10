using System;

namespace WisdomApps.Watchers.Extensions {
	public static class EventExtensions {
		public static void Raise<T>(this EventHandler<T> handler, object sender, T args) {
			if(handler != null) {
				handler(sender, args);
			}
		}

		public static void Raise(this EventHandler handler, object sender, EventArgs args) {
			if(handler != null) {
				handler(sender, args);
			}
		}
	}
}
