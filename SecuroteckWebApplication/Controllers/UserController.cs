﻿using SecuroteckWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SecuroteckWebApplication.Controllers
{
    public class UserController : ApiController
    {
        [ActionName("New")]
        public HttpResponseMessage Get(HttpResponseMessage request, string username)
        {
            string response = "";

            if (username == "") //checks if username is empty
                response = "False - User Does Not Exist!Did you mean to do a POST to create a new user?";

            if (!UserDatabaseAccess.checkUserName(username)) //if user does not exist
            {
                response = "False - User Does Not Exist! Did you mean to do a POST to create a new user?";
            }
            if (UserDatabaseAccess.checkUserName(username)) //if user does exist
            {
                response = "True - User Does Exist! Did you mean to do a POST to create a new user?";
            }
            return Request.CreateResponse<string>(HttpStatusCode.OK, response);
        }

        [ActionName("New")]
        public HttpResponseMessage Post([FromBody]JToken jtoken)
        {
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            string username = jtoken["username"].ToObject<string>();

            string response = "";
            if (username == null)
            {
                response = "Oops. Make sure your body contains a string with your username and your Content-Type is Content - Type:application / json";
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (!(username == ""))
            {
                response = UserDatabaseAccess.NewUser(username); //sets the newly generated apikey for a new user to the response
                statusCode = HttpStatusCode.OK;
            }

            return Request.CreateResponse<string>(statusCode, response);

        }
    }
}