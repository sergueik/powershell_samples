### Info
this directory contains source code from __Database create using PostgreSQL__ [codeproject article](https://www.codeproject.com/Articles/31969/DBCreate-In-Postgres) with PGSQL configured as nuget dependency

### Usage
test using the [sergueik/springboot_study/tree/master/basic-postgresql](https://github.com/sergueik/springboot_study/tree/master/basic-postgresql)

* NOTE: need to use `utf8` encoding
```text
2023-11-09 15:20:39.060 UTC [1] LOG:  database system is ready to accept connections
2023-11-09 15:22:21.021 UTC [43] ERROR:  encoding "WIN1252" does not match locale "en_US.utf8"
2023-11-09 15:22:21.021 UTC [43] DETAIL:  The chosen LC_CTYPE setting requires encoding "UTF8".
2023-11-09 15:22:21.021 UTC [43] STATEMENT:  CREATE DATABASE "test" WITH OWNER = postgres ENCODING = 'WIN1252'
2023-11-09 15:22:21.121 UTC [44] FATAL:  database "test" does not exist

```

### See Also 
  * [connect to remote PostgreSql database using Powershell](https://stackoverflow.com/questions/9217650/connect-to-remote-postgresql-database-using-powershell)
  * https://www.codeproject.com/Articles/1005146/Use-Postgres-JSON-Type-and-Aggregate-Functions-to
  * https://www.codeproject.com/Articles/1257203/Dockerize-A-Simple-Web-Application-Created-By-Usin
