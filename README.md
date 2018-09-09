# PrintSpoolerFixer
Un-freezes Windows 10's print queue when it messes up
Please note that you must be running the program as administrator for it to work correctly.
Here's what this program does:
1. Stop Print Spooler service
2. Stop "printfilterpipelinesvc.exe" task
3. Delete contents of C:\Windows\System32\spool\PRINTERS
4. Restart the Print Spooler service

Normally you have to restart your computer to fix this problem but this program fixes it without a restart.

Don't want to go through the trouble of compiling this yourself? Download it here:
http://lolyoujellybro.com/makewindowsgreatagain.html#printspooler
