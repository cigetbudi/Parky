﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb
{
    public static class SD
    {
        public static string APIBaseUrl = "https://localhost:44311/";
        public static string NationalParkAPIPath = APIBaseUrl + "api/v1/nationalparks";
        public static string TrailPath = APIBaseUrl + "api/v1/trails";
    }
}