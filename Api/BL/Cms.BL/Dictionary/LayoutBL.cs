using Cms.Core.Common;
using Cms.DL;
using Cms.Model;
using CMS.Core.Database;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Cms.BL
{
    /// <summary>
    /// BL với layout
    /// </summary>
    /// @author nktiem 24/11/2024
    public class LayoutBL : BaseBL<Layout>, ILayoutBL
    {
        #region Field

        private ILayoutDL _layoutDL;
        private IDatabaseService _databaseService;

        #endregion

        #region Contructor

        public LayoutBL(ILayoutDL layoutDL, IDatabaseService databaseService) : base(layoutDL, databaseService)
        {
            _layoutDL = layoutDL;
            _databaseService = databaseService;
        }

        #endregion

        /// <summary>
        /// Lấy cấu hình layout theo Tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task<Layout> GetLayoutByTag(string tag)
        {
            var sql = "select * from layout where layout_tag = @v_layout_tag limit 1";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("v_layout_tag", tag);
            var result = await _databaseService.QueryUsingCommandText<Layout>(sql, param);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Cập nhật cấu hình layout theo Tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task UpdateLayoutByTag(string tag, Layout layout)
        {
            var sql = "Update layout set config = CAST(@v_config AS JSON) where layout_tag = @v_layout_tag";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("v_layout_tag", tag);
            param.Add("v_config", layout.config);
            await _databaseService.ExecuteScalarUsingCommandText<int>(sql, param);
        }
    }
}