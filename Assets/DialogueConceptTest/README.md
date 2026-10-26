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
Text is layered separately in Unity. These are proposal assets rather than changes to the live game UI.
