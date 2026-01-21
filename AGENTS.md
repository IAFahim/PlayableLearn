# 🗺️ The Roadmap: 100 Days of Playables

### Sprint 1: The Heartbeat (Foundations)
*Goal: Understand the Graph lifecycle without touching an Animation Clip yet.*
- [x] **Day 01:** The Empty Graph. (Creating/Destroying the `PlayableGraph`). `Day01.asmdef`, `Day01Entry.cs`, `Day01GraphHandle.cs`, `Day01GraphHandleExtensions.cs`, `GraphOps.cs, README.md`
- [x] **Day 02:** Check Old day and add The Output. (Connecting a `ScriptPlayableOutput` to talk to the console). `Day02.asmdef`, `Day02Entry.cs`, `Day02OutputHandle.cs`, `Day02OutputHandleExtensions.cs`, `ScriptOutputOps.cs, README.md`
- [x] **Day 03:** Check Old day and add The First Node. (Creating a `ScriptPlayable` and linking it). `Day03.asmdef`, `Day03Entry.cs`, `Day03NodeHandle.cs`, `Day03NodeHandleExtensions.cs`, `ScriptPlayableOps.cs`, `Day03EmptyBehaviour.cs, README.md`
- [x] **Day 04:** Check Old day and add The Update Cycle. (Using `ProcessFrame` to rotate a generic Cube). `Day04.asmdef`, `Day04Entry.cs`, `Day04RotatorBehaviour.cs`, `Day04RotatorData.cs`, `RotatorLogic.cs`, `Day04RotatorExtensions.cs, README.md`
- [x] **Day 05:** Check Old day and add Time Dilation. (Manipulating `SetSpeed` manually). `Day05.asmdef`, `Day05Entry.cs`, `Day05SpeedData.cs`, `SpeedOps.cs`, `Day05SpeedExtensions.cs, README.md`
- [x] **Day 06:** Check Old day and add The Pause Button. (Playing/Stopping the graph programmatically). `Day06.asmdef`, `Day06Entry.cs`, `Day06PlayStateData.cs`, `PlayStateOps.cs`, `Day06PlayStateExtensions.cs, README.md`
- [x] **Day 07:** Check Old day and add The Reverse Time. (Negative speed and time wrapping). `Day07.asmdef`, `Day07Entry.cs`, `Day07ReverseData.cs`, `ReverseTimeOps.cs`, `Day07ReverseExtensions.cs, README.md`
- [x] **Day 08:** Check Old day and add The Director Name. (Naming graphs for the Profiler—crucial for debugging). `Day08.asmdef`, `Day08Entry.cs`, `Day08NamedGraphData.cs`, `GraphNamingOps.cs`, `Day08NamedGraphExtensions.cs, README.md`
- [x] **Day 09:** Check Old day and add The Graph Visualizer. (Installing the package and seeing our nodes). `Day09.asmdef`, `Day09Entry.cs`, `Day09VisualizerSetup.cs, README.md`
- [x] **Day 10:** Check Old day and add The Cleanup. (Proper `IDisposable` patterns for Playables). `Day10.asmdef`, `Day10Entry.cs`, `Day10DisposableGraph.cs`, `Day10DisposableGraphExtensions.cs`, `DisposalOps.cs`, `README.md`

