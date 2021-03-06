﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using SecuroteckWebApplication.Models;


namespace SecuroteckWebApplication.Controllers
{
    public class ProtectedController : ApiController
    {
        [ActionName("Hello")]
        [CustomAuthorise]
        public string Get(HttpRequestMessage request)
        {
            string apiKey = request.Headers.GetValues("ApiKey").FirstOrDefault();
            string response = "Hello ";
            string username = "";

            User user = UserDatabaseAccess.checkUserRtnUsr(apiKey);
            username = user.UserName;
            response = response + username;
            return response;
                
        }
        
        [CustomAuthorise]
        [ActionName("SHA1")]
        public HttpResponseMessage Getsha1([FromUri]string message, HttpRequestMessage request)
        {
            string apiKey = request.Headers.GetValues("ApiKey").FirstOrDefault();

            HttpStatusCode statusCode = HttpStatusCode.BadRequest;

            bool userExists = false;

            byte[] messageBytes;

            string response = "Bad Request";

            byte[] responseBytes;

            if (message != null)
            {
                if (userExists = UserDatabaseAccess.checkUserKey(apiKey))
                {
                    messageBytes = Encoding.ASCII.GetBytes(message);
                    SHA1 sha = new SHA1CryptoServiceProvider();
                    responseBytes = sha.ComputeHash(messageBytes);
                    response = BitConverter.ToString(responseBytes).Replace("-", "");
                    statusCode = HttpStatusCode.OK;

                }
            }
            return Request.CreateResponse<string>(statusCode, response);
        }

        
        [CustomAuthorise]
        [ActionName("SHA256")]
        public HttpResponseMessage Getsha256([FromUri]string message, HttpRequestMessage request)
        { 
            string apiKey = request.Headers.GetValues("ApiKey").FirstOrDefault();

            HttpStatusCode statusCode = HttpStatusCode.BadRequest;

            bool userExists = false;

            byte[] messageBytes;

            string response = "Bad Request";

            byte[] responseBytes;

            if (message != null)
            {
                if (userExists = UserDatabaseAccess.checkUserKey(apiKey))
                {
                    messageBytes = Encoding.ASCII.GetBytes(message);
                    SHA256 sha = new SHA256CryptoServiceProvider();
                    responseBytes = sha.ComputeHash(messageBytes);
                    response = BitConverter.ToString(responseBytes).Replace("-", "");
                    statusCode = HttpStatusCode.OK;
                }
            }

            return Request.CreateResponse<string>( statusCode, response);
        }


        [CustomAuthorise]
        [ActionName("GetPublicKey")]
        public HttpResponseMessage GetPublicKey(HttpRequestMessage request)
        {
            bool userExists = false;
            string response = "Bad Request";
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;

            string apiKey = request.Headers.GetValues("ApiKey").FirstOrDefault();

            using (var rsa = WebApiConfig.RSA)
            {
                if (userExists = UserDatabaseAccess.checkUserKey(apiKey))
                {
                    var publicKeyXML = rsa.ToXmlString(false);
                    response = publicKeyXML;
                    statusCode = HttpStatusCode.OK;
                }
            }

            return Request.CreateResponse<string>(statusCode, response);
        }

        [CustomAuthorise]
        [ActionName("Sign")]
        public HttpResponseMessage Get(HttpRequestMessage request, [FromUri] string message)
        {
            bool userExists = false;
            string response = "Bad Request";
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;

            string apiKey = request.Headers.GetValues("ApiKey").FirstOrDefault();

            RSACryptoServiceProvider rsa = WebApiConfig.RSA;

            using (rsa)
            {
                if (userExists = UserDatabaseAccess.checkUserKey(apiKey))
                {
                    byte[] signedMessage;

                    byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                    
                    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    
                    signedMessage = rsa.SignData(messageBytes, sha1);

                    response = BitConverter.ToString(signedMessage);

                    statusCode = HttpStatusCode.OK;
                }
            }
            return Request.CreateResponse<string>(statusCode, response);
        }
    }
}
