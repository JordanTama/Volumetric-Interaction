# **VOLUMETRIC INTERACTION**

This project involves developing a dynamic volumetric (3D) texture rendering implementation to
assess the viability of dynamic volumetric textures in games. The viability will be gauged by
performance benchmarks in a simple interactive environment.
By using dynamic volumetric textures for environment interaction, multiple shortcomings of
traditional approaches can be subverted, resulting in more immersive virtual environments.

## **Task list:**

##### Aim 1: _To create a volumetric texture rendering implementation that can be seamlessly integrated into various projects._

- [ ] **1.1:** Conduct background research
    - [ ] **1.1.1:** Research existing environmental interaction approaches.
    - [ ] **1.1.2:** Research volumetric texture rendering implementations.
    - [ ] **1.1.3:** Identify game engines with features that support volumetric texture rendering.
    - [ ] **1.1.4:** Research distance field generation algorithms.
    - [ ] **1.1.5:** Identify reasonable performance targets from existing games.
    
- [ ] **1.2:** Develop API for volumetric texture rendering.
    - [X] **1.2.1:** Program dynamic manager/actor relationship.
    - [ ] **1.2.2:** Program debug and editor tools.
    
- [ ] **1.3:** Implement volumetric texture rendering.
    - [ ] **1.3.1:** Implement brute force texture rendering algorithm.
    - [ ] **1.3.2:** Program simple vertex displacement shader to debug vector field output.
    - [ ] **1.3.3:** Program ray-marching shader for visualization.
    - [ ] **1.3.4:** Implement faster rendering algorithm.

---

##### Aim 2: _To assess the viability of dynamic volumetric texture rendering in virtual environments with various performance metrics._
- [ ] **2.1:** Set up testing environment.
    - [ ] **2.1.1:** Identify and learn to use profiling/benchmarking tools.
    - [ ] **2.1.2:** Program events that are consistent but test edge cases and stress.
    - [ ] **2.2.3:** Compare output with target performance metrics.

---

##### Aim 3: _To create a simple interactive environment to demonstrate possible uses for the system._
- [ ] **3.1:** Create demonstration shaders.
    - [ ] **3.1.1:** Program interactive grass shader.
    - [ ] **3.1.2:** Program interactive foliage shader.
    - [ ] **3.1.3:** Program interactive fog shader.
    - [ ] **3.1.4:** Program interactive water shader.
    
- [ ] **3.2:** Implement player input.
    - [ ] **3.2.1:** Create player controller.
    
- [ ] **3.3:** Build project.
    - [ ] **3.3.1:** Export to supported target platforms.