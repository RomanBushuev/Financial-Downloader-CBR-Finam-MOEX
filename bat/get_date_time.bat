@echo off
For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c_%%a_%%b)
echo %mydate%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%
pause