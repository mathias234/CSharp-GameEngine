# MonoGameEngine
A Game Engine built with C# using OpenTK

Usage:
use mklink /J in each game folder to link to the res folder that contains all the resources

```
 mklink /J "C:\engine\builds\debug\res" "C:\engine\res"
```

**Known Issues:**
- Moving camera sometimes fades out directional light shadows
- Issues when the engine gets under alot of load it start to blink
- Textures are read upside-down (maybe needs more testing)

**Future Plans:**

- FXAA or other anti aliasing techniques
- More UI stuff like text, and clicking and stuff
- Editor
