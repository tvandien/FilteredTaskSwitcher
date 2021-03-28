# FilteredTaskSwitcher
I recently bought a big monitor, and now I have too many windows open to use the normal alt-tab task switcher in Windows. This app offers an alternative that allows filtered alt-tabs.

# Setup
Set the hotkeys you like in `Forms\TaskSwitch.cs:InitializeTaskSwitchers()`.

Set the processes you'd like to filter for `Forms\TaskSwitch.cs:InitializeTaskSwitchers()`.

# Usage
Press the hotkey you've configured (default is alt-f13, shift-alt-f13, alt-f14, shift-alt-f14). Hold down alt to see the available windows.

# Todo
I want to extend this app to allow filtering based on the windows that are currently behind your mouse, rather than filtering on process name.