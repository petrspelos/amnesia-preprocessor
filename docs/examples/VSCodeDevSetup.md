# Setting up VSCode for Amnesia Development

1) Download and install [Visual Studio Code](https://code.visualstudio.com/)

> Careful, VSCode is different from the big Visual Studio IDE!

2) Open your project's folder in VSCode

> Go to `File > Open Folder` and select the custom story's folder

## Required Extensions setup

3) In VSCode, go to the Extensions manager by pressing <kbd>Ctrl</kbd> + <kbd>Shift</kbd> + <kbd>X</kbd>

> Make sure your are inside VSCode while using this keyboard shortcut. Alternatively you could select the extensions tab from the left menu.

4) Search for and Install an extension called `Amnesia (HPL2) Engine Function Signatures Snippets` searching for `Amnesia` should be unique enough.

> After installation you will have to click a newly created `Reload` button
>
> ![reload](https://i.imgur.com/lWP6w7S.png)

4) Search for and Install an extension called `C/C++`

> Again, don't forget to hit the reload button.

## Required file associtaions

To associate source files with the C++ syntax, we need to setup file associations inside Visual Studio Code.

1. To open settings, first open the Command Palette by pressing <kbd>Ctrl</kbd> + <kbd>Shift</kbd> + <kbd>P</kbd>
2. Type `Open Settings` and select the option with `(JSON)` at the end.
3. If you do not see `FileAssociations` on the right side of your screen, you can add it by searching it in the left view, or manually adding the following section to your right side view:

```json
"files.associations": {
    "*.hps": "cpp",
    "*.ihps": "cpp",
    "*.shps": "cpp",
    "*.lang": "xml",
    "*.cfg": "xml"
}
```
> If your right side view already has some entries, make sure you add a comma after the last one before you add a new one
>
> ❌ In the following example, a comma is missing after `AnotherValueHere`.
>
>```
> {
>   "EntryHere": "ValueHere",
>   "EntryHere": "AnotherValueHere"
>   "files.associations": {
>     "*.hps": "cpp",
>     "*.ihps": "cpp",
>     "*.shps": "cpp",
>     "*.lang": "xml",
>     "*.cfg": "xml"
>   }
> }
>```

## Getting Intellisense to behave

In your `.shps` and `.ihps` you are most likely seeing red squigglies under many functions and types. That is, because the C++ intellisense does not recognize them as proper functions.

We can overcome this issue by including an Amnesia Header. One can be found [HERE](https://github.com/petrspelos/HPL2-VSCode-Syntax/blob/master/resources/AmnesiaHeader.cpp).

You should save this file along with your source code and name it `AmnesiaSignatures.cpp` this will tell the Preprocessor to ignore this include directive.

After that, you can `#include "AmnesiaSignatures.cpp` as the first line of all of your `.shps` and `.files` and enjoy the full C++ Intellisense.

## Congrats

You development environment is now ready for the Preprocessor use!

You can go [HERE](https://github.com/petrspelos/amnesia-preprocessor/blob/master/docs/examples/Installation.md#setting-up-amnesia-preprocessor) to learn about setting up and using the Preprocessor.
