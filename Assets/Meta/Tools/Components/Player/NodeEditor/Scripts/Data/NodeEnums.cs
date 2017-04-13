namespace Meta.Tools
{
    public enum NodeType
    {
        //System
        MonoBehaviourLifeCycle = 0,
        //DataTypes
        Float = 20,
        //Arithmetic Operators
        Add = 40,
        //Meta MonoBehaviours
        Timeline = 60
    }

    public enum IOType
    {
        //Actions
        Action = 0,
        //Values
        Numeric = 50, Float, Int, Bool,
    }
}