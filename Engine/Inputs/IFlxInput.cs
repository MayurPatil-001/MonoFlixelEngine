namespace Engine.Inputs
{
    public interface IFlxInput
    {
        bool JustReleased { get; }
        bool Released { get; }
        bool Pressed { get; }
        bool JustPressed { get; }
    }
}
