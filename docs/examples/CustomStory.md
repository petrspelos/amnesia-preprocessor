# Custom Story Example
## Using Amnesia: Preprocessor for a custom story

This demonstrates a suggested structure of a custom story for optimal use of the Preprocessor.

Here's our example custom story's structure:
```
📁exampleStory
    ├ 📄custom_story_settings.cfg
    ├ 📄extra_english.lang
    └ 📁maps
        ├ 📄00_Intro.map
        ├ 📄01_House.map
        └ 📁source
            ├ 📄00_Intro.shps
            ├ 📄01_House.shps
            └ 📁includes
                ├ 📄MyCommonMethods.ihps
                └ 📄MyOtherInclude.ihps
```

>ℹ **What's important to notice**
>
> * There are no `.hps` files inside the `maps` directory. That's because Amnesia Preprocessor will output these files and we won't be directly editing them.
>
>* We created a `source` directory for our `.shps` and inside it a `includes` directory for `.ihps`. This makes the separation clear and it makes it easy to find the files we're looking for.
>
>* This custom story will not be playable, until we run Amnesia Preprocessor on it to generate the `.hps` files.

## File contents

Let's open up a few of these files...

`00_Intro.shps`
```cpp
#include "MyCommonMethods.ihps"

void OnStart()
{
    SetInDarknessEffectsActive(false);
    RegisterPlayerCallback("SoundArea", "PlayDoorSound");
}

void PlayDoorSound(string a, string b, int s)
{
    PlaySoundAtEntity("door_creak.snt");
}
```

> ℹ **What's important to notice**
> 
> * There is an `include` directive referencing `MyCommonMethods.ihps` this means we can use anything that is defined inside that file. For example `RegisterPlayerCallback` and `PlaySoundAtEntity` functions. (The play sound function is an [overload](https://en.wikibooks.org/wiki/Computer_Programming/Function_overloading))
>
> * The `door_creak.snt` file is not preloaded. Because this is an explicit use, the preprocessor automatically generates a `PreloadSound` function call in the `.hps`.

Let's take a look into the included file...

`MyCommonMethods.ihps`
```cpp
void RegisterPlayerCallback(string area, string callbackFunction)
{
    AddEntityCollideCallback(
        "Player",         /* Collision Object */
        area,             /* Collision Subject */
        callbackFunction, /* Callback Function */
        true,             /* Delete after Collision */
        0                 /* Collision State */
    );
}

void PlaySoundAtEntity(string sntSound)
{
    PlaySoundAtEntity(
        "",       /* Sound name */
        sntSound, /* Sound file */ 
        "Player", /* Sound source entity */
        0.0f,     /* Fade-in seconds */
        false     /* Save sound with the game */
    );
}
```

> ℹ **What's important to notice**
>
> * These smaller and neater functions wrap the old large and bulky Amnesia API ones. This is a good strategy to make your code much easier to read.
>
> * These functions will be copied into all `.hps` files that come from a `.shps` that includes this file. Meaning if we make a change to this file, it will reflect at all places where it is included and used. This is the most powerful thing about the preprocessor. ⚡

## Transitive dependency

Sometimes your `.ihps` include files may want to use functions defined in other `.ihps` files. This can be done by using the `include` directive inside the `.ihps` file itself.

To prevent circular dependency, where `A.ihps` depends on `B.ihps` which depends back on `A.ihps`, these are not actually copied. Instead, the dependency is foced on the `.shps` file that includes this dependency.

If a required `include` directive is missing, the preprocessing will fail and you will be alerted that you are indeed missing an `include` directive.

![Transitive dependency error](https://i.imgur.com/U9NpyuI.png)

## Amnesia Events inside `.ihps` files

Amnesia has a couple of functions it calls during some of the common game events. The most common ones are `OnEnter`, `OnLeave`, and `OnStart`.

These can be defined inside `.ihps` files and will be merged with any implementation that may already be defined in the parent `.shps` file.

**Example:**

`A.shps`
```cpp
#include "MyLibraryOfMethods.ihps"
#include "ForceLantern.ihps"

void OnEnter()
{
    DoSomething();
    DoSomethingElse();
}
```

`ForceLantern.ihps`
```cpp
#include "MyLibraryOfMethods.ihps"

void OnEnter()
{
    RunLanternCheck();
}

void RunLanternCheck()
{
    if(!HasItem("Lantern"))
    {
        GiveItemFromFile("lantern.ent", "Lantern");
    }
}

void OnStart()
{
    DoSomeLogic();
}
```

> ℹ **What's important to notice**
>
> * Since `OnEnter` already exists in the parent `.shps` file, these two methods will be merged.
>
> * Since `OnStart` does not exist in the parent `.shps` file, one will be created.
>
> * The logic is delegated into a `RunLanternCheck` function instead of having logic directly in the `OnEnter`. If this is not done, the preprocessor cannot resolve multiple nested bodies `{ }` and while the preprocessing will not fail, the resulting `.hps` file will most likely crash.

## Final remarks

And there you have it. Now you should be able to structure your custom story to use the Preprocessor to the fullest.

Make sure you go back to `README.md` and see what else you can do with the preprocessor as well as how to set it up on your machine.
