using System;
using PX.Data;
using Kedge.DAC;
using PX.Objects.PO;
using PX.Objects.PM;
using PX.Objects.IN;
using PX.Objects.GL;
using System.Collections.Generic;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.AP;


/***
 * ====2021-06-21 :12103 ====Alton 
 * 1.���⧹�u�������ƶq, �Ф��n�p����]������l�ƶq, �u�n�֭p�q���ܧ�(�������M�׵���)���⪺�ܧ�ƶq.
 * 2.���⧹�u�������ƻ�, �Ф��n�p����]������l���B, �u�n�֭p�q���ܧ�(�������M�׵���)���⪺�ܧ���B.
 * ====2021-06-22:�f�Y====Alton
 * ����ƶq&�_���ѳ̫�@���令�֭p
 * ====2021-06-28 :12112 ====Alton
 * 1.����̫e���·зs�W�w��M�ץ���(PMCostBudget.ProjectTaskID)�αM�ץ��ȴy�z(PMCostBudget.ProjectTaskID_description)
 * 2.����ƶq/�ƻ�, �ЧP�_�p�G�����⪺�q���ܧ�, ����ܵ��⪺�ƶq/���B, �p�G�S���i��L����, ����ܤ����������ƶq/�ƻ�
 * 3.�p�G�ӵ����]�������h�����⪺�q���ܧ�, �Ш̷ӭq���ܧ�RefNbr�Ƨ�, �γ̤j(�pCO00001, CO00002, �Хu��CO00002)����������q���ܧ�, ���n�֥[. ����q���ܧ�s���]�Хu���(CO00002)
 * 4.�q���ܧ�s���b����label�Ч令����渹.
 * 5.�p�G����q���ܧ󪬺A�٤��OOpen, �Ф��n�b�����e�{, �{�b�|���O�_����O'N', ���O���q���ܧ�s��
 * 
 * **/
namespace Kedge
{
    /**
     * <summary>
     * KG605001�w��o�]�дڤ@����(reportID=KG605005)
     * KG605002�w��o�]�дڵ���@����(reportID=KG605006)
     * </summary>
     * */
    public class KG605002Report : PXGraph<KG605002Report>
    {

        //public PXSave<MasterTable> Save;
        //public PXCancel<MasterTable> Cancel;

        #region view
        public PXFilter<KG605002Filter> MasterView;
        public PXSelect<KG605002> DetailsView;
        #endregion

        #region Button

