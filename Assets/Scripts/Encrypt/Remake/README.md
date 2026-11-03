# ADFGVX_TEST

## Integrated work test

Open `Assets/Scenes/Minigame/WorkIntegration_TEST.unity` alone to try Normal/Confidential
document and cipher work. Confidential work first runs a short Maze authentication.
The host uses this existing ADFGVX_TEST scene, locks it to the selected decrypt stage,
and records successful submission in memory. Use its work-list/abort button or F10 to return.
The integration menu enables the existing tutorial automatically by default; its tutorial toggle
can disable it before launch. In-scene tutorial controls still retain the current answers.
Opening ADFGVX_TEST by itself retains the standalone behavior described below.
See `docs/WorkIntegrationTest.md` for the complete test flow and validation limits.

Open `Assets/Scenes/Minigame/ADFGVX_TEST.unity` and press Play. This scene runs by itself;
it does not need MainController or DirectPlayController. Its uGUI is generated at runtime.
The green terminal panels retain the original minigame's visual direction.
The test scene uses the project's existing NeoDunggeunmo SDF font throughout the UI.
Tutorial explanations and buttons use polite Korean; symbols are limited to glyphs present in its atlas.

## Data

The controller reads the existing `Resources/GameData/Encrypt/ADFGVXStageData.json`
and `Resources/GameData/Encrypt/Tables/Table_N.txt` through `Resources.Load` (also works in a build).
There is no duplicate task JSON, hardcoded replacement answer, or write to Resources.
Both intermediate output and final answers are checked against the actual cipher rules on load.

- Decrypt: stages 6 and 12. Stage 6 is selected when the scene opens.
- Encrypt: stages 5, 8 and 9.
- Header arrows cycle through tasks in the selected mode.
- Same letters in a key keep their left-to-right order.

## Play

The focused tutorial starts automatically for each new task. It guides actual actions all the
way to submission for encryption and decryption, including all remaining letters and columns.
Only the current control remains clickable. Explanations have an explicit continue button;
gameplay steps advance only when the player performs the required action.
The first decrypt character teaches the left row and top column separately, then highlights
their intersection. The first successful character is followed by a result-slot explanation.
Key typos keep the learner on the key step. Table navigation points toward the required table.
Guidance is computed from actual puzzle state, so undo or resuming a partial task cannot leave
the learner waiting for an already completed operation.

Use the card's `혼자 해보기` button to leave guidance without losing answers. The header's
`튜토리얼 켜기` button restarts explanations from the current puzzle state without resetting it.
An inspector `Auto Start Tutorial` setting controls automatic startup.
The spotlight follows the shared tutorial's visual interaction using a remake-specific Overlay
adapter: it uses real uGUI controls and reacquires the focus target whenever the screen rebuilds.
It does not change the shared TutorialController/Border or other games' tutorials.

Section numbers match the header steps. Section 01 contains document loading and key entry;
the transposition/substitution numbers swap with the mode; section 04 contains results and submission.
The current section has a cyan border, tinted background and a current-step badge.
The former work-guide panel is removed; the highlight toggle is inside the expanded substitution panel.
Panel and control borders are four inset 2px edges rather than an outside Outline effect,
with pixel-perfect Canvas rendering to keep their edges visible at scaled resolutions.

1. Open the document. The assignment lists its key and table number.
2. Enter and apply the key. Lowercase and surrounding whitespace are accepted.
3. Decryption: select the next ciphertext chunk, then click the appropriate ranked column.
   Once all columns are filled, select a pair and click its decoded character in the table.
4. Encryption: select a plaintext character, then click it in the table to record its pair.
   After substitution, click columns in rank order to assemble the final ciphertext.
5. Click any result slot to replace it. Other slots stay intact. A changed encryption
   substitution invalidates its dependent transposition; undo restores both together.
6. Submit when both operations are complete. Validation is immediate and does not block input.

`Ctrl+Z`: undo; `Ctrl+Y`: redo; left/right arrows: select a result slot.
These shortcuts are suppressed while editing the key. The key field supports normal text editing.
Hint highlighting can be switched off. Reset asks for confirmation and affects only the current task.

Mode and task switches retain separate progress **for the current Play session only**.
In standalone mode this test scene does not call ClearWork, change the story's save files, or replace
the production ADFGVX scenes. Completed tasks remain visible for review.
No file or key input triggers the old blocking console animations.

## Validation

From the project root, run `./Tools/AdfgvxValidation/Validate.ps1` using .NET SDK 9 and the
installed Unity 2021.3.45f2 libraries. `-UnityEditor` can override the Editor directory.
The script compiles the actual model **and Unity UI controller** against the installed Unity/TMP/uGUI
assemblies, then runs all five existing tasks, incorrect-input recovery, state dependencies,
undo/redo, repeated-key ordering, and unequal-column cipher round trips without requiring a Unity license.
It also completes all five tasks by following only the tutorial's suggested actions, checks key-error
recovery and resumed progress, and verifies 54 spotlight/explanation positions stay on screen without
covering the target. These are logic/geometry checks, not a substitute for Unity pointer-input QA.

The scene still needs a Unity Play-mode visual/input check. The local Unity Editor could not
start during this session because its license was unavailable.
