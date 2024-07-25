// **************************************************
// Custom code for UD01Form
// Created: 10/17/2022 3:40:04 PM
// **************************************************
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using Ice.BO;
using Ice.UI;
using Ice.Lib;
using Ice.Adapters;
using Ice.Lib.Customization;
using Ice.Lib.ExtendedProps;
using Ice.Lib.Framework;
using Ice.Lib.Searches;
using Ice.UI.FormFunctions;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using System.Collections.Generic;
using Erp.Adapters;
using System.Drawing;
using System.Collections;
public class Script
{
	// ** Wizard Insert Location - Do Not Remove 'Begin/End Wizard Added Module Level Variables' Comments! **
	// Begin Wizard Added Module Level Variables **

	// End Wizard Added Module Level Variables **

	// Add Custom Module Level Variables Here **
	RptCustSalesAmt obj=new RptCustSalesAmt();
	public void InitializeCustomCode()
	{
		// ** Wizard Insert Location - Do not delete 'Begin/End Wizard Added Variable Initialization' lines **
		// Begin Wizard Added Variable Initialization
		obj.Initialize(csm);
		// End Wizard Added Variable Initialization

		// Begin Wizard Added Custom Method Calls

		// End Wizard Added Custom Method Calls
	}

	public void DestroyCustomCode()
	{
		// ** Wizard Insert Location - Do not delete 'Begin/End Wizard Added Object Disposal' lines **
		// Begin Wizard Added Object Disposal
		obj.Destroy();
		// End Wizard Added Object Disposal

		// Begin Custom Code Disposal

		// End Custom Code Disposal
	}

