using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.PM;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Objects.PO;
using PX.Objects.AP;
using PX.Objects.CR;
using Kedge.DAC;
using PX.Objects.EP;
using System.Linq;
using static Kedge.KGDailyRenterActualEntry;
using PX.SM;
/**
 * ====2021-05-06: ====Alton
 * �NQty�ӷ��אּ ConfirmQty
 * 
 * ===2021/11/03 === Althea
 * Modify ���ͥ[���ڳ��,�u�ƽs��AD�@�P
 * **/
namespace Kedge
{
    public class KGRenterConfirmProcess : PXGraph<KGRenterConfirmProcess>
    {
        #region Filter

        //2020/04/23 �����R�� �s�W �ƻs�K�W �W�U�@�� ���s
        public PXSave<RenterFilter> Save;
        public PXCancel<RenterFilter> Cancel;

        public PXFilter<RenterFilter> RenterFilter;

        //2020/02/11 ADD �L�o���N��ƶq��0�����,�]�������T�{
        public PXSelectJoin<KGDailyRenterVendor,
                    InnerJoin<KGDailyRenter,
                        On<KGDailyRenter.dailyRenterID,
                        Equal<KGDailyRenterVendor.dailyRenterID>,
                        And<KGDailyRenterVendor.type,Equal<word.a>>>,
                    InnerJoin<Users,
                        On<Users.pKID,
                        Equal<KGDailyRenterVendor.createdByID>>>>,
                    Where<KGDailyRenterVendor.insteadQty,Greater<word.zero>>,
                    OrderBy<Desc<KGDailyRenterVendor.dailyRenterID>>> RenterVendors;
       
        public PXSelect<KGDailyRenter, Where<KGDailyRenter.status, 
            In3<ACCTUALLY, PENDINGONSITE, ONSITE>>> DailyRenters;
        protected virtual IEnumerable renterVendors()
        {
            PXSelectJoin<KGDailyRenterVendor,
                   InnerJoin<KGDailyRenter,
                       On<KGDailyRenter.dailyRenterID,
                       Equal<KGDailyRenterVendor.dailyRenterID>,
                       And<KGDailyRenterVendor.type, Equal<word.a>>>,
                   InnerJoin<Users,
                       On<Users.pKID,
                       Equal<KGDailyRenterVendor.createdByID>>>>,
                   Where<KGDailyRenterVendor.insteadQty, Greater<word.zero>>,
                   OrderBy<Desc<KGDailyRenterVendor.dailyRenterID>>> query =
            new PXSelectJoin<KGDailyRenterVendor,
                    InnerJoin<KGDailyRenter,
                        On<KGDailyRenter.dailyRenterID,
                        Equal<KGDailyRenterVendor.dailyRenterID>,
                        And<KGDailyRenterVendor.type, Equal<word.a>>>,
                    InnerJoin<Users,
                        On<Users.pKID,
                        Equal<KGDailyRenterVendor.createdByID>>>>,
                    Where<KGDailyRenterVendor.insteadQty, Greater<word.zero>>,
                    OrderBy<Desc<KGDailyRenterVendor.dailyRenterID>>>(this);
            // Adding filtering conditions to the query
            RenterFilter filter = RenterFilter.Current;
            if (filter.ContractID != null)
                query.WhereAnd<Where<KGDailyRenterVendor.contractID,
                    Equal<Current<RenterFilter.contractID>>>>();
            if (filter.OrderNbr != null)
                query.WhereAnd<Where<KGDailyRenterVendor.orderNbr,
                    Equal<Current<RenterFilter.orderNbr>>>>();
            if (filter.WorkDateFrom != null)
                query.WhereAnd<Where<KGDailyRenter.workDate,
                    GreaterEqual<Current<RenterFilter.workDateFrom>>>>();
            if (filter.WorkDateTo != null)
                query.WhereAnd<Where<KGDailyRenter.workDate,
                    LessEqual<Current<RenterFilter.workDateTo>>>>();
            if (filter.VendorID != null)
                query.WhereAnd<Where<KGDailyRenterVendor.vendorID,
                    Equal<Current<RenterFilter.vendorID>>>>();
            if (filter.Status != null)
                query.WhereAnd<Where<KGDailyRenterVendor.status,
                    Equal<Current<RenterFilter.status>>>>();
            if (filter.CreatedByID != null)
                query.WhereAnd<Where<Users.username,
                    Equal<Current<RenterFilter.createdByID>>>>();
            return query.Select();
        }

        #endregion

        //2020/07/15 button�令PXLongOperation�g�k

        #region Action
        /*public PXAction<RenterFilter> ActionMenu;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Action")]
        protected void actionMenu() { }*/


        ///<summary>�|ñ���s�޿�</summary>
        ///1.�s��諸�T�{�ƶq(DailyRenterVendor)
        ///2.�ˮ֧�諸�T�{�ƶq���S���W��DailyRenter,
        ///3.��窱�A��w�|ñ
        ///4.�g�JbatchNbr�аO�������
        ///5..��ܳ���
        ///6.�u��"�w��u"�i�ϥΦ����s
       
