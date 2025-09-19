using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Selector", story: "Execute children until one succeeds", category: "Flow", id: "4eeeea5c5ace1a6be7eb8ad2e01b2b2c")]
public class SelectorSequence : Composite
{
    [CreateProperty] int m_CurrentChild;

    /// <inheritdoc cref="OnStart" />
    protected override Status OnStart()
    {
        m_CurrentChild = 0;
        return StartChild(m_CurrentChild);
    }

    /// <inheritdoc cref="OnUpdate" />
    protected override Status OnUpdate()
    {
        var currentChild = Children[m_CurrentChild];
        Status childStatus = currentChild.CurrentStatus;
        if (childStatus == Status.Failure)
        {
            return StartChild(++m_CurrentChild);
        }
        return childStatus == Status.Running ? Status.Waiting : childStatus;
    }

    protected Status StartChild(int childIndex)
    {
        if (m_CurrentChild >= Children.Count)
        {
            return Status.Success;
        }
        var childStatus = StartNode(Children[childIndex]);
        return childStatus switch
        {
            Status.Failure => childIndex + 1 >= Children.Count ? Status.Success : Status.Running,
            Status.Running => Status.Waiting,
            _ => childStatus
        };
    }
}

