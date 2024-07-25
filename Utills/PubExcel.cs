public class PubExcel
{
    public string defName = "excel";
    public string SaveFileName = "";
    private Workbook workbook;
    public string FontName { get; set; }
    public int FontSize { get; set; }
    public Color FontColor { get; set; }
    public Color FillColor { get; set; }
    public Color CellBorderLineColor { get; set; }
    public CellBorderLineStyle CellBorderLineStyle { get; set; }
    public ExcelDefaultableBoolean Bold { get; set; }
    public ExcelDefaultableBoolean Italic { get; set; }
    public FontUnderlineStyle FontUnderlineStyle { get; set; }
    private Worksheet _CurrentSheet = null;
    public Worksheet CurrentSheet {
        get {
            if (_CurrentSheet == null) {
                if (workbook.Worksheets.Count > 0) {
                    _CurrentSheet = workbook.Worksheets[0];
                }
            }
            return _CurrentSheet;
        }
        set {
            _CurrentSheet = value;
        }
    }
    /// <summary>实例化</summary>
    public PubExcel()
    {
        workbook = new Workbook();
        workbook.SetCurrentFormat(WorkbookFormat.Excel2007);
        FontName = "Times New Roman";
        FontSize = 9;
        FontColor = Color.Black;
        FillColor = Color.White;
        CellBorderLineColor= Color.Black;
        CellBorderLineStyle =CellBorderLineStyle.Thin;
        Bold = ExcelDefaultableBoolean.False;
        Italic=ExcelDefaultableBoolean.False;
        FontUnderlineStyle = FontUnderlineStyle.None;
    }
    /// <summary>加载Excel文件</summary>
    public bool Load(string sFileName)
    {
        try
        {
            workbook = Workbook.Load(sFileName);
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>Excel转DataTable</summary>
    public void ToDataTable(ref DataTable dt, string sSheetName = "Sheet1", int headRow = 1)
    {
        string colName = "";
        try
        {
            Worksheet aWorksheet = workbook.Worksheets[sSheetName];
            foreach (var row in aWorksheet.Rows)
            {
                if (row.Index >= headRow)
                {
                    DataRow dr = dt.NewRow();
                    foreach (var cell in row.Cells)
                    {
                        colName = Convert26(cell.ColumnIndex + 1);
                        if (headRow > 0)
                        { 
                            var colNameRow = aWorksheet.Rows[headRow-1];
                            colName = colNameRow.Cells[cell.ColumnIndex].Value.ToString();
                        }
                        if (dt.Columns.Contains(colName))
                        {
                            if (dt.Columns[colName].DataType == typeof(DateTime))
                            {
                                dr[colName] = DateTime.FromOADate(Convert.ToInt32(cell.Value));
                            }
                            else dr[colName] = cell.Value;
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
        }
        catch(Exception e1)
        {
            throw new Exception("Column("+ colName + ") Load Error."+e1.Message);
        }
    }

    /// <summary>新建Excel表格</summary>
    public Worksheet NewSheet(string sSheetName)
    {
        CurrentSheet = workbook.Worksheets.Add(sSheetName);
        return CurrentSheet;
    }
    /// <summary>获取Excel表格</summary>
    public Worksheet GetSheet(string sSheetName)
    {
        CurrentSheet = workbook.Worksheets[sSheetName];
        return CurrentSheet;
    }

    public int SetSheetCaption(int startRow, int startCol, DataTable dt, string[] columns, int lrtb = 0,int height=40, ExcelDefaultableBoolean wrapText= ExcelDefaultableBoolean.True)
    {
        int col = startCol;
        int colNum = 0;
        CurrentSheet.Rows[startRow].Height = height*15;
        CurrentSheet.Rows[startRow].CellFormat.WrapText = wrapText;//自动换行
        for (int c = 0; c < columns.Length; c++)
        {
            if (dt.Columns.Contains(columns[c]))
            {
                DataColumn column = dt.Columns[columns[c]];
                WorksheetCell cell1 = GetCell(startRow, col);
                SetCellValue(cell1, column.Caption);
                //SetCellFill(cell1, System.Drawing.Color.FromArgb(199, 199, 199));
                SetCellBorder(cell1, lrtb);
                if (column.DataType == typeof(decimal) || column.DataType == typeof(int)) {
                    SetCellAlignment(cell1, HorizontalCellAlignment.Center);
                }
                if (column.ExtendedProperties["Width"] != null)
                {
                    CurrentSheet.Columns[col].Width = Convert.ToInt32(column.ExtendedProperties["Width"]) * 51;
                }
            }
            col++;
            colNum++;
        }
        return colNum;
    }
    public int SetSheetCaptionAndName(int startRow, int startCol, DataTable dt, string[] columns, int lrtb = 0,int height=40, ExcelDefaultableBoolean wrapText= ExcelDefaultableBoolean.True)
    {
        int col = startCol;
        int colNum = 0;
        CurrentSheet.Rows[startRow].Height = height*15;
        CurrentSheet.Rows[startRow].CellFormat.WrapText = wrapText;//自动换行
        for (int c = 0; c < columns.Length; c++)
        {
            if (dt.Columns.Contains(columns[c]))
            {
                DataColumn column = dt.Columns[columns[c]];
                WorksheetCell cell1 = GetCell(startRow, col);
                SetCellValue(cell1, column.Caption);
                SetCellBorder(cell1, lrtb);
                cell1.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                if (column.DataType == typeof(decimal) || column.DataType == typeof(int)) {
                    SetCellAlignment(cell1, HorizontalCellAlignment.Right);
                }
                WorksheetCell cell2 = GetCell(startRow+1, col);
                SetCellValue(cell2, column.ColumnName);
                SetCellBorder(cell2, lrtb);
                cell2.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                if (column.DataType == typeof(decimal) || column.DataType == typeof(int))
                {
                    SetCellAlignment(cell2, HorizontalCellAlignment.Right);
                }
                if (column.ExtendedProperties["Width"] != null)
                {
                    CurrentSheet.Columns[col].Width = Convert.ToInt32(column.ExtendedProperties["Width"]) * 51;
                }

            }  
            col++;
            colNum++;
        }
        return colNum;
    }
    public void                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        SetSheetValue(int startRow, int startCol, DataRow[] drs,string[] columns, int lrtb = 0)
    {
        SetSheetValue(startRow, startCol, drs, columns, new List<string>(columns), lrtb);
    }

    public void SetSheetValue(int startRow, int startCol, DataRow dr, string[] columns, int lrtb = 0)
    {
        SetSheetValue(startRow, startCol, dr, columns, new List<string>(columns), lrtb);
    }
    /// <summary>将DataRow[]数据放入Excel表格中</summary>
    /// <param name="startRow">开始放入的行索引,第1行索引为0</param>
    /// <param name="startCol">开始放入的列索引,第1列索引为0</param>
    /// <param name="drs">DataRow[]数据</param>
    /// <param name="allColumns">excel中所有需要显示的列</param>
    /// <param name="showColumns">当前需要显示的列</param>
    /// <param name="lrtb">边框</param>
    public void SetSheetValue(int startRow, int startCol, DataRow[] drs, string[] allColumns, List<string> showColumns, int lrtb = 0)
    {
        for (int r = 0; r < drs.Length; r++)
        {
            SetSheetValue(startRow + r, startCol, drs[r], allColumns, showColumns, lrtb);
        }
    }
    public void SetSheetValueH(int startRow, int startCol, DataRow[] drs, string[] allColumns, List<string> hiddenColumns, int lrtb = 0)
    {
        List<string> showColumns = new List<string>();
        for (int i = 0; i < allColumns.Length; i++)
        {
            if (hiddenColumns == null || !hiddenColumns.Contains(allColumns[i])) showColumns.Add(allColumns[i]);
        }
        SetSheetValue(startRow, startCol, drs, allColumns, showColumns, lrtb);
    }
    public void SetSheetValue(int startRow, int startCol, DataRow dr, string[] allColumns, List<string> showColumns, int lrtb = 0) {
        
        if (dr != null)
        {
            int row = startRow;
            int col = startCol;
            for (int c = 0; c < allColumns.Length; c++)
            {
                if (dr.Table.Columns.Contains(allColumns[c]))
                {
                    DataColumn column = dr.Table.Columns[allColumns[c]];
                    if (showColumns.Contains(allColumns[c]))
                    {
                        
                        WorksheetCell cell = GetCell(row, col);
                        SetCellValue(cell, dr[allColumns[c]]);
                        SetCellBorder(cell, lrtb);
                        string formatString = Convert.ToString(column.ExtendedProperties["FormatString"]);
                        if (string.IsNullOrEmpty(formatString))
                        {
                            int columnDigits = 0;
                            if (column.DataType == typeof(decimal))
                            {
                                if (column.ExtendedProperties["ColumnDigits"] != null)
                                {
                                    columnDigits = Convert.ToInt32(column.ExtendedProperties["ColumnDigits"]);
                                }
                                else if (column.ExtendedProperties["Format"] != null)
                                {
                                    string format = Convert.ToString(column.ExtendedProperties["Format"]);
                                    int idx = format.IndexOf('.') + 1;
                                    columnDigits = format.Substring(idx, format.Length - idx).Length;
                                }
                            }
                            SetCellFormatString(cell, column.DataType, columnDigits);
                        }
                        else
                        {
                            SetCellFormatString(cell, formatString);
                            if (column.DataType == typeof(decimal) || column.DataType == typeof(int))
                            {
                                SetCellAlignment(cell, HorizontalCellAlignment.Right);

                            }
                        }
                    }
                } 
                col++;
            }
        }
    }
    public void SetCellFormatString(WorksheetCell cell,string formatString="") {
        if(string.IsNullOrEmpty(formatString)) cell.CellFormat.FormatString = "General";
        else cell.CellFormat.FormatString = formatString;
    }
    public void SetCellFormatString(WorksheetCell cell, Type dataType, int columnDigits = 2)
    {
        if (dataType == typeof(int))
        {
            cell.CellFormat.FormatString = "#,##0";
        }
        else if (dataType == typeof(decimal))
        {
            if (columnDigits == 0) cell.CellFormat.FormatString = "#,##0";
            else cell.CellFormat.FormatString = "_(* #,##0.00_);_(* (#,##0.00);_(* \"-\"??_);_(@_)";
            
            
        }
        else if (dataType == typeof(DateTime))
        {
            cell.CellFormat.FormatString = "yyyy/m/d";
        }
        else cell.CellFormat.FormatString = "General";
        if (dataType == typeof(decimal) || dataType == typeof(int) || dataType == typeof(DateTime))
        {
            SetCellAlignment(cell, HorizontalCellAlignment.Right);
        }
    }

    public void CreateTotal(int startRow, int startCol, DataRow[] drs, string[] allColumns, string[] totalColumns, int lrtb = 0)
    {
        if (drs.Length > 0)
        {
            DataRow dr = drs[0].Table.NewRow();
            for (int c = 0; c < allColumns.Length; c++)
            {
                if (drs[0].Table.Columns.Contains(allColumns[c]))
                {
                    DataColumn column = drs[0].Table.Columns[allColumns[c]];
                    List<string> columns2 = new List<string>(totalColumns);
                    if (columns2.Contains(allColumns[c]))
                    {
                        decimal value = 0m;
                        for (int i = 0; i < drs.Length; i++) value += Convert.ToDecimal(drs[i][allColumns[c]]);
                        dr[allColumns[c]] = value;
                    }
                    
                }
            }
            CreateTotal(startRow, startCol, dr, allColumns, totalColumns, lrtb);
        }
    }
    public void CreateTotal(int startRow, int startCol, DataRow dr, string[] allColumns, string[] totalColumns, int lrtb = 0)
    {
        SetSheetValue(startRow, startCol, dr, allColumns, new List<string>(totalColumns), lrtb);
    }
    public int GetColumnIndex(int startCol, DataTable dt, string[] columns, string colName)
    {
        int col = -1;
        bool r = false;
        for (int c = 0; c < columns.Length; c++)
        {
            if (dt.Columns.Contains(columns[c]))
            {
                col++;
                if (string.Compare(colName, columns[c], true) == 0) {
                    r = true;
                    break;
                } 
            }
        }
        if (r) return (col + startCol);
        else return -1;
    }

    public WorksheetCell GetCell(int row, int col)
    {
        return CurrentSheet.Rows[row].Cells[col];
    }
    public void SetCellBorder(WorksheetCell cell1, int lrtb = 15) {
        SetCellBorder(cell1, CellBorderLineColor, lrtb, CellBorderLineStyle);
    }
    public void SetCellBorder(WorksheetCell cell1,Color color, int lrtb=15,CellBorderLineStyle cellBorderLineStyle = CellBorderLineStyle.Thin)
    {
            WorkbookColorInfo workbookColorInfo = new WorkbookColorInfo(color);
        if (lrtb > 15) lrtb = 15;
        if (lrtb >= LRTB.B)
        {
            cell1.CellFormat.BottomBorderColorInfo = workbookColorInfo;
            cell1.CellFormat.BottomBorderStyle= cellBorderLineStyle;
            lrtb -= LRTB.B;
        }
        if (lrtb >= LRTB.T)
        {
            cell1.CellFormat.TopBorderColorInfo = workbookColorInfo;
            cell1.CellFormat.TopBorderStyle = cellBorderLineStyle;
            lrtb -= LRTB.T;
        }
        if (lrtb >= LRTB.R)
        {
            cell1.CellFormat.RightBorderColorInfo = workbookColorInfo;
            cell1.CellFormat.RightBorderStyle = cellBorderLineStyle;
            lrtb -= LRTB.R;
        }
        if (lrtb == LRTB.L)
        {
            cell1.CellFormat.LeftBorderColorInfo = workbookColorInfo;
            cell1.CellFormat.LeftBorderStyle = cellBorderLineStyle;
        }
    }
    public void SetOuterBorders(int startRow, int startCol, int endRow, int endCol)
    {
        WorksheetCell cell;
        for (int r = startRow; r <= endRow; r++)
        {
            cell = GetCell(r, startCol);
            SetCellBorder(cell, PubExcel.LRTB.L);
            cell = GetCell(r, endCol);
            SetCellBorder(cell, PubExcel.LRTB.R);
        }
        for (int c = startCol; c <= endCol; c++)
        {
            cell = GetCell(startRow, c);
            SetCellBorder(cell, PubExcel.LRTB.T);
            cell = GetCell(endRow, c);
            SetCellBorder(cell, PubExcel.LRTB.B);
        }
    }
    public void SetAllBorders(int startRow, int startCol, int endRow, int endCol)
    {
        WorksheetCell cell;
        for (int r = startRow; r <= endRow; r++)
        {
            for (int c = startCol; c <= endCol; c++)
            {
                cell = GetCell(r, c);
                SetCellBorder(cell, PubExcel.LRTB.ALL);
            }
        }
    }
    public void SetCellFill(WorksheetCell cell1, System.Drawing.Color color)
    {
        //System.Drawing.Color.FromArgb(199, 199, 199)
        cell1.CellFormat.Fill = CellFill.CreateSolidFill(color);
    }
    public void SetCellsFill(int firstRow, int firstColumn, int lastRow, int lastColumn, System.Drawing.Color color)
    {
        for (int r = firstRow; r <= lastRow; r++) {
            for (int c = firstColumn; c <= lastColumn; c++)
            {
                SetCellFill(CurrentSheet.Rows[r].Cells[c], color);
            }
        }
    }
    public void SetCellAlignment(WorksheetCell cell1, HorizontalCellAlignment horizontalCellAlignment= HorizontalCellAlignment.Left,VerticalCellAlignment verticalCellAlignment=VerticalCellAlignment.Center)
    {
        cell1.CellFormat.Alignment = horizontalCellAlignment;
        cell1.CellFormat.VerticalAlignment = verticalCellAlignment;
    }

    public void SetRegionsAlignment(WorksheetRegion region, HorizontalCellAlignment horizontalCellAlignment = HorizontalCellAlignment.Left, VerticalCellAlignment verticalCellAlignment = VerticalCellAlignment.Center)
    {
        for (int r = region.FirstRow; r <= region.LastRow; r++) {
            for (int c = region.FirstColumn; c <= region.LastColumn; c++)
            {
                WorksheetCell cell1 = GetCell(r, c);
                cell1.CellFormat.Alignment = horizontalCellAlignment;
                cell1.CellFormat.VerticalAlignment = verticalCellAlignment;
            }
        }
    }

    public void SetCellFont(WorksheetCell cell1, string fontName = "Times New Roman", int fontSize = 10, ExcelDefaultableBoolean bold = ExcelDefaultableBoolean.False, FontUnderlineStyle fontUnderlineStyle = FontUnderlineStyle.None, ExcelDefaultableBoolean italic = ExcelDefaultableBoolean.False)
    {
        if(!string.IsNullOrEmpty(fontName))cell1.CellFormat.Font.Name = fontName;
        if(fontSize!=0) cell1.CellFormat.Font.Height = fontSize*20;
        cell1.CellFormat.Font.ColorInfo = new WorkbookColorInfo(FontColor);
        cell1.CellFormat.Font.Italic= italic;
        cell1.CellFormat.Font.Bold = bold;
        cell1.CellFormat.Font.UnderlineStyle = fontUnderlineStyle;
    }

    public void SetCellValue(WorksheetCell cell1,object value, Type dataType=null, int columnDigits = 2)
    {
        cell1.Value = value;
        SetCellFont(cell1, FontName, FontSize, Bold, FontUnderlineStyle,Italic);
        SetCellAlignment(cell1);
        if (dataType != null) SetCellFormatString(cell1, dataType, columnDigits);
    }
    public void SetCellValue(WorksheetCell cell1, object value, string formatString)
    {
        SetCellValue(cell1, value);
        SetCellFormatString(cell1, formatString);
    }
    /// <summary>合并单元格,索引从0开始</summary>
    public void MergedCells(int firstRow, int firstColumn, int lastRow, int lastColumn)
    {
        CurrentSheet.MergedCellsRegions.Add(firstRow, firstColumn, lastRow, lastColumn);
    }

    public void SetCellStyle(WorksheetCell cell, string cellStyleJson)
    {
        if (!string.IsNullOrEmpty(cellStyleJson))
        {
            CellStyle cellStyle = new CellStyle(cellStyleJson); //将json数据转化为对象类型 
            string fontName = FontName;
            int fontSize = FontSize;
            FontUnderlineStyle fontUnderlineStyle = FontUnderlineStyle;
            ExcelDefaultableBoolean bold = Bold;
            ExcelDefaultableBoolean italic = Italic;
            CellBorderLineStyle cellBorderLineStyle = CellBorderLineStyle;
            if (cellStyle != null)
            {
                if (!string.IsNullOrEmpty(cellStyle.FontName)) fontName = cellStyle.FontName;
                if (!string.IsNullOrEmpty(cellStyle.FontSize)) fontSize = Convert.ToInt32(cellStyle.FontSize);
                if (!string.IsNullOrEmpty(cellStyle.FontUnderlineStyle)) fontUnderlineStyle = GetStyleByString(cellStyle.FontUnderlineStyle, FontUnderlineStyle);
                if (!string.IsNullOrEmpty(cellStyle.Bold)) bold = GetStyleByString(cellStyle.Bold, Bold);
                if (!string.IsNullOrEmpty(cellStyle.Italic)) italic = GetStyleByString(cellStyle.Italic, Italic);
                if (!string.IsNullOrEmpty(cellStyle.CellBorderLineStyle)) cellBorderLineStyle = GetStyleByString(cellStyle.CellBorderLineStyle, CellBorderLineStyle);
                if (!string.IsNullOrEmpty(cellStyle.BorderStyle))SetCellBorder(cell, CellBorderLineColor, Convert.ToInt32(cellStyle.BorderStyle), cellBorderLineStyle);
            }
            SetCellFont(cell, fontName, fontSize, bold, fontUnderlineStyle, italic);
        }
    }

    private T GetStyleByString<T>(string styleStr, T defaultValue) {

        foreach (T v in Enum.GetValues(typeof(T)))
        {
            string strName = Enum.GetName(typeof(T), v);
            if (strName.Equals(styleStr)) return v;
        }
        return defaultValue;
    }   

    /// <summary>数字转字母</summary>
    private string Convert26(int num)
    {
        var str = "";
        while (num > 0)
        {
            var m = num % 26;
            if (m == 0) m = 26;
            str = ((char)(m + 64)).ToString() + str;
            num = (num - m) / 26;
        }
        return str;
    }

    /// <summary>保存Excel文件</summary>
    public bool Save(string sFileName)
    {
        try
        {
            if (CheckFile(sFileName))
            {
                SaveFileName = sFileName;
                WorkbookFormat? format = Workbook.GetWorkbookFormat(sFileName);
                if (!format.HasValue) return false;
                workbook.SetCurrentFormat(format.Value);
                if (workbook.Worksheets.Count <= 0) workbook.Worksheets.Add("Sheet1");
                workbook.Save(sFileName);
                return true;
            }
        }
        catch (Exception e1)
        {
            MessageBox.Show(e1.Message);
        }
        return false;
    }
    /// <summary>保存Excel文件</summary>
    public bool Save()
    {

        SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "Excel Workbook(*.xlsx)|*.xlsx|Excel 97-2003 Workbook(*.xls)|*.xls";//设置文件类型
        sfd.FileName = defName;//设置默认文件名
        sfd.DefaultExt = "xlsx";//设置默认格式（可以不设）
        sfd.AddExtension = true;//设置自动在文件名中添加扩展名
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            return Save(sfd.FileName);
        }
        return false;
        
    }
    public bool Open() {
        bool bl = false;
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "Files|*.xls;*.xlsx";
        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            string file = dialog.FileName;
            bl= Load(file);
        }
        return bl;
    }

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    public static extern IntPtr _lopen(string lpPathName, int iReadWrite);
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    public static extern bool CloseHandle(IntPtr hObject);
    public const int OF_READWRITE = 2;
    public const int OF_SHARE_DENY_NONE = 0x40;
    public readonly IntPtr HFILE_ERROR = new IntPtr(-1);
    private bool CheckFile(string vFileName)

    {
        try
        {
            if (System.IO.File.Exists(vFileName))
            {
                IntPtr vHandle = _lopen(vFileName, OF_READWRITE | OF_SHARE_DENY_NONE);
                if (vHandle == HFILE_ERROR)
                {
                    MessageBox.Show("Excel file has been occupied, please close the open file first");
                    return false;
                }
                CloseHandle(vHandle);
            }
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public class CellStyle
    {
        public string FontName { get; set; }
        public string FontSize { get; set; }
        /// <summary>Default,None,Single,Double,SingleAccounting,DoubleAccounting</summary>
        public string FontUnderlineStyle { get; set; }
        /// <summary> Default,None,Thin,Medium,Dashed,Dotted ,Thick,Double,Hair,MediumDashed ,DashDot,MediumDashDot,DashDotDot,MediumDashDotDot,SlantedDashDot</summary>
        public string CellBorderLineStyle { get; set; }
        public string BorderStyle { get; set; }
        public string Bold { get; set; }//True,False
        public string Italic { get; set; }//True,False

        public CellStyle(string jsonStr="") {
            if (jsonStr != "")
            {
                string[] strArray = jsonStr.Trim().TrimStart('{').TrimEnd('}').Split(',');
                for (int i = 0; i < strArray.Length; i++)
                {
                    string[] strArray1 = strArray[i].Split(':');
                    if (strArray1.Length == 2)
                    {
                        string key = strArray1[0].Trim().Trim('"');
                        string value = strArray1[1].Trim().Trim('"');
                        this.GetType().GetProperty(key).SetValue(this, value);
                    }
                }
            }
        }
    }

    public class LRTB {
        public static int None = 0;
        public static int L = 1;
        public static int R = 2;
        public static int T = 4;
        public static int B = 8;
        public static int ALL = 15;
    }
    
}