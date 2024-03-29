﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_Port : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_Port()
        {
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "実行成功。";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@マーク[" + this.DoType + "]、見つかりません。@RowURL:" +HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.

        #region OrderOfDept 部门顺序调整 .
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string OrderOfDept_Init()
        {
            string sql = "SELECT No,Name,ParentNo,Idx FROM Port_Dept";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            return "";
        }
        #endregion xxx 界面方法.

        /// <summary>
        /// 获得流程树
        /// </summary>
        /// <returns></returns>
        public string Default_GenerFlowTree()
        {
            WF_Admin_CCBPMDesigner en = new WF_Admin_CCBPMDesigner();
            return en.GetFlowTreeTable2019();
        }

    }
}
