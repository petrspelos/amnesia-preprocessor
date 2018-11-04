# Setting up Amnesia: Preprocessor

Amnesia: Preprocessor is a collection of `.dll` files with an `.exe` console application capable of processing your project's files.

1) Getting a copy

To get a copy of the preprocessor you can either clone/download the repository and compile it yourself, or use one of the [already built releases](https://github.com/petrspelos/amnesia-preprocessor/releases).

For this guide, we'll assume you downloaded a pre-built release.

In the archive file, you should fine a folder called `AmnesiaPreprocessor` this folder contains all the necessary files for the preprocessor to work correctly.

It also includes the `AmnesiaPreprocessor.exe` file.

> * Note that if you're building the project yourself, the executable will most likely be called `AmnesiaPreprocessor.ConsoleApp.exe`, there is no harm in renaming just the `.exe` file.

2) Setup your tools directory

Because we will refer to the preprocessor's path later in this setup guide, it is recommended that you pick a path that has **no spaces in its name** and is fairly short.

We recommend a path like `C:/tools`. You can create a directory directly in your `C:` (or any other) drive to store tools like this.

> * You don't have to have a new copy of the preprocessor for each custom story. This is why we're creating a standard path to the preprocessor so that we can refer to it later.

3) Copy the preprocessor into the target directory

If your tools director is `C:/tools`, the path to the preprocessor should be
```
C:/tools/AmnesiaPreprocessor/AmnesiaPreprocessor.exe
```
If your path is different, note down your path to the preprocessor.

> ✨ _Optional advanced suggestion_
>
> You can add the preprocessor path to the PATH variable of your Command Line to remove the need for the full preprocessor path.

# Using the Preprocessor

To use the preprocessor, make sure you have completed all the steps in the `Setting up Amnesia: Preprocessor` section of this document.

There are two main ways of using the preprocessor:
* Through an IDE like [Visual Studio Code](https://code.visualstudio.com/)
* Through the Command Line

The first option is objectively better in the long run, and is more beginner friendly.

The second option is for people who know what they're doing or are familiar with other CLI-based compilers such as `gcc` or `csm`.

## Using the Preprocessor in Visual Studio Code

Before you start using the preprocessor make sure you [setup VSCode for Amnesia's Development](https://github.com/petrspelos/amnesia-preprocessor/blob/master/docs/examples/VSCodeDevSetup.md)

1) Open your project folder in Visual Studio Code.

You should see your project files in the tree view.
![tree view](https://i.imgur.com/55IGhJo.png)

2) Create a Tasks file

We need to tell Visual Studio that we're going to use the Preprocessor to build our custom story.

To do that, we use a `tasks.json` file. We can have VSCode generate this file for us.

Press <kbd>Ctrl</kbd> + <kbd>Shift</kbd> + <kbd>P</kbd>

This will open the `Command Palette` in it, type `Build` and select the following option:

```
Tasks: Configure Default Build Task
```

Selecting this option should give you an option to `Create tasks.json file from a template`.

> If this doesn't happen, it is possible you already have a `tasks.json` file in a `.vscode` directory and you should use that one and skip to the next step.

In the next prompt, select `Others` to be able to execute a custom command on build.

This should generate a new `tasks.json` file from a template. This file should be created inside a `.vscode` folder in the root of your project.

Here's a gif showing this step:
![example tasks](https://thumbs.gfycat.com/PepperyParallelEkaltadeta-size_restricted.gif)

3) Setup a build task

* Open the newly created `tasks.json` inside the `.vscode` folder.

* If you don't have any other task in your project, change your template to look like the following:

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "Preprocess",
            "type": "shell",
            "command": "[PATH-TO-YOUR-PREPROCESSOR] '-cs:${workspaceFolder}'",
            "group": {
                "kind": "build",
                "isDefault": true
            }
        }
    ]
}
```

Make sure you change `[PATH-TO-YOUR-PREPROCESSOR]` to the actual path to your preprocessor.

If you followed this guide it should look like this:

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "Preprocess",
            "type": "shell",
            "command": "C:\\tools\\AmnesiaPreprocessor\\AmnesiaPreprocessor.exe '-cs:${workspaceFolder}'",
            "group": {
                "kind": "build",
                "isDefault": true
            }
        }
    ]
}
```
⚠ Do not change `{workspaceFolder}` to a path, this is a VSCode variable and it will use your custom story's folder based on the project you opened.

4) Try building your project

If you have some `.shps` and `.ihps` files inside your project, try running the preprocessor by pressing

<kbd>Ctrl</kbd> + <kbd>Shift</kbd> + <kbd>B</kbd>

Inside VSCode.

This should be the result:
![result](https://thumbs.gfycat.com/HonestCarefulHousefly-size_restricted.gif)

This will generate your `.hps` files into your `maps` folder.

⚠ Don't forget to build your project after each change before you try it in game. Your changes are not reflected until you rebuild.

## Preprocess on Save

To make the preprocessor process your files every time you save, you will need an extension called `Trigger Task on Save`, you can find it again in the extensions manager in VSCode.

> Don't forget to hit the reload button after installing the extension.

1) Let's create a new Preprocessing task that will no pop-up the console window.

You can add as many tasks to your `tasks.json` as you want, in this example, we have two tasks `Preprocess` and `Preprocess-Silent` the difference is that the Silent variation does not open an integrated terminal, which is good for triggering on save.

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "Preprocess",
            "type": "shell",
            "command": "C:\\tools\\AmnesiaPreprocessor\\AmnesiaPreprocessor.exe '-cs:${workspaceFolder}'",
            "group": {
                "kind": "build",
                "isDefault": true
            }
        },
        {
            "label": "Preprocess-Silent",
            "type": "shell",
            "command": "C:\\tools\\AmnesiaPreprocessor\\AmnesiaPreprocessor.exe '-cs:${workspaceFolder}'",
            "presentation": {
                "reveal": "never",
            }
        }
    ]
}
```

> Don't forget to change your Preprocessor path if it is different for your use-case!

2) In the same folder as your `tasks.json` create (or open if it already exists) a `settings.json` file.

Inside it include the following snippet:
```json
"triggerTaskOnSave.tasks": {
    "Preprocess-Silent": [
        "**/*.shps",
        "**/*.ihps"
    ]
}
```

The `Preprocess-Silent` refers to the name of the task defined inside `tasks.json`

If you are creating this file for the first time, make sure you place the snippet in between `{` and `}`.

For a new file, your setting should look like this:
```json
{
    "triggerTaskOnSave.tasks": {
        "Preprocess-Silent": [
            "**/*.shps",
            "**/*.ihps"
        ]
    }
}
```

If `settings.json` aready existed for you, there should be other values before the `triggerTaskOnSave` section.

For example:

```json
{
    "git.ignoreLimitWarning": true,
    "triggerTaskOnSave.tasks": {
        "Preprocess-Silent": [
            "**/*.shps",
            "**/*.ihps"
        ]
    }
}
```

And there you have it, now whenever you save any `.ihps` or `.shps` file the preprocessing task will run. :tada:
