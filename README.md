# AustinMan-Voxel

Abstract

The aim of this work is to develop a 3d representation of the human body so that it
contains also “internal” texture sampled from the AustinMan dataset. AustinMan is a
voxel model of the human body that is being developed for simulations. It contains
1878 horizontal (only) slices of a whole human body in different resolutions. The
grayscale value of each pixel corresponds to a different layer of the body, such as
tissues, bones and organs, for a total of 64 layers. This internal texture is then used to
build new images from the intersection of a plane with the body. The power of this
concept lies in the fact that is possible to build slices of the body in any orientation, so
not just only horizontal. The body can be also set in different poses and the position of
the internal points is recalculated.

Regarding voxelizations, two algorithms are presented. The first, very simple, is a
simple voxelization (Minecraft style), that builds meshes triangulating cubes. It is used
to build simple models for the human skin and for every internal layer of the human
body. Then, a slightly more complex algorithm, Marching Cubes, is presented. It
works creates a polygonal surface mesh from a 3D scalar field by “marching” (looping)
through the 3D space, and determining each configuration for the given cube. It builds
the final human skin mesh that, once bone-armored, is used to generate the colliders
essential to capture the deformation of single body parts, like hands, arms, feet, etc.,
that is one main point of this MRI simulation.

Key-words: Voxelization, UV mapping, Three-Dimensional UVW deformation, Mesh
generation, Mesh rigging,
