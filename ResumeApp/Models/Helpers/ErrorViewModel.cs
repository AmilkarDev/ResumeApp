﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResumeApp.Models.Helpers
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}