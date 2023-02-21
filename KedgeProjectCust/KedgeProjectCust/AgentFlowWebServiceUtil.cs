using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Data.DependencyInjection;
using PX.LicensePolicy;
using PX.Objects.Common;
using PX.Objects.Common.Extensions;
using PX.Objects.Common.Bql;
using PX.Objects.Common.Discount;
using PX.Objects.BQLConstants;
using Kedge.DAC;
using PX.Objects.AP;
using PX.Objects.EP;
using PX.Objects.CS;

namespace Kedge.DAC
{
    public  class AgentFlowWebServiceUtil
    {
        public static String getName(String name)
        {
            name = getAgentID(name) ?? name;
            String newName = "";
            char[] delimiterChars = { '\\' };
            String[] names = name.Split(delimiterChars);
            newName = names[names.Length - 1];
            return newName;
        }

        public static String user = "louis.lu";
        public static void createROProcessKedgeURL(String loginID,String caseNo,int? projectID,ref String status, ref String message)
        {
            //0測試機，1正式機
            String approveSite = null;
            try
            {
                approveSite = getReturnURL(projectID);
            }catch (Exception e)
            {
                message = "KGSetUp沒有相關資訊";
                return;
            }
            
            if ("0".Equals(approveSite))
            {
                //status = "http://";
                using (Kedge.ws.EngineerPR.EngineerPRService soapClient = new Kedge.ws.EngineerPR.EngineerPRService())
                {
                    try
                    {
                        status = soapClient.createROProcessKedgeURL(getName(loginID), caseNo);
                        //status = soapClient.createROProcessKedgeURL(user, caseNo);
                    }
                    catch (PXException pxe)
                    {

                        throw pxe;
                    }
                    catch (Exception e)
                    {
                        message = "AgentFlow寫入createROProcess異常";
                    }

                }
            }
            else if("1".Equals(approveSite))
            {
                using (Kedge.ws.EngineerPRExt.EngineerPRService soapClient = new Kedge.ws.EngineerPRExt.EngineerPRService())
                {
                    try
                    {
                        status = soapClient.createROProcessKedgeURL(getName(loginID), caseNo);
                        //status = soapClient.createROProcessKedgeURL(user, caseNo);
                    }
                    catch (PXException pxe)
                    {

                        throw pxe;
                    }
                    catch (Exception e)
                    {
                        message = "AgentFlow寫入createROProcess異常";
                    }

                }
            }
        }


