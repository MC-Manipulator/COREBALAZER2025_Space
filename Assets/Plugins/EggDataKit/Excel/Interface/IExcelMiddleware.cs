#region

//文件创建者：Egg
//创建时间：01-23 03:02

#endregion

namespace EggFramework.Util.Excel
{
    public interface IExcelMiddleware<in T> where T : IExcelEntity
    {
        public void Process(T data);
    }
}