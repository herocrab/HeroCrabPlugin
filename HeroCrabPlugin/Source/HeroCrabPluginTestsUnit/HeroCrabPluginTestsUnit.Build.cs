using Flax.Build;

// ReSharper disable CheckNamespace

/// <summary>
/// HeroCrabPluginTestsUnit project build configuration.
/// </summary>
public class HeroCrabPluginTestsUnitTarget : Target
{
    /// <inheritdoc />
    public HeroCrabPluginTestsUnitTarget()
    {
        Name = ProjectName = OutputName = "HeroCrabPluginTestsUnit";
    }

    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        Type = TargetType.DotNet;
        OutputType = TargetOutputType.Library;
        Platforms = new[]
        {
            TargetPlatform.Windows
        };
        Configurations = new[]
        {
            TargetConfiguration.Debug,
            TargetConfiguration.Development,
            TargetConfiguration.Release
        };
        CustomExternalProjectFilePath = System.IO.Path.Combine(FolderPath, "HeroCrabPluginTestsUnit.csproj");
    }
}
