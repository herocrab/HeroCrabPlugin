using Flax.Build;
// ReSharper disable CheckNamespace

/// <summary>
/// HeroCrabPluginTestsIntegration project build configuration.
/// </summary>
public class HeroCrabPluginTestsIntegrationTarget : Target
{
    /// <inheritdoc />
    public HeroCrabPluginTestsIntegrationTarget()
    {
        Name = ProjectName = OutputName = "HeroCrabPluginTestsIntegration";
    }

    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        Type = TargetType.DotNet;
        OutputType = TargetOutputType.Library;
        Platforms = new[]
        {
            TargetPlatform.Windows,
        };
        Configurations = new[]
        {
            TargetConfiguration.Debug,
			TargetConfiguration.Development,
            TargetConfiguration.Release,
        };
        CustomExternalProjectFilePath = System.IO.Path.Combine(FolderPath, "HeroCrabPluginTestsIntegration.csproj");
    }
}
