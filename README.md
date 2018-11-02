# Amnesia: Preprocessor

![Badge](PreprocessorBadge.png)

* Include this badge on your project's page to support the preprocessor. ⭐

HTML embed:
```html
<a href="https://github.com/petrspelos/amnesia-preprocessor"><img src="https://github.com/petrspelos/amnesia-preprocessor/blob/master/PreprocessorBadge.png?raw=true"></a>
```

## About Preprocessor

Amnesia: Preprocessor is software that enables you to write your Amnesia scripts with modern programming language features. The main feature being an `include` directive to be able to use a single source file in multiple other scripts.

* **Will people playing my mod need the preprocessor?**

No, you will distribute the already processed `.hps` files and there is no need to include `.shps` and `.ihps` files inside your final release. (you should, however, keep a backup of these somewhere, should you want to edit the mod to fix a bug, for example.)

## Examples & Resources

* [Example custom story](https://github.com/petrspelos/amnesia-preprocessor/blob/master/docs/examples/CustomStory.md)
* [Installation & Usage guide](https://github.com/petrspelos/amnesia-preprocessor/blob/master/docs/examples/Installation.md)

## What changes?

Instead of writing typical `.hps` files, two different file standards are created.

* `.shps` (source HPS) files contain your Amnesia code and you are free to use any additional syntax this software brings. This file will be translated into `.hps` that Amnesia can read.

* `.ihps` (include HPS) follows the same format as `.shps` but doesn't turn into an `.hps`, instead, it can be included in multiple other `.shps` files for code reusability. See [custom story example](https://github.com/petrspelos/amnesia-preprocessor/blob/master/docs/examples/CustomStory.md).

### Here's an overview of the process
![Process Overview](https://i.imgur.com/S1lnugp.png)

## Features

### Include directive

Copy pasting the same code from one map to another can be a pain. Especially when you then make changes to that snippet and go looking for every place you pasted it to. This can lead to errors.

`include` directive fixes this issue, by allowing you to write your code into a single file and then use it from other files just by including it.

Example:

_1) Write your snippet in a `.ihps` file._

`MyInclude.ihps`
```cpp
void FunctionIAmUsingOften()
{
    // ... Code here ...
}
```
_2) Add an include directive to your `.shps` files._
```cpp
#include "MyInclude.ihps"

void OnStart()
{
    FunctionIAmUsingOften();
}
```

When you need to make changes, you only need to edit the `.ihps` file. Every `.shps` file that includes it will get the updated version.

* **Can my `.ihps` also include other `.ihps`?**

Yes, when an `.ihps` file includes another `.ihps`, this is called **transitive include**. And instead of copying the contents, an include is forced upon the parent `.shps` file.

> If `A.ihps` depends on `B.ihps`, then `X.shps` that includes `A.ihps` must also include `B.ihps`.

If this confuses you, take a look at the following visualization:

![visualization](https://i.imgur.com/XXnKYek.png)

> * If the red `include` is missing, the preprocessing will fail due to the transitive `include`.
>
> * The preprocessor will output an error like this:
> ![Transitive dependency error](https://i.imgur.com/U9NpyuI.png)

* **Can I use OnStart, OnEnter, and OnLeave in my `.ihps` files?**

Yes, you can read more about it in the `Amnesia Events inside .ihps files` section of the [custom story example](https://github.com/petrspelos/amnesia-preprocessor/blob/master/docs/examples/CustomStory.md).

## Preload Generation

The preprocessor automatically generates `PreloadSound` and `PreloadParticleSystem` calls. For your `.shps` files if the usage is explicit.

* Explicit usage uses the full sound/particle name string with the `.ps` or `.snt` extension.
```cpp
void OnStart()
{
    // Explicit usage:
    PlaySound("mySound.snt");

    // Non-explicit usage:
    PlaySound("coolSound" + number + ".snt");
}
```
* Non-explicit cases are not supported, because that would require full code analysis, which is beyond the project's scope. My goal is to keep the preprocessor quick and lean.

## Code minification

Due to the creation of new file types, it can be easy to accidentally edit a generated `.hps` file and then wonder why your changes are not reflected.

That's one of the reasons why the preprocessor support code minification.

Code minification is a process that makes code harder to read for humans, but in a sense easier for a machine. It achieves it by removing newlines and whitespaces from the code.

Preprocessor's minification process also removes the comments.

* Before minification:
```cpp
void OnStart()
{
    // This is a function call
    MyFunction();
}

// This is my function
void MyFunction()
{
    if(condition)
    {
        OtherCall();
    }
}
```

* Minified code:
```cpp
void OnStart(){MyFunction();}void MyFunction(){if(condition){OtherCall();}}
```
> * While hard to read for a human, computers can usually process this type of a file faster. The file size also decreases and you can be sure you won't accidentally edit this file.

### Debugging minified code

Sometimes Amnesia crashes due to a syntax error in your code.

The preprocessor unfortunatelly cannot protect you from this type of an error.

If an `.hps` file contains minified code, the error message can be very hard to process. It can tell you an error was found on line 1, column 15655.

For this, the preprocessor has an optional `--no-min` flag that will skip the minification process and will allow you to go through your processed `.hps` file to debug your code.

You can read more about flags in the Setting up section of this document.

### String interpolation

Programming languages such as [C#](https://en.wikipedia.org/wiki/C_Sharp_(programming_language)) have a feature called `string interpolation` that makes it easier to compose strings containing variables.

This syntax is directly copied from the C# standard.

If you put a `$` in front of your string literal, you can include variables and pieces of code just by wrapping them in a body `{}`.

⚠ There currently is no way of actually typing `{` or `}` inside an interpolated string, since it will get processed as a variable.

* Without string interpolation:
```cpp
string entityName = "MyEntity";
for(int i = 0; i < 20; i++)
{
    SetEntityActive(entityName + "_" + i, true);
    
    if(i > 10)
    {
        entityName = "MyOtherEntity";
    }
}
```

* With string interpolation
```cpp
string entityName = "MyEntity";
for(int i = 0; i < 20; i++)
{
    SetEntityActive($"{entityName}_{i}", true);
    
    if(i > 10)
    {
        entityName = "MyOtherEntity";
    }
}
```
> * Notice the easier to read syntax inside the SetEntityActive call.

## Setting up the preprocessor

The Preprocessor is written in C# using [.NET Core](https://www.microsoft.com/net).

Because of this, it is cross-platform.

To get the application installed on your machine with the recommended settings, see the [installation guide](https://github.com/petrspelos/amnesia-preprocessor/blob/master/docs/examples/Installation.md).
