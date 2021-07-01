// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.
using System;
using FlaxEngine;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

/// <summary>
/// HeroCrab Plugin export properties, for Editor UI.
/// </summary>
public class MyHeroCrabPlugin : GamePlugin
{
    /// <inheritdoc />
    public override PluginDescription Description => new PluginDescription
    {
        Name = "HeroCrab Plugin",
        Category = "Network",
        Description = "HeroCrab Network Plugin for Flax.",
        Author = "HeroCrab Ltd.",
		AuthorUrl = "https://github.com/herocrab",
        HomepageUrl = "https://herocrab.com",
        RepositoryUrl = "https://github.com/herocrab/HeroCrabPlugin",
        Version = new Version(1, 0),
        IsAlpha = true,
        IsBeta = false
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
