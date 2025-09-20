## ExpressionEngine (.NET)

DDD-style console app with Lexer, Parser, Evaluator services and a function registry. Variables are passed as `Dictionary<string,string>` and consumed via `VAR(name)`.

### Structure

```
dotnet/ExpressionEngine/
  Application/
    ExpressionService.cs
    IEvaluator.cs
    IExpressionService.cs
    ILexer.cs
    IParser.cs
  Domain/
    Ast.cs
    Tokens.cs
  Infrastructure/
    Evaluator.cs
    FunctionRegistry.cs
    Lexer.cs
    Parser.cs
  Program.cs
  ExpressionEngine.csproj
```

### Build & Run

```
cd dotnet/ExpressionEngine
dotnet build
dotnet run
```

### Examples

- `[ADD(1,2)]` => 3
- `[CONCAT("Hello", " ", "World")]` => "Hello World"
- `[IF([EQUALS(1,2)], "True", "False")]` => "False"
- `[ADD([VAR("a")],[VAR("b")])]` => with variables a=1, b=2 => 3
- `[CONCAT([VAR("greeting")], " ", "World")]` => with greeting=Hello => "Hello World"