        public static void createPOProcessKedgeURL(String loginID, String caseNo, int? projectID, ref String status, ref String message)
        {
            //0測試機，1正式機
            //0測試機，1正式機
            String approveSite = null;
            try
            {
                approveSite = getReturnURL(projectID);
            }
            catch (Exception e)
            {
                message = "KGSetUp沒有相關資訊";
                throw new PXException("KGSetUp沒有相關資訊");
            }

            if ("0".Equals(approveSite))
            {
                //status = "http://";
                try
                {
                    using (Kedge.ws.EngineerPR.EngineerPRService service = new Kedge.ws.EngineerPR.EngineerPRService())
                    {
                        string url = service.createPOProcessKedgeURL(getName(loginID), caseNo);
                        //string url = service.createPOProcessKedgeURL(user, caseNo);
                        status = url;
                        if (url.StartsWith("http") == false)
                        {
                            message = "Failed to call AgentFlow";
                            throw new PXException("Failed to call AgentFlow.");
                        }
                    }
                }
                catch (PXException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    message = "There is an exception to writing to AgentFlow[createPOProcessKedgeURL].  "+e.Source;
                    throw new PXException("There is an exception to writing to AgentFlow [createPOProcessKedgeURL]. {0}", e.Source);

                }
            }
            else if ("1".Equals(approveSite))
            {
                try
                {
                    using (Kedge.ws.EngineerPRExt.EngineerPRService service = new Kedge.ws.EngineerPRExt.EngineerPRService())
                    {
                        string url = service.createPOProcessKedgeURL(getName(loginID), caseNo);
                        //string url = service.createPOProcessKedgeURL(user, caseNo);
                        status = url;
                        if (url.StartsWith("http") == false)
                        {
                            message = "Failed to call AgentFlow";
                            throw new PXException("Failed to call AgentFlow.");
                        }
                    }
                }
                catch (PXException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    message = "There is an exception to writing to AgentFlow[createPOProcessKedgeURL].  " + e.Source;
                    throw new PXException("There is an exception to writing to AgentFlow [createPOProcessKedgeURL]. {0}", e.Source);

                }
            }


        }
        public static void createContractProcessKedgeURL(String loginID, String caseNo, int? projectID, ref String status, ref String message) {

            //0測試機，1正式機
            String approveSite = null;
            try
            {
                approveSite = getReturnURL(projectID);
            }
            catch (Exception e)
            {
                message = "KGSetUp沒有相關資訊";
                return;
            }
            if ("0".Equals(approveSite))
            {
                //status = "http://";
                using (Kedge.ws.EngineerPR.EngineerPRService service = new Kedge.ws.EngineerPR.EngineerPRService())
                {
                    try
                    {
                        status = service.createContractProcessKedgeURL(getName(loginID), caseNo);
                        //status = service.createContractProcessKedgeURL(user, caseNo);
                    }
                    catch (PXException pxe)
                    {

                        throw pxe;
                    }
                    catch (Exception e)
                    {
                        message = "AgentFlow寫入ContractProcessKedge異常";
                    }

                }
            }
            else if ("1".Equals(approveSite))
            {
                using (Kedge.ws.EngineerPRExt.EngineerPRService service = new Kedge.ws.EngineerPRExt.EngineerPRService())
                {
                    try
                    {
                        status = service.createContractProcessKedgeURL(getName(loginID), caseNo);
                        //status = service.createContractProcessKedgeURL(user, caseNo);
                    }
                    catch (PXException pxe)
                    {

                        throw pxe;
                    }
                    catch (Exception e)
                    {
                        message = "AgentFlow寫入ContractProcessKedge異常";
                    }

                }
            }


        }
        public static void createdoStartURL(String loginID, String caseNo, int? projectID, ref String status, ref String message)
        {
            //0測試機，1正式機
            String approveSite = null;
            try
            {
                approveSite = getReturnURL(projectID);
            }
            catch (Exception e)
            {
                message = "KGSetUp沒有相關資訊";
                return;
            }
            if ("0".Equals(approveSite))
            {
                //status = "http://";
                using (Kedge.ws.EngineerBillWeb.EngineerBillWebKedgeService soapClient = new Kedge.ws.EngineerBillWeb.EngineerBillWebKedgeService())
                {
                    try
                    {
                        System.Threading.Thread.Sleep(3000);
                        status = soapClient.doStartURL(getName(loginID), caseNo);
                        //status = soapClient.doStartURL(user, caseNo);
                    }
                    catch (PXException e)
                    {

                        throw e;
                    }
                    catch (Exception e)
                    {
                        //throw new PXException("AgentFlow寫入createROProcess異常");
                        message = APInvoiceEntry_Extension.Message.CreateROProcessError;
                    }

                }
            }
            else if ("1".Equals(approveSite))
            {
                using (Kedge.ws.EngineerBillWebExt.EngineerBillWebKedgeService soapClient = new Kedge.ws.EngineerBillWebExt.EngineerBillWebKedgeService())
                {
                    try
                    {
                        System.Threading.Thread.Sleep(3000);
                        status = soapClient.doStartURL(getName(loginID), caseNo);
                        //status = soapClient.doStartURL(user, caseNo);
                    }
                    catch (PXException e)
                    {

                        throw e;
                    }
                    catch (Exception e)
                    {
                        //throw new PXException("AgentFlow寫入createROProcess異常");
                        message = APInvoiceEntry_Extension.Message.CreateROProcessError;
                    }

                }
            }
        }
        //0測試機，1正式機
        public static void createChangeManagementProcessKedgeURL(String loginID, String caseNo, int? projectID, ref String status, ref String message)
        {
            //0測試機，1正式機
            String approveSite = null;
            try
            {
                approveSite = getReturnURL(projectID);
            }
            catch (Exception e)
            {
                message = "KGSetUp沒有相關資訊";
                return;
            }
            if ("0".Equals(approveSite))
            {
                // = "http://";
                using (Kedge.ws.EngineerPR.EngineerPRService soapClient = new Kedge.ws.EngineerPR.EngineerPRService())
                {
                    try
                    {
                        status = soapClient.createChangeManagementProcessKedgeURL(getName(loginID), caseNo);
                       //status = soapClient.createChangeManagementProcessKedgeURL(user, caseNo);
                    }
                    catch (PXException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        message = "AgentFlow寫入createROProcess異常";
                    }
                }
            }
            else if ("1".Equals(approveSite))
            {
                using (Kedge.ws.EngineerPRExt.EngineerPRService soapClient = new Kedge.ws.EngineerPRExt.EngineerPRService())
                {
                    try
                    {
                        status = soapClient.createChangeManagementProcessKedgeURL(getName(loginID), caseNo);
                        //status = soapClient.createChangeManagementProcessKedgeURL(user, caseNo);
                    }
                    catch (PXException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        message = "AgentFlow寫入createROProcess異常";
                    }
                }
            }
        }
        public static void createOpenEngTravleAPEForm(String loginID, String refNbr, String enable, int? projectID, ref String status, ref String message) {
            String approveSite = null;
            try
            {
                approveSite = getReturnURL(projectID);
            }
            catch (Exception e)
            {
                message = "KGSetUp沒有相關資訊";
                return;
            }
            if ("0".Equals(approveSite))
            {
                using (Kedge.ws.EngineerERPWeb.EngineerERPWebService soapClient = new Kedge.ws.EngineerERPWeb.EngineerERPWebService())
                {
                    //RefNbr
                    try
                    {
                        status=soapClient.openEngTravleAPEForm(getName(loginID), refNbr, enable);
                        //status = soapClient.openEngTravleAPEForm(user, refNbr, enable);
                    }
                    catch (PXException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        message = "AgentFlow寫入OpenEngTravleAPEForm異常";
                    }
                }
            }
            else if ("1".Equals(approveSite))
            {
                using (Kedge.ws.EngineerERPWebExt.EngineerERPWebService soapClient = new Kedge.ws.EngineerERPWebExt.EngineerERPWebService())
                {
                    //RefNbr
                    try
                    {
                        status = soapClient.openEngTravleAPEForm(getName(loginID), refNbr, enable);
                        //status = soapClient.openEngTravleAPEForm(user, refNbr, enable);
                    }
                    catch (PXException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        message = "AgentFlow寫入OpenEngTravleAPEForm異常";
                    }
                }
            }


        }
        public static void getEngTravleAmount( String refNbr, int? projectID, ref String status, ref String message) {
            String approveSite = null;
            try
            {
                approveSite = getReturnURL(projectID);
            }
            catch (Exception e)
            {
                message = "KGSetUp沒有相關資訊";
                return;
            }
            if ("0".Equals(approveSite))
            {
                using (Kedge.ws.EngineerERPWeb.EngineerERPWebService soapClient = new Kedge.ws.EngineerERPWeb.EngineerERPWebService())
                {
                    //RefNbr
                    try
                    {
                        status = soapClient.getEngTravleAmount(refNbr);
              
                    }
                    catch (PXException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        message = "找不到差旅費資料";
                    }
                }
            }
            else if ("1".Equals(approveSite))
            {
                using (Kedge.ws.EngineerERPWebExt.EngineerERPWebService soapClient = new Kedge.ws.EngineerERPWebExt.EngineerERPWebService())
                {
                    //RefNbr
                    try
                    {
                        status = soapClient.getEngTravleAmount(refNbr);

                    }
                    catch (PXException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        message = "找不到差旅費資料";
                    }
                }
            }
        }

