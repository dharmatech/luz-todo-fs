namespace FSToDoList

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.EntityFrameworkCore
open FSToDoList.DataContext

type Startup(configuration: IConfiguration) =

    member _.Configuration = configuration
    
    member _.ConfigureServices(services: IServiceCollection) =
        services.AddControllers() |> ignore
        services.AddDbContext<ToDoContext>(fun options -> options.UseInMemoryDatabase("DB_ToDo") |> ignore) |> ignore
    
    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        
        app
            // .UseHttpsRedirection()
            .UseRouting() 
            // .UseAuthorization()
            .UseEndpoints(fun endpoints -> endpoints.MapControllers() |> ignore)
            |> ignore