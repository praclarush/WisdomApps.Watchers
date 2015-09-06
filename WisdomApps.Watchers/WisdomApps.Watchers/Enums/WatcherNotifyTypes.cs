using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomApps.Watchers.Enums
{
	[Flags]
	public enum QueuedFileNotifyFilters {		
		Attributes			= 1 << 0,
		CreationTime		= 1 << 1,
		DirectoryRenamed	= 1 << 2,
		FileName			= 1 << 3,
		LastAccess			= 1 << 4,
		LastWrite			= 1 << 5,
		Security			= 1 << 6,
		Size				= 1 << 7,
		Created				= 1 << 8,
		Deleted				= 1 << 9,
		FileRenamed			= 1 << 10,
		All					= 1 << 11
	}
}
