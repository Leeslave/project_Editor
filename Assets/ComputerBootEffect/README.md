# Computer Boot Screen Effect
## Introduction
This package contain scripts to simulate boot screen of a computer. Folder *Scenes* contains a example scene.

## How to use
1. Create Gameobject → UI → Text in your scene, and clear text area.
2. Change font to *Perfect DOS VGA 437* if you want to make it more like DOS.
3. Add *Pre Amin* script to the text object, if you want to let this text object show after a few seconds. Set value of *WaitSecond*, and drag *Text (Script)* compotent to *GUI Text Ctrl*.
4. Add *Anim Text* script to the text object, and drag *Text (Script)* compotent to *GUI Text Ctrl*.
    - *Default String Time* : Default delay time between characters in string. For example, if it is set to 0.01, then it will display a string "ABC" like this: "A" → 0.01 second → "B" → 0.01 second → "C". Set to 0 will display a string instantly.
    - *Default Text Time* : Default delay time between strings in text. For example, if it is set to 0.01, and *Default String Time*  is set to 0, then it will display a text containing "ABC" and "DEF" like this: "ABC" → 0.01 second → "DEF".
    - *Ta* : Text asset for display, check script section for usage.
    - *Ti* :  Value parse from *Ta*, use for debugging.
5. Add *Anim Order Controller* to the text object, and set *Anims* size to 2 (or more if you have more anim script on the object). Drag *Pre Anim* and *Anim Text* script to *Element 0* and *Element 1*. The Anims will display in that order.

## Text asset config
Check out *BIOSInfo.json* in the folder *Text* for basic understand of the structure.
Here is more information about values.

- *ass* : Anim Strings, should contain all *Anim String* objects which want to display.

*Anim String*  has four properties.

1. *info* : String to display. Add *"\n"* if you want to start in a new line.
2. *infoType* : How string is displayed. When it is set to 1, it will display a int number in a different way. It only support int numbers.
3. *textTime* : Same functionality as *Default Text Time* . Set to -1 if you want to use the value of *Default Text Time* , or set to a positive number to specify the text time of this info.
4. *stringTime* : Same functionality as *Default String Time* . Set to -1 if you want to use the value of *Default String Time* , or set to a positive number to specify the string time of this info. When *infoType* is set to 1, this value controls the total display time of the number effect.

## Notice
Because fonts have license issues, so I remove other fonts and replace them with Ubuntu's fonts. If you want to make it more real, you can use **ENDOS** in the *Canvas → Head* object and **Perfect DOS VGA 437** in the *Canvas → info* object.
