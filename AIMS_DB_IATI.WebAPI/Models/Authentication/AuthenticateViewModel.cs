﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AIMS_BD_IATI.WebAPI.Models.Authentication
{
    public class AuthenticateViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}