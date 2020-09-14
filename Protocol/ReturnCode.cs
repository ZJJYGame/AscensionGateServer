namespace Protocol
{
    public enum GateReturnCode : short
    {
        Success = 0,
        Error = 1,
        Fail = 2,
        Empty = 3,
        InvalidOperationParameter = 4,
        InvalidOperation = 5,
        ItemAlreadyExists=6,
        ItemNotFound=7,
    }
}
