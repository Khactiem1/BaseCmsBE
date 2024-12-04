using Cms.Model;

namespace Cms.BL
{
    /// <summary>
    /// interface BL với layout
    /// </summary>
    /// @author nktiem 24/11/2024
    public interface ILayoutBL : IBaseBL<Layout>
    {
        /// <summary>
        /// Lấy cấu hình layout theo Tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Task<Layout> GetLayoutByTag(string tag);

        /// <summary>
        /// Cập nhật cấu hình layout theo Tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Task UpdateLayoutByTag(string tag, Layout layout);
    }
}
