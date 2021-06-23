# Guide to the QuietTime source

This is a fairly small project that can be understood without much effort. The most important classes are the `AudioService` and SchedulerService classes that implement audio interfacing and job scheduling respectively. The classes in the ViewModels folder are the 'glue' that provides most of the program's functionality.

## Models

Currently there is only one model, the Schedule, which represents a set period in which the system's volume is manipulated. A Schedule consists of a start point, an endpoint, a maximum volume during this period and what the maximum should be set to once the period ends.

## Services

The AudioService encapsulates interaction with the actual audio hardware of the user's computer. It is what does the actual work of limiting your audio. It provides events for when the user's volume has changed levels, or when the volume is locked/unlocked.

The SchedulerService encapsulates queueing up a Schedule model to be executed later. Currently, all Schedules are treated the same way: a start point and an end point that repeats every day forever. This class is essentially a convenient wrapper for Quartz.NET functionality.

## ViewModels

These classes contain little actual logic, but provide intelligent properties that WPF can bind to for a reactive user interface. The ScheduleWindowVM is the bridge between the front-end of creating a Schedule and the SchedulerService back-end.

## Views

These are the user-facing XAML pages which create the interface.

The code-behind to the MainWindow contains additional code for two purposes:

* Automatically scaling UI elements when the window is resized
* Sending the program to the system tray when the user closes the window, rather than shutting down the application entirely

## Startup

The code-behind to the App.xaml file is where the dependency injection container is set up and the program instantiated. Unhandled exceptions are also caught here and logged.

## Other

The Converters class provides ways for WPF to intelligently bind to values for a more interesting UI. For example, the background of the main screen changes colour when the volume is locked. This is achieved through a BoolToButtonTextConverter in this class.

The Extensions class contains static extension methods that had no home elsewhere.

The EventIds class is a convenient way to add extra information to log files.