        //0測試機，1正式機
        public static void createProccessBill(String CompanyID, String AfmNo, String AgentFlowID, String DBName, int? projectID, ref String status, ref String message)
        {
            String approveSite = null;
            try
            {
                approveSite = getReturnURL(projectID);
            }
            catch (Exception e)
            {
                message = "KGSetUp沒有相關資訊";
                return;
            }
            if ("0".Equals(approveSite))
            {
                using (Kedge.ws.ExpenseClaimWeb.KEDGEWebServiceService soapClient = new ws.ExpenseClaimWeb.KEDGEWebServiceService())
                {
                    try
                    {
                        status = soapClient.createProccessBill(CompanyID, AfmNo, AgentFlowID, DBName);
                        if (status.Contains("Fail")) message = "AF流程啟動失敗，回傳AF檢查錯誤訊息，可能原因：1.無法正常參照ERP資料 2.相關資料不存在於AF中。";
                    }
                    catch (PXException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        message = "AF判斷項目錯誤訊息";
                    }
                }
            }
            else if ("1".Equals(approveSite))
            {
                
            }
        }


        //0測試機，1正式機
        public static String getReturnURL(int? projectID)
        {
            PXGraph graph = new PXGraph();
            KGApproveSetup setUp = PXSelect<KGApproveSetup, Where<KGApproveSetup.contractID,
                                Equal<Required<KGApproveSetup.contractID>>>>.Select(graph, projectID);
            if (setUp != null)
            {
                return setUp.ApprovalSite;
            }
            else {
                throw  new  Exception("Error");
            }
        }

        public static bool? getIsAutoApproved(int? projectID)
        {
            PXGraph graph = new PXGraph();
            KGApproveSetup setUp = PXSelect<KGApproveSetup, Where<KGApproveSetup.contractID,
                                Equal<Required<KGApproveSetup.contractID>>>>.Select(graph, projectID);
            if (setUp != null)
            {
                return setUp.IsAutoApproved;
            }
            else
            {
                throw new Exception("Error");
            }
        }

        /**
         * <summary>
         * 取得Employee Agent Flow ID
         * </summary>
         * **/
        private static String getAgentID(string username) {
            PXResultset<CSAnswers> rs = PXSelectJoin<CSAnswers,
                InnerJoin<EPEmployee, On<EPEmployee.noteID, Equal<CSAnswers.refNoteID>>,
                InnerJoin<PX.SM.Users,On<PX.SM.Users.pKID,Equal<EPEmployee.userID>>>>,
                Where<PX.SM.Users.username, Equal<Required<PX.SM.Users.username>>,
                    And<CSAnswers.attributeID,Equal<Required<CSAnswers.attributeID>>>>
                >.Select(new PXGraph(), username,"AGENTID");
            if (rs.Count == 0) {
                return null;
            }
            return ((CSAnswers)rs).Value;
        }

    }
}
