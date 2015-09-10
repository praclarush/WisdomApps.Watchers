 #pragma warning disable 1587
///<copyright file="Disposable.cs" company="Wisdom Applications">
///Copyright (c) Wisdom Applications 2015 All Rights Reserved
///<author>Nathan Bremmer</author>
///</copyright>
#pragma warning restore 1587

using System;

namespace WisdomApps.Watchers.Helpers {
	public abstract class Disposable : IDisposable {
		protected bool Disposed { get; private set; }

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}


		protected virtual void Dispose(bool disposing) {
			this.Disposed = true;
		}

		~Disposable() {
			this.Dispose(false);
		}
	}
}
