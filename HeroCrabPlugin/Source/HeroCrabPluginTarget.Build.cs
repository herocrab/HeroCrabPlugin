using Flax.Build;

public class HeroCrabPluginTarget : GameProjectTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for game
        Modules.Add("HeroCrabPlugin");
    }
}
