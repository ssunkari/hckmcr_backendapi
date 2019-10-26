FROM microsoft/dotnet:2.2-sdk AS build-env

ARG framework=netcoreapp2.2
ARG buildConfiguration=Release

WORKDIR /code

COPY src src
#COPY tests tests
COPY DaoApi.sln .

# RUN dotnet restore -s https://www.nuget.org/api/v2/ -s http://nexus.build.zuto.cloud:8081/nexus/service/local/nuget/cl4u/
RUN dotnet restore -s https://www.nuget.org/api/v2/ 

#RUN dotnet test tests/*/*.csproj -c ${buildConfiguration} -f ${framework}

RUN dotnet publish src/DaoApi/DaoApi.csproj -o /build -c ${buildConfiguration} -f ${framework}

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /build /app
ENV TZ=Europe/London
ENTRYPOINT ["dotnet", "DaoApi.dll"]