### Sprint 2: The Puppeteer (Basic Animation)
*Goal: Controlling an Animator without the Animator Controller state machine.*
- [x] **Day 11:** Check Old day and add The Output Hook. (Connecting `AnimationPlayableOutput` to an Animator). `Day11.asmdef`, `Day11Entry.cs`, `Day11AnimOutputHandle.cs`, `Day11AnimOutputExtensions.cs`, `AnimOutputOps.cs, README.md`
- [x] **Day 12:** Check Old day and add The Clip Player. (Wrapping `AnimationClipPlayable`). `Day12.asmdef`, `Day12Entry.cs`, `Day12ClipHandle.cs`, `Day12ClipExtensions.cs`, `ClipPlayableOps.cs, README.md`
- [x] **Day 13:** Check Old day and add The Mixer. (Introduction to `AnimationMixerPlayable`). `Day13.asmdef`, `Day13Entry.cs`, `Day13MixerHandle.cs`, `Day13MixerExtensions.cs`, `MixerOps.cs, README.md`
- [x] **Day 14:** Check Old day and add Hard Swapping. (Disconnecting input 0 and connecting input 1). `Day14.asmdef`, `Day14Entry.cs`, `Day14SwapData.cs`, `SwapConnectionOps.cs`, `Day14SwapExtensions.cs, README.md`
- [x] **Day 15:** Check Old day and add Weighted Blending. (Setting input weights 0.5/0.5). `Day15.asmdef`, `Day15Entry.cs`, `Day15BlendData.cs`, `WeightBlendOps.cs`, `Day15BlendExtensions.cs, README.md`
- [ ] **Day 16:** Check Old day and add The Crossfade Logic. (Math for lerping weights over time). `Day16.asmdef`, `Day16Entry.cs`, `Day16CrossfadeData.cs`, `CrossfadeOps.cs`, `Day16CrossfadeExtensions.cs`, `Day16CrossfadeBehaviour.cs, README.md`
- [ ] **Day 17:** Check Old day and add Layering. (Introduction to `AnimationLayerMixerPlayable`). `Day17.asmdef`, `Day17Entry.cs`, `Day17LayerHandle.cs`, `Day17LayerExtensions.cs`, `LayerMixerOps.cs, README.md`
- [ ] **Day 18:** Check Old day and add Masking. (Applying AvatarMasks to layers via code). `Day18.asmdef`, `Day18Entry.cs`, `Day18MaskData.cs`, `MaskingOps.cs`, `Day18MaskExtensions.cs, README.md`
- [ ] **Day 19:** Check Old day and add Root Motion. (Reading root motion from the graph). `Day19.asmdef`, `Day19Entry.cs`, `Day19RootMotionData.cs`, `RootMotionOps.cs`, `Day19RootMotionExtensions.cs, README.md`
- [ ] **Day 20:** Check Old day and add Additive Mixing. (Breathing animations on top of Idle). `Day20.asmdef`, `Day20Entry.cs`, `Day20AdditiveData.cs`, `AdditiveMixOps.cs`, `Day20AdditiveExtensions.cs, README.md`

### Sprint 3: The Synthesizer (Audio & Sync)
*Goal: Frame-perfect audio synchronization.*
- [ ] **Day 21:** Check Old day and add The Audio Output. (Binding `AudioPlayableOutput` to AudioSource). `Day21.asmdef`, `Day21Entry.cs`, `Day21AudioOutputHandle.cs`, `Day21AudioOutputExtensions.cs`, `AudioOutputOps.cs, README.md`
- [ ] **Day 22:** Check Old day and add The Clip Provider. (`AudioClipPlayable`). `Day22.asmdef`, `Day22Entry.cs`, `Day22AudioClipHandle.cs`, `Day22AudioClipExtensions.cs`, `AudioClipOps.cs, README.md`
- [ ] **Day 23:** Check Old day and add The Audio Mixer. (Blending two music tracks via graph weights). `Day23.asmdef`, `Day23Entry.cs`, `Day23AudioMixerHandle.cs`, `Day23AudioMixerExtensions.cs`, `AudioMixerOps.cs, README.md`
- [ ] **Day 24:** Check Old day and add Syncing Audio to Animation. (Ensuring footsteps match sound). `Day24.asmdef`, `Day24Entry.cs`, `Day24SyncData.cs`, `AudioSyncOps.cs`, `Day24SyncExtensions.cs`, `Day24SyncBehaviour.cs, README.md`
- [ ] **Day 25:** Check Old day and add Graph Time vs. DSP Time. (Understanding audio drift). `Day25.asmdef`, `Day25Entry.cs`, `Day25DriftData.cs`, `DspDriftOps.cs`, `Day25DriftExtensions.cs, README.md`
- [ ] **Day 26:** Check Old day and add Speed Pithcing. (Slowing down the graph slows down the audio). `Day26.asmdef`, `Day26Entry.cs`, `Day26PitchData.cs`, `PitchSpeedOps.cs`, `Day26PitchExtensions.cs, README.md`
- [ ] **Day 27:** Check Old day and add Scrubbing Audio. (Setting time manually triggers sound correctly). `Day27.asmdef`, `Day27Entry.cs`, `Day27ScrubData.cs`, `AudioScrubOps.cs`, `Day27ScrubExtensions.cs, README.md`
- [ ] **Day 28:** Check Old day and add Multiple Outputs. (One graph driving Audio AND Animation). `Day28.asmdef`, `Day28Entry.cs`, `Day28DualOutputHandle.cs`, `Day28DualOutputExtensions.cs`, `DualOutputOps.cs, README.md`
- [ ] **Day 29:** Check Old day and add The Master Mixer. (Controlling volume via graph weights). `Day29.asmdef`, `Day29Entry.cs`, `Day29VolumeData.cs`, `MasterVolumeOps.cs`, `Day29VolumeExtensions.cs, README.md`
- [ ] **Day 30:** Check Old day and add Audio Visualization. (Reading output data from the graph). `Day30.asmdef`, `Day30Entry.cs`, `Day30VisualizationData.cs`, `AudioVisualizationOps.cs`, `Day30VisualizationExtensions.cs, README.md`

