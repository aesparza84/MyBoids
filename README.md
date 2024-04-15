# My Boids
A project that brings the 3 rules of Boids to make to make a 'Flocking' simulation

## The Rules
- **Alignment**: (A boid) Observes its neighbors within a small radius, averages the '_headings_' of the neighbors, and heads toward the **general** heading of all neighbors.
- **Cohesion**: (A boid) Observes its neighbors within a small radius, averages the '_positions_' of the neighbors, and heads toward the **Center** of a cluster of boids.
- **Seperation**: (A boid) Observes its neighbors within a smaller radius (smaller than other radius) to determine if a neighboring boid / Obstacle will collide. Calculates an average _Spacing_ vector to avoid collisions in environment. 
