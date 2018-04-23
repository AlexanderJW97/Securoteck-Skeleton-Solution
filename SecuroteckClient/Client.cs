using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Cryptography;
using System.Xml;

namespace SecuroteckClient
{
    #region Task 8 and beyond
    class Client
    {
        private static string request = "";
        private static int portNumber = 24702;
        private static string controller = "";
        private static string action = "";
        private static string[] function = new string[2];
        private static string requestForServer = "";
        private static string storedUsername = "";
        private static Guid storedApiKey;
        private static Guid emptyGuid;
        private static string emptyGuidStr = emptyGuid.ToString();
        private static bool userExitRequest = false;
        private static int numRequests = 0;
        private static string userPostName = "";
        private static string publickey = "";



        static void Main(string[] args)
        {



            request = Start();

            do
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:" + portNumber + "/api/");
                client.Timeout = TimeSpan.FromMilliseconds(20000);
                //client.MaxResponseContentBufferSize = 

                if (numRequests > 0)
                    request = Restart();

                string determineFunctionString = DetermineFunction(request);

                if (determineFunctionString != "Exit")
                {
                    function = determineFunctionString.Split('/');
                    requestForServer = launchFunction(function, request);
                    RunAsync(requestForServer, client, controller, action).Wait();
                }
                else
                {
                    userExitRequest = true;
                }


            } while (userExitRequest == false);

        }

        /// <summary>
        /// Starts the client by asking what they want to do
        /// </summary>
        /// <returns>returns the user input</returns>
        static string Start()
        {
            Console.WriteLine("Hello. What would you like to do?");
            return Console.ReadLine();
        }

        static string Restart()
        {
            Console.WriteLine("What would you like to do next?");
            string request = Console.ReadLine();
            Console.Clear();
            requestForServer = "";
            return request;

        }

        /// <summary>
        /// Determines the function the user has asked for
        /// </summary>
        /// <param name="request">the users input</param>
        /// <returns>returns a string consisting of the controller name and the action to be performed, split with a backslash</returns>
        static string DetermineFunction(string request)
        {
            string function = "";

            if (request == "Exit")
            {
                function = request;
            }
            else if (request != "")
            {
                string[] requestParts = request.Split(' ');
                controller = requestParts[0];
                action = requestParts[1];
                function = controller + "/" + action;
            }
            return function;

        }

        /// <summary>
        /// Launches the required method in order to build a request
        /// </summary>
        /// <param name="function">the string array containing the action and controller</param>
        /// <param name="originalRequest">the original request sent by the user</param>
        /// <returns>returns the request for the server</returns>
        static string launchFunction(string[] function, string originalRequest)
        {
            string request = "";
            controller = function[0];
            action = function[1];
            switch (controller)
            {
                case "TalkBack":
                    controller = "talkback";
                    switch (action)
                    {
                        case "Hello":
                            action = "hello";
                            request = TalkbackHello(controller, action);
                            break;
                        case "Sort":
                            action = "sort";
                            request = TalkbackSort(originalRequest, controller, action);
                            break;
                    }
                    break;

                case "User":
                    {
                        controller = "user";
                        switch (action)
                        {
                            case "Get":
                                action = "new";
                                request = UserGet(controller, action, originalRequest);
                                break;
                            case "Set":
                                UserSet(originalRequest);
                                break;
                            case "Post":
                                action = "new";
                                request = UserPost(controller, action, originalRequest);
                                break;
                            case "Delete":
                                action = "delete";
                                request = UserDelete(controller, action, originalRequest);
                                break;
                        }
                    }
                    break;

                case "Protected":
                    {
                        controller = "protected";
                        switch (action)
                        {
                            case "Hello":
                                action = "hello";
                                request = ProtectedHello(controller, action);
                                break;
                            case "SHA1":
                                action = "sha1";
                                request = ProtectedSHA1(controller, action, originalRequest);
                                break;
                            case "SHA256":
                                action = "sha256";
                                request = ProtectedSHA256(controller, action, originalRequest);
                                break;
                            case "Get":
                                action = "getpublickey";
                                request = ProtectedGetPublicKey(controller, action);
                                break;
                            case "Sign":
                                action = "sign";
                                request = ProtectedSign(controller, action, originalRequest);
                                break;
                        }
                    }
                    break;

            }
            return request;

        }

