using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.EP;
using PX.Objects.GL;
using static RCGV.GV.Util.GVList;
using GVList = RCGV.GV.Util.GVList;
using RCGV.GV.Attribute.Selector;
using RCGV.GV.DAC;
using RC.Util;

namespace RCGV.GV
{
    public class GVMediaDeclaration : PXGraph<GVMediaDeclaration>
    {
        #region Views
        public PXFilter<GVARAPInvFilter> ARAPINVfilter;
        public PXSelect<GVDataView> DataViews;
        #endregion

        #region Delegate Date Views
        public virtual IEnumerable dataViews()
        {
            ///<remarks>Mantis [0012315] #3</remarks>
            GVARAPInvFilter filter = ARAPINVfilter.Current;

            List<GVDataView> lists = DataViews.Cache.Cached.RowCast<GVDataView>().ToList();

            if (filter?.DeclareBatchNbr != null)
            {
                lists.RemoveAll(r => r.DeclareBatchNbr != filter.DeclareBatchNbr);
            }

            for (int i = 0; i < lists.Count; i++)
            {
                yield return lists[i];
            }
        }
        #endregion

        #region Actions

        #region SearchData
        public PXAction<GVARAPInvFilter> SearchData;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "���o���")]
        protected IEnumerable searchData(PXAdapter adapter)
        {
            GVARAPInvFilter filter = ARAPINVfilter.Current;
            //if (filter == null) return adapter.Get();
            bool isAP = In(filter.GvType, "I", "IO");
            bool isAR = In(filter.GvType, "O", "IO");
            int seq = 1;
            DataViews.Cache.AllowInsert = true;
            DataViews.Cache.Clear();
            //PXResultset<GVDataView> rs = new PXResultset<GVDataView>();
            if (isAP)
            {
                //----AP Invoice
                foreach (GVApGuiInvoice item in GetAPGuiInvoice(filter))
                {
                    //rs.Add(new PXResult<GVDataView>(GetData(ref seq, item)));
                    DataViews.Insert(GetData(ref seq, item));
                }
                //----AP Allowance
                foreach (GVApGuiCmInvoice item in GetAPGuiCMInvoice(filter))
                {
                    foreach (GVApGuiCmInvoiceLine line in GetAPGuiCMInvoiceLine(item.GuiCmInvoiceID))
                    {
                        //rs.Add(new PXResult<GVDataView>(GetData(ref seq, item, line)));
                        DataViews.Insert(GetData(ref seq, item, line));
                    }
                }
            }
            if (isAR)
            {
                //----AR Invoice
                foreach (GVArGuiInvoice item in GetARGuiInvoice(filter))
                {
                    //rs.Add(new PXResult<GVDataView>(GetData(ref seq, item)));
                    DataViews.Insert(GetData(ref seq, item));
                }
                //----AR Allowance
                foreach (GVArGuiCmInvoice item in GetARGuiCMInvoice(filter))
                {
                    foreach (GVArGuiCmInvoiceLine line in GetARGuiCMInvoiceLine(item.GuiCmInvoiceID))
                    {
                        //rs.Add(new PXResult<GVDataView>(GetData(ref seq, item, line)));
                        DataViews.Insert(GetData(ref seq, item, line));
                    }
                }
            }
            //filter.Datas = rs;
            return adapter.Get();
        }
        #endregion

        #region ExportTxt
        public PXAction<GVARAPInvFilter> ExportTxt;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "���ʹC��ӳ�")]
        protected IEnumerable exportTxt(PXAdapter adapter)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                GVARAPInvFilter filter = ARAPINVfilter.Current;
                GVRegistration gVRegistration = GetGVRegistration(filter.RegistrationCD);
                string declareBatchNbr = DateTime.Now.ToString("yyyyMMddHHmmss");
                //�ɦW�G�ӳ��H�νs
                RCEleTxtFileUtil util = new RCEleTxtFileUtil(gVRegistration.GovUniformNumber + "_" + declareBatchNbr, "BIG5");
                int index = 0;
                foreach (GVDataView data in DataViews.Select())
                {
                    if (data.Selected != true) continue;
                    index++;
                    //�J�}
                    bool isSummary = data.GroupRemark == GVGuiGroupRemark.SUMMARY;
                    //�䲼
                    bool isInvoice = data.VoucherCategory == GVGuiVoucherCategory.TAXABLE;
                    #region 1-2(2)
                    String str1_2 = "";
                    //�榡�N��(2)�GGuiType
                    String GuiType = RCEleTxtFileUtil.GetDataStrByByte(data.GuiType, 2, false, util.ENCODING);
                    str1_2 = GuiType;
                    #endregion
                    #region 3-11(9)
                    //�ӳ��H�|�y�s��(9)�GRegistrationCD
                    String RegistrationCD = RCEleTxtFileUtil.GetDataStrByByte(filter.RegistrationCD, 9, false, util.ENCODING);
                    String str3_11 = RegistrationCD;
                    #endregion
                    #region 12-18(7)
                    //�y����(7)�G�s��s������0
                    String Seq = RCEleTxtFileUtil.GetDataStrByLenght(index.ToString(), 7, true, '0');
                    String str12_18 = Seq;
                    #endregion
                    #region 19-21(3)
                    //���ݦ~��(3)�GDeclareYear(�������~)����0
                    int _DeclareYear = (data.DeclareYear ?? 0) - 1911;
                    String DeclareYear = RCEleTxtFileUtil.GetDataStrByLenght(_DeclareYear.ToString(), 3, true, '0');
                    String str19_21 = DeclareYear;
                    #endregion
                    #region 22-23(2)
                    //���ݤ��(2)�GDeclareMonth����0
                    String DeclareMonth = RCEleTxtFileUtil.GetDataStrByLenght(data.DeclareMonth.ToString(), 2, true, '0');
                    String str22_23 = DeclareMonth;
                    #endregion
                    #region 24-31(8)
                    String str24_31 = "";
                    if (isSummary)
                    {
                        //�o������(8)�G�J�}�o������
                        String InvoiceNbrEnd = GetInvoiceNbrEnd(data);
                        str24_31 = InvoiceNbrEnd;
                    }
                    else
                    {
                        //�R���H�νs(8)�GCustUniformNumber�A�i����ӳ��H�νsGVRegistration.GovUniformNumber
                        String BuyerID = RCEleTxtFileUtil.GetDataStrByByte(data.BuyerID, 8, false, util.ENCODING);
                        str24_31 = BuyerID;
                    }
                    #endregion
                    #region 32-39(8)
                    String str32_39 = "";
                    if (isSummary)
                    {
                        //�J�`�i��(4)�G�J�}GroupCnt�B�D�J�}�νs�e�|�X(���ک�ť�)
                        //�ť�(4)
                        String GroupCnt = data.GroupRemark == GVList.GVGuiGroupRemark.SUMMARY ? data.GroupCnt?.ToString() : data.BuyerID.Substring(0, 4);
                        GroupCnt = RCEleTxtFileUtil.GetDataStrByLenght(GroupCnt, 4, true, '0');
                        str32_39 = RCEleTxtFileUtil.GetDataStrByLenght(GroupCnt, 4, false, ' ');//+�|�Ӫť�
                    }
                    else
                    {
                        //�P��H�νs(8)�GVendorUniformNumber�A�P����ӳ��H�νsGVRegistration.GovUniformNumber
                        String SellerID = RCEleTxtFileUtil.GetDataStrByByte(data.SellerID, 8, false, util.ENCODING);
                        str32_39 = SellerID;
                    }
                    #endregion
                    #region 40-49(10)
                    String str40_49 = "";
                    if (isInvoice)
                    {
                        //�o���r�y(2)�GinvoiceNbr�e2�X
                        //�o���_��(8)�GinboiceNbr��8��
                        str40_49 = RCEleTxtFileUtil.GetDataStrByByte(data.GuiInvoiceNbr, 10, false, util.ENCODING);
                    }
                    else
                    {
                        //��L���Ҹ��X(10)|���@�Ʒ~����y����(10)�G�������OVoucherCategory=��L���ҮɡA��GuiInvoiceNbr
                        String OtherRcertificate =
                            data.VoucherCategory == GVList.GVGuiVoucherCategory.OTHERCERTIFICATE ?
                            data.GuiInvoiceNbr : "";
                        OtherRcertificate = RCEleTxtFileUtil.GetDataStrByByte(OtherRcertificate, 10, false, util.ENCODING);
                    }
                    #endregion
                    #region 50-61(12)
                    //�P����B(12)|��~�|�|��(12)�GsalesAmt�A����0
                    String SalesAmt = RCEleTxtFileUtil.GetDataStrByLenght(data.SalesAmt?.ToString("0"), 12, true, '0');
                    String str50_61 = SalesAmt;
                    #endregion
                    #region 62-62(1)
                    //�ҵ|�O(1)�GTaxCode--�@�o�h��F
                    String TaxCode = RCEleTxtFileUtil.GetDataStrByByte(data.TaxCode, 1, false, util.ENCODING);
                    String str62_62 = data.Status == GVStatus.VOIDINVOICE ? "F" : TaxCode;
                    #endregion
                    #region 63-72(10)
                    //��~�|�B(10)�GTaxAmt�A����0
                    String TaxAmt = RCEleTxtFileUtil.GetDataStrByLenght(data.TaxAmt?.ToString("0"), 10, true, '0');
                    String str63_72 = TaxAmt;
                    #endregion
                    #region 73-73(1)
                    //����N��(1)�G�i���o�� DeductionCode
                    String DeductionCode = RCEleTxtFileUtil.GetDataStrByByte(data.DeductionCode, 1, false, util.ENCODING);
                    String str73_73 = DeductionCode;
                    #endregion
                    #region 74-78(5)
                    // �ť�(5)
                    String str74_78 = RCEleTxtFileUtil.GetDataStrByLenght("", 5, true, ' ');
                    #endregion
                    #region 79-79(1)
                    //�S�ص|�v(1)
                    String SpecialTaxType = RCEleTxtFileUtil.GetDataStrByByte(GetSpecialTaxType(data, gVRegistration), 1, false, util.ENCODING);
                    String str79_79 = SpecialTaxType;
                    #endregion
                    #region 80-80(1)
                    String GroupRemark = In(data.GroupRemark, GVList.GVGuiGroupRemark.SUMMARY, GVList.GVGuiGroupRemark.APPORTIONMENT) ? data.GroupRemark : "";
                    String str80_80 = GroupRemark;
                    #endregion
                    #region 81-81(1)
                    String str81_81 = " ";
                    #endregion

                    util.InputLine(
                          str1_2
                        + str3_11
                        + str12_18
                        + str19_21
                        + str22_23
                        + str24_31
                        + str32_39
                        + str40_49
                        + str50_61
                        + str62_62
                        + str63_72
                        + str73_73
                        + str74_78
                        + str79_79
                        + str80_80
                        + str81_81
                        );
                    UpdateBatchNbr(data, declareBatchNbr);
                }
                UpdateMediaDeclarationTemp(declareBatchNbr);
                this.DataViews.View.RequestRefresh();

                ts.Complete();
                util.Dowload();
            }
            return adapter.Get();
        }
        #endregion

        #region Report401
        public PXAction<GVARAPInvFilter> report401;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "401 ����", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
        protected virtual void Report401()
        {
            OpenGVReport(true);
        }
        #endregion

        #region Report403
        public PXAction<GVARAPInvFilter> report403;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "403 ����", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
        protected virtual void Report403()
        {
            OpenGVReport(false);
        }
        #endregion

        #region  toggleSource
        public PXAction<GVARAPInvFilter> ToggleSource;
        [PXButton()]
        [PXUIField(DisplayName = "����/��������", MapEnableRights = PXCacheRights.Select)]
        public IEnumerable toggleSource(PXAdapter adapter)
        {
            bool hasSelected = !DataViews.View.SelectMulti().ToList().RowCast<GVDataView>().First().Selected.GetValueOrDefault();

            foreach (GVDataView detail in DataViews.View.SelectMulti().RowCast<GVDataView>())
            {
                detail.Selected = hasSelected;

                DataViews.Cache.Update(detail);
            }

            return adapter.Get();
        }
        #endregion

        #endregion

        #region HyperLink
        #region ViewGuiInvoice
        public PXAction<GVDataView> ViewGuiInvoice;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGuiInvoice()
        {
            GVDataView row = DataViews.Current;
            if (row == null) return;
            if (row.GvType == GVARAPInvFilter.I)
            {
                new HyperLinkUtil<GVApInvoiceMaint>(GetGVAPInvoiceByID(row.ID), true);
            }
            else
            {
                new HyperLinkUtil<GVArInvoiceEntry>(GetGVARInvoiceByID(row.ID), true);
            }

        }
        #endregion
        #region ViewAllowance
        public PXAction<GVDataView> ViewAllowance;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAllowance()
        {
            GVDataView row = DataViews.Current;
            if (row == null) return;
            if (row.GvType == GVARAPInvFilter.I)
            {
                new HyperLinkUtil<GVApCmInvoiceEntry>(GetGVAPCmInvoiceByID(row.ID), true);
            }
            else
            {
                new HyperLinkUtil<GVArGuiCmInvoiceMaint>(GetGVARCmInvoiceByID(row.ID), true);
            }
        }
        #endregion

        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<GVARAPInvFilter> e)
        {
            if (e.Row == null) return;
            PXUIFieldAttribute.SetReadOnly(DataViews.Cache, null, true);
            PXUIFieldAttribute.SetReadOnly<GVDataView.selected>(DataViews.Cache, null, false);
            DataViews.Cache.AllowDelete = DataViews.Cache.AllowInsert = false;

            ///<remarks>Mantis [0012315] #4</remarks>
            //bool hasDeclBatchNbr = !string.IsNullOrEmpty(e.Row.DeclareBatchNbr);

            //report401.SetEnabled(hasDeclBatchNbr);
            //report403.SetEnabled(hasDeclBatchNbr);
        }

        protected virtual void _(Events.FieldDefaulting<GVARAPInvFilter, GVARAPInvFilter.period> e)
        {
            if (e.Row == null) return;
            int? month = this.Accessinfo.BusinessDate?.Month;
            e.NewValue = month + (month % 2);
        }

        protected virtual void _(Events.FieldUpdated<GVARAPInvFilter, GVARAPInvFilter.period> e)
        {
            if (e.Row == null) return;
            e.Cache.SetDefaultExt<GVARAPInvFilter.declareMonthFrom>(e.Row);
            e.Cache.SetDefaultExt<GVARAPInvFilter.declareMonthTo>(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<GVARAPInvFilter, GVARAPInvFilter.declareMonthFrom> e)
        {
            if (e.Row == null) return;
            e.NewValue = e.Row.Period - 1;
        }

        protected virtual void _(Events.FieldDefaulting<GVARAPInvFilter, GVARAPInvFilter.declareMonthTo> e)
        {
            if (e.Row == null) return;
            e.NewValue = e.Row.Period;
        }

        protected void _(Events.FieldUpdated<GVDataView, GVDataView.selected> e)
        {
            if (e.Row == null) return;
            var selected = e.NewValue;
            GVDataView row = e.Row;
        }
        #endregion

        #region Methods
        /// <summary>
        /// ��sdeclareBatchNbr, declaredByID & declaredDateTime
        /// Mantis [0012315] #5
        /// </summary>
        /// <param name="data"></param>
        /// <param name="declareBatchNbr"></param>
        public virtual void UpdateBatchNbr(GVDataView data, string declareBatchNbr)
        {
            switch (data.Type)
            {
                case GVDataView.ViewType.AP:
                    PXUpdate<Set<GVApGuiInvoice.declareBatchNbr, Required<GVApGuiInvoice.declareBatchNbr>,
                                 Set<GVApGuiInvoice.declaredByID, Required<GVApGuiInvoice.declaredByID>,
                                     Set<GVApGuiInvoice.declaredDateTime, Required<GVApGuiInvoice.declaredDateTime>>>>,
                             GVApGuiInvoice,
                             Where<GVApGuiInvoice.guiInvoiceID, Equal<Required<GVApGuiInvoice.guiInvoiceID>>>>
                             .Update(this, declareBatchNbr, this.Accessinfo.UserID, DateTime.Now, data.ID);
                    break;
                case GVDataView.ViewType.AR:
                    PXUpdate<Set<GVArGuiInvoice.declareBatchNbr, Required<GVArGuiInvoice.declareBatchNbr>,
                                 Set<GVArGuiInvoice.declaredByID, Required<GVArGuiInvoice.declaredByID>,
                                     Set<GVArGuiInvoice.declaredDateTime, Required<GVArGuiInvoice.declaredDateTime>>>>,
                             GVArGuiInvoice,
                             Where<GVArGuiInvoice.guiInvoiceID, Equal<Required<GVArGuiInvoice.guiInvoiceID>>>>
                             .Update(this, declareBatchNbr, this.Accessinfo.UserID, DateTime.Now, data.ID);
                    break;
                case GVDataView.ViewType.APCM:
                    PXUpdate<Set<GVApGuiCmInvoice.declareBatchNbr, Required<GVApGuiCmInvoice.declareBatchNbr>,
                                 Set<GVApGuiCmInvoice.declaredByID, Required<GVApGuiCmInvoice.declaredByID>,
                                     Set<GVApGuiCmInvoice.declaredDateTime, Required<GVApGuiCmInvoice.declaredDateTime>>>>,
                             GVApGuiCmInvoice,
                             Where<GVApGuiCmInvoice.guiCmInvoiceID, Equal<Required<GVApGuiCmInvoice.guiCmInvoiceID>>>>
                             .Update(this, declareBatchNbr, this.Accessinfo.UserID, DateTime.Now, data.ID);
                    break;
                case GVDataView.ViewType.ARCM:
                    PXUpdate<Set<GVArGuiCmInvoice.declareBatchNbr, Required<GVArGuiCmInvoice.declareBatchNbr>,
                                 Set<GVArGuiCmInvoice.declaredByID, Required<GVArGuiCmInvoice.declaredByID>,
                                     Set<GVArGuiCmInvoice.declaredDateTime, Required<GVArGuiCmInvoice.declaredDateTime>>>>,
                             GVArGuiCmInvoice,
                             Where<GVArGuiCmInvoice.guiCmInvoiceID, Equal<Required<GVArGuiCmInvoice.guiCmInvoiceID>>>>
                             .Update(this, declareBatchNbr, this.Accessinfo.UserID, DateTime.Now, data.ID);
                    break;
            }
        }

        /// <summary>
        /// ���o�S��|��
        /// </summary>
        /// <param name="gVRegistration"></param>
        /// <returns></returns>
        public virtual String GetSpecialTaxType(GVDataView data, GVRegistration gVRegistration)
        {
            //�P�� �o��
            if (data.GuiCmInvoiceNbr == null && data.GvType == GVARAPInvFilter.O)
            {
                return gVRegistration.SpecialTaxType == GVSpecialTaxType.SpecialTaxType_0 ? "" : gVRegistration.SpecialTaxType;
            }
            return "";
        }

        /// <summary>
        /// ���o�J�}�o������
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual String GetInvoiceNbrEnd(GVDataView data)
        {
            String invoiceNbr = data.GuiInvoiceNbr.Substring(2, 8);
            int endNbr = int.Parse(invoiceNbr) + (data.GroupCnt ?? 0) - 1;
            return RCEleTxtFileUtil.GetDataStrByLenght(endNbr.ToString(), 8, true, '0');
        }

        /// <summary>
        /// ���o�o����8�X
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual String GetInvoiceNbr(GVDataView data)
        {
            //�i��+�D�o�� �^�Ǫ�
            if (data.GvType == GVARAPInvFilter.I && data.VoucherCategory != GVList.GVGuiVoucherCategory.TAXABLE)
            {
                return RCEleTxtFileUtil.GetDataStrByLenght("", 8, false, ' ');
            }
            return RCEleTxtFileUtil.GetDataStrByLenght(data.GuiInvoiceNbr.Substring(2, 8), 8, false, ' ');
        }

        /// <summary>
        /// ���o�r�y2
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual String GetWord(GVDataView data)
        {
            //�i��+�D�o�� �^�Ǫ�
            if (data.GvType == GVARAPInvFilter.I && data.VoucherCategory != GVList.GVGuiVoucherCategory.TAXABLE)
            {
                return RCEleTxtFileUtil.GetDataStrByLenght("", 2, false, ' ');
            }
            return RCEleTxtFileUtil.GetDataStrByLenght(data.GuiInvoiceNbr.Substring(0, 2), 2, false, ' ');
        }

        /// <summary>
        /// ///<remarks>Mantis [0012315] #1</remarks>
        /// </summary>
        private void OpenGVReport(bool is401Rpt)
        {
            const string GVReport401 = "GV603001";
            const string GVReport403 = "GV603002";
            var currentFilter = this.ARAPINVfilter.Current;

            string declareBatchNbr = GetMaxDeclareBatchNbr(currentFilter);
            if (declareBatchNbr == null) return;
            int? declareYear = GetYearByDeclareBatchNbr(declareBatchNbr);
            //���̫�@�����ӳ�
            throw new PXReportRequiredException(new Dictionary<string, string>()
            {
                ["RegistrationCD"] = currentFilter.RegistrationCD,
                ["DeclareYear"] = declareYear.ToString(),
                ["DeclareStartMonth"] = "1",
                ["DeclareEndMonth"] = "12",
                ["DeclareBatchNbr"] = declareBatchNbr
            },
                  is401Rpt == true ? GVReport401 : GVReport403);
        }

        #region GetData
        public virtual GVDataView GetData(ref int seq, GVApGuiInvoice item)
        {
            GVDataView data = new GVDataView();
            data.Type = GVDataView.ViewType.AP;
            data.ID = item.GuiInvoiceID;
            data.Seq = seq++;
            data.GvType = "I";
            data.Status = item.Status;
            data.GuiInvoiceNbr = item.GuiInvoiceNbr;
            data.InvoiceDate = item.InvoiceDate;
            data.DeclareYear = item.DeclareYear;
            data.DeclareMonth = item.DeclareMonth;
            data.GuiType = item.GuiType;
            data.InvoiceDesc = item.Remark;
            data.SalesAmt = item.SalesAmt;
            data.TaxAmt = item.TaxAmt;
            data.TotalAmt = item.TotalAmt;
            data.GroupRemark = item.GroupRemark;
            data.DeductionCode = item.DeductionCode;
            data.TaxCode = item.TaxCode;
            data.VoucherCategory = item.VoucherCategory;
            data.BuyerID = GetGVRegistration(item.RegistrationCD).GovUniformNumber;
            data.SellerID = item.VendorUniformNumber;
            data.GroupCnt = item.GroupCnt;
            data.DeclareBatchNbr = item.DeclareBatchNbr;
            data.DeclaredByID = item.DeclaredByID;
            data.DeclaredDateTime = item.DeclaredDateTime;
            return data;
        }

        public virtual GVDataView GetData(ref int seq, GVApGuiCmInvoice item, GVApGuiCmInvoiceLine line)
        {
            GVDataView data = new GVDataView();
            data.Type = GVDataView.ViewType.APCM;
            data.ID = item.GuiCmInvoiceID;
            data.Seq = seq++;
            data.GvType = "I";
            data.Status = item.Status;
            data.GuiInvoiceNbr = line.ApGuiInvoiceNbr;
            data.GuiCmInvoiceNbr = item.GuiCmInvoiceNbr;
            data.InvoiceDate = item.InvoiceDate;
            data.DeclareYear = item.DeclareYear;
            data.DeclareMonth = item.DeclareMonth;
            data.GuiType = item.GuiType;
            data.InvoiceDesc = item.Remark;
            data.SalesAmt = line.SalesAmt;
            data.TaxAmt = line.TaxAmt;
            data.TotalAmt = (line.SalesAmt ?? 0m) + (line.TaxAmt ?? 0m);
            data.GroupRemark = GVGuiGroupRemark.UNSUMMARY;
            data.TaxCode = item.TaxCode;
            data.VoucherCategory = GVList.GVGuiVoucherCategory.TAXABLE;
            data.BuyerID = GetGVRegistration(item.RegistrationCD).GovUniformNumber;
            data.SellerID = item.VendorUniformNumber;
            data.DeclareBatchNbr = item.DeclareBatchNbr;
            data.DeclaredByID = item.DeclaredByID;
            data.DeclaredDateTime = item.DeclaredDateTime;
            return data;
        }

        public virtual GVDataView GetData(ref int seq, GVArGuiInvoice item)
        {
            GVDataView data = new GVDataView();
            data.Type = GVDataView.ViewType.AR;
            data.ID = item.GuiInvoiceID;
            data.Seq = seq++;
            data.GvType = "O";
            data.Status = item.Status;
            data.GuiInvoiceNbr = item.GuiInvoiceNbr;
            data.InvoiceDate = item.InvoiceDate;
            data.DeclareYear = item.DeclareYear;
            data.DeclareMonth = item.DeclareMonth;
            data.GuiType = item.GuiType;
            data.InvoiceDesc = item.Remark;
            data.SalesAmt = item.SalesAmt;
            data.TaxAmt = item.TaxAmt;
            data.TotalAmt = item.TotalAmt;
            data.GroupRemark = GVGuiGroupRemark.UNSUMMARY;
            data.TaxCode = item.TaxCode;
            data.VoucherCategory = item.InvoiceType == GVList.GVGuiInvoiceType.INVOICE ? GVList.GVGuiVoucherCategory.TAXABLE : GVList.GVGuiVoucherCategory.OTHERCERTIFICATE;
            data.BuyerID = item.CustUniformNumber;
            data.SellerID = GetGVRegistration(item.RegistrationCD).GovUniformNumber;
            data.DeclareBatchNbr = item.DeclareBatchNbr;
            data.DeclaredByID = item.DeclaredByID;
            data.DeclaredDateTime = item.DeclaredDateTime;
            return data;
        }

        public virtual GVDataView GetData(ref int seq, GVArGuiCmInvoice item, GVArGuiCmInvoiceLine line)
        {
            GVDataView data = new GVDataView();
            data.Type = GVDataView.ViewType.ARCM;
            data.ID = item.GuiCmInvoiceID;
            data.Seq = seq++;
            data.GvType = "O";
            data.Status = item.Status;
            data.GuiInvoiceNbr = line.ArGuiInvoiceNbr;
            data.GuiCmInvoiceNbr = item.GuiCmInvoiceNbr;
            data.InvoiceDate = item.InvoiceDate;
            data.DeclareYear = item.DeclareYear;
            data.DeclareMonth = item.DeclareMonth;
            data.GuiType = item.GuiType;
            data.InvoiceDesc = item.Remark;
            data.SalesAmt = line.SalesAmt;
            data.TaxAmt = line.TaxAmt;
            data.TotalAmt = (line.SalesAmt ?? 0m) + (line.TaxAmt ?? 0m);
            data.GroupRemark = GVGuiGroupRemark.UNSUMMARY;
            data.TaxCode = item.TaxCode;
            data.VoucherCategory = GVList.GVGuiVoucherCategory.TAXABLE;
            data.BuyerID = item.CustUniformNumber;
            data.SellerID = GetGVRegistration(item.RegistrationCD).GovUniformNumber;
            data.DeclareBatchNbr = item.DeclareBatchNbr;
            data.DeclaredByID = item.DeclaredByID;
            data.DeclaredDateTime = item.DeclaredDateTime;
            return data;
        }
        #endregion

        public bool In(object baseVal, params object[] paramVals)
        {
            bool flag = false;
            foreach (object paramVal in paramVals)
            {
                flag = flag || (paramVal.Equals(baseVal));
            }
            return flag;
        }

        public virtual void UpdateMediaDeclarationTemp(string declareBatchNbr)
        {
            var filter = this.ARAPINVfilter.Current;
            GVMediaDeclarationTemp temp = GVMediaDeclarationTemp.PK.Find(this, filter.DeclareYearFrom, filter.Period, filter.RegistrationCD);
            if (temp == null)
            {
                this.Caches<GVMediaDeclarationTemp>().Insert(
                    new GVMediaDeclarationTemp()
                    {
                        DeclareYear = filter.DeclareYearFrom,
                        Period = filter.Period,
                        RegistrationCD = filter.RegistrationCD,
                        DeclareBatchNbr = declareBatchNbr
                    });
                this.Caches<GVMediaDeclarationTemp>().Persist(PXDBOperation.Insert);
            }
            else
            {
                temp.DeclareBatchNbr = declareBatchNbr;
                this.Caches<GVMediaDeclarationTemp>().Update(temp);
                this.Caches<GVMediaDeclarationTemp>().Persist(PXDBOperation.Update);
            }
            this.Caches<GVMediaDeclarationTemp>().Clear();
        }

        #endregion

        #region BQLs
        public virtual string GetMaxDeclareBatchNbr(GVARAPInvFilter filter)
        {
            APARGui401DataV data = PXSelectGroupBy<APARGui401DataV,
                                    Where<APARGui401DataV.registrationCD, Equal<Required<APARGui401DataV.registrationCD>>//registrationCD
                                   >,
                                    Aggregate<Max<APARGui401DataV.declareBatchNbr>>>
                                   .Select(this, filter.RegistrationCD);
            return data?.DeclareBatchNbr;
        }

        public virtual int? GetYearByDeclareBatchNbr(string declareBatchNbr)
        {
            APARGui401DataV data = PXSelect<APARGui401DataV,
                                    Where<APARGui401DataV.declareBatchNbr, Equal<Required<APARGui401DataV.declareBatchNbr>>//registrationCD
                                   >>
                                   .Select(this, declareBatchNbr);
            return data?.DeclareYear;
        }

        public virtual GVRegistration GetGVRegistration(string registrationCD)
        {
            return PXSelect<GVRegistration,
                Where<GVRegistration.registrationCD, Equal<Required<GVRegistration.registrationCD>>>>
                .Select(this, registrationCD);
        }

        public virtual PXResultset<GVApGuiInvoice> GetAPGuiInvoice(GVARAPInvFilter filter)
        {
            ///<remarks>Because of the potential NULL cache problem with the original graph.</remarks>
            return PXSelect<GVApGuiInvoice, Where<GVApGuiInvoice.registrationCD, Equal<Required<GVApGuiInvoice.registrationCD>>,//registrationCD
                                                  And<GVApGuiInvoice.declareYear, GreaterEqual<Required<GVApGuiInvoice.declareYear>>,//declareYearFrom
                                                      And<GVApGuiInvoice.declareYear, LessEqual<Required<GVApGuiInvoice.declareYear>>,//declareYearTo
                                                          And<GVApGuiInvoice.declareMonth, GreaterEqual<Required<GVApGuiInvoice.declareMonth>>,//declareMonthFrom
                                                              And<GVApGuiInvoice.declareMonth, LessEqual<Required<GVApGuiInvoice.declareMonth>>,//declareMonthTo
                                                                                                                                                //And<GVApGuiInvoice.invoiceType, Equal<GVGuiInvoiceType.invoice>,
                                                                  And<GVApGuiInvoice.status, Equal<GVStatus.open>>>>>>>>
                                            .Select(/*this*/new PXGraph(), filter.RegistrationCD, filter.DeclareYearFrom, filter.DeclareYearTo, filter.DeclareMonthFrom, filter.DeclareMonthTo);
        }

        public virtual PXResultset<GVArGuiInvoice> GetARGuiInvoice(GVARAPInvFilter filter)
        {
            return PXSelect<GVArGuiInvoice, Where<GVArGuiInvoice.registrationCD, Equal<Required<GVArGuiInvoice.registrationCD>>,//registrationCD
                And<GVArGuiInvoice.declareYear, GreaterEqual<Required<GVArGuiInvoice.declareYear>>,//declareYearFrom
                And<GVArGuiInvoice.declareYear, LessEqual<Required<GVArGuiInvoice.declareYear>>,//declareYearTo
                And<GVArGuiInvoice.declareMonth, GreaterEqual<Required<GVArGuiInvoice.declareMonth>>,//declareMonthFrom
                And<GVArGuiInvoice.declareMonth, LessEqual<Required<GVArGuiInvoice.declareMonth>>,//declareMonthTo
                                                                                                  //And<GVArGuiInvoice.invoiceType, Equal<GVGuiInvoiceType.invoice>,
                And<GVArGuiInvoice.status, In3<GVStatus.open, GVStatus.voidinvoice>>>
                >>>>
                >.Select(this, filter.RegistrationCD, filter.DeclareYearFrom, filter.DeclareYearTo, filter.DeclareMonthFrom, filter.DeclareMonthTo);
        }

        public virtual PXResultset<GVApGuiCmInvoice> GetAPGuiCMInvoice(GVARAPInvFilter filter)
        {
            return PXSelect<GVApGuiCmInvoice, Where<GVApGuiCmInvoice.registrationCD, Equal<Required<GVApGuiCmInvoice.registrationCD>>,//registrationCD
                And<GVApGuiCmInvoice.declareYear, GreaterEqual<Required<GVApGuiCmInvoice.declareYear>>,//declareYearFrom
                And<GVApGuiCmInvoice.declareYear, LessEqual<Required<GVApGuiCmInvoice.declareYear>>,//declareYearTo
                And<GVApGuiCmInvoice.declareMonth, GreaterEqual<Required<GVApGuiCmInvoice.declareMonth>>,//declareMonthFrom
                And<GVApGuiCmInvoice.declareMonth, LessEqual<Required<GVApGuiCmInvoice.declareMonth>>,//declareMonthTo
                And<GVApGuiCmInvoice.status, Equal<GVStatus.open>>>
                >>>>
                >.Select(this, filter.RegistrationCD, filter.DeclareYearFrom, filter.DeclareYearTo, filter.DeclareMonthFrom, filter.DeclareMonthTo);
        }

        public virtual PXResultset<GVArGuiCmInvoice> GetARGuiCMInvoice(GVARAPInvFilter filter)
        {
            return PXSelect<GVArGuiCmInvoice, Where<GVArGuiCmInvoice.registrationCD, Equal<Required<GVArGuiCmInvoice.registrationCD>>,//registrationCD
                And<GVArGuiCmInvoice.declareYear, GreaterEqual<Required<GVArGuiCmInvoice.declareYear>>,//declareYearFrom
                And<GVArGuiCmInvoice.declareYear, LessEqual<Required<GVArGuiCmInvoice.declareYear>>,//declareYearTo
                And<GVArGuiCmInvoice.declareMonth, GreaterEqual<Required<GVArGuiCmInvoice.declareMonth>>,//declareMonthFrom
                And<GVArGuiCmInvoice.declareMonth, LessEqual<Required<GVArGuiCmInvoice.declareMonth>>,//declareMonthTo
                And<GVArGuiCmInvoice.status, Equal<GVStatus.open>>>
                >>>>
                >.Select(this, filter.RegistrationCD, filter.DeclareYearFrom, filter.DeclareYearTo, filter.DeclareMonthFrom, filter.DeclareMonthTo);
        }

        public virtual PXResultset<GVApGuiCmInvoiceLine> GetAPGuiCMInvoiceLine(int? cmInvoiceID)
        {
            return PXSelect<GVApGuiCmInvoiceLine,
                Where<GVApGuiCmInvoiceLine.guiCmInvoiceID, Equal<Required<GVApGuiCmInvoiceLine.guiCmInvoiceID>>>
            >.Select(this, cmInvoiceID);
        }

        public virtual PXResultset<GVArGuiCmInvoiceLine> GetARGuiCMInvoiceLine(int? cmInvoiceID)
        {
            return PXSelect<GVArGuiCmInvoiceLine,
                Where<GVArGuiCmInvoiceLine.guiCmInvoiceID, Equal<Required<GVArGuiCmInvoiceLine.guiCmInvoiceID>>>
            >.Select(this, cmInvoiceID);
        }

        public virtual GVApGuiInvoice GetGVAPInvoiceByID(int? id)
        {
            return PXSelect<GVApGuiInvoice,
                Where<GVApGuiInvoice.guiInvoiceID, Equal<Required<GVApGuiInvoice.guiInvoiceID>>>
            >.Select(this, id);
        }

        public virtual GVArGuiInvoice GetGVARInvoiceByID(int? id)
        {
            return PXSelect<GVArGuiInvoice,
                Where<GVArGuiInvoice.guiInvoiceID, Equal<Required<GVArGuiInvoice.guiInvoiceID>>>
            >.Select(this, id);
        }

        public virtual GVApGuiCmInvoice GetGVAPCmInvoiceByID(int? id)
        {
            return PXSelect<GVApGuiCmInvoice,
                Where<GVApGuiCmInvoice.guiCmInvoiceID, Equal<Required<GVApGuiCmInvoice.guiCmInvoiceID>>>
            >.Select(this, id);
        }

        public virtual GVArGuiCmInvoice GetGVARCmInvoiceByID(int? id)
        {
            return PXSelect<GVArGuiCmInvoice,
                Where<GVArGuiCmInvoice.guiCmInvoiceID, Equal<Required<GVArGuiCmInvoice.guiCmInvoiceID>>>
            >.Select(this, id);
        }
        #endregion
    }

    #region Tables

    #region GVARAPInvFilter
    [Serializable]
    public class GVARAPInvFilter : IBqlTable
    {
        #region RegistrationCD
        [PXString(9, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Registration CD", Required = true)]
        [RegistrationCD()]
        [PXUnboundDefault(typeof(
            Search2<GVRegistration.registrationCD,
                InnerJoin<GVRegistrationBranch, On<GVRegistration.registrationID, Equal<GVRegistrationBranch.registrationID>>,
                InnerJoin<Branch, On<Branch.bAccountID, Equal<GVRegistrationBranch.bAccountID>>>>,
                Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>),
            PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string RegistrationCD { get; set; }
        public abstract class registrationCD : PX.Data.BQL.BqlString.Field<registrationCD> { }
        #endregion

        #region DeclareYearFrom
        [PXInt(MaxValue = 9999, MinValue = 1900)]
        [PXUIField(DisplayName = "Declare Year From")]
        [PXUnboundDefault(typeof(DatePart<DatePart.year, Current<AccessInfo.businessDate>>))]
        //[GenericFinYearSelector(typeof(Search3<FinYear.year, OrderBy<Desc<FinYear.year>>>), null)]
        public virtual int? DeclareYearFrom { get; set; }
        public abstract class declareYearFrom : PX.Data.BQL.BqlInt.Field<declareYearFrom> { }
        #endregion

        #region DeclareYearTo
        [PXInt()]
        [PXUIField(DisplayName = "Declare Year To")]
        //[PXUnboundDefault(typeof(DatePart<DatePart.year, Current<AccessInfo.businessDate>>))]
        [PXFormula(typeof(GVARAPInvFilter.declareYearFrom))]
        public virtual int? DeclareYearTo { get; set; }
        public abstract class declareYearTo : PX.Data.BQL.BqlInt.Field<declareYearTo> { }
        #endregion

        #region Period
        [PXInt()]
        [PXUIField(DisplayName = "Period")]
        [PXUnboundDefault]
        //[PXIntList(new int[] { 2, 4, 6, 8, 10, 12 }, new string[] { "2", "4", "6", "8", "10", "12" })]
        [PXIntList(new int[] { 2, 4, 6, 8, 10, 12 }, new string[] { "1-2��", "3-4��", "5-6��", "7-8��", "9-10��", "11-12��" })]
        public virtual int? Period { get; set; }
        public abstract class period : PX.Data.BQL.BqlInt.Field<period> { }
        #endregion

        #region DeclareMonthFrom
        [PXInt(MaxValue = 12, MinValue = 1)]
        [PXUIField(DisplayName = "Declare Month From")]
        [PXUnboundDefault]
        public virtual int? DeclareMonthFrom { get; set; }
        public abstract class declareMonthFrom : PX.Data.BQL.BqlInt.Field<declareMonthFrom> { }
        #endregion

        #region DeclareMonthTo
        [PXInt(MaxValue = 12, MinValue = 1)]
        [PXUIField(DisplayName = "Declare Month To")]
        [PXUnboundDefault]
        public virtual int? DeclareMonthTo { get; set; }
        public abstract class declareMonthTo : PX.Data.BQL.BqlInt.Field<declareMonthTo> { }
        #endregion

        #region GvType
        [PXString()]
        [PXUIField(DisplayName = "Gv Type")]
        [PXUnboundDefault("IO")]
        public virtual string GvType { get; set; }
        public abstract class gvType : PX.Data.BQL.BqlString.Field<gvType> { }
        #endregion

        #region DeclareBatchNbr
        [PXString()]
        [PXUIField(DisplayName = "Declare Batch Nbr")]
        public virtual string DeclareBatchNbr { get; set; }
        public abstract class declareBatchNbr : PX.Data.BQL.BqlString.Field<declareBatchNbr> { }
        #endregion

        #region const
        /// <summary> �i�P�� </summary>
        public const string IO = "IO";
        /// <summary> �i�� </summary>
        public const string I = "I";
        /// <summary> �P�� </summary>
        public const string O = "O";

        /// <summary> �i�P�� </summary>
        public class io : PX.Data.BQL.BqlString.Constant<io> { public io() : base(IO) {; } }
        /// <summary> �i�� </summary>
        public class i : PX.Data.BQL.BqlString.Constant<i> { public i() : base(I) {; } }
        /// <summary> �P�� </summary>
        public class o : PX.Data.BQL.BqlString.Constant<o> { public o() : base(O) {; } }
        #endregion

        public PXResultset<GVDataView> Datas { get; set; }
    }
    #endregion

    #region GVDataView
    /// <summary>
    /// Mantis [0012315] #6
    /// </summary>
    [Serializable]
    [PXCacheName("GVDataView")]
    public partial class GVDataView : IBqlTable
    {
        #region ID
        [PXDBInt()]
        public virtual int? ID { get; set; }
        public abstract class id : PX.Data.BQL.BqlInt.Field<id> { }
        #endregion

        #region Seq
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Seq")]
        public virtual int? Seq { get; set; }
        public abstract class seq : PX.Data.BQL.BqlInt.Field<seq> { }
        #endregion

        #region Selected
        [PXDBBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region GvType 
        [PXDBString(IsFixed = true)]
        [PXUIField(DisplayName = "Gv Type")]
        [PXStringList(new string[] { GVARAPInvFilter.I, GVARAPInvFilter.O },
                      new string[] { "�i��", "�P��" })]
        public virtual string GvType { get; set; }
        public abstract class gvType : PX.Data.BQL.BqlString.Field<gvType> { }
        #endregion

        #region GuiInvoiceNbr 
        [PXDBString()]
        [PXUIField(DisplayName = "Gui Invoice Nbr")]
        public virtual string GuiInvoiceNbr { get; set; }
        public abstract class guiInvoiceNbr : PX.Data.BQL.BqlString.Field<guiInvoiceNbr> { }
        #endregion

        #region GuiCmInvoiceNbr 
        [PXDBString()]
        [PXUIField(DisplayName = "Gui Cm Invoice Nbr")]
        public virtual string GuiCmInvoiceNbr { get; set; }
        public abstract class guiCmInvoiceNbr : PX.Data.BQL.BqlString.Field<guiCmInvoiceNbr> { }
        #endregion

        #region InvoiceDate 
        [PXDBDate()]
        [PXUIField(DisplayName = "Invoice Date")]
        public virtual DateTime? InvoiceDate { get; set; }
        public abstract class invoiceDate : PX.Data.BQL.BqlDateTime.Field<invoiceDate> { }
        #endregion

        #region GuiType 
        [PXDBString()]
        [PXUIField(DisplayName = "Gui Type")]
        [GVList.GVGuiType]
        public virtual string GuiType { get; set; }
        public abstract class guiType : PX.Data.BQL.BqlString.Field<guiType> { }
        #endregion

        #region InvoiceDesc  
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Invoice Desc")]
        public virtual string InvoiceDesc { get; set; }
        public abstract class invoiceDesc : PX.Data.BQL.BqlString.Field<invoiceDesc> { }
        #endregion

        #region SalesAmt  
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Sales Amt")]
        public virtual decimal? SalesAmt { get; set; }
        public abstract class salesAmt : PX.Data.BQL.BqlDecimal.Field<salesAmt> { }
        #endregion

        #region TaxAmt  
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Tax Amt")]
        public virtual decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion

        #region TotalAmt  
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Total Amt")]
        public virtual decimal? TotalAmt { get; set; }
        public abstract class totalAmt : PX.Data.BQL.BqlDecimal.Field<totalAmt> { }
        #endregion

        #region SellerID
        [PXDBString()]
        [PXUIField(DisplayName = "Seller ID")]
        public virtual string SellerID { get; set; }
        public abstract class sellerID : PX.Data.BQL.BqlString.Field<sellerID> { }
        #endregion

        #region BuyerID
        [PXDBString()]
        [PXUIField(DisplayName = "Buyer ID")]
        public virtual string BuyerID { get; set; }
        public abstract class buyerID : PX.Data.BQL.BqlString.Field<buyerID> { }
        #endregion

        #region VoucherCategory
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Voucher Category")]
        [GVGuiVoucherCategory]
        public virtual string VoucherCategory { get; set; }
        public abstract class voucherCategory : PX.Data.BQL.BqlString.Field<voucherCategory> { }
        #endregion

        #region TaxCode
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Code")]
        [GVTaxCode]
        public virtual string TaxCode { get; set; }
        public abstract class taxCode : PX.Data.BQL.BqlString.Field<taxCode> { }
        #endregion

        #region DeductionCode
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Deduction Code")]
        [GVGuiDeductionCode]
        public virtual string DeductionCode { get; set; }
        public abstract class deductionCode : PX.Data.BQL.BqlString.Field<deductionCode> { }
        #endregion

        #region GroupRemark
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "GroupRemark")]
        [GVGuiGroupRemark]
        public virtual string GroupRemark { get; set; }
        public abstract class groupRemark : PX.Data.BQL.BqlString.Field<groupRemark> { }
        #endregion

        #region GroupCnt
        [PXDBInt()]
        [PXUIField(DisplayName = "Group Count")]
        public virtual int? GroupCnt { get; set; }
        public abstract class groupCnt : PX.Data.BQL.BqlInt.Field<groupCnt> { }
        #endregion

        #region DeclareYear
        [PXDBInt()]
        [PXUIField(DisplayName = "Declare Year")]
        public virtual int? DeclareYear { get; set; }
        public abstract class declareYear : PX.Data.BQL.BqlInt.Field<declareYear> { }
        #endregion

        #region DeclareMonth
        [PXDBInt()]
        [PXUIField(DisplayName = "Declare Month")]
        public virtual int? DeclareMonth { get; set; }
        public abstract class declareMonth : PX.Data.BQL.BqlInt.Field<declareMonth> { }
        #endregion

        #region Status
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status")]
        [GVList.GVStatus]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region DeclareBatchNbr
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "Declare Batch Nbr")]
        public virtual string DeclareBatchNbr { get; set; }
        public abstract class declareBatchNbr : PX.Data.BQL.BqlString.Field<declareBatchNbr> { }
        #endregion

        #region DeclaredByID
        [PXGuid()]
        [PXUIField(DisplayName = "Declared On")]
        [PXSelector(typeof(Search<EPEmployee.userID, Where<EPEmployee.userID, Equal<Current<declaredByID>>>>),
                    SubstituteKey = typeof(EPEmployee.acctName),
                    ValidateValue = false)]
        public virtual Guid? DeclaredByID { get; set; }
        public abstract class declaredByID : PX.Data.BQL.BqlGuid.Field<declaredByID> { }
        #endregion

        #region DeclaredDateTime
        [PXDateAndTime(UseTimeZone = true)]
        [PXUIField(DisplayName = "Declared Date Time")]
        public virtual DateTime? DeclaredDateTime { get; set; }
        public abstract class declaredDateTime : PX.Data.BQL.BqlDateTime.Field<declaredDateTime> { }
        #endregion

        public virtual ViewType Type { get; set; }
        public enum ViewType
        {
            AP, AR, APCM, ARCM
        }
    }
    #endregion

    #endregion
}