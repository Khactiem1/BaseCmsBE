using CMS.Core.Database;
using System;
using System.Collections.Generic;

namespace Cms.DL
{
    /// <summary>
    /// Base DL tầng DL
    /// </summary>
    /// @author nktiem 24/11/2024
    public class BaseDL<T> : IBaseDL<T>
    {
        #region Field

        private IDatabaseService _databaseService;

        #endregion

        #region Contructor

        public BaseDL(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        #endregion

        #region Method

        #endregion
    }
}
