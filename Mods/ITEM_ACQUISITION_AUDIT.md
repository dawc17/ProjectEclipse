# Item acquisition and purchase policy audit

Evidence checked against the working tree on 2026-09-12. This is implementation
planning, not a list of supported Lua functions. No DE content is applied.

## Native routes

| Route | Evidence | Consequence for policy |
| --- | --- | --- |
| Shop affordability | `ListSF.CLKECIFEMNB` computes price using `count` | Quantity must survive the dispatch and grant path. |
| Shop dispatch | `ListSF.KCBCGDFKNME` changes balances and invokes `IGLBLDKOMML` | A purchase restriction must run before balance changes, not in a later notification. |
| Common shop grant | `IGLBLDKOMML` calls `GEFDJDIINND(item, count, ...)`, then use/equip and purchase quest dispatch | Record successful purchase identity and quantity before callbacks can consume/remove the item or re-enter purchasing. |
| Alternate immediate purchases | `ItemBuyHelper.IHHKNBPKGHD`, `MGMAJHLAICA`, `NIEAANPCGLC` call their own `KCBCGDFKNME` | A policy only installed in `ListSF` leaves a bypass. These methods also have their own balance/save sequence. |
| Delivery/upgrades | Separate actions share parts of the purchase dispatcher | Upgrading an existing item or accelerating delivery must not consume a new-item purchase allowance. |
| Free/real-money items | `Item_Free` and `Item_Buy_Real` return through separate handlers | Do not assume the common grant callback covers them, or invent behavior for external payment fulfillment. |
| Consumption | `ListSF.AFGHCIDFAHB` checks `ANNCECNAEPN` and calls `EFPHIJGNGKP` | Current owned count cannot prove historical purchases. |
| Lua story notification | `ModRuntime.CaptureStoryEvent` maps the purchase quest event to item/recipe metadata | It discards the native action and quantity and only runs with active scripts/subscribers. It cannot serve as an authoritative historical purchase ledger. |

## Fixed prerequisite

The shop dispatcher accepted `count`, but called the common grant method without
it, selecting the default of one. It now forwards the quantity. Null items and
nonpositive quantities are rejected before balance changes. The separate recipe
delivery path is unchanged.

`Tools/TestPurchaseQuantity.ps1` executes the production dispatcher with controlled
balance/grant/UI services: three purchase currencies/categories preserve quantity,
invalid quantities and null items cause no mutation, and the default remains one.
This does not prove native inventory persistence, arithmetic overflow handling,
concurrent/reentrant purchase safety or live shop behavior.

## Requirements before exposing purchase limits

1. Use a profile-backed ledger distinct from consumable inventory. Separate
   completed purchase transactions and purchased unit counts; retain item identity
   while a mod is disabled. Historical purchases before ledger support are unknown,
   not inferred from current inventory.
2. Define purchase versus grant/upgrade/delivery explicitly. An API should expose
   this distinction through typed context rather than raw action names or XML.
3. Check limits before charging on every supported route. Keep an in-progress
   reservation until grant and bookkeeping complete so callbacks cannot re-enter
   the same allowance. Undo reservations on failure; reject oversized quantity
   before price arithmetic.
4. Store completion with the inventory/balance save boundary. A separately saved
   receipt introduces crash windows. Verify reload and profile-switch behavior.
5. Make UI availability reflect the same policy as execution. A hidden button alone
   is not enforcement. Preserve the original game presentation for restrictions.
6. Test gifts, consumption, repeat purchase, upgrades, failed grant, insufficient
   funds, callbacks, disable/re-enable, save reload, and independent profiles.

`SingleTimeBuy` is present in archived DE records, but the active vanilla occurrence
found in `list.xml` is commented out. There is no existing native parser or policy
implementation to merely expose. Its archived name alone does not decide whether
the allowance concerns one transaction or one unit. The reusable API should make
that choice explicit; downstream content can select its intended rule.

Additional verified prerequisite: affordability now rejects negative prices, invalid quantities and totals exceeding Int64 before multiplication/subtraction. Previously a positive wraparound total could be treated as affordable. The dispatcher/affordability fixture now covers 40 cases with controlled services; inventory count overflow, durable purchase history and physical shop acceptance remain open.
