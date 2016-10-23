#r "../node_modules/fable-core/Fable.Core.dll"
#load "../node_modules/fable-import-three/Fable.Import.Three.fs"

open Fable.Import
open System
open Fable.Core
open Fable.Core.JsInterop


/// Represents the API exposed by ImprovedNoise script
type IGinger =
    abstract init: unit -> unit

let Ginger = importMember<unit->IGinger>("../app/js/ginger.js")

let ginger = Ginger()
ginger.init()


Browser.document.getElementById("hide-header")?addEventListener("click",
    fun ev -> Browser.document.getElementById("sv-lab-header")?remove() );

Browser.document.getElementById("copytoclipboard-image")?addEventListener("click",
    fun ev -> 
        let image = Browser.document.getElementById("screenshot-image") :?> Browser.HTMLImageElement
        let timestamp = System.DateTime.Now.Ticks.ToString() //original was milliseconds; this is just as good
        let download  = Browser.document.createElement_a();
        download.href <- image.src;
        download?download <- "sv-ginger-" + timestamp + ".jpg";
        download.click() 
        let modal = Browser.document.getElementById("screenshot-modal");
        modal.classList.add("hidden") );

//Did not bother with bower clipboard dependency; that button now just dismisses window
Browser.document.getElementById("copytoclipboard-share")?addEventListener("click",
    fun ev -> 
            let modal = Browser.document.getElementById("share-modal");
            modal.classList.add("hidden") );