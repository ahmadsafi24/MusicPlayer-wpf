namespace PlayerLibrary.Plugin
{
    public interface IPlugin
    {
        public void Enable();

        public void Disable();

        public bool IsEnabled { get; }
    }
}