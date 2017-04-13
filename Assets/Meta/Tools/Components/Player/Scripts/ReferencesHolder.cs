using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReferencesHolder
{
    #region Scene Objects References
    [SerializeField]
    private List<Component> _components = new List<Component>();
    [SerializeField]
    private List<int> _componentsIndexes = new List<int>();
    #endregion

    public int Add(Component component)
    {
        if (_components.Contains(component))
        {
            return _componentsIndexes[_components.IndexOf(component)];
        }

        _components.Add(component);

        int id = (component.GetInstanceID() + component.GetHashCode()).GetHashCode();

        _componentsIndexes.Add(id);

        return id;
    }

    public Component Get(int id)
    {
        if (!_componentsIndexes.Contains(id))
        {
            return null;
        }

        return _components[_componentsIndexes.IndexOf(id)];
    }
}
