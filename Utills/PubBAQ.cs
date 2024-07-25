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

