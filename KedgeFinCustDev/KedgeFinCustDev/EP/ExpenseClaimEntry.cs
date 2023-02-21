using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.FS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using CC.DAC;
using EP.Util;
using EP.DAC;
using Fin.DAC;
using NM.DAC;
using NM.Util;
using RC.Util;
using Kedge.DAC;
using Kedge.Util;
using RCGV.GV.DAC;
using RCGV.GV.Util;
using static CC.Util.CCList;
using static EP.Util.EPStringList;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Data.BQL;
using static PX.Data.PXImportAttribute;

namespace PX.Objects.EP
{
    /**
     * ======2021/02/08 0011941 Edit By Althea=====
     * EP301000�O�Υӽ� �I�ڤ�k�վ�
     * 	1.�I�ڤ�k��KGBillPayment.PaymentPct�Ч令�i�H��J�p���I���
     * 	2.PaymentPct�[�`�n����100((�w���L
     * 	3.�s�W��J��PaymentPct, �Ц۰ʭp��PaymentAmount
     * 	 ======2021/02/08 0011926 Edit By Althea=====
     * 	 EP301000�O�Υӽеo�������ˮֽվ�
     * 	 1.�o�����ҳ��S����J�]�i�H����
     * 	 2.���~�T�� "�o�����B�P�ӽЪ��B���šI"�·Эקאּ"�o�����Ҫ��B�P�ӽЪ��B���šI"
     * 	 ======2021/02/09 0011925 Edit By Althea=====
     * 	 1.��EPExpenseClaim.DocType='RGU'�ɡG
     * 	 - ����� 12.�p�GUsrDocType = 'RGU', �L�b��, ��sCCReceivableCheck�����A��'AppliedRTN'�A�אּ�G
     * 	 a. ���u����v�ɡA��update CCReceivableCheck.Status=AppliedRTN
     * 	 2.��EPExpenseClaim.DocType='STD'�ɡG
     * 	 �~�n�}�ҵo��&�I�ڤ覡��g
     * 	 ======2021/02/09 0011941 Edit By Althea=====
     * 	 ���ˬd KGBillPayment.PaymentAmount�n����EPExpenseClaim.CuryDocBal
     * 	 ======2021/02/18 0011943 Edit By Althea=====
     * 	 DocType=STD, UsrFinancialYear������,�s���ˮ�docdate &detaildate �~��O�_��UsrFinancialYear������ۦP
     * 	 ======2021/02/24 0011926 Edit By Althea=====
     * 	 Update �o�����Ҫ��B�P�|�B���ˮ�:
     * 	 if header.CuryVatTaxableTotal>0�ܭn�P�_,
     * 	 SUM(SalesAmt) = header.CuryVatTaxableTotal && SUM(TaxAmt) = header.CuryTaxTotal
     * 	 ���~�T���אּ: "�o�����Ҫ����B(�|�B)���šI"
     * 	 ======2021/02/24 0011962 Edit By Althea=====
     * 	 Update Default
     * 	 UsrGuarType/UsrGuarClass �אּ��Graph�gdefault
     * 	 �]�����Pdoctype�U�~�n���w�]
     * 	 
     * ======2021-03-02 :11970 ======Alton
     * ���AP301010�o�����ҽվ�
     * 1.�NGVApGuiInvoice.InvoiceType���o�����ҵe���W, ����.
     * 2.InvoiceType = 'I', �гs�ʱNGVApGuiInvoice.GuiType�]�� '21'
     * 2.InvoiceType = 'I', GVApGuiInvoice.GuiInvoiceNbr���ץ�����10�X, ��X���^��r��, ��K�X���Ʀr
     * 3.InvoiceType = 'R', �гs�ʱNGVApGuiInvoice.GuiType�]�� '27'
     * 4.�p�GInvoiceType = 'R', ���ˬdGVApGuiInvoice.GuiInvoiceNbr�O�_����
     * 5.�p�GInvoiceType = 'R', GVApGuiInvoice.GuiInvoiceNbr���w�]����(�̪����i�W�LDB������40).
     * 6.�бNVendor�令�D����.
     * 7.�NGVApGuiInvoice.VendorName���o�����ҵe���W, ����, free keyin. �p�G�ϥΪ̬D��Vendor, �бNVendor������BAccount.AcctName���GVApGuiInvoice.VendorName.
     * 8.�бNGVApGuiInvoice.VendorUniformNumber�}��s��, �u���J�K�X�Ʀr. ��qVendor�s�ʱa�X�νs�޿褣��, ���O�ϥΪ̥i�H����Jvendor, �ۦ��JGVApGuiInvoice.VendorName��GVApGuiInvoice.VendorUniformNumber
     * 9.�s�ɼg�JGVApGuiInvoice��, �Цh��U�C���, �õ��U�C�w�]��
     *   VoucherCategory : �p�GInvoiceType = 'R', �мg�J "R". �p�GInvoiceType = 'I', �мg�J"I"
     *   DeductionCode : �йw�]�g�J "1"
     *   Remark : �мg�JPRegister.DocDesc
     * 10.GVApGuiInvoice.TaxCode, �p�GInvoiceType = 'R', �мg�J "3". �p�GInvoiceType = 'I', �мg�J"1"
     * 11.�p����g�JGVApGuiInvoice�D�ɮ�, �мg�JStatus��"Hold"
     * 
     * ======2021-03-10 :11970 ======Alton
     * 1.InvoiceType = 'R'�ɡAVendorUniformNumber�אּ�D����A�Ф@�֭ק�AP301000���o������tab
     * 2.GVApGuiInvoice.TaxCode ���GVList.TaxCode
     * 
     * ======2021-03-10 : �N���|======Althea
     * 1.ADD: �N���|Tab DAC:TWNWHT
     * 2.ADD Cache Attached: TWNWHT.EPRefNbr Default Current APInvoice.RefNbr
     * 3.ADD Cache Attached: TWNWHT.UsrPersonalID Default =Current EPExpenseClaim.UsrVendorID.UsrPersonalID
     * 
     * =====2021-03-18 : 11970 ====Alton
     * 1.EP�s�ɮ�, GVApGuiInvoice.totalAmt �мg�JGVApGuiInvoice.SalesAmt+GVApGuiInvoice.TaxAmt
     * 2.EP�s�ɮ�, �мg�JGVApGuiInvoice.Hold = 1
     * 3.��GVApGuiInvoice.InvoiceDate ���ʮ�, �гs�ʲ���GVApGuiInvoice.DeclareYear, GVApGuiInvoice.DeclareMonth
     * 
     * ===2021/03/25 :11992 === Althea
     * 1. Modify �N���|Tab DAC : TWNWHT -> TWNWHTTran
     * 2. Add Persist: �s�ɮ�,EPDetails�ˬd�O�_����ValuationType,�Y�L�h�s�W�@��,�Y����sAmt�Y�i
     * 3. Add ��TWNWHTTran��sWHTAmt,�p��sum��header��trantype�ϧO��amt���
     * 
     * ===2021/03/26 ===Althea
     * 1. CostCodeID ��M�ת������w��̪�
     * 2. �Ydetails ��usrValuationType != B ,�h���ˬd�w��
     * 
     * ===2021/03/29 :0011988 === Althea
     * Add EPPaymentModiReq TT/NMR/NMP/CCR/CCP View
     * 
     * ====2021-04-16 : 11926 === Alton
     * �o�����Ҹ�ƥ[�`�ˮ֭קאּ�GGVApGuiInvoiceFinRef.TaxAmt>0�ɡA�~�ݭn�N�������SalesAmt�@�_�[�`�B�ˬd�O�_����CuryVatTaxableTotal
     * 
     * ====2021-04-20 : 12016 ====Alton
     * 1.KGBillPayment�s�WVendorID���, int
     * 2.�w�]��APRegister/EPExpenseClaim���Y��VendorID
     * 3.�ϥΪ̲��ʮ�, �гs�ʧ���VendorLocationID��lov, �ña�JVendorID������AcctName��CheckTitle
     * 
     * ====2021-04-21 : 12015 ===Alton
     * 1. KGBillPayment�s�W������A����ܦb�e���W�G
     *     (1) PostageAmt, Decimal (18,6) :
     *               a. �Ч� KGBillPayment.PaymentAmount ����Base���p����B
     *               b. �p���޿��follow KG�ǲ��K�n�]�w(KG105001)�����u�l��O�]�w�vGroup
     *     (2) ActPayAmt, Decimal (18,6):
     *                 a. ActPayAmt = KGBillPayment.PaymentAmount- PostageAmt
     * 2. ��KGBillPayment.PaymentMethod= TT or CHECK�ɡA�~�ݭn�p��item 1��������A
     *       ��L�I�ڤ覡�Ъ�����s�B���ݰ��B��
     * 
     * ===2021/04/22 Mantis: 0012029 ===Althea
     * Add Log:
     * ��XUsrApprovalLevelID,�a�X����KGBudApprovalLevel��
     * 1) CreditAccountID 2) CreditSubID 3) InventoryID
     * 
     * ====2021-04-26 : 11974====Alton
     * �L�b������AEP�o����T�ܰ�Ū�A�ݭn�ק�Цܶi���o���D��
     * 
     * ====2021-04-27 : 12015====Alton
     * 1.�l��O�p���ΥI�ڤ覡�P�_�Y������
     * 2.KGBillpayment add orderby
     * 3.GVInvoice���B�ˮ֭ץ�
     * 
     * ====2021-04-28 : �f�Y=====Alton
     * EP���p��KGBillpayment�l��O
     * 
     * ====2021-05-06 : ====Alton
     * 1.���YUsrReturnUrl read only
     * 2.���YUsrFinancialYear LOV �����Ƨ�
     * 
     * ====2021-05-07:====Alton
     * 1.�K�[�l��O�p��
     * 2.�����o�����B�ˮ�(CheckGVAmt)
     * 
     * ===2021/05/11 :�f�Y===Althea
     * VendorLocation Selector Add LocationFinExt.usrIsReserveAcct to Show
     * 
     * ===2021/05/13 :Tiffany�f�Y===Althea
     * Fix �M���ˬd�w��:�p�G�������OSTD �N���n�ˬd�M�׹w��
     * Fix �Y����������n�M�Ź���Tab��T
     * 
     * ===2021/05/18 :Tiffany�f�Y===Althea
     * ���O�Ҳ�Tab��,�M�׿粒���a�X�Ĥ@�����������Ȫ��p���椸
     * 
     * ===2021-05-19 :12045===Alton
     * 	1.�o�����Ҫ��|�B�}��ϥΪ̥i�H�L��
     *  2.�ϥΪ̿�J�o�����ҥ��|���B,�t�Φ۰ʭp��5%�|�B, ���ϥΪ̥i�H�����L�յ|�B
     *  
     *  ===2021/05/20 :0012048 ===Althea
     *  KGBillPayment.VendorLocationID LOV�e���бNUsrIsReserveAcct, 
     * �Щ�b�Ĥ@�����.LOV Order by UsrIsReserveAcct(����), LocationCD(�ɾ�), �������v�᪺LocationLID�b�Ĥ@��
     * 
     * ====2021-05-24:12053====Alton
     * 1. KGBillPayment, �s�W�@����� IsPostageFree, bits, �w�]True
     * 2. �I�ڤ覡�e���s�W�@�����IsPostageFree, ����b�l��O����
     * 3. �ϥΪ̱NIsPostageFree�Ŀﲾ��, �бN���O�k0, �í��s�p��ActPayAmt
     * 
     * ===2021/05/31 :0012065 ===Althea
     * CC����Tab �g����W��View
     * 
     * ====2021-06-08:�f�Y ====Alton
     * ��]�G�ϥΪ̤������ǿ�J���ȡA�b����s�s�s���|�B�Q�\���F
     * �ץ��G
     *  1.InvoiceType���ʮɤ�Ĳ�otaxAmt�p��
     *  2.InvoiceType = ����(R)�ATaxCode = ���|(TAXABLE)
     *  3.�}��������O�A�|�����O��쵹�L�̧�
     *  
     *  ===2021/06/09 :�f�Y===Althea
     *  �]���N���|�ﰵ�b�s��,�t�Υ�����KGBillPayment�A�h��N����ƨ�EPDetails,�ɭP���B����
     *  �ҥH��KGBillPayment�ˮ֧��WHT��graph �����ʧ��A�ˮ�
     *  
     *  ===2021/06/11 : 0012089 ===Althea
     *  Header��UsrFinPeroid �[�Wdefault �t�Φ~
     *  DocType = GUR(�O�Ҳ�),CuryUnitCost������
     *  
     *  ==2021/06/24 : 0012105 === Althea
     *  PaymentMethod Add E:Auth(�¦�)
     *  VendorLocation & BankAccountID Selector Add Filter :Same withe Cash
     *  Default Auth same with Cash
     *  
     *  ===2021/07/07 :0012128 === Althea
     *  UsrVendorID���ʮɱa�XvendorLocation ���defLocationID
     *  
     *  ===2021/07/09 :0012128 === Althea
     *  Add KGBillPayment . PaymentMethod Default
     *  PaymentMethod=���YUsrVendorLocationID�������I�ڤ�k(VPaymentMethodID, Check�h�䲼, TT�h�q��, COUPON�h§��, ��L�Ҭ��{��)
     *  
     *  ===2021-07-13:12147:====Alton
     *  1.�бN�I�ڤ覡����PaymentDate���e��, ��m�b��I���B�᭱.
     *  2.�w�]��EPExpenseClaim.DocDate + KGBillPayment.PaymentPeriod
     *  3.��KGBillPayment.PricingType='�@��p��'��, �ϥΪ̿�JKGBillPayment.PaymentPeriod �гs�ʧ�sPaymentDate(EPExpenseClaim.DocDate + KGBillPayment.PaymentPeriod). ��KGBillPayment.PricingType='��L-�����p�I�ڤ�'��, �N���p�ʧ�sPaymentDate
     *  4.��ϥΪ̹L�b�дڽдڳ��, �p����ݭn�P�_KGBillPayment.PricingType, �p�GKGBillPayment.PricingType='�@��p��'��, �~�̷ӥثe�p���檺�޿��sPaymentDate
     *  
     *  ===2021/07/13 :0011824 ===althea
     *  �ˬd�w����B�쥻�b�s���ˬd �אּSubmit Action
     *  ����ˬd�S�M�׹w�⪺�޿�
     *  �אּ��view
     *  
     *  ===2021/07/13 :0012124 ===Althea
     *  EPDetails UsrApprovalLevelID Seletor Add LevelDesc ��X�Ҧ��W��
     *  
     *  ===2021/07/15 :0012156 ===Althea
     *  OTH �令 CHG
     *  OTH ���}EPDetails & �I�ڤ覡 & �o��
     *  OTH ���ݭn�ˬd�I�ڤ覡���S����
     *  
     *  ====2021-08-02:12179====Alton
     *  1.KGBillpayment.paymentAmount���w�]�ȥu���bPaymentPct���ʮɭ��s�p��ӵ����B
     *  2.���������PaymentAmount���u�ʭp��
     *  
     *  ===2021/08/02 :0012176 ===Althea
     *  Add PaymentMethod = F same with Cash
     *  
     *  ===2021/08/04 :0012180 ===Althea
     *  Add UsrDocType = STD , ContractID/CostcodeID/ TaskID ,Readonly = True
     *  
     *  ===2021/08/05 :Louis Line ===Althea
     *  ���ޱ� �A�Q��--> Add UsrDocType = STD , ContractID/CostcodeID/ TaskID ,Readonly = True
     *  
     *  ===2021/08/11 :0012192 === Althea
     *  Add EPPaymentModifReq NMP & NMR Default �s��
     *  
     *  ===2021/08/13 :0012197 === Althea
     *  InventoryID Selector Modify:
     *  ��ӽ��������ǲ��@�~�Ψ�L�ӽ�
     *  ��selector�i�H�D��inventoryItem = �O�� �H�~��
     *  
     *  ===2021/08/13 :0012199 === Althea
     *  1. �Y��ʥӽ�����, details: usrCreditAmt = 0
     *  2.�Y���CuryTranAmtWithTaxes, CuryTranAmtWithTaxes>0 -->update usrCreditAmt = 0
     *  3.�Y���usrCreditAmt, usrCreditAmt >0 --> update QTY = 0  & update CuryTranAmtWithTaxes =0
     *  4.�s���ˮ�: details: usrCreditAmt > 0, �ˬd : UsrCreditAccountID/UsrCreditSubID���ର��
     *  5.�s���ˮ�: header: �ˬd : �[�`details CuryTranAmtWithTaxes =  �[�`details usrCreditAmt
     *  
     *  ===2021/08/13 :0012200 === Althea
     *  �]�ȩ���Tab add Group��LINK TO GL:
     *  ��ܡ��ǲ��@�~�����ͪ�GL Batch������T
     *  
     *  == 2021/08/19 :0012204 === Althea
     *  ����: GLB �W�ߥX�@��Tab����
     *  ��������GLB��EPDetails�s���ˮ�
     *  
     *  === 2021/08/20 :0012204 === Althea
     *  ������GLB��,�ˮ�:
     *  1. header: GLBView ��sum(CreditAmt)=sum(DebitAmt)
     *  2. details: GLBView �� CreditAmt & DebitAmt ����>0
     *  Add CreditAmt & DebitAmt Update:
     *  1.��CreditAmt update >0, DebitAmt = 0
     *  2.��DebitAmt update>0, ��CreditAmt =0
     *  
     *  ===2021/08/24 :0012204 === Althea
     *  �W�z�Ĥ@�I��촣����ˮ�
     *  
     *  ===2021/09/01 :0012218 === Althea
     *  ExpenseAccount Updated Add:
     *  if(DocType = STD & (ExpenseAccount Type In(Asset,Liability)) ) 
     *  Update ExpenseSub = 9999-999
     *  
     *  ===2021/10/19 :0012256 === Althea
     *  Add DocType = BTN:
     *  1. ���}��EPDetails��Tab�ϥ�
     *  2. Header : UsrCuryDocBal Visible = False
     *  3. Details :   
     *  �ϥΪ̬D�粒EPExpenseClaimDetails.UsrBankAccountID,
     *  �бNNMBankAccount����CashAccount��
     *  CashAccount.AccountID = ExpensseDetails.ExpenseAccountID
     *  CashAccount.SubID =ExpensseDetails.ExpenseSubID
     *  
     *  ===2021/11/02 : 0012256 === Althea
     *  DocType = BTN
     *  �s�ɮ�,�Y�ɶU��������dialog�����߰ݽT�w�n�s��?
     *  �����,�Y�ɶU�������Xerror
     *  
     *  ===2021/12/08 : 0012274 === Althea
     *  �ǲ��@�~ detail tab �бN�u�W�ǡv�\��}��
     *  
     *  ===2022/04/14===Jeff
     *  Mantis [0012303] - 0025625 
     *  Move the logic from Submit to Release.
     **/

