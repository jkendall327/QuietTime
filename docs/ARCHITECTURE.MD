# Guide to the QuietTime source

QuietTime is a small project. If you are familiar with WPF and MVVM, nothing should be too surprising. The most interesting classes are the `AudioService` and `SchedulerService`. These implement audio interfacing and job scheduling.

## Projects

The QuietTime project houses the WPF application with UI concerns and Windows-specific service implementations.

QuietTime.Core contains domain objects, interfaces and portable code.

## QuietTime.Controls

The `NavigationBar` handles switching between screens.

The `TrayIcon` is for the system tray icon and its context menu.

## Models

A `Schedule` represents a period of time where the system's volume is locked. It consists of a start point, an endpoint, a maximum volume, and what the maximum should be set to once the period ends.

The `ScheduleDTO` is used for serializing awkward `TimeOnly` and `JobKey` properties.

## Services

The `IAudioLocker` interface and `AudioLocker` implementation encapsulate the user's actual audio hardware. It is what actually limits your audio. It provides events for when the user's volume has changed levels, or when the volume is locked/unlocked.

The `AudioLocker` implementation is in the UI layer because it relies on a Windows-specific NuGet package and I didn't want that tied to the domain project.

The `IScheduler` and `Scheduler` encapsulate queueing up a Schedule for later execution. Currently, all Schedules are a start point and an end point that repeats daily. This current implementation is essentially a wrapper for Quartz.NET.

`INotifier` and `Notifier` encapsulate sending messages to the user through toast notifications. It currently does this with Hardcodet's `TrayIcon` class, out of convenience.

`ISerializer` and `Serializer` encapsulate saving a user's Schedules, with appropriate error-handling.

The `Navigator` handles MVVM navigation between ViewModels.

The `Autostarter` encapsulates getting QuietTime to start automatically on Windows sign-in.

## ViewModels

These classes provide data WPF binds to for the user interface.

The ScheduleWindowVM is the bridge between the front-end of creating a Schedule and the SchedulerService back-end.

The AudioAwareBaseVM is used as a parent class for the tray icon's viewmodel and the main page's viewmodel.

## Views

These are the user-facing XAML pages which create the interface.

The `HostWindow` 'contains' the various Page files as well as the `NavigationBar` control.

## Startup

The program root is found in the code-behind to App.xaml. This is where DI is set up.

## Other

The Converters file contains classes that let WPF handle more data than it can by default. For instance, translating a `TimeOnly` property into a `DateTime` object for UI display.

The `Extensions` class contains static extension methods.

The `EventIds` class provides strongly-typed event classification for log files.

The `MessageLevel` enum provides strongly-typed importance levels for user-facing notifications.

The `VolumeAmounts` enum is for changing the current max volume in discrete steps rather than hard-coded numbers.

The `DIContainerProvider` encapsulates setting up dependency injection for the app.

`UserSettings` is a strongly-typed way of getting and storing user settings.

The `BindingProxy` class is used to work around a limitation of WPF's databinding for context-menus on elements outside of the visual tree (in this case, the system tray icon).
