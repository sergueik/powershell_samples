FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src


COPY pbkdf2-csharp.sln ./
ADD Program Program
ADD Utils Utils
ADD Test Test


RUN dotnet restore pbkdf2-csharp.sln --runtime linux-musl-x64 && \
    dotnet build pbkdf2-csharp.sln -c Release --no-restore && \
    dotnet test Test -c Release --no-build 

# Publish only the executable and dependencies for alpine
RUN dotnet publish Program/Program.csproj -c Release -r linux-musl-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true -o /app

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine3.16 AS runtime
# NOTE: container does not include the .NET runtime itself.
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./Program"]

