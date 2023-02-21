using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//https://www.itread01.com/content/1541149285.html //參考網站
namespace Kedge.DAC
{
    public class ApprovalUtil 
    {

        public class LoginInfo
        {
            public string name { get; set; }
            public string password { get; set; }
            public string company { get; set; }
        }
        private const string loginUriTest = "/entity/auth/login";
        private const string checkApproveUriTest = "/entity/Kedge/17.200.001/Approvals/";
        private const string approveUriTest = "/entity/Kedge/17.200.001/Approvals/Process";
        private const string loginOutTest = "/entity/auth/logout";
        private const string rejectTest = "/entity/Kedge/17.200.001/Approvals/reject";

        /*
        private const string loginUriTest = "http://localhost/ERPAQDB/entity/auth/login";
        private const string checkApproveUriTest = "http://localhost/ERPAQDB/entity/Kedge/17.200.001/Approvals/";
        private const string approveUriTest = "http://localhost/ERPAQDB/entity/Kedge/17.200.001/Approvals/Process";
        private const string loginOutTest = "http://localhost/ERPAQDB/entity/auth/logout";*/

        /*
        private const string loginUriTest = "https://clouderp.kedge.com.tw/KedgeDEV/entity/auth/login";
        private const string checkApproveUriTest = "https://clouderp.kedge.com.tw/KedgeDev/entity/Kedge/17.200.001/Approvals/";
        private const string approveUriTest = "https://clouderp.kedge.com.tw/KedgeDev/entity/Kedge/17.200.001/Approvals/Process";
        private const string loginOutTest = "https://clouderp.kedge.com.tw/KedgeDev/entity/auth/logout";*/

        public ApprovalUtil()
        {
        }
        //暫時不用停止維修
        public   static async void Run(String name,String password,String company,int approvalID)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    /*
                    var info = new LoginInfo
                    {
                        name = "louis.lu",
                        password = "louis@lu",
                        company = "Company",
                    };*/
                    var info = new LoginInfo
                    {
                        name = "louis.lu",
                        password = "louis@lu",
                        company = "MyCompany",
                    };

                    //string json = @"{ 'name':'louis.lu','password':'louis@lu','company':'Company'}";
                    //string json = @"{ 'name':'Jerry','password':'Welcome1','company':'Company'}";
                    string json = JsonConvert.SerializeObject(info);
                    HttpContent fooContent = new StringContent(json, Encoding.ASCII, "application/json");
                    // HttpRequestMessage request = await client.PostAsync(uri, fooContent);
                    HttpResponseMessage response = await client.PostAsync(loginUriTest, fooContent);
                    //HttpResponseMessage response = await client.PutAsync(uri, json);
                    //HttpResponseMessage response = await client.GetAsync(uri);
                    Console.WriteLine(response.ToString());
                    //response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                    Console.WriteLine("----------------------------");
                    Console.WriteLine(response.StatusCode.ToString());
                    Console.WriteLine((int)response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                    Console.WriteLine(response.IsSuccessStatusCode);
                    //Console.WriteLine(response.RequestMessage);
                    Console.WriteLine("----------------------------");
                    //---------------------------------

                    //確認審核

                    try
                    {
   
                        //String url = checkApproveUriTest + 5216;
                        String url = checkApproveUriTest + approvalID;//For DEV
                        HttpResponseMessage response2 = await client.GetAsync(url);
                        Console.WriteLine(response2.ToString());
                        //response.EnsureSuccessStatusCode();

                        string responseBody2 = await response2.Content.ReadAsStringAsync();

                        Console.WriteLine("----------------------------");

                        Console.WriteLine(url);
                        Console.WriteLine(responseBody2);
                        Console.WriteLine(response2.StatusCode.ToString());
                        Console.WriteLine(response2.ReasonPhrase);
                        Console.WriteLine("----------------------------");
                    }

                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);
                    }


