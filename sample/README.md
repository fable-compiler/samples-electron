# Simple sample of an Electron

## Goal

This project aim to provide a basic sample of how to use Fable to build an electron application.

The application will show 2 square on the screen and refresh the application every time you update the **Client application**.

## How to build ?

1. Run `yarn install`
2. Run `.paket/paket.exe restore`
3. Move into the client folder: `cd client`
4. Run `dotnet restore`
5. Run `dotnet fable yarn-start`
6. In a new termical, run `yarn launch`.

## Architecture

The sample is split in two part:

1. The electron application, responsible to create a new window
2. The client application, responsible to generate the code using by `index.html`

### Electron application

The code of this application is located into the `electron` folder. Here we use an `.fsx` file but you can also use an fsproj if your application is more complex.

We use **Fable Splitter** to compile this application.

### Client application

The code of this application is located into the `client` folder.

We use **webpack** to compile this application. It's important to note that we do not use **webpack-dev-server** as we want the files to be written into the disk and not serve from a distant server.