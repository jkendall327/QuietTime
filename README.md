# QuietTime

Like [f.lux](https://justgetflux.com/) for your ears, QuietTime lets you cap your computer's maximum volume to prevent long-term hearing loss or make sure you don't blast out loud music at night.

![Screenshot](docs/main_window_screenshot.png)

You can use QuietTime to cap your system volume either on-command or according to schedules that you create, which repeat daily.

This program is inspired by, and indebted to, [Quiet on the Set](https://github.com/troylar/quiet-on-the-set). I wanted to make an updated version of the same idea in WPF, rather than Winforms, that included scheduling.

## Features

* Lock your system's max volume
* Create schedules with defined start and end points:
  * Cap your max volume to 20% at 8PM, then set it to 60% at 9AM the next day
  * Cap your volume to 0% during working hours (like 9AM - 5PM), then set it to 100% during your free time
  * And any other possibilities you can think of!
* Schedules saved in simple, human-readable JSON format so you can tweak them by hand
* Will run in the background if closed (but can be shut down completely through the tray icon)

## Installation and usage

QuietTime is a stand-alone executable. You should be able to simply click the .exe and get started.

Your settings are saved in a file called `appsettings.json`. By default, your saved schedules are in a file called `schedules.json`. You can edit both of these files directly, but any changes won't be noticed until you restart the program.

By default, the program doesn't exit completely when you close the main window. Instead, it goes to the system tray. Right-click on the icon there and choose the exit option to completely close QuietTime.

## Technologies

QuietTime is made with WPF and targets the .NET 6.0 runtime (how exciting!). It uses several NuGet packages:

* The Extended WPF Toolkit by Xceed for some user controls
* The Hardcodet NotifyIcon library for the tray icon
* Various Microsoft.Extensions libraries for dependency injection, configuration and logging
* The Microsoft MVVM Toolkit to help implement the MVVM design pattern
* NAudio by Mark Heath to interact with system audio
* NReco by Vitalii Fedorchenko for simple file-logging
* Quartz.NET by Marko Lahma for the scheduling framework back-end

## Issues and future goals

The back-end code for scheduling periods of lowered volume is quite messy. While it works, I'm not confident in it. I plan to add unit-tests to this and other parts of the program.

## Contributing

Please see CONTRIBUTING.md and SOURCE_GUIDE.md in the `docs` folder.

## Credits

[Catalin Fertu](https://catalinfertu.com/) for the program icon.
[Quiet on the Set](https://github.com/troylar/quiet-on-the-set) for initial inspiration.

## License

MIT License.
