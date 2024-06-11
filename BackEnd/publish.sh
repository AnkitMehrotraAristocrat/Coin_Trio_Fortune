#!/bin/bash
cd ../output
for d in */ ; do
    [ -L "${d%/}" ] && continue
    dotnet nuget push ${d}*.nupkg --api-key $NUGET_API_KEY -s https://jfrog.madness.games/artifactory/api/nuget/nuget-local/${d%/}
done