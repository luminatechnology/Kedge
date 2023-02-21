using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using PX.Objects.PO;
using Kedge.DAC;
using PX.Objects.CT;
using PX.Objects.CR;
using PX.Objects.CS;
using RCGV.GV.DAC;
using RCGV.GV;
using System.Globalization;
using System.Data.Common;
using Oracle.ManagedDataAccess.Types;
using PX.Objects.AP;

namespace Kedge
{
    public class VoucherUntil : PXGraph<VoucherUntil>
    {

        #region VoucherInsert

        //For Testing
        public static void testconnection(DateTime? voucherdate, KGVoucherH voucherH)
        {
            string conn_str = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.14.201)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=BIEE.READYCOM.COM.TW)));User Id=fin;Password=fin;";
            string SQL_cmd = "INSERT INTO VOUCHER_T(COMPANY,VOUCHER_KEY," +
                "VOUCHER_DATE)VALUES (:COMPANY, :VOUCHER_KEY,:VOUCHER_DATE)";
            //string Call_cmd = "SELECT * FROM VOUCHER_T WHERE 1";
            #region Insert Data
            using (OracleConnection conn = new OracleConnection(conn_str))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                /*if(conn.State == ConnectionState.Open)
                {
                    throw new Exception("���\!");
                }*/
                #region Insert Data
                OracleCommand cmd = new OracleCommand(SQL_cmd, conn);
                //BindByName�w�]��false�ASQL�|�̷Ӷ��ǥN�J�Ѽ�
                //�Y�]��true�ɡA�h�̰ѼƦW�٥N�J�Ѽ�
                cmd.BindByName = true;

                //�[�J�Ѽ�
                DateTime date = (DateTime)voucherdate;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("COMPANY", "K02");
                cmd.Parameters.Add("VOUCHER_KEY", "testNo1");
                cmd.Parameters.Add("VOUCHER_DATE", date.ToSimpleTaiwanDate());
                OracleDataAdapter DataAdapter = new OracleDataAdapter();
                DataAdapter.SelectCommand = cmd;
                DataSet ds = new DataSet();
                DataAdapter.Fill(ds);
                cmd.Dispose();
                #endregion
                conn.Dispose();
                conn.Close();
                //�d�ߨ쪺��Ƴ��s�bds���F
            }
            #endregion

            #region Call Funtion
            using (OracleConnection callfuntionconn = new OracleConnection(conn_str))
            {
                if (callfuntionconn.State == ConnectionState.Closed)
                {
                    callfuntionconn.Open();
                }
                //�إ�command�I�sfunction: "testFun"
                OracleCommand callfuntion = new OracleCommand();
                callfuntion.Connection = callfuntionconn;
                callfuntion.CommandText = "ACU_TRANS_ERP_PKG.get_vendor_exist_flag";
                //�]�wcommand type��StoredProcedure
                callfuntion.CommandType = CommandType.StoredProcedure;


                callfuntion.Parameters.Add(new OracleParameter("p_VENDOR_id", OracleDbType.Varchar2, ParameterDirection.Input));
                //callfuntion.Parameters["p_VENDOR_id"].Direction = ParameterDirection.Input;
                callfuntion.Parameters["p_VENDOR_id"].Value = voucherH.VendorCD;
                callfuntion.Parameters.Add(new OracleParameter("v_exist_flag", OracleDbType.Varchar2, ParameterDirection.ReturnValue));
                //callfuntion.Parameters["v_exist_flag"].Direction = ParameterDirection.Output;

                //Simple
                //�[�J�Ѽ�no_in
                //cmd.Parameters.Add(new OracleParameter("NO_IN", OracleDbType.Varchar2));
                //�ѼƤ�Vin/out/in out
                //cmd.Parameters["NO_IN"].Direction = ParameterDirection.InputOutput;
                //�Ѽƭ�
                //cmd.Parameters["NO_IN"].Value = 1;


                //����R�O, �öǦ^�^�ǭ�
                callfuntion.ExecuteNonQuery();
                bool ReturnValue = VendorisExit(voucherH, callfuntionconn);
                if (!ReturnValue)
                {
                    throw new Exception("N");
                }
                callfuntionconn.Close();
                throw new Exception("Input =" + callfuntion.Parameters["p_VENDOR_id"].Value.ToString() +
                    "Output = " + callfuntion.Parameters["v_exist_flag"].Value.ToString());
                //��ܰѼ�no_in
                //this.Label2.Text = cmd.Parameters["NO_IN"].Value.ToString();
                //��ܦ^�ǭ�
                //this.Label3.Text = cmd.Parameters["ret"].Value.ToString();


            }
            #endregion
        }

        public static void InsertToOracle(DateTime? voucherdate, KGVoucherH voucherH
            , PXResultset<KGVoucherL> set, Contact contact, BAccount bAccount, Address address,
            CSAnswers cSAnswer, PXResultset<GVApGuiInvoice> gVApGuiInvoiceset, 
            string Company, string UserName)
        {
            //����DB Addr
            string testconn_str = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.14.201)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=BIEE.READYCOM.COM.TW)));User Id=fin;Password=fin;";
            //����DB Addr
            string conn_str = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=220.130.9.10)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=SIT.KINDOM.COM.TW)));User Id=apps;Password=apps;";
            //�ܼ�DB Addr
            KGVoucherProcess graph = PXGraph.CreateInstance<KGVoucherProcess>();
            KGFinIntegrationSetup finIntegrationSetup =
                PXSelect<KGFinIntegrationSetup,
                Where<KGFinIntegrationSetup.contractID, Equal<Required<KGVoucherH.contractID>>>>
                .Select(graph, voucherH.ContractID);
            if (finIntegrationSetup == null)
            {
                throw new Exception("���M�ש|���]�w����s�u!");
            }
            KGFinConnectSetup connectSetup =
                PXSelect<KGFinConnectSetup,
                Where<KGFinConnectSetup.connectSite, Equal<Required<KGFinConnectSetup.connectSite>>>>
                .Select(graph, finIntegrationSetup.ConnectSite);
            string setconn_str = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + connectSetup.Host.Trim() +
                ")(PORT=" + connectSetup.Port +
                ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + connectSetup.Sid.Trim() +
                ")));User Id=" + connectSetup.UserName.Trim() + ";Password=" + connectSetup.UserPwd.Trim() + ";";

            string InsertToVoucher_tCmd = "INSERT INTO VOUCHER_T(COMPANY,VOUCHER_KEY," +
                "VOUCHER_DATE,VOUCHER_NO,ENABLE_FLAG,OPERATOR,CREATE_DATE,DR_AMT," +
                "CR_AMT,ACCOUNTANT_CHECK_USER,ACCOUNTANT_CHECK_DATE,POSTING_FLAG," +
                "UNIFORM_NO,KIND,PROJECT,MARK_FLAG,ACC_UID,ACCTYPE,VENDOR_TYPE," +
                "VOUCHER_SOURCE)" +
                "VALUES (:COMPANY, :VOUCHER_KEY,:VOUCHER_DATE,:VOUCHER_NO,:ENABLE_FLAG," +
                ":OPERATOR,:CREATE_DATE,:DR_AMT,:CR_AMT,:ACCOUNTANT_CHECK_USER," +
                ":ACCOUNTANT_CHECK_DATE,:POSTING_FLAG,:UNIFORM_NO,:KIND,:PROJECT," +
                ":MARK_FLAG,:ACC_UID,:ACCTYPE,:VENDOR_TYPE,:VOUCHER_SOURCE)";


            string InsertToVendorCmd = "INSERT INTO Vendor(VENDOR_ID,UNIFORM_NO," +
                "NAME_TW,PHONE_1,PHONE_2,FAX,CELLULAR,INVOICE_ADDRESS,"+
                "CONTACT_ADDRESS,RESPONSOR)" +
                "VALUES (:VENDOR_ID,:UNIFORM_NO,:NAME_TW,:PHONE_1,:PHONE_2," +
                ":FAX,:CELLULAR,:INVOICE_ADDRESS,:CONTACT_ADDRESS,:RESPONSOR)";

            string InsertToInventoryCmd = "INSERT INTO INVENTORY_INVOICE_t(" +
                "INVOICE_NO,VOUCHER_NO,INVOICE_DATE,UNIFORM_YY,UNIFORM_MM,AMOUNT," +
                "TAX_CODE,TAX_AMOUNT,CLASS_CODE,CREATE_DATE,COMPANY,VOUCHER_KEY,ITEM_NO," +
                "UNIFORM_NO,IVO_KIND,BELONG_YY,BELONG_MM,PROJECT,ACC_UID,ACCTYPE)" +
                "VALUES (:INVOICE_NO,:VOUCHER_NO,:INVOICE_DATE,:UNIFORM_YY,:UNIFORM_MM," +
                ":AMOUNT,:TAX_CODE,:TAX_AMOUNT,:CLASS_CODE,:CREATE_DATE,:COMPANY,:VOUCHER_KEY," +
                ":ITEM_NO,:UNIFORM_NO,:IVO_KIND,:BELONG_YY,:BELONG_MM,:PROJECT,:ACC_UID,:ACCTYPE)";

            string InsertToVoucher_Item_tCmd = "INSERT INTO VOUCHER_ITEM_t(" +
                "COMPANY,VOUCHER_KEY,ITEM_NO,VOUCHER_NO,ACCOUNT_NO,PROJECT,DIGEST," +
                "CD,AMT,OPERATOR,CREATE_DATE,UNIFORM_NO,MARK_FLAG,DATE_DUE,ACC_UID," +
                "ACCTYPE,VENDOR_TYPE)" +
                "VALUES(:COMPANY,:VOUCHER_KEY,:ITEM_NO,:VOUCHER_NO,:ACCOUNT_NO," +
                ":PROJECT,:DIGEST,:CD,:AMT,:OPERATOR,:CREATE_DATE,:UNIFORM_NO," +
                ":MARK_FLAG,:DATE_DUE,:ACC_UID,:ACCType,:VENDOR_TYPE)";



            //string SelectVendor = "select get_vendor_exist_flag('86149733') from dual";

            string Voucher_Key = "";
            string Voucher_No = "";
            string CheckInventoryCmd = "";
            //Check Invoice, True:Invoice Exist, False:Invoice not Exist.
            bool InvoiceExist=false;
            //Check Vendor, True: Vendor Exist, False: Vendor not Exist.
            bool ReturnValue=false;

            //2020/03/16 ���ܶ��� 1)�ˮ� 2)��Key& No 3)Insert Data

            //Check
            using (OracleConnection conn = new OracleConnection(setconn_str))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                //Check Vendor Exist or not
                ReturnValue = VendorisExit(voucherH, conn);
   
                //Check Invoice Exist or not
                if (gVApGuiInvoiceset != null)
                {
                    foreach (GVApGuiInvoice gVApGuiInvoice in gVApGuiInvoiceset)
                    {
                        //�ˮֵo���O�_�Q����L
                        CheckInventoryCmd = "select * from INVENTORY_INVOICE_t where INVOICE_NO='" + gVApGuiInvoice.GuiInvoiceNbr +
                            "' and COMPANY='" + Company + "' and UNIFORM_YY='" + (gVApGuiInvoice.DeclareYear - 1911).ToString() + "'";
                        if (CheckInventory(conn, CheckInventoryCmd))
                        {
                            InvoiceExist = true;
                            throw new Exception("���o���w�Q����L!");
                        }
                    }
                }

                conn.Dispose();
                conn.Close();
            }

            //GetKey & GetNo
            using (OracleConnection getVoucherconn = new OracleConnection(setconn_str))
            {
                //if (getVoucherconn.State == ConnectionState.Closed)
                //{
                //    getVoucherconn.Open();
                //}
                DateTime date = (DateTime)voucherdate;

                #region Funtion Methods
                //Get_Voucher_No
                //OracleCommand Nocmd = new OracleCommand("ACU_TRANS_ERP_PKG.GET_voucher_no", getVoucherconn);
                //Nocmd.CommandType = CommandType.StoredProcedure;

                //OracleParameter resultNoParam = new OracleParameter("@Result", OracleDbType.Varchar2, 11);

                //// ReturnValue
                //resultNoParam.Direction = ParameterDirection.ReturnValue;

                //// Add to parameters
                //Nocmd.Parameters.Add(resultNoParam);

                //// Add parameter @p_Emp_Id and set value = 100.
                //Nocmd.Parameters.Add("@P_voucher_date", OracleDbType.Varchar2).Value = date;
                //Nocmd.Parameters.Add("@P_COMPANY_ID", OracleDbType.Varchar2).Value = Company;

                //// Call function.
                //Nocmd.ExecuteNonQuery();
                //OracleString ret = (OracleString)resultNoParam.Value;
                //Voucher_No = ret.ToString();
                //Nocmd.Dispose();

                ////Get_Voucher_Key
                //OracleCommand Keycmd = new OracleCommand("ACU_TRANS_ERP_PKG.GET_voucher_key", getVoucherconn);
                //Keycmd.CommandType = CommandType.StoredProcedure;
                //OracleParameter resultKeyParam = new OracleParameter("@Result", OracleDbType.Varchar2, 11);

                //// ReturnValue
                //resultKeyParam.Direction = ParameterDirection.ReturnValue;

                //// Add to parameters
                //Keycmd.Parameters.Add(resultKeyParam);

                //// Add parameter @p_Emp_Id and set value = 100.
                //Keycmd.Parameters.Add("@P_voucher_date", OracleDbType.Varchar2).Value = date;
                //Keycmd.Parameters.Add("@P_COMPANY_ID", OracleDbType.Varchar2).Value = Company;
                //Keycmd.Parameters.Add("@P_USER_ID", OracleDbType.Varchar2).Value = UserName;
                //// Call function.
                //Keycmd.ExecuteNonQuery();
                //OracleString Keyret = (OracleString)resultKeyParam.Value;
                //Voucher_Key = Keyret.ToString();
                //Keycmd.Dispose();
                #endregion

                #region Procedure Methods
                ///First getKey, Second getNo
                //Key
                OracleCommand KeyCmd = new OracleCommand("ACU_TRANS_ERP_PKG.GET_voucher_key", getVoucherconn);
                KeyCmd.CommandType = CommandType.StoredProcedure;

                KeyCmd.Parameters.Add(new OracleParameter("P_voucher_date", OracleDbType.Varchar2));
                KeyCmd.Parameters["P_voucher_date"].Direction = ParameterDirection.Input;
                KeyCmd.Parameters["P_voucher_date"].Value = date.To8VarCharTaiwanDate().ToString();
                KeyCmd.Parameters.Add(new OracleParameter("P_COMPANY_ID", OracleDbType.Varchar2));
                KeyCmd.Parameters["P_COMPANY_ID"].Direction = ParameterDirection.Input;
                KeyCmd.Parameters["P_COMPANY_ID"].Value = Company;
                KeyCmd.Parameters.Add(new OracleParameter("P_USER_ID", OracleDbType.Varchar2));
                KeyCmd.Parameters["P_USER_ID"].Direction = ParameterDirection.Input;
                KeyCmd.Parameters["P_USER_ID"].Value = UserName;

                OracleParameter key = KeyCmd.Parameters.Add(new OracleParameter("p_voucher_key", OracleDbType.Varchar2, 11));
                KeyCmd.Parameters["p_voucher_key"].Direction = ParameterDirection.Output;
                try
                {
                    getVoucherconn.Open();
                    KeyCmd.ExecuteNonQuery();
                    Voucher_Key = key.Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Get Voucher Key Error!", ex);
                }
                finally
                {
                    KeyCmd.Dispose();
                    
                }
                //No
                OracleCommand NoCmd = new OracleCommand("ACU_TRANS_ERP_PKG.GET_voucher_num", getVoucherconn);
                NoCmd.CommandType = CommandType.StoredProcedure;

                //�[�J�Ѽ�no_in
                NoCmd.Parameters.Add(new OracleParameter("P_voucher_date", OracleDbType.Varchar2));
                //�ѼƤ�Vin/out/in out
                NoCmd.Parameters["P_voucher_date"].Direction = ParameterDirection.Input;
                //�Ѽƭ�
                NoCmd.Parameters["P_voucher_date"].Value = date.To8VarCharTaiwanDate().ToString();

                NoCmd.Parameters.Add(new OracleParameter("P_COMPANY_ID", OracleDbType.Varchar2));
                NoCmd.Parameters["P_COMPANY_ID"].Direction = ParameterDirection.Input;
                NoCmd.Parameters["P_COMPANY_ID"].Value = Company;

                OracleParameter no = NoCmd.Parameters.Add(new OracleParameter("p_voucher_no", OracleDbType.Varchar2, 11));
                NoCmd.Parameters["p_voucher_no"].Direction = ParameterDirection.Output;




                try
                {
                    
                    NoCmd.ExecuteNonQuery();
                    Voucher_No = no.Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Get Voucher Number Error!", ex);
                }
                finally
                {
                    NoCmd.Dispose();
                    getVoucherconn.Close();
                }

                

                #endregion

            }

            //Insert Data
            using (OracleConnection conn = new OracleConnection(setconn_str))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                //Return Value = false, Insert Vendor_t to OracleDB
                if (!ReturnValue)
                {
                    InsertVendor(voucherH, conn, InsertToVendorCmd, bAccount, contact, address, cSAnswer);
                }

                //ADD Voucher_t
                InsertVoucher_t(voucherH, conn, InsertToVoucher_tCmd, Voucher_Key, Voucher_No, UserName, Company);
                int No = 1;
                int ItemNo = No;
                //ADD Voucher_Item_t
                foreach (KGVoucherL voucherL in set)
                {
                    InsertVoucher_Item_t(voucherL, conn, InsertToVoucher_Item_tCmd, bAccount, No, Voucher_Key, Voucher_No, UserName, Company);
                    No++;
                }

                //If InvoiceExist = false, ADD Invoice_t
                if (gVApGuiInvoiceset != null && InvoiceExist == false)
                {
                    foreach (GVApGuiInvoice gVApGuiInvoice in gVApGuiInvoiceset)
                    {

                        //ADD INVENTORY_INVOICE_t(�h��)                            
                        BAccount bAccountVendor = PXSelect<BAccount,
                                Where<BAccount.bAccountID, Equal<Required<GVApGuiInvoice.vendor>>>>
                                .Select(graph, gVApGuiInvoice.Vendor);
                        InsertInventory_Invoice_t(voucherH, conn, InsertToInventoryCmd, gVApGuiInvoice, bAccountVendor, Voucher_Key, Voucher_No, Company);

                    }
                }
                    

                conn.Dispose();
                conn.Close();

                //KGVoucherProcess graph = PXGraph.CreateInstance<KGVoucherProcess>();     
                voucherH.VoucherKey = Voucher_Key;
                voucherH.VoucherNo = Voucher_No;
                voucherH.Status = "P";
                //graph.AllVouchers.Update(voucherH);

            }


        }
        #endregion

        #region Method
        public static void InsertVoucher_t(KGVoucherH voucherH, OracleConnection conn,
            string InsertToVoucher_tCmd, string Voucher_Key, string Voucher_No, string UserName,
            string Company)
        {
            OracleCommand cmd = new OracleCommand(InsertToVoucher_tCmd, conn);
            //BindByName�w�]��false�ASQL�|�̷Ӷ��ǥN�J�Ѽ�
            //�Y�]��true�ɡA�h�̰ѼƦW�٥N�J�Ѽ�
            cmd.BindByName = true;

            //�[�J�Ѽ�           
            //Insert Voucher_t
            DateTime date = (DateTime)voucherH.VoucherDate;
            KGVoucherProcess graph = PXGraph.CreateInstance<KGVoucherProcess>();
            DateTime systemdate = (DateTime)graph.Accessinfo.BusinessDate;

            cmd.Parameters.Clear();
            cmd.Parameters.Add(":COMPANY", Company);//���q�OK02  ���q�����qID
            cmd.Parameters.Add(":VOUCHER_KEY", Voucher_Key);//�W�����
            cmd.Parameters.Add(":VOUCHER_DATE", date.To8VarCharTaiwanDate().ToString());//����ɫ��w�n�쪺�ǲ�����A���઺�e����
            cmd.Parameters.Add(":VOUCHER_NO", Voucher_No);//�W�����
            cmd.Parameters.Add(":ENABLE_FLAG", "Y");//�T�w��Y
            cmd.Parameters.Add(":OPERATOR", UserName); //�W��n�J��OPERATOR UserName
            cmd.Parameters.Add(":CREATE_DATE", systemdate); //sysdate
            cmd.Parameters.Add(":DR_AMT", "0"); //��TRIGGER�g�J FIN.voucher_item_ai  --v_VOUCHER.DR_AMT,
            cmd.Parameters.Add(":CR_AMT", "0"); //��TRIGGER�g�J FIN.voucher_item_ai  --v_VOUCHER.CR_AMT,
            cmd.Parameters.Add(":ACCOUNTANT_CHECK_USER", UserName); //--�W��n�J��--ACCOUNTANT_CHECK_USER
            cmd.Parameters.Add(":ACCOUNTANT_CHECK_DATE", systemdate); //sysdate
            cmd.Parameters.Add(":POSTING_FLAG", "N"); //�T�w��N
            cmd.Parameters.Add(":UNIFORM_NO", voucherH.VendorCD.Trim()); //�����ӽs��
            cmd.Parameters.Add(":KIND", "P"); //�T�w��P
            cmd.Parameters.Add(":PROJECT", voucherH.ContractCD.Trim()); //PROJECT
            cmd.Parameters.Add(":MARK_FLAG", "N"); //�T�w��N
            cmd.Parameters.Add(":ACC_UID", ""); //COMS���ߤ@�����A�ҥH��ť�
            cmd.Parameters.Add(":ACCTYPE", "Sub"); //�T�w��Sub
            cmd.Parameters.Add(":VENDOR_TYPE", voucherH.VendorType.Trim()); //���u��E�A�����Ӭ�V
            cmd.Parameters.Add(":VOUCHER_SOURCE", "Acumatica"); //'Acumatica'

            cmd.ExecuteNonQuery();
            cmd.Dispose();

        }

        public static void InsertVoucher_Item_t(KGVoucherL voucherL, OracleConnection conn,
            string InsertToVoucher_Item_tCmd, BAccount bAccount, int No, string Voucher_Key,
            string Voucher_No, string UserName, string Company)
        {

            OracleCommand cmd = new OracleCommand(InsertToVoucher_Item_tCmd, conn);
            //BindByName�w�]��false�ASQL�|�̷Ӷ��ǥN�J�Ѽ�
            //�Y�]��true�ɡA�h�̰ѼƦW�٥N�J�Ѽ�
            cmd.BindByName = true;
            KGVoucherProcess graph = PXGraph.CreateInstance<KGVoucherProcess>();
            DateTime systemdate = (DateTime)graph.Accessinfo.BusinessDate;
            string digest = "";
            string Project = "";
            if (voucherL.Digest != null)
            {
                digest = voucherL.Digest.Trim();
            }
            if(voucherL.ContractID!= null)
            {
                Project= voucherL.ContractCD.Trim();
            }

            cmd.Parameters.Add(":COMPANY", Company); //--���q�OK02
            cmd.Parameters.Add(":VOUCHER_KEY", Voucher_Key);//--�P���Yvoucher_key
            cmd.Parameters.Add(":ITEM_NO", No);//--VOURCHER_ITEM.ITEM_NO�ǲ������ӧǸ�
            cmd.Parameters.Add(":VOUCHER_NO", Voucher_No);//--�P���Yvoucher_no
            cmd.Parameters.Add(":ACCOUNT_NO", voucherL.AccountCD.Trim());
            cmd.Parameters.Add(":PROJECT", Project); //--�P���Y�M�ץN�XPROJECT
            cmd.Parameters.Add(":DIGEST", digest); //--�K�n�����PCOMS�W�h
            cmd.Parameters.Add(":CD", voucherL.Cd);//--�U�謰C, �ɤ謰D
            cmd.Parameters.Add(":AMT", voucherL.Amt); //--�Ӭ�ت����B�L�׭ɶU�Ҭ�����
            cmd.Parameters.Add(":OPERATOR", UserName);//V_USER_ID--�W�����
            cmd.Parameters.Add(":CREATE_DATE", systemdate);//systemdate
            cmd.Parameters.Add(":UNIFORM_NO", bAccount.AcctCD.Trim()); //--�����ӽs��
            cmd.Parameters.Add(":MARK_FLAG", "N");//--�T�w��N
            cmd.Parameters.Add(":DATE_DUE", voucherL.DueDate);//--���ڨ����A�̷ӹL�b����p������??
            cmd.Parameters.Add(":ACC_UID", "");//--v_VOUCHER_ITEM.Acc_UID,--COMS���ߤ@�����AACUMATICA��VOUCHER_SOURCE
            cmd.Parameters.Add(":ACCTYPE", "Sub");//--�T�w��Sub
            cmd.Parameters.Add(":VENDOR_TYPE", voucherL.VendorType.Trim());//--���u��E�A�����Ӭ�V
            //cmd.Parameters.Add(":VOUCHER_SOURCE", "Acumatica");//--Acumatica
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        private static bool VendorisExit(KGVoucherH voucherH, OracleConnection conn)
        {

            OracleCommand cmd = new OracleCommand("ACU_TRANS_ERP_PKG.get_vendor_exist_flag", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            OracleParameter resultParam = new OracleParameter("@Result", OracleDbType.Varchar2, 1);

            // ReturnValue
            resultParam.Direction = ParameterDirection.ReturnValue;

            // Add to parameters
            cmd.Parameters.Add(resultParam);

            // Add parameter @p_Emp_Id and set value = 100.
            cmd.Parameters.Add("@p_vendor_id", OracleDbType.Varchar2).Value = voucherH.VendorCD.Trim();

            // Call function.
            cmd.ExecuteNonQuery();
            OracleString ret = (OracleString)resultParam.Value;

            if (ret.ToString() == "Y")
            {
                return true;
            }
            else if (ret.ToString() == "N")
            {
                return false;
            }
            else
            {
                throw new Exception("Vendor Exit or not Error!");
            }

        }

        public static void InsertVendor(KGVoucherH voucherH, OracleConnection conn,
            string InsertToVendorCmd, BAccount bAccount, Contact contact,
            Address address, CSAnswers cSAnswer)
        {
            //Insert Vendor

            OracleCommand cmdVendor = new OracleCommand(InsertToVendorCmd, conn);
            cmdVendor.BindByName = true;
            cmdVendor.Parameters.Add(":VENDOR_ID", voucherH.VendorCD.Trim());//�����ӽs��
            cmdVendor.Parameters.Add(":UNIFORM_NO", voucherH.VendorCD.Trim());//�����ӽs��
            cmdVendor.Parameters.Add(":NAME_TW", bAccount.AcctName.Trim());//�����ӦW��
            cmdVendor.Parameters.Add(":PHONE_2", "");//�����ӥD��/�q��2
            cmdVendor.Parameters.Add(":CELLULAR", "");//CELLULAR
            if (contact != null)
            {
                if (contact.Phone1 != null)
                {
                    cmdVendor.Parameters.Add(":PHONE_1", contact.Phone1.Trim());//�����ӥD��/�q��1
                }
                else
                {
                    cmdVendor.Parameters.Add(":PHONE_1", "");//�����ӥD��/�q��1                   
                }
                if (contact.Fax != null)
                {
                    cmdVendor.Parameters.Add(":FAX", contact.Fax.Trim());//�����ӥD��/�ǯu���X
                }
                else
                {
                    cmdVendor.Parameters.Add(":FAX", "");//�����ӥD��/�ǯu���X
                }

            }
            else
            {
                cmdVendor.Parameters.Add(":PHONE_1", "");//�����ӥD��/�q��1
                cmdVendor.Parameters.Add(":FAX", "");//�����ӥD��/�ǯu���X
            }
            if (address != null)
            {
                if(address.AddressLine1!=null)
                {
                   
                    cmdVendor.Parameters.Add(":INVOICE_ADDRESS", address.AddressLine1.Trim());//�����ӥD�� �a�}1-- INVOICE_ADDRESS
                    cmdVendor.Parameters.Add(":CONTACT_ADDRESS", address.AddressLine1.Trim());//�����ӥD�� �a�}1-- CONTACT_ADDRESS
                }else
                {
                    cmdVendor.Parameters.Add(":INVOICE_ADDRESS", "");//�����ӥD�� �a�}1-- INVOICE_ADDRESS
                    cmdVendor.Parameters.Add(":CONTACT_ADDRESS", "");//�����ӥD�� �a�}1-- CONTACT_ADDRESS
                }
            }
            else
            {
                cmdVendor.Parameters.Add(":INVOICE_ADDRESS", "");//�����ӥD�� �a�}1-- INVOICE_ADDRESS
                cmdVendor.Parameters.Add(":CONTACT_ADDRESS", "");//�����ӥD�� �a�}1-- CONTACT_ADDRESS
            }
            if (cSAnswer != null)
            {
                if(cSAnswer.Value!=null)
                {
                    cmdVendor.Parameters.Add(":RESPONSOR", cSAnswer.Value.Trim());//�����ӥD��/ �ݩ�/�t�d�H -- CONTACT
                }
                
            }
            else
            {
                cmdVendor.Parameters.Add(":RESPONSOR", "");//�����ӥD��/ �ݩ�/�t�d�H -- CONTACT
            }
            cmdVendor.ExecuteNonQuery();
            cmdVendor.Dispose();
        }

        public static void InsertInventory_Invoice_t(KGVoucherH voucherH, OracleConnection conn,
            string InsertToInventoryCmd, GVApGuiInvoice gVApGuiInvoice, BAccount bAccountVendor,
            string Voucher_Key, string Voucher_No, string Company)
        {
            KGVoucherProcess graph = PXGraph.CreateInstance<KGVoucherProcess>();
            KGVoucherL voucherL = PXSelect<KGVoucherL,
                Where<KGVoucherL.voucherHeaderID, Equal<Required<KGVoucherL.voucherHeaderID>>,
            And<KGVoucherL.accountCD, Equal<Required<KGVoucherL.accountCD>>>>>
            .Select(graph, voucherH.VoucherHeaderID, "1155");
            if (voucherL != null)
            {
                if (gVApGuiInvoice != null)
                {

                    DateTime systemdate = (DateTime)graph.Accessinfo.BusinessDate;
                    DateTime InvoiceDate = (DateTime)gVApGuiInvoice.InvoiceDate;
                    TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
                    string Year = taiwanCalendar.GetYear(InvoiceDate).ToString().Trim();
                    string Month = TwoCharMonth(InvoiceDate.Month.ToString().Trim());//�������X
                    string SystemYear = taiwanCalendar.GetYear(systemdate).ToString().Trim();
                    string SystemMonth = TwoCharMonth(systemdate.Month.ToString().Trim());//�������X
                    string IVO_Kind = "";
                    if (gVApGuiInvoice.GuiType == "21" || gVApGuiInvoice.GuiType == "23" || gVApGuiInvoice.GuiType == "24"
                        || gVApGuiInvoice.GuiType == "25" || gVApGuiInvoice.GuiType == "26" || gVApGuiInvoice.GuiType == "27")
                    {
                        IVO_Kind = "1";
                    }
                    else if (gVApGuiInvoice.GuiType == "22")
                    {
                        IVO_Kind = "2";
                    }
                    else if (gVApGuiInvoice.GuiType == "28" || gVApGuiInvoice.GuiType == "29")
                    {
                        IVO_Kind = "3";
                    }
                    string Tax_code = "";
                    if (gVApGuiInvoice.TaxAmt == 0)
                    {
                        Tax_code = "0";
                    }
                    else if (gVApGuiInvoice.TaxAmt >= 1)
                    {
                        Tax_code = "1";
                    }
                    else if (gVApGuiInvoice.TaxAmt == null)
                    {
                        Tax_code = "2";
                    }

                    //Insert Inventory_Invoice
                    OracleCommand Inventorycmd = new OracleCommand(InsertToInventoryCmd, conn);
                    Inventorycmd.BindByName = true;
                    Inventorycmd.Parameters.Add(":INVOICE_NO", gVApGuiInvoice.GuiInvoiceNbr.Trim()); //gVApGuiInvoice.GuiInvoiceNbr.Trim()

                    Inventorycmd.Parameters.Add(":VOUCHER_NO", Voucher_No);// Voucher_No

                    Inventorycmd.Parameters.Add(":INVOICE_DATE", InvoiceDate);//(DateTime)gVApGuiInvoice.InvoiceDate
                    Inventorycmd.Parameters.Add(":UNIFORM_YY", Year);//Year
                    Inventorycmd.Parameters.Add(":UNIFORM_MM", Month);//Month
                    Inventorycmd.Parameters.Add(":AMOUNT", gVApGuiInvoice.SalesAmt); //gVApGuiInvoice.SalesAmt
                    Inventorycmd.Parameters.Add(":TAX_CODE", Tax_code);
                    Inventorycmd.Parameters.Add(":TAX_AMOUNT", gVApGuiInvoice.TaxAmt);//gVApGuiInvoice.TaxAmt
                    Inventorycmd.Parameters.Add(":CLASS_CODE", "1");//1
                    Inventorycmd.Parameters.Add(":CREATE_DATE", systemdate); //systemdate
                    Inventorycmd.Parameters.Add(":COMPANY", Company); //--���q�OK02
                    Inventorycmd.Parameters.Add(":VOUCHER_KEY", Voucher_Key);//--�P���Yvoucher_key
                    Inventorycmd.Parameters.Add(":ITEM_NO", voucherL.ItemNo);//--�Ǹ���1�}�l
                    Inventorycmd.Parameters.Add(":UNIFORM_NO", bAccountVendor.AcctCD.Trim());//--�����ӽs��(�o��TAB) bAccountVendor.AcctCD.Trim()
                    Inventorycmd.Parameters.Add(":IVO_KIND", IVO_Kind);//--�o�����O(�o��TAB) gVApGuiInvoice.GuiType.Trim()
                    Inventorycmd.Parameters.Add(":BELONG_YY", SystemYear);//--�t�Τ������~3�X(�o��TAB) SystemYear
                    Inventorycmd.Parameters.Add(":BELONG_MM", SystemMonth);//--�t�Τ����2�X(�o��TAB) SystemMonth                             
                    Inventorycmd.Parameters.Add(":PROJECT", voucherH.ContractCD.Trim()); //--�P���Y�M�ץN�XPROJECT
                    Inventorycmd.Parameters.Add(":ACC_UID", "");//--v_VOUCHER_ITEM.Acc_UID,--COMS���ߤ@�����AACUMATICA��VOUCHER_SOURCE
                    Inventorycmd.Parameters.Add(":ACCTYPE", "Sub");//--�T�w��Sub

                    Inventorycmd.ExecuteNonQuery();
                    Inventorycmd.Dispose();
                }
            }

        }

        private static bool CheckInventory(OracleConnection conn, string InventoryCmd)
        {
            OracleCommand cmd = new OracleCommand(InventoryCmd, conn);
            OracleDataAdapter DataAdapter = new OracleDataAdapter();
            DataAdapter.SelectCommand = cmd;

            DataTable dt = new DataTable();
            DataAdapter.Fill(dt);
            if (dt.Rows.Count != 0)
            {
                return true;
            }
            return false;
        }

        public static string TwoCharMonth(string Month)
        {            
            if(Month.Length == 1)
            {
                return "0" + Month;
            }
            else
            {
                return Month;
            }
        }

        #endregion

        #region Payment Date Method
        //���o�����I�ڤ��
        public static  DateTime GetLongTremPaymentDate(PXGraph graph,DateTime PaymentDate, int? PaymentPeriod)
        {
            DateTime LongTrem; 
            double period = (double)PaymentPeriod;
            //�[�W�����ѼƤ��
            LongTrem = PaymentDate.AddDays(period);
            //���o�� �Ӥ���O�_�b�϶���From To
            KGSetUp setUp = PXSelect<KGSetUp>.Select(graph);
            if(setUp.PaymentDateEnd == null || setUp.PaymentDateMid==null ||
                setUp.PaymentDayFrom == null || setUp.PaymentDayTo ==null)
            {
                throw new Exception("�Ц�KG���n�]�w���@�I�ڤ���Ҧ����!");
            }
            int day = LongTrem.Day;
            if(setUp.PaymentDayFrom<=day && day<=setUp.PaymentDayTo)
            {
                LongTrem = DateTime.Parse(LongTrem.ToString("yyyy-MM-"+setUp.PaymentDateMid));             
            }
            else
            {
                LongTrem = (DateTime.Parse(LongTrem.ToString("yyyy-MM-" + setUp.PaymentDateEnd)));
            }
            return LongTrem;
        }
        #endregion
    }
}