module Mime

open Fable.Import
open Node.Exports

type MimeInfo =
    { Text : string
      Image : string
      Unkown : string
      Directory : string
      Pdf : string
      Html : string
      Word : string
      Powerpoint : string
      Movie : string
      Audio : string
      Css : string }

let iconMimeInfo =
    { Text = "fa-file-text-o"
      Image = "fa-file-image-o"
      Unkown = "fa-file-o"
      Directory = "fa-folder-o"
      Pdf = "fa-file-pdf-o"
      Html = "fa-html5"
      Word = "fa-file-word-o"
      Powerpoint = "fa-file-powerpoint-o"
      Movie = "fa-file-video-o"
      Audio = "fa-file-audio-o"
      Css = "fa-css3" }

let filetypeMimeInfo =
    { Text = "Text document"
      Image = "Image"
      Unkown = ""
      Directory = "fa-folder-o"
      Pdf = "PDF document"
      Html = "Html document"
      Word = "Word document"
      Powerpoint = "Powerpoint document"
      Movie = "Video document"
      Audio = "Audio document"
      Css = "Css document" }

let mapMimeInfo (info : MimeInfo) path isDirectory  =
    if isDirectory then
        info.Directory
    else
        let fileInfo = Path.parse path
        match fileInfo.ext with
        | ".txt" | ".md" -> info.Text
        | ".jpg" | ".jpge" | ".png" | ".gif" | ".bmp" -> info.Image
        | ".pdf" -> info.Pdf
        | ".css" -> info.Css
        | ".html" -> info.Html
        | ".doc" | ".docx" -> info.Word
        | ".ppt" | ".pptx" -> info.Powerpoint
        | ".mkv" | ".avi" | ".rmvb" -> info.Movie
        | ".mp3" -> info.Audio
        | _ -> info.Unkown

let determineIcon = mapMimeInfo iconMimeInfo

let determineFileType = mapMimeInfo filetypeMimeInfo