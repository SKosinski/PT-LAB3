﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using PT_LAB3.Data;
using PT_LAB3.Models;

namespace PT_LAB3.Controllers
{
    [Route("api/[controller]")]
    // PODP6
    [EnableCors("CorsPolicy")]
    // PODP1
    // PODP4.2
    [Authorize]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public Models.Status Get()
        {
            var status = new Status()
            {
                Name = "OK"
            };

            return status;

        }
    }
}