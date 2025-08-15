public class PubTool
{
    /// <summary>隐藏工具栏中新建,删除,保存工具</summary>
    public static void HiddenToolbars(EpiBaseForm form, string[] tools = null)
    {
        if (tools == null)
        {
            tools = new string[] { "NewTool", "NewMenuTool", "DeleteTool", "SaveTool" };
        }
        UltraToolbarsManager baseToolbarsManager = (UltraToolbarsManager)(form.GetType().InvokeMember("baseToolbarsManager", System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, form, null));
        for (var i = 0; i < tools.Length; i++)
        {
            baseToolbarsManager.Tools[tools[i]].SharedProps.Visible = false;
            baseToolbarsManager.Tools[tools[i]].SharedProps.Enabled = false;
        }
    }
    /// <summary>隐藏工具栏中新建,删除,保存工具</summary>
    public static void HiddenToolbars(Ice.Lib.Customization.CustomScriptManager csm, string[] tools = null)
    {
        if (tools == null)
        {
            tools = new string[] { "NewTool", "NewMenuTool", "DeleteTool", "SaveTool" };
        }
        UltraToolbarsManager baseToolbarsManager = ((UltraToolbarsManager)(csm.GetGlobalInstance("baseToolbarsManager")));

        for (var i = 0; i < tools.Length; i++)
        {
            baseToolbarsManager.Tools[tools[i]].SharedProps.Visible = false;
            baseToolbarsManager.Tools[tools[i]].SharedProps.Enabled = false;
        }
    }
    public static ButtonTool GetNewButtonTool(string key, string caption, string imageKey, ToolClickEventHandler btToolToolClick, ToolDisplayStyle displayStyle = ToolDisplayStyle.ImageOnlyOnToolbars)
    {
        ButtonTool btTool = new ButtonTool(key);
        btTool.SharedProps.Caption = caption;
        btTool.SharedProps.DisplayStyle = displayStyle;
        if (!string.IsNullOrEmpty(imageKey)) EpiBaseForm.setImageOnTool(btTool, imageKey);
        btTool.ToolClick += btToolToolClick;
        return btTool;
    }

    public static PopupMenuTool GetPopupMenuTool(string key, string caption, string imageKey)
    {
        PopupMenuTool puTool = new PopupMenuTool(key);
        puTool.SharedProps.Caption = caption;
        puTool.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
        if (!string.IsNullOrEmpty(imageKey)) EpiBaseForm.setImageOnTool(puTool, imageKey);
        return puTool;
    }
    public static void ChangeTool(Ice.Lib.Customization.CustomScriptManager csm, string toolKey, ButtonTool btTool)
    {
        UltraToolbarsManager baseToolbarsManager = ((UltraToolbarsManager)(csm.GetGlobalInstance("baseToolbarsManager")));
        baseToolbarsManager.Tools.Add(btTool);
        foreach (var toolbar in baseToolbarsManager.Toolbars)
        {
            for (int i = 0; i < toolbar.Tools.Count; i++)
            {
                if (toolbar.Tools[i].Key == toolKey)
                {
                    toolbar.Tools[i].SharedProps.Visible = false;
                    toolbar.Tools.InsertTool(i, btTool.Key);
                    return;
                }
            }
        }
    }
    public static void AddTool(Ice.Lib.Customization.CustomScriptManager csm, string toolKey, ButtonTool btTool,bool afterTool=true)
    {
        UltraToolbarsManager baseToolbarsManager = ((UltraToolbarsManager)(csm.GetGlobalInstance("baseToolbarsManager")));
        baseToolbarsManager.Tools.Add(btTool);
        foreach (var toolbar in baseToolbarsManager.Toolbars)
        {
            for (int i = 0; i < toolbar.Tools.Count; i++)
            {
                if (toolbar.Tools[i].Key == toolKey)
                {
                    if(afterTool) toolbar.Tools.InsertTool(i+1, btTool.Key);
                    else toolbar.Tools.InsertTool(i, btTool.Key);
                    return;
                }
            }
        }
    }

    public static UltraToolbar GetStandardToolbars(Ice.Lib.Customization.CustomScriptManager csm)
    {
        UltraToolbarsManager baseToolbarsManager = ((UltraToolbarsManager)(csm.GetGlobalInstance("baseToolbarsManager")));
        return baseToolbarsManager.Toolbars["Standard Tools"];
    }

    /// <summary>可用于ContextMenu删除Tools</summary>
    public static void RemoveTool(List<string> keyArray, PopupMenuTool popupMenu)
    {
        for (int i = popupMenu.Tools.Count - 1; i >= 0; i--)
        {
            ToolBase tool = popupMenu.Tools[i];
            if (keyArray.Contains(tool.Key))
            {
                popupMenu.Tools.Remove(tool);
            }
        }
    }

    /// <summary>可用于ContextMenu保留Tools</summary>
    public static void RemainTool(List<string> keyArray, PopupMenuTool popupMenu)
    {
        for (int i = popupMenu.Tools.Count - 1; i >= 0; i--)
        {
            ToolBase tool = popupMenu.Tools[i];
            if (!keyArray.Contains(tool.Key))
            {
                popupMenu.Tools.Remove(tool);
            }
        }
    }
}

