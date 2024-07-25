var context = (Erp.ErpContext)Ice.Services.ContextFactory.CreateContext();
var erpContext = new Erp.Internal.Lib.CCredChk(context);
var Db = erpContext.Db;
string company = Session.CompanyID;
string key4_RPTData = "RPTData";
string key5_RPTParameter = "RPTParameter";
string key5_RPT830CustSalesAmt = "RPT830CustSalesAmt";
//删除1天前历史数据
string _key1 = "K" + DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmmssfff");
var ud21Rows = (from row in Db.UD21
               where string.Compare(row.Company, company, true) == 0
               && string.Compare(row.Key1, _key1, true) < 0
               && string.Compare(row.Key4, key4_RPTData, true) == 0
               select row).ToList();
foreach (var ud21Row1 in ud21Rows)
{
    Db.UD21.Delete(ud21Row1);
}
//开始计算报表数据
var ud21Row = (from row in Db.UD21
               where string.Compare(row.Company, company, true) == 0
               && string.Compare(row.Key1, key1, true) == 0
               && string.Compare(row.Key5, key5_RPTParameter, true) == 0
               select row).FirstOrDefault();
if (ud21Row == null)
{
    //没有找到报表参数
    throw new Ice.BLException("Not found Report Parameter.");
}
DateTime? fromDate = ud21Row.Date01;
DateTime? toDate = ud21Row.Date02;
string custID1 = ud21Row.Character01;
bool isStartsWith = ud21Row.Character01.EndsWith("*");
if (isStartsWith) custID1 = custID1.Replace("*", "");
string salesPersonCode= ud21Row.ShortChar01;
bool showCurrentYearMonth = ud21Row.CheckBox01;
bool includeFreeText = ud21Row.CheckBox02;
string msg = "";
try
{
    var customerRows = (from row in Db.Customer
                    where string.Compare(row.Company, company, true) == 0
                    && (custID1 == "" || string.Compare(row.CustID, custID1, true) == 0 
                        || (isStartsWith && row.CustID.StartsWith(custID1)))
                     && (salesPersonCode == "" || string.Compare(row.TerritoryID, salesPersonCode, true) == 0)
                    select row).OrderBy(r => r.CustID).ToList();

    int totalNum = customerRows.Count();
    if (totalNum == 0)
    {
        //没有找到物料
        throw new Ice.BLException("Not found Customer.");
    }
    decimal num = 0m; 
    decimal line1 = 0;
    for (int i = 0; i < totalNum; i++)
    {
        string custID = customerRows[i].CustID;
        int custNum= customerRows[i].CustNum;
        msg = custID + System.Environment.NewLine;
        num++;
        decimal rr = Math.Round(num / totalNum*100, 2);
        if (rr < 100)
        {
            ud21Row.Number20 = rr;
            Db.Validate(ud21Row);
        }
        var invcHeadQuery = (from row in Db.InvcHead
                     where string.Compare(row.Company, company, true) == 0
                     && row.CustNum == custNum
                     && (fromDate == null || row.InvoiceDate >= fromDate)
                     && (toDate == null || row.InvoiceDate <= toDate)
                     && row.RefCancelled == 0
                     && row.RefCancelledBy == 0
                     && row.Posted == true
                     && (string.Compare(row.GroupID, "StartUp", true) != 0 || string.Compare(row.InvoiceSuffix, "CM", true) != 0)
                     && string.Compare(row.InvoiceSuffix, "UR", true) != 0
                     select row);

        if (!includeFreeText)
        {
            //invcHeadQuery = invcHeadQuery.Where(row => string.Compare(row.InvoiceType, "MIS", true) != 0);
            invcHeadQuery = invcHeadQuery.Where(row => string.Compare(row.TranDocTypeID, "BCM", true) != 0 && string.Compare(row.TranDocTypeID, "BSX", true) != 0);
        }

        var invcHeadRows = invcHeadQuery.ToList();
        
        foreach (var invcHeadRow in invcHeadRows) {
            string year = Convert.ToDateTime(invcHeadRow.InvoiceDate).Year.ToString();
            string yearMonth = Convert.ToDateTime(invcHeadRow.InvoiceDate).ToString("yyyy/MM");
            bool currentYear = Convert.ToDateTime(invcHeadRow.InvoiceDate).Year == DateTime.Now.Year;
            decimal invcAmt = invcHeadRow.InvoiceAmt;

            var invcDtlRows = (from row in Db.InvcDtl
                                where string.Compare(row.Company, company, true) == 0
                                && row.InvoiceNum== invcHeadRow.InvoiceNum
                               select row).ToList();
            foreach (var invcDtlRow in invcDtlRows) {
                UD21 ud21CustSalesAmtRow = (from row in Db.UD21
                                where string.Compare(row.Company, company, true) == 0
                                && string.Compare(row.Key1, key1, true) == 0
                                && string.Compare(row.Key5, key5_RPT830CustSalesAmt, true) == 0
                                && string.Compare(row.ShortChar01, custID, true)==0
                                && string.Compare(row.ShortChar03, yearMonth, true) == 0
                                select row).FirstOrDefault(); 
                decimal avCost= invcDtlRow.SellingShipQty * (invcDtlRow.MtlUnitCost + invcDtlRow.LbrUnitCost + invcDtlRow.BurUnitCost + invcDtlRow.SubUnitCost + invcDtlRow.MtlBurUnitCost);
                if (ud21CustSalesAmtRow == null)
                {
                    line1++;
                    ud21CustSalesAmtRow = new UD21();
                    ud21CustSalesAmtRow.Company = company;
                    ud21CustSalesAmtRow.Key1 = key1;
                    ud21CustSalesAmtRow.Key2 = "L" + line1.ToString("000000");
                    ud21CustSalesAmtRow.Key4 = key4_RPTData;
                    ud21CustSalesAmtRow.Key5 = key5_RPT830CustSalesAmt;
                    ud21CustSalesAmtRow.ShortChar01 = custID;
                    ud21CustSalesAmtRow.ShortChar02 = year;
                    ud21CustSalesAmtRow.ShortChar03 = yearMonth;
                    ud21CustSalesAmtRow.ShortChar04 = customerRows[i].TerritoryID;
                    ud21CustSalesAmtRow.Character01 = customerRows[i].Name;
                    ud21CustSalesAmtRow.Number01 = invcAmt;
                    ud21CustSalesAmtRow.Number02 = avCost;
                    ud21CustSalesAmtRow.CheckBox01 = currentYear;
                    Db.UD21.Insert(ud21CustSalesAmtRow);
                }
                else {
                    ud21CustSalesAmtRow.Number01 = ud21CustSalesAmtRow.Number01 + invcAmt;
                    ud21CustSalesAmtRow.Number02 = ud21CustSalesAmtRow.Number02 + avCost;
                }
                Db.Validate(ud21CustSalesAmtRow);
                invcAmt = 0;
            }
        }
    }
    ud21Row.Number20 = 100m;
    ud21Row.Character10 = msg;
}
catch (Exception e1)
{
    throw new Exception(msg+ e1.Message);
}
finally {
    ud21Row.CheckBox20 = true;
    Db.Validate(ud21Row);
}