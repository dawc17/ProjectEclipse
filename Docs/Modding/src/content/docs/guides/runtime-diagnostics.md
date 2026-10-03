---
title: Debug callbacks in game
description: Find failing or expensive Lua callbacks with the performance overlay and its saved report.
---

Use the game's callback diagnostics when a mod loads successfully but a rule,
framework service, AI decision or button behaves unexpectedly. You can inspect
the running mod session without editing recovered C# or attaching a debugger.
No manifest capability or extra Lua registration is needed.

## Capture a reproduction

1. Turn on the **Performance overlay** in **Options > Display**, or press **F3**.
   It cycles Off, Compact and Detailed. Detailed includes the Lua summary.
2. Reproduce the problem once. Enable the overlay before the callbacks you want
   timed. Entrypoint timings are captured only if it was already enabled when
   the script session started.
3. Press **F4** while the overlay is on. The game writes a performance report
   into its persistent data directory's `Diagnostics` folder. The path is copied
   to the clipboard and displayed on screen.
4. Open that text file and find **Active mod session** and **Lua callback
   diagnostics**. They show the actual active versions in resolved dependency
   order, registration/state diagnostics, callback statistics and recent failures.

The display option also works without a keyboard. Report saving currently uses
the F4 shortcut; there is no touch-only export button. Saving a report does not
send it anywhere.

Detailed mode displays the two callbacks with the most accumulated **self time**,
plus counts of timed calls and failures. Long names are shortened on the overlay;
the report includes longer callback identities. Compact mode collects the same
callback timings but shows only the existing compact performance display.

## Understand a timing row

The report groups observations by mod owner and callback identity:

| Column | Meaning |
| --- | --- |
| `owner` | The mod whose Lua function ran. A framework handler belongs to its provider. |
| `callback` | A definition/event, UI handler, story handler, migration or entrypoint source. |
| `timed_calls` | Invocations measured while the overlay was enabled. |
| `failures` | Exceptions escaping bounded callback execution, including untimed failures. |
| `budget_exceeded` | Invocations stopped by the Lua instruction limit. |
| `total_ms` | Accumulated inclusive wall time. It includes nested callbacks and host operations. |
| `self_ms` | Accumulated time after subtracting measured nested callbacks. |
| `max_ms` | Longest measured inclusive invocation. |

For example, a Focus add-on damage callback may call a Focus framework service.
The provider's handler gets its own row. Its time also belongs to the caller's
inclusive time; subtracting it from the caller's self time prevents counting that
same nested work twice when comparing owners. Host operations such as updating a
HUD remain in the callback that requested them.

These are wall-clock measurements around bounded execution, including worker
setup and cleanup. They are affected by the machine, garbage collection and
instrumentation. They are not a count of Lua instructions, simulation time,
per-frame budgets or proof that a callback caused a particular frame spike.
The frame statistics elsewhere in the report have their own sampling window.
Callback totals cover the script session's enabled recording intervals,
including loading; switching the overlay off and back on resumes those totals.
Starting a new script session clears the history.

## Read a nested trace or error

Recent timed invocations carry a sequence number, parent sequence, depth,
milliseconds since the diagnostics session began, inclusive/self time, the number
of forced instruction-budget yields, and success/error status. They appear in
**completion order**, oldest completion first. A provider completes before its
caller, so sequence numbers can appear out of numerical order. An old parent can
already have left the retained history.

`BUDGET_EXCEEDED` means that invocation reached its actual instruction limit.
An error message merely containing those words does not change the classification.
`forced_yields` counts interpreter budget suspensions, not ordinary Lua yielding
or an exact instruction total. Callbacks still cannot yield their own coroutines.

Failures are retained even with the overlay off. They say `untimed` if no
measurement began for that invocation. A provider error caught through
[`sf2.extensions.try_call`](../../api/extensions/#sf2extensionstry_call) still
appears as a failed provider invocation, while its caller may succeed. An error
caught entirely inside the same Lua function with `pcall` does not escape that
invocation and does not count as a callback failure.

Only execution passing through the bounded Lua runner is observed. Syntax/load
errors, native asset failures and validation performed before/after a callback
can appear in ordinary game logs or registration/state diagnostics instead.
Logging `sf2.log.error` records a message; it does not itself fail an invocation.

## Make the reproduction understandable

Use a meaningful definition ID and log decisions at transitions rather than
printing on every tick. For example, inside an already registered combat handler:

```lua
-- sf2 is the module loaded by this script. self and event are callback inputs.
local function on_damage_dealt(self, _, event)
    if event.damage > 0 and not event.blocked then
        self.state.hits = self.state.hits + 1
        if self.state.hits == 3 then
            sf2.log.debug("Three-hit objective reached")
        end
    end
end
```

Declare `hits` in that behavior's state schema and attach the handler normally;
this fragment does not register or attach a rule. See
[programmable rules](../programmable-rules/) for a complete setup.
If a service fails, include its request inputs in a focused log message before
changing state, then reproduce again after restarting with the fixed script.
Callback diagnostics do not undo earlier gameplay or state operations.

## Retention and limits

The session retains at most **1,024 distinct owner/callback statistics rows**,
**128 timed invocations** and a separate **64 failures**. Successes cannot flush
the failure history. Error text is limited to **2,048 characters** and stored
callback names to **512 characters**; newlines/tabs become spaces. The report
shows lifetime failure counts even after older error entries are overwritten.
It also reports statistics overflow and recording nesting beyond **64 levels**.
Those diagnostic bounds do not grant deeper execution or change existing Lua
callback/framework limits.

Timing and successful traces stop when the overlay is off. Failure collection
remains bounded. Stored records contain detached strings and numbers, with no
Lua closures, event arguments, fighter handles, profile data tables or widgets.
This is a local observation tool; it does not alter random streams, saved state,
content fingerprints, callback order, instruction limits or result authority.
Review error messages and the existing system details before sharing a report.

Production Lua fixtures verify nesting, provider ownership, actual instruction
exhaustion, recovery, disabled recording, retention bounds and detached snapshots.
An isolated Unity Play Mode fixture exercises the real overlay's F3/F4 update
paths with controlled key/session sources, rendered text and the saved report.
Physical keyboard/device input and full-game callback/frame attribution remain
separate acceptance work.
