@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0SwitchSaveProfile.ps1" -Action CaptureComplete -Force
pause
