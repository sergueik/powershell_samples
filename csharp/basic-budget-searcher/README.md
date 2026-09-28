Exactly. The missing input isn't really another search criterion. It is a search budget / termination policy.
bounded asynchronous filesystem traversal engine with pluggable result sinks
NOTE: cancellation and inaccessible folders/files as first-class outcomes,
immediate children of the selected root:
concept it becomes a set of global counters + events.
the traversal itself doesn't need to know why it has stopped

What to search

* Root folder — `BrowseForFolder`
* File mask —  e.g. `*.xml`, `invoice*.pdf`, etc.
* Content condition — literal text, regex, perhaps eventually metadata/size/date conditions.


**When to stop**

|Mode|Input|Meaning|
|----|-----|-------|
|File limit|N files|Stop after examining N files|
|Folder limit|N folders|Stop after visiting N top level directories, recursively|
|Time limit|N seconds/minutes|Stop when the elapsed-time budget expires|
|Unlimited|—|Search until the entire reachable tree is exhausted|

these condition could be composable, rather than mutually exclusive:


*Stop when any limit is reached*.

For example:

Search `\\server\share\department`, recursively, for `*.xml` containing `CUSTOMER_ID`, for up to 5,000 files, 10 top level folders, or 60 seconds — *whichever occurs first*.

That is much more appropriate for an SMB environment than pretending the user is doing a normal local Windows Explorer search.
