#region Using

using System.IO;
using System.Linq;
using System.Text;
using Milan.FrontEnd.Core.v5_1_1.Meta;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

#endregion

#region CreateTemplateScript

/// <summary>
/// Creates and displays to the user the new template script.
/// </summary>
public class CreateTemplateScript : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        Object obj = TemplateScripts.CreateScript(pathName, resourceFile);
        ProjectWindowUtil.ShowCreatedAsset(obj);
    }
}

#endregion

/// <summary>
/// Editor class for creating code files from templates.
/// </summary>
public class TemplateScripts
{
    /// <summary>
    /// The C# script icon.
    /// </summary>
    private static Texture2D scriptIcon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);

    /// <summary>
    /// Creates the script.
    /// </summary>
    /// <param name="pathName"></param>
    /// <param name="templatePath"></param>
    /// <returns></returns>
    internal static Object CreateScript(string pathName, string templatePath)
    {
        string newFilePath = Path.GetFullPath(pathName);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
        string className = NormalizeClassName(fileNameWithoutExtension);

        string templateText = string.Empty;

        if (File.Exists(templatePath))
        {
            using (var sr = new StreamReader(templatePath))
            {
                templateText = sr.ReadToEnd();
            }

            // Get custom class name.
            templateText = templateText.Replace("##NAME##", className);
            // Replace the generic ##GAMEID## with the game ID for this game.
            templateText = templateText.Replace("##GAMEID##", GetGameId());

            UTF8Encoding encoding = new UTF8Encoding(true, false);

            using (var sw = new StreamWriter(newFilePath, false, encoding))
            {
                sw.Write(templateText);
            }

            AssetDatabase.ImportAsset(pathName);

            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }
        else
        {
            Debug.LogError(string.Format("The template file was not found: {0}", templatePath));

            return null;
        }
    }

    /// <summary>
    /// Any normalization that needs to happen.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private static string NormalizeClassName(string fileName)
    {
        return fileName.Replace(" ", string.Empty);
    }

    /// <summary>
    /// Creates a new code file from a template file.
    /// </summary>
    /// <param name="initialName">The initial name to give the file in the UI</param>
    /// <param name="templatePath">The full path of the template file to use</param>
    public static void CreateFromTemplate(string initialName, string templatePath)
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<CreateTemplateScript>(),
            initialName,
            scriptIcon,
            templatePath);
    }

    /// <summary>
    /// Finds and returns the GameId, such as "N5024".
    /// </summary>
    /// <returns></returns>
    public static string GetGameId()
    {
        var meta = GetMetaCapabilities();

        return meta.AppID;
    }

    /// <summary>
    /// Finds and returns the MetaCapabilities file for this game.
    /// </summary>
    /// <returns></returns>
    private static MetaCapabilities GetMetaCapabilities()
    {
        // Find all meta capability files.
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(MetaCapabilities).Name);

        // Since there should only ever be one per project, just grab the first one and get it's path.
        var path = AssetDatabase.GUIDToAssetPath(guids[0]);

        // Return the found metacapabilities file.
        return AssetDatabase.LoadAssetAtPath<MetaCapabilities>(path);
    }
}
