#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
# docker build -t wild-milan-host-service-api:latest -t wild-milan-host-service-api:0.0.5 --build-arg BUILD_VERSION=1.0.0 .
# docker run -d -p 8888:8080 --env ASPNETCORE_ENVIRONMENT=Development wild-milan-host-service-api:latest
# docker run -p 8888:80 --env ASPNETCORE_ENVIRONMENT=Development --entrypoint /bin/bash -it wild-milan-host-service-api:0.0.5

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_VERSION=0.0.0
ARG JFROG_USERNAME
ARG JFROG_PASSWORD
WORKDIR /source
COPY . ./

# FROM build AS publish
RUN dotnet nuget add source https://madness.jfrog.io/artifactory/api/nuget/nuget -n NugetProductMadness  -u ${JFROG_USERNAME} -p ${JFROG_PASSWORD} --store-password-in-clear-text
#RUN dotnet build ./Wildcat.Milan.Host.sln -c Debug -o /app/build/ --no-incremental
RUN dotnet build ./Wildcat.Milan.Host.sln -c Release -o /app/build/ --no-incremental

# FROM base AS final

# # Install the agent
# RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
# && echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
# && wget https://download.newrelic.com/548C16BF.gpg \
# && apt-key add 548C16BF.gpg \
# && apt-get update \
# && apt-get install -y newrelic-dotnet-agent

# # Set agent version as env var
# # RUN export NEW_RELIC_AGENT_VERSION=$(dpkg -s newrelic-dotnet-agent | grep Version)

# # Enable the agent
# ENV CORECLR_ENABLE_PROFILING=1 \
# CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
# CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
# CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
# NEW_RELIC_APP_NAME=wild-milan-host-service-api

# WORKDIR /app
# COPY --from=build /source /source
# COPY --from=build /app/build .
# RUN mkdir -p /app/plugins/
# RUN cp -R /source/Milan.Host/plugins/* /app/plugins/
# ENTRYPOINT ["dotnet", "Milan.Host.dll"]

