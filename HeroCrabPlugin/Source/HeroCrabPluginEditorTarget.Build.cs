using Flax.Build;

public class HeroCrabPluginEditorTarget : GameProjectEditorTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for editor
        Modules.Add("HeroCrabPlugin");
    }
}
