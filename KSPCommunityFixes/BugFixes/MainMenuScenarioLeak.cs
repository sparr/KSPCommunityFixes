using System;
using UnityEngine;

namespace KSPCommunityFixes.BugFixes
{
    // ScenarioRunner adds the main menu scenarios one frame after the main menu scene is loaded, keeping
    // those whose target scenes include HighLogic.LoadedScene. If another scene was requested in between,
    // LoadedScene is already that scene, and the runner has already cleared its modules for the change.
    // Modules targeting both scenes are then created in the new scene from an empty node, alongside the
    // ones the game loads from the save. Principia aborts when a second instance of its plugin is created.
    // If this happens after the game has loaded the save, it also replaces the runner's scenario list
    // with the main menu's, and the next save keeps only the main menu scenarios.
    // Skip the main menu scenarios when the main menu is no longer the loaded scene.
    internal class MainMenuScenarioLeak : BasePatch
    {
        protected override Version VersionMin => new Version(1, 12, 0);

        protected override void ApplyPatches()
        {
            AddPatch(PatchType.Prefix, typeof(ScenarioRunner), "onLevelWasLoaded");
        }

        private static bool ScenarioRunner_onLevelWasLoaded_Prefix(GameScenes scene)
        {
            if (scene == GameScenes.MAINMENU && HighLogic.LoadedScene != GameScenes.MAINMENU)
            {
                Debug.Log($"[KSPCF:MainMenuScenarioLeak] Not adding main menu scenarios, the scene is now {HighLogic.LoadedScene}");
                return false;
            }

            return true;
        }
    }
}
