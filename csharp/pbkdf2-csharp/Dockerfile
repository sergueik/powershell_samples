FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY ./pbkdf2-csharp.sln ./
COPY ./Program ./Program
COPY ./Utils ./Utils
COPY ./Test ./Test

RUN dotnet restore pbkdf2-csharp.sln --runtime linux-x64

RUN dotnet build pbkdf2-csharp.sln -c Release --no-restore

RUN dotnet test Test -c Release --no-build

# make a release build
RUN dotnet publish --no-self-contained --runtime linux-x64 \
    --configuration Release \
    --output /app/bin/ --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app/bin
COPY --from=build /app/bin /app/bin

# leave all arguments hanging
WORKDIR /app/bin
# 
RUN  chmod +x Program
# NOTE: exec form
# ENTRYPOINT ./Program
ENTRYPOINT [ "./Program" ]