        private static string ProtectedSign(string controller, string action, string originalRequest)
        {
            if (storedApiKey.Equals(emptyGuid))
            {
                Console.WriteLine("You need to do a User Post or User Set first");
            }
            else
            {
                string[] requestParts = originalRequest.Split(' ');
                string message = "";
                for (int i = 2; i < requestParts.Length; i++)
                    message += requestParts[i];
                requestForServer += controller + "/" + action + "?message=" + message;
            }
            return requestForServer;

        }

        private static string ProtectedGetPublicKey(string controller, string action)
        {
            if (storedApiKey.ToString() == emptyGuidStr)
            {
                Console.WriteLine("You need to do a User Post or User Set first");
            }
            else
            {
                requestForServer += controller + "/" + action;
            }
            return requestForServer;
        }

        private static string ProtectedSHA1(string controller, string action, string originalRequest)
        {
            string[] requestParts = originalRequest.Split(' ');
            string message = requestParts[2];
            string storedApiKeyStr = storedApiKey.ToString();
            if (storedApiKeyStr == emptyGuidStr)
            {
                Console.WriteLine("You need to do a User Post or User Set first");
            }
            else
            {
                requestForServer += controller + "/" + action + "?message=" + message;
            }
            return requestForServer;
        }

        private static string ProtectedSHA256(string controller, string action, string originalRequest)
        {
            string[] requestParts = originalRequest.Split(' ');
            string message = requestParts[2];
            string storedApiKeyStr = storedApiKey.ToString();
            if (storedApiKeyStr == emptyGuidStr)
            {
                Console.WriteLine("You need to do a User Post or User Set first");
            }
            else
            {
                requestForServer += controller + "/" + action + "?message=" + message;
            }
            return requestForServer;
        }

        private static string ProtectedHello(string controller, string action)
        {
            string storedApiKeyStr = storedApiKey.ToString();
            if (storedApiKeyStr == emptyGuidStr)
            {
                Console.WriteLine("You need to do a User Post or User Set first");
            }
            else
            {
                requestForServer += controller + "/" + action;
            }
            return requestForServer;
        }

        private static string UserDelete(string controller, string action, string originalRequest)
        {
            string[] requestParts = originalRequest.Split(' ');

            string storedApiKeyStr = storedApiKey.ToString();
            if (storedUsername == "" && storedApiKeyStr == emptyGuidStr)
            {
                Console.WriteLine("You need to do a User Post or User Set ");
            }
            requestForServer += controller + "/removeuser?username=" + storedUsername;

            return requestForServer;
        }

        private static string UserPost(string controller, string action, string originalRequest)
        {
            string[] requestParts = originalRequest.Split(' ');

            userPostName = requestParts[2];

            requestForServer += controller + "/" + action;

            return requestForServer;
        }

        private static void UserSet(string originalRequest)
        {
            string response = "";
            try
            {
                string[] requestParts = originalRequest.Split(' ');
                storedUsername = requestParts[2];
                storedApiKey = Guid.Parse(requestParts[3]);
                response = "Stored";
            }
            catch
            {
                response = "Username and ApiKey could not be saved.";
            }
            Console.WriteLine(response);
        }

        private static string UserGet(string controller, string action, string originalRequest)
        {
            string[] requestParts = originalRequest.Split(' ');
            string username = "";
            username = requestParts[2];
            requestForServer += controller + "/new?username=" + username;
            return requestForServer;
        }

        private static string TalkbackHello(string controller, string action)
        {
            requestForServer += controller + "/" + action;
            return requestForServer;
        }

