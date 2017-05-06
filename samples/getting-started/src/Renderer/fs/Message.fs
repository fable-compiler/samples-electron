module App.Message

open Fable.Core.JsInterop

let message = importMember<string> "../js/Message.js"
