## File: [Assets/Day08/Day08Timeline.playable](Assets/Day08/Day08Timeline.playable)

```plantuml
@startuml

concise "Tween Health Track" as T01
concise "Activation Track" as T02
concise "Tween Health Track" as T03
concise "Activation Track" as T04

@0
@677

@T01
102 is "Tween Health"
@334 <-> @402 : Blend
334 is "Tween Health"
634 is {hidden}

@T02
78 is "Active"
378 is {hidden}

@T03
0 is "Tween Health"
300 is {hidden}
378 is "Tween Health"
678 is {hidden}

@T04
78 is "Active"
378 is {hidden}

@enduml
```

[![](https://img.plantuml.biz/plantuml/svg/ZP7H2i8W58Rl1T_XiFl0fcmH4TPL3x0N47TWqd8mhIwYT-_a1h4Qj0j1Zf_V_uiwHDF7wwLbp7RUke2GLNT43oSqRMoWweqzPs02L6BDFz31HdSpqNL-WkHyWKV-zoZEEDE2Lb6MQP_Yro875wRgZ0wbLB1RxK4h0hPmRD7NdBsdCtGnZ1-Dgslqpz5EpSjDmbCb950EWo-GseivhHHYIe_NV-9dhMf1FDFqBFgh5m00)](https://editor.plantuml.com/uml/ZP7H2i8W58Rl1T_XiFl0fcmH4TPL3x0N47TWqd8mhIwYT-_a1h4Qj0j1Zf_V_uiwHDF7wwLbp7RUke2GLNT43oSqRMoWweqzPs02L6BDFz31HdSpqNL-WkHyWKV-zoZEEDE2Lb6MQP_Yro875wRgZ0wbLB1RxK4h0hPmRD7NdBsdCtGnZ1-Dgslqpz5EpSjDmbCb950EWo-GseivhHHYIe_NV-9dhMf1FDFqBFgh5m00)