        static string TalkbackSort(string originalRequest, string controller, string action)
        {
            string[] requestParts = originalRequest.Split(' ');

            int numInts = 0;

            foreach (char c in requestParts[2])
            {
                if (char.IsDigit(c))
                {
                    numInts++;
                }
            }

            string[] sortInts = (requestParts[2].ToString()).Split(',');

            string sortIntsString = "";

            int i = 0;

            foreach (string s in sortInts)
            {
                foreach (char c in s)
                {
                    if (char.IsDigit(c))
                        sortIntsString += c.ToString();

                }
                sortInts[i] = sortIntsString;
                sortIntsString = "";
                i++;
            }

            int k = 0;

            foreach (string j in sortInts)
            {
                string integer = "";

                if (k != 0)
                {
                    integer = "&integers=" + j.ToString();
                    sortIntsString += integer;
                }
                else
                {
                    integer = "integers=" + j.ToString();
                    sortIntsString += integer;
                }
                k++;
            }

            requestForServer += controller + "/" + action + "?" + sortIntsString;
            return requestForServer;

        }

        static async Task RunAsync(string requestForServer, HttpClient client, string controller, string action)
        {
            HttpClient runAsyncClient = client;
            runAsyncClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


            try
            {

                Task<string> taskStr;
                Task<bool> taskBool;



                switch (controller)
                {
                    case "talkback":
                        {
                            taskStr = GetStringAsync(requestForServer, runAsyncClient);
                            if (await Task.WhenAny(taskStr, Task.Delay(20000)) == taskStr)
                            {
                                Console.WriteLine(taskStr.Result);
                                numRequests++;
                            }
                            else
                            {
                                Console.WriteLine("Request timed out");
                            }

                        }
                        break;
                    case "user":
                        {
                            switch (action)
                            {
                                case "get":
                                    {
                                        taskStr = GetStringAsync(requestForServer, runAsyncClient);
                                        if (await Task.WhenAny(taskStr, Task.Delay(20000)) == taskStr)
                                        {
                                            Console.WriteLine(taskStr.Result);
                                            numRequests++;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Request timed out");
                                        }
                                        break;
                                    }
                                case "set":
                                    break;
                                case "new":
                                    {
                                        taskStr = PostStringAsync(requestForServer, runAsyncClient, userPostName);
                                        if (await Task.WhenAny(taskStr, Task.Delay(20000)) == taskStr)
                                        {
                                            Console.WriteLine("Got API key.");
                                            string storedApiKeyStr = taskStr.Result.Replace("\"", "");
                                            storedApiKey = Guid.Parse(storedApiKeyStr);
                                            
                                            numRequests++;
                                        }

                                        else
                                        {
                                            Console.WriteLine("Request timed out");
                                        }
                                        break;
                                    }

                                case "delete":
                                    {
                                        taskBool = Delete(requestForServer, client);
                                        if (taskBool.Result == true)
                                            Console.WriteLine("True");
                                        else if (taskBool.Result == false)
                                            Console.WriteLine("False");

                                        break;
                                    }
                            }
                            break;
                        }
                    case "protected":
                        {
                            switch (action)
                            {
                                case "hello":
                                    {
                                        if (storedApiKey.Equals(emptyGuid))
                                        {
                                            Console.WriteLine("You need to do a User Post or User Set first.");
                                        }
                                        else if (storedApiKey != null)
                                        {
                                            string storedApikeyStr = storedApiKey.ToString();
                                            client.DefaultRequestHeaders.Add("apikey", storedApikeyStr);
                                            taskStr = GetStringAsync(requestForServer, client);
                                            if (await Task.WhenAny(taskStr, Task.Delay(20000)) == taskStr)
                                            {
                                                Console.WriteLine(taskStr.Result);
                                                numRequests++;
                                            }
                                        }
                                        break;
                                    }
                                case "sha1":
                                    {
                                       
                                        if (storedApiKey.Equals(emptyGuid))
                                        {
                                            Console.WriteLine("You need to do a User Post or User Set first.");
                                        }
                                        else if (storedApiKey != null)
                                        {
                                            string storedApikeyStr = storedApiKey.ToString();
                                            client.DefaultRequestHeaders.Add("apikey", storedApikeyStr);
                                            taskStr = GetStringAsync(requestForServer, client);
                                            if (await Task.WhenAny(taskStr, Task.Delay(20000)) == taskStr)
                                            {
                                                Console.WriteLine(taskStr.Result);
                                                numRequests++;
                                            }
                                        }
                                        break;
                                    }
                                case "sha256":
                                    {
                                        
                                        if (storedApiKey.Equals(emptyGuid))
                                        {
                                            Console.WriteLine("You need to do a User Post or User Set first.");
                                        }
                                        else if (storedApiKey != null)
                                        {
                                            string storedApikeyStr = storedApiKey.ToString();
                                            client.DefaultRequestHeaders.Add("apikey", storedApikeyStr);
                                            taskStr = GetStringAsync(requestForServer, client);
                                            if (await Task.WhenAny(taskStr, Task.Delay(20000)) == taskStr)
                                            {
                                                Console.WriteLine(taskStr.Result);
                                                numRequests++;
                                            }
                                        }
                                        break;
                                    }
                                case "getpublickey":
                                    {
                                        if (storedApiKey.Equals(emptyGuid))
                                        {
                                            Console.WriteLine("You need to do a User Post or User Set first.");
                                        }
                                        else if (storedApiKey != null)
                                        {
                                            string storedApikeyStr = storedApiKey.ToString();
                                            client.DefaultRequestHeaders.Add("apikey", storedApikeyStr);
                                            taskStr = GetStringAsync(requestForServer, client);
                                            if (await Task.WhenAny(taskStr, Task.Delay(20000)) == taskStr)
                                            {
                                                if (taskStr.Result != null)
                                                {
                                                    var key = taskStr.Result;
                                                    publickey = XmlConvert.ToString(key);
                                                    Console.WriteLine("Got Public Key");
                                                    numRequests++;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Couldn't get the Public Key");
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case "sign":
                                    {
                                        if (storedApiKey.Equals(emptyGuid))
                                        {
                                            Console.WriteLine("You need to do a User Post or User Set first.");
                                        }
                                        else if (storedApiKey != null)
                                        {
                                            string storedApikeyStr = storedApiKey.ToString();
                                            client.DefaultRequestHeaders.Add("apikey", storedApikeyStr);
                                            taskStr = GetStringAsync(requestForServer, client);
                                            if (await Task.WhenAny(taskStr, Task.Delay(20000)) == taskStr)
                                            {
                                                if (taskStr.Result != null)
                                                {
                                                    publickey = taskStr.Result;
                                                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                                                    rsa.FromXmlString(publickey);
                                                    RSAParameters pubKey = rsa.ToXMLString();
                                                    Console.WriteLine("Message was successfully signed");
                                                    numRequests++;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Couldn't get the Public Key");
                                                }
                                            }
                                        }
                                        break;
                                    }
                            }

                            break;
                        }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }

        }

        static async Task<string> GetStringAsync(string path, HttpClient client)
        {
            Console.WriteLine("...please wait...");
            string responseString = "";
            HttpResponseMessage response = await client.GetAsync(path);
            responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }


        static async Task<string> PostStringAsync(string path, HttpClient client, string requestBody)
        {
            Console.WriteLine("...please wait...");
            string responseString = "";
            HttpResponseMessage response = await client.PostAsJsonAsync(path, requestBody);
            responseString = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                storedUsername = requestBody;

            }
            return responseString;
        }
        static async Task<bool> Delete(string path, HttpClient client)
        {
            //path += storedUsername;
            Console.WriteLine("...please wait...");
            bool userDeleted = false;
            string storedApiKeyStr = storedApiKey.ToString();
            client.DefaultRequestHeaders.Add("apikey", storedApiKeyStr);
            
            HttpResponseMessage response = await client.DeleteAsync(path); 
            string userDeletedStr = await response.Content.ReadAsStringAsync();

            switch (userDeletedStr)
            {
                case "true":
                    userDeleted = true;
                    break;
                case "false":
                    userDeleted = false;
                    break;
            }

            return userDeleted;
        }
        /*static async Task<string> SetStringAsync(string path, HttpClient client, string requestBody)
        {
            Console.WriteLine("...please wait...");
            string responseString = "";
            HttpResponseMessage response = await client.PutAsJsonAsync(path, HttpContent);
            responseString = await response.Content.ReadAsStringAsync();


            return responseString;
        }
        */

    }
    #endregion
}