        #region �������
        public PXAction<KG605002Filter> ProcessAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "�������")]
        protected void processAction()
        {
            KG605002Report graph = PXGraph.CreateInstance<KG605002Report>();
            String screenID = graph.Accessinfo.ScreenID;
            bool isFinalRp = "KG.60.50.02" == screenID;

            //����ReportID:KG605006
            //�L����ReportID:KG605005
            String reportID = isFinalRp ? "KG605006" : "KG605005";

            //�R��7�ѫe���¸��
            DateTime businessDate = (DateTime)graph.Accessinfo.BusinessDate;
            DeleteOldData(businessDate, 7);

            //�}�l���ͳ���
            DateTime now = System.DateTime.Now;
            string BatchID = graph.Accessinfo.UserName + now.ToString("yyyyMMddHHmmss") + reportID;
            KG605002Filter filter = MasterView.Current;
            if (filter != null)
            {
                if (filter.ProjectID == null)
                {
                    String msg = "�M�פ��i���ŭ�!";
                    MasterView.Cache.RaiseExceptionHandling<KG605002Filter.projectID>(
                        filter, filter.ProjectID, new PXSetPropertyException(msg));
                    return;
                }
                if (filter.DeadLine == null)
                {
                    String msg = "����餣�i���ŭ�!";
                    MasterView.Cache.RaiseExceptionHandling<KG605002Filter.deadLine>(
                        filter, filter.DeadLine, new PXSetPropertyException(msg));
                    return;
                }

                Dictionary<string, string> patams = new Dictionary<string, string>();
                DateTime date = (DateTime)filter.DeadLine;


                Contract contract = GetContract(filter.ProjectID);
                //���oTitle��T
                string p_contractCD = contract.ContractCD;
                string p_description = contract.Description;
                //string p_printDate = now.ToString("yyyy/MM/dd HH:mm:ss");


                //����ReportData
                string p_totalAmt = CreateReportData(BatchID, filter.ProjectID, filter.DeadLine, isFinalRp).ToString();
                //�ǤJ����Ѽ�
                patams["P_BatchID"] = BatchID;
                patams["P_ContractCD"] = p_contractCD;
                patams["P_Description"] = p_description;
                patams["P_DeadLine"] = date.ToString("yyyy/MM/dd");
                patams["P_TotalAmt"] = p_totalAmt;
                throw new PXReportRequiredException(patams, reportID, "Report");
            }
        }
        #endregion
        #endregion

        #region Method

        /**
         * <summary>
         * ���oContract��T
         * </summary>
         */
        private Contract GetContract(int? ProjectID)
        {
            return PXSelect<Contract,
                Where<Contract.contractID, Equal<Required<POOrder.projectID>>>>
                .Select(this, ProjectID);
        }

        /**
         * <summary>
         * ����ReportData
         * </summary>
         */
        private decimal? CreateReportData(String BatchID, int? projectID, DateTime? deadLine, bool isFinalRp)
        {
            decimal? totalAmt = 0;
            int seq = 0;
            PXResultset<Segment> costCodeSplit = GetSegment("COSTCODE");
            PXResultset<Segment> taskSplit = GetSegment("PROTASK");
            KG605002 header = null;
            foreach (PXResult<PMBudget, PMTask, PMCostCode, InventoryItem, Account, CSAttributeDetail> data in GetDetailHeader(projectID))
            {
                //subDetail�Ѽ�
                int? costCodeID = ((PMBudget)data).CostCodeID;
                int? inventoryID = ((PMBudget)data).InventoryID;
                int? taskID = ((PMBudget)data).ProjectTaskID;
                bool isNewGroup = true;
                //���o���]����
                PXResultset<POLine> po_rs = GetSubDetailByPO(deadLine, projectID, taskID, costCodeID, inventoryID);
                //����]��������ƮɡA�ĥΤ��]������T
                if (po_rs.Count > 0)
                {
                    foreach (PXResult<POLine, POOrder, Contract, Vendor> poData in po_rs)
                    {
                        KG605002 item = NewKG605002(data, BatchID, seq++, isNewGroup, costCodeSplit, taskSplit);
                        //����header��T
                        if (isNewGroup)
                        {
                            header = item;
                        }
                        isNewGroup = false;
                        item.ProjectCode = ((Contract)poData).ContractCD;
                        //�o�]�X���s��
                        item.OrderNbr = ((POOrder)poData).OrderNbr;
                        //�o�]�X������
                        item.OrderDesc = ((POOrder)poData).OrderDesc;
                        //�o�]�����
                        item.VendorName = ((Vendor)poData).AcctName;
                        //�o�]�ƶq
                        item.OrderQty = ((POLine)poData).OrderQty;
                        //�o�]���
                        item.CuryUnitCost = ((POLine)poData).CuryUnitCost;
                        //�o�]�_��
                        item.CuryLineAmt = ((POLine)poData).CuryLineAmt;
                        //��y�z
                        item.TranDesc = ((POLine)poData).TranDesc;

                        /* ���p���� 20200121
                        //����ƶq
                        item.BilledQty = ((POLine)poData).BilledQty;
                        //����_��
                        item.CuryBilledAmt = ((POLine)poData).CuryBilledAmt;
                        */
                        //�p����Ѽ�
                        string orderNbr = ((POOrder)poData).OrderNbr;
                        string orderType = ((POOrder)poData).OrderType;
                        int? branchID = ((POOrder)poData).BranchID; ;
                        int? vendorID = ((POOrder)poData).VendorID; ;
                        int? vendorLocationID = ((POOrder)poData).VendorLocationID;
                        int? lineNbr = ((POLine)poData).LineNbr;
                        PXResultset<APTran> ap_t =
                            GetAPInvoice(orderNbr, orderType, branchID, vendorID, vendorLocationID, projectID, inventoryID, costCodeID, taskID, lineNbr);

                        //2021-06-22 add by alton ����ƶq&�_���ѳ̫�@���令�֭p
                        decimal billedQty = 0m;
                        decimal curyBilledAmt = 0m;
                        foreach (PXResult<APTran, APRegister> apData in ap_t)
                        {
                            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>((APRegister)apData);
                            if (((APRegister)apData).Hold == false && apRegisterExt.UsrFlowStatus == "D")
                            {
                                //����ƶq
                                //item.BilledQty = ((APTran)apData).Qty;
                                billedQty += ((APTran)apData)?.Qty ?? 0m;
                                //����_��
                                //item.CuryBilledAmt = ((APTran)apData).CuryLineAmt;
                                curyBilledAmt += ((APTran)apData)?.CuryLineAmt ?? 0m;
                            }
                        }
                        item.BilledQty = billedQty;
                        item.CuryBilledAmt = curyBilledAmt;

                        if (isFinalRp)
                        {
                            //���o���]�����M�׵���
                            PXResultset<PMChangeOrderLine> poc_rs = GetSubDetailByPOC(orderNbr, orderType, lineNbr);
                            //Mark by Alton mantis:12103 user���M�׵���ɪ����]�����Ȯ��Ӱ������A�]�����p���l���B�ƶq
                            //2021-06-28 ����ƶq/�ƻ�, �ЧP�_�p�G�����⪺�q���ܧ�, ����ܵ��⪺�ƶq/���B, �p�G�S���i��L����, ����ܤ����������ƶq/�ƻ�
                            decimal? finalQty = item.OrderQty ?? 0;//�_�l�Ȭ��G<�o�]����>�ƶq
                            decimal? finalAmount = item.CuryLineAmt ?? 0;//�_�l�Ȭ��G<�o�]����>�_��
                            String isFinal = "N";
                            String refNbr = "";
                            if (poc_rs.Count > 0)
                            {
                                //2021-06-28 �p�G�ӵ����]�������h�����⪺�q���ܧ�, �Ш̷ӭq���ܧ�RefNbr�Ƨ�, �γ̤j(�pCO00001, CO00002, �Хu��CO00002)����������q���ܧ�, ���n�֥[. ����q���ܧ�s���]�Хu���(CO00002)
                                //foreach (PMChangeOrderLine pmcol in poc_rs)
                                //{
                                //    refNbr = pmcol.RefNbr;
                                //    finalQty += (pmcol.Qty == null ? 0 : pmcol.Qty);
                                //    finalAmount += (pmcol.Amount == null ? 0 : pmcol.Amount);
                                //    isFinal = "Y";
                                //}
                                //�u��@��
                                PMChangeOrderLine pmcol = poc_rs;
                                refNbr = pmcol.RefNbr;
                                finalQty = pmcol.Qty ?? 0;
                                finalAmount = pmcol.Amount ?? 0;
                                isFinal = "Y";
                            }

                            item.IsFinal = isFinal;
                            item.RefNbr = refNbr;
                            item.FinalQty = finalQty;
                            item.FinalAmount = finalAmount;
                            //�p��[�`
                            totalAmt += item.FinalAmount;
                        }
                        //�p��l�B
                        SetBalance(header, item);
                        //�g�JDB
                        DetailsView.Update(item);
                    }
                    //���s��sheader
                }
                //�_�h��Φ����w��
                else
                {
                    KG605002 item = NewKG605002(data, BatchID, seq++, isNewGroup, costCodeSplit, taskSplit);
                    if (isNewGroup)
                    {
                        header = item;
                    }
                    isNewGroup = false;
                    if (isFinalRp)
                    {
                        //edit alton 20191114 ��[�O�_����] ��N�� ���u�ƶq&�_�� �ݬ�0
                        decimal? finalQty = 0;
                        decimal? finalAmount = 0;
                        PXResultset<PMChangeOrderBudget> pmcob_rs = GetSubDetailByPOB(projectID, taskID, costCodeID, inventoryID);
                        String isFinal = "N";
                        foreach (PMChangeOrderBudget pmcob in pmcob_rs)
                        {

                            if (isFinal == "N")
                            {//edit alton 20191114 ��[�O�_����] ��N�� ���u�ƶq&�_�� �ݬ�0
                                finalQty = (item.RevisedQty == null ? 0 : item.RevisedQty);//�_�l�Ȭ��G<�w��>�ƶq
                                finalAmount = (item.CuryRevisedAmount == null ? 0 : item.CuryRevisedAmount);//�_�l�Ȭ��G<�w��>�_��
                            }

                            finalQty += (pmcob.Qty == null ? 0 : pmcob.Qty);
                            finalAmount += (pmcob.Amount == null ? 0 : pmcob.Amount);
                            isFinal = "Y";
                        }
                        item.IsFinal = isFinal;
                        item.FinalQty = finalQty;
                        item.FinalAmount = finalAmount;
                        //�p��[�`
                        totalAmt += item.FinalAmount;
                    }
                    //�p��l�B
                    SetBalance(header, item);
                    //�g�JDB
                    DetailsView.Update(item);
                }
                //���s��sHeader��T
                if (header != null)
                {
                    DetailsView.Update(header);
                }
            }

            base.Persist();
            return totalAmt;
        }

        /**
         * <summary>
         * ���Y��T
         * </summary>
         * */
        private KG605002 NewKG605002(PXResult<PMBudget, PMTask, PMCostCode, InventoryItem, Account, CSAttributeDetail> data, String BatchID, int seq, bool isNewGroup, PXResultset<Segment> costCodeSplit, PXResultset<Segment> taskSplit)
        {
            PMCostCode pMCostCode = (PMCostCode)data;
            InventoryItem inventoryItem = (InventoryItem)data;
            CSAttributeDetail cSAttributeDetail = (CSAttributeDetail)data;
            Account account = (Account)data;
            PMBudget pMBudget = (PMBudget)data;
            PMTask pMTask = (PMTask)data;

            KG605002 item = new KG605002();
            item.BatchID = BatchID;
            item.BatchSeq = seq;
            item.IsFinal = "N";
            if (isNewGroup)
            {
                PMBudgetExt pmbe = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pMBudget);
                //�M�ץ���
                item.ProjectTaskCD = DoSplit(taskSplit, pMTask.TaskCD);
                //�M�ץ��ȴy�z
                item.ProjectTaskDesc = pMTask.Description;
                //�p���椸
                item.CostCodeCD = DoSplit(costCodeSplit, pMCostCode.CostCodeCD);
                //�p���椸�y�z
                item.CostCodeDesc = pMCostCode.Description;
                //�u���N�X
                item.InventoryCD = inventoryItem.InventoryCD;
                //�u���N�X
                item.InvetoryDesc = pmbe.UsrInventoryDesc;
                //�|�p���
                item.AccountCD = account.AccountCD;
                //���
                item.Uom = pMBudget.UOM;
                //�o�]����
                //item.UsrInvPrTypeName = pmbe.UsrInvPrType;
                item.UsrInvPrTypeName = cSAttributeDetail.Description;
                //�w��ƶq
                item.RevisedQty = pMBudget.RevisedQty;
                //�w����
                item.CuryUnitRate = pMBudget.CuryUnitRate;
                //�w��_��
                item.CuryRevisedAmount = pMBudget.CuryRevisedAmount;
                //�w��l�B�_�l�Ȭ��w��_��
                item.Balance = item.CuryRevisedAmount;
            }
            return item;
        }

        /**
         * <summary>
         * �p��l�B
         * </summary>
         * <param name="header">header��T</param>
         * <param name="item">���Ӹ�T</param>
         */
        private void SetBalance(KG605002 header, KG605002 item)
        {
            if (item.OrderNbr != null)
            {//�o�]����
                if (header.Uom == "��")
                {//���="��"�G�w��l�B-�o�]�����ƻ�
                    header.Balance -= item.CuryLineAmt;
                }
                else
                {//���!="��"�G�w��l�B-(�w���� * �o�]�ƶq)
                    header.Balance -= (header.CuryUnitRate * item.OrderQty);
                }
            }
            else
            {//�D�o�]
                if (item.IsFinal == "Y")
                {//�w����
                    //�w��l�B-����_��
                    header.Balance -= item.FinalAmount;
                }
            }
        }

        /**
         * <summary>
         * �_�I�B�z
         * </summary>
         * **/
        private String DoSplit(PXResultset<Segment> splits, string strVal)
        {
            String returnVal = "";
            String endSplit = "";
            int index = 0;
            if (strVal != null)
            {
                foreach (Segment split in splits)
                {
                    returnVal += endSplit + strVal.Substring(index, (int)split.Length);
                    endSplit = split.Separator;
                    index += (int)split.Length;
                    if (index > strVal.Length)
                    {
                        index = strVal.Length;
                    }
                }
                return returnVal;
            }
            return strVal;
        }


        /**
         * <summary>
         * ���oSegment
         * </summary>
         */
        private PXResultset<Segment> GetSegment(string dimensionID)
        {
            return
                PXSelect<Segment,
                    Where<Segment.dimensionID, Equal<Required<Segment.dimensionID>>>,
                    OrderBy<Asc<Segment.segmentID>>
                >.Select(this, dimensionID);
        }

        /**
         * <summary>
         * ���oDetailHeader��T
         * </summary>
         */
        private PXResultset<PMBudget> GetDetailHeader(int? ProjectID)
        {
            return
                PXSelectJoin<
                    PMBudget,
                    InnerJoin<PMTask, On<PMBudget.projectTaskID, Equal<PMTask.taskID>, And<PMBudget.projectID, Equal<PMTask.projectID>>>,
                    InnerJoin<PMCostCode, On<PMBudget.costCodeID, Equal<PMCostCode.costCodeID>>,
                    InnerJoin<InventoryItem, On<PMBudget.inventoryID, Equal<InventoryItem.inventoryID>>,
                    InnerJoin<Account, On<PMBudget.accountGroupID, Equal<Account.accountGroupID>>,
                    LeftJoin<CSAttributeDetail, On<PMBudgetExt.usrInvPrType, Equal<CSAttributeDetail.valueID>, And<CSAttributeDetail.attributeID, Equal<Required<CSAttributeDetail.attributeID>>>>>
                    >>>>,
                    Where<PMBudget.projectID, Equal<Required<PMBudget.projectID>>,
                    And<PMBudget.type, Equal<Required<PMBudget.type>>>
                    >,
                    OrderBy<Asc<PMCostCode.costCodeCD, Asc<PMBudget.inventoryID, Asc<Account.accountCD>>>>

              >
                .Select(this, "INVPRTYPE", ProjectID, "E");
        }

        /**
         * <summary>
         * ���oSubDetail-���]������T
         * </summary>
         */
        private PXResultset<POLine> GetSubDetailByPO(DateTime? deadLine, int? projectID, int? taskID, int? costCodeID, int? inventoryID)
        {
            return
                PXSelectJoin<
                    POLine,
                    InnerJoin<POOrder, On<POLine.orderNbr, Equal<POOrder.orderNbr>, And<POLine.orderType, Equal<POOrder.orderType>>>,
                    InnerJoin<Contract, On<POLine.projectID, Equal<Contract.contractID>>,
                    LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>>
                    >>>,
                    Where<POOrder.orderDate, LessEqual<Required<POOrder.orderDate>>,//deadLine
                    And<POLine.projectID, Equal<Required<POLine.projectID>>,//projectID
                    And<POLine.taskID, Equal<Required<POLine.taskID>>,//taskID
                    And<POLine.inventoryID, Equal<Required<POLine.inventoryID>>,//inventoryID
                    And<POLine.costCodeID, Equal<Required<POLine.costCodeID>>//costCodeID
                    >>>>>,
                    OrderBy<Asc<Contract.contractCD, Asc<POOrder.orderNbr>>>

              >
                .Select(this, deadLine, projectID, taskID, inventoryID, costCodeID);
        }

        /**
         * <summary>
         * ���oSubDetail-���]����-�M�׵����T
         * </summary>
         */
        private PXResultset<PMChangeOrderLine> GetSubDetailByPOC(string orderNbr, string orderType, int? lineNbr)
        {
            return
                PXSelectJoin<
                    PMChangeOrderLine,
                    InnerJoin<PMChangeOrder, On<PMChangeOrderLine.refNbr, Equal<PMChangeOrder.refNbr>>>,
                    Where<PMChangeOrderLine.pOOrderNbr, Equal<Required<PMChangeOrderLine.pOOrderNbr>>,//orderNbr
                    And<PMChangeOrderLine.pOOrderType, Equal<Required<PMChangeOrderLine.pOOrderType>>,//orderType
                    And<PMChangeOrderLine.pOLineNbr, Equal<Required<PMChangeOrderLine.pOLineNbr>>,//lineNbr
                    And<PMChangeOrder.classID, Equal<Required<PMChangeOrder.classID>>,//'�M�׵���'
                    And<PMChangeOrder.status, Equal<ChangeOrderStatus.open>//'O' -Open //20210628 �n�P�_status //20191112 status ���P�_
                    >>>>>,
                    OrderBy<Desc<PMChangeOrderLine.refNbr>>
              >
                .Select(this, orderNbr, orderType, lineNbr, "�M�׵���");
        }

        /**
         * <summary>
         * ���oSubDetail- �����w��-�M�׵���
         * </summary>
         */
        private PXResultset<PMChangeOrderBudget> GetSubDetailByPOB(int? projectID, int? taskID, int? costCodeID, int? inventoryID)
        {
            return
                PXSelectJoin<
                    PMChangeOrderBudget,
                    InnerJoin<PMChangeOrder, On<PMChangeOrderBudget.refNbr, Equal<PMChangeOrder.refNbr>>>,
                    Where<PMChangeOrderBudget.projectID, Equal<Required<PMChangeOrderBudget.projectID>>,//projectID
                    And<PMChangeOrderBudget.projectTaskID, Equal<Required<PMChangeOrderBudget.projectTaskID>>,//taskID
                    And<PMChangeOrderBudget.inventoryID, Equal<Required<PMChangeOrderBudget.inventoryID>>,//inventoryID
                    And<PMChangeOrderBudget.costCodeID, Equal<Required<PMChangeOrderBudget.costCodeID>>,//costCodeID
                    And<PMChangeOrder.classID, Equal<Required<PMChangeOrder.classID>>,//'�M�׵���'
                    And<PMChangeOrder.status, Equal<Required<PMChangeOrder.status>>//'O' -Open
                    >>>>>>
              >
                .Select(this, projectID, taskID, inventoryID, costCodeID, "�M�׵���", "O");
        }

        /**
         * <summary>
         * ���o�p������Ӫ� �ƶq�B�����p�p
         * </summary>
         */
        private PXResultset<APTran> GetAPInvoice(string orderNbr, string orderType, int? branchID, int? vendorID, int? vendorLocationID, int? projectID, int? inventoryID, int? costCodeID, int? taskID, int? poLineNbr)
        {
            return
                PXSelectJoin<
                    APTran,
                    InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTran.refNbr>>>,
                    Where<APRegisterExt.usrPONbr, Equal<Required<APRegisterExt.usrPONbr>>,//OrderNbr
                    And<APRegisterExt.usrPOOrderType, Equal<Required<APRegisterExt.usrPOOrderType>>,//OrderType
                    And<APRegister.branchID, Equal<Required<APRegister.branchID>>,//branchID
                    And<APRegister.vendorID, Equal<Required<APRegister.vendorID>>,//vendorID
                    And<APRegister.vendorLocationID, Equal<Required<APRegister.vendorLocationID>>,//vendorLocationID
                    And<APRegister.docType, Equal<Required<APRegister.docType>>,// = 'INV'
                    And<APTranExt.usrValuationType, Equal<Required<APTranExt.usrValuationType>>,// = 'B'
                    And<APTran.projectID, Equal<Required<APTran.projectID>>,//projectID
                    And<APTran.inventoryID, Equal<Required<APTran.inventoryID>>,//inventoryID
                    And<APTran.costCodeID, Equal<Required<APTran.costCodeID>>,//costCodeID
                    And<APTran.taskID, Equal<Required<APTran.taskID>>,//taskID
                    And<APTran.pOLineNbr, Equal<Required<APTran.pOLineNbr>>//poLineNbr
                    >>>>>>>>>>>>
              >
                .Select(this, orderNbr, orderType, branchID, vendorID, vendorLocationID, "INV", "B"
                    , projectID, inventoryID, costCodeID, taskID, poLineNbr);
        }

        /**
         * <summary>
         * �R���¸��
         * </summary>
         * <param name="date">��e�ɶ�</param>
         * <param name="day">�w�R���X�ѫe�����</param>
         */
        private void DeleteOldData(DateTime date, int day)
        {
            DateTime deadLine = date.AddDays(-day);
            PXResultset<KG605002> set = PXSelect<KG605002, Where<KG605002.createdDateTime, LessEqual<Required<KG605002.createdDateTime>>>>.Select(this, deadLine);
            foreach (KG605002 old in set)
            {
                DetailsView.Delete(old);
            }
            base.Persist();
        }

        #endregion

        #region filter
        [Serializable]
        public class KG605002Filter : IBqlTable
        {
            #region ProjectID
            [PXInt()]
            [PXUIField(DisplayName = "ProjectID", Required = true)]

            [PXSelector(typeof(Search2<PMProject.contractID,
                    LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                    LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                    Equal<PMProject.contractID>>>>,
                    Where<PMProject.baseType, Equal<CTPRType.project>,
                     And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>)
                    , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                    typeof(Customer.acctName), typeof(PMProject.status),
                    typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false)]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : IBqlField { }
            #endregion

            #region DeadLine
            [PXDate()]
            [PXUIField(DisplayName = "Dead Line", Required = true)]
            [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual DateTime? DeadLine { get; set; }
            public abstract class deadLine : IBqlField { }
            #endregion
        }
        #endregion

    }
}