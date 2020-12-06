namespace FSToDoList

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open FSToDoList.DataContext

module Program =
    let exitCode = 0

    let CreateHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(fun webBuilder ->
                webBuilder.UseStartup<Startup>() |> ignore
            )

    [<EntryPoint>]
    let main args =
        let host = CreateHostBuilder(args).Build()
        use scope = host.Services.CreateScope()
        let services = scope.ServiceProvider
        let context = services.GetRequiredService<ToDoContext>()

        Initialize(context) |> ignore
        
        host.Run()

        exitCode