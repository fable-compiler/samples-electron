module.exports =
/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId])
/******/ 			return installedModules[moduleId].exports;
/******/
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			exports: {},
/******/ 			id: moduleId,
/******/ 			loaded: false
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.loaded = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(0);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ function(module, exports, __webpack_require__) {

	"use strict";
	
	exports.__esModule = true;
	exports.mainWindow = undefined;
	exports.createMainWindow = createMainWindow;
	
	var _electron = __webpack_require__(1);
	
	var _electron2 = _interopRequireDefault(_electron);
	
	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
	
	var mainWindow = exports.mainWindow = null;
	
	function createMainWindow() {
	    var options = {};
	    options.width = 800;
	    options.height = 600;
	    var webPreferences = {};
	    webPreferences.experimentalFeatures = true;
	    options.webPreferences = webPreferences;
	    var window = new _electron2.default.BrowserWindow(options);
	    window.loadURL("file://" + __dirname + "/../index.html");
	    window.on("closed", function () {
	        exports.mainWindow = mainWindow = null;
	    });
	    exports.mainWindow = mainWindow = window;
	}
	
	_electron2.default.app.on("ready", function () {
	    createMainWindow();
	});
	
	_electron2.default.app.on("window-all-closed", function () {
	    if (process.platform !== "darwin") {
	        _electron2.default.app.quit();
	    }
	});
	
	_electron2.default.app.on("activate", function () {
	    if (function () {
	        return mainWindow == null;
	    }()) {
	        createMainWindow();
	    }
	});


/***/ },
/* 1 */
/***/ function(module, exports) {

	module.exports = require("electron");

/***/ }
/******/ ]);
//# sourceMappingURL=main.js.map