
namespace KG.Util
{
    /**
     * ===2021/08/24 :0012209 === Althea
     * Add KGVoucherType For KGVoucherUtil: CreateGLFromKGVoucher
     * 原本只有正向,多一個反向
     **/
    public class KGConst
    {
        public const string OPEN = "N";
        public const string BALANCE = "B";
        public const string ADR = "ADR";
        public const string INV = "INV";
        public const string PPM = "PPM";
        public const string REVENUE = "R";
        public const string COST = "C";
        public const string TT = "TT";
        public const string CHECK = "CHECK";
        public const string CASH = "CASH";
        public const string EP = "EP";

        public class balance : PX.Data.BQL.BqlString.Constant<balance> { public balance() : base(BALANCE) {; } }
        public class oPEN : PX.Data.BQL.BqlString.Constant<oPEN> { public oPEN() : base(OPEN) {; } }
        public class aDR : PX.Data.BQL.BqlString.Constant<aDR> { public aDR() : base(ADR) {; } }
        public class iNV : PX.Data.BQL.BqlString.Constant<iNV> { public iNV() : base(INV) {; } }
        public class pPM : PX.Data.BQL.BqlString.Constant<pPM> { public pPM() : base(PPM) {; } }
        public class revenue : PX.Data.BQL.BqlString.Constant<revenue> { public revenue() : base(REVENUE) {; } }
        public class cost : PX.Data.BQL.BqlString.Constant<cost> { public cost() : base(COST) {; } }
        public class tt : PX.Data.BQL.BqlString.Constant<tt> { public tt() : base(TT) {; } }
        public class check : PX.Data.BQL.BqlString.Constant<check> { public check() : base(CHECK) {; } }
        public class cash : PX.Data.BQL.BqlString.Constant<cash> { public cash() : base(CASH) {; } }
        public class ep : PX.Data.BQL.BqlString.Constant<ep> { public ep() : base(EP) {; } }
    }

    #region KG Voucher Type
    public class KGVoucherType
    {
        //Const
        /// <summary>
        /// 產生正向GL
        /// </summary>
        public const int CREATEGL = 1;
        /// <summary>
        /// 產生逆向GL
        /// </summary>
        public const int REVERSEGL = 2;

        /// <summary>
        /// 產生正向GL
        /// </summary>
        public class createGL : PX.Data.BQL.BqlInt.Constant<createGL> { public createGL() : base(CREATEGL) {; } }
        /// <summary>
        /// 產生逆向GL
        /// </summary>
        public class reverseGL : PX.Data.BQL.BqlInt.Constant<reverseGL> { public reverseGL() : base(REVERSEGL) {; } }
      
    }
    #endregion

    #region GL Stage Code
    public class GLStageCode
    {
        public const string APBillReleaseA0 = "A0"; //計價單過帳)
        public const string NMPConfirmP1 = "P1";//應付票據確認P1
        public const string NMPConfirmP2 = "P2";//應付票據確認過帳APPayment
        public const string NMPConfirmP3 = "P3";//應付票據確認P3
        public const string NMPConfirmP4 = "P4";//應付票據確認郵資
        public const string NMPCashP5 = "P5";//應付票據兌現
        public const string NMPVoidPA = "PA";//應付票據作廢PA
        public const string NMPVoidPB = "PB";//應付票據作廢PB
        public const string NMPVoidPC = "PC";//應付票據作廢PC
        public const string NMPVoidPD = "PD";//應付票據作廢PD
        public const string NMPVoidPE = "PE";//應付票據作廢PE
        public const string NMPVoidPZ = "PZ";//應付票據作廢預付款PZ
        public const string NMPTtFeedbackT1 = "T1";//應付帳款電匯回饋T1
        public const string NMPTtFeedbackT2 = "T2";//應付帳款電匯回饋郵資T2
        public const string NMPWriteoffB1 = "B1";//現金禮卷授扣沖銷B1
        public const string NMPWriteoffB2 = "B2";//暫付款沖銷B2
        public const string NMRPaymentReleaseR0 = "R0";//應收憑單過帳R0
        public const string NMRPaymentReleaseR1 = "R1";//應收憑單過帳R1
        public const string NMRReceiveR2 = "R2";//應收票據收票R2
        public const string NMRCollectR3 = "R3";//應收票據託收R3
        public const string NMRCashR4 = "R4";//應收票據兌現R4
        public const string NMRPaymentReleaseR5 = "R5";//自行開立的應收憑單過帳R5
        public const string NMRVoidRB = "RB";//應收票據撤票(收票)RB
        public const string NMRVoidRA = "RA";//應收票據撤票(託收)RA
        public const string NMRVoidRE = "RE";//應收票據撤票(兌現)RE
        public const string NMPaymentVoidRF = "RF";//應收憑單作廢RF
        public const string NMRVoidRC = "RC";//應收反轉(CHeck)RC
        public const string NMRVoidRD = "RD";//應收反轉(非Check)RD
        public const string NMPayableConfirm = "B";//應付票據確認
        //public const string NMPayableCash = "C";//應付票據兌現
        //public const string NMPayableVoid = "D";//應付票據撤退票
        //public const string ARPaymentRelease = "E";//收款過帳
        //public const string NMReceivableCollect = "F";//應收票據託收
        //public const string NMReceivableCash = "G";//應收票據兌現
        //public const string NMReceivableVoid = "H";//應收票據撤退票
        public const string APPaymentVoid = "I";//付款作廢
        //public const string APPaymentRelease = "J";//付款過帳
        public const string EPDepositM1 = "M1"; // 定存評價

