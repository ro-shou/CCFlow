﻿using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程类别属性
    /// </summary>
    public class FlowSortAttr : EntityTreeAttr
    {
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 域/系统编号
        /// </summary>
        public const string Domain = "Domain";
    }
    /// <summary>
    ///  流程类别
    /// </summary>
    public class FlowSort : EntityTree
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(FlowSortAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(FlowSortAttr.OrgNo, value);
            }
        }
        public string Domain
        {
            get
            {
                return this.GetValStrByKey(FlowSortAttr.Domain);
            }
            set
            {
                this.SetValByKey(FlowSortAttr.Domain, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 流程类别
        /// </summary>
        public FlowSort()
        {
        }
        /// <summary>
        /// 流程类别
        /// </summary>
        /// <param name="_No"></param>
        public FlowSort(string _No) : base(_No) { }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        /// <summary>
        /// 流程类别Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_FlowSort", "フローカテゴリ");

                map.AddTBStringPK(FlowSortAttr.No, null, "ナンバリング", true, true, 1, 100, 20);
                map.AddTBString(FlowSortAttr.ParentNo, null, "親ノードNo", true, true, 0, 100, 30);
                map.AddTBString(FlowSortAttr.Name, null, "名前", true, false, 0, 200, 30,true);
                map.AddTBString(FlowSortAttr.OrgNo, "0", "組織番号（0はシステム組織）", true, false, 0, 150, 30);
                map.SetHelperAlert(FlowSortAttr.OrgNo, "異なる組織を区別するために使用されるフロー：グループには複数の子会社があり、各子会社には独自のビジネスフローがある");

                map.AddTBString(FlowSortAttr.Domain, null, "ドメイン/システム番号", true, false, 0, 100, 30);
                map.SetHelperAlert(FlowSortAttr.Domain, "異なるシステムのフローを区別するために使用されます。たとえば、グループには複数のサブシステムがあり、各サブシステムには独自のフローがあるため、これらのフローをサブシステムとしてマークする必要があります。");
                map.AddTBInt(FlowSortAttr.Idx, 0, "Idx", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeUpdateInsertAction()
        {
            string sql = "UPDATE WF_GenerWorkFlow SET Domain='" + this.Domain + "' WHERE FK_FlowSort='" + this.No + "'";
            DBAccess.RunSQL(sql);

            //@sly
            sql = "UPDATE WF_Emp SET StartFlows='' ";
            DBAccess.RunSQL(sql);

            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 流程类别
    /// </summary>
    public class FlowSorts : EntitiesTree
    {
        /// <summary>
        /// 流程类别s
        /// </summary>
        public FlowSorts() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowSort();
            }
        }
        public override int RetrieveAll()
        {
            int i = base.RetrieveAll( FlowSortAttr.Idx );
            if (i == 0)
            {
                FlowSort fs = new FlowSort();
                fs.Name = "公式文書";
                fs.No = "01";
                fs.Insert();

                fs = new FlowSort();
                fs.Name = "オフィス";
                fs.No = "02";
                fs.Insert();
                i = base.RetrieveAll();
            }

            return i;
        }


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowSort> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowSort>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowSort> Tolist()
        {
            System.Collections.Generic.List<FlowSort> list = new System.Collections.Generic.List<FlowSort>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowSort)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