                    //int approvalID = 3350; //For Dev
                    //int approvalID = 5216;
                    //審核
                    try
                    {
                        //String approveInfo = "{'entity':['RQ301000']}";
                        String approveInfo = "{'entity':{'ApprovalID' :  { 'value': " + approvalID + "},'Selected' : { 'value' : true}}}";

                        HttpContent approveInfoContent = new StringContent(approveInfo, Encoding.UTF8, "application/json");
                        HttpResponseMessage approveRes = await client.PostAsync(approveUriTest, approveInfoContent);

                        //response.EnsureSuccessStatusCode();

                        string approveResBody = await approveRes.Content.ReadAsStringAsync();

                        Console.WriteLine("開始審核----------------------------");
                        Console.WriteLine(approveUriTest);
                        Console.WriteLine(approveRes.ToString());
                        Console.WriteLine(approveResBody.ToString());

                        Console.WriteLine(approveInfo);
                        Console.WriteLine(approveRes.StatusCode.ToString());
                        Console.WriteLine((int)approveRes.StatusCode);
                        Console.WriteLine(approveRes.ReasonPhrase);
                        Console.WriteLine(approveRes.IsSuccessStatusCode);
                        //Console.WriteLine(response.RequestMessage);
                        Console.WriteLine("----------------------------");
                    }

                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);
                    }
                    //---------------------------------

                    //登出
        
                    fooContent = new StringContent(json, Encoding.ASCII, "application/json");
           
                    HttpResponseMessage response3 = await client.PostAsync(loginOutTest, fooContent);
                    //HttpResponseMessage response = await client.PutAsync(uri, json);
                    //HttpResponseMessage response = await client.GetAsync(uri);
                
                    Console.WriteLine(response3.ToString());
                    //response.EnsureSuccessStatusCode();
               
                    string responseBody3 = await response3.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody3);
                    Console.WriteLine("----------------------------");
                    Console.WriteLine(response3.StatusCode.ToString());
                    Console.WriteLine((int)response3.StatusCode);
                    Console.WriteLine(response3.ReasonPhrase);
                    Console.WriteLine(response3.IsSuccessStatusCode);
                    //Console.WriteLine(response.RequestMessage);
                    Console.WriteLine("----------------------------");

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }


        }
        public static void Reject( int? approvalID, ref String approveMessage)
        {
            String companyName = PX.Data.PXLogin.ExtractCompany(PX.Common.PXContext.PXIdentity.IdentityName);
            Reject("louis.lu", "louis@lu", companyName, approvalID, ref approveMessage);
        }
        public static void Reject(String name, String password, String company, int? approvalID, ref String approveMessage)
        {
            String uri = PX.Common.PXUrl.SiteUrlWithPath();

            //int myCompany = PX.Data.Update.PXInstanceHelper.CurrentCompany;
            //String companyName=    PX.Data.PXLogin.ExtractCompany(PX.Common.PXContext.PXIdentity.IdentityName);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(100);
                    /*
                    var info = new LoginInfo
                    {
                        name = "louis.lu",
                        password = "louis@lu",
                        company = "Company",
                    };*/
                    //company = "MyCompany",
                    var info = new LoginInfo
                    {
                        name = name,
                        password = password,
                        company = company,
                    };

                    //string json = @"{ 'name':'louis.lu','password':'louis@lu','company':'Company'}";
                    //string json = @"{ 'name':'Jerry','password':'Welcome1','company':'Company'}";
                    string json = JsonConvert.SerializeObject(info);
                    HttpContent loginContent = new StringContent(json, Encoding.ASCII, "application/json");
                    // HttpRequestMessage request = await client.PostAsync(uri, fooContent);
                    var loginTask = client.PostAsync(uri + loginUriTest, loginContent);
                    var response = loginTask.Result;

                    Console.WriteLine(response.ToString());
                    var loginContentTask = response.Content.ReadAsStringAsync();
                    Console.WriteLine(loginContentTask.Result);
                    Console.WriteLine("----------------------------");
                    Console.WriteLine(response.StatusCode.ToString());
                    int statusCode = (int)response.StatusCode;

                    Console.WriteLine(statusCode);
                    Console.WriteLine(response.ReasonPhrase);
                    Console.WriteLine(response.IsSuccessStatusCode);
                    Console.WriteLine("----------------------------");
                    if (statusCode != 204)
                    {
                        //approveMessage = "自動審核功能-登入失敗-請手定審核";
                        approveMessage = "自動審核功能失敗-請手定審核";
                        return;
                    }
                    //確認審核

                    try
                    {

                        //String url = checkApproveUriTest + 5216;
                        String url = uri + checkApproveUriTest + approvalID;//For DEV
                        var approveCheckTask = client.GetAsync(url);
                        var response2 = approveCheckTask.Result;

                        Console.WriteLine(response2.ToString());
                        //response.EnsureSuccessStatusCode();

                        var approveCheckContentTask = response2.Content.ReadAsStringAsync();
                        //string responseBody2 =  response2.Content.ReadAsStringAsync();

                        Console.WriteLine("----------------------------");
                        Console.WriteLine(url);
                        Console.WriteLine(approveCheckContentTask.Result);
                        statusCode = (int)response2.StatusCode;
                        Console.WriteLine(statusCode);
                        Console.WriteLine(response2.ReasonPhrase);
                        Console.WriteLine("----------------------------");
                        if (statusCode != 202)
                        {
                            //approveMessage = "自動審核功能-取得核可文件失敗-請手定審核";
                            approveMessage = "自動審核功能失敗-請手定審核";
                            loginContent = new StringContent(json, Encoding.ASCII, "application/json");
                            //登出返回
                            client.PostAsync(uri + loginOutTest, loginContent);
                            return;
                        }
                    }

                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);
                    }


                    //int approvalID = 3350; //For Dev
                    //int approvalID = 5216;
                    //審核
                    try
                    {
                        String approveInfo = "{'entity':{'ApprovalID' :  { 'value': " + approvalID + "},'Selected' : { 'value' : true}}}";
                        HttpContent approveInfoContent = new StringContent(approveInfo, Encoding.UTF8, "application/json");
                        var approveTask = client.PostAsync(uri + rejectTest, approveInfoContent);
                        var approveRes = approveTask.Result;
                        //HttpResponseMessage approveRes =  client.PostAsync(approveUriTest, approveInfoContent);
                        //response.EnsureSuccessStatusCode();
                        var approveContentTask = approveRes.Content.ReadAsStringAsync();

                        Console.WriteLine("開始拒絕----------------------------");
                        Console.WriteLine(uri + rejectTest);
                        Console.WriteLine(approveRes.ToString());
                        Console.WriteLine(approveContentTask.Result);

                        Console.WriteLine(approveInfo);
                        Console.WriteLine(approveRes.StatusCode.ToString());
                        statusCode = (int)approveRes.StatusCode;
                        Console.WriteLine(statusCode);

                        Console.WriteLine(approveRes.ReasonPhrase);
                        Console.WriteLine(approveRes.IsSuccessStatusCode);
                        //Console.WriteLine(response.RequestMessage);
                        Console.WriteLine("----------------------------");
                        if (statusCode != 202)
                        {
                            //approveMessage = "自動審核功能-拒絕失敗-請手定審核";
                            approveMessage = "自動審核功能失敗-請手定審核";
                            loginContent = new StringContent(json, Encoding.ASCII, "application/json");
                            //登出返回
                            client.PostAsync(uri + loginOutTest, loginContent);
                            return;
                        }
                    }

                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);
                    }
                    //---------------------------------

                    //登出
                    loginContent = new StringContent(json, Encoding.ASCII, "application/json");
                    var logOutTask = client.PostAsync(uri + loginOutTest, loginContent);
                    var response3 = logOutTask.Result;
                    Console.WriteLine(response3.ToString()); ;
                    var logOutContentTask = response3.Content.ReadAsStringAsync();
                    Console.WriteLine(logOutContentTask.Result);
                    Console.WriteLine("----------------------------");
                    Console.WriteLine(response3.StatusCode.ToString());
                    statusCode = (int)response3.StatusCode;
                    Console.WriteLine(statusCode);
                    Console.WriteLine(response3.ReasonPhrase);
                    Console.WriteLine(response3.IsSuccessStatusCode);
                    //Console.WriteLine(response.RequestMessage);
                    Console.WriteLine("----------------------------");

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
        }
        public static void Approve(int? approvalID, ref String approveMessage)
        {
            String companyName = PX.Data.PXLogin.ExtractCompany(PX.Common.PXContext.PXIdentity.IdentityName);
            Approve("louis.lu", "louis@lu", companyName, approvalID, ref approveMessage);
        }



        public static  void Approve(String name, String password, String company, int? approvalID, ref String approveMessage)
        {
            String uri     = PX.Common.PXUrl.SiteUrlWithPath();

            //int myCompany = PX.Data.Update.PXInstanceHelper.CurrentCompany;
            //String companyName=    PX.Data.PXLogin.ExtractCompany(PX.Common.PXContext.PXIdentity.IdentityName);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(100);
                    /*
                    var info = new LoginInfo
                    {
                        name = "louis.lu",
                        password = "louis@lu",
                        company = "Company",
                    };*/
                    //company = "MyCompany",
                    var info = new LoginInfo
                    {
                        name = name,
                        password = password,
                        company = company,
                    };

                    //string json = @"{ 'name':'louis.lu','password':'louis@lu','company':'Company'}";
                    //string json = @"{ 'name':'Jerry','password':'Welcome1','company':'Company'}";
                    string json = JsonConvert.SerializeObject(info);
                    HttpContent loginContent = new StringContent(json, Encoding.ASCII, "application/json");
                    // HttpRequestMessage request = await client.PostAsync(uri, fooContent);
                    var loginTask =  client.PostAsync(uri+loginUriTest, loginContent);
                    var response = loginTask.Result;

                    Console.WriteLine(response.ToString());
                    var loginContentTask =  response.Content.ReadAsStringAsync();
                    Console.WriteLine(loginContentTask.Result);
                    Console.WriteLine("----------------------------");
                    Console.WriteLine(response.StatusCode.ToString());
                    int statusCode=   (int)response.StatusCode;
                  
                    Console.WriteLine(statusCode);
                    Console.WriteLine(response.ReasonPhrase);
                    Console.WriteLine(response.IsSuccessStatusCode);
                    Console.WriteLine("----------------------------");
                    if (statusCode != 204)
                    {
                        approveMessage = "自動審核功能-登入失敗-請手定審核-" + statusCode;
                       // approveMessage = "自動審核功能失敗-請手定審核";
                        return;
                    }
                    //確認審核

                    try
                    {

                        //String url = checkApproveUriTest + 5216;
                        String url = uri + checkApproveUriTest + approvalID;//For DEV
                        var approveCheckTask = client.GetAsync(url);
                        var response2 = approveCheckTask.Result;
                      
                        Console.WriteLine(response2.ToString());
                        //response.EnsureSuccessStatusCode();

                        var approveCheckContentTask = response2.Content.ReadAsStringAsync();
                        //string responseBody2 =  response2.Content.ReadAsStringAsync();

                        Console.WriteLine("----------------------------");
                        Console.WriteLine(url);
                        Console.WriteLine(approveCheckContentTask.Result);
                        statusCode = (int)response2.StatusCode;
                        Console.WriteLine(statusCode);
                        Console.WriteLine(response2.ReasonPhrase);
                        Console.WriteLine("----------------------------");
                        
                        if (statusCode != 202 && statusCode != 200)
                        {
                            approveMessage = "自動審核功能-取得核可文件失敗-請手定審核-" + statusCode;
                            //approveMessage = "自動審核功能失敗-請手定審核-" + statusCode;
                            loginContent = new StringContent(json, Encoding.ASCII, "application/json");
                            //登出返回
                            client.PostAsync(uri + loginOutTest, loginContent);
                            return;
                        }
                    }

                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);
                    }


                    //int approvalID = 3350; //For Dev
                    //int approvalID = 5216;
                    //審核
                    try
                    {
                        String approveInfo = "{'entity':{'ApprovalID' :  { 'value': " + approvalID + "},'Selected' : { 'value' : true}}}";
                        HttpContent approveInfoContent = new StringContent(approveInfo, Encoding.UTF8, "application/json");
                        var approveTask = client.PostAsync(uri + approveUriTest, approveInfoContent);
                        var approveRes = approveTask.Result;
                        //HttpResponseMessage approveRes =  client.PostAsync(approveUriTest, approveInfoContent);
                        //response.EnsureSuccessStatusCode();
                        var approveContentTask =   approveRes.Content.ReadAsStringAsync();

                        Console.WriteLine("開始審核----------------------------");
                        Console.WriteLine(uri + approveUriTest);
                        Console.WriteLine(approveRes.ToString());
                        Console.WriteLine(approveContentTask.Result);

                        Console.WriteLine(approveInfo);
                        Console.WriteLine(approveRes.StatusCode.ToString());
                        statusCode = (int)approveRes.StatusCode;
                        Console.WriteLine(statusCode);
                        
                        Console.WriteLine(approveRes.ReasonPhrase);
                        Console.WriteLine(approveRes.IsSuccessStatusCode);
                        //Console.WriteLine(response.RequestMessage);
                        Console.WriteLine("----------------------------");
                        if (statusCode != 202)
                        {
                            approveMessage = "自動審核功能-審核失敗-請手定審核-" + statusCode;
                            //approveMessage = "自動審核功能失敗-請手定審核-"+ statusCode;
                            loginContent = new StringContent(json, Encoding.ASCII, "application/json");
                            //登出返回
                            client.PostAsync(uri + loginOutTest, loginContent);
                            return;
                        }
                        else {
                            approveMessage = "自動審核功能-成功-請重新整理畫面"+ statusCode;
                        }
                    }

                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);
                    }
                    //---------------------------------

                    //登出
                    loginContent = new StringContent(json, Encoding.ASCII, "application/json");
                    var logOutTask =  client.PostAsync(uri + loginOutTest, loginContent);
                    var response3 = logOutTask.Result;
                    Console.WriteLine(response3.ToString());;
                    var logOutContentTask =  response3.Content.ReadAsStringAsync();
                    Console.WriteLine(logOutContentTask.Result);
                    Console.WriteLine("----------------------------");
                    Console.WriteLine(response3.StatusCode.ToString());
                    statusCode = (int)response3.StatusCode;
                    Console.WriteLine(statusCode);
                    Console.WriteLine(response3.ReasonPhrase);
                    Console.WriteLine(response3.IsSuccessStatusCode);
                    //Console.WriteLine(response.RequestMessage);
                     Console.WriteLine("----------------------------");

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
        }
    }
}