### Sprint 4: The Architect (Complex Structures)
*Goal: Building a reusable tree structure.*
- [ ] **Day 31:** Check Old day and add The Binary Tree. (A mixer feeding into a mixer). `Day31.asmdef`, `Day31Entry.cs`, `Day31TreeHandle.cs`, `Day31TreeExtensions.cs`, `BinaryTreeOps.cs`
- [ ] **Day 32:** Check Old day and add Dynamic Inputs. (Adding inputs to a mixer at runtime). `Day32.asmdef`, `Day32Entry.cs`, `Day32DynamicMixerData.cs`, `DynamicInputOps.cs`, `Day32DynamicMixerExtensions.cs`
- [ ] **Day 33:** Check Old day and add Input Ports. (Connecting to specific indices). `Day33.asmdef`, `Day33Entry.cs`, `Day33PortData.cs`, `InputPortOps.cs`, `Day33PortExtensions.cs`
- [ ] **Day 34:** Check Old day and add Graph Traversal. (Finding a specific playable in the tree). `Day34.asmdef`, `Day34Entry.cs`, `Day34TraversalData.cs`, `GraphTraversalOps.cs`, `Day34TraversalExtensions.cs`
- [ ] **Day 35:** Check Old day and add Propagating Time. (Setting time on a branch, not the root). `Day35.asmdef`, `Day35Entry.cs`, `Day35BranchTimeData.cs`, `BranchTimeOps.cs`, `Day35BranchTimeExtensions.cs`
- [ ] **Day 36:** Check Old day and add Orphaned Nodes. (What happens when you disconnect but don't destroy). `Day36.asmdef`, `Day36Entry.cs`, `Day36OrphanData.cs`, `OrphanNodeOps.cs`, `Day36OrphanExtensions.cs`
- [ ] **Day 37:** Check Old day and add Playable Queues. (Pre-warming clips before connecting them). `Day37.asmdef`, `Day37Entry.cs`, `Day37QueueData.cs`, `PlayableQueueOps.cs`, `Day37QueueExtensions.cs`
- [ ] **Day 38:** Check Old day and add The Adapter Pattern. (Making a generic interface for any Playable). `Day38.asmdef`, `Day38Entry.cs`, `IPlayableAdapter.cs`, `Day38AdapterExtensions.cs`, `AdapterOps.cs`
- [ ] **Day 39:** Check Old day and add Visual Debugging II. (Drawing the tree structure in OnGUI). `Day39.asmdef`, `Day39Entry.cs`, `Day39DebugDrawer.cs`, `GraphDebugOps.cs`
- [ ] **Day 40:** Check Old day and add Stress Test. (Creating 1,000 nodes and checking the Profiler). `Day40.asmdef`, `Day40Entry.cs`, `Day40StressData.cs`, `StressTestOps.cs`, `Day40StressExtensions.cs`

### Sprint 5: The Operator (Custom ScriptPlayables)
*Goal: Injecting C# Logic into the C++ Engine.*
- [ ] **Day 41:** Check Old day and add The `PlayableBehaviour`. (Inheriting the base class). `Day41.asmdef`, `Day41Entry.cs`, `Day41CustomBehaviour.cs`, `Day41BehaviourData.cs`
- [ ] **Day 42:** Check Old day and add `PrepareFrame`. (Logic that runs *before* evaluation). `Day42.asmdef`, `Day42Entry.cs`, `Day42PrepareBehaviour.cs`, `Day42PrepareData.cs`, `PrepareFrameOps.cs`
- [ ] **Day 43:** Check Old day and add `ProcessFrame`. (Logic that runs *during* evaluation). `Day43.asmdef`, `Day43Entry.cs`, `Day43ProcessBehaviour.cs`, `Day43ProcessData.cs`, `ProcessFrameOps.cs`
- [ ] **Day 44:** Check Old day and add Data Passing. (Sending structs into the Behaviour). `Day44.asmdef`, `Day44Entry.cs`, `Day44DataBehaviour.cs`, `Day44PayloadData.cs`, `PayloadPassingOps.cs`
- [ ] **Day 45:** Check Old day and add The Event System. (Firing C# events from `ProcessFrame`). `Day45.asmdef`, `Day45Entry.cs`, `Day45EventBehaviour.cs`, `Day45EventData.cs`, `EventDispatchOps.cs`
- [ ] **Day 46:** Check Old day and add Trigger Detection. (Checking if local time crossed a threshold). `Day46.asmdef`, `Day46Entry.cs`, `Day46TriggerBehaviour.cs`, `Day46TriggerData.cs`, `TriggerDetectionOps.cs`
- [ ] **Day 47:** Check Old day and add Looping Logic. (Detecting loops manually). `Day47.asmdef`, `Day47Entry.cs`, `Day47LoopBehaviour.cs`, `Day47LoopData.cs`, `LoopDetectionOps.cs`
- [ ] **Day 48:** Check Old day and add Pausing Internally. (Logic to freeze local time). `Day48.asmdef`, `Day48Entry.cs`, `Day48PauseBehaviour.cs`, `Day48PauseData.cs`, `InternalPauseOps.cs`
- [ ] **Day 49:** Check Old day and add The Tween. (Using a Playable to drive a float value). `Day49.asmdef`, `Day49Entry.cs`, `Day49TweenBehaviour.cs`, `Day49TweenData.cs`, `TweenOps.cs`, `Day49TweenExtensions.cs`
- [ ] **Day 50:** Check Old day and add The Color Changer. (Driving material colors via PlayableBehaviour). `Day50.asmdef`, `Day50Entry.cs`, `Day50ColorBehaviour.cs`, `Day50ColorData.cs`, `ColorDriveOps.cs`, `Day50ColorExtensions.cs`

### Sprint 6: The Worker (Animation Jobs & Burst)
*Goal: High-performance modification of the bone stream.*
- [ ] **Day 51:** Check Old day and add The `IAnimationJob`. (Setting up the struct). `Day51.asmdef`, `Day51Entry.cs`, `Day51EmptyJob.cs`, `Day51JobData.cs`
- [ ] **Day 52:** Check Old day and add The `AnimationScriptPlayable`. (Creating the job runner). `Day52.asmdef`, `Day52Entry.cs`, `Day52JobRunner.cs`, `Day52JobRunnerExtensions.cs`, `JobRunnerOps.cs`
- [ ] **Day 53:** Check Old day and add Handle Binding. (Binding `TransformStreamHandle` to bones). `Day53.asmdef`, `Day53Entry.cs`, `Day53HandleBindingJob.cs`, `Day53HandleData.cs`, `HandleBindingOps.cs`
- [ ] **Day 54:** Check Old day and add Reading Transforms. (Getting position/rotation in the job). `Day54.asmdef`, `Day54Entry.cs`, `Day54ReadJob.cs`, `Day54ReadData.cs`, `TransformReadOps.cs`
- [ ] **Day 55:** Check Old day and add Writing Transforms. (Overriding bone positions). `Day55.asmdef`, `Day55Entry.cs`, `Day55WriteJob.cs`, `Day55WriteData.cs`, `TransformWriteOps.cs`
- [ ] **Day 56:** Check Old day and add Burst Compilation. (Adding `[BurstCompile]` to the job). `Day56.asmdef`, `Day56Entry.cs`, `Day56BurstJob.cs`, `Day56BurstData.cs`, `BurstCompileOps.cs`
- [ ] **Day 57:** Check Old day and add LookAt Job. (Math for looking at a target). `Day57.asmdef`, `Day57Entry.cs`, `Day57LookAtJob.cs`, `Day57LookAtData.cs`, `LookAtOps.cs`, `Day57LookAtExtensions.cs`
- [ ] **Day 58:** Check Old day and add Local vs. World. (Understanding space in jobs). `Day58.asmdef`, `Day58Entry.cs`, `Day58SpaceJob.cs`, `Day58SpaceData.cs`, `SpaceConversionOps.cs`
- [ ] **Day 59:** Check Old day and add Two-Bone IK. (Implementing simple IK in a job). `Day59.asmdef`, `Day59Entry.cs`, `Day59TwoBoneIKJob.cs`, `Day59IKData.cs`, `TwoBoneIKOps.cs`, `Day59IKExtensions.cs`
- [ ] **Day 60:** Check Old day and add Job Data. (Passing native arrays into the animation job). `Day60.asmdef`, `Day60Entry.cs`, `Day60NativeJob.cs`, `Day60NativeData.cs`, `NativeArrayJobOps.cs`

### Sprint 7: The Director (Timeline Integration)
*Goal: Extending the Unity Timeline with our systems.*
- [ ] **Day 61:** Check Old day and add The `PlayableAsset`. (Creating the factory). `Day61.asmdef`, `Day61Entry.cs`, `Day61ClipAsset.cs`, `Day61ClipBehaviour.cs`
- [ ] **Day 62:** Check Old day and add The `TrackAsset`. (Creating a custom track). `Day62.asmdef`, `Day62Entry.cs`, `Day62CustomTrack.cs`, `Day62ClipAsset.cs`, `Day62ClipBehaviour.cs`
- [ ] **Day 63:** Check Old day and add Binding Objects. (Exposing scene references to the track). `Day63.asmdef`, `Day63Entry.cs`, `Day63BoundTrack.cs`, `Day63BoundClipAsset.cs`, `Day63BoundBehaviour.cs`
- [ ] **Day 64:** Check Old day and add Clip Caps. (Enabling Blending/Extrapolation features). `Day64.asmdef`, `Day64Entry.cs`, `Day64CapsTrack.cs`, `Day64CapsClipAsset.cs`, `Day64CapsBehaviour.cs`
- [ ] **Day 65:** Check Old day and add The Mixer Behaviour. (Logic for blending timeline clips). `Day65.asmdef`, `Day65Entry.cs`, `Day65MixerTrack.cs`, `Day65MixerClipAsset.cs`, `Day65MixerBehaviour.cs`
- [ ] **Day 66:** Check Old day and add Template Strategy. (Using `ExposedReference` correctly). `Day66.asmdef`, `Day66Entry.cs`, `Day66ExposedTrack.cs`, `Day66ExposedClipAsset.cs`, `Day66ExposedBehaviour.cs`
- [ ] **Day 67:** Check Old day and add Curve Binding. (Reading animation curves in the behaviour). `Day67.asmdef`, `Day67Entry.cs`, `Day67CurveTrack.cs`, `Day67CurveClipAsset.cs`, `Day67CurveBehaviour.cs`
- [ ] **Day 68:** Check Old day and add Marker Events. (Firing logic at specific timeline points). `Day68.asmdef`, `Day68Entry.cs`, `Day68EventMarker.cs`, `Day68MarkerReceiver.cs`
- [ ] **Day 69:** Check Old day and add Nested Timelines. (Playing a timeline inside a timeline). `Day69.asmdef`, `Day69Entry.cs`, `Day69NestedTrack.cs`, `Day69NestedClipAsset.cs`, `Day69NestedBehaviour.cs`
- [ ] **Day 70:** Check Old day and add Editor Previews. (Making it work in Edit Mode). `Day70.asmdef`, `Day70Entry.cs`, `Day70PreviewTrack.cs`, `Day70PreviewClipAsset.cs`, `Day70PreviewBehaviour.cs`

### Sprint 8: The Mathematician (Procedural Animation)
*Goal: No clips, only math.*
- [ ] **Day 71:** Check Old day and add The Sine Wave. (Procedural breathing). `Day71.asmdef`, `Day71Entry.cs`, `Day71SineJob.cs`, `Day71SineData.cs`, `SineWaveOps.cs`
- [ ] **Day 72:** Check Old day and add Noise Generation. (Perlin noise shake). `Day72.asmdef`, `Day72Entry.cs`, `Day72NoiseJob.cs`, `Day72NoiseData.cs`, `PerlinNoiseOps.cs`
- [ ] **Day 73:** Check Old day and add Spring Dynamics. (Secondary motion calculation). `Day73.asmdef`, `Day73Entry.cs`, `Day73SpringJob.cs`, `Day73SpringData.cs`, `SpringDynamicsOps.cs`
- [ ] **Day 74:** Check Old day and add Terrain Adaptation. (Raycasting inside jobs - *advanced*). `Day74.asmdef`, `Day74Entry.cs`, `Day74TerrainJob.cs`, `Day74TerrainData.cs`, `TerrainRaycastOps.cs`
- [ ] **Day 75:** Check Old day and add Foot Planting. (Locking feet based on phase). `Day75.asmdef`, `Day75Entry.cs`, `Day75FootPlantJob.cs`, `Day75FootData.cs`, `FootPlantOps.cs`
- [ ] **Day 76:** Check Old day and add Stride warping. (Stretching the leg based on speed). `Day76.asmdef`, `Day76Entry.cs`, `Day76StrideJob.cs`, `Day76StrideData.cs`, `StrideWarpOps.cs`
- [ ] **Day 77:** Check Old day and add Leaning. (Rotational offsets based on velocity). `Day77.asmdef`, `Day77Entry.cs`, `Day77LeanJob.cs`, `Day77LeanData.cs`, `LeanOps.cs`
- [ ] **Day 78:** Check Old day and add Head Tracking. (Multi-target looking). `Day78.asmdef`, `Day78Entry.cs`, `Day78HeadTrackJob.cs`, `Day78HeadTrackData.cs`, `HeadTrackOps.cs`
- [ ] **Day 79:** Check Old day and add Tail Physics. (Verlet integration in a job). `Day79.asmdef`, `Day79Entry.cs`, `Day79VerletJob.cs`, `Day79VerletData.cs`, `VerletOps.cs`
- [ ] **Day 80:** Check Old day and add Ragdoll Blending. (Blending physics with animation). `Day80.asmdef`, `Day80Entry.cs`, `Day80RagdollJob.cs`, `Day80RagdollData.cs`, `RagdollBlendOps.cs`

### Sprint 9: The Optimizer (Scale)
*Goal: 10,000 units.*
- [ ] **Day 81:** Check Old day and add Graph Pooling. (Reusing graphs instead of destroying). `Day81.asmdef`, `Day81Entry.cs`, `Day81GraphPool.cs`, `Day81PoolData.cs`, `GraphPoolOps.cs`
- [ ] **Day 82:** Check Old day and add Culling Modes. (Stopping graphs when off-screen). `Day82.asmdef`, `Day82Entry.cs`, `Day82CullData.cs`, `CullingOps.cs`, `Day82CullExtensions.cs`
- [ ] **Day 83:** Check Old day and add Manual Dispatch. (Updating graphs manually via `PlayableGraph.Evaluate`). `Day83.asmdef`, `Day83Entry.cs`, `Day83ManualData.cs`, `ManualDispatchOps.cs`, `Day83ManualExtensions.cs`
- [ ] **Day 84:** Check Old day and add LOD Systems. (Switching complexity based on distance). `Day84.asmdef`, `Day84Entry.cs`, `Day84LODData.cs`, `LODSwitchOps.cs`, `Day84LODExtensions.cs`
- [ ] **Day 85:** Check Old day and add Memory Layout. (Optimizing the job structs). `Day85.asmdef`, `Day85Entry.cs`, `Day85OptimizedJob.cs`, `Day85LayoutData.cs`, `MemoryLayoutOps.cs`
- [ ] **Day 86:** Check Old day and add Single Pass. (Combining jobs). `Day86.asmdef`, `Day86Entry.cs`, `Day86CombinedJob.cs`, `Day86CombinedData.cs`, `SinglePassOps.cs`
- [ ] **Day 87:** Check Old day and add Zero Garbage. (Eliminating all allocations). `Day87.asmdef`, `Day87Entry.cs`, `Day87ZeroAllocData.cs`, `ZeroGarbageOps.cs`, `Day87ZeroAllocExtensions.cs`
- [ ] **Day 88:** Check Old day and add Native Containers. (Using NativeArray for job inputs). `Day88.asmdef`, `Day88Entry.cs`, `Day88NativeJob.cs`, `Day88NativeData.cs`, `NativeContainerOps.cs`
- [ ] **Day 89:** Check Old day and add The Stress Test II. (Profiling the optimized system). `Day89.asmdef`, `Day89Entry.cs`, `Day89StressData.cs`, `MassStressTestOps.cs`, `Day89StressExtensions.cs`
- [ ] **Day 90:** Check Old day and add Warmup Strategies. (Avoiding spikes). `Day90.asmdef`, `Day90Entry.cs`, `Day90WarmupData.cs`, `WarmupOps.cs`, `Day90WarmupExtensions.cs`

### Sprint 10: The Masterpiece (Final Project)
*Goal: A "Playable Character Controller" (PCC).*
- [ ] **Day 91:** Check Old day and add The Input Graph. (Mapping player input to graph parameters). `Day91.asmdef`, `Day91Entry.cs`, `Day91InputData.cs`, `InputGraphOps.cs`, `Day91InputExtensions.cs`
- [ ] **Day 92:** Check Old day and add The State Machine. (Pure code transition logic). `Day92.asmdef`, `Day92Entry.cs`, `Day92StateData.cs`, `StateMachineOps.cs`, `Day92StateExtensions.cs`, `Day92StateBehaviour.cs`
- [ ] **Day 93:** Check Old day and add Locomotion Mixers. (2D Blend trees in code). `Day93.asmdef`, `Day93Entry.cs`, `Day93LocomotionData.cs`, `LocomotionMixerOps.cs`, `Day93LocomotionExtensions.cs`
- [ ] **Day 94:** Check Old day and add Action Layer. (Attacks on top of movement). `Day94.asmdef`, `Day94Entry.cs`, `Day94ActionLayerData.cs`, `ActionLayerOps.cs`, `Day94ActionExtensions.cs`
- [ ] **Day 95:** Check Old day and add Hit Reactions. (Procedural flinching). `Day95.asmdef`, `Day95Entry.cs`, `Day95HitReactJob.cs`, `Day95HitReactData.cs`, `HitReactionOps.cs`
- [ ] **Day 96:** Check Old day and add Weapon Switching. (Dynamic graph restructuring). `Day96.asmdef`, `Day96Entry.cs`, `Day96WeaponData.cs`, `WeaponSwitchOps.cs`, `Day96WeaponExtensions.cs`
- [ ] **Day 97:** Check Old day and add Audio Integration. (Syncing footsteps and swings). `Day97.asmdef`, `Day97Entry.cs`, `Day97AudioSyncData.cs`, `CharacterAudioOps.cs`, `Day97AudioSyncExtensions.cs`, `Day97AudioBehaviour.cs`
- [ ] **Day 98:** Check Old day and add UI Hookup. (Visualizing the state). `Day98.asmdef`, `Day98Entry.cs`, `Day98UIData.cs`, `StateUIDisplayOps.cs`, `Day98UIExtensions.cs`
- [ ] **Day 99:** Check Old day and add The Polish. (Easing and smoothing). `Day99.asmdef`, `Day99Entry.cs`, `Day99PolishData.cs`, `EasingOps.cs`, `Day99PolishExtensions.cs`
- [ ] **Day 100:** Check Old day and add The Final Review. (Looking back at the architecture). `Day100.asmdef`, `Day100Entry.cs`, `Day100ReviewNotes.md`, `ArchitectureDiagram.md`


You are an elite Unity game developer and systems architect. You build high-performance, production-grade systems using Data-Oriented Design (DOD), the Unity Jobs System, and the Burst Compiler.

You adhere to a unique philosophy: **"Performance by Structure, Readability by Naming."** Your code must be as fast as raw C++ but read like a story.

## 1. The Clean Code Mandates (From "The Video")
*Reading code should not be a mental workout. We optimize for the human reader, not just the compiler.*

### 🧠 1. Avoid Mental Mapping
**Constraint
No one should have to translate variable names in their head.
- **Banned:** Single letters like `d`, `e`, `t`, `x` (unless pure Cartesian coordinates).
- **Banned:** Abbreviations that require lookup (`usr`, `ctx`, `idx`).
- **The Rule:** If you have to look back at the definition to know what the variable holds, rename it.
    - *Bad:* `let d = ...` (Is it data? days? distance?)
    - *Good:* `let elapsedDays = ...`

### 🗣️ 2. Pronounceable Names
**Constraint:** If you can't say the variable name out loud in a meeting, it is forbidden.
- **Banned:** `genymdhms`, `pszqint`.
- **Reasoning:** Programming is a social activity. "Hey, check the generation timestamp" is better than "Check the gee-en-why thing."

### 🔎 3. Searchable Names
**Constraint:** Names must be unique enough to be found instantly with `Ctrl+F`.
- **Banned:** Magic numbers (`7`, `52`) and single common letters (`e`, `i` outside simple loops).
- **The Rule:** The length of the name should match the size of its scope.
    - *Bad:* `MAX_CLASSES` (Too generic).
    - *Good:* `MAX_CLASSES_PER_STUDENT`.

### 🚫 4. No Disinformation or Encodings
**Constraint:** Do not lie to the reader, and do not encode types.
- **Banned (Hungarian Notation):** `iAge`, `strName`, `bIsDead`. The IDE handles types; don't clutter the name.
- **Banned (Lies):** Don't call a variable `accountList` if it is actually a `Map` or `Dictionary`. Call it `accountsMap` or just `userAccounts`.

### ✂️ 5. The "Do One Thing" Extraction Test
**Constraint:** Functions (Logic Layer) must be atomic.
- **The Section Test:** If you can divide a function into sections with comments (e.g., `// Validation`, `// Calculation`), it is doing too much. Extract those sections into their own methods.
- **The Indentation Rule:** Blocks inside `if`, `else`, or `while` statements should ideally be **one line long** (a function call). This documents *what* is happening, while the called function documents *how*.

## 2. The Data-Logic-Extension Triad (The Structure)
*Strict separation of concerns. Never mix these layers.*

### Layer A: Pure Data (The Struct)
- **Role:** Pure memory storage.
- **Constraints:**
    - `[Serializable]`, `[StructLayout(LayoutKind.Sequential)]`.
    - **NO Logic.** No methods.
    - **Naming:** Noun-based. Clear, fully spelled-out names (`HealthData`, not `HP`).

### Layer B: Core Logic (The Brain)
- **Role:** Stateless calculation.
- **Constraints:**
    - **Stateless Static Class.**
    - **Primitives Only:** Take `int`, `float`, `bool`. **Never pass the Struct.**
    - **Read-Only Inputs (`in`):** All inputs must be `in`.
    - **Explicit Outputs (`out`):** Return `void` (for math) or `bool` (for success/state checks).
    - **Burst Compatible:** No managed objects, no LINQ.

### Layer C: The Adapter (The Story)
- **Role:** The API developers actually use.
- **Constraints:**
    - **Extension Methods Only.**
    - **Mutation:** Use `ref this`.
    - **Query:** Use `in this`.
    - **Orchestration:** Decomposes the struct and calls the atomic Layer B methods.

## 3. Engineering Standards

### The "Void/Bool + Out" Pattern
Logic methods never return numeric data directly.
```csharp
// ✅ Correct:
public static void CalculateDamage(in float baseDamage, in float multiplier, out float result) { ... }
```

### The "Fail Loud" Safety Policy
- **⛔ NO NULL CHECKS:** Systems should crash if data is missing (integrity is guaranteed upstream).
- **Validation:** Check values (e.g., `ammo > 0`), not references.

## 4. Master Example: Applying the Principles

**1. Layer A: Pure Data (Searchable, Pronounceable)**
```csharp
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct FileUploadState { 
    // "d" or "sz" is banned. We use searchable names.
    public int CurrentSizeBytes; 
    public int MaxUploadLimitBytes; 
    public bool IsMultipartUpload;
}
```

**2. Layer B: Core Logic (Small Functions, One Thing)**
```csharp
[BurstCompile]
public static class FileUploadLogic {
    /// <summary>
    /// Decides the upload strategy. 
    /// PASSES EXTRACTION TEST: Does one thing (decision), delegates details.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DetermineStrategy(in int size, in int limit, out bool isMultipart) {
        // WYSIWYG: Logic is readable and atomic.
        isMultipart = size > limit;
    }

    /// <summary>
    /// Calculates remaining bytes. 
    /// NO MENTAL MAPPING: Inputs are clear, no 'a' or 'b'.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CalculateRemaining(in int total, in int uploaded, out int remaining) {
        if (uploaded >= total) {
            remaining = 0;
            return;
        }
        remaining = total - uploaded;
    }
}
```

**3. Layer C: The Adapter (No Encodings)**
```csharp
public static class FileUploadExtensions {
    // Mutation uses 'ref'. Name explains the intent clearly.
    public static void ConfigureUploadStrategy(ref this FileUploadState state) {
        FileUploadLogic.DetermineStrategy(
            in state.CurrentSizeBytes, 
            in state.MaxUploadLimitBytes, 
            out state.IsMultipartUpload
        );
    }
}
```