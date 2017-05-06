Fable Electron Getting Started
=====================

**This is an Electron adaptation of [the regular Fable getting started project](https://github.com/fable-compiler/fable-getting-started).**

For the Impatient & Experienced
======================
- `git clone https://github.com/fable-compiler/fable-electron.git`
- In the `samples/getting-started` folder, run `yarn install` (may require installing `yarn` with `npm install --global yarn`)
- `dotnet restore`
- `dotnet restore src/Main`
- `dotnet restore src/Renderer`
- In a terminal, `dotnet fable npm-run watch` to start a build/watch task
- In VsCode, run a debug task, or in a separate terminal, `npm run start`


For Everyone Else
======================

Read the docs at [the regular Fable getting started project](https://github.com/fable-compiler/fable-getting-started).

Almost everything is the same except

- We are using Electron to present the output js rather than a web server
- The `public` web server directory structure has been transformed to `app`
- Our output file is `main.js` instead of `bundle.js`