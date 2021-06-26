# Guide to the QuietTime source

QuietTime is a small project. If you are familiar with WPF and MVVM, nothing should be too surprising. The most interesting classes are the `AudioService` and `SchedulerService`. These implement audio interfacing and job scheduling, respectively.

## Models

Currently there is only one model, the Schedule. This represents a period of time where the system's volume is locked. A Schedule consists of a start point, an endpoint, a maximum volume, and what the maximum should be set to once the period ends.

A DTO is used for serializing awkward `TimwOnly` and `JobKey` properties.

## Services

The `AudioService` encapsulates the user's actual audio hardware. It is what actually limits your audio. It provides events for when the user's volume has changed levels, or when the volume is locked/unlocked.

The `SchedulerService` encapsulates queueing up a Schedule for later execution. Currently, all Schedules are a start point and an end point that repeats daily. This class is essentially a wrapper for Quartz.NET.

The `NotificationService` encapsulates sending messages to the user (currently only through toast notifications). It does this with Hardcodet's TrayIcon class, out of convenience, but this implementation could be easily swapped out.

The `SerializerService` encapsulates saving a user's Schedules, with appropriate error-handling.

## ViewModels

These classes provide data WPF binds to for the user interface. The ScheduleWindowVM is the bridge between the front-end of creating a Schedule and the SchedulerService back-end.

## Views

These are the user-facing XAML pages which create the interface.

The code-behind contains additional code for:

* Automatically scaling UI elements when the window is resized
* Sending the program to the system tray when the user closes the main window

## Startup

The code-behind to App.xaml is where the dependency injection container is set up and the program instantiated. Command-line arguments and unhandled exceptions are also processed here.

## Other

The Converters class lets WPF handle more data than it can by default. For instance, translating a `TimeOnly` property into a `DateTime` object for UI display.

The `Extensions` class contains static extension methods that had no home elsewhere.

The `EventIds` class is a strongly-typed way to add extra information to log files.

The `ScaleValueHelper` provides a simple way for windows to automatically scale their contents when resized.

The `Settings` class is used with the Options pattern to provide access to strongly-typed program settings at runtime.