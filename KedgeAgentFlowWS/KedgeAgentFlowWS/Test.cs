using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Kedge.ws.AgentflowWeb;
namespace Kedge.ws
{
    

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Module)]
    public class FodyTestAttribute : Attribute
    {
        protected object InitInstance;
        protected MethodBase InitMethod;
        protected Object[] Args;
        public void Init(object instance, MethodBase method, object[] args)
        {
            InitMethod = method;
            InitInstance = instance;
            Args = args;
        }
        public void OnEntry()
        {
            Console.WriteLine("Before");
        }
        public void OnExit()
        {
            Console.WriteLine("After");
        }
        public void OnException(Exception exception)
        {
        }
    }


    public    class Test
    {
        public void test2() {
            /*using (AgentflowWebService soapClient = new AgentflowWebService())
            {

                //String s2 = soapClient.getCompany();
                //String s2 = soapClient.hello();
                soapClient.login("phille", "phille");
                String a = soapClient.createROProcess("phille", "Req1902261740133790007");
                soapClient.logout("phille");
                //String s = soapClient.createROProcess("phille", "Req1902261740133790007");
                //throw new PXException(s);
            }*/
            /*
            using (Kedge.ws.EngineerBillWeb.EngineerBillWebService soapClient = new Kedge.ws.EngineerBillWeb.EngineerBillWebService() ) {
                soapClient.doStart("123","456");
            }*/
            using (Kedge.ws.EngineerPR.EngineerPRService soapClient = new Kedge.ws.EngineerPR.EngineerPRService())
            {
                soapClient.createROProcessKedgeURL("phille", "Req1902261740133790007");
                soapClient.createROProcessKedge("phille", "Req1902261740133790007"); //工程請購 流程
                soapClient.createPOProcessKedge("phille", "Req1902261740133790007");//工程採購 流程 
                soapClient.createContractProcessKedge("phille", "Req1902261740133790007");//小包合約會辦 流程 
            }
            using (Kedge.ws.EngineerERPWeb.EngineerERPWebService soapClient = new EngineerERPWeb.EngineerERPWebService())
            {
                //RefNbr
                soapClient.openEngTravleAPEForm("phille", "AP000001","Y");
            }
            using (Kedge.ws.ExpenseClaimWeb.KEDGEWebServiceService soapClient = new ws.ExpenseClaimWeb.KEDGEWebServiceService())
            {
                soapClient.createProccessBill("2", "AfmNo", "AgentFlowID", "AgentFlowID");
            }
        }
    }
}
