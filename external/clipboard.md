### Windows 11 Console Copy Quirk Explained

#### Observed Behavior

1. Select text in __Windows Terminal__, __CMD__, or __PowerShell__ using the mouse.
2. Right-click to copy:
   - __If the mouse pointer is moved far outside the selection rectangle__, the selection is successfully copied.
   - __If the mouse pointer stays near the original selection__, nothing is copied.

#### Why This Happens

- The Windows console has a long lineage dating back to __Windows 95__ (Mark Mode / QuickEdit).  
- Selection is __not immediately committed__ to the clipboard; it depends on user input to signal the end of selection.
- __Pointer location relative to selection__ is interpreted as intent:
  - __Right-click inside the selection__ → may be treated as “open context menu” rather than “copy.”  
  - __Right-click outside the selection__ → treated as “commit selection and copy” → succeeds.

- Windows 11 continues this design, but with additional heuristics:
  - The console wants to avoid accidental copy when you right-click very close to the selection origin.
  - Moving the mouse outside the selection rectangle signals “intentional action,” which triggers copy.

#### Workarounds / Reliable Methods

1. __Keyboard-first approach__  
   - After selecting text, press __Ctrl+C__ → always copies, pointer location irrelevant.

2. __Right-click outside selection__  
   - If using mouse-only, move the pointer slightly outside the selected area before right-clicking.

3. __Windows Terminal settings__  
   - Enable __Copy on selection__ in __Settings → Interaction__.  
   - Right-click is no longer needed to copy.

4. __Old CMD alternative__  
   - Use __Mark Mode__: `Alt+Space → Edit → Mark`, select text, then __Enter__ to copy.  
   - Right-click behavior is avoided entirely.

#### Historical Note

- This “copy on selection + right-click commit” behavior is not new.  
- According to classic Windows console design (Windows 95 / QuickEdit), the __clipboard commit relies on mouse intent heuristics__, which is why the pointer’s position still matters today.  
- It’s very likely __Raymond Chen__ would trace the “true origin” of this quirk to __Windows 95__, not Windows 11.

---

__Summary:__  
Windows 11’s console is “smart” about right-click copy, but its intelligence is just heuristics inherited from decades-old QuickEdit logic. Moving the pointer away ensures the console interprets your right-click as a deliberate copy action.
