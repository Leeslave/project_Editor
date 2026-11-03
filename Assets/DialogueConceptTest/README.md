# Comic dialogue UI preview

Open `Test.unity` directly, or use **Tools > Test Setup > Open Comic Dialogue Test**, then Play.

This scene is independent of GameSystem. Production scenes and build settings are unchanged.
The camera uses a plain charcoal clear color; no location artwork is included.

- Art/: transparent PNG sprites, split from the generated atlas.
- Prefabs/: separate dialogue panel, three choices, six function buttons, and complete preview canvas.
- Dialogue and captions are editable Unity UI Text objects using the existing Pretendard font.
- Reference canvas: 1920 x 1080, scaled to the Game view.
- Function controls: 2 columns x 3 rows.
- Preview actions: reset dialogue, show/hide choices, show/hide history, skip to last page, previous, next.
- Choices 1 and 2 show sample replies; choice 3 demonstrates the disabled state.

`Preview.png` is an actual Unity camera render. Isolated batch validation checks serialized references,
choice responses, six controls, the locked choice, and rendering.

Art generated with the built-in ImageGen tool, using the approved comic-cut mockup as reference.
Prompt direction: extract UI only onto true transparent alpha, no backdrop or text; a wide angular
white dialogue panel with black outline and speech pointer, a white parallelogram choice button,
a black speaker plate, and six matching ink icons (talk, branch, history, skip, back, continue).
Text is layered separately in Unity. The original Test scene remains independent.

## In-game use

The Story branch's production `Assets/Prefabs/ChatSystem/ChatCanvas.prefab` now uses these sprites
through `ComicDialogueSkin`. It styles the existing Chat hierarchy at runtime and retains story
choice listeners, history and skip confirmation. The six production controls are dialogue visibility,
choice focus, history, skip, earlier history and continue. Earlier history never rewinds story actions.
Choice paragraphs keep the last dialogue visible and disable skip/continue until a choice is selected.

Portraits now use a 1120px reference-height close-up layout, retaining authored left/right facing.
Their size is unchanged; top positions are 335px (left slots) and 340px (right slots) so lanyard
ID cards sit near the speaker name tab and the dialogue panel crops the lower torso.
Klayton and Clover now prefer `Resources/Chat/Standing` bust sheets copied from `output/standing`.
Each 1254px sheet is sliced into 418px cells in row-major order (8 Klayton, 9 Clover expressions).
Original story filenames and expression indices are retained. Square busts use a 430px display
rectangle, adjusted to match the existing face size and ID-card height, facing into the conversation.
Other characters and unavailable expression indices retain their original portrait assets.
Affiliation appears inside the white dialogue panel on its own 27px row, above dialogue text.
The speaker name and bubble tail switch sides using the displayed character slots and speaker name;
a single highlighted character or single visible character supplies a fallback for other names.
Choice paragraphs keep the preceding speaker direction.

`Art/DialoguePanelRight.png` was created with built-in ImageGen using the left bubble as the edit
reference. Prompt: matching empty monochrome angular dialogue panel with the tail at the upper right,
preserving the white body and black ink outline, transparent background, no text or extra objects.
The right tail now leans up-right, with its tip to the right of its base. Its name tab is offset left
of the tail so the speaker label cannot obscure the point.
