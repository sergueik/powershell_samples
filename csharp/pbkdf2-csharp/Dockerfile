FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY pbkdf2-csharp.sln .
ADD Program Program
ADD Utils Utils
ADD Test Test

RUN dotnet restore pbkdf2-csharp.sln --runtime linux-x64

RUN dotnet build pbkdf2-csharp.sln -c Release --no-restore

RUN dotnet test Test -c Release --no-build

# Publish only the executable and dependencies
RUN dotnet publish Program/Program.csproj -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["./Program"]