        //2019/06/05���~�T���אּ���e�X�{�brow���e��
        #region SendApprovalAction
        public PXAction<RenterFilter> SendApprovalAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Print & Sign")]
        public virtual IEnumerable sendApprovalAction(PXAdapter adapter, [PXString]
            string reportID)
        {
            //KGDailyRenterVendor RV = RenterVendors.Current;
            //2019/06/17��Selected checkbox���^��

            //���ˬd���A
            foreach (KGDailyRenterVendor kGDailyRenterVendor in RenterVendors.Select())
            {
                if (kGDailyRenterVendor != null)
                {
                    if (kGDailyRenterVendor.Selected == true)
                    {
                        if (kGDailyRenterVendor.Status == "S")
                        {
                            //2019/06/17�ˬdConfirmQty�O�_���ŭ�
                            checkConfirmQty(RenterVendors.Cache, kGDailyRenterVendor);
                            if (!CreateValuation(kGDailyRenterVendor, "1"))
                            {
                                setCheckThread = true;
                                RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                                 kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("�нT�{�����������ڪ��B!", PXErrorLevel.RowError));
                                //throw new Exception("�нT�{�����������ڪ��B!");
                            }
                        }
                        else if (kGDailyRenterVendor.Status == "W")
                        {
                            setCheckThread = true;

                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                             kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("�����A���u�w�|ñ�v�C" + "\n" + "�Y�n�Ѱ��A�а���u�Ѱ��|ñ�v�F" + "\n" + "�Y�nñ�{�A�а���uñ�{�ΤW�Ǫ���v!", PXErrorLevel.RowError));
                            //throw new Exception("�����A���u�w�|ñ�v�C" + "\n" + "�Y�n�Ѱ��A�а���u�Ѱ��|ñ�v�F" + "\n" + "�Y�nñ�{�A�а���uñ�{�ΤW�Ǫ���v!");

                        }
                        else if (kGDailyRenterVendor.Status == "C")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                             kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("�����A���u�wñ�{�v�C" + "\n" + "�Y�n�Ѱ��A�а���u�Ѱ�ñ�{�v!", PXErrorLevel.RowError));
                            //throw new Exception("�����A���u�wñ�{�v�C" + "\n" + "�Y�n�Ѱ��A�а���u�Ѱ�ñ�{�v!");

                        }
                    }
                }
            }
            if (!setCheckThread)
            {
                PXLongOperation.StartOperation(this, delegate ()
                {


                    KGRenterConfirmProcess graph = PXGraph.CreateInstance<KGRenterConfirmProcess>();
                    string batchnbr = graph.Accessinfo.UserName
                        + System.DateTime.Now.ToString("yyyyMMddHHmmss");
                    foreach (KGDailyRenterVendor dailyRenterVendor in RenterVendors.Select())
                    {
                        if (dailyRenterVendor.Selected == true)
                        {
                            if (dailyRenterVendor.Status == "S")
                            {
                                if (dailyRenterVendor.ConfirmQty == 0)
                                {
                                    dailyRenterVendor.Status = "C";
                                    dailyRenterVendor.Selected = false;
                                    RenterVendors.Update(dailyRenterVendor);
                                }
                                else if (dailyRenterVendor.ConfirmQty > 0)
                                {
                                    dailyRenterVendor.BatchNbr = batchnbr;
                                    dailyRenterVendor.Status = "W";
                                    dailyRenterVendor.Selected = false;
                                    RenterVendors.Update(dailyRenterVendor);
                                }

                                //2020/07/15 �t�ӽT�{���s�~�n�^���N��ƶq
                                /*KGDailyRenter dailyRenter =
                                                    PXSelect<KGDailyRenter,
                                                    Where<KGDailyRenter.dailyRenterID,
                                                    Equal<Required<KGDailyRenter.dailyRenterID>>>>
                                                    .Select(this, dailyRenterVendor.DailyRenterID);
                                //�^���N��ƶq=�T�{�ƶq �ۿ�ƶq=��ڼƶq-�N��ƶq
                                dailyRenter.ActInsteadQty = dailyRenterVendor.ConfirmQty;
                                dailyRenter.ActSelfQty = dailyRenter.ActQty - dailyRenterVendor.ConfirmQty;
                                KGDailyRenters.Update(dailyRenter);*/

                            }
                        }
                    }
                    base.Persist();
                    KGRenterConfirmProcess pp = CreateInstance<KGRenterConfirmProcess>();
                    NewPageToDisplayReport(pp);
                });

               
            }
      

            #region 2019/05/30�令report�}�ҷs����
            /*KGDailyRenterVendor DRV = RenterVendors.Current;
            if (reportID == null) reportID = "KG601000";
            if (DRV != null)
            {
                Dictionary<string, string> mailParams = new Dictionary<string, string>();
                mailParams["DailyRenterVendorID"] = DRV.DailyRenterVendorID.ToString();
                throw new PXReportRequiredException(mailParams, reportID, "Report");
            }*/
            #endregion
            return adapter.Get();
        }
        #endregion

        /// <summary>�t��ñ�{���s�޿�</summary>
        /// 1.��T�{�ƶq��^DailyRenter(Header)
        /// 2.���ͥ[���ڳ�
        /// 3.��窱�A���wñ�{
        /// 4.�u��"�w�|ñ"�i�H�ϥΦ����s
     
        #region VendorConfirmAction
        public PXAction<RenterFilter> VendorConfirmAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Vendor Confirm", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable vendorConfirmAction(PXAdapter adapter)
        {
            //���ˮ�
            foreach (KGDailyRenterVendor kGDailyRenterVendor in RenterVendors.Select())
            {
                if (kGDailyRenterVendor != null)
                {
                    if (kGDailyRenterVendor.Selected == true)
                    {
                        if (kGDailyRenterVendor.Status == "W")
                        {
                            //2020/09/15 Mantis: 0011686
                            //checkFileLength(RenterVendors.Cache, kGDailyRenterVendor);
                        }
                        else if (kGDailyRenterVendor.Status == "S")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                             kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("�����A���u�w��u�v�C" + "\n" + "�Y�n�|ñ�A�а���u�C�L�η|ñ�v!", PXErrorLevel.RowError));
                            //throw new Exception("�����A���u���|ñ�v�C" + "\n" + "�Y�n�|ñ�A�а���u�C�L�η|ñ�v!");
                        }
                        else if (kGDailyRenterVendor.Status == "C")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                             kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("�����A���u�wñ�{�v�C" + "\n" + "�Y�n�Ѱ��A�а���u�Ѱ�ñ�{�v!", PXErrorLevel.RowError));
                            //throw new Exception("�����A���u�wñ�{�v�C" + "\n" + "�Y�n�Ѱ��A�а���u�Ѱ�ñ�{�v!");
                        }
                    }
                }
            }

            if (!setCheckThread)
            {
                PXLongOperation.StartOperation(this, delegate ()
                {
                    //2020/04/06 �Ҽ{��h����dailyRenterVendor ���^����s�ƶq�� �A��s�e���W��status
                    //2020/11/04 Mantis:0011791  ��X���T�{�����"�N��"�ƶq+�w�T�{�L��"�T�{"�ƶq
                    //2020/11/05 ��Transaction�]
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        foreach (KGDailyRenterVendor dailyRenterVendor in RenterVendors.Select())
                        {
                            if (dailyRenterVendor == null) return;
                            if(dailyRenterVendor.Status =="W" && dailyRenterVendor.Selected == true)
                            {

                                dailyRenterVendor.Status = "C";
                                dailyRenterVendor.Selected = false;                                
                                RenterVendors.Update(dailyRenterVendor);

                                CreateValuation(dailyRenterVendor, "2");

                                base.Persist();
                                //�^���N��ƶq
                                decimal? confirmQty = 0;
                                decimal? unconfirmQty = 0;
                                //��X���A���t�ӽT�{���T�{�ƶq
                                KGDailyRenterVendor confirmRenterVendor =
                                    PXSelectGroupBy<KGDailyRenterVendor,
                                    Where<KGDailyRenterVendor.status, Equal<Required<KGDailyRenterVendor.status>>,
                                    And<KGDailyRenterVendor.insteadQty, Greater<word.zero>,
                                    And<KGDailyRenterVendor.type, Equal<word.a>,
                                    And<KGDailyRenterVendor.dailyRenterID, Equal<Required<KGDailyRenterVendor.dailyRenterID>>>>>>,
                                    Aggregate<GroupBy<KGDailyRenterVendor.dailyRenterID,
                                    Sum<KGDailyRenterVendor.confirmQty>>>>.Select(this, "C", dailyRenterVendor.DailyRenterID);
                                if (confirmRenterVendor != null)
                                    confirmQty = confirmRenterVendor.ConfirmQty;
                                else
                                    confirmQty = 0;

                                //��X���A���w��u�η|ñ������ڥN��ƶq
                                KGDailyRenterVendor unConfirmRenterVendor =
                                    PXSelectGroupBy<KGDailyRenterVendor,
                                    Where<KGDailyRenterVendor.insteadQty, Greater<word.zero>,
                                    And<KGDailyRenterVendor.type, Equal<word.a>,
                                    And<KGDailyRenterVendor.dailyRenterID, Equal<Required<KGDailyRenterVendor.dailyRenterID>>,                          
                                    And<Where<KGDailyRenterVendor.status, Equal<Required<KGDailyRenterVendor.status>>,
                                    Or<KGDailyRenterVendor.status, Equal<Required<KGDailyRenterVendor.status>>>>>>>>,
                                    Aggregate<GroupBy<KGDailyRenterVendor.dailyRenterID,
                                    Sum<KGDailyRenterVendor.insteadQty>>>>.Select(this,dailyRenterVendor.DailyRenterID,"S","W");
                                if (unConfirmRenterVendor != null)
                                    unconfirmQty = unConfirmRenterVendor.InsteadQty;
                                else
                                    unconfirmQty = 0;

                                UpdateDailyRenter(dailyRenterVendor, confirmQty+unconfirmQty);

                            }
                        }
                        ts.Complete();
                    }
                     
                });
        }
            
            return adapter.Get();


        }
        #endregion

        /// <summary>�Ѱ��|ñ���s�޿�</summary>
        /// 1.�u�����A���w�|ñ�i�H�ϥΦ����s
        /// 2.���A��^�w��u
        /// 3.�M��batchNbr

        #region UnSignAction
        public PXAction<RenterFilter> UnSignAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "UnSign", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable unSignAction(PXAdapter adapter)
        {
            //���ˬd���A
            foreach (KGDailyRenterVendor kGDailyRenterVendor in RenterVendors.Select())
            {
                if (kGDailyRenterVendor != null)
                {
                    if (kGDailyRenterVendor.Selected == true)
                    {
                        if (kGDailyRenterVendor.Status == "S")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                           kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("�����A���u�w��u�v�C" + "\n" + "�Y�n�|ñ�A�а���u�C�L�η|ñ�v!", PXErrorLevel.RowError));
                            //throw new Exception("�����A���u���|ñ�v�C" + "\n" + "�Y�n�|ñ�A�а���u�C�L�η|ñ�v!");
                        }
                        else if (kGDailyRenterVendor.Status == "C")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                           kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("�����A���u�wñ�{�v�C" + "\n" + "�Y�n�Ѱ��A�а���u�Ѱ�ñ�{�v!", PXErrorLevel.RowError));
                            //throw new Exception("�����A���u�wñ�{�v�C" + "\n" + "�Y�n�Ѱ��A�а���u�Ѱ�ñ�{�v!");
                        }
                    }
                }
            }
            if (!setCheckThread)
            {
                PXLongOperation.StartOperation(this, delegate ()
                {

                    foreach (KGDailyRenterVendor dailyRenterVendor in RenterVendors.Select())
                    {
                        if (dailyRenterVendor != null)
                        {
                            if (dailyRenterVendor.Selected == true)
                            {
                                if (dailyRenterVendor.Status == "W")
                                {
                                    dailyRenterVendor.Status = "S";
                                    dailyRenterVendor.BatchNbr = null;
                                    dailyRenterVendor.Selected = false;
                                    RenterVendors.Update(dailyRenterVendor);
                                }
                            }
                        }
                    }
                    base.Persist();
                });
            }
            return adapter.Get();
        }
        #endregion

        /// <summary>�Ѱ�ñ�{���s�޿�</summary>
        /// 1.�u��"�wñ�{"�~�i�H�ϥΦ����s
        /// 2.�R���[���ڳ�
        /// 3.��窱�A��"�w�|ñ"
        /// 4.�ˮ֬O�_���w�p��,�Y�p����w�Q�p��,�X���~:���i��w�Q�p��,���i�Ѱ�ñ�{

        #region UnConfirmAction
        public PXAction<RenterFilter> UnConfirmAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Vendor UnConfirm", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable unConfirmAction(PXAdapter adapter)
        {
            //�ˬd���A
            foreach (KGDailyRenterVendor kGDailyRenterVendor in RenterVendors.Select())
            {
                if (kGDailyRenterVendor != null)
                {
                    if (kGDailyRenterVendor.Selected == true)
                    {
                        if (kGDailyRenterVendor.Status == "C")
                        {
                            KGValuationDetail valuationdetail = PXSelectJoin<KGValuationDetail,
                                InnerJoin<KGValuation,
                            On<KGValuation.valuationID,
                            Equal<KGValuationDetail.valuationID>>>,
                                Where<KGValuation.dailyRenterCD,
                                Equal<Required<KGValuation.dailyRenterCD>>>>
                                .Select(this, kGDailyRenterVendor.DailyRenterCD);
                            if (valuationdetail != null)
                            {
                                if (valuationdetail.Status == "B")
                                {
                                    setCheckThread = true;
                                    RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                               kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("�ӵ���Ƥw�g�����p��, ��������t�ӽT�{!", PXErrorLevel.RowError));
                                    //throw new Exception("�ӵ���Ƥw�g�����p��, ��������t�ӽT�{!");
                                }
                            }

                        }
                        else if (kGDailyRenterVendor.Status == "W")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                           kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("�����A���u�w�|ñ�v�C" + "\n" + "�Y�n�Ѱ��A�а���u�Ѱ��|ñ�v�F" + "\n" + "�Y�nñ�{�A�а���uñ�{�ΤW�Ǫ���v!", PXErrorLevel.RowError));
                            //throw new Exception("�����A���u�w�|ñ�v�C" + "\n" + "�Y�n�Ѱ��A�а���u�Ѱ��|ñ�v�F" + "\n" + "�Y�nñ�{�A�а���uñ�{�ΤW�Ǫ���v!");
                        }
                        else if (kGDailyRenterVendor.Status == "S")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                           kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("�����A���u�w��u�v�C" + "\n" + "�Y�n�|ñ�A�а���u�C�L�η|ñ�v!", PXErrorLevel.RowError));
                            //throw new Exception("�����A���u���|ñ�v�C" + "\n" + "�Y�n�|ñ�A�а���u�C�L�η|ñ�v!");
                        }

                    }
                }
            }
            if (!setCheckThread)
            {
                PXLongOperation.StartOperation(this, delegate ()
                {
                    foreach (KGDailyRenterVendor dailyRenterVendor in RenterVendors.Select())
                    {
                        if (dailyRenterVendor != null)
                        {
                            if (dailyRenterVendor.Selected == true)
                            {
                                if (dailyRenterVendor.Status == "C")
                                {
                                    if (dailyRenterVendor.ConfirmQty != 0)
                                    {
                                        DeleteValuation(dailyRenterVendor);
                                    }
                                    dailyRenterVendor.Status = "W";
                                    dailyRenterVendor.Selected = false;
                                    RenterVendors.Update(dailyRenterVendor);
                                }
                            }
                        }
                    }
                    base.Persist();
                });
            }

            return adapter.Get();
        }
        #endregion



        /*public KGRenterConfirmProcess()
        {
            this.ActionMenu.MenuAutoOpen = true;
            this.ActionMenu.AddMenuAction(this.SendApprovalAction);
            this.ActionMenu.AddMenuAction(this.VendorConfirmAction);
            this.ActionMenu.AddMenuAction(this.UnSignAction);
            this.ActionMenu.AddMenuAction(this.UnConfirmAction);
        }*/
        #endregion

        #region Event
        protected virtual void KGDailyRenterVendor_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGDailyRenterVendor row = (KGDailyRenterVendor)e.Row;
            if (row == null) return;
            PXUIFieldAttribute.SetEnabled(sender, row, false);
            PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.selected>(sender, row, true);
            PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.confirmQty>(sender, row, row.Status =="S");
            RenterVendors.AllowInsert = false;
            RenterVendors.AllowDelete = false;
            //2019/04/19ConfrimAmt�令ConfirmInstQty->2019/04/08��confirmInsQty����
            //PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.confirmInstQty>(sender, row, true);
            //PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.selected>(sender, row, true);
            //2019/04/22 ���[if���ˮַ|�X�{null��exception ���~
            if (row != null)
            {
                PXResultset<KGDailyRenter> set = PXSelect<KGDailyRenter,
              Where<KGDailyRenter.dailyRenterID,
              Equal<Required<KGDailyRenter.dailyRenterID>>>>
                      .Select(this, row.DailyRenterID);
                foreach (KGDailyRenter kgorder in set)
                {
                    row.DailyRenterCD = kgorder.DailyRenterCD;
                    row.WorkDate = kgorder.WorkDate;
                }
                //2019/05/08�e���[�J�M�״y�z�b�M�׮���(Unbound)
                PXResultset<PMProject> setpm =
                    PXSelectJoin<PMProject,
                    InnerJoin<KGDailyRenterVendor,
                    On<KGDailyRenterVendor.contractID,
                    Equal<PMProject.contractID>>>,
                    Where<KGDailyRenterVendor.contractID,
                    Equal<Required<KGDailyRenterVendor.contractID>>>>
                    .Select(this, row.ContractID);
                foreach (PMProject pMProject in setpm)
                {
                    row.Contractdesc = pMProject.Description;
                }
            }
            if(row.Status=="S")
            {
                PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.confirmQty>(sender, row, true);
            }
            else
            {
                PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.confirmQty>(sender, row, false);
            }

        }
        /*Sergey�Ъ���k
         * protected virtual void KGDailyRenterVendor_Selected_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)

        {
            KGDailyRenterVendor row = (KGDailyRenterVendor)e.Row;
            if(e.NewValue==null)
            {
                foreach (KGDailyRenterVendor rentervendor in RenterVendors.Select())
                {
                    if (rentervendor.Selected == true && rentervendor.DailyRenterID !=row.DailyRenterID )

                    {

                        rentervendor.Selected = false;

                        sender.Update(rentervendor);

                    }

                }

            }
            
        }*/
        //2019/07/01�N��ƶq�B�ۿ�ƶq�BAmount���n��
        protected virtual void KGDailyRenterVendor_ConfirmQty_FieldUpdated(PXCache sender ,PXFieldUpdatedEventArgs e)
        {
            KGDailyRenterVendor renterVendor = (KGDailyRenterVendor)e.Row;         
            checkConfirmQty(sender, renterVendor);
            KGDailyRenter renter = PXSelect<KGDailyRenter,
                Where<KGDailyRenter.dailyRenterID, Equal<Required<KGDailyRenterVendor.dailyRenterID>>>>
                .Select(this, renterVendor.DailyRenterID);
            POLine poline = getPOLine(renter.LineNbr, renter.OrderNbr);
            if (poline == null)
            {
                renterVendor.Amount = renterVendor.ConfirmQty * 1;
            }
            else
            {
                renterVendor.Amount = renterVendor.ConfirmQty * poline.CuryUnitCost;
            }
        }
        #endregion

        #region Select
        public PXSelect<KGValuation> Valuations;
        public PXSelect<KGValuationDetail> ValuationDetails;
        public PXSetup<KGSetUp> SetUp;
        public PXSelect<KGValuationDetailA,
                 Where<KGValuationDetailA.valuationID,
                     Equal<Current<KGValuation.valuationID>>,
                 And<KGValuationDetailA.pricingType, Equal<word.a>>>> AdditionDetails;
        public PXSelect<KGValuationDetailD,
                 Where<KGValuationDetailD.valuationID,
                   Equal<Current<KGValuation.valuationID>>,
                 And<KGValuationDetailD.pricingType, Equal<word.d>>>> DeductionDetails;
        public PXSelect<KGDailyRenter,
         Where<KGDailyRenter.dailyRenterID,
         Equal<Current<KGDailyRenterVendor.dailyRenterID>>>> KGDailyRenters;

        #endregion

        #region Method
        //�s�W�[���ڳ� 
        //2019/11/08 ADD�ˮ֥\��
        //checkorCreatetype:1=�ˮ֦��ڪ��B checkorCreatetype:2=�s�W�[����
        public bool CreateValuation(KGDailyRenterVendor DRV,string checkorCreatetype)
        {
             //= RenterVendors.Current;
            KGValuation KVHeader = new KGValuation();
            KGValuationDetailD KVDetailD = new KGValuationDetailD();
            KGValuationDetailA KVDetailA = new KGValuationDetailA();

            KVHeader.ContractID = DRV.ContractID;
            KVHeader.ValuationType = "0";
            KVHeader.Hold = false;
            KVHeader.Description = DRV.WorkContent;
            #region 2019/06/05��[���ڳ�������^�N���N�I/�����RenterVendor�����
            /*KVHeader.ValuationType = "2";
              POLine dailyrentervendor =PXSelectJoin<POLine,
                InnerJoin<KGDailyRenterVendor, On<KGDailyRenterVendor.orderNbr, 
                Equal<POLine.orderNbr>>>,
                Where<POLine.lineNbr, 
                Equal<Required<POLine.lineNbr>>>>
                .Select(this,DRV.LineNbr);
            KVHeader.UnitPrice = dailyrentervendor.CuryUnitCost;*/
            #endregion
            KVHeader.Status = "C";
            KVHeader.Uom = "�u";
            //Header+DetailA
            PXResultset<KGDailyRenter> set = PXSelect<KGDailyRenter,
                Where<KGDailyRenter.dailyRenterID,
                Equal<Required<KGDailyRenter.dailyRenterID>>>>
                        .Select(this, DRV.DailyRenterID);
            foreach (KGDailyRenter kgorder in set)
            {
                KVHeader.ValuationDate = kgorder.WorkDate;
                KVHeader.DailyRenterCD = kgorder.DailyRenterCD;
                //Header����
                #region 2019/06/05�ƶq�ܬ��N��ƶq /����ܬ�KGDailyRenter��purchase order Line�����
                /*PXResultset<POLine> po = PXSelect<POLine,
                            Where<POLine.orderNbr,
                                Equal<Required<POLine.orderNbr>>>>
                                .Select(this, DRV.OrderNbr);
                foreach (POLine poline in po)
                {
                    KVHeader.UnitPrice = poline.CuryUnitCost;
                    KVHeader.Qty = poline.UnbilledQty;*/
                #endregion

                POLine poline = PXSelect<POLine,
                    Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                    And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>
                    .Select(this, kgorder.OrderNbr, kgorder.LineNbr);
                KVHeader.UnitPrice = poline.CuryUnitCost;
                //2021-05-06 edit by Alton  InsteadQty��ConfirmQty
                KVHeader.Qty = DRV.ConfirmQty;
                KVHeader.Amount = KVHeader.Qty * KVHeader.UnitPrice;
                PXResultset<POTaxTran> Ptt = PXSelect<POTaxTran,
                            Where<POTaxTran.orderNbr,
                                Equal<Required<POTaxTran.orderNbr>>>>
                                .Select(this, DRV.OrderNbr);
                foreach (POTaxTran taxrate in Ptt)
                {
                    KVHeader.TaxAmt = KVHeader.Amount * (taxrate.TaxRate / 100);
                }

                KVHeader.TotalAmt = KVHeader.TaxAmt + KVHeader.Amount;
                PXResultset<KGSetUp> setup = PXSelect<KGSetUp,
                            Where<KGSetUp.kGManageFeeTaxRate,
                                IsNotNull>>.Select(this);
                foreach (KGSetUp rate in setup)
                {
                    KVHeader.ManageFeeAmt = KVHeader.Amount * (rate.KGManageFeeRate / 100);
                    KVHeader.ManageFeeTaxAmt
                        = KVHeader.ManageFeeAmt * (rate.KGManageFeeTaxRate / 100);
                }
                KVHeader.ManageFeeTotalAmt = KVHeader.ManageFeeAmt + KVHeader.ManageFeeTaxAmt;

                //}
                //�粒�N���N�I->2019/06/12�[���`�B=TotalAmt(�t�|���B)
                KVHeader.AdditionAmt = KVHeader.TotalAmt;
                KVHeader.DeductionAmt = KVHeader.TotalAmt + KVHeader.ManageFeeTotalAmt;

                //DetailA 2019/05/07�אּ�@�릩��(A����Ƥ��ζ�)
                //���Y��type�令�@�릩�ڪ���status�אּV���Y������
                //DetailA 2019/06/05��^�N���N�I(A��ƭn��/status="V")
                KVDetailA.OrderNbr = kgorder.OrderNbr;
                POOrder orderdetailA = PXSelect<POOrder,
                    Where<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>
                    .Select(this, KVDetailA.OrderNbr);
                if (orderdetailA.Status == "V")
                {
                    throw new Exception(String.Format("���渹{0},���A���w�ڵ�,�п�ܧO�i���]�����渹!", orderdetailA.OrderNbr));
                }
                KVDetailA.OrderType = orderdetailA.OrderType;
                KVDetailA.Uom = "�u";
                KVDetailA.Qty = KVHeader.Qty;
                KVDetailA.Amount = KVHeader.Amount;
                KVDetailA.UnitPrice = KVHeader.UnitPrice;
                KVDetailA.TaxAmt = KVHeader.TaxAmt;
                KVDetailA.TotalAmt = KVHeader.TotalAmt;
                KVDetailA.ContractID = KVHeader.ContractID;
                KVDetailA.Status = "V";
                KVDetailA.ManageFeeAmt = 0;
                KVDetailA.ManageFeeTaxAmt = 0;
                KVDetailA.ManageFeeTotalAmt = 0;
            }  
            //DetailD
            KVDetailD.ContractID = KVHeader.ContractID;
            KVDetailD.OrderNbr = DRV.OrderNbr;
            //2020/09/18 Fix POOrder status���w�ڵ� �ΥX������
            POOrder orderdetailD = PXSelect<POOrder,
                   Where<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>
                   .Select(this, KVDetailD.OrderNbr);
            if(orderdetailD.Status =="V")
            {
                throw new Exception(String.Format("���渹{0},���A���w�ڵ�,�п�ܧO�i���]�����渹!",orderdetailD.OrderNbr));
            }
            KVDetailD.OrderType = orderdetailD.OrderType;
            KVDetailD.Qty = KVHeader.Qty;
            KVDetailD.Uom = "�u";
            KVDetailD.Amount = KVHeader.Amount;
            KVDetailD.TaxAmt = KVHeader.TaxAmt;
            KVDetailD.TotalAmt = KVHeader.TotalAmt;
            KVDetailD.ManageFeeAmt = KVHeader.ManageFeeAmt;
            KVDetailD.ManageFeeTaxAmt = KVHeader.ManageFeeTaxAmt;
            KVDetailD.ManageFeeTotalAmt = KVHeader.ManageFeeTotalAmt;
            KVDetailD.Status = "V";
            KVDetailD.UnitPrice = KVHeader.UnitPrice;
            //�s�WDetail InventoryID
            PXResultset<KGSetUp> setupkg = PXSelect<KGSetUp,
               Where<KGSetUp.kGAdditionInventoryID, IsNotNull,
               And<KGSetUp.kGDeductionInventoryID, IsNotNull>>>.Select(this);
            foreach (KGSetUp kgsetup in setupkg)
            {
                KVDetailA.InventoryID = kgsetup.KGAdditionInventoryID;
                //2021/11/03 Modify Add �Y�[�����������N���N�I,�h�u�ƽs���a�P�[�ڪ��u�ƽs���@��
                KVDetailD.InventoryID = kgsetup.KGAdditionInventoryID;
            }
            
            if(checkorCreatetype =="1")
            {
                CalUnBilledAmt calUnBilledAmt = new CalUnBilledAmt();
                if(calUnBilledAmt.TotalUnBilledAmt(this,KVDetailD.OrderNbr)-KVHeader.DeductionAmt<0)
                {

                    return false;
                }
            }
            else if(checkorCreatetype=="2")
            {
                Valuations.Insert(KVHeader);
                DeductionDetails.Insert(KVDetailD);
                AdditionDetails.Insert(KVDetailA);
                return true;
            }

            return true;
        }
        //�R���[���ڳ�
        public void DeleteValuation(KGDailyRenterVendor DRV)
        {
            //KGDailyRenterVendor DRV = RenterVendors.Current;
            PXResultset<KGValuation> SearchDailyRenterID = 
                PXSelect<KGValuation, 
                Where<KGValuation.dailyRenterCD,
                Equal<Required<KGValuation.dailyRenterCD>>>>
                .Select(this, DRV.DailyRenterCD);
            foreach(KGValuation deletevaluation in SearchDailyRenterID )
            {
                if(deletevaluation.DailyRenterCD == DRV.DailyRenterCD)
                {
                    Valuations.Delete(deletevaluation);
                }
                PXResultset<KGValuationDetail> SearchDetail =
                PXSelect<KGValuationDetail,
                Where<KGValuationDetail.valuationID,
                Equal<Required<KGValuationDetail.valuationID>>>>
                .Select(this, deletevaluation.ValuationID);
                foreach(KGValuationDetail deletedetail in SearchDetail)
                {
                    ValuationDetails.Delete(deletedetail);
                }
            }
        }
        //�}�s������Report
        protected virtual void NewPageToDisplayReport(KGRenterConfirmProcess pp)
        {
            //d[ReportMessages.CheckReportFlag] = ReportMessages.CheckReportFlagValue;
            //var requiredException = new PXReportRequiredException(d, "KG601000", PXBaseRedirectException.WindowMode.New, "�t�ӽT�{��");
            KGDailyRenterVendor DRV = RenterVendors.Current;
            if (DRV != null)
            {
                if (DRV.BatchNbr == null)
                {
                    return;
                }
                else
                {
                    Dictionary<string, string> mailParams = new Dictionary<string, string>()
                    {
                        ["BatchNbr"] = DRV.BatchNbr.ToString()
                    };
                    var requiredException = new PXReportRequiredException(mailParams, "KG601000", PXBaseRedirectException.WindowMode.New, "�t�ӽT�{��");
                    requiredException.SeparateWindows = true;
                    //requiredException.AddSibling("KG601000", mailParams);
                    throw new PXRedirectWithReportException(pp, requiredException, "Preview");
                }
                
            }


        }
        //�ˮ�ConfirmQty
        bool setCheckThread = false;
        public bool checkConfirmQty(PXCache sender,KGDailyRenterVendor row)
        {
            KGDailyRenter dailyRenter =
                PXSelect<KGDailyRenter, Where<KGDailyRenter.dailyRenterID, 
                Equal<Required<KGDailyRenter.dailyRenterID>>>>
                .Select(this, row.DailyRenterID);
            //�ˮ֤��i���ŭ�
            if (row.ConfirmQty == null)
            {
                setCheckThread = true;
                RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.confirmQty>(
                        row, row.ConfirmQty, new PXSetPropertyException("���i���ŭ�!"));
                return true;
            }
            //�ˮֽT�{�ƶq���i�j���ڼƶq
            if (dailyRenter.ActQty<row.ConfirmQty)
            {
                setCheckThread = true;
                RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.confirmQty>(
                        row, row.ConfirmQty, new PXSetPropertyException("�T�{�ƶq���i�H�j���ڼƶq"));
                return true;
            }
            return false;
        }
        public bool checkFileLength(PXCache sender,KGDailyRenterVendor row)
        {
            Guid[] files = PXNoteAttribute.GetFileNotes(RenterVendors.Cache, row);
            if (files.Length == 0)
            {
                setCheckThread = true;
                RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
             row, row.Status, new PXSetPropertyException("�Ч��ɮ�!", PXErrorLevel.RowError));
                //throw new PXException("�Ч��ɮ�!");
                return true;
            }
            return false;
        }
        public static PXResultset<POLine> getPOLine(int? lineNbr, String orderNbr)
        {
            PXGraph graph = new PXGraph();

            PXResultset<POLine> set = PXSelect<POLine,
                                               Where<POLine.lineNbr, Equal<Required<POLine.lineNbr>>,
                                                 And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>>>>
                                              .Select(graph, lineNbr, orderNbr);
            return set;
        }
        //2019/11/14�令"�t�ӽT�{"�~�^���N��ƶq�B�ۿ�ƶq�B���B
        public virtual void UpdateDailyRenter(KGDailyRenterVendor dailyRenterVendor,decimal? ConfirmQty)
        {
            KGDailyRenter dailyRenter =
                                            PXSelect<KGDailyRenter,
                                            Where<KGDailyRenter.dailyRenterID,
                                            Equal<Required<KGDailyRenter.dailyRenterID>>>>
                                            .Select(this, dailyRenterVendor.DailyRenterID);
            //�^���N��ƶq=�T�{�ƶq �ۿ�ƶq=��ڼƶq-�N��ƶq
            //2020/04/01 ���i�঳�h��
            dailyRenter.ActInsteadQty = ConfirmQty;
            dailyRenter.ActSelfQty = dailyRenter.ActQty - ConfirmQty;
            KGDailyRenters.Update(dailyRenter);
            base.Persist();
        }
        public override void Persist()
        {
            KGDailyRenterVendor renterVendor = RenterVendors.Current;
            checkConfirmQty(RenterVendors.Cache, renterVendor);
            foreach (KGDailyRenterVendor dailyRenterVendor in RenterVendors.Select())
            {
                if (dailyRenterVendor.Selected == true)
                {
                    if (dailyRenterVendor.Status == "S")
                    {
                        if (dailyRenterVendor.ConfirmQty == 0)
                        {
                            dailyRenterVendor.Status = "C";
                            dailyRenterVendor.Selected = false;
                            RenterVendors.Update(dailyRenterVendor);
                        }
                        else if (dailyRenterVendor.ConfirmQty > 0)
                        {
                            dailyRenterVendor.Selected = false;
                            RenterVendors.Update(dailyRenterVendor);
                        }
                        

                    }
                }
            }
            base.Persist();
        }
        #endregion

        #region Link
        //ValuationCD�W�s��
        public PXAction<RenterFilter> ViewRenter;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "View Daily Renter", Visible = false)]
        protected virtual void viewRenter()
        {
            KGDailyRenterVendor row = RenterVendors.Current;
            // Creating the instance of the graph
            KGDailyRenterActualEntry graph = PXGraph.CreateInstance<KGDailyRenterActualEntry>();
            // Setting the current product for the graph
            graph.DailyRenters.Current = graph.DailyRenters.Search<KGDailyRenter.dailyRenterCD>(
            row.DailyRenterCD);
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.DailyRenters.Current != null)
            {
                throw new PXRedirectRequiredException(graph, true, "DailyRenter Details");
            }


        }
        #endregion
    }

    [Serializable]
    public class RenterFilter : IBqlTable
    {
        #region CreatedByID
        [PXString()]
        [PXUIField(DisplayName = "CreatedByID")]
        [PXSelector(typeof(Search5<Users.username,
             InnerJoin<KGDailyRenterVendor, On<KGDailyRenterVendor.createdByID,
                 Equal<Users.pKID>>>,
             Aggregate<GroupBy<Users.username>>>),
             typeof(KGDailyRenterVendor.createdByID),
             typeof(Users.fullName),
             SubstituteKey = typeof(Users.username))]
        public virtual string CreatedByID { get; set; }
        public abstract class createdByID : IBqlField { }
        #endregion

        #region ContractID
        [PXInt()]
        [PXUIField(DisplayName = "Project")]
        [PXSelector(typeof(Search2<PMProject.contractID,
                LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                Equal<PMProject.contractID>>>>,
                Where<PMProject.baseType, Equal<CTPRType.project>,
                 And<PMProject.nonProject, Equal<False>,
                     And<PMProject.status,Equal<word.a>,
                 And<Match<Current<AccessInfo.userName>>>>>>>)
                , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                typeof(Customer.acctName), typeof(PMProject.status),
                typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : IBqlField { }
        #endregion

        #region OrderNbr
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Order Nbr")]
        [PXSelector(typeof(Search5<POOrder.orderNbr,
              LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>>
                  , LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
              Where<Vendor.bAccountID, IsNotNull,
               And<POLine.projectID, Equal<Current<KGDailyRenter.contractID>>,
                   And<POOrder.orderNbr, NotEqual<Current<KGDailyRenter.orderNbr>>,
                        And<
                            Where2<
                                Where<POOrder.orderType, Equal<POOrderType.regularSubcontract>,
                                    And<Where<POOrder.status, Equal<KGDailyRenterVendor.Open>, Or<POOrder.status, Equal<KGDailyRenterVendor.Complete>>>>>,
                                Or<Where<POOrder.orderType, Equal<POOrderType.regularOrder>,
                                        And<Where<POOrder.status, Equal<KGDailyRenterVendor.Open>, Or<POOrder.status, Equal<KGDailyRenterVendor.Close>, Or<POOrder.status, Equal<KGDailyRenterVendor.Complete>>>>>>>>>>>
                  >,
              Aggregate<GroupBy<POOrder.orderNbr>>>),
            typeof(POOrder.orderNbr),
            typeof(POOrder.orderDesc),
            typeof(POOrder.vendorID),
            typeof(Vendor.acctName),
            typeof(POOrder.orderDate),
            typeof(POOrder.status),
            typeof(Vendor.curyID),
            typeof(POOrder.vendorRefNbr),
            Filterable = true,
            SubstituteKey = typeof(POOrder.orderNbr),
            DescriptionField = typeof(POOrder.orderDesc))]

        /*[PO.RefNbr(
            typeof(Search5<POOrder.orderNbr,
            LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
            And<Match<Vendor, Current<AccessInfo.userName>>>>>,
            Where<POOrder.hold, Equal<False>,
            And2<Where<POOrder.status, Equal<word.n>,
                Or<POOrder.status,Equal<word.c>,
                    Or<POOrder.status,Equal<word.m>>>>,
            And<Where<POOrder.orderType, Equal<word.ro>,
                Or<POOrder.orderType, Equal<word.rs>>>>>>,
            Aggregate<GroupBy<POOrder.orderNbr>>,
            OrderBy<Desc<POOrder.orderNbr>>>),
            DescriptionField = typeof(POOrder.orderDesc)
       )]*/
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : IBqlField { }
        #endregion

        #region VendorID
        [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
        [PXUIField(DisplayName = "Vendor")]
        [PXForeignReference(typeof(Field<RenterFilter.vendorID>.IsRelatedTo<BAccount.bAccountID>))]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : IBqlField { }
        #endregion

        #region WorkDateFrom
        [PXDate()]
        [PXUIField(DisplayName = "Work Date From")]
        public virtual DateTime? WorkDateFrom { get; set; }
        public abstract class workDateFrom : IBqlField { }
        #endregion

        #region WorkDateTo
        [PXDate()]
        [PXUIField(DisplayName = "Work Date To")]
        public virtual DateTime? WorkDateTo { get; set; }
        public abstract class workDateTo : IBqlField { }
        #endregion

        #region Status
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Status")]
        [PXStringList(
                    new string[]
                    {
                      "S", "W","C"

                    },
                    new string[]
                    {
                       "ON SITE", "PENDING SIGN","VENDOR CONFIRM"
        })]
        public virtual string Status { get; set; }
        public abstract class status : IBqlField { }
        #endregion

        

    }

    //�p��Ѿl���p���`���B(PO)
    public class CalUnBilledAmt
    {
        
        //�Ѿl���p���`���B = POLine�j��(���}�o���`���B+�O�d���`���B)+apRegister�h�^�O�d��
        public decimal TotalUnBilledAmt(PXGraph graph, string orderNbr)
        {
            //vTotalUnBilledAmt=�Ѿl���p���`���B
            decimal vTotalUnBilledAmt = 0;

            //�wPONbr��XSum(POLine���}�o���`���B)&Sum(POLine�O�d���`���B)
            PXResultset<POLine> setPoLine = PXSelectGroupBy<POLine,
               Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>>,
               Aggregate<GroupBy<POLine.orderNbr,
               Sum<POLine.curyUnbilledAmt,
               Sum<POLine.curyRetainageAmt>>>>>.Select(graph, orderNbr);
            foreach (POLine pOLine in setPoLine)
            {
                //�קK�ŭ�
                decimal? vCuryUnbilledAmt = pOLine.CuryUnbilledAmt ?? 0;
                decimal? vCuryRetainageAmt = pOLine.CuryRetainageAmt ?? 0;
                 
                vTotalUnBilledAmt = (decimal)(vCuryUnbilledAmt + vCuryRetainageAmt);
            }

            //�wPONbr��X�h�^�O�d���`���B
            APRegister apRegister = PXSelectGroupBy<APRegister,
                Where<APRegisterExt.usrPONbr, Equal<Required<APRegisterExt.usrPONbr>>>,
                Aggregate<GroupBy<APRegisterExt.usrPONbr,
                Sum<APRegister.curyRetainageUnreleasedAmt>>>>
                .Select(graph, orderNbr);
            //�קK�ŭ�
            if (apRegister == null) return vTotalUnBilledAmt;
            decimal? vCuryRetainageUnreleasedAmt = apRegister.CuryRetainageUnreleasedAmt ?? 0;
            vTotalUnBilledAmt = vTotalUnBilledAmt + (decimal)vCuryRetainageUnreleasedAmt;

            return vTotalUnBilledAmt;
        }

        ///2019/11/07 ADD �ˬd���p���`���B�O�_���������`�B
        ///True:�i�H���� False:���i����
        public bool CheckUnbilledAmtEnoughToDeduation(decimal deduationAmt, decimal totalUnBilledAmt)
        {
            if(totalUnBilledAmt - deduationAmt<0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}