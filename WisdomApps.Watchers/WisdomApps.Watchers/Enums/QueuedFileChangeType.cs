using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WisdomApps.Watchers.Enums {
	enum QueuedFileChangeType {
		Attributes = 0,
		CreationTime,		
		FileName,
		LastAccess,
		LastWrite,
		Security,
		Size,
		Created,
		Deleted,
		FileRenamed,
		DirectoryRenamed
	}
}