    public class ExpenseClaimEntry_Extension2 : PXGraphExtension<ExpenseClaimEntry>, IPXPrepareItems
    {
        #region Gobal Variables
        private bool runable = false;
        private bool activWHT = false;
        #endregion

        #region View
        [PXCopyPasteHiddenView]
        public PXSelect<EPExpenseClaimDetails,
            Where<EPExpenseClaimDetails.refNbr, Equal<Current<EPExpenseClaimDetails.refNbr>>>> CCView;

        public PXSelect<GVApGuiInvoiceFinRef, Where<GVApGuiInvoiceFinRef.ePRefNbr, Equal<Current<EPExpenseClaim.refNbr>>>> GVApGuiInvoiceRefs;

        public PXSelect<KGBillPayment,
            Where<KGBillPaymentExt.usrEPRefNbr, Equal<Current<EPExpenseClaim.refNbr>>>,
            OrderBy<Asc<KGBillPayment.createdDateTime>>> KGBillPayments;

        public PXFilter<EPVendorUpdateAsk> VendorUpdateAsk;

        //2021/03/29 Add DAC EPPaymentModiReq
        // public PXFilter<EPPaymentModiReqCount> ReqCount;

        public PXSelect<EPPaymentModiReq,
            Where<EPPaymentModiReq.epRefNbr, Equal<Current<EPExpenseClaim.refNbr>>>> Req;

        public PXSelect<EPPaymentModiReqTT,
            Where<EPPaymentModiReqTT.epRefNbr, Equal<Current<EPExpenseClaim.refNbr>>,
                And<EPPaymentModiReqTT.paymentType, Equal<EPStringList.PaymentType.tt>>>> ReqTT;
        public PXSelect<EPPaymentModiReqNMR,
            Where<EPPaymentModiReqNMR.epRefNbr, Equal<Current<EPExpenseClaim.refNbr>>,
                And<EPPaymentModiReqNMR.paymentType, Equal<EPStringList.PaymentType.nmr>>>> ReqNMR;
        public PXSelect<EPPaymentModiReqNMP,
            Where<EPPaymentModiReqNMP.epRefNbr, Equal<Current<EPExpenseClaim.refNbr>>,
                And<EPPaymentModiReqNMP.paymentType, Equal<EPStringList.PaymentType.nmp>>>> ReqNMP;
        public PXSelect<EPPaymentModiReqCCR,
            Where<EPPaymentModiReqCCR.epRefNbr, Equal<Current<EPExpenseClaim.refNbr>>,
                And<EPPaymentModiReqCCR.paymentType, Equal<EPStringList.PaymentType.ccr>>>> ReqCCR;
        public PXSelect<EPPaymentModiReqCCP,
             Where<EPPaymentModiReqCCP.epRefNbr, Equal<Current<EPExpenseClaim.refNbr>>,
                And<EPPaymentModiReqCCP.paymentType, Equal<EPStringList.PaymentType.ccp>>>> ReqCCP;
        //2021/08/13 Add Mantis: 0012200
        public PXSelectJoinGroupBy<Batch,
            LeftJoin<GLTran, On<GLTran.batchNbr, Equal<Batch.batchNbr>>>,
            Where<GLTran.refNbr, Equal<Current<EPExpenseClaim.refNbr>>>,
            Aggregate<GroupBy<Batch.batchNbr>>> LinkToGL;

        //2021/08/19 Add Mantis: 0012204
        //2021/12/08 Add Mantis: 0012274
        [PXImport]
        public PXSelect<KGExpenseVoucher,
            Where<KGExpenseVoucher.epRefNbr, Equal<Current<EPExpenseClaim.refNbr>>>> GLBView;

        public PXSelectGroupBy<EPExpenseClaimDetails,
            Where<EPExpenseClaimDetails.refNbr, Equal<Current<EPExpenseClaim.refNbr>>,
                And<EPExpenseClaimDetailsExt.usrBankAccountID, IsNotNull>>,
            Aggregate<GroupBy<EPExpenseClaimDetailsExt.usrBankAccountID,
                Sum<EPExpenseClaimDetails.curyTranAmtWithTaxes>>>> ViewBankTxnAmt;

        public PXFilter<EditTempTable> EditAsk;
        #endregion

        #region Override Methods
        public override void Initialize()
        {
            if (!RCFeaturesSetUtil.IsActive(Base, RCFeaturesSetProperties.EXPENSE_CLAIM))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }

            runable = RCFeaturesSetUtil.IsActive(Base);
            activWHT = RCFeaturesSetUtil.IsActiveOP(Base, RCFeaturesSetProperties.WITHHOLDING_TAXES);

            Base.action.AddMenuAction(acctApprove, nameof(Base.Release), false);
        }
        #endregion

        #region Actions
        public PXAction<EPExpenseClaim> acctApprove;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "�S�\�֭�", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable AcctApprove(PXAdapter adapter)
        {
            ///<remarks>Mantis [0012309] - 0025568 #1</remarks>
            Base.ExpenseClaimCurrent.Cache.SetValue<EPExpenseClaimExt.usrApprovalStatus>(Base.ExpenseClaimCurrent.Current,
                                                                                         EPUsrApprovalStatus.AcctApproved);
            Base.ExpenseClaimCurrent.UpdateCurrent();
            Base.Save.Press();

            return adapter.Get();
        }

        public PXAction<EPExpenseClaim> batchReport;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "�ǲ����ӹw��", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable BatchReport(PXAdapter adapter)
        {
            var current = Base.ExpenseClaim.Current;

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["RefNbr_S"] = current.RefNbr,
                ["RefNbr_E"] = current.RefNbr,
                ["DocDate_S"] = current.DocDate?.ToString("yyyy/MM/dd"),
                ["DocDate_E"] = current.DocDate?.ToString("yyyy/MM/dd"),
                ["CreateBy"] = null
            };

