﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Model.Enum
{
    /// <summary>
    /// Enum state save record
    /// </summary>
    public enum EntityState : int
    {
        /// <summary>
        /// Không làm gì cả
        /// </summary>
        None = 0,

        /// <summary>
        /// Thêm
        /// </summary>
        Insert = 1,

        /// <summary>
        /// Sửa
        /// </summary>
        Update = 2,

        /// <summary>
        /// Xoá
        /// </summary>
        Delete = 3,

        /// <summary>
        /// Xoá
        /// </summary>
        Inactive = 4,
    }
}