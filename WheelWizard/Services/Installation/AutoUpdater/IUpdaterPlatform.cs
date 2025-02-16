﻿using Semver;
using System.Threading.Tasks;
using WheelWizard.Models.Github;

namespace WheelWizard.Services.Installation;

public interface IUpdaterPlatform
{
    GithubAsset? GetAssetForCurrentPlatform(GithubRelease release);
    Task ExecuteUpdateAsync(string downloadUrl);
}
