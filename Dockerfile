FROM microsoft/dotnet:2.1-sdk AS build-env

ARG framework=netcoreapp2.1
ARG buildConfiguration=Release

WORKDIR /code

COPY src src
#COPY tests tests
COPY Zuto.Uk.Sample.NetCore.sln .

RUN dotnet restore -s https://www.nuget.org/api/v2/ -s http://nexus.zuto.network:8081/nexus/service/local/nuget/cl4u/

#RUN dotnet test tests/*/*.csproj -c ${buildConfiguration} -f ${framework}

RUN dotnet publish src/Zuto.Uk.Sample.API/Zuto.Uk.Sample.API.csproj -o /build -c ${buildConfiguration} -f ${framework}

# Build runtime image
FROM microsoft/dotnet:2.1.4-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /build /app
ENV TZ=Europe/London
ENTRYPOINT ["dotnet", "Zuto.Uk.Sample.API.dll"]