        public class aPBillReleaseA0 : PX.Data.BQL.BqlString.Constant<aPBillReleaseA0> { public aPBillReleaseA0() : base(APBillReleaseA0) {; } }
        public class nMPConfirmP1 : PX.Data.BQL.BqlString.Constant<nMPConfirmP1> { public nMPConfirmP1() : base(NMPConfirmP1) {; } }
        public class nMPConfirmP2 : PX.Data.BQL.BqlString.Constant<nMPConfirmP2> { public nMPConfirmP2() : base(NMPConfirmP2) {; } }
        public class nMPConfirmP3 : PX.Data.BQL.BqlString.Constant<nMPConfirmP3> { public nMPConfirmP3() : base(NMPConfirmP3) {; } }
        public class nMPConfirmP4 : PX.Data.BQL.BqlString.Constant<nMPConfirmP4> { public nMPConfirmP4() : base(NMPConfirmP4) {; } }
        public class nMPCashP5 : PX.Data.BQL.BqlString.Constant<nMPCashP5> { public nMPCashP5() : base(NMPCashP5) {; } }
        public class nMPVoidPA : PX.Data.BQL.BqlString.Constant<nMPVoidPA> { public nMPVoidPA() : base(NMPVoidPA) {; } }
        public class nMPVoidPB : PX.Data.BQL.BqlString.Constant<nMPVoidPB> { public nMPVoidPB() : base(NMPVoidPB) {; } }
        public class nMPVoidPC : PX.Data.BQL.BqlString.Constant<nMPVoidPC> { public nMPVoidPC() : base(NMPVoidPC) {; } }
        public class nMPVoidPD : PX.Data.BQL.BqlString.Constant<nMPVoidPD> { public nMPVoidPD() : base(NMPVoidPD) {; } }
        public class nMPVoidPE : PX.Data.BQL.BqlString.Constant<nMPVoidPE> { public nMPVoidPE() : base(NMPVoidPE) {; } }
        public class nMPVoidPZ : PX.Data.BQL.BqlString.Constant<nMPVoidPZ> { public nMPVoidPZ() : base(NMPVoidPZ) {; } }
        public class nMPTtFeedbackT1 : PX.Data.BQL.BqlString.Constant<nMPTtFeedbackT1> { public nMPTtFeedbackT1() : base(NMPTtFeedbackT1) {; } }
        public class nMPTtFeedbackT2 : PX.Data.BQL.BqlString.Constant<nMPTtFeedbackT2> { public nMPTtFeedbackT2() : base(NMPTtFeedbackT2) {; } }
        public class nMPWriteoffB1 : PX.Data.BQL.BqlString.Constant<nMPWriteoffB1> { public nMPWriteoffB1() : base(NMPWriteoffB1) {; } }
        public class nMPWriteoffB2 : PX.Data.BQL.BqlString.Constant<nMPWriteoffB2> { public nMPWriteoffB2() : base(NMPWriteoffB2) {; } }
        public class nMRPaymentReleaseR0 : PX.Data.BQL.BqlString.Constant<nMRPaymentReleaseR0> { public nMRPaymentReleaseR0() : base(NMRPaymentReleaseR0) {; } }
        public class nMRPaymentReleaseR1 : PX.Data.BQL.BqlString.Constant<nMRPaymentReleaseR1> { public nMRPaymentReleaseR1() : base(NMRPaymentReleaseR1) {; } }
        public class nMRReceiveR2 : PX.Data.BQL.BqlString.Constant<nMRReceiveR2> { public nMRReceiveR2() : base(NMRReceiveR2) {; } }
        public class nMRCollectR3 : PX.Data.BQL.BqlString.Constant<nMRCollectR3> { public nMRCollectR3() : base(NMRCollectR3) {; } }
        public class nMRCashR4 : PX.Data.BQL.BqlString.Constant<nMRCashR4> { public nMRCashR4() : base(NMRCashR4) {; } }
        public class nMRPaymentReleaseR5 : PX.Data.BQL.BqlString.Constant<nMRPaymentReleaseR5> { public nMRPaymentReleaseR5() : base(NMRPaymentReleaseR5) {; } }
        public class nMRVoidRB : PX.Data.BQL.BqlString.Constant<nMRVoidRB> { public nMRVoidRB() : base(NMRVoidRB) {; } }
        public class nMRVoidRA : PX.Data.BQL.BqlString.Constant<nMRVoidRA> { public nMRVoidRA() : base(NMRVoidRA) {; } }
        public class nMRVoidRC : PX.Data.BQL.BqlString.Constant<nMRVoidRC> { public nMRVoidRC() : base(NMRVoidRC) {; } }
        public class nMRVoidRD : PX.Data.BQL.BqlString.Constant<nMRVoidRD> { public nMRVoidRD() : base(NMRVoidRD) {; } }
        public class nMRVoidRE : PX.Data.BQL.BqlString.Constant<nMRVoidRE> { public nMRVoidRE() : base(NMRVoidRE) {; } }
        public class nMPaymentVoidRF : PX.Data.BQL.BqlString.Constant<nMPaymentVoidRF> { public nMPaymentVoidRF() : base(NMPaymentVoidRF) {; } }
        public class nMPayableConfirm : PX.Data.BQL.BqlString.Constant<nMPayableConfirm> { public nMPayableConfirm() : base(NMPayableConfirm) {; } }
        //public class nMPayableCash : PX.Data.BQL.BqlString.Constant<nMPayableCash> { public nMPayableCash() : base(NMPayableCash) {; } }
        //public class nMPayableVoid : PX.Data.BQL.BqlString.Constant<nMPayableVoid> { public nMPayableVoid() : base(NMPayableVoid) {; } }
        //public class aRPaymentRelease : PX.Data.BQL.BqlString.Constant<aRPaymentRelease> { public aRPaymentRelease() : base(ARPaymentRelease) {; } }
        //public class nMReceivableCollect : PX.Data.BQL.BqlString.Constant<nMReceivableCollect> { public nMReceivableCollect() : base(NMReceivableCollect) {; } }
        //public class nMReceivableCash : PX.Data.BQL.BqlString.Constant<nMReceivableCash> { public nMReceivableCash() : base(NMReceivableCash) {; } }
        //public class nMReceivableVoid : PX.Data.BQL.BqlString.Constant<nMReceivableVoid> { public nMReceivableVoid() : base(NMReceivableVoid) {; } }
        public class aPPaymentVoid : PX.Data.BQL.BqlString.Constant<aPPaymentVoid> { public aPPaymentVoid() : base(APPaymentVoid) {; } }
        //public class aPPaymentRelease : PX.Data.BQL.BqlString.Constant<aPPaymentRelease> { public aPPaymentRelease() : base(APPaymentRelease) {; } }
        public class ePDepositM1 : PX.Data.BQL.BqlString.Constant<ePDepositM1> { public ePDepositM1() : base(EPDepositM1) { } }
    }
    #endregion
}
