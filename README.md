[![Join the chat at https://gitter.im/mj1856/corefx-dateandtime](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/mj1856/corefx-dateandtime?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

This repository contains a prototype project for improving the date and time features of the .NET Framework.  It currently contains:

- New types: `System.Date` and `System.TimeOfDay`
- Extension methods to `DateTime`, `DateTimeOffset`, and `TimeZoneInfo`

This is an unofficial community-based effort, being used to coordinate an implementation that *could* ultimately be merged into the .NET CoreCLR or a nuget package such as `System.Time`.

For more details, see [dotnet/corefx#700](https://github.com/dotnet/corefx/issues/700) and [dotnet/corefx#2116](https://github.com/dotnet/corefx/issues/2116).

#### Current Status

Nothing here is intended to be usable in its current state, as we might introduce breaking changes at any time.  The project is relatively stable, but is still open for debate and prototyping of new items.  We also need to add some more unit tests, and to start documenting examples of the key areas where the improvements matter most.

**Migration in Progress**

This project will soon be migrated to [dotnet/corefxlab](https://github.com/dotnet/corefxlab).  Current issues will be closed out and new ones will be opened as neccesary in the new repo.   Hang tight, this should happen in the next few days or so.
