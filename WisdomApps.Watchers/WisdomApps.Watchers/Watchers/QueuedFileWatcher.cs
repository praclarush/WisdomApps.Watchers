using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomApps.Watchers
{
	//Idea(Nathan): Directory Cleanup on Error; If the watcher raises the error event run though the directory and pick up all files that where forgotten.
	class QueuedFileWatcher
	{
		//Note(Nathan): to remove load on the file watcher events if move directory is enabled use internal queue to temp hold files before so that a new thread can do the moving.  
	}
}
