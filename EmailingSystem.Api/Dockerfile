FROM microsoft/aspnetcore-build:2.0.0 AS build-env

WORKDIR /EmailingSystem.Api

COPY . .

RUN dotnet restore ./EmailingSystem.Api.sln

RUN dotnet build ./EmailingSystem.Api.sln

COPY . ./
RUN dotnet publish -c Release -o out

FROM microsoft/aspnetcore:2.0
WORKDIR /EmailingSystem.Api
EXPOSE 80
COPY --from=build-env /EmailingSystem.Api/EmailingSystem.Api/out .
ENTRYPOINT ["dotnet", "EmailingSystem.Api.dll"]