-- starting now --

-- Completed --

Tony - I wanted to get this completed ASAP this morning, and strategically erred by not checking in a commit as I completed the remaining tests.
Mocking the logger turned out to be more complex that I would have thought, and that took the majority of the time in completing these initial tests.

As I started thinking about this a little more, I realized this implementation is not very scalable because it is mostly using IEnumerable instead of IQueryable, and is brittle
in that a client can't specify any logic to change the default implementations of the different filters.  If I were going to refactor this for production, I would look at both 
implementing a fluent syntax that supports using IQueryable for better execution efficiency as the size of the dataset increases, and using a rules engine strategy to make 
the system more configurable.
