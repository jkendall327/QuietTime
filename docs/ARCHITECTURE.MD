# Guide to the QuietTime source

QuietTime is a small project. If you are familiar with WPF and MVVM, nothing should be too surprising. The most interesting classes are the `AudioService` and `SchedulerService`. These implement audio interfacing and job scheduling, respectively.

## Controls

The `NavigationBar` is what lets you switch between screens.

The `TrayIcon` is for the icon in the system tray and its context menu.

## Models

A `Schedule` represents a period of time where the system's volume is locked. It consists of a start point, an endpoint, a maximum volume, and what the maximum should be set to once the period ends.

The `ScheduleDTO` is used for serializing awkward `TimeOnly` and `JobKey` properties.

## Services

The `AudioService` encapsulates the user's actual audio hardware. It is what actually limits your audio. It provides events for when the user's volume has changed levels, or when the volume is locked/unlocked.

The `Scheduler` encapsulates queueing up a Schedule for later execution. Currently, all Schedules are a start point and an end point that repeats daily. This class is essentially a wrapper for Quartz.NET.

The `Notifier` encapsulates sending messages to the user (currently only through toast notifications). It does this with Hardcodet's `TrayIcon` class, out of convenience, but this implementation could be easily swapped out.

The `Serializer` encapsulates saving a user's Schedules, with appropriate error-handling.

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

The program is instantiated in the code-behind to App.xaml. Command-line arguments are also processed here.

## Other

The Converters class lets WPF handle more data than it can by default. For instance, translating a `TimeOnly` property into a `DateTime` object for UI display.

The `Extensions` class contains static extension methods that had no home elsewhere.

The `EventIds` class provides a strongly-typed way to add extra information to log files.

The `DIContainerProvider` encapsulates setting up dependency injection for the app.

`UserSettings` is a strongly-typed way of getting and storing user settings.
