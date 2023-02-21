using System;
using System.Collections;
using System.Collections.Generic;
using Kedge.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.RQ;
using PX.Objects.CR;
using PX.Objects.CN.Subcontracts.SC.Graphs;

namespace Kedge
{
    public class KGRequisitionsUploadTempEntry : PXGraph<KGRequisitionsUploadTempEntry>, PXImportAttribute.IPXPrepareItems
    {
        #region View
        [PXImport(typeof(KGRequisitionsUploadTemp))]
        public PXSelectOrderBy<KGRequisitionsUploadTemp,
            OrderBy<Desc<KGRequisitionsUploadTemp.refNbr>>> DetailsView;
        #endregion

        #region Action
        public PXSave<KGRequisitionsUploadTemp> Save;
        public PXCancel<KGRequisitionsUploadTemp> Cancel;

        public PXAction<KGRequisitionsUploadTemp> Verification;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "����")]
        protected void verification()
        {
            string refNbr = "";
            int count = 0;
            foreach (KGRequisitionsUploadTemp data in DetailsView.Select())
            {
                if (refNbr != data.RefNbr)
                {
                    refNbr = data.RefNbr;
                    count = getByRefNbr(data).Count;
                }
                data.CountByRefNbr = count;
                doVerificationData(data);
                DetailsView.Update(data);
            }
            base.Persist();
            foreach (KGRequisitionsUploadTemp data in DetailsView.Select())
            {
                checkQtyAmt(data);
            }
            base.Persist();

        }
        #endregion

        #region Event

        public void KGRequisitionsUploadTemp_OrderQty_FieldUpdated(PXCache cash, PXFieldUpdatedEventArgs e)
        {
            KGRequisitionsUploadTemp data = (KGRequisitionsUploadTemp)e.Row;
            setSubTotal(data);
        }
        public void KGRequisitionsUploadTemp_UnitCost_FieldUpdated(PXCache cash, PXFieldUpdatedEventArgs e)
        {
            KGRequisitionsUploadTemp data = (KGRequisitionsUploadTemp)e.Row;
            setSubTotal(data);
        }


        #region Import
        bool preImport = true;
        public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            if (preImport)
            {
                foreach (KGRequisitionsUploadTemp item in DetailsView.Select())
                {
                    DetailsView.Delete(item);
                }
                preImport = false;
            }
            setUom(values);
            setPaymentCalculateMethod(values);

            return true;
        }

        public bool RowImporting(string viewName, object row)
        {
            return true;
        }

        public bool RowImported(string viewName, object row, object oldRow)
        {
            return true;
        }

        public void PrepareItems(string viewName, IEnumerable items)
        {
        }
        #endregion



        #endregion

        #region Method
        private void setUom(IDictionary values)
        {
            String key = "Uom";
            String value = (String)values[key];
            if (value == null) return;
            if (value == "�T") { values[key] = "M2"; }
            else { values[key] = value.ToUpper(); }
        }

        private void setPaymentCalculateMethod(IDictionary values)
        {
            String key = "PaymentCalculateMethod";
            String value = (String)values[key];
            if (value == null) return;
            if (value.Trim() == "�ļƶq�p��k") { values[key] = "A"; }
            else if (value.Trim() == "�ħ��u�ʤ��k") { values[key] = "B"; }
        }

        private void setSubTotal(KGRequisitionsUploadTemp data)
        {
            if (data == null) return;
            data.SubTotalCost = (data.OrderQty ?? 0) * (data.UnitCost ?? 0);
        }


        private void doVerificationData(KGRequisitionsUploadTemp data)
        {
            //���o���������w��
            PXResultset<PMCostBudget> costBudgets = getPMCostBudget(data);
            #region ���L���������w��
            if (costBudgets.Count == 0)
            {
                data.ErrorMsg = "�u�ƵL���������w��";
                return;
            }
            #endregion
            #region �O�_�����h���w��
            if (costBudgets.Count > 1)
            {
                data.ErrorMsg = "�u�ƹ����h�������w��(" + costBudgets.Count + ")";
                return;
            }
            PMCostBudget costBudget = costBudgets;
            data.TaskID = costBudget.ProjectTaskID;
            data.CostCodeID = costBudget.CostCodeID;
            data.AccountGroupID = costBudget.AccountGroupID;
            DetailsView.Update(data);

            #endregion
            #region �u�Ƴ��O�_�s�b
            bool hasUom = false;
            foreach (PXResult<InventoryItem, INUnit> item in getInventoryItem(data))
            {
                InventoryItem inventoryItem = (InventoryItem)item;
                INUnit unit = (INUnit)item;
                hasUom = hasUom || data.Uom == inventoryItem.BaseUnit || data.Uom == unit.FromUnit;
            }
            if (!hasUom)
            {
                data.ErrorMsg = "��줣�@�P";
                return;
            }
            #endregion

            #region �ˬd�g��H
            EPEmployee employee = getEPEmployee(data);
            if (employee == null)
            {
                data.ErrorMsg = "�L���g��H";
                return;
            }
            #endregion

            #region �ˬd������
            BAccount vendor = getVendor(data);
            if (vendor == null)
            {
                data.ErrorMsg = "�L��������";
                return;
            }
            #endregion

            data.ErrorMsg = null;

        }

        private void checkQtyAmt(KGRequisitionsUploadTemp data)
        {
            PXResultset<PMCostBudget> costBudgets = getPMCostBudget(data);
            if (costBudgets.Count != 1) return;
            PMCostBudget costBudget = costBudgets;

            #region �ˬd�w����B & �w��ƶq
            decimal? totalQty = 0;
            decimal? totalAmt = 0;
            #region ���o�w�ϥζq
            //�s�P
            if (data.Type == "P")
            {
                POLine line = getPOLine(data);
                if (line != null)
                {
                    totalQty += (line.OrderQty ?? 0);
                    totalAmt += (line.CuryExtCost ?? 0);
                }
            }
            //���]
            else
            {
                RQRequestLine line = getRQRequestLine(data);
                if (line != null)
                {
                    totalQty += (line.OrderQty ?? 0);
                    totalAmt += (line.CuryEstExtCost ?? 0);
                }

            }
            #endregion
            KGRequisitionsUploadTemp sum = getTempSum(data);
            if (sum != null)
            {
                totalQty += (sum.OrderQty ?? 0);
                totalAmt += (sum.SubTotalCost ?? 0);
            }
            //�D�@�����ˮּƶq
            if (!SubcontractEntry_Extension.type.Equals(costBudget.UOM) && (costBudget.RevisedQty ?? 0) < totalQty)
            {
                if (data.ErrorMsg == null) data.ErrorMsg = "�W�L�w��ƶq";
                else data.ErrorMsg += ",�W�L�w��ƶq";
                DetailsView.Update(data);
                return;
            }
            if ((costBudget.CuryRevisedAmount ?? 0) < totalAmt)
            {
                if (data.ErrorMsg == null) data.ErrorMsg = "�W�L�w����B";
                else data.ErrorMsg += ",�W�L�w����B";
                DetailsView.Update(data);
                return;
            }
            #endregion
        }
        #endregion



        #region BQL

        /**
         * <summary>
         * ���o�M�צ����w��
         * </summary>
         * **/
        private PXResultset<PMCostBudget> getPMCostBudget(KGRequisitionsUploadTemp data)
        {
            List<object> param = new List<object>();
            PXSelectBase<PMCostBudget> comm =
               new PXSelect<PMCostBudget,
                Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                    And<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>
                  >>>(this);

            param.Add(data.ProjectID);
            param.Add(data.InventoryID);
            if (data.TaskID != null)
            {
                comm.WhereAnd<Where<PMCostBudget.projectTaskID, Equal<Required<PMCostBudget.projectTaskID>>>>();
                param.Add(data.TaskID);
            }
            if (data.CostCodeID != null)
            {
                comm.WhereAnd<Where<PMCostBudget.costCodeID, Equal<Required<PMCostBudget.costCodeID>>>>();
                param.Add(data.CostCodeID);
            }
            if (data.AccountGroupID != null)
            {
                comm.WhereAnd<Where<PMCostBudget.accountGroupID, Equal<Required<PMCostBudget.accountGroupID>>>>();
                param.Add(data.AccountGroupID);
            }
            object[] objs = param.ToArray();
            return comm.Select(objs);
        }

        /**
         * <summary>
         * �u�ƶ���
         * </summary>
         * **/
        private PXResultset<InventoryItem> getInventoryItem(KGRequisitionsUploadTemp data)
        {
            return PXSelectJoin<InventoryItem,
                LeftJoin<INUnit, On<InventoryItem.inventoryID, Equal<INUnit.inventoryID>>>,
                Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>
                >>.Select(new PXGraph(), data.InventoryID);
        }

        /**
         * <summary>
         * ���o���u
         * </summary>
         * **/
        private PXResultset<EPEmployee> getEPEmployee(KGRequisitionsUploadTemp data)
        {
            return PXSelect<EPEmployee,
                Where<EPEmployee.acctCD, Equal<Required<EPEmployee.acctCD>>,
                And<EPEmployee.status, Equal<EPEmployee.status.active>>
                >>.Select(new PXGraph(), data.Usercd);
        }


        /**
         * <summary>
         * ���o�w�ϥζq-���]<br/>
         * QrderQty - �q��ƶq<br/>
         * CuryEstExtCost - �����p�p<br/>
         * </summary>
         * **/
        private PXResultset<RQRequestLine> getRQRequestLine(KGRequisitionsUploadTemp data)
        {
            return PXSelectGroupBy<RQRequestLine,
                    Where<RQRequestLineExt.usrContractID, Equal<Required<RQRequestLineExt.usrContractID>>,
                        And<RQRequestLineExt.usrProjectTaskID, Equal<Required<RQRequestLineExt.usrProjectTaskID>>,
                        And<RQRequestLineExt.usrCostCodeID, Equal<Required<RQRequestLineExt.usrCostCodeID>>,
                        And<RQRequestLine.inventoryID, Equal<Required<RQRequestLine.inventoryID>>,
                        And<RQRequestLineExt.usrAccountGroupID, Equal<Required<RQRequestLineExt.usrAccountGroupID>>>>>>>,
                     Aggregate<Sum<RQRequestLine.orderQty, Sum<RQRequestLine.curyEstExtCost>>>
                    >.Select(new PXGraph(), data.ProjectID, data.CostCodeID, data.TaskID, data.InventoryID, data.AccountGroupID);
        }

        /**
         * <summary>
         * ���o�w�ϥζq-�s�P<br/>
         * orderQty - �q��ƶq<br/>
         * curyExtCost - �����p�p<br/>
         * </summary>
         * **/
        private PXResultset<POLine> getPOLine(KGRequisitionsUploadTemp data)
        {
            return PXSelectGroupBy<POLine,
                    Where<POLine.projectID, Equal<Required<POLine.projectID>>,
                        And<POLine.taskID, Equal<Required<POLine.taskID>>,
                        And<POLine.costCodeID, Equal<Required<POLine.costCodeID>>,
                        And<POLine.inventoryID, Equal<Required<POLine.inventoryID>>,
                        And<POLineContractExt.usrAccountGroupID, Equal<Required<POLineContractExt.usrAccountGroupID>>,
                        And<POLine.orderType, Equal<Required<POLine.orderType>>>>>>>>,
                     Aggregate<Sum<POLine.orderQty, Sum<POLine.curyExtCost>>>
                    >.Select(new PXGraph(), data.ProjectID, data.TaskID, data.CostCodeID, data.InventoryID, data.AccountGroupID, "RS");
        }

        /**
         * <summary>
         * ���o��e�ζq<br/>
         * orderQty - �q��ƶq<br/>
         * subTotalCost - �����p�p<br/>
         * </summary>
         * **/
        private PXResultset<KGRequisitionsUploadTemp> getTempSum(KGRequisitionsUploadTemp data)
        {
            return PXSelectGroupBy<KGRequisitionsUploadTemp,
                    Where<KGRequisitionsUploadTemp.projectID, Equal<Required<KGRequisitionsUploadTemp.projectID>>,
                        And<KGRequisitionsUploadTemp.taskID, Equal<Required<KGRequisitionsUploadTemp.taskID>>,
                        And<KGRequisitionsUploadTemp.costCodeID, Equal<Required<KGRequisitionsUploadTemp.costCodeID>>,
                        And<KGRequisitionsUploadTemp.inventoryID, Equal<Required<KGRequisitionsUploadTemp.inventoryID>>,
                        And<KGRequisitionsUploadTemp.accountGroupID, Equal<Required<KGRequisitionsUploadTemp.accountGroupID>>,
                        And<KGRequisitionsUploadTemp.refNbr, Like<Required<KGRequisitionsUploadTemp.refNbr>>>>>>>>,
                     Aggregate<Sum<KGRequisitionsUploadTemp.orderQty, Sum<KGRequisitionsUploadTemp.subTotalCost>>>
                    >.Select(this, data.ProjectID, data.TaskID, data.CostCodeID, data.InventoryID, data.AccountGroupID, data.RefNbr.Substring(0, 1) + "%");
        }


        private PXResultset<KGRequisitionsUploadTemp> getByRefNbr(KGRequisitionsUploadTemp data)
        {
            return PXSelect<KGRequisitionsUploadTemp,
                    Where<KGRequisitionsUploadTemp.refNbr, Equal<Required<KGRequisitionsUploadTemp.refNbr>>>
                    >.Select(new PXGraph(), data.RefNbr);
        }

        private PXResultset<BAccount> getVendor(KGRequisitionsUploadTemp data)
        {
            return PXSelect<BAccount,
                   Where<BAccount.status, Equal<BAccount.status.active>,
                   And<BAccount.type, Equal<BAccountType.vendorType>,
                   And<BAccount.acctCD, Equal<Required<BAccount.acctCD>>>>>
                   >.Select(new PXGraph(), data.VendorCD); ;
        }

        #endregion


    }
}