# Node Network Navigator

Node Network Navigator is a proof-of-concept system for idea management with a
natural user interface. It uses a near-mode Kinect and a projected display
extension to allow a flat surface to be used as a multitouch and gesture-enabled
interaction space, which is configured to create and manipulate nodes
representing ideas or objects.

## Installation

This project generally works on 64-bit Windows 7 machines with Visual Studio C#
2010. You will need to install the
[Kinect SDK](http://www.microsoft.com/en-us/kinectforwindows/develop/developer-downloads.aspx)
and the
[InteractiveSpace SDK](https://github.com/DCog-HCI-UCSD/InteractiveSpaceEngine)
to use the project.

*NB.* If you get InteractiveSpace as a zip bundle, *do not* unzip it. Place the
zip inside `%USERPROFILE%\Documents\Visual Studio 2010\Templates` (or the
Templates directory of your Visual Studio install).

## Interface overview

Nodes currently only show and allow editing of the title of each node. Support
for content may be added in the foreseeable future.

There are four tools available inside the workspace:
* Create node, a green button with a circled plus. Makes a new node with a
default title, in a default location.
* Edit node, a gray button with a pencil. When a node is selected with this
tool active, a form opens for editing the node title.
* Toggle connections, a blue button with two circles joined by an edge. When
two nodes are simultaneously selected with this tool active, the state of
connectivity between the two nodes is toggled (ie. two separate nodes will
become connected, and two connected nodes will be disconnected from each other).
* Delete node, a red button with a garbage can. When a node is selected with
this tool active, it will be destroyed.

We also have a basic file format for loading and saving workspaces, but it has
not been integrated yet. This may happen in the near future.
