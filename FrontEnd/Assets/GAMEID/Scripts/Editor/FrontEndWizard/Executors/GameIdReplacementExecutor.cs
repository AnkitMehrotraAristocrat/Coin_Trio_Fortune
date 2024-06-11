using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
    /// <summary>
    /// Must execute after any executers that reference GAMEID
    /// </summary>
    public class GameIdReplacementExecutor : BaseWizardExecutor
    {
        public GameIdReplacementExecutor()
        {
            _canRerun = false;
        }

        public override void Execute(WizardInputData data)
        {
            Debug.Log("Game Id Replacement");
            RenameAssets(data.GameId);
            RenamePackages(data.GameId);

            AssetDatabase.Refresh();

            FrontEndWizardHelper.LoadSlotScene(OpenSceneMode.Single);
            FrontEndWizardHelper.LoadScene("LocalTestHarness", OpenSceneMode.Additive);
        }
   
        private void RenameAssets(string gameId)
        {
            RenameScene(gameId);
            RenameLuaAssemblyTargets(gameId);
            RenameFiles(gameId, "Assets/");
            RenameAssetGameFolder(gameId);
        }

        private void RenamePackages(string gameId)
        {
            RenameAssemblyFile(gameId);  
            RenameFiles(gameId, "Packages/com.productmadness.wildcat.slots.gameId/");
        }

        private void RenameScene(string gameId)
        {
            FrontEndWizardHelper.CloseAllScenes();
            SceneAsset sceneAsset = FrontEndWizardHelper.GetSlotSceneAsset();
            var path = AssetDatabase.GetAssetPath(sceneAsset);
            AssetDatabase.RenameAsset(path, gameId);
        }

        private void RenameAssetGameFolder(string gameId)
        {
            AssetDatabase.MoveAsset("Assets/GAMEID", "Assets/" + gameId);
        }

        private void RenameLuaAssemblyTargets(string gameId)
        {
            // TODO: Lua replacement
            // var luaTargets = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(LuaAssemblyTargets)));
            //
            // if (luaTargets.Length > 0)
            // {
            //     var path = AssetDatabase.GUIDToAssetPath(luaTargets[0]);
            //     LuaAssemblyTargets luaTargetAsset = AssetDatabase.LoadAssetAtPath<LuaAssemblyTargets>(path);
            //
            //     var assemblyTarget = new AssemblyScriptingTargets();
            //     assemblyTarget.assemblyName = "Assembly" + gameId;
            //     luaTargetAsset.assemblyScriptingTargets[0] = assemblyTarget;
            //
            //     AssetDatabase.RenameAsset(path, gameId + "LuaScriptingTarget");
            // }
        }

        private void RenameFiles(string gameId, string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string file in fileEntries)
            {
                if(!file.Contains("GameIdReplacementExecutor"))
                {
                    string text = File.ReadAllText(file);
                    if (text.Contains("GAMEID"))
                    {
                        text = text.Replace("GAMEID", gameId);
                        File.WriteAllText(file, text);
                    }

                    if (text.Contains("gameid"))
                    {
                        text = text.Replace("gameid", gameId.ToLower());
                        File.WriteAllText(file, text);
                    }
                }
            }

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach(string subdirectoryEntry in subdirectoryEntries)
            {
                RenameFiles(gameId, subdirectoryEntry);
            }
        }

        private void RenameAssemblyFile(string gameId)
        {
            var path = "Packages/com.productmadness.wildcat.slots.gameId/Runtime/";
            var oldAssemblyName = "AssemblyGAMEID";
            var newAssemblyName = "Assembly" + gameId;
            var assemblyPostFix = ".asmdef";

            File.Move(path + oldAssemblyName + assemblyPostFix, 
                path + newAssemblyName + assemblyPostFix);
        }
    }
}
