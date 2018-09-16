echo off
SET PGPATH=C:\"Program Files"\PostgreSQL\9.6\bin\
SET DBUSR=postgres
SET PGPASSWORD=roman

%PGPATH%dropdb.exe -h localhost -p 5432 -U %DBUSR% MIR