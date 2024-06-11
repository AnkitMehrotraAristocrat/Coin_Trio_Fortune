# Milan Game Backend
This solution and project provides a template for building Milan-based game backends.

## Getting Started
This template solution contains two main projects:
* **The Game Backend Project** - Defines the services and configurations that make up a game backend.
* **The Milan Host Project** - A dependency project that provides you with everything that you need to develop locally
  on your desktop.

### Clone The Repository
Start by cloning this repository. Because the Milan Host project is referenced as a Git Submodule, there are a few
options to get the repository fully cloned onto your desktop:

Fully clone the repo and its submodules:

1. `git clone --recursive https://github.com/aristocratdd/milan-game-template`

Or, clone the repo followed by initializing and updating the submodules independently. This can be useful if you need
more control over the the Milan Host submodule for any reason.
1. `git clone https://github.com/aristocratdd/milan-game-template`
1. `cd milan-game-template`
1. `git submodule init backend/Milan/milan-host`
1. `git submodule update`

### Loading The Game Backend Solution
After cloning the repository you should have the following folder structure on your machine:
```
/milan-game-template
  /frontend
  /backend
    /GameBackend
      /Configuration
      /Data
      /Services
      /Steps
      - GameBackend.csproj
      - GameBackend.sln
    /Milan
      /development-adapters
      /milan-host
        - Milan.Host.csproj
```
To work with the game backend open the GameBackend.sln with your preferred IDE (e.g. Visual Studio or Rider). You'll
find that the project that launches when running or debugging is not the GameBackend project but the Milan.Host
project. This is because the Host must run as a debuggable ASP.NET service which discovers, loads, and instantiates
the game backend at run time.

In addition to discovering the game backend, the Host also discovers the included service adapter (
*Milan.ServiceAdapter.Dev*) and storage adapter (*Milan.Storage.FileSystem*) which are both found under the
`/milan-game-template/backend/Milan/development-adapters` folder.

As you develop your game backend you will want to make your life easier by introducing a proper project reference to
the Milan.Host project. Do not simply add a project reference as you normally would because this will introduce a
dependency relationship that could hide issues resolving your game backend's dependencies at Host load/backend
instantiation time. Instead, edit the `/milan-game-template/backend/Milan/milan-host/Milan.Host.csproj` directly and
add the following line:
```xml
<!-- Add this line to /milan-game-template/backend/Milan/milan-host/Milan.Host.csproj -->
<ProjectReference Include="..\..\..\GameBackend\GameBackend.csproj" ReferenceOutputAssembly="false"/>
```

This will ensure that your game backend is rebuilt and is copied to the Host's plugins folder for discovery.

## Interacting With The Game Backend
This backend template comes with a set of development adapters that give you everything you need to get started with
building your game backend:
* **Milan.ServiceAdapter.Dev** - A service adapter that exposes a generic REST endpoint and a Swagger interface that
  may be used to invoke the services on your game backend without the need for a frontend or application.


* **Milan.StorageAdapter.FileSystem** - A storage adapter that reads and writes to disk. The 
[storage.json file](./Milan/milan-host/Milan.Host/storage.json) can easily be inspected for debugging purposes or
deleted entirely when desired. Any hand-modification of the storage file should be followed by a restart of the 
Milan.Host to ensure that the in-memory data matches what is on disk.