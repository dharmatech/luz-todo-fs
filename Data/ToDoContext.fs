namespace FSToDoList

open FSToDoList.Models
open Microsoft.EntityFrameworkCore
open System.Linq

module DataContext =

    type ToDoContext(options : DbContextOptions<ToDoContext>) =
        
        inherit DbContext(options)

        [<DefaultValue>]
        val mutable ToDoItems : DbSet<ToDoItem>
        member public this._ToDoItems   with    get()       = this.ToDoItems
                                        and     set value   = this.ToDoItems <- value

        member this.ToDoItemExist (id:int) = this.ToDoItems.Any(fun x -> x.Id = id)

        member this.GetToDoItem (id:int) = this.ToDoItems.Find(id)

    let Initialize (context : ToDoContext) =
        context.Database.EnsureCreated() |> ignore

        let items : ToDoItem[] =
            [|
                { Id = 0; Name = "Smalltalk-80"; IsComplete = true }
                { Id = 0; Name = "Emacs";        IsComplete = true }
                { Id = 0; Name = "Linux";        IsComplete = false }
            |]

        if not (context.ToDoItems.Any()) then
            context.ToDoItems.AddRange(items) |> ignore
            context.SaveChanges() |> ignore