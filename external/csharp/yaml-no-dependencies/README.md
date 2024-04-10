### Info
clone of [Basic Yaml parser for .net](https://github.com/kthompson/yaml) witn no `packages.config` dependency on YamlDotNet or anything else. Added missing `packages.config` for Nunit.

The goal is to have a reasonably functional full source for later embed into Powershell sceipt to decrypt specific entries found in a generic Java `application.yaml` 

### Usage

* compile library and tester console app
```
```
* parse a generic Java Spring `application.yaml` which typically looks like this

```yaml
spring:
  application:
    name: spring-mongo-crud
  datasource:
    database1:
      url: 'jdbc:postgresql://localhost:5432/DB_NAME'
      username: username
      password: ENC(AAtJwmQioe5oOai++Nu7r7ucxyZNXVPP2AEmA22NOhkSveAtvNaLCSdJ2tLfhLV2)
      driver-class-name: org.postgresql.Driver
      jpa:
        show-sql: true
        open-in-view: false
        generate-ddl: true
        database-platform: org.hibernate.dialect.PostgreSQL10Dialect
        properties:
          hibernate:
            format_sql: true
            order_inserts: true
            order_updates: true
            jdbc:
              batch_size: 15
            globally_quoted_identifiers: true
            globally_quoted_identifiers_skip_column_definitions: true
            query:
              plan_cache_max_size: 4096
              #fail_on_pagination_over_collection_fetch: true
              in_clause_parameter_padding: true
        hibernate:
          ddl-auto: update
    database2:
      data:
        mongodb:
          uri: "mongodb://${MONGO_HOST:localhost}:${MONGO_PORT:27017}/${spring.application.name}"
  profiles:
    active: local
```
* run
```powershell
Tester\bin\Debug\Tester.exe application.yml
```
you will see the streaming processing log of the following structure:

```text
{
 ( string : <spring> )
 :
 {
  ( string : <application> )
  :
  {
   ( string : <name> )
   :
   ( string : <spring-mongo-crud> )
  }
  ( string : <datasource> )
  :
  {
   ( string : <database1> )
   :
   {
    ( string : <url> )
    :
    ( string : <'jdbc:postgresql://localhost:5432/DB_NAME'> )
    ( string : <username> )
    :
    ( string : <user> )
    ( string : <password> )
    :
    ( string : <ENC(AAtJwmQioe5oOai++Nu7r7ucxyZNXVPP2AEmA22NOhkSveAtvNaLCSdJ2tLfhLV2)> )
    ( string : <driver-class-name> )
    :
    ( string : <org.postgresql.Driver> )
    ( string : <jpa> )
    :
    {
     ( string : <show-sql> )
     :
     ( string : <true> )
     ( string : <open-in-view> )
     :
     ( string : <false> )
     ( string : <generate-ddl> )
     :
     ( string : <true> )
     ( string : <database-platform> )
     :
     ( string : <org.hibernate.dialect.PostgreSQL10Dialect> )
     ( string : <properties> )
     :
     {
      ( string : <hibernate> )
      :
      {
       ( string : <format_sql> )
       :
       ( string : <true> )
       ( string : <order_inserts> )
       :
       ( string : <true> )
       ( string : <order_updates> )
       :
       ( string : <true> )
       ( string : <jdbc> )
       :
       {
        ( string : <batch_size> )
        :
        ( string : <15> )
       }
       ( string : <globally_quoted_identifiers> )
       :
       ( string : <true> )
       ( string : <globally_quoted_identifiers_skip_column_definitions> )
       :
       ( string : <true> )
       ( string : <query> )
       :
       {
        ( string : <plan_cache_max_size> )
        :
        ( string : <4096> )
        ( string : <in_clause_parameter_padding> )
        :
        ( string : <true> )
       }
      }
     }
     ( string : <hibernate> )
     :
     {
      ( string : <ddl-auto> )
      :
      ( string : <update> )
     }
    }
   }
   ( string : <database2> )
   :
   {
    ( string : <data> )
    :
    {
     ( string : <mongodb> )
     :
     {
      ( string : <uri> )
      :
      ( string : <"mongodb://${MONGO_HOST:localhost}:${MONGO_PORT:27017}/${spring.application.name}"> )
     }
    }
   }
  }
  ( string : <profiles> )
  :
  {
   ( string : <active> )
   :
   ( string : <local> )
  }
 }
}

```
NOTE: quoting is required when values include colon to prevent parser from crashing;

omitting quotes in line 5 and similar, that is replacing
```yaml
url: 'jdbc:postgresql://localhost:5432/DB_NAME'
```
with
```yaml
url: jdbc:postgresql://localhost:5432/DB_NAME
```
leads to a parser crash:
```text
Error near line 5: Yaml.ParserException: Unterminated inline value at Yaml.YamlParser.ParseValue(Int32 n):line 525
```
Also it is clear that locating the specific node `spring.datasource.database1.password` by a streaming parser which lacks ability of building the object tree:
```text
    ( string : <password> )
    :
    ( string : <ENC(AAtJwmQioe5oOai++Nu7r7ucxyZNXVPP2AEmA22NOhkSveAtvNaLCSdJ2tLfhLV2)> )
```
will be challenging

### NOTE

Currently the Block value and EmptyValue tests are failing therefore YAML with comment(s) 

will crash the parser


### See Also
https://github.com/janno-p/YamlSharp/tree/master

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