            throw new PXReportRequiredException(parameters, "KG606004", PXBaseRedirectException.WindowMode.New, null);
        }

        public PXAction<EPExpenseClaim> editReturnDate;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "�ק�h�^���", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable EditReturnDate(PXAdapter adapter)
        {
            if (CCView.Current == null) return adapter.Get(); ;
            if (EditAsk.AskExt() == WebDialogResult.OK)
            {
                CCView.Cache.SetValueExt<EPExpenseClaimDetailsExt.usrReturnDateCCR>(CCView.Current, EditAsk.Current.ReturnDate);
                CCView.UpdateCurrent();
            }
            EditAsk.Cache.Clear();
            return adapter.Get();
        }
        #endregion

        #region Event Handlers

        #region EPExpenseClaim
        protected virtual void _(Events.FieldUpdated<EPExpenseClaim, EPExpenseClaimExt.usrDocType> e)
        {
            EPExpenseClaim row = (EPExpenseClaim)e.Row;
            if (row == null) return;
            EPExpenseClaimExt rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(row);

            if (rowExt.UsrDocType == EPStringList.EPDocType.STD ||
                rowExt.UsrDocType == EPStringList.EPDocType.GLB ||
                rowExt.UsrDocType == EPStringList.EPDocType.CHG)
                rowExt.UsrTargetType = "V";

            //�Y�����@��дڤ��ݭn��g�o��&�I�ڤ覡
            if (rowExt.UsrDocType != EPStringList.EPDocType.STD)
            {
                EPExpenseClaimDetails details = Base.ExpenseClaimDetails.Select();
                if (details != null)
                {
                    foreach (EPExpenseClaimDetails claimDetails in Base.ExpenseClaimDetails.Select())
                    {
                        Base.ExpenseClaimDetails.Delete(claimDetails);
                    }
                }
                KGBillPayment billPayment = KGBillPayments.Select();
                if (billPayment != null)
                {
                    foreach (KGBillPayment payment in KGBillPayments.Select())
                    {
                        KGBillPayments.Delete(payment);
                    }
                }
                GVApGuiInvoiceFinRef invoiceFinRef = GVApGuiInvoiceRefs.Select();
                if (invoiceFinRef != null)
                {
                    foreach (GVApGuiInvoiceFinRef guiInvoiceFinRef in GVApGuiInvoiceRefs.Select())
                    {
                        GVApGuiInvoiceRefs.Delete(guiInvoiceFinRef);
                    }
                }

            }
            //2021/05/13 Add 
            //�Y������L�ܧ�,���ݭn��g��L���I�ܧ�
            if (rowExt.UsrDocType != EPStringList.EPDocType.CHG)
            {
                EPPaymentModiReqTT reqTT = ReqTT.Select();
                if (reqTT != null)
                {
                    foreach (EPPaymentModiReqTT tt in ReqTT.Select())
                    {
                        ReqTT.Delete(tt);
                    }
                }
                EPPaymentModiReqCCP reqCCP = ReqCCP.Select();
                if (reqCCP != null)
                {
                    foreach (EPPaymentModiReqCCP ccp in ReqCCP.Select())
                    {
                        ReqCCP.Delete(ccp);
                    }
                }
                EPPaymentModiReqCCR reqCCR = ReqCCR.Select();
                if (reqCCR != null)
                {
                    foreach (EPPaymentModiReqCCR ccr in ReqCCR.Select())
                    {
                        ReqCCR.Delete(ccr);
                    }
                }
                EPPaymentModiReqNMP reqNMP = ReqNMP.Select();
                if (reqNMP != null)
                {
                    foreach (EPPaymentModiReqNMP nmp in ReqNMP.Select())
                    {
                        ReqNMP.Delete(nmp);
                    }
                }
                EPPaymentModiReqNMR reqNMR = ReqNMR.Select();
                if (reqNMR != null)
                {
                    foreach (EPPaymentModiReqNMR nmr in ReqNMR.Select())
                    {
                        ReqNMR.Delete(nmr);
                    }
                }
            }

            //�Y�������I�O�Ҳ���ú�^�O�Ҳ�,�M�ūO�Ҳ�����T
            if (rowExt.UsrDocType != EPStringList.EPDocType.GUR ||
                rowExt.UsrDocType != EPStringList.EPDocType.RGU)
            {
                EPExpenseClaimDetails details = CCView.Select();
                if (details != null)
                {
                    foreach (EPExpenseClaimDetails claimDetails in CCView.Select())
                    {
                        EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(claimDetails);
                        detailsExt.UsrGuarClass = null;
                        detailsExt.UsrGuarType = null;
                        detailsExt.UsrPONbr = null;
                        detailsExt.UsrDueDate = null;
                        detailsExt.UsrAuthDate = null;
                        detailsExt.UsrIssueDate = null;
                        detailsExt.UsrGuarReceviableCD = null;
                        Base.ExpenseClaimDetails.Update(claimDetails);
                    }
                }
            }

            //�Y�����ǲ��ӽ�,�M�Ŷǲ��ӽЪ���T
            if (rowExt.UsrDocType != EPStringList.EPDocType.GLB)
            {
                KGExpenseVoucher glbView = GLBView.Select();
                if (glbView != null)
                {
                    foreach (KGExpenseVoucher epVoucher in GLBView.Select())
                    {
                        GLBView.Delete(epVoucher);
                    }
                }
            }

            #region 2021/8/19 Delete Mantis: 0012204
            //2021/08/13 Add Mantis: 0012199
            //�Y�����ǲ��@�~,�M�����
            /*
            if (rowExt.UsrDocType !=EPStringList.EPDocType.GLB)
            {
                foreach(EPExpenseClaimDetails details in Base.ExpenseClaimDetails.Select())
                {
                    EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
                    detailsExt.UsrCreditAmt = 0;
                }
            }
            */
            #endregion
        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaim, EPExpenseClaimExt.usrVendorID> e)
        {
            EPExpenseClaim row = (EPExpenseClaim)e.Row;
            if (row == null) return;
            EPExpenseClaimExt rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(row);

            if (rowExt.UsrVendorID == null)
                rowExt.UsrVendorLocationID = null;
            else
            {
                //2021/07/07 Add Mantis: 0012128 �אּDefault Location
                Location location = GetDefLocation(rowExt.UsrVendorID);
                rowExt.UsrVendorLocationID = location.LocationID;
                //cache.SetValue<EPExpenseClaimExt.usrVendorLocationID>(rowExt, location.LocationID);
            }
        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaim, EPExpenseClaimExt.usrTargetType> e)
        {
            EPExpenseClaim row = e.Row as EPExpenseClaim;
            if (row == null) return;
            EPExpenseClaimExt rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(row);
            if (rowExt.UsrTargetType == "C")
            {
                if (rowExt.UsrVendorID != null || rowExt.UsrVendorLocationID != null)
                {
                    rowExt.UsrVendorID = null;
                    rowExt.UsrVendorLocationID = null;
                }
            }
            else
            {
                if (row.CustomerID != null || row.CustomerLocationID != null)
                {
                    row.CustomerID = null;
                    row.CustomerLocationID = null;
                }
            }

        }
        protected virtual void _(Events.RowSelected<EPExpenseClaim> e)
        {
            EPExpenseClaim row = e.Row as EPExpenseClaim;
            if (row == null) return;
            EPExpenseClaimExt rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(row);
            setEnable(row.Status, row);
            setEPPayModi(row);
            SetCCViewEnable(row);
            SetUsrValuationTypeList();
        }

        protected virtual void _(Events.RowPersisting<EPExpenseClaim> e)
        {
            EPExpenseClaim row = e.Row as EPExpenseClaim;
            if (row == null) return;
            EPExpenseClaimExt rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(row);

            //2021/02/18 Mantis: 0011943
            //DocDate Check FinYear
            //2021/12/13 Mantis: 0012266
            //Check if UsrisTempWriteoff, check details just have one row
            if (rowExt.UsrDocType == EPStringList.EPDocType.STD)
            {
                checkFinYear(rowExt.UsrFinancialYear, row.DocDate);
                if (rowExt.UsrIsTmpPayment ?? false)
                    checkTempEPDetails();
            }

            if (rowExt.UsrDocType == EPStringList.EPDocType.GUR)
            {
                if (row.CustomerID == null && rowExt.UsrVendorID == null)
                {
                    throw new Exception("�ж�g��H��T!");
                }
            }
            if (rowExt.UsrApprovalStatus == EPUsrApprovalStatus.Rejected
                && row.Status.Equals(EPExpenseClaimStatus.HoldStatus))
            {
                rowExt.UsrApprovalStatus = EPUsrApprovalStatus.Hold;
            }
            //2021/11/02 Mantis: 0012256 Ask �ɶU�����T�w�n�s�ɶ�
            //2021/10/22 Mantis: 0012256
            //Check Amt = 0
            //20220808���ĥ�����ˬd�ɶU������
            /**
            if (rowExt.UsrDocType == EPStringList.EPDocType.BTN)
            {
                if (row.CuryDocBal != 0)
                {
                    WebDialogResult result = Base.ExpenseClaim.Ask(ActionsMessages.Warning,
                   PXMessages.LocalizeFormatNoPrefix("�ثe��ڤ����ɶU���������A�A�T�w�n�s�ɶܡH"),
                   MessageButtons.OKCancel, MessageIcon.Warning, true);
                    //checking answer	
                    if (result != WebDialogResult.OK)
                        return;
                }

            }**/

            #region Delete
            //2021/08/24 Mantis: 0012204
            //Move to Submit Check
            //2021/08/20 Mantis: 0012204
            //When EPDocType = GLB, Check Amy Balance
            /*
            if (rowExt.UsrDocType == EPStringList.EPDocType.GLB)
                CheckGLBAmtBalance();
            */
            //2021/08/19 Delete Mantis: 0012204
            //2021/08/13 Add Mantis: 0012199

            /*
            if (rowExt.UsrDocType == EPStringList.EPDocType.GLB)
            {
                decimal? CreditAmt = 0;
                decimal? CuryTranAmtwithTaxes = 0;
                if (Base.ExpenseClaimDetails.Select().Count != 0)
                {
                    foreach (EPExpenseClaimDetails details in Base.ExpenseClaimDetails.Select())
                    {
                        EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
                        CreditAmt = CreditAmt + detailsExt.UsrCreditAmt;
                        CuryTranAmtwithTaxes = CuryTranAmtwithTaxes + details.CuryTranAmtWithTaxes;
                    }
                    if(CuryTranAmtwithTaxes != CreditAmt)
                    {
                        throw new Exception("�ɶU������, �нT�{!");
                    }

                }
            }
            */
            #endregion
        }
        protected virtual void _(Events.FieldDefaulting<EPExpenseClaim, EPExpenseClaimExt.usrFinancialYear> e)
        {
            EPExpenseClaim row = e.Row;
            if (row == null) return;
            EPExpenseClaimExt rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(row);
            rowExt.UsrFinancialYear = ((DateTime)Base.Accessinfo.BusinessDate).ToString("yyyy");

        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaim, EPExpenseClaim.docDate> e)
        {
            EPExpenseClaim row = e.Row;
            if (row == null) return;
            foreach (KGBillPayment item in KGBillPayments.Select())
            {
                KGBillPayment _item = item;
                SetPaymentDate(row, ref _item);
                KGBillPayments.Update(_item);
            }
        }

        protected virtual void _(Events.FieldSelecting<EPExpenseClaimExt.usrStageCode> e)
        {
            var row = e.Row as EPExpenseClaim;

            if (row != null && row.GetExtension<EPExpenseClaimExt>()?.UsrDocType == EPDocType.GLB)
            {
                List<string> allowedValues = new List<string>();
                List<string> allowedLabels = new List<string>();

                foreach (SegmentValue segValue in SelectFrom<SegmentValue>.Where<SegmentValue.dimensionID.IsEqual<@P.AsString>>.View.Select(Base, "KGSTAGECODE"))
                {
                    allowedValues.Add(segValue.Value);
                    allowedLabels.Add(segValue.Descr);
                }

                e.ReturnState = PXStringState.CreateInstance(e.ReturnState, 2, true, nameof(SegmentValue.value), false, -1, string.Empty, allowedValues.ToArray(), allowedLabels.ToArray(), false, null);

                //((PXStringState)e.ReturnState).MultiSelect = false;
            }
        }
        #endregion

        #region EPExpenseClaimDetails
        //���F���UsrApproveLevel �� levelDesc
        public void _(Events.FieldSelecting<KGBudApproveLevel, KGBudApproveLevel.levelDesc> e)
        {
            if (e.Row == null) return;
            KGBudApproveLevel row = e.Row;
            KGBudApproveName name = PXSelect<KGBudApproveName, Where<KGBudApproveName.branch, Equal<Required<KGBudApproveName.branch>>>>.Select(Base, row.BranchID);
            String desc = "";
            if (row.Level1 == true) desc += ((name?.Stage1Name ?? "") + "-");
            if (row.Level2 == true) desc += ((name?.Stage2Name ?? "") + "-");
            if (row.Level3 == true) desc += ((name?.Stage3Name ?? "") + "-");
            if (row.Level4 == true) desc += ((name?.Stage4Name ?? "") + "-");
            if (row.Level5 == true) desc += ((name?.Stage5Name ?? "") + "-");
            if (row.Level6 == true) desc += ((name?.Stage6Name ?? "") + "-");
            if (row.Level7 == true) desc += ((name?.Stage7Name ?? "") + "-");
            if (row.Level8 == true) desc += ((name?.Stage8Name ?? "") + "-");
            if (row.Level9 == true) desc += ((name?.Stage9Name ?? "") + "-");
            if (row.Level10 == true) desc += ((name?.Stage10Name ?? "") + "-");
            if (row.Level11 == true) desc += ((name?.Stage11Name ?? "") + "-");
            if (row.Level12 == true) desc += ((name?.Stage12Name ?? "") + "-");
            if (row.Level13 == true) desc += ((name?.Stage13Name ?? "") + "-");
            if (row.Level14 == true) desc += ((name?.Stage14Name ?? "") + "-");
            if (row.Level15 == true) desc += ((name?.Stage15Name ?? "") + "-");
            if (row.Level16 == true) desc += ((name?.Stage16Name ?? "") + "-");
            if (row.Level17 == true) desc += ((name?.Stage17Name ?? "") + "-");
            if (row.Level18 == true) desc += ((name?.Stage18Name ?? "") + "-");
            if (row.Level19 == true) desc += ((name?.Stage19Name ?? "") + "-");
            if (row.Level20 == true) desc += ((name?.Stage20Name ?? "") + "-");
            if (String.IsNullOrWhiteSpace(desc)) return;
            e.ReturnValue = desc.Substring(0, desc.Length - 1);

        }
        protected virtual void _(Events.RowSelected<EPExpenseClaimDetails> e)
        {
            EPExpenseClaimDetails row = e.Row as EPExpenseClaimDetails;
            if (row == null) return;
            EPExpenseClaim header = Base.ExpenseClaimCurrent.Current;
            if (header == null) return;

            setDetailUI(row, header);
            CCView.Cache.SetDefaultExt<EPExpenseClaimDetailsExt.usrGuarReceviableBatchNbr>(row);
        }
        protected virtual void EPExpenseClaimDetails_UsrGuarType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            EPExpenseClaimDetails details = (EPExpenseClaimDetails)e.Row;
            if (details == null) return;
            EPExpenseClaim header = Base.ExpenseClaim.Current;
            if (header == null) return;
            EPExpenseClaimExt headerExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(header);
            if (headerExt.UsrDocType == EPDocType.GUR || headerExt.UsrDocType == EPDocType.RGU)
            {
                e.NewValue = GuarTypeList.Other;
            }
            else
            {
                e.NewValue = null;
            }

        }
        protected virtual void EPExpenseClaimDetails_UsrGuarClass_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            EPExpenseClaimDetails details = (EPExpenseClaimDetails)e.Row;
            if (details == null) return;
            EPExpenseClaim header = Base.ExpenseClaim.Current;
            if (header == null) return;
            EPExpenseClaimExt headerExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(header);
            if (headerExt.UsrDocType == EPDocType.GUR || headerExt.UsrDocType == EPDocType.RGU)
            {
                e.NewValue = GuarClassList.CommercialPaper;
            }
            else
            {
                e.NewValue = null;
            }

        }
        protected virtual void EPExpenseClaimDetails_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPExpenseClaimDetails details = (EPExpenseClaimDetails)e.Row;
            if (details == null) return;
            if (details.InventoryID != null)
            {
                InventoryItem inventory = GetInventory(details.InventoryID);
                details.TranDesc = inventory.Descr;
            }
            else
            {
                details.TranDesc = null;
            }
        }
        protected virtual void EPExpenseClaimDetails_ContractID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPExpenseClaimDetails details = (EPExpenseClaimDetails)e.Row;
            if (details == null) return;
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            if (claim == null) return;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            //2021/05/13 Modify �Y��������STD�h�����ˬd
            if (claimExt.UsrDocType == EPStringList.EPDocType.STD)
            {
                if (details.ContractID != 0)
                {
                    KGSetUp kGSetUp = GetKGSetUp();
                    if (kGSetUp == null) throw new Exception("�Ч���KG���n�]�w!");
                    KGSetUpExt kGSetUpExt = PXCache<KGSetUp>.GetExtension<KGSetUpExt>(kGSetUp);
                    if (kGSetUpExt.UsrKGPrjManageInventoryID == null) throw new Exception("�Ц�KG���n�]�w��g�M�׺޲z�u�ƽs��!");

                    PMCostBudget costBudget = GetPMCostBudget(details.ContractID, kGSetUpExt.UsrKGPrjManageInventoryID);
                    if (costBudget == null)
                        throw new Exception("�M�ץ��s�C�޲z�O�ιw��");

                    details.TaskID = costBudget.TaskID;
                    details.CostCodeID = costBudget.CostCodeID;
                    InventoryItem inventory = GetInventory(kGSetUpExt.UsrKGPrjManageInventoryID);
                    details.ExpenseAccountID = inventory.COGSAcctID;
                    details.ExpenseSubID = inventory.COGSSubID;

                }
            }
            else
            {
                PMCostBudget costBudget = GetPMCostBudget(details.ContractID);
                details.TaskID = costBudget?.TaskID;
                details.CostCodeID = costBudget?.CostCodeID;
            }
        }
        protected virtual void EPExpenseClaimDetails_UsrGuarReceviableCD_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPExpenseClaimDetails details = (EPExpenseClaimDetails)e.Row;
            if (details == null) return;
            EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
            if (detailsExt.UsrGuarReceviableCD == null) return;
            if ((string)e.OldValue != detailsExt.UsrGuarReceviableCD)
            {
                CCReceivableCheck receivableCheck = GetCCReceivableCheck(detailsExt.UsrGuarReceviableCD);
                details.ExpenseDate = receivableCheck.DocDate;
                details.ExpenseRefNbr = receivableCheck.GuarNbr;
                details.Qty = 1;
                //2021/11/30 ���F�s��EPClaim�����B
                Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(details, receivableCheck.GuarAmt);
                Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.curyExtCost>(details, receivableCheck.GuarAmt);
                details.CuryUnitCost = receivableCheck.GuarAmt;
                details.CuryExtCost = receivableCheck.GuarAmt;
                detailsExt.UsrGuarType = receivableCheck.GuarType;
                detailsExt.UsrGuarClass = receivableCheck.GuarClass;
                detailsExt.UsrDueDate = receivableCheck.DueDate;
                detailsExt.UsrAuthDate = receivableCheck.AuthDate;
                detailsExt.UsrPONbr = receivableCheck.PONbr;
                detailsExt.UsrIssueDate = receivableCheck.IssueDate;
                //2021/11/03 Add Mantis: 0012263
                detailsExt.UsrGuarReceviableBatchNbr = receivableCheck.BatchNbr;
            }
            CCView.Cache.SetDefaultExt<EPExpenseClaimDetailsExt.usrGuarReceviableBatchNbr>(details);
        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetailsExt.usrApprovalLevelID> e)
        {
            EPExpenseClaimDetails details = e.Row as EPExpenseClaimDetails;
            if (details == null) return;
            EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);

            //2021/08/20 Delete UsrCreditAccountID & UsrCreditSubID
            /*
            if (e.NewValue == null)
            {
                detailsExt.UsrCreditAccountID = null;
                detailsExt.UsrCreditSubID = null;
            }
            else
            {
            */
            //2022-12-09 alton add isPreRowImport �Ω󱱨�Upload��Ʈɻ\��inventoryID
            if (e.NewValue != null && (!isPreRowImport || details.InventoryID == null))
            {
                KGBudApproveLevel Budlevel = GetBudLevel((int)e.NewValue);
                if (Budlevel != null)
                {
                    Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.inventoryID>(details, Budlevel.InventoryID);
                    //Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetailsExt.usrCreditAccountID>(details, Budlevel.CreditAccountID);
                    //Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetailsExt.usrCreditSubID>(details, Budlevel.CreditSubID);
                }
            }

        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetails.inventoryID> e)
        {
            EPExpenseClaimDetails details = e.Row as EPExpenseClaimDetails;
            if (details == null) return;
            //���F�A��Ĳ�o�ڪ�expenseAccount fieldupdated.... �ױ���t���޿�
            Base.ExpenseClaimDetails.Cache.SetDefaultExt<EPExpenseClaimDetails.expenseAccountID>(details);

        }
        //2021/09/01 Add Mantis: 0012218
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetails, EPExpenseClaimDetails.expenseAccountID> e)
        {
            EPExpenseClaimDetails row = e.Row;


            if (row == null) return;
            EPExpenseClaim header = Base.ExpenseClaim.Current;
            EPExpenseClaimExt headerExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(header);
            if (headerExt.UsrDocType == EPStringList.EPDocType.STD)
            {
                if (row.ExpenseAccountID == null) row.ExpenseSubID = null;
                else
                {
                    Account account = GetAccount(row.ExpenseAccountID);
                    if (account?.Type == AccountType.Asset || account?.Type == AccountType.Liability)
                    {
                        Sub sub = GetSub("9999999");
                        Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.expenseSubID>(row, sub?.SubID);
                    }
                    else
                    {
                        EPEmployee employee = GetEmpolyeeEPSub(header.EmployeeID);
                        Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.expenseSubID>(row, employee?.ExpenseSubID ?? null);

                    }
                }
            }

        }
        //2021/08/19 Delete Mantis: 0012204
        //2021/08/13 Add Mantis: 0012199
        /*protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetails, EPExpenseClaimDetails.curyTranAmtWithTaxes> e)
        {
            EPExpenseClaimDetails details = e.Row;
            if (details == null) return;
            EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
            if ((details.CuryAmountWithTaxes ?? 0) > 0)
                detailsExt.UsrCreditAmt = 0;
        }
        */
        //2021/08/19 Delete Mantis: 0012204
        //2021/08/13 Add Mantis: 0012199
        /*protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetails, EPExpenseClaimDetailsExt.usrCreditAmt> e)
        {
            EPExpenseClaimDetails details = e.Row;
            if (details == null) return;
            EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
            if(detailsExt.UsrCreditAmt >0)
            {
                Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.qty>(details, 0m);
                Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.curyTranAmtWithTaxes>(details, 0m);
            }
        }
        */
        /*protected virtual void EPExpenseClaimDetails_UsrGuarClass_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPExpenseClaimDetails details = e.Row as EPExpenseClaimDetails;
            EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
            if (details == null) return;
            if((string)e.OldValue != detailsExt.UsrGuarClass || detailsExt.UsrGuarClass==null)
            {
                detailsExt.UsrBankAccountID = null;
                detailsExt.UsrBookCD = null;
            }
        }*/
        //2021/10/19 Add Mantis: 0012256
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetailsExt.usrBankAccountID> e)
        {
            EPExpenseClaimDetails details = e.Row as EPExpenseClaimDetails;
            EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
            if (details == null) return;
            if (detailsExt.UsrBankAccountID != null)
            {
                NMBankAccount bankAccount = GetBankAccount(detailsExt.UsrBankAccountID);
                CA.CashAccount cashAccount = GetCashAccount(bankAccount.CashAccountID ?? throw new Exception("�Ц�NM�Ȧ�b��]�w�{�����!")) ?? throw new Exception("���ˬd���{�����!");
                detailsExt.CashAccountID = cashAccount.CashAccountID;
                details.ExpenseAccountID = cashAccount.AccountID;
                details.ExpenseSubID = cashAccount.SubID;
            }
            else
            {
                detailsExt.CashAccountID = null;
                details.ExpenseAccountID = null;
                details.ExpenseSubID = null;
            }
            //���F���X���B�s��
            Base.ExpenseClaimDetails.Cache.SetDefaultExt<EPExpenseClaimDetailsExt.usrSumBankTxnAmt>(details);
        }
        protected virtual void _(Events.RowPersisting<EPExpenseClaimDetails> e)
        {
            EPExpenseClaimDetails row = (EPExpenseClaimDetails)e.Row;
            if (row == null) return;
            EPExpenseClaimDetailsExt rowExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(row);

            EPExpenseClaim header = Base.ExpenseClaimCurrent.Current;
            if (header == null) return;
            EPExpenseClaimExt headerExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(header);

            //�ˬd�M�׬O�_�@�P
            //�ˬd�w��(UsrDcoType == STD) --> Delete
            if (row.ContractID != null)
            {
                foreach (EPExpenseClaimDetails details in Base.ExpenseClaimDetails.Cache.Cached)
                {
                    if (details.ContractID != row.ContractID)
                    {
                        Contract contract = GetContract(row.ContractID);
                        Base.ExpenseClaimDetails.Cache.RaiseExceptionHandling<EPExpenseClaimDetails.contractID>(
                            row, contract.ContractCD,
                            new PXSetPropertyException("�п�ۦP���M��!"));
                    }
                    /*if(headerExt.UsrIsGuarantee ==true && row.ExpenseRefNbr == null)
                    {
                        Base.ExpenseClaimDetails.Cache.RaiseExceptionHandling<EPExpenseClaimDetails.expenseRefNbr>(
                           row, row.ExpenseRefNbr,
                           new PXSetPropertyException("�ѦҸ��X������!"));
                    }*/
                }

                //2021/07/13 Move to Submit Action
                //�ˮֹw��u�bUsrDocType == STD
                /*
                if (headerExt.UsrDocType == EPStringList.EPDocType.STD)
                {
                    
                    //�M�׬�X
                    if (row.ContractID == 0)
                    {
                        if (checkGLBudgetLine(row, header))
                        {
                            throw new PXSetPropertyException("�w�⤣��", PXErrorLevel.RowError);
                        }
                    }
                    //�M�פ���X
                    else
                    {
                        if (checkPMBudgetLine(row, header))
                        {
                            throw new PXSetPropertyException("�w�⤣��", PXErrorLevel.RowError);
                        }
                        KGSetUp kGSetUp = GetKGSetUp();
                        if (kGSetUp == null) throw new Exception("�Ч���KG���n�]�w!");
                        KGSetUpExt kGSetUpExt = PXCache<KGSetUp>.GetExtension<KGSetUpExt>(kGSetUp);
                        if (kGSetUpExt.UsrKGPrjManageInventoryID == null) throw new Exception("�Ц�KG���n�]�w��g�M�׺޲z�u�ƽs��!");

                        PMCostBudget costBudget = GetPMCostBudget(row.ContractID, kGSetUpExt.UsrKGPrjManageInventoryID);
                        if (costBudget == null)
                            throw new Exception("�M�ץ��s�C�޲z�O�ιw��");

                        row.TaskID = costBudget.TaskID;
                        row.CostCodeID = costBudget.CostCodeID;
                        InventoryItem inventory = GetInventory(kGSetUpExt.UsrKGPrjManageInventoryID);
                        row.ExpenseAccountID = inventory.COGSAcctID;
                        row.ExpenseSubID = inventory.COGSSubID;

                        Base.ExpenseClaimDetails.Update(row);
                    } 
                }*/

            }

            //2021/02/18 Mantis: 0011943
            //ExpenseDate Check FinYear

            if (headerExt.UsrDocType == EPStringList.EPDocType.STD)
            {
                checkFinYear(headerExt.UsrFinancialYear, row.ExpenseDate);

                if (row.ContractID != 0)
                {
                    KGSetUp kGSetUp = GetKGSetUp();
                    if (kGSetUp == null) throw new Exception("�Ч���KG���n�]�w!");
                    KGSetUpExt kGSetUpExt = PXCache<KGSetUp>.GetExtension<KGSetUpExt>(kGSetUp);
                    if (kGSetUpExt.UsrKGPrjManageInventoryID == null) throw new Exception("�Ц�KG���n�]�w��g�M�׺޲z�u�ƽs��!");

                    PMCostBudget costBudget = GetPMCostBudget(row.ContractID, kGSetUpExt.UsrKGPrjManageInventoryID);
                    if (costBudget == null)
                        throw new Exception("�M�ץ��s�C�޲z�O�ιw��");

                    row.TaskID = costBudget.TaskID;
                    row.CostCodeID = costBudget.CostCodeID;
                    InventoryItem inventory = GetInventory(kGSetUpExt.UsrKGPrjManageInventoryID);
                    row.ExpenseAccountID = inventory.COGSAcctID;
                    row.ExpenseSubID = inventory.COGSSubID;

                    Base.ExpenseClaimDetails.Update(row);
                }
            }

            #region 2021/08/19 Delete Mantis: 0012204 
            //2021/08/13 Add Mantis: 0012199
            /*
            if (headerExt.UsrDocType == EPStringList.EPDocType.GLB)
            {
                if ((rowExt.UsrCreditAmt ?? 0) > 0)
                {
                    if (rowExt.UsrCreditAccountID == null)
                    {
                        Base.ExpenseClaimDetails.Cache.RaiseExceptionHandling<EPExpenseClaimDetailsExt.usrCreditAccountID>(
                            row, rowExt.UsrCreditAccountID, new PXSetPropertyException("����줣�i����!"));
                        e.Cancel = true;
                    }
                    if (rowExt.UsrCreditSubID == null)
                    {
                        Base.ExpenseClaimDetails.Cache.RaiseExceptionHandling<EPExpenseClaimDetailsExt.usrCreditSubID>(
                            row, rowExt.UsrCreditSubID, new PXSetPropertyException("����줣�i����!"));
                        e.Cancel = true;
                    }
                }
            }
            */
            #endregion 
        }
        #endregion

        #region GVApGuiInvoiceFinRef
        #region RowEvent
        protected virtual void GVApGuiInvoiceFinRef_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            GVApGuiInvoiceFinRef row = (GVApGuiInvoiceFinRef)e.Row;
            if (!CheckGVApGuiInvoiceFinRef(row)) throw new PXException("GVApGuiInvoiceFinRef has error.");

            EPExpenseClaim master = Base.ExpenseClaim.Current;
            if (master == null) return;
            //2021-04-07 mark by alton Hold����
            //if (row.Hold == null) row.Hold = true;
            row.TotalAmt = row.SalesAmt + row.TaxAmt;
            if (row.RegistrationCD == null)
            {
                Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, Base.Accessinfo.BranchID);
                GVRegistrationBranch gvRegistrationBranch = PXSelect<GVRegistrationBranch, Where<GVRegistrationBranch.bAccountID, Equal<Required<GVRegistrationBranch.bAccountID>>>>.Select(Base, branch.BAccountID);
                if (gvRegistrationBranch != null)
                {
                    GVRegistration gvRegistration = PXSelect<GVRegistration, Where<GVRegistration.registrationID, Equal<Required<GVRegistration.registrationID>>>>.Select(Base, gvRegistrationBranch.RegistrationID);
                    row.RegistrationCD = gvRegistration.RegistrationCD;
                }
            }

        }

        #endregion
        #region FieldUpdate
        protected virtual void _(Events.FieldUpdated<GVApGuiInvoiceFinRef, GVApGuiInvoiceFinRef.invoiceDate> e)
        {
            GVApGuiInvoiceFinRef row = (GVApGuiInvoiceFinRef)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiInvoiceFinRef.declareYear>(row);
            e.Cache.SetDefaultExt<GVApGuiInvoiceFinRef.declareMonth>(row);
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiInvoiceFinRef, GVApGuiInvoiceFinRef.invoiceType> e)
        {
            GVApGuiInvoiceFinRef row = (GVApGuiInvoiceFinRef)e.Row;
            if (row == null) return;
            PXCache cache = GVApGuiInvoiceRefs.Cache;
            if (row.InvoiceType == GVList.GVGuiInvoiceType.INVOICE)
            {
                cache.SetValueExt<GVApGuiInvoiceFinRef.guiType>(row, GVList.GVGuiType.AP.GuiType_21);
                cache.SetValueExt<GVApGuiInvoiceFinRef.voucherCategory>(row, GVList.GVGuiVoucherCategory.TAXABLE);
                cache.SetValueExt<GVApGuiInvoiceFinRef.taxCode>(row, GVList.GVTaxCode.TAXABLE);
            }
            else if (row.InvoiceType == GVList.GVGuiInvoiceType.RECEIPT)
            {
                cache.SetValueExt<GVApGuiInvoiceFinRef.guiType>(row, GVList.GVGuiType.AP.GuiType_27);
                cache.SetValueExt<GVApGuiInvoiceFinRef.voucherCategory>(row, GVList.GVGuiVoucherCategory.OTHERCERTIFICATE);
                cache.SetValueExt<GVApGuiInvoiceFinRef.taxCode>(row, GVList.GVTaxCode.TAXABLE);
            }
            //e.Cache.SetDefaultExt<GVApGuiInvoiceRef.taxAmt>(row);
        }
        protected virtual void _(Events.FieldUpdated<GVApGuiInvoiceFinRef, GVApGuiInvoiceFinRef.vendor> e)
        {
            GVApGuiInvoiceFinRef row = (GVApGuiInvoiceFinRef)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiInvoiceFinRef.vendorUniformNumber>(row);
            e.Cache.SetDefaultExt<GVApGuiInvoiceFinRef.vendorName>(row);
            e.Cache.SetDefaultExt<GVApGuiInvoiceFinRef.vendorAddress>(row);
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiInvoiceRef, GVApGuiInvoiceRef.salesAmt> e)
        {
            GVApGuiInvoiceRef row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiInvoiceRef.taxAmt>(row);
        }
        #endregion
        #region FieldDefaulting
        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceRef, GVApGuiInvoiceRef.taxAmt> e)
        {
            GVApGuiInvoiceRef row = e.Row;
            if (row == null) return;
            decimal taxRate = GVList.GVTaxCode.TAXABLE.Equals(row.TaxCode) ? 0.05m : 0m;
            e.NewValue = round((row.SalesAmt ?? 0) * taxRate);
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceFinRef, GVApGuiInvoiceFinRef.declareYear> e)
        {
            GVApGuiInvoiceFinRef row = (GVApGuiInvoiceFinRef)e.Row;
            if (row == null) return;
            e.NewValue = row.InvoiceDate?.Year;
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceFinRef, GVApGuiInvoiceFinRef.declareMonth> e)
        {
            GVApGuiInvoiceFinRef row = (GVApGuiInvoiceFinRef)e.Row;
            if (row == null) return;
            e.NewValue = row.InvoiceDate?.Month;
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceFinRef, GVApGuiInvoiceFinRef.vendor> e)
        {
            GVApGuiInvoiceFinRef row = (GVApGuiInvoiceFinRef)e.Row;
            if (row == null) return;
            EPExpenseClaim master = Base.ExpenseClaim.Current;
            EPExpenseClaimExt masterExt = Base.ExpenseClaim.Cache.GetExtension<EPExpenseClaimExt>(master);
            e.NewValue = masterExt?.UsrVendorID ?? master.EmployeeID;
        }
        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceFinRef.remark> e)
        {
            GVApGuiInvoiceFinRef row = (GVApGuiInvoiceFinRef)e.Row;
            if (row == null) return;
            e.NewValue = Base.ExpenseClaim.Current.DocDesc;
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceFinRef, GVApGuiInvoiceFinRef.vendorUniformNumber> e)
        {
            GVApGuiInvoiceFinRef row = (GVApGuiInvoiceFinRef)e.Row;
            if (row == null) return;
            EPExpenseClaim master = Base.ExpenseClaim.Current;
            EPExpenseClaimExt masterExt = Base.ExpenseClaim.Cache.GetExtension<EPExpenseClaimExt>(master);
            int? locationID = masterExt.UsrVendorLocationID ?? master.LocationID;
            Location location = GetLocationByDefLocationID(locationID);
            e.NewValue = location?.TaxRegistrationID;
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceFinRef, GVApGuiInvoiceFinRef.vendorAddress> e)
        {
            GVApGuiInvoiceFinRef row = (GVApGuiInvoiceFinRef)e.Row;
            if (row == null) return;
            BAccount2 b = (BAccount2)PXSelectorAttribute.Select<GVApGuiInvoiceFinRef.vendor>(e.Cache, row);
            Address address = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(Base, b?.DefAddressID);
            e.NewValue = address?.AddressLine1;
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceFinRef, GVApGuiInvoiceFinRef.vendorName> e)
        {
            GVApGuiInvoiceFinRef row = (GVApGuiInvoiceFinRef)e.Row;
            if (row == null) return;
            BAccount2 b = (BAccount2)PXSelectorAttribute.Select<GVApGuiInvoiceFinRef.vendor>(e.Cache, row);
            e.NewValue = b?.AcctName;
        }
        #endregion
        #endregion

        #region KGBillPayment
        protected virtual void _(Events.RowPersisting<KGBillPayment> e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            SetPostageAmt(row);
        }

        protected virtual void _(Events.RowSelected<KGBillPayment> e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            BAccount2 ba2 = (BAccount2)PXSelectorAttribute.Select<KGBillPayment.vendorID>(e.Cache, row);
            String type = (ba2?.Type ?? BAccountType.VendorType);
            bool isVendor = type == BAccountType.VendorType;
            bool isGif = row.PaymentMethod == PaymentMethod.D;//§��
            SetUIRequired<KGBillPayment.vendorLocationID>(e.Cache, row, isVendor || !isGif);
            SetKGBillpaymentEnable(row);
        }
        protected virtual void _(Events.FieldDefaulting<KGBillPayment, KGBillPayment.paymentMethod> e)
        {
            KGBillPayment row = e.Row;
            if (row == null) return;
            EPExpenseClaim header = Base.ExpenseClaim.Current;
            if (header == null) return;
            EPExpenseClaimExt headerExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(header);

            string PaymentMethodID = "";
            if (headerExt.UsrVendorLocationID == null)
                e.NewValue = PaymentMethod.A;
            else
            {
                Location location = GetLocationID(headerExt.UsrVendorLocationID);
                switch (location.VPaymentMethodID)
                {
                    default:
                        PaymentMethodID = PaymentMethod.C;
                        break;
                    case "CHECK":
                        PaymentMethodID = PaymentMethod.A;
                        break;
                    case "TT":
                        PaymentMethodID = PaymentMethod.B;
                        break;
                    case "COUPON":
                        PaymentMethodID = PaymentMethod.D;
                        break;
                }
                e.NewValue = PaymentMethodID;
            }
        }

        protected virtual void KGBillPayment_PaymentMethod_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            if ((string)e.OldValue != row.PaymentMethod)
            {
                //cache.SetDefaultExt<KGBillPayment.vendorLocationID>(row);
                cache.SetValueExt<KGBillPayment.vendorLocationID>(row, NMLocationUtil.GetDefLocationByPaymentMethod(row.VendorID, row.PaymentMethod));
            }
        }

        protected virtual void _(Events.FieldDefaulting<KGBillPayment, KGBillPayment.vendorID> e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            EPExpenseClaim master = Base.ExpenseClaim.Current;
            if (master == null) return;
            EPExpenseClaimExt masterExt = Base.ExpenseClaim.Cache.GetExtension<EPExpenseClaimExt>(master);
            e.NewValue = masterExt.UsrVendorID ?? master.EmployeeID;
        }

        protected virtual void _(Events.FieldUpdated<KGBillPayment, KGBillPayment.vendorID> e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            //e.Cache.SetDefaultExt<KGBillPayment.vendorLocationID>(row);
            e.Cache.SetValueExt<KGBillPayment.vendorLocationID>(row, NMLocationUtil.GetDefLocationByPaymentMethod(row.VendorID, row.PaymentMethod));
            e.Cache.SetDefaultExt<KGBillPayment.checkTitle>(row);
        }

        protected virtual void KGBillPayment_VendorLocationID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            if (row.PaymentMethod == PaymentMethod.C || row.PaymentMethod == PaymentMethod.E)
            {
                BAccount baccount = GetBAccount(row.VendorID);
                e.NewValue = baccount?.DefLocationID;
            }
            else
            {
                e.NewValue = NMLocationUtil.GetDefLocationByPaymentMethod(row.VendorID, row.PaymentMethod);
            }
        }

        protected virtual void KGBillPayment_VendorLocationID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            cache.SetDefaultExt<KGBillPayment.bankAccountID>(row);
        }

        protected virtual void KGBillPayment_BankAccountID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            if (row.VendorLocationID != null)
            {
                Location location = GetLocationID(row.VendorLocationID);
                if (location.VCashAccountID == null) return;
                NMBankAccount account = GetBankAccount(location.VCashAccountID, row.PaymentMethod);
                e.NewValue = account?.BankAccountID;
            }
            else
                e.NewValue = null;

        }
        protected virtual void KGBillPayment_CheckTitle_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            BAccount2 bAccount2 = (BAccount2)PXSelectorAttribute.Select<KGBillPayment.vendorID>(cache, row);
            e.NewValue = bAccount2?.AcctName;

        }

        protected virtual void KGBillPayment_PaymentPct_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            EPExpenseClaim header = Base.ExpenseClaim.Current;
            if (row == null) return;
            decimal paymentAmount = (header?.CuryDocBal ?? 0) * (row.PaymentPct ?? 0) / 100;
            paymentAmount = System.Math.Round(paymentAmount, 0, MidpointRounding.AwayFromZero);
            cache.SetValueExt<KGBillPayment.paymentAmount>(row, paymentAmount);

        }

        protected virtual void _(Events.FieldUpdated<KGBillPayment, KGBillPayment.pricingType> e)
        {
            KGBillPayment row = e.Row;
            if (row == null) return;
            SetPaymentDate(Base.ExpenseClaim.Current, ref row);
        }

        protected virtual void _(Events.FieldUpdated<KGBillPayment, KGBillPayment.paymentPeriod> e)
        {
            KGBillPayment row = e.Row;
            if (row == null) return;
            SetPaymentDate(Base.ExpenseClaim.Current, ref row);
        }
        #endregion

        #region EPPaymentModify

        /*
        protected virtual void EPPaymentModiReqTT_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            EPPaymentModiReq row = e.Row as EPPaymentModiReq;
            EPPaymentModiReqTT tt = e.Row as EPPaymentModiReqTT;
            if (row == null) return;
            bool isTT = false;
            decimal? AmtTT = 0;
            foreach (EPPaymentModiReq req in ReqTT.Select())
            {

                isTT = true;
                KGBillPayment payment = GetKGBillPayment(req.NMID);
                if (payment != null)
                    AmtTT = AmtTT + payment.PaymentAmount;
            }

            counts.IsTT = isTT;
            counts.AmtTT = AmtTT;
            ReqCount.Update(counts);
        }
        */
        protected virtual void _(Events.RowPersisting<EPPaymentModiReqNMP> e)
        {
            EPPaymentModiReqNMP row = e.Row;
            if (row == null) return;
            foreach (EPPaymentModiReqNMP item in ReqNMP.Select())
            {
                if (row.PaymentModiReqID == item.PaymentModiReqID) continue;
                if (row.NMID == item.NMID)
                {
                    SetError<EPPaymentModiReqNMP.nmID>(e.Cache, row, row.NMID, "�䲼���X���i����");
                    return;
                }
            }
        }

        protected virtual void _(Events.RowPersisting<EPPaymentModiReqNMR> e)
        {
            EPPaymentModiReqNMR row = e.Row;
            if (row == null) return;
            foreach (EPPaymentModiReqNMR item in ReqNMR.Select())
            {
                if (row.PaymentModiReqID == item.PaymentModiReqID) continue;
                if (row.NMID == item.NMID)
                {
                    SetError<EPPaymentModiReqNMR.nmID>(e.Cache, row, row.NMID, "�䲼���X���i����");
                    return;
                }
            }
        }

        protected virtual void _(Events.RowPersisting<EPPaymentModiReqCCP> e)
        {
            EPPaymentModiReqCCP row = e.Row;
            if (row == null) return;
            foreach (EPPaymentModiReqCCP item in ReqCCP.Select())
            {
                if (row.PaymentModiReqID == item.PaymentModiReqID) continue;
                if (row.RefNbr == item.RefNbr)
                {
                    SetError<EPPaymentModiReqCCP.refNbr>(e.Cache, row, row.RefNbr, "�ѷӸ��X���i����");
                    return;
                }
            }
        }

        protected virtual void _(Events.RowPersisting<EPPaymentModiReqCCR> e)
        {
            EPPaymentModiReqCCR row = e.Row;
            if (row == null) return;
            foreach (EPPaymentModiReqCCR item in ReqCCR.Select())
            {
                if (row.PaymentModiReqID == item.PaymentModiReqID) continue;
                if (row.RefNbr == item.RefNbr)
                {
                    SetError<EPPaymentModiReqCCR.refNbr>(e.Cache, row, row.RefNbr, "�ѷӸ��X���i����");
                    return;
                }
            }
        }

        protected virtual void _(Events.RowPersisting<EPPaymentModiReqTT> e)
        {
            EPPaymentModiReqTT row = e.Row;
            if (row == null) return;
            foreach (EPPaymentModiReqTT item in ReqTT.Select())
            {
                if (row.PaymentModiReqID == item.PaymentModiReqID) continue;
                if (row.NMID == item.NMID)
                {
                    SetError<EPPaymentModiReqTT.nmID>(e.Cache, row, row.NMID, "�ѷӸ��X���i����");
                    return;
                }
            }
        }



        protected virtual void EPPaymentModiReqCCR_RefNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPPaymentModiReq row = e.Row as EPPaymentModiReq;
            EPPaymentModiReqCCR ccr = e.Row as EPPaymentModiReqCCR;

            if (ccr == null) return;
            CCReceivableCheck receivableCheck = GetCCReceivableCheck(row.RefNbr);


            if (receivableCheck != null)
            {
                sender.SetDefaultExt<EPPaymentModiReqCCR.status>(ccr);
                ccr.Amt = receivableCheck.GuarAmt;
            }
            UpdateModifyAmt(PaymentType.CCR, (string)e.OldValue, row.RefNbr);
        }
        protected virtual void EPPaymentModiReqCCP_RefNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPPaymentModiReq row = e.Row as EPPaymentModiReq;
            EPPaymentModiReqCCP ccp = e.Row as EPPaymentModiReqCCP;

            if (ccp == null) return;
            CCPayableCheck payableCheck = GetCCPayableCheck(row.RefNbr);
            if (payableCheck != null)
            {
                sender.SetDefaultExt<EPPaymentModiReqCCP.status>(ccp);
                ccp.Amt = payableCheck.GuarAmt;
            }
            UpdateModifyAmt(PaymentType.CCP, (string)e.OldValue, row.RefNbr);
        }
        protected virtual void EPPaymentModiReqNMP_NMID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPPaymentModiReq row = e.Row as EPPaymentModiReq;
            EPPaymentModiReqNMP nmp = e.Row as EPPaymentModiReqNMP;

            if (nmp == null) return;
            NMPayableCheck payableCheck = GetNMPayableCheck(row.NMID);
            if (payableCheck != null)
            {
                sender.SetDefaultExt<EPPaymentModiReqNMP.status>(nmp);
                sender.SetDefaultExt<EPPaymentModiReqNMP.payBatchNbr>(nmp);
                //20220406 louis MNID�s�ɱa�X�䲼���X
                nmp.CheckNbr = payableCheck.CheckNbr;
                nmp.Amt = payableCheck.OriCuryAmount;
            }
            UpdateModifyAmt(PaymentType.NMP, (int?)e.OldValue, row.NMID);

        }
        protected virtual void EPPaymentModiReqNMR_NMID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPPaymentModiReq row = e.Row as EPPaymentModiReq;
            EPPaymentModiReqNMR nmr = e.Row as EPPaymentModiReqNMR;

            if (nmr == null) return;
            NMReceivableCheck receivableCheck = GetNMReceivableCheck(row.NMID);
            if (receivableCheck != null)
            {
                //nmr.Status = receivableCheck.Status;
                sender.SetDefaultExt<EPPaymentModiReqNMR.status>(nmr);
                sender.SetDefaultExt<EPPaymentModiReqNMR.payBatchNbr>(nmr);
                //20220406 louis MNID�s�ɱa�X�䲼���X
                nmr.CheckNbr = receivableCheck.CheckNbr;
                nmr.Amt = receivableCheck.OriCuryAmount;
            }
            UpdateModifyAmt(PaymentType.NMR, (int?)e.OldValue, row.NMID);
        }

        protected virtual void EPPaymentModiReqTT_NMID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPPaymentModiReq row = e.Row as EPPaymentModiReq;
            EPPaymentModiReqTT tt = e.Row as EPPaymentModiReqTT;
            if (tt == null) return;
            KGBillPayment payment = GetKGBillPayment(row.NMID);
            if (payment != null)
            {
                tt.Amt = payment.PaymentAmount;
            }
            UpdateModifyAmt(PaymentType.TT, (int?)e.OldValue, row.NMID);

        }

        #endregion

        #region KGExpenseVoucher
        protected virtual void _(Events.FieldUpdated<KGExpenseVoucher, KGExpenseVoucher.creditAmt> e)
        {
            KGExpenseVoucher row = e.Row;
            if (row.CreditAmt > 0m)
                row.DebitAmt = 0m;
            if (row.CreditAmt < 0m)
                GLBView.Cache.RaiseExceptionHandling<KGExpenseVoucher.creditAmt>
                    (row, row.CreditAmt, new PXSetPropertyException("���B���i�p��s!"));
        }
        protected virtual void _(Events.FieldUpdated<KGExpenseVoucher, KGExpenseVoucher.debitAmt> e)
        {
            KGExpenseVoucher row = e.Row;
            if (row.DebitAmt > 0m)
                row.CreditAmt = 0m;
            if (row.DebitAmt < 0m)
                GLBView.Cache.RaiseExceptionHandling<KGExpenseVoucher.debitAmt>
                    (row, row.DebitAmt, new PXSetPropertyException("���B���i�p��s!"));
        }
        protected virtual void _(Events.RowPersisting<KGExpenseVoucher> e)
        {
            KGExpenseVoucher row = e.Row;
            if (row == null) return;
            if (row.CreditAmt < 0m)
                GLBView.Cache.RaiseExceptionHandling<KGExpenseVoucher.creditAmt>
                    (row, row.CreditAmt, new PXSetPropertyException("���B���i�p��s!"));
            if (row.DebitAmt < 0m)
                GLBView.Cache.RaiseExceptionHandling<KGExpenseVoucher.debitAmt>
                    (row, row.DebitAmt, new PXSetPropertyException("���B���i�p��s!"));
        }
        #endregion

        #region 2020/09/23 ���ݭn���{���X ��Ȥ�M�Ȥ�Ҧb�a�M���Ӫ������өM�����өҦb�a����
        //2020/09/23 ���ݭn�����P�_
        /*protected virtual void EPExpenseClaim_UsrTargetType_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            EPExpenseClaim row = (EPExpenseClaim)e.Row;
            EPExpenseClaimExt rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(row);

            if (rowExt.UsrTargetType == "V")
            {
                row.CustomerID = null;
                row.CustomerLocationID = null;
            }
            if (rowExt.UsrTargetType == "C")
            {
                rowExt.UsrVendorID = null;
                rowExt.UsrVendorLocationID = null;
            }
        }*/

        //2020/09/23 ���ݭn�л\detail���
        /*protected virtual void EPExpenseClaim_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
        {
            EPExpenseClaim row = e.Row as EPExpenseClaim;
            EPExpenseClaimExt rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(row);
            EPExpenseClaim oldRow = e.OldRow as EPExpenseClaim;
            EPExpenseClaimExt oldRowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(oldRow);

            if (rowExt.UsrVendorID != oldRowExt.UsrVendorID)
            {
                var receipts = Base.ExpenseClaimDetails.Select().AsEnumerable();

                var query = receipts.Where(_ => ((EPExpenseClaimDetails)_).ContractID == null || ProjectDefaultAttribute.IsNonProject(((EPExpenseClaimDetails)_).ContractID));
                if (query.Count() != 0)
                {
                    VendorUpdateAsk.Current.NewVendorID = rowExt.UsrVendorID;
                    VendorUpdateAsk.Current.OldVendorID = oldRowExt.UsrVendorID;
                    VendorUpdateAsk.AskExt();
                }
            }
        }*/
        /* protected virtual void EPExpenseClaimDetails_UsrVendorID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
         {
             EPExpenseClaimDetails row = e.Row as EPExpenseClaimDetails;
             EPExpenseClaimDetailsExt rowExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(row);
             LocationExtAddress location = GetLocation(rowExt.UsrVendorID);

             if (rowExt?.UsrVendorID == null)
                 cache.SetValueExt<EPExpenseClaimDetailsExt.usrVendorLocationID>(rowExt, null);
             //else
                // cache.SetValueExt<EPExpenseClaimDetailsExt.usrVendorLocationID>(rowExt,location.LocationID);
         }
         protected virtual void EPExpenseClaimDetails_UsrVendorLocationID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
         {
             EPExpenseClaim header = Base.ExpenseClaimCurrent.Current;
             EPExpenseClaimExt headerExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(header);

             e.NewValue = headerExt.UsrVendorLocationID;
             e.Cancel = true;
         }*/
        #endregion

        #endregion

        #region HyperLink

        #region View GL
        public PXAction<APInvoice> ViewGL;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGL()
        {
            Batch batch = LinkToGL.Current;
            JournalEntry graph = PXGraph.CreateInstance<JournalEntry>();
            graph.BatchModule.Current = graph.BatchModule.Search<Batch.batchNbr>(batch.BatchNbr, batch.Module);
            if (graph.BatchModule.Current == null) return;
            new HyperLinkUtil<JournalEntry>(graph.BatchModule.Current, true);
        }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// EPExpenseClaim.DocDate & KGBillPayment.PaymentPeriod ���ʮɭnĲ�o����
        /// </summary>
        /// <param name="master"></param>
        /// <param name="row"></param>
        public void SetPaymentDate(EPExpenseClaim master, ref KGBillPayment row)
        {
            if (master == null) return;
            if (row.PricingType == PricingType.A)
            {
                DateTime? date = master.DocDate?.AddDays(row.PaymentPeriod ?? 0);
                KGBillPayments.Cache.SetValueExt<KGBillPayment.paymentDate>(row, date);
            }
        }

        public Decimal? round(Decimal? num)
        {
            if (num == null)
            {
                return 0;
            }
            return System.Math.Round(num ?? 0m, 0, MidpointRounding.AwayFromZero);
        }

        public void SetPostageAmt(KGBillPayment row)
        {
            int? projectID = Base.ExpenseClaimDetails.Current?.ContractID;
            decimal postageAmt = 0m;
            if ((row.IsPostageFree ?? false) == false)
            {
                switch (row.PaymentMethod)
                {
                    case PaymentMethod.A://�䲼
                        postageAmt = CalcPostageUtil.GetPostageAmt(Base, 1, row.PaymentAmount);
                        break;
                    case PaymentMethod.B://�q��
                        postageAmt = CalcPostageUtil.GetPostageAmt(Base, 0, row.PaymentAmount);
                        break;
                    case PaymentMethod.D://§��
                        BAccount2 ba2 = (BAccount2)PXSelectorAttribute.Select<KGBillPayment.vendorID>(KGBillPayments.Cache, row);
                        //�������ӥB���M�סA��䲼�p��
                        if (ba2.Type == BAccountType.VendorType && projectID != null && projectID != 0)
                            postageAmt = CalcPostageUtil.GetPostageAmt(Base, 1, row.PaymentAmount);
                        else
                            postageAmt = 0m;
                        break;
                    default:
                        postageAmt = 0m;
                        break;
                }
            }
            row.PostageAmt = postageAmt;
            row.ActPayAmt = (row.PaymentAmount ?? 0m) - (row.PostageAmt ?? 0m);
        }

        public void SetUIRequired<T>(PXCache cache, object row, bool isRequired) where T : PX.Data.IBqlField
        {
            PXDefaultAttribute.SetPersistingCheck<T>(cache, row, isRequired ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
        }

        public bool CheckGVApGuiInvoiceFinRef(GVApGuiInvoiceFinRef row)
        {
            bool check = true;

            if (row.InvoiceType == GVList.GVGuiInvoiceType.INVOICE)
            {
                #region �ˮֵo�����X�榡
                if (!CheckInvoiceNbrInputMask(row.GuiInvoiceNbr))
                    check = SetError<GVApGuiInvoiceFinRef.guiInvoiceNbr>(row, row.GuiInvoiceNbr, "�榡���~");
                #endregion

                #region �ˬd�o�����X�O�_����
                PXResultset<GVApGuiInvoiceFinRef> set = GetAllInvoiceByNbrNotThisID(row.GuiInvoiceNbr, row.GuiInvoiceID);
                if (set.Count > 0)
                    check = SetError<GVApGuiInvoiceFinRef.guiInvoiceNbr>(row, row.GuiInvoiceNbr, APInvoiceEntry_Extension.Message.GVApGuiInvoiceDuplicateError);
                #endregion
            }

            return check;
        }

        private bool SetError<Field>(GVApGuiInvoiceFinRef row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            GVApGuiInvoiceRefs.Cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.Error));
            return false;
        }

        private bool SetError<Field>(PXCache cache, object row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.RowError));
            return false;
        }

        public virtual bool CheckInvoiceNbrInputMask(string invoiceNbr)
        {
            Regex rgx = new Regex(@"^[A-Z]{2}[0-9]{8}$");
            return rgx.IsMatch(invoiceNbr);
        }

        public void setEnable(string Status, EPExpenseClaim claim)
        {
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            PXUIFieldAttribute.SetReadOnly(LinkToGL.Cache, null, true);
            if (claimExt.UsrDocType != EPStringList.EPDocType.GUR ||
                claimExt.UsrDocType != EPStringList.EPDocType.RGU)
            {
                bool isCust = claimExt.UsrTargetType == "C";

                //2020/12/02 For CC so Open it...
                PXUIFieldAttribute.SetVisible<EPExpenseClaim.customerID>(Base.ExpenseClaim.Cache, claim, isCust);
                PXUIFieldAttribute.SetVisible<EPExpenseClaim.customerLocationID>(Base.ExpenseClaim.Cache, claim, isCust);
                PXUIFieldAttribute.SetVisible<EPExpenseClaimExt.usrVendorID>(Base.ExpenseClaim.Cache, claim, !isCust);
                PXUIFieldAttribute.SetVisible<EPExpenseClaimExt.usrVendorLocationID>(Base.ExpenseClaim.Cache, claim, !isCust);
            }
            else
            {
                setTargetVisible(claimExt.UsrTargetType, claim, Base.ExpenseClaim.Cache);
            }
            PXUIFieldAttribute.SetReadOnly<EPExpenseClaimExt.usrReturnUrl>(Base.ExpenseClaim.Cache, claim, true);

            bool isHold = Status == EPExpenseClaimStatus.HoldStatus;
            bool isApproved = Status == EPExpenseClaimStatus.ApprovedStatus;
            bool isSTD = claimExt.UsrDocType == EPStringList.EPDocType.STD;
            bool isCC = claimExt.UsrDocType == EPStringList.EPDocType.GUR || claimExt.UsrDocType == EPStringList.EPDocType.RGU;
            bool isCHG = claimExt.UsrDocType == EPStringList.EPDocType.CHG;
            bool isGLB = claimExt.UsrDocType == EPStringList.EPDocType.GLB;
            bool isOTH = claimExt.UsrDocType == EPStringList.EPDocType.OTH;
            //2021/10/19 Add Doctype BTN
            bool isBTN = claimExt.UsrDocType == EPStringList.EPDocType.BTN;
            //bool isRelease = Status == EPExpenseClaimStatus.ReleasedStatus;
            ///<remarks>Mantis [0012309] - 0025568 #2</remarks>
            acctApprove.SetEnabled(isSTD == true
                               && claimExt.UsrApprovalStatus == EPUsrApprovalStatus.PendingApproval);

            //if(isSTD == true) {
            Base.release.SetEnabled(claimExt.UsrApprovalStatus == EPUsrApprovalStatus.Approved
                                    || claimExt.UsrApprovalStatus == EPUsrApprovalStatus.AcctApproved
                                    );
            Base.edit.SetEnabled(claimExt.UsrApprovalStatus == EPUsrApprovalStatus.Rejected
                                 || claimExt.UsrApprovalStatus == EPUsrApprovalStatus.AcctApproved
                                 || claimExt.UsrApprovalStatus == EPUsrApprovalStatus.Approved
                                 );
            //}

            PXUIFieldAttribute.SetVisible<EPExpenseClaim.status>(Base.ExpenseClaim.Cache, claim, false);


            #region STD || OTH || BTN
            //2021/04/22 ���A��CHG�u�ݶ}Modfiy Tab��l�����_��
            //2021/05/31 �אּ�u�����A�OSTD�~�i�H��
            //2021/07/15 �Y��GLB�]�}��ϥ�
            //2021/08/19 �Y��GLB�h���_��
            //2021/10/19 �Y��BTN�u�}��EPDetails�ϥ�
            //EPClaim
            Base.ExpenseClaimDetails.AllowDelete = isSTD || isOTH || isBTN;
            Base.ExpenseClaimDetails.AllowInsert = isSTD || isOTH || isBTN;
            Base.ExpenseClaimDetails.AllowUpdate = isSTD || isOTH || isBTN;

            //2021/02/09 �אּSTD || OTH�~�n�}�ҵo��&�I�ڤ覡
            //GVAPInvoice 
            GVApGuiInvoiceRefs.AllowDelete = !(claim.Released ?? false) && (isSTD || isOTH);
            GVApGuiInvoiceRefs.AllowInsert = !(claim.Released ?? false) && (isSTD || isOTH);
            GVApGuiInvoiceRefs.AllowUpdate = !(claim.Released ?? false) && (isSTD || isOTH);
            //PXUIFieldAttribute.SetReadOnly<GVApGuiInvoiceFinRef.taxAmt>(GVApGuiInvoiceRefs.Cache, null, true);

            //KGBillPayment
            KGBillPayments.AllowDelete = isHold && (isSTD || isOTH);
            KGBillPayments.AllowInsert = isHold && (isSTD || isOTH);
            KGBillPayments.AllowUpdate = isHold && (isSTD || isOTH);
            PXUIFieldAttribute.SetVisible<KGBillPayment.vendorID>(KGBillPayments.Cache, null, true);

            //ViewEPAmt
            ViewBankTxnAmt.AllowDelete = false;
            ViewBankTxnAmt.AllowInsert = false;
            ViewBankTxnAmt.AllowUpdate = false;
            #endregion

            #region CC
            //2021/05/31 CCView �W��
            CCView.AllowDelete = isCC && isHold;
            CCView.AllowInsert = isCC && isHold;
            CCView.AllowUpdate = isCC && (isHold || isApproved);

            #endregion

            #region CHG
            //Modify
            ReqNMR.AllowDelete = isCHG && isHold;
            ReqNMR.AllowInsert = isCHG && isHold;
            ReqNMR.AllowUpdate = isCHG && isHold;

            ReqNMP.AllowDelete = isCHG && isHold;
            ReqNMP.AllowInsert = isCHG && isHold;
            ReqNMP.AllowUpdate = isCHG && isHold;

            ReqCCP.AllowDelete = isCHG && isHold;
            ReqCCP.AllowInsert = isCHG && isHold;
            ReqCCP.AllowUpdate = isCHG && isHold;

            ReqCCR.AllowDelete = isCHG && isHold;
            ReqCCR.AllowInsert = isCHG && isHold;
            ReqCCR.AllowUpdate = isCHG && isHold;

            ReqTT.AllowDelete = isCHG && isHold;
            ReqTT.AllowInsert = isCHG && isHold;
            ReqTT.AllowUpdate = isCHG && isHold;
            #endregion

            #region GLB 
            GLBView.AllowDelete = isGLB && isHold;
            GLBView.AllowInsert = isGLB && isHold;
            GLBView.AllowUpdate = isGLB && isHold;

            PXUIFieldAttribute.SetEnabled<EPExpenseClaimExt.usrStageCode>(Base.ExpenseClaim.Cache, claim, isGLB);
            batchReport.SetEnabled(isGLB);
            #endregion

            //2022-12-07 alton ���Ƥ����s�W�ɡADocType�������ʡA�קKEPExpenseClaimDetails������ƿ��~
            bool isInsert = Base.ExpenseClaim.Cache.GetStatus(claim) == PXEntryStatus.Inserted;
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimExt.usrDocType>(Base.ExpenseClaim.Cache, claim, isInsert);
        }

        public void setEPPayModi(EPExpenseClaim claim)
        {
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            EPPaymentModiReqCCP ccp = ReqCCP.Select();
            EPPaymentModiReqCCR ccr = ReqCCR.Select();
            EPPaymentModiReqNMP nmp = ReqNMP.Select();
            EPPaymentModiReqNMR nmr = ReqNMR.Select();
            EPPaymentModiReqTT tt = ReqTT.Select();


            if (ccp != null)
            {
                claimExt.IsCCP = true;
            }
            else
            {
                claimExt.IsCCP = false;
            }
            if (ccr != null)
            {
                claimExt.IsCCR = true;
            }
            else
            {
                claimExt.IsCCR = false;
            }
            if (nmp != null)
            {
                claimExt.IsNMP = true;
            }
            else
            {
                claimExt.IsNMP = false;
            }
            if (nmr != null)
            {
                claimExt.IsNMR = true;
            }
            else
            {
                claimExt.IsNMR = false;
            }
            if (tt != null)
            {
                claimExt.IsTT = true;
            }
            else
            {
                claimExt.IsTT = false;
            }

        }

        public void SetCCViewEnable(EPExpenseClaim claim)
        {
            bool isRGU = claim.GetExtension<EPExpenseClaimExt>().UsrDocType == EPStringList.EPDocType.RGU;
            bool isGUR = claim.GetExtension<EPExpenseClaimExt>().UsrDocType == EPStringList.EPDocType.GUR;
            var claimExt = claim.GetExtension<EPExpenseClaimExt>();
            editReturnDate.SetEnabled(isRGU && (claimExt.UsrApprovalStatus == EPUsrApprovalStatus.Hold || claimExt.UsrApprovalStatus == EPUsrApprovalStatus.PendingApproval));
            if (isRGU)
            {
                if (claim.Status.Equals(EPExpenseClaimStatus.HoldStatus))
                {
                    PXUIFieldAttribute.SetReadOnly(CCView.Cache, null, true);
                    PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetails.contractID>(CCView.Cache, null, false);
                    PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetails.tranDesc>(CCView.Cache, null, false);
                    PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetailsExt.usrGuarReceviableCD>(CCView.Cache, null, false);
                    PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetails.inventoryID>(CCView.Cache, null, false);
                    PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetailsExt.usrApprovalLevelID>(CCView.Cache, null, false);
                }
                else if (claim.Status.Equals(EPExpenseClaimStatus.ApprovedStatus))
                {
                    CCView.AllowUpdate = CCView.Cache.AllowUpdate = true;
                }
            }
            if (isGUR)
            {
                PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetailsExt.usrAuthDate>(CCView.Cache, null, false);
                PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetailsExt.usrDueDate>(CCView.Cache, null, false);
                PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetailsExt.usrGuarClass>(CCView.Cache, null, false);
                PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetailsExt.usrGuarType>(CCView.Cache, null, false);
                PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetailsExt.usrPONbr>(CCView.Cache, null, false);
                PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetailsExt.usrIssueDate>(CCView.Cache, null, false);
            }
        }

        public void setDetailUI(EPExpenseClaimDetails row, EPExpenseClaim header)
        {
            EPExpenseClaimExt headerExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(header);

            bool isGUR = headerExt.UsrDocType == EPStringList.EPDocType.GUR;
            bool isRGU = headerExt.UsrDocType == EPStringList.EPDocType.RGU;
            //2021/10/19 Add BTN Type
            bool isBTN = headerExt.UsrDocType == EPStringList.EPDocType.BTN;

            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetailsExt.usrVoucherType>(Base.ExpenseClaimDetails.Cache, row, !isGUR);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetailsExt.usrVoucherType>(Base.ExpenseClaimDetails.Cache, row, !isGUR);
            PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetailsExt.usrBankAccountID>(Base.ExpenseClaimDetails.Cache, row, !isBTN);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetails.customerID>(Base.ExpenseClaimDetails.Cache, row, false);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetails.customerLocationID>(Base.ExpenseClaimDetails.Cache, row, false);
        }

        private void SetKGBillpaymentEnable(KGBillPayment row)
        {
            PXUIFieldAttribute.SetEnabled<KGBillPayment.paymentDate>(KGBillPayments.Cache, row, row.PricingType == PricingType.B);
        }

        private void setTargetVisible(string TargetType, EPExpenseClaim expenseClaim, PXCache cache)
        {
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(expenseClaim);
            if (TargetType == "C")
            {
                if (claimExt.UsrVendorID != null || claimExt.UsrVendorLocationID != null)
                {
                    Base.ExpenseClaimCurrent.SetValueExt<EPExpenseClaimExt.usrVendorID>(expenseClaim, null);
                }
                PXUIFieldAttribute.SetVisible<EPExpenseClaim.customerID>(cache, expenseClaim, true);
                PXUIFieldAttribute.SetVisible<EPExpenseClaim.customerLocationID>(cache, expenseClaim, true);
            }
            else
            {
                if (expenseClaim.CustomerID != null || expenseClaim.CustomerLocationID != null)
                {
                    Base.ExpenseClaimCurrent.SetValueExt<EPExpenseClaim.customerID>(expenseClaim, null);
                }
                PXUIFieldAttribute.SetVisible<EPExpenseClaim.customerID>(cache, expenseClaim, false);
                PXUIFieldAttribute.SetVisible<EPExpenseClaim.customerLocationID>(cache, expenseClaim, false);
            }
        }

        /// <summary>
        /// Check Budget with no project (projectID = 0)
        /// </summary>
        /// <param name="details">which current details have to check</param>
        /// <param name="header">Current Header</param>
        /// <returns>
        /// True : Budget Not Enough 
        /// False : Budget Enough
        /// </returns>
        #region Old Check GL Budget 
        /*
        private bool checkGLBudgetLine(EPExpenseClaimDetails details, EPExpenseClaim header)
        {
            EPExpenseClaimExt headerExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(header);
            //2021/03/26 Add �Y�p�����������p���N���ˬd�w��
            EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
            if (detailsExt.UsrValuationType != ValuationTypeStringList.B) return false;

            //��XLedgerID,����BalanceType ='B'��̷s�@��
            Ledger ledgerB = GetLedger("B");
            //��X���w�ⶵ��,
            //���󬰬��=ClaimDetails.ExpenseAccountID, 
            //�l���=ClaimDetails.ExpenseSubID,
            //LedgerID
            //,isRelease = True
            GLBudgetLine budgetLine = PXSelect<GLBudgetLine,
                Where<GLBudgetLine.branchID, Equal<Required<BudgetFilter.branchID>>,
                    And<GLBudgetLine.ledgerID, Equal<Required<BudgetFilter.ledgerID>>,
                        And<GLBudgetLine.finYear, Equal<Required<BudgetFilter.finYear>>,
                            And<GLBudgetLine.accountID, Equal<Required<EPExpenseClaimDetails.expenseAccountID>>,
                                And<GLBudgetLine.subID, Equal<Required<EPExpenseClaimDetails.expenseSubID>>,
                                    And<GLBudgetLine.released, Equal<True>>>>>>>,
                OrderBy<Asc<GLBudgetLine.sortOrder>>>
                .Select(Base, details.BranchID, ledgerB.LedgerID,
                                    //2021/02/19 �令��UsrFinancialYear
                                    //Mantis:0011943
                                    //((DateTime)Base.Accessinfo.BusinessDate).ToString("yyyy"),
                                    headerExt.UsrFinancialYear,
                                    details.ExpenseAccountID, details.ExpenseSubID);
            if (budgetLine == null) return false;
            GLBudgetLineExt budgetLineExt = PXCache<GLBudgetLine>.GetExtension<GLBudgetLineExt>(budgetLine);
            //�Y���w�ⶵ�ت�UsrIsBudgetCheck= true �~�ݭn�ˮ�
            if (budgetLineExt.UsrIsBudgetCheck == true)
            {
                PXResultset<GLBudgetLineDetail> linedetailset =
                        PXSelect<GLBudgetLineDetail,
                        Where<GLBudgetLineDetail.groupID, Equal<Required<GLBudgetLine.groupID>>>>
                        .Select(Base, budgetLine.GroupID);
                if (linedetailset == null) throw new Exception("�Цܹw����@�C���w��!");
                //�w���`�M
                decimal? budgetAmount = 0;
                //��~1��(EPExpenseClaim.DocDate)
                int FinYear = int.Parse(((DateTime)header.DocDate).ToString("yyyy") + "01");
                //��~���(EPExpenseClaim.DocDate)
                int FinYearAndMonth = int.Parse(((DateTime)header.DocDate).ToString("yyyyMM"));

                if (budgetLineExt.UsrBudgetCheckType == "YTD")
                {
                    //�w���`�B����~�פ@���O�Υӽ�EPExpenseClaim.DocDate�Ӥ�w��[�`(GLBudgetLineDetail.ReleasedAmount)

                    foreach (GLBudgetLineDetail lineM in linedetailset)
                    {
                        if (Int32.Parse(lineM.FinPeriodID) <= FinYearAndMonth &&
                            Int32.Parse(lineM.FinPeriodID) >= FinYear)
                        {
                            budgetAmount = budgetAmount + lineM.ReleasedAmount;
                        }
                    }
                }
                else if (budgetLineExt.UsrBudgetCheckType == "YTT")
                {
                    //�w���`�B����~�פ@����~�פQ�G��w��[�`(GLBudgetLineDetail.ReleasedAmount).

                    foreach (GLBudgetLineDetail lineY in linedetailset)
                    {
                        budgetAmount = budgetAmount + lineY.ReleasedAmount;
                    }
                }

                //�w�ϥΪ��w��
                decimal? budgetUsedAmt = 0;
                //�ɤ�[�`
                decimal? DebitTotalAmt = 0;
                //�U��[�`
                decimal? CrebitTotalAmt = 0;
                //���ڪ�����
                Ledger ledgerA = GetLedger("A");
                PXResultset<GLTran> transet =
                    PXSelect<GLTran,
                    Where<GLTran.accountID, Equal<Required<EPExpenseClaimDetails.expenseAccountID>>,
                        And<GLTran.subID, Equal<Required<EPExpenseClaimDetails.expenseSubID>>,
                            And<GLTran.branchID, Equal<Required<EPExpenseClaimDetails.branchID>>,
                                And<GLTran.ledgerID, Equal<Required<GLTran.ledgerID>>,
                                    And<GLTran.released, Equal<True>>>>>>>
                                    .Select(Base, details.ExpenseAccountID, details.ExpenseSubID, details.BranchID, ledgerA.LedgerID);
                if (transet == null) return false;
                foreach (GLTran tran in transet)
                {
                    if (int.Parse(((DateTime)tran.TranDate).ToString("yyyyMM")) >= FinYearAndMonth &&
                        int.Parse(((DateTime)tran.TranDate).ToString("yyyyMM")) <= FinYear)
                    {
                        DebitTotalAmt = DebitTotalAmt + tran.DebitAmt;
                        CrebitTotalAmt = CrebitTotalAmt + tran.CreditAmt;
                    }
                }
                budgetUsedAmt = DebitTotalAmt - CrebitTotalAmt;

                //CuryExtCost�[�`
                decimal? CuryExtCostAmt = 0;



                foreach (EPExpenseClaimDetails claimdetails in Base.ExpenseClaimDetails.Cache.Cached)
                {
                    if (claimdetails.ExpenseAccountID == details.ExpenseAccountID
                        && claimdetails.ExpenseSubID == details.ExpenseSubID)
                    {
                        CuryExtCostAmt = CuryExtCostAmt + claimdetails.CuryExtCost;

                    }
                }
                if (CuryExtCostAmt > budgetAmount - budgetUsedAmt)
                {
                    return true;
                }
                else
                    return false;

            }
            else return false;

        }
        */
        #endregion
        private bool checkGLBudgetLine(EPExpenseClaimDetails details, EPExpenseClaim header)
        {
            EPExpenseClaimExt headerExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(header);
            //2021/03/26 Add �Y�p�����������p���N���ˬd�w��
            EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
            if (detailsExt.UsrValuationType != ValuationTypeStringList.B) return false;
            int NowYear = ((DateTime)Base.Accessinfo.BusinessDate).Year;
            int FinYear = Convert.ToInt32(headerExt.UsrFinancialYear);
            //�w���`�M
            decimal? budgetAmount = 0;
            //��~1��(EPExpenseClaim.DocDate)
            int FinYear01 = int.Parse(((DateTime)header.DocDate).ToString("yyyy") + "01");
            //��~���(EPExpenseClaim.DocDate)
            int FinYearAndMonth = int.Parse(((DateTime)header.DocDate).ToString("yyyyMM"));

            if (NowYear == FinYear)
            {
                PXGraph graph = new PXGraph();
                PXResultset<KGCurrAdminBudgetV> currBset = PXSelectGroupBy<KGCurrAdminBudgetV,
                    Where<KGCurrAdminBudgetV.branchID, Equal<Required<KGCurrAdminBudgetV.branchID>>,
                    And<KGCurrAdminBudgetV.accountID, Equal<Required<KGCurrAdminBudgetV.accountID>>,
                    And<KGCurrAdminBudgetV.subid, Equal<Required<KGCurrAdminBudgetV.subid>>,
                    And<KGCurrAdminBudgetV.finYear, Equal<Required<KGCurrAdminBudgetV.finYear>>>>>>,
                    Aggregate<
                    GroupBy<KGCurrAdminBudgetV.accountID,
                    GroupBy<KGCurrAdminBudgetV.subid,
                    GroupBy<KGCurrAdminBudgetV.finPeriodID,
                    GroupBy<KGCurrAdminBudgetV.branchID>>>>>>
                    .Select(graph, details.BranchID, details.ExpenseAccountID, details.ExpenseSubID, FinYear);
                if (currBset.Count == 0) return false;
                foreach (KGCurrAdminBudgetV currB in currBset)
                {
                    if (currB.BudgetCheckType == "YTD")
                    {
                        if (Int32.Parse(currB.FinPeriodID) <= FinYearAndMonth &&
                           Int32.Parse(currB.FinPeriodID) >= FinYear01)
                        {
                            budgetAmount = budgetAmount + currB.RemainBudgetAmt;
                        }
                    }
                    else if (currB.BudgetCheckType == "YTT")
                    {
                        budgetAmount = budgetAmount + currB.RemainBudgetAmt;
                    }
                }

            }
            else if (NowYear - 1 == FinYear)
            {
                PXResultset<KGPastAdminBudgetV> pastBset = PXSelectGroupBy<KGPastAdminBudgetV,
                    Where<KGPastAdminBudgetV.branchID, Equal<Required<KGPastAdminBudgetV.branchID>>,
                    And<KGPastAdminBudgetV.accountID, Equal<Required<KGPastAdminBudgetV.accountID>>,
                    And<KGPastAdminBudgetV.subid, Equal<Required<KGPastAdminBudgetV.subid>>,
                    And<KGPastAdminBudgetV.finYear, Equal<Required<KGPastAdminBudgetV.finYear>>>>>>,
                    Aggregate<
                    GroupBy<KGPastAdminBudgetV.accountID,
                    GroupBy<KGPastAdminBudgetV.subid,
                    GroupBy<KGPastAdminBudgetV.finYear,
                    GroupBy<KGPastAdminBudgetV.branchID>>>>>>
                    .Select(Base, details.BranchID, details.ExpenseAccountID, details.ExpenseSubID, FinYear);
                if (pastBset.Count == 0) return false;

                foreach (KGPastAdminBudgetV pastB in pastBset)
                {
                    if (pastB.BudgetCheckType == "YTD")
                    {
                        if (Int32.Parse(pastB.FinPeriodID) <= FinYearAndMonth &&
                           Int32.Parse(pastB.FinPeriodID) >= FinYear01)
                        {
                            budgetAmount = budgetAmount + pastB.RemainBudgetAmt;
                        }
                    }
                    else if (pastB.BudgetCheckType == "YTT")
                    {
                        budgetAmount = budgetAmount + pastB.RemainBudgetAmt;
                    }
                }
            }
            else if (NowYear + 1 == FinYear)
            {
                PXResultset<KGNextAdminBudgetV> nextBset = PXSelectGroupBy<KGNextAdminBudgetV,
                    Where<KGNextAdminBudgetV.branchID, Equal<Required<KGNextAdminBudgetV.branchID>>,
                    And<KGNextAdminBudgetV.accountID, Equal<Required<KGNextAdminBudgetV.accountID>>,
                    And<KGNextAdminBudgetV.subid, Equal<Required<KGNextAdminBudgetV.subid>>,
                    And<KGNextAdminBudgetV.finYear, Equal<Required<KGNextAdminBudgetV.finYear>>>>>>,
                    Aggregate<
                    GroupBy<KGNextAdminBudgetV.accountID,
                    GroupBy<KGNextAdminBudgetV.subid,
                    GroupBy<KGNextAdminBudgetV.finYear,
                    GroupBy<KGNextAdminBudgetV.branchID>>>>>>
                    .Select(Base, details.BranchID, details.ExpenseAccountID, details.ExpenseSubID, FinYear);
                if (nextBset.Count == 0) return false;

                foreach (KGNextAdminBudgetV nextB in nextBset)
                {
                    if (nextB.BudgetCheckType == "YTD")
                    {
                        if (Int32.Parse(nextB.FinPeriodID) <= FinYearAndMonth &&
                           Int32.Parse(nextB.FinPeriodID) >= FinYear01)
                        {
                            budgetAmount = budgetAmount + nextB.RemainBudgetAmt;
                        }
                    }
                    else if (nextB.BudgetCheckType == "YTT")
                    {
                        budgetAmount = budgetAmount + nextB.RemainBudgetAmt;
                    }
                }
            }
            //CuryExtCost�[�`
            decimal? CuryExtCostAmt = 0;
            foreach (EPExpenseClaimDetails claimdetails in Base.ExpenseClaimDetails.Cache.Cached)
            {
                if (claimdetails.ExpenseAccountID == details.ExpenseAccountID
                    && claimdetails.ExpenseSubID == details.ExpenseSubID)
                {
                    CuryExtCostAmt = CuryExtCostAmt + claimdetails.CuryExtCost;

                }
            }
            if (budgetAmount < 0)
            {
                Base.ExpenseClaimDetails.Cache.RaiseExceptionHandling<EPExpenseClaimDetails.curyExtCost>(details, details,
                    new PXSetPropertyException("�w�⤣��, �Ѿl�w����B��" + decimal.Round(((budgetAmount ?? 0) + (details.CuryExtCost ?? 0))).ToString(), PXErrorLevel.RowError));

                return true;
            }
            else
                return false;


        }

        /// <summary>
        /// Check Budget with  project (projectID != 0)
        /// </summary>
        /// <param name="details">which current details have to check</param>
        /// <param name="header">Current Header</param>
        /// <returns>
        /// True : Budget Not Enough 
        /// False : Budget Enough
        /// </returns>
        private bool checkPMBudgetLine(EPExpenseClaimDetails details, EPExpenseClaim header)
        {
            //2021/03/26 Add �Y�p�����������p���N���ˬd�w��
            EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
            if (detailsExt.UsrValuationType != ValuationTypeStringList.B) return false;

            KGSetUp kGSetUp = GetKGSetUp();
            KGSetUpExt kGSetUpExt = PXCache<KGSetUp>.GetExtension<KGSetUpExt>(kGSetUp);
            PMCostBudget costBudget = GetPMCostBudget(details.ContractID, kGSetUpExt.UsrKGPrjManageInventoryID);
            if (costBudget == null)
                throw new Exception("�M�ץ��s�C�޲z�O�ιw��!");

            //CuryExtCost�[�`
            decimal? CuryExtCostAmt = 0;

            PXResultset<EPExpenseClaimDetails> claimDetailset =
                PXSelect<EPExpenseClaimDetails,
                    Where<EPExpenseClaimDetails.expenseAccountID, Equal<Required<EPExpenseClaimDetails.expenseAccountID>>,
                        And<EPExpenseClaimDetails.expenseSubID, Equal<Required<EPExpenseClaimDetails.expenseSubID>>,
                            And<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>>>
                            .Select(Base, details.ExpenseAccountID, details.ExpenseSubID, header.RefNbr);
            foreach (EPExpenseClaimDetails claimdetails in claimDetailset)
            {
                CuryExtCostAmt = CuryExtCostAmt + claimdetails.CuryExtCost;
            }

            //�ӽЪ��B�j��w��l�B
            if (CuryExtCostAmt > costBudget.CuryVarianceAmount)
            {
                Base.ExpenseClaimDetails.Cache.RaiseExceptionHandling<EPExpenseClaimDetails.curyExtCost>(details, details,
                    new PXSetPropertyException("�w�⤣��, �Ѿl�w����B��" + costBudget.CuryVarianceAmount.ToString(), PXErrorLevel.RowError));
                return true;
            }
            else
                return false;

        }

        /*--Mark 2021-08-02 :12179
        //2021/02/08 Add Mantis:0011941
        /// <summary>
        /// Cal Payment Amount
        /// </summary>
        public void setPaymentAmount()
        {
            //KGBillSummary sum = getKGBillSummary();
            EPExpenseClaim header = Base.ExpenseClaim.Current;
            Decimal total = 0;
            decimal? pct = 0;
            foreach (KGBillPayment kgBillPayment in KGBillPayments.Cache.Cached)
            {
                decimal paymentAmount = (header.CuryDocBal ?? 0) * (kgBillPayment.PaymentPct ?? 0) / 100;
                paymentAmount = System.Math.Round(paymentAmount, 0, MidpointRounding.AwayFromZero);
                total += paymentAmount;
                pct = kgBillPayment.PaymentPct + pct;

                if (!(header.CuryDocBal ?? 0).Equals(total) && pct == 100)
                {
                    paymentAmount -= (total - (header.CuryDocBal ?? 0m));
                }
                KGBillPayments.Cache.SetValueExt<KGBillPayment.paymentAmount>(kgBillPayment, paymentAmount);
                //KGBillPayments.Update(kgBillPayment);
            }

        }*/

        //2021/02/18 Add Mantis:0011943
        /// <summary>
        /// Check : Whether the FinYear is the same as the year of the date
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="date"></param>
        private void checkFinYear(string FinYear, DateTime? date)
        {
            EPExpenseClaim header = Base.ExpenseClaim.Current;
            if (header == null) return;
            EPExpenseClaimExt headerExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(header);
            if (FinYear == null || FinYear == "")
                Base.ExpenseClaim.Cache.RaiseExceptionHandling<EPExpenseClaimExt.usrFinancialYear>
                        (header, headerExt.UsrFinancialYear,
                        new Exception("�йw��~��������!"));
            else
            {
                if (headerExt.UsrDocType == EPStringList.EPDocType.STD)
                {
                    string dateYear = date?.ToString("yyyy");
                    if (FinYear != dateYear)
                    {

                        Base.ExpenseClaim.Cache.RaiseExceptionHandling<EPExpenseClaimExt.usrFinancialYear>
                            (header, headerExt.UsrFinancialYear,
                            new Exception("���ˬd����O�_�P�w�⪺�~���ۦP!"));
                    }
                }

            }

        }

        /// <summary>
        /// Mantis: 0012266
        /// Check when TempWriteoff EPDetails just can have one row
        /// </summary>
        private void checkTempEPDetails()
        {
            int count = Base.ExpenseClaimDetails.Select().Count;
            if (count > 1)
            {
                throw new Exception("�ȥI�ڥӽХu�঳�@������!");
            }
        }

        private void UpdateModifyAmt(string ModiPaymentType, int? OldValue, int? NewValue)
        {
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);

            switch (ModiPaymentType)
            {
                case PaymentType.NMP:
                    if (OldValue != null)
                    {
                        NMPayableCheck oldcheck = GetNMPayableCheck(OldValue);
                        if (oldcheck != null)
                            claimExt.UsrAmtNMP = claimExt.UsrAmtNMP ?? 0 - oldcheck.OriCuryAmount;
                    }
                    if (NewValue != null)
                    {
                        NMPayableCheck payableCheck = GetNMPayableCheck(NewValue);
                        claimExt.UsrAmtNMP = claimExt.UsrAmtNMP ?? 0 + payableCheck.OriCuryAmount;
                    }
                    break;
                case PaymentType.NMR:
                    if (OldValue != NewValue)
                    {
                        if (OldValue != null)
                        {
                            NMReceivableCheck oldcheck = GetNMReceivableCheck(OldValue);
                            if (oldcheck != null)
                                claimExt.UsrAmtNMR = claimExt.UsrAmtNMR ?? 0 - oldcheck.OriCuryAmount;
                        }
                        if (NewValue != null)
                        {
                            NMReceivableCheck receivableCheck = GetNMReceivableCheck(NewValue);
                            claimExt.UsrAmtNMR = claimExt.UsrAmtNMR ?? 0 + receivableCheck.OriCuryAmount;
                        }
                    }
                    break;
                case PaymentType.TT:
                    if (OldValue != NewValue)
                    {
                        if (OldValue != null)
                        {
                            KGBillPayment oldcheck = GetKGBillPayment(OldValue);
                            if (oldcheck != null)
                                claimExt.UsrAmtTT = claimExt.UsrAmtTT ?? 0 - oldcheck.PaymentAmount;
                        }
                        if (NewValue != null)
                        {
                            KGBillPayment payment = GetKGBillPayment(NewValue);
                            claimExt.UsrAmtTT = claimExt.UsrAmtTT ?? 0 + payment.PaymentAmount;
                        }
                    }
                    break;
            }

        }

        private void UpdateModifyAmt(string ModiPaymentType, string OldValue, string NewValue)
        {
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);

            switch (ModiPaymentType)
            {
                case PaymentType.CCP:
                    if (OldValue != null)
                    {
                        CCPayableCheck oldcheck = GetCCPayableCheck(OldValue);
                        if (oldcheck != null)
                            claimExt.UsrAmtCCP = claimExt.UsrAmtCCP ?? 0 - oldcheck.GuarAmt;
                    }
                    if (NewValue != null)
                    {
                        CCPayableCheck payableCheck = GetCCPayableCheck(NewValue);
                        claimExt.UsrAmtCCP = claimExt.UsrAmtCCP ?? 0 + payableCheck.GuarAmt;
                    }
                    break;
                case PaymentType.CCR:
                    if (OldValue != NewValue)
                    {
                        if (OldValue != null)
                        {
                            CCReceivableCheck oldcheck = GetCCReceivableCheck(OldValue);
                            if (oldcheck != null)
                                claimExt.UsrAmtCCR = claimExt.UsrAmtCCR ?? 0 - oldcheck.GuarAmt;
                        }
                        if (NewValue != null)
                        {
                            CCReceivableCheck receivableCheck = GetCCReceivableCheck(NewValue);
                            claimExt.UsrAmtCCR = claimExt.UsrAmtCCR ?? 0 + receivableCheck.GuarAmt;
                        }
                    }
                    break;

            }
        }

        private void CheckGLBAmtBalance()
        {
            if (GLBView.Select() == null) return;
            decimal? CreditAmt = 0;
            decimal? DebitAmt = 0;

            foreach (KGExpenseVoucher voucher in GLBView.Select())
            {
                CreditAmt = CreditAmt + (voucher.CreditAmt ?? 0);
                DebitAmt = DebitAmt + (voucher.DebitAmt ?? 0);
            }
            if (CreditAmt != DebitAmt)
                throw new Exception("�ɶU������, �нT�{!");
        }

        private void SetUsrValuationTypeList() {

            PXStringListAttribute.SetList<EPExpenseClaimDetailsExt.usrValuationType>(Base.ExpenseClaimDetails.Cache,null,
                new string[] { 
                    ValuationTypeStringList.A, ValuationTypeStringList.D, 
                    ValuationTypeStringList.R, ValuationTypeStringList.P,
                    ValuationTypeStringList.B, ValuationTypeStringList.WITH_TAX,//20230103 mantis:12386 W -> WITH_TAX(T)
                    ValuationTypeStringList.SECOND, ValuationTypeStringList.L, //20230103 mantis:12386 S -> SECOND(2)
                    ValuationTypeStringList.H, ValuationTypeStringList.N, 
                    ValuationTypeStringList.C },
                new string[] {"�[��", "����", "�O�d�ڰh�^", "�w�I��", "�p��",  "�N���|", "���O�ɥR","�ҫO�O","���O�O","�h���","���K" }
                );
        }
        #endregion

        #region Select Table
        public virtual PXResultset<GVApGuiInvoiceFinRef> GetAllInvoiceByNbrNotThisID(string invoiceNbr, int? thisGuiInvoiceID)
        {
            return PXSelect<GVApGuiInvoiceFinRef,
                Where<GVApGuiInvoiceFinRef.guiInvoiceNbr, Equal<Required<GVApGuiInvoiceFinRef.guiInvoiceNbr>>
                            , And<GVApGuiInvoiceFinRef.guiInvoiceID, NotEqual<Required<GVApGuiInvoiceFinRef.guiInvoiceID>>,
                            And<GVApGuiInvoiceFinRef.invoiceType, Equal<Required<GVApGuiInvoiceFinRef.invoiceType>>>>>>
                            .Select(Base, invoiceNbr, thisGuiInvoiceID, GVList.GVGuiInvoiceType.INVOICE);
        }

        private InventoryItem GetInventory(int? InventoryID)
        {
            return PXSelect<InventoryItem,
                Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                .Select(Base, InventoryID);
        }
        private Location GetDefLocation(int? BAccountID)
        {
            BAccount2 account2 = GetBAccount(BAccountID);
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(Base, account2.DefLocationID);
        }
        private Location GetLocationByDefLocationID(int? deflocationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(Base, deflocationID);
        }
        private BAccount2 GetBAccount(int? BAccount)
        {
            return PXSelect<BAccount2,
                Where<BAccount2.bAccountID, Equal<Required<BAccount2.bAccountID>>>>
                .Select(Base, BAccount);
        }
        private Location GetLocationID(int? VendorLocationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(Base, VendorLocationID);
        }
        private NMBankAccount GetBankAccount(int? CashAccount, string PaymentMethodID)
        {
            string paymentMethod = null;
            if (PaymentMethodID == PaymentMethod.A || PaymentMethodID == PaymentMethod.D)
            {
                paymentMethod = "CHECK";
            }
            else if (PaymentMethodID == PaymentMethod.B)
            {
                paymentMethod = "TT";
            }
            else if (PaymentMethodID == PaymentMethod.C || PaymentMethodID == PaymentMethod.E)
            {
                paymentMethod = "CASH";
            }
            if (paymentMethod == "CASH")
            {
                return PXSelect<NMBankAccount,
                    Where<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>>>
                    .Select(Base, CashAccount);
            }
            else
            {
                return PXSelect<NMBankAccount,
                Where<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>,
                And<NMBankAccount.paymentMethodID, Equal<Required<NMBankAccount.paymentMethodID>>>>>
                .Select(Base, CashAccount, paymentMethod);
            }

        }
        private NMBankAccount GetBankAccount(int? NMBankAccountID)
        {
            return PXSelect<NMBankAccount,
            Where<NMBankAccount.bankAccountID, Equal<Required<NMBankAccount.bankAccountID>>>>
            .Select(Base, NMBankAccountID);
        }

        private Contract GetContract(int? ContractID)
        {
            return PXSelect<Contract, Where<Contract.contractID, Equal<Required<EPExpenseClaimDetails.contractID>>>>
                            .Select(Base, ContractID);
        }
        private Ledger GetLedger(string BalanceType)
        {
            return PXSelect<Ledger,
                Where<Ledger.balanceType, Equal<Required<Ledger.balanceType>>>,
                OrderBy<Desc<Ledger.createdDateTime>>>
                .Select(Base, BalanceType);
        }
        private KGSetUp GetKGSetUp()
        {
            return PXSelect<KGSetUp>.Select(Base);
        }
        private PMCostBudget GetPMCostBudget(int? ProjectID, int? InventoryID)
        {
            return PXSelect<PMCostBudget,
                Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                    And<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>>>>
                    .Select(Base, ProjectID, InventoryID);
        }
        private PMCostBudget GetPMCostBudget(int? ProjectID)
        {
            return PXSelect<PMCostBudget,
                Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>>>
                    .Select(Base, ProjectID);
        }
        private CCReceivableCheck GetCCReceivableCheck(string ReceivableCD)
        {
            return PXSelect<CCReceivableCheck,
                Where<CCReceivableCheck.guarReceviableCD, Equal<Required<CCReceivableCheck.guarReceviableCD>>>>
                .Select(Base, ReceivableCD);
        }
        private CCPayableCheck GetCCPayableCheck(string PayableCD)
        {
            return PXSelect<CCPayableCheck,
                       Where<CCPayableCheck.guarPayableCD, Equal<Required<EPPaymentModiReqCCP.refNbr>>>>
                       .Select(Base, PayableCD);
        }
        private NMPayableCheck GetNMPayableCheck(int? NMID)
        {
            return PXSelect<NMPayableCheck,
                       Where<NMPayableCheck.payableCheckID, Equal<Required<EPPaymentModiReqNMP.nmID>>>>
                       .Select(Base, NMID);
        }
        private NMReceivableCheck GetNMReceivableCheck(int? NMID)
        {
            return PXSelect<NMReceivableCheck,
                       Where<NMReceivableCheck.receivableCheckID, Equal<Required<EPPaymentModiReqNMR.nmID>>>>
                       .Select(Base, NMID);
        }
        private KGBillPayment GetKGBillPayment(int? BillPaymentID)
        {
            return PXSelect<KGBillPayment,
                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Select(Base, BillPaymentID);
        }
        private KGBudApproveLevel GetBudLevel(int? BudLevelID)
        {
            return PXSelect<KGBudApproveLevel,
                Where<KGBudApproveLevel.budLevelID, Equal<Required<EPExpenseClaimDetailsExt.usrApprovalLevelID>>>>
                .Select(Base, BudLevelID);
        }
        private EPExpenseClaimDetails GetEPDetailsofValuationType(string RefNbr, string VauationType)
        {
            return PXSelect<EPExpenseClaimDetails,
                Where<EPExpenseClaimDetails.refNbr, Equal<Required<TWNWHTTran.refNbr>>,
                And<EPExpenseClaimDetailsExt.usrValuationType, Equal<Required<EPExpenseClaimDetailsExt.usrValuationType>>>>>
                .Select(Base, RefNbr, VauationType);
        }
        private Account GetAccount(int? AccountID)
        {
            return PXSelect<Account,
                Where<Account.accountID, Equal<Required<Account.accountID>>>>
                .Select(Base, AccountID);
        }
        private Sub GetSub(string SubCD)
        {
            return PXSelect<Sub,
                Where<Sub.subCD, Equal<Required<Sub.subCD>>>>
                .Select(Base, SubCD);
        }
        private EPEmployee GetEmpolyeeEPSub(int? BAccountID)
        {
            return PXSelect<EPEmployee,
                Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>
                .Select(Base, BAccountID);
        }
        private CA.CashAccount GetCashAccount(int? cashAccountID)
        {
            return PXSelect<CA.CashAccount,
                Where<CA.CashAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>>>
                .Select(Base, cashAccountID);
        }
        #endregion

        #region Delegate Methods
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                EPExpenseClaim master = Base.ExpenseClaim.Current;
                if (master != null)
                {
                    #region 2021/06/09 Move to EPWHT
                    //�]���N���|�s�ɮɷ|�h�s�WEPDetails �ɭP���B���� �ҥH�o�q�n�h��N���|�������ʤ����ˮ�
                    /*
                    EPExpenseClaimExt masterExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(master);
                    GVApGuiInvoiceFinRef invoiceFinRef = GVApGuiInvoiceRefs.Current;
                    KGBillPayment billPayment = KGBillPayments.Current;
                    //2021/02/09 Mantis: 0011925
                    if (masterExt.UsrDocType == EPStringList.EPDocType.STD)
                    {
                        //KGBillPayment Check
                        if (billPayment != null)
                        {
                            decimal? PaymentPct = 0;
                            //2021/02/09 Add Mantis:0011941
                            decimal? PaymentAmount = 0;
                            foreach (KGBillPayment epBillPayment in KGBillPayments.Select())
                            {
                                PaymentPct = epBillPayment.PaymentPct + PaymentPct;
                                PaymentAmount = PaymentAmount + epBillPayment.PaymentAmount;
                            }
                            if (PaymentPct != 100)
                                KGBillPayments.Cache.RaiseExceptionHandling<KGBillPayment.paymentPct>
                                    (billPayment, billPayment.PaymentPct, new Exception("�нT�{�I�ڤ�v��100%!"));
                            //throw new Exception("�нT�{�I�ڤ�v��100%!");
                            if (PaymentAmount != master.CuryDocBal)
                                KGBillPayments.Cache.RaiseExceptionHandling<KGBillPayment.paymentAmount>
                                    (billPayment, billPayment.PaymentAmount, new Exception("�нT�{�I�ڪ��B�P�ӽЪ��B�۲�!"));
                        }
                    }
                    */
                    #endregion
                }
                else
                {
                    //�R�� �s�ʪ�GVApGuiInvoiceRefs/KGBillPayments/TWNWHTTran
                    foreach (EPExpenseClaim claim in Base.ExpenseClaim.Cache.Deleted)
                    {
                        foreach (GVApGuiInvoiceFinRef invoice in GVApGuiInvoiceRefs.Select())
                        {
                            GVApGuiInvoiceRefs.Delete(invoice);
                        }
                        foreach (KGBillPayment payment in KGBillPayments.Select())
                        {
                            KGBillPayments.Delete(payment);
                        }

                    }
                }
                baseMethod();
                ts.Complete(Base);


                PXUIFieldAttribute.SetEnabled<EPExpenseClaimExt.usrDocType>(Base.ExpenseClaim.Cache, master, Base.ExpenseClaim.Cache.GetStatus(master) == PXEntryStatus.Inserted);
            }
        }

        public delegate void SubmitDelegate();
        [PXOverride]
        public void Submit(SubmitDelegate baseMethod)
        {
            EPExpenseClaim master = Base.ExpenseClaim.Current;

            if (master == null)
            {
                baseMethod();
            }
            else
            {
                EPExpenseClaimExt masterExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(master);

                if (masterExt.UsrDocType == EPStringList.EPDocType.STD)
                {
                    KGBillPayment billPayment = KGBillPayments.Select();

                    if (billPayment == null)
                        throw new Exception("�к��@�I�ڤ覡!");
                    //2022-10-28 :12363 alton 
                    bool checkTT = true;
                    foreach (KGBillPayment payment in KGBillPayments.Select())
                    {
                        try
                        {
                            NMLocationUtil.CheckKGBillPaymentLocation(payment);
                        }
                        catch (Exception e)
                        {
                            checkTT = false;
                            SetError<KGBillPayment.vendorLocationID>(KGBillPayments.Cache, payment, payment.VendorLocationID, e.Message);
                        }
                    }
                    if (!checkTT)
                        throw new PXException("���ˬd�I�ڤ覡!");

                    Base.Persist();
                    //2021/07/13 Add Mantis: 0011824
                    foreach (EPExpenseClaimDetails details in Base.ExpenseClaimDetails.Cache.Cached)
                    {
                        //�M�׬�X
                        if (details.ContractID == 0)
                        {
                            if (checkGLBudgetLine(details, master))
                                return;
                        }
                        //�M�פ���X
                        else
                        {
                            if (checkPMBudgetLine(details, master))
                                return;
                        }
                    }
                }
                ////2021/02/09 Mantis:0011925 �����,��CCReceivableCheck���A�אּAppliedRTN
                //if (masterExt.UsrDocType == EPStringList.EPDocType.RGU)
                //{
                //    //PXResultset<EPExpenseClaimDetails> detailset =
                //    //    PXSelect<EPExpenseClaimDetails,
                //    //    Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                //    //    .Select(Base, master.RefNbr);

                //    foreach (EPExpenseClaimDetails details in Base.ExpenseClaimDetails.Select())
                //    {
                //        string guarRecCD = details.GetExtension<EPExpenseClaimDetailsExt>().UsrGuarReceviableCD;

                //        CC.CCReceivableEntry.CreateAppliedRTNVoucher(guarRecCD);

                //        PXUpdate<Set<CCReceivableCheck.status, CCList.CCReceivableStatus.appliedRTN>,
                //                 CCReceivableCheck,
                //                 Where<CCReceivableCheck.guarReceviableCD, Equal<Required<EPExpenseClaimDetailsExt.usrGuarReceviableCD>>>>
                //                 .Update(Base, guarRecCD);
                //    }
                //}
                //2021/08/24 Mantis: 0012204
                //When EPDocType = GLB, Check Amy Balance
                if (masterExt.UsrDocType == EPStringList.EPDocType.GLB)
                    CheckGLBAmtBalance();
                //2021/11/02 Mantis: 0012256
                //if Doctype = 'BTN' , Check CuryDocBal = 0
                //20220808 ���ĥ�����ˬd�ɶU
                /**
                if (masterExt.UsrDocType == EPStringList.EPDocType.BTN)
                {
                    if (master.CuryDocBal != 0)
                        throw new Exception("�ɶU����,�L�k����!");
                }**/

                //2022/03/11 Mantis: 0012291
                baseMethod();

                if (masterExt.UsrDocType == EPStringList.EPDocType.STD && masterExt.UsrSkipFlowFlag != true)
                {
                    //Agent Flow ID:
                    //1. Value in Employee Attribute (ID= AGENTID)
                    //2. Username in User
                    // if Username contains "\" (eg.kindom.com.tw\yuhsiang_tseng) get substring after "\"
                    // else get whole Username

                    ExpenseClaimEntry expenseClaimEntry = PXGraph.CreateInstance<ExpenseClaimEntry>();
                    string companyID = PX.Data.Update.PXInstanceHelper.CurrentCompany.ToString();
                    string afmNo = master.RefNbr;
                    string attributeID = "AGENTID";
                    string agentFlowID = SelectFrom<CSAnswers>.LeftJoin<BAccount>.On<BAccount.noteID.IsEqual<CSAnswers.refNoteID>>.Where<BAccount.bAccountID.IsEqual<@P.AsInt>.And<CSAnswers.attributeID.IsEqual<@P.AsString>>>.View.Select(expenseClaimEntry, master.EmployeeID, attributeID).TopFirst?.Value;
                    if (agentFlowID == null)
                    {
                        agentFlowID = Base.Accessinfo.UserName;//PXAccess.GetUserName();
                        if (agentFlowID.Contains("\\"))
                        {
                            string[] temp = agentFlowID.Split('\\');
                            if (temp[1] != null) agentFlowID = temp[1];
                        }
                    }
                    string dBname = "KedgeTest";
                    int? contractID = SelectFrom<EPExpenseClaimDetails>.Where<EPExpenseClaimDetails.refNbr.IsEqual<@P.AsString>>.View.Select(expenseClaimEntry, master.RefNbr).TopFirst?.ContractID;
                    string tempMessage = null;
                    string status = null;
                    if (companyID != null && afmNo != null && agentFlowID != null && dBname != null)
                    {
                        AgentFlowWebServiceUtil.createProccessBill(companyID, afmNo, agentFlowID, dBname, contractID, ref status, ref tempMessage);

                        //KGSetUp�S��������T

                        if (status == null || status.Contains("Fail"))
                        {
                            //Call Base Edit ("Put On Hold")
                            Base.edit.Press();

                            masterExt.UsrApprovalStatus = EPUsrApprovalStatus.Hold;
                            Base.ExpenseClaim.Update(master);
                            //Base.Actions.PressSave();

                            Base.ExpenseClaim.Cache.RaiseExceptionHandling<EPExpenseClaimExt.usrApprovalStatus>(
                                master, masterExt.UsrApprovalStatus,
                                new PXSetPropertyException(tempMessage));
                        }
                        else
                        {
                            masterExt.UsrApprovalStatus = EPUsrApprovalStatus.PendingApproval;
                            Base.ExpenseClaim.Update(master);
                        }
                    }
                    masterExt.UsrApprovalStatus = EPUsrApprovalStatus.PendingApproval;
                    Base.ExpenseClaim.Update(master);
                }
                else
                {
                    masterExt.UsrApprovalStatus = EPUsrApprovalStatus.Approved;
                    Base.ExpenseClaim.Update(master);
                    //Base.Actions.PressSave();
                }

            }
        }
        /**
        public delegate void ReleaseDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable Release(PXAdapter adapter, ReleaseDelegate baseMethod)
        {
            baseMethod(adapter);

            //2021/02/09 Mantis:0011925 �����,��CCReceivableCheck���A�אּAppliedRTN
            if (Base.ExpenseClaim.Current?.GetExtension<EPExpenseClaimExt>()?.UsrDocType == EPStringList.EPDocType.RGU)
            {
                foreach (EPExpenseClaimDetails details in Base.ExpenseClaimDetails.Select())
                {
                    CC.CCReceivableEntry.CreateAppliedRTNVoucher(details.GetExtension<EPExpenseClaimDetailsExt>().UsrGuarReceviableCD);
                }
            }

            return adapter.Get();
        }**/

        public delegate void EditDelegate();
        [PXOverride]
        public virtual void Edit(EditDelegate baseMethod)
        {

            if (Base.ExpenseClaim.Current != null)
            {
                EPExpenseClaim master = Base.ExpenseClaim.Current;
                EPExpenseClaimExt masterExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(master);
                masterExt.UsrApprovalStatus = EPUsrApprovalStatus.Hold;
            }
            baseMethod();
        }

        #endregion

        #region Cache Attached

        #region EPClaim
        //2021/10/22 Delete Mantis: 0012256 Phil�����Ω�
        //2021/10/18 Add Mantis: 0012256
        /*
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIVisible(typeof(Where<Current<EPExpenseClaimExt.usrDocType>, NotEqual<EPStringList.EPDocType.btn>>))]
        protected void _(Events.CacheAttached<EPExpenseClaim.curyDocBal> e)
        {
        }
        */
        #endregion

        #region EPClaimDetails
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<Current<EPExpenseClaimExt.usrDocType>, Equal<EPStringList.EPDocType.gur>>))]
        protected void EPExpenseClaimDetails_CuryUnitCost_CacheAttached(PXCache sender)
        {
        }

        //2021/08/13 Add Mantis: 0012197 �Y��������L�ӽЩζǲ��@�~ InventoryID�i�H�D��O�ΥH�~��
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXRestrictor(typeof(Where<Where<Current<EPExpenseClaimExt.usrDocType>, In3<EPDocType.glb, EPDocType.oth>,
            Or<InventoryItem.itemType, Equal<INItemTypes.expenseItem>>>>),
            Messages.InventoryItemIsNotAnExpenseType)]
        protected void EPExpenseClaimDetails_InventoryID_CacheAttached(PXCache sender)
        {
        }

        //2021/10/19 Add Mantis: 0012256 
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXFormula(typeof(Switch<Case<Where<Current<EPExpenseClaimDetailsExt.usrBankAccountID>, IsNull>,
                                    Selector<EPExpenseClaimDetails.inventoryID, InventoryItem.cOGSAcctID>>,
                                    Selector<EPExpenseClaimDetailsExt.cashAccountID, CA.CashAccount.accountID>>))]
        protected void _(Events.CacheAttached<EPExpenseClaimDetails.expenseAccountID> e)
        {
        }

        //2021/10/19 Add Mantis: 0012256 
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXFormula(typeof(Switch<Case<Where<Current<EPExpenseClaimDetailsExt.usrBankAccountID>, IsNull>,
                                    Selector<EPExpenseClaimDetails.inventoryID, InventoryItem.cOGSSubID>>,
                                    Selector<EPExpenseClaimDetailsExt.cashAccountID, CA.CashAccount.subID>>))]
        protected void _(Events.CacheAttached<EPExpenseClaimDetails.expenseSubID> e)
        {
        }

        #endregion

        #region KGBillPayment
        //[PXDBString(30)]
        //[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXUIField(DisplayName = "AP Ref. Nbr.")]
        //protected void KGBillPayment_RefNbr_CacheAttached(PXCache sender) { }
        #region UsrEPRefNbr
        [PXDBString(15)]
        [PXUIField(DisplayName = "EPRefNbr")]
        [PXDBDefault(typeof(EPExpenseClaim.refNbr))]
        [PXParent(typeof(Select<EPExpenseClaim,
                    Where<EPExpenseClaim.refNbr,
                    Equal<Current<KGBillPaymentExt.usrEPRefNbr>>>>))]
        protected void KGBillPayment_UsrEPRefNbr_CacheAttached(PXCache sender) { }
        #endregion

        #region VendorID
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Vendor ID", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        protected void KGBillPayment_VendorID_CacheAttached(PXCache sender) { }
        #endregion

        #region VendorLocationID
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault()]
        [PXSelectorWithCustomOrderBy(typeof(Search2<Location.locationID,
            InnerJoin<BAccount2, On<BAccount2.bAccountID, Equal<Location.bAccountID>>>,
            Where<True, Equal<True>,
                And<Current<KGBillPayment.vendorID>, Equal<Location.bAccountID>,
                And<Where2<
                    Where<BAccount2.type, NotEqual<BAccountType.employeeType>,
                        And<Where<Current<KGBillPayment.paymentMethod>,
                            //2021/06/24 Add Auth same with Cash
                            //2021/08/02 Add TempWriteoff same with Cash
                            In3<PaymentMethod.cash, PaymentMethod.auth>,
                    Or2<Where<Current<KGBillPayment.paymentMethod>, Equal<PaymentMethod.wireTransfer>,
                        And<Location.vPaymentMethodID, Equal<word.tt>>>,
                    Or<Where<Current<KGBillPayment.paymentMethod>,
                        In3<PaymentMethod.check, PaymentMethod.giftCertificate>,
                        And<Location.vPaymentMethodID, Equal<word.check>>>
                    >>>>>,
                Or<Where<BAccount2.type, Equal<BAccountType.employeeType>>>>>
                >>,
            //2021/05/20 Add Mantis:0012048
            OrderBy<Desc<LocationFinExt.usrIsReserveAcct, Asc<Location.locationCD>>>>),
            //2021/05/11 Add to Show
            typeof(LocationFinExt.usrIsReserveAcct),
            typeof(Location.locationCD),
            typeof(BAccount2.acctCD),
            typeof(BAccount2.acctName),

            SubstituteKey = typeof(Location.locationCD))]
        protected void KGBillPayment_VendorLocationID_CacheAttached(PXCache sender) { }
        #endregion

        //20210121 Take off mantis:0011899
        /*[PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Ref. Nbr.")]
        [PXUIRequired(typeof(Where<Current<EPExpenseClaimExt.usrIsGuarantee>,Equal<True>>))]
        protected void EPExpenseClaim_ExpenseRefNbr_CacheAttached(PXCache sender)
        {
        }*/
        #region BankAccountID
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account CD", Visible = true)]
        [PXSelector(typeof(Search2<NMBankAccount.bankAccountID,
            InnerJoin<CA.CashAccount, On<CA.CashAccount.cashAccountID, Equal<NMBankAccount.cashAccountID>>>,
            Where2<
                Where<Current<KGBillPayment.paymentMethod>, In3<PaymentMethod.check, PaymentMethod.giftCertificate>,
                    And<NMBankAccount.paymentMethodID, Equal<KG.Util.KGConst.check>>>,
                Or2<Where<Current<KGBillPayment.paymentMethod>, Equal<PaymentMethod.wireTransfer>,
                    And<NMBankAccount.paymentMethodID, Equal<KG.Util.KGConst.tt>>>,
                Or<Where<Current<KGBillPayment.paymentMethod>,
                    //2021/06/24 Add Auth same with Cash
                    //2021/08/02 Add TempWriteoff same with Cash
                    In3<PaymentMethod.cash, PaymentMethod.auth>>>>>>),
            typeof(NMBankAccount.bankAccountCD),
            typeof(CA.CashAccount.descr),
            typeof(NMBankAccount.bankCode),
            typeof(NMBankAccount.bankName),
            DescriptionField = typeof(NMBankAccount.bankName),
            SubstituteKey = typeof(NMBankAccount.bankAccountCD))]
        protected void KGBillPayment_BankAccountID_CacheAttached(PXCache sender) { }
        #endregion

        #region PaymentPct
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Payment Pct")]
        [PXDefault("100")]
        protected void KGBillPayment_PaymentPct_CacheAttached(PXCache sender) { }
        #endregion

        #region PaymentAmount
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(typeof(Current<EPExpenseClaim.curyDocBal>))]
        protected void KGBillPayment_PaymentAmount_CacheAttached(PXCache sender) { }
        #endregion

        #region PaymentDate
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(typeof(Current<EPExpenseClaim.docDate>))]
        //[PXUIEnabled(typeof(Where<Current<KGBillPayment.pricingType>, Equal<PricingType.other>>))]
        protected void _(Events.CacheAttached<KGBillPayment.paymentDate> e) { }
        #endregion

        #region IsPostageFree
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Is Postage Free")]
        [PXDefault(typeof(True), PersistingCheck = PXPersistingCheck.Nothing)]
        protected void KGBillPayment_IsPostageFree_CacheAttached(PXCache sender) { }
        #endregion

        #region BillPaymentID
        /// <summary>
        /// Enable the primary key definition as DB.
        /// </summary>
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBIdentity(IsKey = true)]
        protected virtual void _(Events.CacheAttached<KGBillPayment.billPaymentID> e) { }
        #endregion

        #region DocType
        /// <summary>
        /// Remove one of PK field conditions.
        /// </summary>
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBString(3, IsFixed = true)]
        [PXDBDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<KGBillPayment.docType> e) { }
        #endregion

        #region RefNbr
        /// <summary>
        /// Remove one of PK field conditions.
        /// </summary>
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBString(30, IsUnicode = true)]
        [PXDBDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<KGBillPayment.refNbr> e) { }
        #endregion

        #region LineNbr
        /// <summary>
        /// Remove one of PK field conditions.
        /// </summary>
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBInt()]
        protected virtual void _(Events.CacheAttached<KGBillPayment.lineNbr> e) { }
        #endregion

        #endregion

        #region GVApGuiInvoiceFinRef
        #region Remark 
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]

        protected void GVApGuiInvoiceFinRef_Remark_CacheAttached(PXCache sender) { }
        #endregion

        #region DeductionCode 
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault("1")]
        protected void GVApGuiInvoiceFinRef_DeductionCode_CacheAttached(PXCache sender) { }
        #endregion

        #region Status
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(APInvoiceStatus.Hold)]
        protected void GVApGuiInvoiceFinRef_Status_CacheAttached(PXCache sender) { }
        #endregion

        #region InvoiceType
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Invoice Type", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        protected void GVApGuiInvoiceFinRef_InvoiceType_CacheAttached(PXCache sender) { }
        #endregion

        #region VendorName
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Vendor Name", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        protected void GVApGuiInvoiceFinRef_VendorName_CacheAttached(PXCache sender) { }
        #endregion

        #region VendorUniformNumber
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBString(InputMask = "00000000")]
        [PXUIField(DisplayName = "VendorUniformNumber")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<GVApGuiInvoiceRef.invoiceType, Equal<GVList.GVGuiInvoiceType.invoice>>))]
        protected void GVApGuiInvoiceFinRef_VendorUniformNumber_CacheAttached(PXCache sender) { }
        #endregion

        #region Vendor
        [PXDBInt]
        [PXUIField(DisplayName = "Vendor")]
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXSelector(typeof(Search<BAccount2.bAccountID, Where<BAccount2.type,
                In3<BAccountType.vendorType, BAccountType.employeeType>,
            And<BAccount2.status, Equal<EPConst.active>>>>),
            typeof(BAccount2.acctCD),
            typeof(BAccount2.acctName),
            typeof(BAccount2.status),
            SubstituteKey = typeof(BAccount2.acctCD), DescriptionField = typeof(BAccount2.acctName))]
        [PXUIRequired(typeof(False))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        protected void GVApGuiInvoiceFinRef_Vendor_CacheAttached(PXCache sender) { }
        #endregion

        #region GuiInvoiceNbr
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBString(40, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXUIRequired(typeof(True))]
        protected void GVApGuiInvoiceFinRef_GuiInvoiceNbr_CacheAttached(PXCache sender) { }
        #endregion

        #region SalesAmt
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Sales Amt", Enabled = true)]
        protected void GVApGuiInvoiceFinRef_SalesAmt_CacheAttached(PXCache sender) { }
        #endregion

        #region Hold 
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(true)]
        protected void GVApGuiInvoiceFinRef_Hold_CacheAttached(PXCache sender) { }
        #endregion

        #endregion

        #endregion

        #region Import
        public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            isPreRowImport = true; return true;
        }

        bool isPreRowImport = false;
        public bool RowImporting(string viewName, object row) => row == null;

        public bool RowImported(string viewName, object row, object oldRow) => oldRow == null;

        public void PrepareItems(string viewName, IEnumerable items) { }
        #endregion

        #region Dialog
        public class EditTempTable : IBqlTable
        {
            #region ReturnDate
            [PXDate()]
            [PXUIField(DisplayName = "Return Date")]
            public virtual DateTime? ReturnDate { get; set; }
            public abstract class returnDate : BqlType<IBqlDateTime, string>.Field<returnDate> { }
            #endregion
        }
        #endregion

        #region 2020/09/23 ���ݭn
        /*public PXAction<EPExpenseClaim> changeOkV;
         [PXUIField(DisplayName = "Change")]
         [PXButton()]
         protected virtual IEnumerable ChangeOkV(PXAdapter adapter)
         {
             if (VendorUpdateAsk.Current.VendorUpdateAnswer != EPVendorUpdateAsk.Nothing)
             {
                 var query = Base.ExpenseClaimDetails.Select().AsEnumerable().Where(_ => ((EPExpenseClaimDetails)_).ContractID == null || ProjectDefaultAttribute.IsNonProject(((EPExpenseClaimDetails)_).ContractID));
                 foreach (EPExpenseClaimDetails item in query)
                 {
                     EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(item);

                     if (VendorUpdateAsk.Current.VendorUpdateAnswer == EPVendorUpdateAsk.AllLines ||
                         detailsExt.UsrVendorID == VendorUpdateAsk.Current.OldVendorID)
                     {
                         LocationExtAddress locationExtAddress = GetLocation(VendorUpdateAsk.Current.NewVendorID);
                         Base.ExpenseClaimDetails.Cache.SetValueExt<EPExpenseClaimDetailsExt.usrVendorID>(item, VendorUpdateAsk.Current.NewVendorID);                     
                         Base.ExpenseClaimDetails.Cache.SetValueExt<EPExpenseClaimDetailsExt.usrVendorID>(item,locationExtAddress.LocationID);

                         Base.ExpenseClaimDetails.Cache.Update(item);
                     }
                 }
             }
             return adapter.Get();
         }

         public PXAction<EPExpenseClaim> changeCancelV;
         [PXUIField(DisplayName = "Cancel")]
         [PXButton()]
         protected virtual IEnumerable ChangeCancelV(PXAdapter adapter)
         {
             Base.ExpenseClaim.Cache.SetValueExt<EPExpenseClaimExt.usrVendorID>(Base.ExpenseClaim.Current, VendorUpdateAsk.Current.OldVendorID);
             return adapter.Get();
         }
         */
        [Serializable]
        [PXHidden]
        public partial class EPVendorUpdateAsk : IBqlTable
        {
            #region PeriodDateSel
            public abstract class vendorUpdateAnswer : PX.Data.BQL.BqlString.Field<vendorUpdateAnswer> { }

            [PXDBString(1, IsFixed = true)]
            [PXUIField(DisplayName = "Date Based On", Visibility = PXUIVisibility.Visible)]
            [PXDefault(SelectedVendor)]
            [PXStringList(new string[] { SelectedVendor, AllLines, Nothing },
                new string[] { SelectVendor, Messages.AllLines, Messages.Nothing })]
            public virtual string VendorUpdateAnswer
            {
                get;
                set;
            }
            #endregion

            #region NewVendorID

            public abstract class newVendorID : PX.Data.BQL.BqlInt.Field<newVendorID> { }

            [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
            [AP.VendorActive(DescriptionField = typeof(AP.Vendor.acctName))]
            public virtual int? NewVendorID
            {
                get;
                set;
            }
            #endregion

            #region OldVendorID
            public abstract class oldVendorID : PX.Data.BQL.BqlInt.Field<oldVendorID> { }

            [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
            [AP.VendorActive(DescriptionField = typeof(AP.Vendor.acctName))]
            public virtual int? OldVendorID
            {
                get;
                set;
            }
            #endregion

            public const String SelectedCustomer = "S";
            public const String AllLines = "A";
            public const String Nothing = "N";
            public const String SelectedVendor = "S";
            public const String SelectVendor = "Lines with Selected Vendor.";
        }

        #endregion
    }
}