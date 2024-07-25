public class PubAdp
{

    public static bool GetFastReportFile(EpiTransaction otrans, string baqRptID, out string baqID, out byte[] frFileByte,out string editor)
    {
        PubAdp adp = new PubAdp(otrans, "DynamicReport");
        baqID = "";
        bool bl = false;
        frFileByte = null;
        editor = "";
        try
        {
            adp.BOConnect();
            if (adp.GetByID(new object[] { baqRptID }, new Type[] { typeof(string) }))
            {
                DataSet ds1 = adp.GetCurrentDataSet();
                if (ds1.Tables["BAQReport"].Rows.Count > 0)
                {
                    string frFile = ds1.Tables["BAQReport"].Rows[0]["FRFile_c"].ToString();
                    baqID = ds1.Tables["BAQReport"].Rows[0]["ExportID"].ToString();
                    editor = ds1.Tables["BAQReport"].Rows[0]["Editor_c"].ToString();
                    frFileByte = Convert.FromBase64String(frFile);
                    bl = true;
                }
            }
        }
        finally
        {
            adp.Dispose();
        }
        return bl;
    }
    public static bool SetFastReportFile(EpiTransaction otrans, string baqRptID, byte[] frFileByte)
    {
        PubAdp adp = new PubAdp(otrans, "DynamicReport");
        bool bl = false;
        try
        {
            adp.BOConnect();
            if (adp.GetByID(new object[] { baqRptID }, new Type[] { typeof(string) }))
            {
                string frFile = Convert.ToBase64String(frFileByte);
                bl= adp.UpdateColumnValue("BAQReport", "FRFile_c", frFile);
            }
        }
        finally
        {
            adp.Dispose();
        }
        return bl;
    }
    ///<summary>获取适配器数据(反射)</summary>
    public static DataTable GetAdapterData(EpiTransaction otrans, string adpName, Ice.Lib.Searches.SearchOptions opts = null, string tableName = null)
    {
        PubAdp adp = new PubAdp(otrans, adpName);
        DataTable dt = new DataTable();
        try
        {
            adp.BOConnect();
            if (opts == null)
            {
                opts = Ice.Lib.Searches.SearchOptions.CreateRuntimeSearch(new System.Collections.Hashtable(), Ice.Lib.Searches.DataSetMode.RowsDataSet);
            }
            DialogResult result = adp.InvokeSearch(opts);
            if (result == DialogResult.OK)
            {
                DataSet ds1 = adp.GetCurrentDataSet();
                if (tableName == null)
                {
                    dt = ds1.Tables[0].Copy();
                }
                else
                {
                    dt = ds1.Tables[tableName].Copy();
                }
            }
        }
        finally
        {
            adp.Dispose();
        }
        return dt;
    }
    ///<summary>获取适配器数据(反射)</summary>
    public static DataTable GetAdapterData(EpiTransaction otrans, string adpName, string whereStr, string tableName = null)
    {
        PubAdp adp = new PubAdp(otrans, adpName);
        DataTable dt = new DataTable();
        try
        {
            adp.BOConnect();
            System.Collections.Hashtable ht = new System.Collections.Hashtable();
            ht.Add(adpName.TrimEnd("Adapter".ToCharArray()), whereStr);
            Ice.Lib.Searches.SearchOptions opts = Ice.Lib.Searches.SearchOptions.CreateRuntimeSearch(ht, Ice.Lib.Searches.DataSetMode.RowsDataSet);
            DialogResult result = adp.InvokeSearch(opts);
            if (result == DialogResult.OK)
            {
                DataSet ds1 = adp.GetCurrentDataSet();
                if (tableName == null)
                {
                    dt = ds1.Tables[0].Copy();
                }
                else
                {
                    dt = ds1.Tables[tableName].Copy();
                }
            }
        }
        finally
        {
            adp.Dispose();
        }
        return dt;
    }
    /// <summary> 调用快速搜索界面 </summary>
    public static object OpenQuickSearchForm(object otrans, string QuickSearchID, bool multiSelect = false, int pageSize = 0, DataTable ControlSetting = null)
    {
        PubAdp adp = new PubAdp(otrans, "QuickSearch");
        object obj = null;
        try
        {
            adp.BOConnect();
            obj = adp.InvokeFun("ShowQuickSearchForm", new object[] { otrans, QuickSearchID, multiSelect, pageSize, ControlSetting }, new Type[] { typeof(EpiTransaction), typeof(string), typeof(bool), typeof(int), typeof(DataTable) });
        }
        finally
        {
            adp.Dispose();
        }
        return obj;
    }
    public static string GetNewCode(EpiTransaction otrans, string ruleCode,string tableName = "UD01", string _formatCode = "N0000000", string _resetType = "NoReset")
    {
        string code = ""; 
        PubAdp adp = new PubAdp(otrans, tableName);
        if (CheckNumberingRule(adp, ruleCode, tableName, _formatCode, _resetType))
        {
            try
            {
                string formatCode = Convert.ToString(adp.GetColumnValue(tableName, "Character02"));
                int nextNum = Convert.ToInt32(adp.GetColumnValue(tableName, "Number01"));
                string resetType = Convert.ToString(adp.GetColumnValue(tableName, "ShortChar01"));
                int year = Convert.ToInt32(adp.GetColumnValue(tableName, "Number02"));
                int month = Convert.ToInt32(adp.GetColumnValue(tableName, "Number03"));
                int day= Convert.ToInt32(adp.GetColumnValue(tableName, "Number04"));
                if (resetType.ToLower() == "Year".ToLower() || resetType.ToLower() == "Month".ToLower() || resetType == "Day".ToLower())
                {

                    if (resetType.ToLower() == "Year".ToLower() && DateTime.Today.Year != year)
                    {
                        nextNum = 1;
                        year = DateTime.Today.Year;
                    }
                    else if (resetType.ToLower() == "Month".ToLower() && (DateTime.Today.Year != year || DateTime.Today.Month != month))
                    {
                        nextNum = 1;
                        year = DateTime.Today.Year;
                        month = DateTime.Today.Month;
                    }
                    else if (resetType.ToLower() == "Day".ToLower() && (DateTime.Today.Year != year || DateTime.Today.Month != month || DateTime.Today.Day != day))
                    {
                        nextNum = 1;
                        year = DateTime.Today.Year;
                        month = DateTime.Today.Month;
                        day= DateTime.Today.Day;
                    }
                }
                if (string.IsNullOrEmpty(formatCode))
                {
                    code = nextNum.ToString();
                }
                else
                {
                    string qz = formatCode.TrimEnd('0');
                    int length = formatCode.Length - qz.Length;
                    code = qz + nextNum.ToString().PadLeft(length, '0');
                    code = code.Replace("yyyy", DateTime.Today.Year.ToString("0000"));
                    code = code.Replace("yy", DateTime.Today.Year.ToString("0000").Substring(2,2));
                    code = code.Replace("MM", DateTime.Today.Month.ToString("00"));
                    code = code.Replace("dd", DateTime.Today.Day.ToString("00"));
                }
                System.Collections.Generic.Dictionary<string, object> keyValues = new System.Collections.Generic.Dictionary<string, object>();
                keyValues.Add("Number01", nextNum + 1);
                keyValues.Add("Number02", year);
                keyValues.Add("Number03", month);
                keyValues.Add("Number04", day);
                adp.UpdateValues(tableName, keyValues);
            }
            catch (Exception e1)
            {
                throw new Exception("Get new code(" + ruleCode + ") error!" + e1.Message);
            }
            finally
            {
                adp.Dispose();
            }
        }
        return code;
    }
    private static bool CheckNumberingRule(PubAdp adp, string ruleCode, string udName = "UD01",string formatCode= "N0000000", string resetType="NoReset")
    {
        
        if (string.IsNullOrEmpty(ruleCode)) throw new Exception("Rule code is null.");
        bool bl = adp.GetByID(new object[] { new object[] { ruleCode, "", "", "", "NumberingRule" } }, new Type[] { typeof(object[]) });
        if (!bl)
        {
            if ((Boolean)adp.InvokeFun("GetaNew" + udName))
            {
                DataSet  ds = adp.GetCurrentDataSet();
                DataRow editRow = ds.Tables[udName].Rows[(ds.Tables[udName].Rows.Count - 1)];
                editRow.BeginEdit();
                editRow["Key1"] = ruleCode;
                editRow["Key5"] = "NumberingRule";
                editRow["Character01"] = ruleCode;
                editRow["Character02"] = formatCode;
                editRow["ShortChar01"] = resetType;
                editRow["Number01"] = 1;
                editRow["Number02"] = DateTime.Now.Year;
                editRow["Number03"] = DateTime.Now.Month;
                editRow.EndEdit();
                adp.Update();
                bl = true;
            }
        }
        return bl;
    }
    public static DataTable GetUserCodes(EpiTransaction otrans, string codeTypeID,string filterStr= "IsActive='true'")
    {
        PubAdp adp = new PubAdp(otrans, "UserCodes");
        adp.BOConnect();
        try
        {
            DataTable udCodesDT = adp.GetCurrentDataSet().Tables["UDCodes"];
            DataTable dt = udCodesDT.Clone();
            if (adp.GetByID(new object[] { codeTypeID }, new Type[] { typeof(string) }))
            {
                PubFun.CopyTable(udCodesDT, dt, filterStr);
            }
            return dt;
        }
        catch (Exception e1)
        {
            throw new Exception("Get User Codes(" + codeTypeID + ") error!" + e1.Message);
        }
        finally
        {
            adp.Dispose();
        }
    }

    public EpiTransaction oTrans = null;
    private Type Type = null;
    private object Adp = null;
    public string AdpName = "";
    public PubAdp()
    {

    }

    public PubAdp(object otrans,string adpName, string className1 = null)
    {
        InitAdp(otrans, adpName, className1);
    }

    /// <summary>初始化Adapter</summary>
    public void InitAdp(object otrans, string adpName, string className1 = null)
    {
        oTrans = (EpiTransaction)otrans;
        AdpName = adpName;
        string dllfile = "Erp.Adapters." + AdpName + ".dll";
        string className = "Erp.Adapters." + AdpName + "Adapter";
        if (!System.IO.File.Exists(Application.StartupPath + "\\" + dllfile))
        {
            dllfile = "Ice.Adapters." + AdpName + ".dll";
            className = "Ice.Adapters." + AdpName + "Adapter";
        }
        if (!System.IO.File.Exists(Application.StartupPath + "\\" + dllfile))
        {
            dllfile = AdpName;
            className = className1;
        }
        System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(Application.StartupPath + "\\" + dllfile);
        Type = assembly.GetType(className);
        Adp = Activator.CreateInstance(Type, new object[] { otrans });
        BOConnect();
    }
    /// <summary>析构函数 </summary>
    ~PubAdp()
    {
        Dispose();
    }
    /// <summary>调用funName方法,obj数组为参数,types为参数类型</summary>
    public object InvokeFun(string funName, object[] obj = null, Type[] types = null)
    {
        System.Reflection.MethodInfo Fun;
        if (types == null) Fun = Type.GetMethod(funName);
        else Fun = Type.GetMethod(funName, types);
        return Fun.Invoke(Adp, obj);
    }
    /// <summary>反射获取属性值</summary>
    public object GetPropertyValue(string attrName)
    {
        Type type = Adp.GetType();
        System.Reflection.PropertyInfo propertyInfo = type.GetProperty(attrName);
        return propertyInfo.GetValue(Adp);
    }
    /// <summary>反射执行BOConnect方法</summary>
    public bool BOConnect()
    {
        return (bool)InvokeFun("BOConnect");
    }
    /// <summary>反射执行Update方法</summary>
    public bool Update()
    {
        return (bool)InvokeFun("Update");
    }
    /// <summary>反射执行Dispose方法</summary>
    public void Dispose()
    {
        InvokeFun("Dispose");
        // _PubAdapter.Hashtable.Remove(Name);
    }
    /// <summary>反射执行ClearData方法</summary>
    public void ClearData()
    {
        InvokeFun("ClearData");
    }
    /// <summary>反射执行GetByID方法</summary>
    public virtual bool GetByID(object[] obj, Type[] types)
    {
        try
        {
            return (bool)InvokeFun("GetByID", obj, types);
        }
        catch {
            return false;
        }
    }
    /// <summary>反射执行DeleteByID方法</summary>
    public virtual bool DeleteByID(object[] obj, Type[] types)
    {
        return (bool)InvokeFun("DeleteByID", obj, types);
    }
    /// <summary>反射执行Delete方法</summary>
    public bool Delete(DataRow dr)
    {
        return (bool)InvokeFun("Delete", new object[] { dr }, new Type[] { typeof(DataRow) });
    }
    /// <summary>反射执行GetCurrentDataSet方法</summary>
    public DataSet GetCurrentDataSet()
    {
        return (DataSet)InvokeFun("GetCurrentDataSet", new object[] { Ice.Lib.Searches.DataSetMode.RowsDataSet }, new Type[] { typeof(Ice.Lib.Searches.DataSetMode) });
    }
    /// <summary>反射执行InvokeSearch方法</summary>
    public DialogResult InvokeSearch(Ice.Lib.Searches.SearchOptions opts)
    {
        return (DialogResult)InvokeFun("InvokeSearch", new object[] { opts }, new Type[] { typeof(Ice.Lib.Searches.SearchOptions) });
    }
    /// <summary>获取表第一行数据中某列的值</summary>
    public object GetColumnValue(string tableName, string columnName)
    {
        if (Adp != null)
        {
            DataSet ds = GetCurrentDataSet();
            if (!ds.Tables.Contains(tableName))
            {
                throw new Exception("Not Found Table '" + tableName + "'!");
            }
            if (ds.Tables[tableName].Rows.Count > 0)
            {
                return ds.Tables[tableName].Rows[0][columnName];
            }
            throw new Exception("The number of rows is 0!");
        }
        else throw new Exception("Adapter is null!");
    }
    /// <summary>更新表第一行某列的值</summary>
    public bool UpdateColumnValue(string tableName, string columnName, object value)
    {
        try
        {
            if (Adp != null)
            {
                DataSet ds = GetCurrentDataSet();
                if (ds.Tables[tableName].Rows.Count > 0)
                {
                    ds.Tables[tableName].Rows[0].BeginEdit();
                    ds.Tables[tableName].Rows[0][columnName] = value;
                    ds.Tables[tableName].Rows[0].EndEdit();
                    Update();
                    return true;
                }
            }
            else throw new Exception("Adapter is null!");
        }
        catch (Exception e1)
        {
            throw e1;
        }
        return false;
    }
    public bool UpdateValues(string tableName, System.Collections.Generic.Dictionary<string, object> keyValues)
    {
        try
        {
            if (Adp != null)
            {
                DataSet ds = GetCurrentDataSet();
                if (ds.Tables[tableName]!=null && ds.Tables[tableName].Rows.Count > 0 && keyValues!=null && keyValues.Count>0)
                {
            
                    ds.Tables[tableName].Rows[0].BeginEdit();
                    foreach (var item in keyValues)
                    {
                        ds.Tables[tableName].Rows[0][item.Key] = item.Value;
                    }
                    ds.Tables[tableName].Rows[0].EndEdit();
                    Update();
                    return true;
                }
            }
            else throw new Exception("Adapter is null!");
        }
        catch (Exception e1)
        {
            throw e1;
        }
        return false;
    }

    public void AddUDTableRow(System.Collections.Generic.Dictionary<string, object> keyValues) {
        InvokeFun("GetaNew" + AdpName);
        DataSet ds = GetCurrentDataSet();
        DataRow newRow = ds.Tables[AdpName].Rows[0];
        newRow.BeginEdit();
        foreach (var item in keyValues)
        {
            newRow[item.Key] = item.Value;
        }
        newRow.EndEdit();
        Update();
    }
}
