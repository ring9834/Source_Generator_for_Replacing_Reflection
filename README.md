# Source Generator for Clean Code
This example demonstrates one of the classic patterns where source generators serve as a compile-time alternative to runtime reflection — even though the specific example itself is deliberately simple.

## No runtime reflection at all for this feature
1.The generator sees classes with the custom attribute (in this case, HiFromGenerator).

2.It generates the exact method you need (in this case, HiFromGeneratedCode()).

3.You get the method for free at compile-time, without writing it yourself or using reflection at runtime direct, fast method call (no reflection, no type lookup, no Invoke).

## When to Use a Source Generator
The same boilerplate / pattern repeats ≥10–20 times

We expect the implementation to change/improve in the future  

We want to enforce a convention via an attribute

We want consistency across a large codebase or library

We're building something reusable (NuGet package, company framework)

Part of code (consuming the generated code)
```sh
using HiGenerator;  // gets the attribute automatically

namespace Method_Consumer
{
    [HiFromGenerator]  // this triggers generation
    partial class Person
    {
        public string Name { get; set; } = "Alice";
    }
}
```
