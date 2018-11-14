# ASP .NET Core Unit Of Work

A library for Microsoft.EntityFrameworkCore to support generic repository, unit of work patterns, and multiple database controls with distributed transaction supported.

## Getting Started

This is simple to integrated the library into you asp.net core project. You may used mssql, mysql, postgresql with Unit Of Work through [EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/2.1.4) library. 

### Installing

First install the library from [Nuget](https://www.nuget.org/packages/AspNet.Core.UnitOfWork/).

For Package Manager

```
Install-Package AspNet.Core.UnitOfWork -Version 2.0.0
```

For .Net CLI

```
dotnet add package AspNet.Core.UnitOfWork --version 2.0.0
```

## Quickly start

### How to use Unit Of Work

Firs Step to Add Unit Of Work into service collection in startup file

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // use in memory for testing.
    services
        .AddDbContext<TestContext>(opt => opt.UseInMemoryDatabase())
        .AddUnitOfWork<TestContext>();
}
```

You can Add Multiple Db Context Into UnitOfWork
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // use in memory for testing.
    services
        .AddDbContext<TestContext1>(opt => opt.UseInMemoryDatabase())
        .AddDbContext<TestContext2>(opt => opt.UseInMemoryDatabase())
        .AddUnitOfWork<TestContext1,TestContext2>();
}
```

Second Step, Inject the Unit Of Work into your controllers and performing CRUD operation using the UnitOfWork

```csharp
private readonly IUnitOfWork _unitOfWork;

// 1. IRepositoryFactory used for readonly scenario;
// 2. IUnitOfWork used for read/write scenario;
// 3. IUnitOfWork<TContext> used for multiple databases scenario;
public ValuesController(IUnitOfWork unitOfWork)
{
    _unitOfWork = unitOfWork;
}

public async Task<IActionResult> Get(string userId)
{
    User user = await GetUnitOfWork().FindAsync(p => p.userId == userId);

    return user ?? BadRequest() : user;
}

private IGenericRepository<User> GetUnitOfWork()
{
    return _unitOfWork.Repository<User>();
}

public async Task<IActionResult> Post(User user)
{
    User user = await GetUnitOfWork().SaveAsync(user);
    await GetUnitOfWork.SaveAsync();
    return user ?? BadRequest() : user;
}
```

### How to use Raw Sql in UnitOfWork

If you want to execute raw sql command, so you do that using this library

```csharp
public async Task<ActionResult> Get()
{
    try{

        string query = "Name == 'Nahid Hasan'";

        SqlParameter name = new SqlParameter("@name","Nahid Hasan");

        var result = await _unitOfWork.ExecFilterAsync<TestClass,Test>(query, p => new Test(){Name = p.Name}, name);

        return new JsonResult(result);

    }catch(Exception ex)
    {
        return new JsonResult(ex.Message) {StatusCode = 400};
    }
}
```

## Acknowledgments
Their project actually inspired me to do some awesome library for Unit Of Work. I specially thanks to Md. Masud.
* [Arch](https://github.com/Arch)
* [Md. Masud](https://github.com/jamdmasud)

