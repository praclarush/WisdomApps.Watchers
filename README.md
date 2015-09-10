# README #

### What is this repository for? ###

* A Collection of Different File and Folder Monitors/Watchers
* 1.0.0.0

### Current Watchers/Monitors ###

* Folder Availability Watcher (watches a directory to make sure it's available and raises events when that availability changes ie.. Network Folders))
* A Multi-Directory Folder Availability Watcher (Monitors multiple different directories, that are all passed though the same events)
* Queued File System Watcher (uses FileSystemWatcher)
* A Multi-Directory Queued File System Watcher (Watchers multiple different directories, that are all passed though the same events)
* Queued Timed File System Watcher (uses a polling system on a timer)
* A Multi-Directory Queued Timed File System Watcher (Watchers multiple different directories, that are all passed though the same events)

### Other Things that will be moved from the Library ###

* ObservableConcurrentQueue - A Concurrent Queue that raises events when a item is Enqueued, Dequeued, or Peeked
* Extensions for Events