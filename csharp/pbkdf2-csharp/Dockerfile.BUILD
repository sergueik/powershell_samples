FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY ./pbkdf2-csharp.sln ./
COPY ./Program ./Program
COPY ./Utils ./Utils
COPY ./Test ./Test

# RUN dotnet restore pbkdf2-csharp.sln
RUN dotnet restore pbkdf2-csharp.sln --runtime linux-x64

RUN dotnet build pbkdf2-csharp.sln -c Release --no-restore

RUN dotnet test Test -c Release --no-build

RUN dotnet publish --no-self-contained --runtime linux-x64 \
    --configuration Release \
    --output /app/bin/ --no-restore
