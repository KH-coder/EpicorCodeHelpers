    public class PubPane
    {
        /// <summary>设置面板顺序(Key)</summary>
        public static void SetTabIndex(Ice.Lib.Customization.CustomScriptManager csm, string parentPanelGuid, string tabKey, int index)
        {
            EpiDockManagerPanel dmp = PubFun.GetControlByGuid<EpiDockManagerPanel>(csm, parentPanelGuid);
            for (int i = 0; i < dmp.baseDockManager.DockAreas[0].Panes.Count; i++)
            {
                Infragistics.Win.UltraWinDock.DockablePaneBase pane = dmp.baseDockManager.DockAreas[0].Panes[i];
                if (pane.Key == tabKey)
                {
                    dmp.baseDockManager.DockAreas[0].Panes.RemoveAt(i);
                    dmp.baseDockManager.DockAreas[0].Panes.Insert(pane, index);
                    dmp.baseDockManager.DockAreas[0].Panes[0].Activate();
                    break;
                }
            }
        }
        public static void SetTabIndex(Ice.Lib.Customization.CustomScriptManager csm, string parentPanelGuid, int index1, int index)
        {
            EpiDockManagerPanel dmp = PubFun.GetControlByGuid<EpiDockManagerPanel>(csm, parentPanelGuid);
            Infragistics.Win.UltraWinDock.DockablePaneBase pane = dmp.baseDockManager.DockAreas[0].Panes[index1];
            dmp.baseDockManager.DockAreas[0].Panes.RemoveAt(index1);
            dmp.baseDockManager.DockAreas[0].Panes.Insert(pane, index);
            dmp.baseDockManager.DockAreas[0].Panes[0].Activate();
        }

        /// <summary>隐藏面板</summary>
        public static Infragistics.Win.UltraWinDock.DockableControlPane RemoveTab(Ice.Lib.Customization.CustomScriptManager csm, string parentPanelGuid, string tabKey)
        {
            EpiDockManagerPanel dmp = PubFun.GetControlByGuid<EpiDockManagerPanel>(csm, parentPanelGuid);
            for (int i = 0; i < dmp.baseDockManager.DockAreas[0].Panes.Count; i++)
            {
                Infragistics.Win.UltraWinDock.DockableControlPane pane = (Infragistics.Win.UltraWinDock.DockableControlPane)dmp.baseDockManager.DockAreas[0].Panes[i];
                if (pane.Control.Name == tabKey)
                {
                    dmp.baseDockManager.DockAreas[0].Panes.RemoveAt(i);
                    dmp.baseDockManager.DockAreas[0].Panes[0].Activate();
                    return pane;
                }
            }
            return null;
        }
        /// <summary>显示面板</summary>
        public static void InsertTab(Ice.Lib.Customization.CustomScriptManager csm, string parentPanelGuid, Infragistics.Win.UltraWinDock.DockableControlPane pane, int index)
        {
            EpiDockManagerPanel dmp = PubFun.GetControlByGuid<EpiDockManagerPanel>(csm, parentPanelGuid);
            dmp.baseDockManager.DockAreas[0].Panes.Insert(pane, index);
            dmp.baseDockManager.DockAreas[0].Panes[0].Activate();
        }
        public static void AddAreaPane(Ice.Lib.Customization.CustomScriptManager csm, string parentPanelGuid, Ice.Lib.Framework.EpiBasePanel[] panels, int index, System.Drawing.Size size, Infragistics.Win.UltraWinDock.DockedLocation dockedLocation = Infragistics.Win.UltraWinDock.DockedLocation.DockedTop, Infragistics.Win.DefaultableBoolean showCaption = Infragistics.Win.DefaultableBoolean.False, string text = "")
        {
            Ice.Lib.Customization.PersonalizeCustomizeManager personalizeCustomizeManager = csm.PersonalizeCustomizeManager;
            System.Collections.Hashtable customSheets = personalizeCustomizeManager.CustControlMan.CustomSheetsHT;
            EpiDockManagerPanel dmp = PubFun.GetControlByGuid<EpiDockManagerPanel>(csm, parentPanelGuid);
            for (int i = 0; i < panels.Length; i++)
            {
                Ice.Lib.Framework.EpiBasePanel panel = panels[i];
                if (customSheets.ContainsKey(panel.EpiGuid))
                {
                    Infragistics.Win.UltraWinDock.DockablePaneBase dockableControlPane = (Infragistics.Win.UltraWinDock.DockablePaneBase)customSheets[panel.EpiGuid];
                    if (dockableControlPane.Parent == null)
                    {
                        if (dmp.baseDockManager.DockAreas.Count <= index)
                        {
                            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane = GetNewDockAreaPane(dockedLocation, size, showCaption, text);
                            dockAreaPane.Panes.Add(dockableControlPane);
                            dmp.baseDockManager.DockAreas.Insert(dockAreaPane, index);

                        }
                        else
                        {
                            dmp.baseDockManager.DockAreas[index].Panes.Add(dockableControlPane);
                        }
                    }
                }
            }
        }
        public static Infragistics.Win.UltraWinDock.DockAreaPane GetNewDockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation dockedLocation, System.Drawing.Size size, Infragistics.Win.DefaultableBoolean showCaption, string text)
        {
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane = new Infragistics.Win.UltraWinDock.DockAreaPane(dockedLocation, "Key" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            dockAreaPane.Size = size;
            dockAreaPane.ChildPaneStyle = Infragistics.Win.UltraWinDock.ChildPaneStyle.TabGroup;
            dockAreaPane.GroupSettings.TabLocation = Infragistics.Win.UltraWinDock.Location.Top;
            dockAreaPane.Settings.ShowCaption = showCaption;
            dockAreaPane.Text = text;
            dockAreaPane.TextTab = text;
            return dockAreaPane;
        }
        /// <summary>删除所有面板组</summary>
        public static void ClearAreaPane(Ice.Lib.Customization.CustomScriptManager csm, string parentPanelGuid)
        {
            Ice.Lib.Customization.PersonalizeCustomizeManager personalizeCustomizeManager = csm.PersonalizeCustomizeManager;
            EpiDockManagerPanel dmp = PubFun.GetControlByGuid<EpiDockManagerPanel>(csm, parentPanelGuid);
            dmp.baseDockManager.DockAreas.Clear();


        }
        /// <summary>添加面板</summary>
        public static void AddPanel(Ice.Lib.Customization.CustomScriptManager csm, string parentPanelGuid, Ice.Lib.Framework.EpiBasePanel panel, int index = -1)
        {
            Ice.Lib.Customization.PersonalizeCustomizeManager personalizeCustomizeManager = csm.PersonalizeCustomizeManager;
            System.Collections.Hashtable customSheets = personalizeCustomizeManager.CustControlMan.CustomSheetsHT;
            if (customSheets.ContainsKey(panel.EpiGuid))
            {
                Infragistics.Win.UltraWinDock.DockablePaneBase dockableControlPane = (Infragistics.Win.UltraWinDock.DockablePaneBase)customSheets[panel.EpiGuid];
                EpiDockManagerPanel dmp = PubFun.GetControlByGuid<EpiDockManagerPanel>(csm, parentPanelGuid);
                bool bl = false;
                for (int i = 0; i < dmp.baseDockManager.DockAreas[0].Panes.Count; i++)
                {
                    Infragistics.Win.UltraWinDock.DockablePaneBase pane = dmp.baseDockManager.DockAreas[0].Panes[i];
                    if (pane.Key == dockableControlPane.Key)
                    {
                        return;
                    }
                }
                if (bl == false)
                {

                    if (index >= 0) dmp.baseDockManager.DockAreas[0].Panes.Insert(dockableControlPane, index);
                    else dmp.baseDockManager.DockAreas[0].Panes.Add(dockableControlPane);
                }
            }
        }
        /// <summary>激活面板</summary>
        public static void ActiveTab(EpiBaseForm form, string sheetKey)
        {
            form.ActivateSheet(sheetKey);
        }
        public static void AddStatusPanel(EpiBaseForm form, string text, string ico = "ToDo")
        {
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel usp = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            usp.Key = "USP" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            usp.Text = text;
            if (ico != "") usp.Appearance.Image = Ice.Lib.EpiUIImages.SmallEnabledImages.Images[Ice.Lib.EpiUIImages.IndexOf(ico)];
            usp.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;
            usp.WrapText = Infragistics.Win.DefaultableBoolean.True;
            Infragistics.Win.UltraWinStatusBar.UltraStatusBar baseStatusBar2;
            Control[] ctls = form.Controls.Find("baseStatusBar", true);
            if (ctls.Length > 0)
            {
                baseStatusBar2 = (Infragistics.Win.UltraWinStatusBar.UltraStatusBar)ctls[0];
                baseStatusBar2.Panels.AddRange(new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel[1] { usp });
            }
        }
    }
