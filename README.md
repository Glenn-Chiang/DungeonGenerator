# dungeon-generator
A demonstration of 2D procedural dungeon generation in Unity

## Approach
1. Room placement with Binary Space Partitioning (BSP)
  - We start with a base room occupying the full space, then recursively subdivide it into smaller rooms
  - On every iteration, each room is split into 2 smaller rooms. This means that for n iterations, the total number of rooms generated is n<sup>2</sup>.
  - Each split can either be horizontal or vertical. We can set a minimum and maximum aspect ratio and force a horizontal or vertical split if a room is too tall or too wide respectively.
  - The horizontal and vertical split positions can be constrained within a small range to create more homogenous, similar-sized rooms, or within a large range to create more heterogenous rooms of varying sizes
    
2. Corridor creation with A* Pathfinding
 - The A* pathfinding algorithm is used to create paths between rooms
 - Different costs can be assigned to empty and filled (floor and wall) cells to control the distribution and shapes of paths formed
 - For example, a higher cost can be assigned to filled cells and a lower cost can be assigned to empty cells to bias the algorithm toward reusing existing empty paths instead of carving out new paths through walls
 - We can also assign a cost for directional changes to make the algorithm avoid turning and favour straighter paths
