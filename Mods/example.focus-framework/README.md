# Focus Framework and Focus Trial

Enable both `example.focus-framework` and `example.focus-addon`, then enter Act I
tournament battle 3 in normal or Eclipse mode. The add-on mounts a Focus HUD and
passes outgoing hits to the framework. Every third positive, unblocked hit gains
50% additional pending damage. The framework owns the saved Focus resource; the
add-on owns the fight rule, damage operation and HUD.

Another mod can declare the framework dependency and acquire the same services.
Eclipse validates each request and response and supplies the caller's real mod ID.
This demonstrates shared Lua functionality without sharing native fighter handles,
editing base XML or using a privileged namespace.

Focus survives save/reload and temporary removal using the normal mod state
contract. Starting a new profile starts at zero. Disabling the add-on removes its
rules/UI without deleting the framework's resource. Disable conflicting examples
when inspecting this mechanic in isolation.

The managed runner `Tools/Tests/Modding/TestModExtensions.ps1` executes the actual
two-mod Lua and production routing with controlled combat/UI sources. Native
HUD rendering, font loading, changing pixels, removal/reinstall state preservation
and teardown also pass in isolated Unity 6.6 Play Mode through
`Tools/Tests/Modding/TestModExtensionsUnity.ps1`. Real contact timing, native
menu/profile integration and a full game playtest still require acceptance.
No custom outcome, extra fighters or unrestricted engine access is claimed.
