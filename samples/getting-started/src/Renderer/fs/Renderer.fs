module App.Renderer

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

let body = Browser.document.getElementsByTagName_h1().[0]
body.textContent <- App.Message.message
