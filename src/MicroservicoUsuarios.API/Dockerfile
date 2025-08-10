FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.sln .
COPY src/MicroservicoUsuarios.API/*.csproj src/MicroservicoUsuarios.API/
COPY src/MicroservicoUsuarios.Application/*.csproj src/MicroservicoUsuarios.Application/
COPY src/MicroservicoUsuarios.Core/*.csproj src/MicroservicoUsuarios.Core/
COPY src/MicroservicoUsuarios.Infrastructure/*.csproj src/MicroservicoUsuarios.Infrastructure/
COPY tests/MicroservicoUsuarios.Tests.Unit/*.csproj tests/MicroservicoUsuarios.Tests.Unit/

RUN dotnet restore

COPY . .

WORKDIR /app/src/MicroservicoUsuarios.API

COPY src/MicroservicoUsuarios.API/appsettings.json .

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app

COPY --from=build /app /app

RUN dotnet tool install --global dotnet-ef

ENV PATH="${PATH}:/root/.dotnet/tools"

ENTRYPOINT ["/bin/bash", "-c", "dotnet ef database update --project src/MicroservicoUsuarios.Infrastructure/ --startup-project src/MicroservicoUsuarios.API/ && dotnet /app/publish/MicroservicoUsuarios.API.dll"]