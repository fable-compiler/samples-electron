# fable-electron-webGLTerrain
An electron app that 
- implements a webGLTerrain demo using three.js
- uses fable to transpile js and vscode to debug the electron process using source maps

Notable changes to [fable browser sample](https://github.com/fable-compiler/Fable/tree/master/samples/browser/webGLTerrain)

- three.js is brought in via npm
- node syntax used in source app/js to make available via node/webpack, e.g.
```
var  THREE = require('three')
module.exports.FirstPersonControls = function ( object, domElement ) {
```
- different `importX` syntax used in F# to access this source app/js (see renderer.fsx)

Adapted from 

- [fable browser webGLTerrain](https://github.com/fable-compiler/Fable/tree/master/samples/browser/webGLTerrain)
- [electron-quick-start](https://github.com/electron/electron-quick-start)
- [fable-electron](https://github.com/fable-compiler/fable-electron/tree/master/samples/helloworld)
- [getting started with fable](http://kcieslak.io/Getting-Started-with-Fable-and-Webpack)
- [debug electron apps with vscode](http://code.matsu.io/1)

## What's Electron?

The Electron framework lets you write cross-platform desktop applications using JavaScript, HTML and CSS. It is based on Node.js and Chromium and is used by the Atom editor and many other apps.
> https://github.com/electron/electron

## What's Fable?

[Fable](http://fable.io/) is a F# compiler that emits javascript. In this sample we write code in F#, transpile it appropriately into an Electron app, and then debug that app. 

This is a minimal Electron application based on the [Quick Start Guide](http://electron.atom.io/docs/latest/tutorial/quick-start) within the Electron documentation.

A basic Electron application needs just these files:

- `index.html` - A web page to render.
- `main.fsx` - Starts the app and creates a browser window to render HTML.
- `package.json` - Points to the app's main file and lists its details and dependencies.

You can learn more about each of these components within the [Quick Start Guide](http://electron.atom.io/docs/latest/tutorial/quick-start).

Additionally this sample adds

- `fableconfig.json` - for [fable compiler options](http://fable.io/); tutorial [here](http://kcieslak.io/Getting-Started-with-Fable-and-Webpack)
- `.vscode/launch.json` - for debugging in vscode; tutorial [here](http://code.matsu.io/1)
- `.vscode/tasks.json` - for build/watch tasks; tutorial [here](http://kcieslak.io/Getting-Started-with-Fable-and-Webpack)

## To Use

To clone and run this repository you'll need [Git](https://git-scm.com) and [Node.js](https://nodejs.org/en/download/) (which comes with [npm](http://npmjs.com)) installed on your computer. Some linux distributions call `node` something else, e.g. `nodejs`, which may cause you problems.

To use VSCode you will need to [install it](https://code.visualstudio.com/download).

I've also installed these extensions:

- [Debugger for Chrome](https://marketplace.visualstudio.com/items/msjsdiag.debugger-for-chrome). It must be version 1.1.0 or greater to allow breakpoints to be set in F#, see [here](https://github.com/octref/vscode-electron-debug/issues/2#issuecomment-251800254)

   **Ionide-fsharp does not seem to be necessary but slightly helpful for intellisense**

From your command line:

1. Clone this repository

   `git clone https://github.com/fable-compiler/fable-electron.git`

2. Build and Run

    * From command line
      1. Go into the samples/webGLTerrain directory

         `cd samples/webGLTerrain`

      2. Run fable (fableconfig.json contains the compiler options)

         `./node_modules/.bin/fable`

      3. Run electron

         `npm start`

    * From VSCode
      1. Start VSCode from the samples directory

         `code webGLTerrain`
      2. Run the build task

         `View -> Command Palette ; type run build task`
      3. Put a breakpoint in main.js or renderer.js, e.g. inside `createMainWindow`
      4. Enter debug mode, select main or render debug dropdown depending on breakpoint location
      5. Run the debugger. Debugger will break at F# line corresponding to the location of your js breakpoint. You may need to `ctrl-r` in Electron to refresh and hit breakpoints in render process, see [here](http://code.matsu.io/1)
      6. Once the debugger has hit the breakpoint you can set other breakpoints in F# that will function. These breakpoints appear to be in the sourcemap. If you set breakpoints in the F# source proper they will not function as expected and show up as greyed out breakpoints in the debug view while the debug session is running.

      **You can also run the `watch` task so that files are transpiled automatically on save. Enable by View -> Command Palette -> Type `tasks: run task` -> Select `watch`. Or ctrl-p and type `task watch`**

#### License [Unlicense (Public Domain)](LICENSE.md)
