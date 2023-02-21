using System;
using PX.Data;
using Kedge.DAC;
using System.Collections;
using PX.Objects.PO;
using PX.Objects.PM;
using PX.Objects.IN;
using PX.Objects.GL;
using System.Collections.Generic;
using PX.Objects.CT;
using PX.Objects.AP;
using PX.Objects.RQ;

namespace Kedge
{
    public class KG605003Report : PXGraph<KG605003Report>
    {
        public PXFilter<KG605003Filter> Filters;
        public PXSelect<KG605003> Details;
        
        #region Action

        #region �������

        public PXAction<KG605003Filter> ProcessAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "�������")]
        protected void processAction()
        {
            KG605003 kG605003 = Details.Current;
            KG605003Report graph = PXGraph.CreateInstance<KG605003Report>();
            DateTime date = DateTime.Now;
            //7�ѫe����ƧR��
            
            string BatchID = graph.Accessinfo.UserName+ date.ToString("yyyyMMddHHmmss");
            DeleteDB(date);
            CreateLine(BatchID);
            KG605003Filter filter = Filters.Current;
            String reportID = "KG605004";
            //�令�A������ScreenID
            if (filter != null)
            {
                Dictionary<string, string> mailParams = new Dictionary<string, string>();
                DateTime DeadLine = (DateTime)filter.DeadLine;
                Contract contract = GetContract(filter.ProjectID);
                string ProjectCD = contract.ContractCD;
                //�U���n��A���d�߰ѼƤ@��
                mailParams["ProjectID"] = ProjectCD;
                mailParams["DeadLine"] = DeadLine.ToString("yyyy/MM/dd");
                mailParams["BatchID"] = BatchID;
                throw new PXReportRequiredException(mailParams, reportID, "Report");
            }


        }

        #endregion

        #endregion

        #region Methods

        public void CreateLine(string BatchID)
        {
            KG605003Filter filter = Filters.Current;
            int Seq = 0;
            //2020/02/05 ADD OrderBy ����
            PXResultset<POLine> set = PXSelect<POLine, 
                Where<POLine.projectID, Equal<Required<KG605003Filter.projectID>>>,
                OrderBy<Asc<POLine.orderNbr, Asc<POLine.lineNbr>>>>
                .Select(this, filter.ProjectID);
            foreach (POLine poline in set)
            {
                KG605003 kG605003 = new KG605003();
                kG605003.BatchID = BatchID;
                kG605003.BatchSeq = Seq;
                Seq++;
                kG605003.OrderType = poline.OrderType;
                kG605003.OrderNbr = poline.OrderNbr;
                kG605003.LineNbr = poline.LineNbr;
                PMCostCode costCode = GetCostCode(poline.CostCodeID);
                if(costCode !=null)
                {
                    kG605003.CostCodeCD = costCode.CostCodeCD??"";
                    kG605003.CostCodeDesc = costCode.Description??"";
                }

                InventoryItem inventory = GetInventory(poline.InventoryID);
                if(inventory !=null)
                {
                    kG605003.InventoryCD = inventory.InventoryCD??"";
                }
                kG605003.InventoryDesc = poline.TranDesc??"";

                Account account = GetAccount(poline.ExpenseAcctID);
                if(account !=null)
                kG605003.AccountCD = account.AccountCD??"";

                kG605003.Uom = poline.UOM;
                PMBudget budget = GetBudget(poline.ProjectID, poline.TaskID,
                    poline.CostCodeID, poline.InventoryID);
                if(budget !=null)
                {
                    kG605003.RevisedQty = budget.RevisedQty??0;
                    kG605003.CuryUnitRate = budget.CuryUnitRate??0;
                    kG605003.CuryRevisedAmount = budget.CuryRevisedAmount??0;
                }

                kG605003.OrderQty = poline.OrderQty;
                kG605003.CuryUnitCost = poline.CuryUnitCost;
                kG605003.CuryLineAmt = poline.CuryLineAmt;
                //2020/01/30 ADD 4����� ���ʵ��l+���]�W��<�ƶq&���&�ƻ�>
                //���]�W���M���]���������Y�n���z�L���ʳ�
                //���]���������ʳ�
                kG605003.RQOrderQty = 0;
                kG605003.RQUnitCost = 0;
                kG605003.RQAmt = 0;
                RQRequestLine requestLine = GetRequestLine(poline);
                if (requestLine != null)
                {
                    kG605003.RQOrderQty = requestLine.OrderQty ?? 0;
                    kG605003.RQUnitCost = requestLine.CuryEstUnitCost ?? 0;
                    kG605003.RQAmt = requestLine.CuryEstExtCost ?? 0;
                }

                ///���ʵ��l
                ///if���]����
                ///����(�D�@����): (���]�W��.��� �V �o�]����.���) * �o�]����.�ƶq
                ///����(�@����): ���]�W��.�ƻ� - �o�]����.�ƻ�
                ///else if �s�P�X��else if "���]����"�ꤣ������� "���]�W��"Line
                ///�a0
               if(requestLine ==null)
                {
                    kG605003.PurchaseBalance = 0;
                }
               else
                {
                    if (poline.OrderType == "RO")
                    {
                        if (kG605003.Uom != "��")
                        {
                            kG605003.PurchaseBalance = (kG605003.RQUnitCost - kG605003.CuryUnitCost) * kG605003.OrderQty;
                        }
                        else
                        {
                            kG605003.PurchaseBalance = kG605003.RQAmt - kG605003.CuryLineAmt;
                        }
                    }
                    else
                    {
                        kG605003.PurchaseBalance = 0;
                    }
                }
               
                

                ///2020/01/22 ADD ����ƶq&�ƻ� �n�[����
                ///��PO���uAPInvoice.DocType=��INV��(�p��) + APRegister.UsrFlowStatus = ��D�� (�w�֥i)�v
                ///�B�uAPTran.UsrValuationType=��B��(�p��)�v��APTran.Qty�BAPTran.CuryLineAmt
                
                APTran aPTran = GetBilled(poline);
                if (aPTran != null)
                {
                    kG605003.BilledQty = aPTran.Qty;//poline.BilledQty;
                    kG605003.CuryBilledAmt = aPTran.CuryLineAmt;//poline.CuryBilledAmt;
                }else
                {
                    kG605003.BilledQty = 0;//poline.BilledQty;
                    kG605003.CuryBilledAmt = 0;//poline.CuryBilledAmt;
                }
                
                PMChangeOrderLine changeOrderLine = GetChangeOrderLine(poline.OrderNbr,
                    poline.OrderType, poline.LineNbr);
                if(changeOrderLine == null)
                {
                    kG605003.IsBalance = "N";
                    kG605003.FinalQty = poline.OrderQty;
                    kG605003.FinalAmount = poline.CuryLineAmt;
                }
                else
                {
                    kG605003.IsBalance = "Y";
                    kG605003.FinalQty = changeOrderLine.Qty + poline.OrderQty;
                    kG605003.FinalAmount = changeOrderLine.Amount + poline.CuryLineAmt;
                }
                Details.Update(kG605003);
            }
            base.Persist();
        }

        public void DeleteDB(DateTime date)
        {
            DateTime LastWeek = date.AddDays(-7);
            PXResultset<KG605003> set = PXSelect<KG605003,
                Where<KG605003.createdDateTime, 
                LessEqual<Required<KG605003.createdDateTime>>>>
                .Select(this, LastWeek);               
            foreach(KG605003 DBDetail in set)
            {
                if (DBDetail == null) return;
                Details.Delete(DBDetail);                                
            }
            base.Persist();
        }

        private PMCostCode GetCostCode(int? CostCodeID)
        {
            return PXSelect<PMCostCode,
                Where<PMCostCode.costCodeID, Equal<Required<POLine.costCodeID>>>>
                .Select(this, CostCodeID);
        }

        private InventoryItem GetInventory(int? InventoryID)
        {
            return PXSelect<InventoryItem,
                Where<InventoryItem.inventoryID, Equal<Required<POLine.inventoryID>>>>
                .Select(this, InventoryID);
      
        }

        private Account GetAccount(int? AccountID)
        {
            return PXSelect<Account,
                Where<Account.accountID, Equal<Required<POLine.expenseAcctID>>>>
                .Select(this, AccountID);
        }

        private PMBudget GetBudget(int? ProjectID,int? TaskID,
            int? CostCodeID, int? InventoryID )
        {
            return PXSelect<PMBudget,
                Where<PMBudget.projectID, Equal<Required<POLine.projectID>>,
                And<PMBudget.projectTaskID,Equal<Required<POLine.taskID>>,
                And<PMBudget.costCodeID,Equal<Required<POLine.costCodeID>>,
                And<PMBudget.inventoryID,Equal<Required<POLine.inventoryID>>,
                And<PMBudget.type,Equal<word.e>>>>>>>
                .Select(this, ProjectID,TaskID,CostCodeID,InventoryID);
        }

        private PMChangeOrderLine GetChangeOrderLine(string OrderNbr,string OrderType, 
            int? LineNbr)
        {
            return PXSelectJoin<PMChangeOrderLine,
                InnerJoin<PMChangeOrder,On<PMChangeOrder.refNbr,Equal<PMChangeOrderLine.refNbr>>>,
                    Where<PMChangeOrder.classID,Equal<Required<PMChangeOrder.classID>>,
                    And<PMChangeOrder.status,Equal<word.o>,
                    And<PMChangeOrderLine.pOOrderNbr,Equal<Required<POLine.orderNbr>>,
                    And<PMChangeOrderLine.pOOrderType,Equal<Required<POLine.orderType>>,
                    And<PMChangeOrderLine.pOLineNbr,Equal<Required<POLine.lineNbr>>>>>>>>
                .Select(this,"�M�׵���",OrderNbr,OrderType,LineNbr);
        }

        private Contract GetContract(int? ProjectID)
        {
            return PXSelect<Contract,
                Where<Contract.contractID,Equal<Required<POOrder.projectID>>>>
                .Select(this, ProjectID);
        }

        //���o����p��
        private APTran GetBilled(POLine pOLine)
        {
            return PXSelectJoinGroupBy<APTran,
            InnerJoin<APInvoice, On<APInvoice.refNbr, Equal<APTran.refNbr>>,
            InnerJoin<APRegister, On<APRegister.refNbr, Equal<APInvoice.refNbr>>,
            InnerJoin<KGFlowSubAcc, On<KGFlowSubAcc.accUID, Equal<APRegisterExt.usrKGFlowUID>>>>>,
            Where<APTran.pONbr, Equal<Required<POLine.orderNbr>>,
            And<APTran.pOLineNbr, Equal<Required<POLine.lineNbr>>,
            And<APTran.projectID, Equal<Required<POLine.projectID>>,
            And<APTran.taskID, Equal<Required<POLine.taskID>>,
            And<APTran.costCodeID, Equal<Required<POLine.costCodeID>>,
            And<APTran.inventoryID, Equal<Required<POLine.inventoryID>>,
            And<APRegister.hold, Equal<False>,
            And<APRegisterExt.usrKGFlowUID, IsNotNull,
            And<APInvoice.docType, Equal<WordType.iNV>,
            And<KGFlowSubAcc.stateuid, Equal<word.stateD>,
            And<APTranExt.usrValuationType, Equal<word.b>>>>>>>>>>>>,
            Aggregate<GroupBy<APTran.pOLineNbr,
            Sum<APTran.qty,
            Sum<APTran.curyLineAmt>>>>>
            .Select(this, pOLine.OrderNbr, pOLine.LineNbr, pOLine.ProjectID,
            pOLine.TaskID, pOLine.CostCodeID, pOLine.InventoryID);


        }

        //���o���]�W��
        private RQRequestLine GetRequestLine(POLine pOline)
        {
            return PXSelectJoin<RQRequestLine,
                InnerJoin<RQRequisitionContent,
                On<RQRequisitionContent.orderNbr, Equal<RQRequestLine.orderNbr>,
                And<RQRequisitionContent.lineNbr,Equal<RQRequestLine.lineNbr>>>,
                InnerJoin<RQRequisitionLine,
                On<RQRequisitionLine.reqNbr,Equal<RQRequisitionContent.reqNbr>,
                And<RQRequisitionLine.lineNbr,Equal<RQRequisitionContent.reqLineNbr>>>>>,
                Where<RQRequisitionLine.reqNbr,Equal<Required<POLine.rQReqNbr>>,
                And<RQRequisitionLine.lineNbr,Equal<Required<POLine.rQReqLineNbr>>>>>
                .Select(this,pOline.RQReqNbr,pOline.RQReqLineNbr);
        }

        #endregion

    }
}