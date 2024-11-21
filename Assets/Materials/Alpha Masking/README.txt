HOW TO USE:

- To apply the mask to objects, attach those objects to the same parent (may have more than one level of hierarchy) and put the mask directly under that parent, too.

- Clicking "Apply Mask to Siblings in Hierarchy" will detect all siblings (including their children) and attempt to apply the mask to all them. If an object uses the Default Sprite or Unlit/Transparent shader, the mask will be applied to it.

- The mask can be moved, scaled and rotated freely in the Editor, but it can only be rotated over a chosen axis (depending on what mapping axis is selected).


MASK PARAMETERS:

- Mask Mapping World Axis: defines, over which axis the mask should be applied. This is usually the axis, which corresponds with the camera direction.
- Invert Axis: in case you need to map the mask over an inverted axis.
- Clamp Alpha Horizontally: if the texture isn't clamped by Unity (in import settings), then you can choose to clamp it horizontally only (it will be repeated vertically, unless chosen otherwise).
- Clamp Alpha Vertically: if the texture isn't clamped by Unity (in import settings), then you can choose to clamp it vertically only (it will be repeated horizontally, unless chosen otherwise).
- Clamping Border: if one of the two bove settings are enabled, you can use this variable to tweak the "edge" with of clamping. Depending on the alpha texture size and its usage, you might run into texture clamping issues. In that case, try increasing (or lowering) the Clamping Border value.
- Use Mask A Channel (not RGB): the mask uses the texture RGB channels by default. Toggle "Use Mask A Channel (not RGB)" to use the Alpha channel of the texture instead.
- Display Mask: toggle this setting to enable or disable the visibility of the mask. This setting is only available in the Editor (and while not running the player).


THING TO HAVE IN MIND:

- You can either create your own materials for masked Sprites/3D objects manually, or the Mask will create its own.

- When using prefabs of masked objects, it's better to manually create materials.