# QuietTime

Like [f.lux](https://justgetflux.com/) for your ears, QuietTime caps your computer's maximum volume to prevent long-term hearing damage or accidentally blasting your ears with loud music late at night.

![Screenshot](docs/main_window_screenshot.png)

You can limit your system volume manually, or according to schedules that you create.

This program is inspired by [Quiet on the Set](https://github.com/troylar/quiet-on-the-set). I wanted to make an updated version of the same idea in WPF, rather than Winforms, that included scheduling.

## Features

* Lock your system's max volume
* Create schedules with defined start and end points:
  * Cap your max volume to 20% at 8PM, then set it to 60% at 9AM the next day
  * Cap your volume to 0% during working hours (like 9AM - 5PM), then set it to 100% during your free time
  * And any other possibilities you can think of!
* Schedules and settings file saved in simple, human-readable JSON format you can tweak by hand
* Runs in background by default (but can be completely closed through the tray icon)

## Installation and usage

QuietTime is a stand-alone executable. Simply click the .exe to get started.

Your settings are saved in a file called `appsettings.json`. By default, your saved schedules are in a file called `schedules.json`. You can edit both of these files directly, but changes won't be reflected until you restart the program.

In the settings window, you can choose for QuietTime to automatically launch when you sign-in to Windows. You can also opt for it to start minimized (in the system tray).

## Technologies

QuietTime is made with WPF and targets the .NET 6.0 runtime (how exciting!). It uses several NuGet packages:

* The Extended WPF Toolkit by Xceed for some user controls
* The Hardcodet NotifyIcon library for the tray icon
* Microsoft.Extensions for dependency injection, configuration and logging
* The Microsoft MVVM Toolkit to help implement the MVVM design pattern
* NAudio by Mark Heath for system audio
* NReco by Vitalii Fedorchenko for simple file-logging
* Quartz.NET by Marko Lahma for scheduling

## Issues and future goals

The back-end code for scheduling periods of lowered volume is quite messy. While it works, I'm not confident in it. I plan to add unit-tests to this and other parts of the program.

I have only tested QuietTime on my personal set-up. Please let me know if something doesn't work on your system!

## Contributing

Please see CONTRIBUTING.md and ARCHITECTURE.md in the `docs` folder.

## Credits

[Catalin Fertu](https://catalinfertu.com/) for the program icon.
[Quiet on the Set](https://github.com/troylar/quiet-on-the-set) for initial inspiration.
