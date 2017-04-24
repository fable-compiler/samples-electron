# fable-electron sample with React and Material UI

To run the sample:

- First, go to the directory where this README is and install `npm` dependencies:

```
npm install
```

- After that, install `dotnet` dependencies for the current directory and those containing the F# projects (`Main` and `Renderer`) for the corresponding Electron threads.

```
dotnet restore
dotnet restore src/Main
dotnet restore src/Renderer
```

> Depending on your terminal, you can run all the commands in one line using the appropriate syntax. E.g. `dotnet restore; dotnet restore src/Main; dotnet restore src/Renderer`

- The previous steps only need to be completed once (unless dependencies change). During development, you can start the Fable server and Webpack in watch mode using the following command (`watch` is a script in package.json that just invokes `webpack -w`):

```
dotnet fable npm-run watch
```

- Fable and webpack will watch the F# files and recompile is there's any change. **In a different terminal** run the npm `start` script to start Electron.

```
npm run start
```
