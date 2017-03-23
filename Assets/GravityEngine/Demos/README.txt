Demonstration Scenes:

On-line documentation:
http://nbodyphysics.com/blog/gravity-engine-doc

Docs for script elements:
http://nbodyphysics.com/gravityengine/html/

OrbitDecay:
==========
Shows a star with a planet with an initial elliptical orbit.
A SimpleOrbitDecay script is attached to the planet and this
script reduces the planet's velocity by a small amount each
frame (controlled by the decay parameter)

RandomPlanets:
==============
Random planets has a central star with a RandomPlanets script
attached. When the scene starts this script programatically
adds random planets with orbit elements constrained by the
parameters of the RandomPlanet component. 

This script is a good template for script-based creation of
planets with specific orbits. 

Solar System Inner/Outer
========================
Shows the inner/outer solar system build with the Inspector
Here in the Canvas element the SolarSystem mass has
the SolarSystem component attached. 

Additional bodies can be added by the "Add Body" button. 

Tutorial video:
http://nbodyphysics.com/blog/gravity-engine-doc-1-3/build-the-solar-system/

Three Bodies
============
Uses the ThreeBodySolution class and the AZTIntegrator to show
three body configurations. In general three body configurations
are unstable, but in the the past 30 years there has been a number
of interesting solutions discovered. These are available in this
code. 

Video: 
http://nbodyphysics.com/blog/gravity-engine-doc-1-3/demonstrations/

Force InverseR
==============
Demonstrates the use of an alternate gravity law (1/R) and the
resulting orbits. 

Force is selected in the GravityEngine Advanced drawer. 

If the force is changed, then the initial velocities of bodies
in the scene will likely need adjusting. 

Custom forces can be defined. See the CustomForce example script
and the Alternative Forces demo video. 


Runtime Add Delete
==================
Inter-active scene that provides buttons to add/remove massive and
massless bodies in circular orbits around a star. 

See the AddDeleteTest.cs script for detail on how this is done. 



