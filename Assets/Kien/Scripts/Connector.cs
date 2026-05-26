using UnityEngine;
using System.Collections.Generic;

public class Connector : MonoBehaviour
{
    // ===== STATIC REGISTRY (Production Safe) =====
    public static readonly List<Connector> All = new List<Connector>();

    [Header("Direction (local space)")]
    [Tooltip("Hướng đầu ống trong local space")]
    public Vector3 localDirection = Vector3.forward;

    [HideInInspector]
    public Connector connectedTo;

    public bool IsConnected => connectedTo != null;

    // ===== REGISTER / UNREGISTER =====
    void OnEnable()
    {
        if (!All.Contains(this))
            All.Add(this);
    }

    void OnDisable()
    {
        All.Remove(this);
    }

    // ===== WORLD DIRECTION =====
    public Vector3 WorldDirection
    {
        get { return transform.TransformDirection(localDirection).normalized; }
    }

    // ===== CONNECT =====
    public void Connect(Connector other)
    {
        if (other == null || other == this) return;
        if (IsConnected || other.IsConnected) return;

        connectedTo = other;
        other.connectedTo = this;
    }

    public void ForceDisconnect()
    {
        if (connectedTo == null) return;

        Connector other = connectedTo;

        connectedTo = null;

        if (other != null && other.connectedTo == this)
            other.connectedTo = null;
    }
}