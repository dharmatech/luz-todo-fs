namespace FSToDoList

open System.Collections.Generic
open Microsoft.AspNetCore.Mvc
open FSToDoList.DataContext
open FSToDoList.Models
open Microsoft.EntityFrameworkCore
open Microsoft.AspNetCore.Http
open System.Linq
open System.Threading.Tasks

[<ApiController>]
[<Route("api/ToDoItems")>]
type ToDoItemsController private () =

    inherit ControllerBase()

    [<DefaultValue>]
    val mutable _Context : ToDoContext

    new (context : ToDoContext) as this =
        ToDoItemsController () then
        this._Context <- context

    [<HttpGet>]
    member this.Get() =
        ActionResult<IEnumerable<ToDoItem>>(this._Context.ToDoItems)

    [<Route("search")>]
    [<HttpGet>]
    member this.Get([<FromBody>] _Values : ToDoItem[]) =
        if base.ModelState.IsValid then

            let items : List<ToDoItem> = new List<ToDoItem>()

            let obj = base.Ok(items)

            for value in _Values do
                if (value.Id = 0) then
                    items.AddRange(this._Context.ToDoItems.Where(fun elt -> elt.Name = value.Name).ToList())
                else if (this._Context.ToDoItemExist(value.Id)) then
                    items.Add(this._Context.ToDoItems.Find(value.Id))

            if (items.Count = 0) then
                ActionResult<IActionResult>(base.NotFound("NOT FOUND!, The search returned 0 values"))
            else
                ActionResult<IActionResult>(base.Ok(items))
        else
            ActionResult<IActionResult>(base.BadRequest(base.ModelState))

    [<HttpGet("{id}")>]
    member this.Get(id:int) =
        if base.ModelState.IsValid then
            if not (this._Context.ToDoItemExist(id)) then
                ActionResult<IActionResult>(base.NotFound("NOT FOUND!, There is no ToDoItem with this code: " + id.ToString()))
            else
                ActionResult<IActionResult>(base.Ok(this._Context.GetToDoItem(id)))
        else
            ActionResult<IActionResult>(base.BadRequest(base.ModelState))

    [<HttpPost>]
    member this.Post([<FromBody>] _ToDoItem : ToDoItem) =
        if (base.ModelState.IsValid) then 
            if not( isNull _ToDoItem.Name ) then
                if ( _ToDoItem.Id <> 0 ) then //check if the ID is set
                    ActionResult<IActionResult>(base.BadRequest("BAD REQUEST, the ToDoItemID is autoincremented")) // the ToDoItem is autoincremented
                else 
                        this._Context.ToDoItems.Add(_ToDoItem) |> ignore
                        this._Context.SaveChanges() |> ignore
                        ActionResult<IActionResult>(base.Ok(this._Context.ToDoItems.Last()))
            else
                ActionResult<IActionResult>(base.BadRequest("BAD REQUEST!, the field Initials can not be null"))                    
        else
            ActionResult<IActionResult>(base.BadRequest(base.ModelState))

    [<HttpPut("{id}")>]
     member this.Put( id:int, [<FromBody>] _ToDoItem : ToDoItem) =
        if (base.ModelState.IsValid) then 
            if not( isNull _ToDoItem.Name ) then
                if (_ToDoItem.Id <> id) then 
                    ActionResult<IActionResult>(base.BadRequest())
                else
                        try//error handler
                            this._Context.Entry(_ToDoItem).State = EntityState.Modified |> ignore
                            this._Context.SaveChanges() |> ignore
                            ActionResult<IActionResult>(base.Ok(_ToDoItem))
                        with ex ->
                            if not( this._Context.ToDoItemExist(id) ) then
                                ActionResult<IActionResult>(base.NotFound())
                            else 
                                ActionResult<IActionResult>(base.BadRequest())
            else
                ActionResult<IActionResult>(base.BadRequest())                                
        else    
            ActionResult<IActionResult>(base.BadRequest(base.ModelState))

    [<HttpDelete("{id}")>]
    member this.Delete(id:int) =
        if (base.ModelState.IsValid) then 
            if not( this._Context.ToDoItemExist(id) ) then 
                ActionResult<IActionResult>(base.NotFound())
            else (
                    this._Context.ToDoItems.Remove(this._Context.GetToDoItem(id)) |> ignore
                    this._Context.SaveChanges() |> ignore
                    ActionResult<IActionResult>(base.Ok(this._Context.ToDoItems.Last()))
            )
        else
            ActionResult<IActionResult>(base.BadRequest(base.ModelState))