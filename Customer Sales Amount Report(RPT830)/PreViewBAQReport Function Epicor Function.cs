var context = (Erp.ErpContext)Ice.Services.ContextFactory.CreateContext();
            var erpContext = new Erp.Internal.Lib.CCredChk(context);
            var Db = erpContext.Db;
            string company = Session.CompanyID;
            string key5_RPTParameter = "RPTParameter";

            var rptParameterRow = (from row in Db.UD21
                           where string.Compare(row.Company, company, true) == 0
                           && string.Compare(row.Key1, key1, true) == 0
                           && string.Compare(row.Key5, key5_RPTParameter, true) == 0
                           select row).FirstOrDefault();
            if (rptParameterRow == null) {
                throw new Exception("Not found report parameter date.");
            } 
            string baqReportID = rptParameterRow.ShortChar16;
            string baqID = rptParameterRow.ShortChar17;
            string agentID = rptParameterRow.ShortChar18; 

            var reportParamTS = new Ice.Tablesets.BAQReportTableset();
            var reportParamTable = reportParamTS.BAQReportParam;
            var reportParamRow = new Ice.Tablesets.BAQReportParamRow();
            reportParamTable.Add(reportParamRow);
            var prompts = new Dictionary<string, object>();
            var filters = new Dictionary<string, IEnumerable<object>>();
            prompts.Add("Key1", key1);
            reportParamRow.BAQID = baqID;
            reportParamRow.ReportTitle = "";
            reportParamRow.Summary = false;
            reportParamRow.Check01 = false;
            reportParamRow.Check02 = false;
            reportParamRow.Check03 = false;
            reportParamRow.Check04 = false;
            reportParamRow.Check05 = false;
            reportParamRow.Number01 = 0;
            reportParamRow.Number02 = 0;
            reportParamRow.Number03 = 0;
            reportParamRow.Number04 = 0;
            reportParamRow.Number05 = 0;
            reportParamRow.ReportID = baqReportID;
            reportParamRow.Check06 = false;
            reportParamRow.Check07 = false;
            reportParamRow.Check08 = false;
            reportParamRow.Check09 = false;
            reportParamRow.Check10 = false;
            reportParamRow.ArchiveCode = 0;
            reportParamRow.DateFormat = "m/d/yyyy";
            reportParamRow.NumericFormat = ",.";
            reportParamRow.PrintReportParameters = false;
            reportParamRow.SSRSEnableRouting = false;
            reportParamRow.DesignMode = false;
            var autoPrintHandler = new Ice.Lib.AutoPrintHandler(Db);
            reportParamRow.Filter1 = autoPrintHandler.BuildBAQReportCriteriaDocumentAndUpdateReportParameters(baqReportID, reportParamRow, prompts, filters);

            reportParamRow.AutoAction = "AUTOPRV";
            reportParamRow.ReportStyleNum = 1;
            reportParamRow.WorkstationID = Session.TaskClientID ?? string.Empty;
            if (reportParamTable.Columns.Contains("ReportID"))
            {
                reportParamRow["ReportID"] = baqReportID;
            }
            reportParamRow.TaskNote = "";
            reportParamRow.SSRSRenderFormat = "PDF";
            var reportService = Ice.Assemblies.ServiceRenderer.GetService<Ice.Contracts.BAQReportSvcContract>(Db, true);
            if (reportService == null)
            {
                throw new Ice.Common.BusinessObjectException("Report service 'Ice.Contracts.BAQReportSvcContract' not available");
            }
            try
            {
                reportService.SubmitToAgent(reportParamTS, agentID, 0, 0, "Ice.Services.Rpt.BAQReport");
            }
            catch (Exception ex)
            {
                throw new Ice.Common.BusinessObjectException("Failed to submit autoprint report. Error: " + ex.ToString());
            }