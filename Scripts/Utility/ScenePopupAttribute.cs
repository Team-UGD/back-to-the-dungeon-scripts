using System;
using UnityEngine;

/// <summary>
/// If you define it for string or integer field, you can select a scene information of scenes in build settings on inspector view.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ScenePopupAttribute : PropertyAttribute
{
    public readonly bool index;
    public readonly bool onlyEnabled;

    public ScenePopupAttribute() : this(true, true) { }

    /// <param name="index">display index - defulat: true</param>
    public ScenePopupAttribute(bool index) : this(index, true) { }

    /// <param name="index">display index - defulat: true</param>
    /// <param name="onlyEnabled">display only enabled scenes in build settings - default: true</param>
    public ScenePopupAttribute(bool index, bool onlyEnabled)
    {
        this.index = index;
        this.onlyEnabled = onlyEnabled;
    }
}
