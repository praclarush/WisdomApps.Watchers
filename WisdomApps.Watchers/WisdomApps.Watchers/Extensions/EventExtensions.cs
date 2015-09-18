using System;

namespace WisdomApps.Watchers.Extensions {
	public static class EventExtensions {
		public static void Raise<T>(this EventHandler<T> handler, object sender, T args) {
			var safeHandler = handler;
			if(safeHandler != null) {
				safeHandler(sender, args);
			}
		}

		public static void Raise(this EventHandler handler, object sender, EventArgs args) {
			var safeHandler = handler;
			if(safeHandler != null) {
				safeHandler(sender, args);
			}
		}
	}
}
