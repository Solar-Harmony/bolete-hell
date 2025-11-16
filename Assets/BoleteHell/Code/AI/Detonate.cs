using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Detonate")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Detonate", message: "Start detonation", category: "Events", id: "0d69474d8339cc09aa939cd0b4194e69")]
public sealed partial class Detonate : EventChannel { }

