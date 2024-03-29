﻿using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// 授权属性
    /// </summary>
    public class AuthAttr
    {
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string MyPK = "MyPK";
        /// <summary>
        /// 类型(0=全部流程1=指定流程)
        /// </summary>
        public const string AuthType = "AuthType";
        /// <summary>
        /// 授权人
        /// </summary>
        public const string Auther = "Auther";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FlowNo = "FlowNo";
        /// <summary>
        /// 流程名称
        /// </summary>
        public const string FlowName = "FlowName";
        /// <summary>
        /// 取回日期
        /// </summary>
        public const string TakeBackDT = "TakeBackDT";
        /// <summary>
        /// 人员编号.
        /// </summary>
        public const string EmpNo = "EmpNo";
        /// <summary>
        /// 人员名称
        /// </summary>
        public const string EmpName = "EmpName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        
    }
    /// <summary>
    /// 授权
    /// </summary>
    public class Auth : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(AuthAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(AuthAttr.FlowNo, value);
            }
        }
        /// <summary>
        /// 取回日期
        /// </summary>
        public string TakeBackDT
        {
            get
            {
                return this.GetValStringByKey(AuthAttr.TakeBackDT);
            }
            set
            {
                this.SetValByKey(AuthAttr.TakeBackDT, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 授权
        /// </summary>
        public Auth()
        {
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Auth", "認可");

                map.AddMyPK();

                map.AddTBString(AuthAttr.Auther, null, "承認者", true, false, 0, 100, 10);
                map.AddTBInt(AuthAttr.AuthType, 0, "タイプ（0=すべてのフロー1=指定されたフロー）", true, false);

                map.AddTBString(AuthAttr.EmpNo, null, "職員番号への委任", true, false, 0, 100, 10);
                map.AddTBString(AuthAttr.EmpName, null, "委任された人の名前", true, false, 0, 100, 10);

                map.AddTBString(AuthAttr.FlowNo, null, "フロー番号", true, false, 0, 100, 10);
                map.AddTBString(AuthAttr.FlowName, null, "フロー名", true, false, 0, 100, 10);

                map.AddTBDate(AuthAttr.TakeBackDT, null, "取得日", true, false);
                map.AddTBDate(AuthAttr.RDT, null, "記録日", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.MyPK = BP.DA.DBAccess.GenerGUID();
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 授权
    /// </summary>
    public class Auths : EntitiesMyPK
    {
        /// <summary>
        /// 授权
        /// </summary>
        public Auths() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Auth();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Auth> ToJavaList()
        {
            return (System.Collections.Generic.IList<Auth>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Auth> Tolist()
        {
            System.Collections.Generic.List<Auth> list = new System.Collections.Generic.List<Auth>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Auth)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
