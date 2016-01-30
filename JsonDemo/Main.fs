namespace JsonDemo

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Server

type Action =
    | Home
    | Api of RestApi.Action

module Skin =
    open System.Web

    type MainTemplate = Templating.Template<"Main.html">

    let WithTemplate title body : Async<Content<Action>> =
        Content.Page(
            Body = [
                MainTemplate.Doc(
                    title = title,
                    body = body
                )
            ]
        )

module Site =

    let HomePage (ctx: Context<Action>) =
        Skin.WithTemplate "HomePage"
            [
                client <@ Client.Main() @>
            ]

    let Main =
        Sitelet.Sum [
            Sitelet.Content "/" Home HomePage
            Sitelet.Shift "api" (Sitelet.EmbedInUnion <@ Api @> RestApi.Sitelet)
        ]

[<Sealed>]
type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Main
        member this.Actions = [Home]

type Global() =
    inherit System.Web.HttpApplication()

    member g.Application_Start(sender: obj, args: System.EventArgs) =
        ()

[<assembly: Website(typeof<Website>)>]
do ()
