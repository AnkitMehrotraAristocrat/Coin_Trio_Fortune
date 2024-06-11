using UnityEditor;

/// <summary>
/// Displays the right-click context menu for the template script items.
///
/// To add a new template script, simply create a text file that represents the template script you want and then make a new
/// static method with a unique menu item that points to the new text template script.
/// 
/// </summary>
public class TemplateScriptMenuItems
{
    private const string _menuItemPath = "Assets/Create/NMG/Script Templates/";
    private const int _menuItemPriority = 0;

    [MenuItem(_menuItemPath + "State Presenter Template", false, _menuItemPriority)]
    private static void CreateStatePresenter()
    {
        TemplateScripts.CreateFromTemplate("TemplateStatePresenter.cs",
            @"Assets/" + TemplateScripts.GetGameId() + "/Scripts/Editor/TemplateScripts/Scripts/StatePresenterTemplate.txt");
    }

    [MenuItem(_menuItemPath + "State Trigger Template", false, _menuItemPriority)]
    private static void CreateTrigger()
    {
        TemplateScripts.CreateFromTemplate("TemplateTrigger.cs",
            @"Assets/" + TemplateScripts.GetGameId() + "/Scripts/Editor/TemplateScripts/Scripts/TriggerTemplate.txt");
    }

    [MenuItem(_menuItemPath + "Driver Template", false, _menuItemPriority)]
    private static void CreateDriver()
    {
        TemplateScripts.CreateFromTemplate("TemplateDriver.cs",
            @"Assets/" + TemplateScripts.GetGameId() + "/Scripts/Editor/TemplateScripts/Scripts/DriverTemplate.txt");
    }

    [MenuItem(_menuItemPath + "Model Template", false, _menuItemPriority)]
    private static void CreateModel()
    {
        TemplateScripts.CreateFromTemplate("TemplateModel.cs",
            @"Assets/" + TemplateScripts.GetGameId() + "/Scripts/Editor/TemplateScripts/Scripts/ModelTemplate.txt");
    }

    [MenuItem(_menuItemPath + "Payload Data Template", false, _menuItemPriority)]
    private static void CreatePayloadData()
    {
        TemplateScripts.CreateFromTemplate("TemplatePayloadData.cs",
            @"Assets/" + TemplateScripts.GetGameId() + "/Scripts/Editor/TemplateScripts/Scripts/PayloadDataTemplate.txt");
    }
}
