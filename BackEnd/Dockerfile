FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
ARG BUILD_VERSION=0.0.0
ARG JFROG_USERNAME
ARG JFROG_PASSWORD
WORKDIR /source
COPY . .

RUN dotnet nuget add source https://madness.jfrog.io/artifactory/api/nuget/nuget -n NugetProductMadness  -u ${JFROG_USERNAME} -p ${JFROG_PASSWORD} --store-password-in-clear-text
RUN dotnet restore Wildcat.Milan.Backend.sln
RUN dotnet build ./Wildcat.Milan.Backend.sln -c Release -o /app/build/ --no-incremental -p:Version=${BUILD_VERSION}
RUN dotnet test Wildcat.Milan.Backend.sln

FROM base AS final

# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y newrelic-dotnet-agent

# Set agent version as env var
# RUN export NEW_RELIC_AGENT_VERSION=$(dpkg -s newrelic-dotnet-agent | grep Version)

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
NEW_RELIC_APP_NAME=wild-milan-host-service-api

WORKDIR /app
COPY --from=build /source /source
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "Wildcat.Milan.Host.dll"]