	private void UD01Form_Load(object sender, EventArgs args)
	{
		PubPane.AddStatusPanel(oTrans.EpiBaseForm,"Ver:1.00.20230320", "ToDo");
		obj.FormLoad("Customer Sales Amount Report(RPT830)");
	}
}
    public class RptCustSalesAmt
    {
        Ice.UI.Shared.UDSupport.UDSingleViewTransaction oTrans;
        Ice.Lib.Customization.CustomScriptManager csm;
        UltraToolbarsManager baseToolbarsManager;
        public Ice.Core.Session session;
        EpiBaseForm udForm;
        readonly string UDName = "UD01";

        DataTable ParameterDT;
        EpiDataView edvParameter;
        //
        EpiGroupBox epiGBReportParam;
        EpiDateTimeEditor epiDTEFromDate;
        EpiDateTimeEditor epiDTEToDate;
        EpiButton epiBtCustomer;
        EpiButton epiBtSalesPerson;
        EpiButton epiBtSubmit;

        string LanguageID;
        string UserID;
        const string Key4_RPTData = "RPTData";
        const string Key5_RPTParameter= "RPTParameter";
        System.Threading.Thread thread;
        string AgentID;
        public void Initialize(Ice.Lib.Customization.CustomScriptManager csm1)
        {
            csm = csm1;
            baseToolbarsManager = ((UltraToolbarsManager)(csm.GetGlobalInstance("baseToolbarsManager")));
            oTrans = ((Ice.UI.Shared.UDSupport.UDSingleViewTransaction)(csm.GetGlobalInstance("oTrans")));
            session = (Ice.Core.Session)(oTrans.Session);
            LanguageID = session.LanguageID;
            UserID = session.UserID;
            udForm = ((EpiBaseForm)(csm.GetGlobalInstance(UDName + "Form")));
            InitEpiDataView();
            //Get Controls
            epiGBReportParam = PubFun.GetControlByName<EpiGroupBox>(csm, "epiGBReportParam");
            epiDTEFromDate = PubFun.GetControlByName<EpiDateTimeEditor>(csm, "epiDTEFromDate");
            epiDTEToDate = PubFun.GetControlByName<EpiDateTimeEditor>(csm, "epiDTEToDate");
            epiBtCustomer = PubFun.GetControlByName<EpiButton>(csm, "epiBtCustomer");
            epiBtSalesPerson = PubFun.GetControlByName<EpiButton>(csm, "epiBtSalesPerson");
            epiBtSubmit = PubFun.GetControlByName<EpiButton>(csm, "epiBtSubmit");
            //
            epiBtCustomer.Click += EpiBtCustomer_Click;
            epiBtSalesPerson.Click += EpiBtSalesPerson_Click;
            epiBtSubmit.Click += EpiBtSubmit_Click;
        }

        public void Destroy()
        {
            epiBtCustomer.Click -= EpiBtCustomer_Click;
            epiBtSalesPerson.Click -= EpiBtSalesPerson_Click;
            epiBtSubmit.Click -= EpiBtSubmit_Click;
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
                thread = null;
            }
        }

        public void FormLoad(string title)
        {
            SetTitle(title);
            HiddenLeftTree();
            InitTool(); 
            InitControls();
            GetAgentID();

        }
        public void GetAgentID()
        {
            SysAgentAdapter adp = new SysAgentAdapter(oTrans);
            try
            {
                adp.BOConnect();
                adp.GetDefaultTaskAgentID(out AgentID);
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
            finally
            {
                adp.Dispose();
            }
        }
        private void InitTool()
        {
            baseToolbarsManager.Toolbars["Navigation Toolbar"].Visible = false;//隐藏搜索工具栏
            PubTool.HiddenToolbars(udForm, new string[] { "NewTool", "NewMenuTool", "DeleteTool", "SaveTool", "RefreshTool", "ClearTool", "AttachmentTool" });
        }
        private void SetTitle(string title)
        {
            Infragistics.Win.UltraWinDock.DockableWindow dockableWindow1 = (Infragistics.Win.UltraWinDock.DockableWindow)(udForm.Controls["windowDockingArea2"].Controls["dockableWindow1"]);
            udForm.Text = title;
            dockableWindow1.Pane.Text = "";
        }
        private void HiddenLeftTree()
        {
            Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea1 = (Infragistics.Win.UltraWinDock.WindowDockingArea)(udForm.Controls["windowDockingArea1"]);
            windowDockingArea1.Visible = false;
        }
        private void InitControls()
        {
            epiGBReportParam.Dock = DockStyle.Fill;
            epiGBReportParam.SendToBack();
            PubFun.InitEpiDateTimeEditor(epiDTEFromDate, "yyyy/mm", "yyyy/MM");
            PubFun.InitEpiDateTimeEditor(epiDTEToDate, "yyyy/mm", "yyyy/MM");


            epiDTEFromDate.MaxDate = DateTime.Now.Date;
            epiDTEToDate.MaxDate = DateTime.Now.Date;
        }

        private void InitEpiDataView()
        {
            //Parameter
            ParameterDT = new DataTable();
            ParameterDT.Columns.AddRange(new DataColumn[] {
                 PubFun.NewColumn("Date01","From Date","",80,typeof(DateTime))
                ,PubFun.NewColumn("Date02","To Date","",80,typeof(DateTime))
                ,PubFun.NewColumn("Character01","Customer ID")
                ,PubFun.NewColumn("ShortChar01","Sales Person Code")
                ,PubFun.NewColumn("CheckBox01","Show Current year month","",80,typeof(bool))
				,PubFun.NewColumn("CheckBox02","Exclude Free-text invoice","",80,typeof(bool))           
                ,PubFun.NewColumn("SysRowID")
            });
            DataRow dr = ParameterDT.NewRow();
            ParameterDT.Rows.Add(dr);
            dr["Date01"] = new DateTime(DateTime.Now.Year - 2, 1, 1);
            dr["Date02"] = DateTime.Now.Date;
            edvParameter = new EpiDataView();
            edvParameter.dataView = new DataView(ParameterDT);
            if ((oTrans.EpiDataViews.ContainsKey("Parameter") == false))
            {
                oTrans.Add("Parameter", edvParameter);
            }
        }

    
        private void EpiBtSubmit_Click(object sender, EventArgs e)
        {
            if (edvParameter.CurrentDataRow != null)
            {
                if (CheckParameter())
                {
                    string key1 = "K" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    SaveParameter(key1);
                    CalcReportData(key1);
                    if (thread != null && thread.IsAlive)
                    {
                        thread.Abort();
                        thread = null;
                    }
                    ThreadParameter tp = new ThreadParameter()
                    {
                        Key1 = key1,
                        oTrans = oTrans,
                    };
                    thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(GetStatus));
                    thread.Start(tp);
                }
            }
        }

        private bool CheckParameter()
        {
            //if (Convert.ToString(edvParameter.CurrentDataRow["ShortChar01"]) == "")
            //{
            //    MessageBox.Show("Option cannot be empty.");
            //    return false;
            //}
            return true;
        }

        struct ThreadParameter
        {
            public string Key1;
            public Ice.UI.Shared.UDSupport.UDSingleViewTransaction oTrans;
        };

        private void GetStatus(object _tp)
        {
            try
            {
                epiGBReportParam.ReadOnly = true;
                oTrans.StatusPanel.Text = "Submitting";
                ThreadParameter tp = (ThreadParameter)_tp;
                while (true)
                {
                    DataTable dt = PubFun.GetAdapterData(tp.oTrans, "UD21", "Key1='" + tp.Key1 + "' and Key5='" + Key5_RPTParameter + "'");
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt.Rows[0]["CheckBox20"]))break;
                    }
                    else break;
                    System.Threading.Thread.Sleep(3000);
                }
            }
            finally
            {
                oTrans.StatusPanel.Text = "Submitted";
                epiGBReportParam.ReadOnly = false;
            }
        }

        public void CalcReportData(string key1) {
            ScheduledFunctionAdapter adp1 = new ScheduledFunctionAdapter(oTrans);
            try
            {
                adp1.BOConnect();
                adp1.GetNewParameters();
                if (adp1.ScheduledFunctionData.ScheduledFunctionParam.Rows.Count > 0)
                {
                    DataRow dr = adp1.ScheduledFunctionData.ScheduledFunctionParam.Rows[0];
                    dr.BeginEdit();
                    dr["LibraryId"] = "RPTCalc";
                    dr["LibraryDescription"] = "Report Calculation";
                    dr["FunctionId"] = "RPT830";
                    dr["FunctionDescription"] = "FSD029 Customer Sales Amount Report(RPT830)";
                    dr["AgentID"] = AgentID;
                    dr["RecurringTask"] = false;
                    dr["WorkstationID"] = session.WorkstationID;
                    dr["ParameterValues"] = "{ \"key1\":\"" + key1 + "\",\"SysRowID\":null}";//          <ParameterValues>{"year":"2023","month":"2","SysRowID":null}</ParameterValues>
                    dr.EndEdit();
                    adp1.SubmitToAgent(AgentID, 0, 0);
                }
            }
            catch (Exception e1)
            {
                throw e1;
            }
            finally
            {
                adp1.Dispose();
            }
        }
        public void SaveParameter(string key1) {
            UD21Adapter adp = new UD21Adapter(oTrans);
            adp.BOConnect();
            try
            {
                DateTime now = DateTime.Now;
                adp.GetaNewUD21();
                DataRow newRow = adp.UD21Data.UD21.Rows[adp.UD21Data.UD21.Rows.Count - 1];
                newRow.BeginEdit();
                newRow["Key1"] = key1;
                newRow["Key4"] = Key4_RPTData;
                newRow["Key5"] = Key5_RPTParameter;
                if (!Convert.IsDBNull(edvParameter.CurrentDataRow["Date01"]))
                {
                    newRow["Date01"] =PubDate.FirstDayOfMonth(Convert.ToDateTime(edvParameter.CurrentDataRow["Date01"])).Date;
                }
                if (!Convert.IsDBNull(edvParameter.CurrentDataRow["Date02"]))
                {
                    newRow["Date02"] = PubDate.LastDayOfMonth(Convert.ToDateTime(edvParameter.CurrentDataRow["Date02"])).Date;
                }
                newRow["ShortChar01"] = edvParameter.CurrentDataRow["ShortChar01"];
                newRow["Character01"] = edvParameter.CurrentDataRow["Character01"];
                newRow["CheckBox01"] = edvParameter.CurrentDataRow["CheckBox01"];
                newRow["CheckBox02"] = edvParameter.CurrentDataRow["CheckBox02"];
                newRow["ShortChar16"] = "BK_CustSalesAmt";//BAQ报表ID
                newRow["ShortChar17"] = "BK_BAQ_Report";//BAQ ID
                newRow["ShortChar18"] = AgentID;
                newRow["ShortChar19"] = UserID;
                newRow["ShortChar20"] = now.ToString("yyyy/MM/dd HH:mm:ss");
                newRow["Date20"] = now.Date;
                newRow.EndEdit();
                adp.Update();

            }
            catch (Exception e1)
            {
                throw e1;
            }
            finally
            {
                adp.Dispose();
            }

        }  
        private void EpiBtCustomer_Click(object sender, EventArgs e)
        {
            DataTable dt = PubFun.GetAdapterData(oTrans, "Customer", "", true, false);
            if (dt.Rows.Count > 0) {
                edvParameter.CurrentDataRow.BeginEdit();
                edvParameter.CurrentDataRow["Character01"] = dt.Rows[0]["CustID"];
                edvParameter.CurrentDataRow.EndEdit();
            }
            
        }
        private void EpiBtSalesPerson_Click(object sender, EventArgs e)
        {
            DataTable dt = PubFun.GetAdapterData(oTrans, "SalesTer", "", true, false);
            if (dt.Rows.Count > 0)
            {
                edvParameter.CurrentDataRow.BeginEdit();
                edvParameter.CurrentDataRow["ShortChar01"] = dt.Rows[0]["TerritoryID"];
                edvParameter.CurrentDataRow.EndEdit();
            }
        }

    }
    /// <summary>公共方法</summary>
    public static class PubFun
    {
        #region Epicor常用方法
        public static string GetErrorMsgByCode(string errCode, bool isNewLine = true)
        {
            string msg = Script.GetStringByID(errCode);
            if (msg == "")
                return errCode;
            if (isNewLine)
                return string.Format("[ErrorCode:{0}]\n{1}", errCode, msg);
            return string.Format("[ErrorCode:{0}]\t{1}", errCode, msg);
        }
        /// <summary>按GUID获取Epicor控件</summary>
        public static T GetControlByGuid<T>(Ice.Lib.Customization.CustomScriptManager csm, string guid)
        {
            Control control=csm.GetNativeControlReference(guid);
            if (control != null) return (T)(Object)control;
            throw new Exception("Get Control By Guid(" + guid + ") Error.");
        }
        /// <summary>按Name获取Epicor控件</summary>
        public static T GetControlByName<T>(Ice.Lib.Customization.CustomScriptManager csm, string name)
        {
            foreach (object obj in csm.PersonalizeCustomizeManager.ControlsHT.Values) {
                Control control = (Control)obj;
                if (control.Name.ToUpper() == name.ToUpper()) {
                    return (T)obj;
                }
            }
            throw new Exception("CustomScriptManager Get Control By Name(" + name+") Error1.");
        }
        /// <summary>按Name获取Epicor控件</summary>
        public static T GetControlByName<T>(Control control, string name)
        {
            Control[] controls = control.Controls.Find(name, true);
            if (controls.Length > 0) return (T)((Object)(controls[0]));
            throw new Exception("Control Get Control By Name(" + name + ") Error2.");
        }

        public static void HiddenUDFormLeftTree(EpiBaseForm udForm,string windowDockingAreaName="windowDockingArea1")
        {
            Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea = (Infragistics.Win.UltraWinDock.WindowDockingArea)(udForm.Controls[windowDockingAreaName]);
            windowDockingArea.Visible = false;
        }

        public static void SetFormTitle(EpiBaseForm form,string title)
        {
            form.Text = title;
            if (form.Name.Length == 8 && form.Name.StartsWith("UD"))
            {
                Infragistics.Win.UltraWinDock.DockableWindow dockableWindow1 = (Infragistics.Win.UltraWinDock.DockableWindow)(form.Controls["windowDockingArea2"].Controls["dockableWindow1"]);
                dockableWindow1.Pane.Text = title;
            }
            else if (form.Name.Length == 9 && form.Name.StartsWith("UD"))
            {
                Infragistics.Win.UltraWinDock.DockableWindow dockableWindow4 = (Infragistics.Win.UltraWinDock.DockableWindow)(form.Controls["windowDockingArea2"].Controls["dockableWindow4"]);
                Infragistics.Win.UltraWinDock.DockableWindow dockableWindow3 = (Infragistics.Win.UltraWinDock.DockableWindow)(form.Controls["windowDockingArea1"].Controls["dockableWindow3"]);
                dockableWindow4.Pane.Text = title;
                dockableWindow3.Pane.Text = title;
            }
            else {
                Infragistics.Win.UltraWinDock.DockableWindow dockableWindow2 = (Infragistics.Win.UltraWinDock.DockableWindow)(form.Controls["windowDockingArea2"].Controls["dockableWindow2"]);
                Infragistics.Win.UltraWinDock.DockableWindow dockableWindow3 = (Infragistics.Win.UltraWinDock.DockableWindow)(form.Controls["windowDockingArea1"].Controls["dockableWindow3"]);
                dockableWindow2.Pane.Text = title;
                dockableWindow3.Pane.Text = title;
            }
        }
        public static void SetFormTitle(EpiBaseForm form, string title, string windowDockingAreaName, string dockableWindowName)
        {
            form.Text = title;
            Infragistics.Win.UltraWinDock.DockableWindow dockableWindow1 = (Infragistics.Win.UltraWinDock.DockableWindow)(form.Controls[windowDockingAreaName].Controls[dockableWindowName]);
            dockableWindow1.Pane.Text = title;

        }
       

        /// <summary>从adapter获取数据</summary>
        public static DataTable GetAdapterData(object otran, string AdapterN, string whereClause = "", bool showSeach = false, bool multiSelect = true)
        {
            if (!AdapterN.EndsWith("Adapter")) AdapterN += "Adapter";
            bool recSelected;
            DataSet dsAdapter = Ice.UI.FormFunctions.SearchFunctions.listLookup(otran, AdapterN, out recSelected, showSeach, whereClause, multiSelect);
            DataTable dt = new DataTable();
            if (recSelected) dt = dsAdapter.Tables[0];
            return dt;
        }

        /// <summary>打开菜单</summary>
        public static UIReflector OpenMenu(object otrans, string menuID, object valueIn = null, bool isModal = false, object contextValue = null)
        {
            LaunchFormOptions opts = new LaunchFormOptions();
            opts.ValueIn = valueIn;
            opts.ContextValue = contextValue;
            if (isModal)
            {
                opts.IsModal = true;
                opts.SuppressFormSearch = true;
            }
            return OpenMenu(otrans, menuID, opts);
        }
        /// <summary>打开菜单</summary>
        public static UIReflector OpenMenu(object otrans, string menuID, LaunchFormOptions opts)
        {
            return ProcessCaller.LaunchCallbackForm(otrans, menuID, opts);
        }
        /// <summary></summary>
        public static Ice.Lib.Searches.SearchOptions GetSearchOptions(System.Collections.Hashtable wcHash)
        {
            Ice.Lib.Searches.SearchOptions opts = Ice.Lib.Searches.SearchOptions.CreateRuntimeSearch(wcHash, Ice.Lib.Searches.DataSetMode.RowsDataSet);
            return opts;
        }

        /// <summary> EpiDataView添加新字段</summary>
        public static void EpiDataViewAddColumns(EpiDataView edv, DataColumn[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                edv.dataView.Table.Columns.Add(CopyColumn(columns[i]));
            }
        }

        public static DataColumn NewColumn(string columnName, string caption = "", string captionSch = "", int width = 80, Type dataType = null, bool allowEdit = true, bool hidden = false, int columnDigits = 2, string format = "", string maskInput = "", string formatString = "")
        {
            if (string.IsNullOrEmpty(caption)) caption = columnName;
            if (dataType == null) dataType = typeof(string);
            if (columnDigits < 0) columnDigits = 0;
            DataColumn column = new DataColumn();
            column.ColumnName = columnName;
            column.Caption = caption;
            column.DataType = dataType;
            column.ExtendedProperties["CaptionSch"] = captionSch;
            column.ExtendedProperties["Width"] = width;
            column.ExtendedProperties["ColumnDigits"] = columnDigits;
            column.ExtendedProperties["Hidden"] = hidden;
            column.ExtendedProperties["AllowEdit"] = allowEdit;
            column.ExtendedProperties["Format"] = format;//表格控件单元格显示格式
            column.ExtendedProperties["MaskInput"] = maskInput;//表格控件单元格输入格式
            column.ExtendedProperties["FormatString"] = formatString;//Excel单元格格式
            if (dataType == typeof(decimal) || dataType == typeof(int))
            {
                column.DefaultValue = 0;
                if (formatString == "")
                {
                    if (dataType == typeof(int) || columnDigits == 0)
                    {
                        column.ExtendedProperties["FormatString"] = "#,##0";
                    }
                    else
                    {
                        column.ExtendedProperties["FormatString"] = "#,##0." + new string('0', columnDigits);
                    }
                }
            }
            else if (dataType == typeof(bool))
            {
                column.DefaultValue = false;
            }
            else if (dataType == typeof(string))
            {
                column.ExtendedProperties["MaxLength"] = 2000;
                column.DefaultValue = "";
            }
            return column;
        }
        public static DataColumn NewFilterColumn(string tableName, string columnName, string caption, Type type = null, object defaultValue = null, bool hidden = false, bool readOnly = false, string DefaultOperator = "=", DataTable listTable = null, string listTableValueColumn = "Value", string listTableTextColumn = "Text")
        {
            if (type == null) type = typeof(string);
            DataColumn col1 = new DataColumn(columnName, type);
            col1.Caption = caption;
            col1.ReadOnly = readOnly;
            col1.DefaultValue = defaultValue;
            col1.ExtendedProperties.Add("TableName", tableName);
            col1.ExtendedProperties["DefaultOperator"] = DefaultOperator;
            col1.ExtendedProperties["Hidden"] = hidden;
            col1.ExtendedProperties.Add("ListTable", listTable);
            col1.ExtendedProperties.Add("ListTableValueColumn", listTableValueColumn);
            col1.ExtendedProperties.Add("ListTableTextColumn", listTableTextColumn);
            return col1;
        }
        public static DataColumn CopyColumn(DataColumn column1)
        {
            if (column1 == null) return null;
            DataColumn column = new DataColumn();
            column.ColumnName = column1.ColumnName;
            column.Caption = column1.Caption;
            column.DataType = column1.DataType;
            if (column.DataType == typeof(decimal) || column.DataType == typeof(int))
            {
                column.DefaultValue = 0;
            }
            else if (column.DataType == typeof(bool))
            {
                column.DefaultValue = false;
            }
            else if (column.DataType == typeof(string))
            {
                column.DefaultValue = "";
            }
            foreach (string key in column.ExtendedProperties)
            {
                column.ExtendedProperties[key] = column1.ExtendedProperties[key];
            }

            return column;
        }
        /// <summary>禁用列排序功能</summary>
        public static void DisableEpiUltraGridSorting(Infragistics.Win.UltraWinGrid.UltraGrid ug1, System.Collections.Generic.List<string> columns =null) {
            for (int i = 0; i < ug1.DisplayLayout.Bands[0].Columns.Count; i++) {
               
                if (columns == null)
                {
                    ug1.DisplayLayout.Bands[0].Columns[i].SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
                }
                else { 
                    string columnName = ug1.DisplayLayout.Bands[0].Columns[i].Key;
                    if (columns.Contains(columnName)) {
                        ug1.DisplayLayout.Bands[0].Columns[i].SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
                    }
                }
            }
        }

        /// <summary> 初始化EpiUltraGrid字段</summary>
        public static DataTable InitEpiUltraGrid(Infragistics.Win.UltraWinGrid.UltraGrid ug1, DataTable dt, string languageID = "sch")
        {
            ug1.DataSource = dt;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (ug1.DisplayLayout.Bands[0].Columns.Exists(dt.Columns[i].ColumnName))
                {
                    Infragistics.Win.UltraWinGrid.UltraGridColumn ugColumn = ug1.DisplayLayout.Bands[0].Columns[dt.Columns[i].ColumnName];
                    string captionSch = Convert.ToString(dt.Columns[i].ExtendedProperties["CaptionSch"]);
                    if (languageID.ToLower() == "sch" && !string.IsNullOrEmpty(captionSch)) ugColumn.Header.Caption = captionSch;
                    else ugColumn.Header.Caption = dt.Columns[i].Caption;
                    ugColumn.Width = Convert.ToInt32(dt.Columns[i].ExtendedProperties["Width"]);
                    ugColumn.Hidden = Convert.ToBoolean(dt.Columns[i].ExtendedProperties["Hidden"]);
                    ugColumn.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                    
                    if (Convert.ToBoolean(dt.Columns[i].ExtendedProperties["AllowEdit"]) == false)
                    {
                        ugColumn.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
                        ugColumn.CellAppearance.BackColor = System.Drawing.Color.Gainsboro;
                    }
                    else
                    {
                        ugColumn.CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                        ugColumn.CellAppearance.BackColor = System.Drawing.Color.White;
                    }
                    if (dt.Columns[i].DataType == typeof(decimal))
                    {
                        int ColumnDigits = Convert.ToInt32(dt.Columns[i].ExtendedProperties["ColumnDigits"]);
                        InitEpiUGNumber(ug1, dt.Columns[i].ColumnName, ColumnDigits, 0);
                    }
                    else if (dt.Columns[i].DataType == typeof(int))
                    {
                        InitEpiUGNumber(ug1, dt.Columns[i].ColumnName, 0, 0);
                    }
                    else if (dt.Columns[i].DataType == typeof(DateTime))
                    {
                        ugColumn.Format = "yyyy/MM/dd";
                    }
                    else if (ugColumn.DataType == typeof(string)) {
                        int maxLength = 2000;
                        if (dt.Columns[i].ExtendedProperties["MaxLength"] != null) {
                            maxLength = Convert.ToInt32(dt.Columns[i].ExtendedProperties["MaxLength"]);
                        }
                        ugColumn.MaxLength = maxLength; 
                    }
                    string Format = Convert.ToString(dt.Columns[i].ExtendedProperties["Format"]);
                    if (!string.IsNullOrEmpty(Format)) ugColumn.Format = Format;
                    string MaskInput = Convert.ToString(dt.Columns[i].ExtendedProperties["MaskInput"]);
                    if (!string.IsNullOrEmpty(MaskInput)) ugColumn.MaskInput = MaskInput;
                }
            }
            return dt;
        }


        public static void SetEpiUGReadonly(Infragistics.Win.UltraWinGrid.UltraGrid ug1, bool _readonly, string languageID = "sch")
        {
            if (_readonly)
            {
                for (int i = 0; i < ug1.DisplayLayout.Bands[0].Columns.Count; i++)
                {
                    Infragistics.Win.UltraWinGrid.UltraGridColumn ugColumn = ug1.DisplayLayout.Bands[0].Columns[i];
                    ugColumn.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
                    ugColumn.CellAppearance.BackColor = System.Drawing.Color.Gainsboro;
                }
            }
            else
            {
                if (ug1.DataSource is DataTable)
                {
                    InitEpiUltraGrid(ug1, (DataTable)ug1.DataSource, languageID);
                }
                else
                {
                    for (int i = 0; i < ug1.DisplayLayout.Bands[0].Columns.Count; i++)
                    {
                        Infragistics.Win.UltraWinGrid.UltraGridColumn ugColumn = ug1.DisplayLayout.Bands[0].Columns[i];
                        ugColumn.CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                        ugColumn.CellAppearance.BackColor = System.Drawing.Color.White;
                    }
                }
            }
        }
        public static void SetEpiUGAllow(Infragistics.Win.UltraWinGrid.UltraGrid ug1, Infragistics.Win.UltraWinGrid.AllowAddNew allowAddNew= Infragistics.Win.UltraWinGrid.AllowAddNew.No, Infragistics.Win.DefaultableBoolean allowDelete= Infragistics.Win.DefaultableBoolean.False)
        {
            ug1.DisplayLayout.Override.AllowAddNew = allowAddNew;
            ug1.DisplayLayout.Override.AllowDelete = allowDelete;
        }
        /// <summary> 固定EpiUltraGrid的栏位</summary>
        public static void SetEpiUGFixed(Infragistics.Win.UltraWinGrid.UltraGrid ug1, System.Collections.Generic.List<string> columns)
        {
            if (columns != null)
            {
                ug1.DisplayLayout.UseFixedHeaders = true;
                for (int i = 0; i < ug1.DisplayLayout.Bands[0].Columns.Count; i++) {
                    string columnName = ug1.DisplayLayout.Bands[0].Columns[i].Key;
                    ug1.DisplayLayout.Bands[0].Columns[columnName].Header.Fixed = true;
                }
            }
        }
        /// <summary> 初始化EpiUltraGrid字段</summary>
        public static void InitEpiUltraGridB(Infragistics.Win.UltraWinGrid.UltraGrid ug1, DataColumn[] columns, bool setVisiblePosition = true, string languageID = "sch")
        {
            for (int i = 0; i < columns.Length; i++)
            {
                if (ug1.DisplayLayout.Bands[0].Columns.Exists(columns[i].ColumnName))
                {
                    Infragistics.Win.UltraWinGrid.UltraGridColumn ugColumn = ug1.DisplayLayout.Bands[0].Columns[columns[i].ColumnName];
                    string captionSch = Convert.ToString(columns[i].ExtendedProperties["CaptionSch"]);
                    if (languageID.ToLower() == "sch" && !string.IsNullOrEmpty(captionSch)) ugColumn.Header.Caption = captionSch;
                    else ugColumn.Header.Caption = columns[i].Caption;
                    ugColumn.Width = Convert.ToInt32(columns[i].ExtendedProperties["Width"]);
                    ugColumn.Hidden = Convert.ToBoolean(columns[i].ExtendedProperties["Hidden"]);
                    ugColumn.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                    if (Convert.ToBoolean(columns[i].ExtendedProperties["AllowEdit"]) == false)
                    {
                        ugColumn.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
                        ugColumn.CellAppearance.BackColor = System.Drawing.Color.Gainsboro;
                    }
                    if (columns[i].DataType == typeof(decimal))
                    {
                        int ColumnDigits = Convert.ToInt32(columns[i].ExtendedProperties["ColumnDigits"]);
                        InitEpiUGNumber(ug1, columns[i].ColumnName, ColumnDigits, 0);
                    }
                    else if (columns[i].DataType == typeof(int))
                    {
                        InitEpiUGNumber(ug1, columns[i].ColumnName, 0, 0);
                    }
                    else if (columns[i].DataType == typeof(DateTime))
                    {
                        ugColumn.Format = "yyyy/MM/dd";
                    }
                    else if (ugColumn.DataType == typeof(string))
                    {
                        int maxLength = 2000;
                        if (columns[i].ExtendedProperties["MaxLength"] != null)
                        {
                            maxLength = Convert.ToInt32(columns[i].ExtendedProperties["MaxLength"]);
                        }
                        ugColumn.MaxLength = maxLength;
                    }
                    string Format = Convert.ToString(columns[i].ExtendedProperties["Format"]);
                    if (!string.IsNullOrEmpty(Format)) ugColumn.Format = Format;
                    string MaskInput = Convert.ToString(columns[i].ExtendedProperties["MaskInput"]);
                    if (!string.IsNullOrEmpty(MaskInput)) ugColumn.MaskInput = MaskInput;
                }
            }
            if (setVisiblePosition)
            {
                System.Collections.Generic.List<string> columnsName = new System.Collections.Generic.List<string>();
                for (int i = 0; i < columns.Length; i++)
                {
                    if (Convert.ToBoolean(columns[i].ExtendedProperties["Hidden"]) == false) columnsName.Add(columns[i].ColumnName);
                }
                SetUltraGridVisiblePosition(ug1, columnsName);
            }
        }
        public static void InitEpiUltraGridC(Infragistics.Win.UltraWinGrid.UltraGrid ug1, DataColumn[] columns, bool setVisible = true, string languageID = "sch")
        {
            for (int i = 0; i < columns.Length; i++)
            {
                if (ug1.DisplayLayout.Bands[0].Columns.Exists(columns[i].ColumnName))
                {
                    Infragistics.Win.UltraWinGrid.UltraGridColumn ugColumn = ug1.DisplayLayout.Bands[0].Columns[columns[i].ColumnName];
                    string captionSch = Convert.ToString(columns[i].ExtendedProperties["CaptionSch"]);
                    if (languageID.ToLower() == "sch" && !string.IsNullOrEmpty(captionSch)) ugColumn.Header.Caption = captionSch;
                    else ugColumn.Header.Caption = columns[i].Caption;
                    ugColumn.Width = Convert.ToInt32(columns[i].ExtendedProperties["Width"]);
                    ugColumn.Hidden = Convert.ToBoolean(columns[i].ExtendedProperties["Hidden"]);
                    ugColumn.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                    if (Convert.ToBoolean(columns[i].ExtendedProperties["AllowEdit"]) == false)
                    {
                        ugColumn.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
                        ugColumn.CellAppearance.BackColor = System.Drawing.Color.Gainsboro;
                    }
                    if (columns[i].DataType == typeof(decimal))
                    {
                        int ColumnDigits = Convert.ToInt32(columns[i].ExtendedProperties["ColumnDigits"]);
                        InitEpiUGNumber(ug1, columns[i].ColumnName, ColumnDigits, 0);
                    }
                    else if (columns[i].DataType == typeof(int))
                    {
                        InitEpiUGNumber(ug1, columns[i].ColumnName, 0, 0);
                    }
                    else if (columns[i].DataType == typeof(DateTime))
                    {
                        ugColumn.Format = "yyyy/MM/dd";
                    }
                    else if (ugColumn.DataType == typeof(string))
                    {
                        int maxLength = 2000;
                        if (columns[i].ExtendedProperties["MaxLength"] != null)
                        {
                            maxLength = Convert.ToInt32(columns[i].ExtendedProperties["MaxLength"]);
                        }
                        ugColumn.MaxLength = maxLength;
                    }
                    string Format = Convert.ToString(columns[i].ExtendedProperties["Format"]);
                    if (!string.IsNullOrEmpty(Format)) ugColumn.Format = Format;
                    string MaskInput = Convert.ToString(columns[i].ExtendedProperties["MaskInput"]);
                    if (!string.IsNullOrEmpty(MaskInput)) ugColumn.MaskInput = MaskInput;
                }
            }
            if (setVisible)
            {
                SetUltraGridVisible(ug1, columns);
            }
        }
        /// <summary> 初始化EpiUltraGrid数字字段</summary>
        public static void InitEpiUGNumber(Infragistics.Win.UltraWinGrid.UltraGrid ug1, string colName, int n = 0, int bandsIndex = 0)
        {
            string str1 = "###,###,##0";
            string str2 = "-nnn,nnn,nnn";
            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                {
                    str1 += ".";
                    str2 += ".";
                }
                str1 += "0";
                str2 += "n";
            }
            ug1.DisplayLayout.Bands[bandsIndex].Columns[colName].Format = str1;
            ug1.DisplayLayout.Bands[bandsIndex].Columns[colName].MaskInput = str2;
            ug1.DisplayLayout.Bands[bandsIndex].Columns[colName].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        /// <summary>设置UltraGrid字段顺序</summary>
        public static void SetUltraGridVisiblePosition(Infragistics.Win.UltraWinGrid.UltraGrid ug1, System.Collections.Generic.List<string> columnsName, int bandsIndex = 0, bool hiddenOtherColumns=true)
        {
            for (int i = ug1.DisplayLayout.Bands[bandsIndex].Columns.Count - 1; i >= 0; i--)
            {
                ug1.DisplayLayout.Bands[bandsIndex].Columns[i].Header.VisiblePosition += columnsName.Count;
                if (hiddenOtherColumns)
                {
                    string columnName = ug1.DisplayLayout.Bands[bandsIndex].Columns[i].Key;
                    ug1.DisplayLayout.Bands[bandsIndex].Columns[i].Hidden = (!columnsName.Contains(columnName));
                }
            }
            for (int i = 0; i < columnsName.Count; i++)
            {
                if (ug1.DisplayLayout.Bands[bandsIndex].Columns.Exists(columnsName[i]))
                    ug1.DisplayLayout.Bands[bandsIndex].Columns[columnsName[i]].Header.VisiblePosition = i;
            }
        }

        public static void SetUltraGridVisible(Infragistics.Win.UltraWinGrid.UltraGrid ug1, DataColumn[] columns, int bandsIndex = 0)
        {
            System.Collections.Generic.List<string> columnsName = new System.Collections.Generic.List<string>();
            for (int i = 0; i < columns.Length; i++)
            {
                if (Convert.ToBoolean(columns[i].ExtendedProperties["Hidden"]) == false) columnsName.Add(columns[i].ColumnName);
            }
            for (int i = ug1.DisplayLayout.Bands[bandsIndex].Columns.Count - 1; i >= 0; i--)
            {
                string columnName = ug1.DisplayLayout.Bands[bandsIndex].Columns[i].Key;
                ug1.DisplayLayout.Bands[bandsIndex].Columns[i].Hidden = (!columnsName.Contains(columnName));
            }
        }
        /// <summary>设置EpiTextBox控件显示按钮</summary>
        public static Infragistics.Win.UltraWinEditors.EditorButton SetEpiTBShowButton(EpiTextBox epiTB,string buttonText, Infragistics.Win.UltraWinEditors.EditorButtonEventHandler button_Click,bool buttonsRight=true,int buttonWidth=0)
        {
            Infragistics.Win.UltraWinEditors.EditorButton button = new Infragistics.Win.UltraWinEditors.EditorButton();
            button.Text = buttonText;
            button.Click += button_Click;
            if(buttonWidth>0) button.Width = buttonWidth;
            if (buttonsRight) epiTB.ButtonsRight.Add(button);
            else epiTB.ButtonsLeft.Add(button);
            return button;
        }


        /// <summary>设置GroupBy</summary>
        public static void SetGroupBy(Infragistics.Win.UltraWinGrid.UltraGrid ug1, string columnsName, int bandsIndex = 0)
        {
            ug1.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            ug1.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            ug1.DisplayLayout.GroupByBox.Hidden = true;
            ug1.DisplayLayout.Bands[bandsIndex].SortedColumns.Clear();
            ug1.DisplayLayout.Bands[bandsIndex].SortedColumns.Add(columnsName, false, true);
            ug1.DisplayLayout.Bands[bandsIndex].GroupHeadersVisible = true;
            ug1.DisplayLayout.Bands[bandsIndex].Override.AllowGroupMoving = Infragistics.Win.UltraWinGrid.AllowGroupMoving.NotAllowed;
            ug1.DisplayLayout.Bands[bandsIndex].Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
        }

        /// <summary>设置UltraGrid字段汇总</summary>
        public static void SetSum(Infragistics.Win.UltraWinGrid.UltraGrid ug1, string columnsName, int bandsIndex = 0)
        {

            ug1.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.BasedOnDataType;
            if (ug1.DisplayLayout.Bands[bandsIndex].Columns.Exists(columnsName))
            {
                Infragistics.Win.UltraWinGrid.UltraGridColumn colAmount = ug1.DisplayLayout.Bands[bandsIndex].Columns[columnsName];
                Infragistics.Win.UltraWinGrid.SummarySettings summarySettings = ug1.DisplayLayout.Bands[bandsIndex].Summaries.Add(columnsName, Infragistics.Win.UltraWinGrid.SummaryType.Sum, colAmount, Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn);
                summarySettings.SummaryPositionColumn = summarySettings.SourceColumn;
                summarySettings.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                summarySettings.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
                summarySettings.ShowCalculatingText = Infragistics.Win.DefaultableBoolean.False;
                summarySettings.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed;
                //summarySettings_Amount.DisplayFormat = "Total = {0:###.00}";
            }
        }

        /// <summary> 初始化EpiUltraGrid字段下拉列表</summary>
        public static void InitEpiUltraGridDDL(Infragistics.Win.UltraWinGrid.UltraGrid ug1, string colName, DataTable dt, string valueColName, string textColName = null, int bandsIndex = 0, Infragistics.Win.UltraWinGrid.ColumnStyle columnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDown, System.Collections.Generic.List<string> fields = null)
        {
            if (textColName == null) textColName = valueColName;
            if (dt != null && dt.Columns.Contains(valueColName) && dt.Columns.Contains(textColName))
            {
                Infragistics.Win.UltraWinGrid.UltraDropDown ultraDropDown = new Infragistics.Win.UltraWinGrid.UltraDropDown();
                if (fields == null || fields.Count == 0)
                {
                    fields = new System.Collections.Generic.List<string>();
                    if (dt.Columns.Contains(valueColName)) fields.Add(valueColName);
                    if (valueColName != textColName && dt.Columns.Contains(textColName)) fields.Add(textColName);
                    dt.DefaultView.RowFilter = "";
                    ultraDropDown.DataSource = dt.DefaultView.ToTable(true, fields.ToArray());
                    ultraDropDown.ValueMember = valueColName;
                    ultraDropDown.DisplayMember = textColName;
                    ultraDropDown.DisplayLayout.Bands[0].Columns[valueColName].Hidden = (valueColName != textColName);
                    ultraDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
                }
                else
                {
                    dt.DefaultView.RowFilter = "";
                    ultraDropDown.DataSource = dt.DefaultView.ToTable(true, fields.ToArray());
                    ultraDropDown.ValueMember = valueColName;
                    ultraDropDown.DisplayMember = textColName;
                    ultraDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
                }
                ug1.DisplayLayout.Bands[bandsIndex].Columns[colName].Style = columnStyle;
                ug1.DisplayLayout.Bands[bandsIndex].Columns[colName].ValueList = ultraDropDown;
                ug1.DisplayLayout.Bands[bandsIndex].Columns[colName].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                ug1.DisplayLayout.Bands[bandsIndex].Columns[colName].AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            }
        }

        /// <summary> 初始化EpiUltraGrid字段下拉列表</summary>
        public static void InitEpiUltraGridDDL1(Infragistics.Win.UltraWinGrid.UltraGrid ug1, string colName, DataTable dt, string valueColName, string textColName = null, int bandsIndex = 0, Infragistics.Win.UltraWinGrid.ColumnStyle columnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDown)
        {
            if (textColName == null) textColName = valueColName;
            if (dt != null && dt.Columns.Contains(valueColName) && dt.Columns.Contains(textColName))
            {
                string valueListName = colName + "List";
                if (ug1.DisplayLayout.ValueLists.Contains(valueListName))
                {
                    ug1.DisplayLayout.ValueLists.Remove(valueListName);
                }
                Infragistics.Win.ValueList valueList = ug1.DisplayLayout.ValueLists.Add(valueListName);
                if (dt != null)
                {
                    System.Collections.Hashtable ht = new System.Collections.Hashtable();
                    foreach (DataRow dr in dt.Rows)
                    {
                        string v = Convert.ToString(dr[valueColName]);
                        if (ht[v] == null)
                        {
                            valueList.ValueListItems.Add(dr[valueColName], Convert.ToString(dr[textColName]));
                            ht[v] = true;
                        }
                    }
                }
                ug1.DisplayLayout.Bands[bandsIndex].Columns[colName].Style = columnStyle;
                ug1.DisplayLayout.Bands[bandsIndex].Columns[colName].ValueList = valueList;
                ug1.DisplayLayout.Bands[bandsIndex].Columns[colName].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                ug1.DisplayLayout.Bands[bandsIndex].Columns[colName].AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            }
        }
        public static void InitEpiUltraGridCellDDL(Infragistics.Win.UltraWinGrid.UltraGridCell cell, DataTable dt, string valueColName, string textColName = null, Infragistics.Win.UltraWinGrid.ColumnStyle columnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDown, System.Collections.Generic.List<string> fields = null)
        { 
            if (textColName == null) textColName = valueColName;
            if (dt != null && dt.Columns.Contains(valueColName) && dt.Columns.Contains(textColName))
            {
                Infragistics.Win.UltraWinGrid.UltraDropDown ultraDropDown = new Infragistics.Win.UltraWinGrid.UltraDropDown();
                //EpiUltraCombo ultraDropDown = new EpiUltraCombo();
                if (fields == null || fields.Count == 0)
                {
                    fields = new System.Collections.Generic.List<string>();
                    if (dt.Columns.Contains(valueColName)) fields.Add(valueColName);
                    if (valueColName != textColName && dt.Columns.Contains(textColName)) fields.Add(textColName);
                }
                if (fields.Count > 0)
                {
                    dt.DefaultView.RowFilter = "";
                    DataTable dt1 = dt.DefaultView.ToTable(true, fields.ToArray());
                    if (dt1.Rows.Count == 0) dt1.Rows.Add(dt1.NewRow());
                    ultraDropDown.DataSource = dt1;
                    ultraDropDown.ValueMember = valueColName;
                    ultraDropDown.DisplayMember = textColName;
                    ultraDropDown.DisplayLayout.Bands[0].Columns[valueColName].Hidden = (valueColName != textColName);
                    ultraDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
                    cell.Style = columnStyle;
                    cell.ValueList = ultraDropDown;
                    cell.Column.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                    cell.Column.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
                }
            }
            else
            {
                cell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                cell.ValueList = null;
            }
        }

        /// <summary> 初始化EpiNumericEditor数据格式</summary>
        public static void InitEpiNumericEditor(EpiNumericEditor epiNE, int n = 0, int m = 9, bool fg = true)
        {
            string str1 = "##0";// "###,###,##0";
            string str2 = "-nnn";// "-nnn,nnn,nnn";
            m -= 3;
            while (m > 0)
            {
                str1 = "###" + (fg ? "," : "") + str1;
                str2 = str2 + (fg ? "," : "") + "nnn";
                m -= 3;
            }
            if (n > 0)
            {
                str1 += ".";
                str2 += ".";
                str1 = str1.PadRight(str1.Length + n, '0');
                str2 = str2.PadRight(str2.Length + n, 'n');
            }  
            if (n > 0) epiNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            else epiNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Integer;
            epiNE.FormatString = str1;
            epiNE.MaskInput = str2;
        }

        /// <summary> 批量初始化GroupBox中的EpiNumericEditor数据格式</summary>
        public static void InitEpiNumericEditor(EpiGroupBox groupBox, int n = 0, int m = 9, bool fg = true, System.Collections.Generic.List<string> controlName=null)
        {
            foreach (Control control in groupBox.Controls)
            {
                if (control is EpiNumericEditor)
                {
                    if (controlName==null || (controlName != null && controlName.Contains(control.Name))) {
                          InitEpiNumericEditor((EpiNumericEditor)control, n, m, fg);
                    }
                }
            }
        }
        /// <summary> 初始化EpiDateTime数据格式</summary>
        public static void InitEpiDateTimeEditor(EpiDateTimeEditor epiDTE, string maskInput = "dd/mm/yyyy", string formatString = "dd/MM/yyyy")
        {
            epiDTE.MaskInput = maskInput;
            epiDTE.FormatString = formatString;
        }

        /// <summary> 批量初始化GroupBox中的EpiDateTime数据格式</summary>
        public static void InitEpiDateTimeEditor(EpiGroupBox groupBox, string maskInput = "dd/mm/yyyy", string formatString = "dd/MM/yyyy")
        {
            foreach (var control in groupBox.Controls)
            {
                if (control is EpiDateTimeEditor)
                {
                    InitEpiDateTimeEditor((EpiDateTimeEditor)control, maskInput, formatString);
                }
            }
        }

        /// <summary> 批量初始化EpiDateTime数据格式</summary>
        public static void InitEpiDateTimeEditor(Ice.Lib.Customization.CustomScriptManager csm, string maskInput = "dd/mm/yyyy", string formatString = "dd/MM/yyyy")
        {
            foreach (object control in csm.PersonalizeCustomizeManager.ControlsHT.Values)
            {
                if (control is EpiDateTimeEditor)
                {
                    InitEpiDateTimeEditor((EpiDateTimeEditor)control, maskInput, formatString);
                }
            }
        }
        /// <summary> 初始时EpiDataView设置columns只读</summary>
        public static void SetEDVReadOnly(EpiDataView edv, string[] columns, bool readOnly = true)
        {
            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>(columns);
            SetEDVReadOnly(edv, list);
        }
        /// <summary> 初始时EpiDataView设置columns只读</summary>
        public static void SetEDVReadOnly(EpiDataView edv, System.Collections.Generic.List<string> columns, bool readOnly = true)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                if (edv.dataView.Table.Columns.Contains(columns[i]))
                {
                    edv.dataView.Table.Columns[columns[i]].ExtendedProperties["ReadOnly"] = readOnly;
                }
            }
        }
        /// <summary> 初始时EpiDataView设置列只读，除columns中的列外</summary>
        public static DataTable SetEDVReadWrite(EpiDataView edv, string[] columns)
        {
            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>(columns);
            return SetEDVReadWrite(edv, list);
        }
        /// <summary> 初始时EpiDataView设置列只读，除columns中的列外</summary>
        public static DataTable SetEDVReadWrite(EpiDataView edv, System.Collections.Generic.List<string> columns)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ColumnName", typeof(string));
            dt.Columns.Add("Attribute", typeof(string));
            dt.Columns.Add("Value", typeof(bool));
            for (int i = 0; i < edv.dataView.Table.Columns.Count; i++)
            {
                string columnName = edv.dataView.Table.Columns[i].ColumnName;
                dt.Rows.Add(columnName, "ReadOnly", Convert.ToBoolean(edv.dataView.Table.Columns[i].ExtendedProperties["ReadOnly"]));
                edv.dataView.Table.Columns[columnName].ExtendedProperties["ReadOnly"] = !columns.Contains(columnName);
            }
            return dt;
        }
        /// <summary> 通过RowRule设置EpiDataView的columns只读</summary>
        public static void SetEDVReadOnly(EpiTransaction oTrans, string viewName, System.Collections.Generic.List<string> columns, Ice.Lib.ExtendedProps.RowRuleConditionDelegate2 rowRuleConditionDelegate2)
        {
            Ice.Lib.ExtendedProps.RowRule rr1 = new Ice.Lib.ExtendedProps.RowRule(null, rowRuleConditionDelegate2, null);
            for (int i = 0; i < columns.Count; i++)
            {
                rr1.AddAction(Ice.Lib.ExtendedProps.RuleAction.AddControlSettings(oTrans, viewName + "." + columns[i], Ice.Lib.ExtendedProps.SettingStyle.ReadOnly));
            }
        ((EpiDataView)(oTrans.EpiDataViews[viewName])).AddRowRule(rr1);
        }

        /// <summary> 通过RowRule设置EpiDataView中列只读，除columns中的列外</summary>
        public static void SetEDVReadWrite(EpiTransaction oTrans, string viewName, System.Collections.Generic.List<string> columns, Ice.Lib.ExtendedProps.RowRuleConditionDelegate2 rowRuleConditionDelegate2)
        {
            EpiDataView edv = (EpiDataView)(oTrans.EpiDataViews[viewName]);
            Ice.Lib.ExtendedProps.RowRule rr1 = new Ice.Lib.ExtendedProps.RowRule(null, rowRuleConditionDelegate2, null);
            for (int i = 0; i < edv.dataView.Table.Columns.Count; i++)
            {
                string columnName = edv.dataView.Table.Columns[i].ColumnName;
                if (columns.Contains(columnName) == false)
                {
                    rr1.AddAction(Ice.Lib.ExtendedProps.RuleAction.AddControlSettings(oTrans, viewName + "." + columnName, Ice.Lib.ExtendedProps.SettingStyle.ReadOnly));
                }
            }
            ((EpiDataView)(oTrans.EpiDataViews[viewName])).AddRowRule(rr1);
        }

        /// <summary> 通过RowRule设置EpiDataView整行只读</summary>
        public static void SetEDVReadOnlyRow(EpiTransaction oTrans, string viewName, Ice.Lib.ExtendedProps.RowRuleConditionDelegate2 rowRuleConditionDelegate2)
        {
            Ice.Lib.ExtendedProps.RowRule rr1 = new Ice.Lib.ExtendedProps.RowRule(null, rowRuleConditionDelegate2, null);
            rr1.AddAction(Ice.Lib.ExtendedProps.RuleAction.DisableRow(oTrans, viewName));
            ((EpiDataView)(oTrans.EpiDataViews[viewName])).AddRowRule(rr1);
        }

        /// <summary> 通过RowRule设置背景色和字体色</summary>
        public static void SetEDVBackColor(EpiTransaction oTrans, string viewName, Ice.Lib.ExtendedProps.RowRuleConditionDelegate2 rowRuleConditionDelegate2, System.Drawing.Color backColor, System.Drawing.Color foreColor, string styleSetName, System.Collections.Generic.List<string> excludeColumns)
        {
            if (excludeColumns == null) excludeColumns = new System.Collections.Generic.List<string>();
             EpiDataView edv = (EpiDataView)(oTrans.EpiDataViews[viewName]);
            Ice.Lib.ExtendedProps.RowRule rr1 = new Ice.Lib.ExtendedProps.RowRule(null, rowRuleConditionDelegate2, null); 
            Ice.Lib.ExtendedProps.ControlSettings controlSettings = new Ice.Lib.ExtendedProps.ControlSettings();
            controlSettings.BackColor = backColor;
            controlSettings.ForeColor = foreColor;
            controlSettings.StyleSetName = "MyStyle" + styleSetName;
            for (int i = 0; i < edv.dataView.Table.Columns.Count; i++)
            {
                string columnName = edv.dataView.Table.Columns[i].ColumnName;
                if (excludeColumns.Contains(columnName) == false)
                {
                    rr1.AddAction(Ice.Lib.ExtendedProps.RuleAction.AddControlSettings(oTrans, viewName + "." + columnName, controlSettings));
                }
            }
            ((EpiDataView)(oTrans.EpiDataViews[viewName])).AddRowRule(rr1);
        }
        /// <summary> 通过RowRule设置字体色</summary>
        public static void SetEDVForeColor(EpiTransaction oTrans, string viewName, Ice.Lib.ExtendedProps.RowRuleConditionDelegate2 rowRuleConditionDelegate2, System.Drawing.Color foreColor, string styleSetName, System.Collections.Generic.List<string> excludeColumns)
        {
            if (excludeColumns == null) excludeColumns = new System.Collections.Generic.List<string>();
            EpiDataView edv = (EpiDataView)(oTrans.EpiDataViews[viewName]);
            Ice.Lib.ExtendedProps.RowRule rr1 = new Ice.Lib.ExtendedProps.RowRule(null, rowRuleConditionDelegate2, null);
            Ice.Lib.ExtendedProps.ControlSettings controlSettings = new Ice.Lib.ExtendedProps.ControlSettings();
            controlSettings.ForeColor = foreColor;
            controlSettings.StyleSetName = "MyStyle" + styleSetName;
            for (int i = 0; i < edv.dataView.Table.Columns.Count; i++)
            {
                string columnName = edv.dataView.Table.Columns[i].ColumnName;
                if (excludeColumns.Contains(columnName) == false)
                {
                    rr1.AddAction(Ice.Lib.ExtendedProps.RuleAction.AddControlSettings(oTrans, viewName + "." + columnName, controlSettings));
                }
            }
            ((EpiDataView)(oTrans.EpiDataViews[viewName])).AddRowRule(rr1);
        }
        //<summary> 通过RowRule设置EpiDataView,根据自定义方法返回值,执行自定义行为</summary>
        public static void SetEDVRuleAction(EpiTransaction oTrans, string viewName, Ice.Lib.ExtendedProps.RowRuleConditionDelegate2 rowRuleConditionDelegate2, Ice.Lib.ExtendedProps.RowRuleActionDelegate2 rowRuleActionDelegate2)
        {
            Ice.Lib.ExtendedProps.RowRule rr1 = new Ice.Lib.ExtendedProps.RowRule(null, rowRuleConditionDelegate2, null, rowRuleActionDelegate2, null);
            ((EpiDataView)(oTrans.EpiDataViews[viewName])).AddRowRule(rr1);
        }
        /// <summary> EpiDataView设置属性</summary>
        public static void SetEDVAttribute(EpiDataView edv, DataTable dt)
        {
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                string columnName = dt.Rows[r]["ColumnName"].ToString();
                string attribute = dt.Rows[r]["Attribute"].ToString();
                edv.dataView.Table.Columns[columnName].ExtendedProperties[attribute] = dt.Rows[r]["Value"];
            }
        }

        /// <summary> EpiDataView设置columns隐藏</summary>
        public static void SetEDVHidden(EpiDataView edv, System.Collections.Generic.List<string> columns)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                if (edv.dataView.Table.Columns.Contains(columns[i]))
                {
                    edv.dataView.Table.Columns[columns[i]].ExtendedProperties["IsHidden"] = true;
                }
            }
        }
        /// <summary> EpiDataView设置columns显示</summary>
        public static void SetEDVDisplay(EpiDataView edv, System.Collections.Generic.List<string> columns)
        {
            for (int i = 0; i < edv.dataView.Table.Columns.Count; i++)
            {
                edv.dataView.Table.Columns[i].ExtendedProperties["IsHidden"] = !columns.Contains(edv.dataView.Table.Columns[i].ColumnName);

            }
        }

        /// <summary>隐藏控件(默认隐藏UD界面左边控件)</summary>
        public static void HiddenControls(EpiBaseForm form, string[] hiddenControls = null)
        {
            if (hiddenControls == null)
            {
                hiddenControls = new string[] { "dockableWindows3", "_SonomaFormUnpinnedTabAreaLeft" };
            }
            for (int i = 0; i < form.Controls.Count; i++)
            {
                if (Array.IndexOf(hiddenControls, form.Controls[i].Name) >= 0)
                {
                    form.Controls[i].Visible = false;
                }
            }
        }

        /// <summary>绑定DataTable数据到EpiUltraCombo控件,valueColName为空则取第一列列名，displayColName为空则取第二列列名(第二列不存在时，取valueColName值)</summary>
        public static void LoadDropDown(EpiUltraCombo comControl, DataTable dt1, string valueColName = "", string displayColName = "", string[] fields = null, bool connect = false)
        {
            if (comControl == null) return;
            try
            {
                DataTable dt = dt1.Copy();
                if (string.IsNullOrEmpty(valueColName))
                {
                    if (dt.Columns.Count > 0)
                    {
                        valueColName = dt.Columns[0].ColumnName;
                    }
                    else return;
                }
                if (string.IsNullOrEmpty(displayColName))
                {
                    if (dt.Columns.Count > 1)
                    {
                        displayColName = dt.Columns[1].ColumnName;
                    }
                    else
                    {
                        displayColName = valueColName;
                    }
                }
                if (!dt.Columns.Contains(valueColName)) throw new Exception("Binding data exception, missing column '" + valueColName + "' in data source!");
                if (!dt.Columns.Contains(displayColName)) throw new Exception("Binding data exception, missing column '" + displayColName + "' in data source!");
                string[] columns = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++) columns[i] = dt.Columns[i].ColumnName;
                if (valueColName != displayColName && connect)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i][displayColName] = Convert.ToString(dt.Rows[i][valueColName]) + "->" + Convert.ToString(dt.Rows[i][displayColName]);
                    }
                }
                comControl.ValueMember = valueColName;
                comControl.DisplayMember = displayColName;
                dt.DefaultView.RowFilter = "";
                comControl.DataSource = dt.DefaultView.ToTable(true, columns);
                if (fields == null) fields = new string[] { displayColName };
                comControl.SetColumnFilter(fields);
                comControl.DropDownWidth = comControl.Width;
                comControl.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                comControl.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            }
            catch
            {
                comControl.DataSource = null;
            }
        }
        /// <summary>EpiButton设置显示图片 </summary>
        public static void SetButtomImage(EpiButton epiBt,string imageKey, Infragistics.Win.HAlign hAlign= Infragistics.Win.HAlign.Center, Infragistics.Win.VAlign vAlign= Infragistics.Win.VAlign.Middle) {
            epiBt.Appearance.Image = Ice.Lib.EpiUIImages.GetImage(imageKey, Ice.Lib.EpiUIImages.ImageSizes.Small);
            epiBt.Appearance.ImageHAlign = hAlign;
            epiBt.Appearance.ImageVAlign = vAlign;
        }
        /// <summary>EpiUltraGrid列添加EditButton</summary>
        public static void SetCellButton(EpiUltraGrid epiUG,string columnName, string imageName= "Search", Ice.Lib.EpiUIImages.ImageSizes imageSize= Ice.Lib.EpiUIImages.ImageSizes.Small)
        {
            System.Drawing.Image binocularsImageSmall = Ice.Lib.EpiUIImages.GetImage(imageName, imageSize);
            epiUG.DisplayLayout.Bands[0].Columns[columnName].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
            epiUG.DisplayLayout.Bands[0].Columns[columnName].CellButtonAppearance.Image = binocularsImageSmall;
        }

        /// <summary>判断行是否在编辑状态</summary>
        public static bool IsEditing(EpiDataView edv)
        {
            if (edv.Row >= 0)
            {
                if (edv.CurrentDataRow.RowState == DataRowState.Added || edv.CurrentDataRow.RowState == DataRowState.Modified) return true;
            }
            return false;
        }

        #endregion

        #region 一般方法
        public static string JoinString(this string thisStr, string joinStr, string separatorStr="")
        {
            if (joinStr != "")
            {
                if (thisStr != "" && separatorStr != "") thisStr += separatorStr;
                thisStr += joinStr;
            }
            return thisStr;
        }
        /// <summary>判断是否包含属性</summary>
        public static bool ContainProperty(object instance, string propertyName)
        {
            if (instance != null && !string.IsNullOrEmpty(propertyName))
            {
                System.Reflection.PropertyInfo _findedPropertyInfo = instance.GetType().GetProperty(propertyName);
                return (_findedPropertyInfo != null);
            }
            return false;
        }

        public static string String2Unicode(string source)
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(source);
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }
        public static bool IsNumeric(string str)
        {
            bool bl = false;
            int n = 0;
            for (int i1 = 0; i1 < str.Length; i1++) {
                if (str[i1] == '.') n++;
            }
            if (!string.IsNullOrEmpty(str) && n <= 1)
            { 
                bl = true;
                for (int i = 0; i < str.Length; i++)
                {
                    if ((str[i] < '0' || str[i] > '9') && str[i] != '.')
                    {
                        bl = false;
                        break;
                    }
                }
            }
            return bl;
        }

        public static void SetMaxLength(DataTable dt,int maxLength=2000) {
            foreach (DataColumn col in dt.Columns) {
                if (col.DataType==typeof(string))
                {
                    if (col.ExtendedProperties["MaxLength"] != null) {
                        maxLength = Convert.ToInt32(col.ExtendedProperties["MaxLength"]);
                    }
                    col.MaxLength = maxLength;
                }
            }
        }

        /// <summary>从fromDT复制相同列名的数据到toDT</summary>
        public static void CopyTable(DataTable fromDT, DataTable toDT, string filter = null)
        {
            DataRow[] drs = fromDT.Select(Convert.ToString(filter));
            for (int r = 0; r < drs.Length; r++)
            {
                DataRow dr = toDT.NewRow();
                CopyRow(drs[r], dr);
                toDT.Rows.Add(dr);
            }
        }


        /// <summary>使用fromDT相同列名的数据刷新toDT表数据</summary>
        public static void RefreshTable(DataTable fromDT, DataTable toDT, System.Collections.Generic.List<string> keyCols, System.Collections.Generic.List<string> refreshCols=null)
        {
            bool sysRowID = false;
            try
            {
                if (toDT.Columns.Contains("SysRowID")) sysRowID = true;
                else toDT.Columns.Add("SysRowID");
                if (keyCols == null || keyCols.Count == 0) return;
                if (refreshCols == null)
                {
                    refreshCols = new System.Collections.Generic.List<string> { };
                    for (int i = 0; i < fromDT.Columns.Count; i++)
                    {
                        if (!keyCols.Contains(fromDT.Columns[i].ColumnName)) refreshCols.Add(fromDT.Columns[i].ColumnName);
                    }
                }
                for (int i = 0; i < fromDT.Rows.Count; i++)
                {
                    string filterStr = "";
                    for (int k = 0; k < keyCols.Count; k++)
                    {
                        if (filterStr != "") filterStr += " And";
                        if (toDT.Columns[keyCols[k]].DataType == typeof(int) || toDT.Columns[keyCols[k]].DataType == typeof(decimal)
                            || toDT.Columns[keyCols[k]].DataType == typeof(bool))
                        {
                            filterStr += keyCols[k] + "=" + Convert.ToString(fromDT.Rows[i][keyCols[k]]);
                        }
                        else
                        {
                            filterStr += keyCols[k] + "='" + Convert.ToString(fromDT.Rows[i][keyCols[k]]) + "'";
                        }
                    }
                    DataRow[] drs = toDT.Select(filterStr);
                    if (drs.Length > 0)
                    {
                        for (int c = 0; c < refreshCols.Count; c++)
                        {
                            if (fromDT.Columns.Contains(refreshCols[c]) && toDT.Columns.Contains(refreshCols[c]))
                            {
                                drs[0][refreshCols[c]] = fromDT.Rows[i][refreshCols[c]];
                            }
                        }
                    }
                    else
                    {
                        DataRow row = toDT.NewRow();
                        CopyRow(fromDT.Rows[i], row);
                        toDT.Rows.Add(row);
                    }
                }
            }
            catch (Exception e1)
            {
                throw e1;
            }
            finally {
                if (sysRowID == false) toDT.Columns.Remove("SysRowID");
            }
        }

        /// <summary>从fromRow复制相同列名的数据到toRow,cols中的列不用复制</summary>
        public static void CopyRow(DataRow fromRow, DataRow toRow, System.Collections.Generic.List<string> excludeCols = null)
        {
            for (int i = 0; i < toRow.Table.Columns.Count; i++)
            {
                string columnName = toRow.Table.Columns[i].ColumnName;
                if ((excludeCols == null || excludeCols.Contains(columnName) == false) && fromRow.Table.Columns.Contains(columnName))
                {
                    toRow[columnName] = fromRow[columnName];
                }
            }
        }
        /// <summary>从fromRow复制相同列名的数据到toRow,只复制cols中的列</summary>
        public static void CopyRow1(DataRow fromRow, DataRow toRow, System.Collections.Generic.List<string> cols)
        {
            if (cols == null) return;
            for (int i = 0; i < toRow.Table.Columns.Count; i++)
            {
                string columnName = toRow.Table.Columns[i].ColumnName;
                if (cols.Contains(columnName) && fromRow.Table.Columns.Contains(columnName))
                {
                    toRow[columnName] = fromRow[columnName];
                }
            }
        }
        /// <summary>从fromRow复制相同列名的数据到toRow,cols中的列不用复制</summary>
        public static void CopyRow(Infragistics.Win.UltraWinGrid.UltraGridRow fromRow, DataRow toRow, System.Collections.Generic.List<string> excludeCols = null)
        {
            for (int i = 0; i < toRow.Table.Columns.Count; i++)
            {
                string columnName = toRow.Table.Columns[i].ColumnName;
                if ((excludeCols == null || excludeCols.Contains(columnName) == false) && fromRow.Cells.HasCell(columnName))
                {
                    toRow[columnName] = fromRow.Cells[columnName].Value;
                }
            }
        }
        /// <summary>复制UD表数据,从fromRow到toRow</summary>
        public static void CopyUDRow(DataRow fromRow, DataRow toRow)
        {
            System.Collections.Generic.List<string> cols = new System.Collections.Generic.List<string> { "SysRevID", "SysRowID", "UD_SysRevID", "ForeignSysRowID" };
            CopyRow(fromRow, toRow, cols);
        }
        /// <summary>去掉重复行</summary>
        public static DataTable TableDist(DataTable dt, string[] filedNames, string tableName = "Dist")
        {
            DataView dv = dt.DefaultView;
            if (dt.Rows.Count > 0)
            {
                DataTable DistTable = dv.ToTable(tableName,true, filedNames);
                return DistTable;
            }
            else return dt;
        }
        /// <summary>分组合计</summary>
        public static DataTable TableGroup(DataTable dt, string[] groupColumns, string[] sumColumns)
        {
            DataTable dtResult = null;
            if (dt.Rows.Count > 0)
            {
                dtResult = dt.DefaultView.ToTable(true, groupColumns);
                for (int j = 0; j < sumColumns.Length; j++)
                {
                    dtResult.Columns.Add(PubFun.CopyColumn(dt.Columns[sumColumns[j]]));
                }
                for (int i = 0; i < dtResult.Rows.Count; i++)
                {
                    string filterStr = "";
                    for (int j = 0; j < sumColumns.Length; j++)
                    {
                        dtResult.Rows[i][sumColumns[j]] = PubFun.ToDecimal(dt.Compute("sum(" + sumColumns[j] + ")", filterStr));
                    }
                }
            }
            return dtResult;
        }
        /// <summary>分组合计</summary>
        public static DataTable TableGroup1(DataTable dt, string[] groupColumns, string[] sumColumns,string column)
        {
            DataTable dtResult = null;
            if (dt.Rows.Count > 0)
            {
                dtResult = dt.DefaultView.ToTable(true, groupColumns);
                DataTable dt1 = dt.DefaultView.ToTable(true, column);
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    string columnValue = dt1.Rows[i][column].ToString();
                    for (int j = 0; j < sumColumns.Length; j++)
                    {
                        DataColumn col1 = PubFun.CopyColumn(dt.Columns[sumColumns[j]]);
                        col1.ColumnName = sumColumns[j] + "_" + columnValue;
                        col1.Caption = col1.Caption + columnValue;
                        dtResult.Columns.Add(col1);
                    }
                }
                for (int i = 0; i < dtResult.Rows.Count; i++)
                {
                    string filterStr = ""; //groupColumns[0]+"='"+ dtResult.Rows[i][groupColumns[0]].ToString() + "'";
                    for (int i2 = 0; i2 < groupColumns.Length; i2++) {//
                        if(filterStr=="") filterStr= groupColumns[i2] + "='" + dtResult.Rows[i][groupColumns[i2]].ToString() + "'";
                        else filterStr +=" and "+ groupColumns[i2] + "='" + dtResult.Rows[i][groupColumns[i2]].ToString() + "'";
                    }
                    for (int i1 = 0; i1 < dt1.Rows.Count; i1++)
                    {
                        string columnValue = dt1.Rows[i1][column].ToString();
                        string filterStr1 = " and " + column + "='" + columnValue + "'";
                        if(filterStr=="") filterStr1 =  column + "='" + columnValue + "'";
                        for (int j = 0; j < sumColumns.Length; j++)
                        {
                            string columnName = sumColumns[j] + "_" + columnValue;
                            dtResult.Rows[i][columnName] = PubFun.ToDecimal(dt.Compute("sum(" + sumColumns[j] + ")", filterStr+ filterStr1));
                        }
                    }
                }
            }
            return dtResult;
        }



        /// <summary></summary>
        public static void BeginLoadData(DataTable dt)
        {
            dt.ExtendedProperties["IsLoadData"] = true;
        }
        /// <summary></summary>
        public static void EndLoadData(DataTable dt)
        {
            dt.ExtendedProperties["IsLoadData"] = false;
        }
        /// <summary></summary>
        public static bool IsLoadData(DataTable dt)
        {
            return Convert.ToBoolean(dt.ExtendedProperties["IsLoadData"]);
        }

        public static decimal ToDecimal(object value)
        {
            return Convert.ToDecimal(Convert.IsDBNull(value) ? 0m : value);
        }
        /// <summary>获取下个字母</summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public static string GetNextSingleLetter(string letter)
        {
            if (letter.Length > 1) throw new Exception("It's not a single letter.");
            if (letter.ToUpper() == "Z") throw new Exception("It's the last letter.");
            int x = (int)Convert.ToByte(letter[0]);
            x = x + 1;
            return Convert.ToChar(x).ToString();
        }
        #endregion
    }

    public class PubDate
    {
        /// <summary>取得某月的第一天</summary>
        public static DateTime FirstDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }
        //// <summary>取得某月的最后一天</summary>
        public static DateTime LastDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
        }
        //// <summary>取得上个月的最后一天</summary>
        public static DateTime LastDayOfPreviousMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddDays(-1);
        }
        //// <summary>取得下个月的第一天</summary>
        public static DateTime FirstDayOfNextMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1);
        }
    }

    /// <summary>公共方法</summary>
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

    /// <summary>公共方法</summary>
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
    public class PubBAQ
    {

        /// <summary>从BAQ获取数据</summary>
        public static DataTable GetBAQData(Ice.Lib.Framework.EpiTransaction otrans, string queryID, Where[] queryWhere, bool clearWhere = false)
        {
            DataTable dt = null;
            Ice.Adapters.DynamicQueryAdapter dqa = new Ice.Adapters.DynamicQueryAdapter(otrans);
            dqa.SearchForm = new Ice.Lib.Searches.EpiSearchBase();
            try
            {
                dqa.BOConnect();
                bool bl = dqa.GetByID(queryID);
                if (bl)
                {
                    if (clearWhere) dqa.QueryDesignData.QueryWhereItem.Clear();
                    Ice.BO.QueryExecutionDataSet qed = new Ice.BO.QueryExecutionDataSet();
                    if (queryWhere != null)
                    {
                        for (int r = 0; r < queryWhere.Length; r++)
                        {
                            if (queryWhere[r].TableID.Equals("@@Parameters"))
                            {
                                qed.ExecutionParameter.AddExecutionParameterRow(
                                    queryWhere[r].FieldName,
                                    queryWhere[r].RValue,
                                    queryWhere[r].DataType,
                                    false,
                                    Guid.Empty,
                                    "A");
                            }
                            else
                            {
                                Ice.BO.DynamicQueryDataSet.QueryWhereItemRow row1 = queryWhere[r].GetNewQueryWhereItemRow(dqa);
                                dqa.QueryDesignData.QueryWhereItem.Rows.Add(row1);
                            }

                        }
                    }

                    dqa.Execute(dqa.QueryDesignData, qed);
                    if (dqa.QueryResults.Tables["Errors"].Rows.Count > 0)
                    {
                        throw new Exception(queryID + "," + Convert.ToString(dqa.QueryResults.Tables["Errors"].Rows[0]["ErrorText"]));
                    }
                    dt = dqa.QueryResults.Tables["Results"];
                }
            }
            finally
            {
                dqa.Dispose();
            }
            return dt;
        }
        public static DataTable GetBAQData(Ice.Lib.Framework.EpiTransaction otrans, string queryID, Parameter[] queryParameter, bool clearParameter = true)
        {
            DataTable result = null;
            try
            {
                using (Ice.Adapters.DynamicQueryAdapter dqa = new Ice.Adapters.DynamicQueryAdapter(otrans))
                {
                    dqa.BOConnect();
                    Ice.BO.QueryExecutionDataSet dsQueryExecution = dqa.GetQueryExecutionParametersByID(queryID);
                    if (clearParameter) dsQueryExecution.ExecutionParameter.Clear();
                    if (queryParameter != null)
                    {
                        for (int r = 0; r < queryParameter.Length; r++)
                        {
                            dsQueryExecution.ExecutionParameter.AddExecutionParameterRow(queryParameter[r].ParameterID, queryParameter[r].ParameterValue, queryParameter[r].ValueType, queryParameter[r].IsEmpty, Guid.NewGuid(), "A");
                        }
                    }
                    dqa.ExecuteByID(queryID, dsQueryExecution);
                    if (dqa.QueryResults.Tables["Errors"].Rows.Count > 0)
                    {
                        throw new Exception(queryID + "," + Convert.ToString(dqa.QueryResults.Tables["Errors"].Rows[0]["ErrorText"]));
                    }
                    result = dqa.QueryResults.Tables["Results"];
                }
            }
            catch (Exception ex)
            {
                throw new Exception(queryID + "," + ex.Message);
            }
            return result;
        }
        public static DataTable GetBAQDataExe(Ice.Lib.Framework.EpiTransaction otrans, string queryID, Parameter[] queryParameter, bool clearParameter = true)
        {

            DataTable result = null;
            try
            {
                using (Ice.Adapters.DynamicQueryAdapter dqa = new Ice.Adapters.DynamicQueryAdapter(otrans))
                {
                    dqa.BOConnect();
                     dqa.GetByID(queryID);
                    Ice.BO.DynamicQueryDataSet ds = dqa.QueryDesignData;
                    Ice.BO.QueryExecutionDataSet dsQueryExecution = dqa.GetQueryExecutionParameters(ds);
                    if (clearParameter) dsQueryExecution.ExecutionParameter.Clear();
                    if (queryParameter != null)
                    {
                        for (int r = 0; r < queryParameter.Length; r++)
                        {
                            dsQueryExecution.ExecutionParameter.AddExecutionParameterRow(queryParameter[r].ParameterID, queryParameter[r].ParameterValue, queryParameter[r].ValueType, queryParameter[r].IsEmpty, Guid.NewGuid(), "A");
                        }
                    }
                    dqa.Execute(ds, dsQueryExecution);
                    if (dqa.QueryResults.Tables["Errors"].Rows.Count > 0)
                    {
                        throw new Exception(queryID + "," + Convert.ToString(dqa.QueryResults.Tables["Errors"].Rows[0]["ErrorText"]));
                    }
                    result = dqa.QueryResults.Tables["Results"];
                }
            }
            catch (Exception ex)
            {
                throw new Exception(queryID + "," + ex.Message);
            }
            return result;
        }
        /// <summary>从BAQ获取数据</summary>
        public static DataTable GetBAQData(Ice.Lib.Framework.EpiTransaction otrans, string queryID, Where[] queryWhere, Parameter[] queryParameter, bool clearWhere = false, bool clearParameter = true)
        {
            DataTable dt = null;
            Ice.Adapters.DynamicQueryAdapter dqa = new Ice.Adapters.DynamicQueryAdapter(otrans);
            dqa.SearchForm = new Ice.Lib.Searches.EpiSearchBase();
            try
            {
                dqa.BOConnect();
                bool bl = dqa.GetByID(queryID);
                if (bl)
                {
                    if (clearWhere) dqa.QueryDesignData.QueryWhereItem.Clear();
                    if (queryWhere != null)
                    {
                        for (int r = 0; r < queryWhere.Length; r++)
                        {
                            Ice.BO.DynamicQueryDataSet.QueryWhereItemRow row1 = queryWhere[r].GetNewQueryWhereItemRow(dqa);
                            dqa.QueryDesignData.QueryWhereItem.Rows.Add(row1);
                        }
                    }
                    Ice.BO.QueryExecutionDataSet qed = dqa.GetQueryExecutionParameters(dqa.QueryDesignData);
                    if (clearParameter) qed.ExecutionParameter.Clear();
                    if (queryParameter != null)
                    {
                        for (int r = 0; r < queryParameter.Length; r++)
                        {
                            qed.ExecutionParameter.AddExecutionParameterRow(queryParameter[r].ParameterID, queryParameter[r].ParameterValue, queryParameter[r].ValueType, queryParameter[r].IsEmpty, Guid.NewGuid(), "A");
                        }
                    }
                    dqa.Execute(dqa.QueryDesignData, qed);
                    if (dqa.QueryResults.Tables["Errors"].Rows.Count > 0)
                    {
                        throw new Exception(queryID + "," + Convert.ToString(dqa.QueryResults.Tables["Errors"].Rows[0]["ErrorText"]));
                    }
                    dt = dqa.QueryResults.Tables["Results"];
                }
            }
            finally
            {
                dqa.Dispose();
            }
            return dt;
        }

        public static void AddListParameter(System.Collections.Generic.List<PubBAQ.Parameter> list, string parameterName, string[] items, string dataType)
        {
            for (int i = 0; i < items.Length; i++)
            {
                list.Add(new PubBAQ.Parameter(parameterName, items[i], dataType));
            }
        }
        public class DataType
        {
            public static string Nvarchar = "nvarchar";
            public static string Bit = "bit";
            public static string Date = "date";
            public static string Decimal = "decimal";
            public static string Int = "int";
        }

        /// <summary>BAQ筛选条件</summary>
        public class Where
        {
            /// <summary>公司</summary>
            public string Company { get; set; }
            /// <summary>BAQ</summary>
            public string QueryID { get; set; }
            /// <summary></summary>
            public Guid SubQueryID { get; set; }
            /// <summary>表名称</summary>
            public string TableID { get; set; }
            /// <summary></summary>
            public Guid CriteriaID { get; set; }
            /// <summary>序号</summary>
            public int Seq { get; set; }
            /// <summary>表字段名称</summary>
            public string FieldName { get; set; }
            /// <summary>表字段类型(nvarchar,bit,date,decimal,int...)</summary>
            public string DataType { get; set; }
            /// <summary>操作符</summary>
            public string CompOp { get; set; }
            /// <summary>And,Or</summary>
            public string AndOr { get; set; }
            /// <summary></summary>
            public bool Neg { get; set; }
            /// <summary>左边符号(空,"(")</summary>
            public string LeftP { get; set; }
            /// <summary>右边符号(空,")")</summary>
            public string RightP { get; set; }
            /// <summary></summary>
            public bool IsConst { get; set; }
            /// <summary></summary>
            public int CriteriaType { get; set; }
            /// <summary></summary>
            public string ToTableID { get; set; }
            /// <summary></summary>
            public string ToFieldName { get; set; }
            /// <summary></summary>
            public string ToDataType { get; set; }
            /// <summary>筛选值</summary>
            public string RValue { get; set; }
            /// <summary></summary>
            public bool ExtSecurity { get; set; }
            /// <summary></summary>
            public long SysRevID { get; set; }
            /// <summary></summary>
            public Guid SysRowID { get; set; }
            /// <summary></summary>
            public int BitFlag { get; set; }
            /// <summary></summary>
            public string RowMod { get; set; }
            /// <summary>初始化函数(tableID和fieldName不带前缀)</summary>
            public Where(string tableID, string fieldName, string rValue, string dataType, string andOr = "", string compOp = "=", string leftP = "", string rightP = "", int seq = 0, string toDataType = "")
            {
                TableID = tableID;
                Seq = seq;
                FieldName = fieldName;
                DataType = dataType;
                CompOp = compOp;
                AndOr = andOr;
                Neg = false;
                LeftP = leftP;
                RightP = rightP;
                IsConst = true;
                CriteriaType = 2;
                ToTableID = "";
                ToFieldName = "";
                ToDataType = toDataType;
                RValue = rValue;
                ExtSecurity = false;
                BitFlag = 0;
                RowMod = "";
            }
            /// <summary>当前对象转DynamicQueryDataSet.QueryWhereItemRow</summary>
            public Ice.BO.DynamicQueryDataSet.QueryWhereItemRow GetNewQueryWhereItemRow(Ice.Adapters.DynamicQueryAdapter dqa)
            {
                Ice.BO.DynamicQueryDataSet.QueryWhereItemRow row1 = dqa.QueryDesignData.QueryWhereItem.NewQueryWhereItemRow();
                DataRow dr = dqa.QueryDesignData.QuerySubQuery.Select("Type='TopLevel'")[0];
                row1.Company = dr["Company"].ToString();
                row1.QueryID = dr["QueryID"].ToString();
                row1.SubQueryID = (Guid)dr["SubQueryID"];
                row1.TableID = TableID;
                DataRow[] drs = dqa.QueryDesignData.QueryWhereItem.Select("TableID='" + TableID.Replace("_UD","") + "' or TableID='" + TableID.Replace("_UD", "") + "_UD'");
                if (Seq == 0)
                {
                    if (drs.Length > 0)
                        Seq = Convert.ToInt32(dqa.QueryDesignData.QueryWhereItem.Compute("Max(Seq)", "TRUE")) + 1;
                    else Seq = 1;
                }
                row1.Seq = Seq;
                row1.FieldName = FieldName;
                row1.DataType = DataType;
                row1.CompOp = CompOp;
                if (AndOr == "" && drs.Length > 0) AndOr = "And";
                row1.AndOr = AndOr;
                row1.Neg = Neg;
                row1.LeftP = LeftP;
                row1.RightP = RightP;
                row1.IsConst = IsConst;
                row1.CriteriaType = CriteriaType;
                row1.ToTableID = ToTableID;
                row1.ToFieldName = ToFieldName;
                row1.ToDataType = ToDataType;
                row1.RValue = RValue;
                row1.ExtSecurity = ExtSecurity;
                row1.BitFlag = BitFlag;
                row1.RowMod = RowMod;
                return row1;
            }
            public override string ToString()
            {
                if (CompOp.ToLower() == "in")
                {
                    if (!RValue.StartsWith("'"))
                    {
                        RValue = string.Format("'{0}'", RValue.Replace(",", "','"));
                    }
                    return " " + AndOr + " " + LeftP + FieldName + " " + CompOp + " (" + RValue + ")" + RightP;
                }
                else return " " + AndOr + " " + LeftP + FieldName + " " + CompOp + " '" + RValue + "'" + RightP;
            }
        }

        public class Parameter
        {

            public string ParameterID { get; set; }
            public string ParameterValue { get; set; }
            public string ValueType { get; set; }
            public bool IsEmpty { get; set; }
            public Parameter(string parameterID, string parameterValue, string valueType, bool isEmpty = false)
            {
                ParameterID = parameterID;
                ParameterValue = parameterValue;
                ValueType = valueType;
                IsEmpty = isEmpty;
            }
        }
    }


















