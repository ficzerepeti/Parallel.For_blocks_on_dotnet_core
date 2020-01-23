# Parallel.For_blocks_on_dotnet_core

This project is created to demonstrate an odd behaviour which I didn't expect.

In case all ThreadPool threads are used then Parallel.For works as expected under .Net framework but blocks under .Net core.

This did complete on my machine: `dotnet run -f net472`

This hang on line where Parallel.For is called: `dotnet run -f netcoreapp3.1`
