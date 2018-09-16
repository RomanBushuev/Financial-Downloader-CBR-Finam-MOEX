echo off
SET PGPATH=C:\"Program Files"\PostgreSQL\9.6\bin\
SET SVPATH=C:\Users\bushuevroman\YandexDisk\MarketData\EndPoints\2018.06.30\
SET DBUSR=postgres
SET PGPASSWORD=roman

SET BACKUPFILE=2018_06_30_23_00_00.dump
%PGPATH%dropdb.exe -h localhost -p 5432 -U %DBUSR% MIR
%PGPATH%createdb.exe -h localhost -p 5432 -U %DBUSR% -T template0 MIR
%PGPATH%pg_restore.exe -h localhost -p 5432 -U %DBUSR% -d MIR %SVPATH%%BACKUPFILE%