using System.IO;
using Flax.Build;
using Flax.Build.NativeCpp;

public class HeroCrabPlugin : GameModule
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

		// C#-only scripting
		BuildNativeCode = false;
    }

    /// <inheritdoc />
    public override void Setup(BuildOptions options)
    {
        base.Setup(options);

        // Here you can modify the build options for your game module
        // To reference another module use: options.PublicDependencies.Add("Audio");
        // To add C++ define use: options.PublicDefinitions.Add("COMPILE_WITH_FLAX");
        // To learn more see scripting documentation.
        options.DependencyFiles.Add(Path.Combine(FolderPath, "..", "..", "Lib", "enet.dll"));
        options.ScriptingAPI.FileReferences.Add(Path.Combine(FolderPath, "..", "..", "Lib", "ENet-CSharp.dll"));
    }
}
