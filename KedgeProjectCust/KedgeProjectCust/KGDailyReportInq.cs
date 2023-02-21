using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Objects.PM;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using Kedge.DAC;


namespace Kedge
{
    public class KGDailyReportInq : PXGraph<KGDailyReportInq>
    {

        public PXCancel<KGDailyReportFilter> Cancel;
        public PXFilter<KGDailyReportFilter> addItemFilter;

        [PXFilterable]
        public PXSelect<KGDailyReport,
                    Where<KGDailyReport.contractID, Equal<Current<KGDailyReportFilter.contractID>>,
                        And<KGDailyReport.workDate, LessEqual<Current<KGDailyReportFilter.dateTo>>,
                        And<KGDailyReport.workDate, GreaterEqual<Current<KGDailyReportFilter.dateFrom>>>>>
                        > KGDailyReports;
        /*
        public PXSelect<PMTask,
                    Where<PMTask.projectID, Equal<Current<KGDailyReport.contractID>>,
                        And<PMTask.status, Equal<ProjectTaskStatus.active>>>>
                         pmTasks;*/
        protected virtual IEnumerable kGDailyReports()
        {
            // Defining a dynamic data view
            PXSelectBase<KGDailyReport> query = new PXSelect<KGDailyReport>(this);
            // The current filtering parameters
            KGDailyReportFilter filter = addItemFilter.Current;
            if (filter.ContractID != null)
            {
                query.WhereAnd<Where<KGDailyReport.contractID, Equal<Current<KGDailyReportFilter.contractID>>>>();

            }
            if (filter.DateFrom != null)
            {
                
                query.WhereAnd<Where<KGDailyReport.workDate, GreaterEqual<Current<KGDailyReportFilter.dateFrom>>>>();
            }
            if (filter.DateTo != null)
            {
                query.WhereAnd<Where<KGDailyReport.workDate, LessEqual<Current<KGDailyReportFilter.dateTo>>>>();
            }
            return query.Select();
        }

        public PXAction<KGDailyReportFilter> ViewKGReport;
        [PXButton]
        protected virtual void viewKGReport()
        {
            KGDailyReport row = KGDailyReports.Current;
            // Creating the instance of the graph
            KGDailyReportEntry graph = PXGraph.CreateInstance<KGDailyReportEntry>();
            // Setting the current product for the graph
            graph.Dailys.Current = graph.Dailys.Search<KGDailyReport.dailyReportCD>(row.DailyReportCD);
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.Dailys.Current != null)
            {
                throw new PXRedirectRequiredException(graph, true, "Daily Report");
            }
        }



        [Serializable]
        public class KGDailyReportFilter : IBqlTable
        {


            #region DateFrom
            public abstract class dateFrom : PX.Data.IBqlField
            {
            }
            protected DateTime? _DateFrom;
            [PXDate()]
            [PXDefault()]
            [PXUIField(DisplayName = "Task Date From")]

            public virtual DateTime? DateFrom
            {
                get
                {
                    return this._DateFrom;
                }
                set
                {
                    this._DateFrom = value;
                }
            }

            #endregion

            #region DateTo
            public abstract class dateTo : PX.Data.IBqlField
            {
            }
            protected DateTime? _DateTo;
            [PXDate()]
            [PXDefault()]
            [PXUIField(DisplayName = "Task Date To")]

            public virtual DateTime? DateTo
            {
                get
                {
                    return this._DateTo;
                }
                set
                {
                    this._DateTo = value;
                }
            }

            #endregion

            #region ContractID
            [PXInt()]
            [PXUIField(DisplayName = "Project")]
            [PXSelector(typeof(Search2<PMProject.contractID,
                LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                Equal<PMProject.contractID>>>>,
                Where<PMProject.baseType, Equal<CTPRType.project>,
                 And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>)
                , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                typeof(Customer.acctName), typeof(PMProject.status),
                typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false, DescriptionField = typeof(PMProject.description))]
            public virtual int? ContractID { get; set; }
            public abstract class contractID : IBqlField { }
            #endregion

        }
    }
}