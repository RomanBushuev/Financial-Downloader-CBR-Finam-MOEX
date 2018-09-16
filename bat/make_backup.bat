echo off
SET PGPATH=C:\"Program Files"\PostgreSQL\9.6\bin\
SET SVPATH=C:\Users\RomanBushuev\YandexDisk\MarketData\backups\
SET DBUSR=postgres
SET DBDUMP=.sql
SET PGPASSWORD=roman

For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c_%%a_%%b)
SET BACKUPFILE=%mydate%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%
%PGPATH%pg_dump.exe -h localhost -p 5432 -U %DBUSR% -d MIR >%SVPATH%%BACKUPFILE%%DBDUMP%