public class MyHeroCrabPlugin : GamePlugin
{
    /// <inheritdoc />
    public override PluginDescription Description => new PluginDescription
    {
        Name = "HeroCrab Plugin",
        Category = "Network",
        Description = "HeroCrab Network Plugin for Flax.",
        Author = "Someone Inc.",
		AuthorUrl = "https://github.com/herocrab",
        HomepageUrl = "https://herocrab.com",
        RepositoryUrl = "https://github.com/herocrab/HeroCrabPlugin",
        Version = new Version(1, 0),
        IsAlpha = true,
        IsBeta = false,
    };

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        Debug.Log("HeroCrabPlugin initialization!");
    }

    /// <inheritdoc />
    public override void Deinitialize()
    {
        Debug.Log("HeroCrabPlugin cleanup!");

        base.Deinitialize();
    }
}