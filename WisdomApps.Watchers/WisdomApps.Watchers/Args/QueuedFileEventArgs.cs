﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisdomApps.Watchers.Enums;

namespace WisdomApps.Watchers.Args
{
	public class QueuedFileEventArgs {
		private System.IO.FileInfo _fileInfo;

		public string FileName { get; private set; }
		public string WatchedDirectory { get; private set; }
		public string FullPath { get; private set; }
		public QueuedFileChangeType ChangeType { get; private set; }

		public System.IO.FileInfo FileInfo {
			get {
				if(_fileInfo == null) {
					_fileInfo = new System.IO.FileInfo(FullPath);
				}

				return _fileInfo;
			}
		}

		public QueuedFileEventArgs(string fileName, string watchedDirectory, string fullPath, QueuedFileChangeType changeType) {
			this.FileName = fileName;
			this.WatchedDirectory = watchedDirectory;
			this.ChangeType = changeType;
			this.FullPath = fullPath;
		}
	}
}
