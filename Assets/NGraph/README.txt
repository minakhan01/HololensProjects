Documentation: http://www.niugnepsoftware.com/ngraph/
Support: http://forum.niugnepsoftware.com/viewforum.php?f=9

=== YOU ARE READY NOW ===
If you are using the new native Unity GUI (called uGUI) in Unity 4.6 or later, you are ready to go!
Make sure you have a UI Canvas in your scene and then use the Graph Wizard located under Window->Graph Pro.

===YOU ARE NOT READY YET, IF...===
You are using a 3rd party GUI toolkit.

---Extract the packages you need---
Becuase Graph Master suports multiple GUI front ends you MUST choose the packages to extract next to this REAMME file.
For example, if you want to use Graph Master with NGUI you will need to extract the "NGUI" Unity Package.
If you want to use Graph Master with Daikon Forge you will need to extract the "DaikonForge" Unity Package.

If you extract a package and you do not have the front end code for it installed, you will compile get errors.

===Note About NGUI versions===
If you running NGUI 3.0.3 (or less) you can still use this package - you just have to delete the "Editor" folder under Graph Master.
This will remove the ability to use the Graph Master Creation Wizard(s).  Just add a game object called "Graph" with the "UINgraph" component attached under an NGUI Panel.
Make sure to fill out the UINgraph Inspector properties.



===Versions===
---2.0.0---
[NEW] Renamed asset to "Graph Master".
[NEW] uGUI Supported!  The new native Unity 4.6 GUI is now supported out-of-the-box.
[NEW] All margins (bottom, left, top, and right) are now configurable in size.
[FIX] Some components of the graph would draw in the wrong order after a re-size or other change in properties. Drawing order is now preserved.
[FIX] Reveal option on plots now finish correctly and show entire plot.
[FIX] Markers now behave correctly with reveal option.
[UPDATE] Width and Height options now available in the creation dialog.
[CHECK] NGUI v3.7.5 confirmed to work with this release.
[CHECK] Daikon Forge v1.0.16 hf1 confirmed to work with this release.
[CHECK] 2D Toolkit v2.5 confirmed to work with this release.
[NOTE] This will the the last update with Daikon Forge support.  The Daikon Forge team has been discontinued thier asset, so we will no longer support it.

---1.3.0---
[NEW] Grid lines!  You can now effect grid lines via the custom Inspector or programatically.
      Options exist for:
      - Grid line separation (in both the x and y directions)
      - Grid line thickness
      - Grid line color

---1.2.2---
[FIX] NGUI v3.5.3 compatibility fixes.
[FIX] When Window size changes, graphs now behave correctly.

---1.2.1---
[FIX] Daikon Forge callbacks now use correct types in callbacks.
[FIX] Daikon Forge panels that have thier enabled or visiable propery set will now correctly affect the graph.
[NEW] Data labels can now be added to any series using "addDataLabel(...)".

---1.2.0---
[FIX] Plots now constrained to plot area (values lower and higher than the plot area will not draw).
[NEW] Equations!!! An Equation Plot type has been added.  Most functions are sported - sin, cos, tan, log, ln, PI, E, etc.
[NEW] 2D Tool Kit GUI system support.
[NEW] Reveal speed added.
[NEW] 2DTK, NGUI and Daikon Forge graph types all have sported editor customizations - this makes it much easier to change the look and feel of the graph from the editor.

---1.1.2---
[FIX] Script errors when building for stand alone have been fixed.
[FIX] Labels in Daikon Forge (v1.0.13 f2) drawing in correct spots now.

---1.1.1---
[NEW] Daikon Forge support!
[NEW] Bar Plots!
[NEW] Added ability to define the axis location for both X and Y axes.
[NEW] Marker color can now be different than plot color.
[NEW] Added callback to allow for custom labels.  Label callback is called when a label is rendered allowing cusom code to be run on label.
[NEW] NGUI - Added options for Dynamic (true type) font or bitmap font.

[FIX] NGUI 3.0.4 font axis position corrected.
[FIX] Memory leak fixed related to destroying and recreating meshes.

---1.0.1---
Initial release