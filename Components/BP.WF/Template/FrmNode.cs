﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点表单属性	  
    /// </summary>
    public class FrmNodeAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Frm = "FK_Frm";
        /// <summary>
        /// 工作节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 是否readonly.
        /// </summary>
        public const string IsEdit = "IsEdit";
        /// <summary>
        /// IsPrint
        /// </summary>
        public const string IsPrint = "IsPrint";
        /// <summary>
        /// 是否启用装载填充事件.
        /// </summary>
        public const string IsEnableLoadData = "IsEnableLoadData";
        /// <summary>
        /// 打开表单时是否覆盖其他节点页面？ 
        /// </summary>
        public const string IsCloseEtcFrm = "IsCloseEtcFrm";
        /// <summary>
        /// 是否1变N(对于分流节点有效)
        /// </summary>
        public const string Is1ToN = "Is1ToN";
        /// <summary>
        /// 是否默认打开
        /// </summary>
        public const string IsDefaultOpen = "IsDefaultOpen";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// FK_Flow
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 表单类型
        /// </summary>
        public const string FrmType = "FrmType";
        /// <summary>
        /// 方案
        /// </summary>
        public const string FrmSln = "FrmSln";
        /// <summary>
        /// 是否启用审核组件
        /// </summary>
        public const string IsEnableFWC = "IsEnableFWC";
        /// <summary>
        /// 谁是主键？
        /// </summary>
        public const string WhoIsPK = "WhoIsPK";
        /// <summary>
        /// 模版文件
        /// </summary>
        public const string TempleteFile = "TempleteFile";
        /// <summary>
        /// 是否显示
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 关键字段
        /// </summary>
        public const string GuanJianZiDuan = "GuanJianZiDuan";
        /// <summary>
        /// 汇总
        /// </summary>
        public const string HuiZong = "HuiZong";
        /// <summary>
        /// 表单启用规则
        /// </summary>
        public const string FrmEnableRole = "FrmEnableRole";
        /// <summary>
        ///  表单启动表达式.
        /// </summary>
        public const string FrmEnableExp = "FrmEnableExp";
        /// <summary>
        /// 表单显示的名字
        /// </summary>
        public const string FrmNameShow = "FrmNameShow";
    }
    /// <summary>
    /// 谁是主键？
    /// </summary>
    public enum WhoIsPK
    {
        /// <summary>
        /// 流程ID是主键 
        /// </summary>
        OID,
        /// <summary>
        ///  FID是主键(干流程的WorkID)
        /// </summary>
        FID,
        /// <summary>
        /// 父流程ID是主键
        /// </summary>
        PWorkID,
        /// <summary>
        /// 延续流程ID是主键
        /// </summary>
        CWorkID,
        /// <summary>
        /// 爷爷流程ID是主键
        /// </summary>
        PPWorkID,
        /// <summary>
        /// 太爷爷流程ID是主键
        /// </summary>
        PPPWorkID

    }
    /// <summary>
    /// 方案类型
    /// </summary>
    public enum FrmSln
    {
        /// <summary>
        /// 默认方案
        /// </summary>
        Default,
        /// <summary>
        /// 只读方案
        /// </summary>
        Readonly,
        /// <summary>
        /// 自定义方案
        /// </summary>
        Self
    }

    /// <summary>
    /// 表单启用规则
    /// </summary>
    public enum FrmEnableRole
    {
        /// <summary>
        /// 始终启用
        /// </summary>
        Allways,
        /// <summary>
        /// 有数据时启用
        /// </summary>
        WhenHaveData,
        /// <summary>
        /// 有参数时启用
        /// </summary>
        WhenHaveFrmPara,
        /// <summary>
        /// 按表单的字段表达式
        /// </summary>
        ByFrmFields,
        /// <summary>
        /// 按SQL表达式
        /// </summary>
        BySQL,
        /// <summary>
        /// 不启用
        /// </summary>
        Disable,
        /// <summary>
        /// 按岗位
        /// </summary>
        ByStation,
        /// <summary>
        /// 按部门
        /// </summary>
        ByDept
    }
    /// <summary>
    /// 节点表单
    /// 节点的工作节点有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class FrmNode : EntityMyPK
    {
        #region 关于节点与office表单的toolbar权限控制方案.

        #endregion 关于节点与office表单的toolbar权限控制方案.

        #region 基本属性
        public string FrmUrl
        {
            get
            {
                switch (this.HisFrmType)
                {
                    case FrmType.FoolForm:
                        return Glo.CCFlowAppPath + "/WF/CCForm/FrmFix";
                    case FrmType.FreeFrm:
                        return Glo.CCFlowAppPath + "/WF/CCForm/Frm";
                    default:
                        throw new Exception("err,未処理。");
                }
            }
        }
        private Frm _hisFrm = null;
        public Frm HisFrm
        {
            get
            {
                if (this._hisFrm == null)
                {
                    this._hisFrm = new Frm(this.FK_Frm);
                    this._hisFrm.HisFrmNode = this;
                }
                return this._hisFrm;
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// 表单类型
        /// </summary>
        public BP.Sys.FrmType HisFrmType
        {
            get
            {
                return (BP.Sys.FrmType)this.GetValIntByKey(FrmNodeAttr.FrmType);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FrmType, (int)value);
            }
        }
        /// <summary>
        /// 表单类型
        /// </summary>
        public string HisFrmTypeText
        {
            get
            {
                SysEnum se = new SysEnum(FrmNodeAttr.FrmType, (int)this.HisFrmType);
                return se.Lab;
                // return (BP.Sys.FrmType)this.GetValIntByKey(FrmNodeAttr.FrmType);
            }
        }
        /// <summary>
        /// 是否启用装载填充事件
        /// </summary>
        public bool IsEnableLoadData
        {
            get
            {
                return this.GetValBooleanByKey(FrmNodeAttr.IsEnableLoadData);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.IsEnableLoadData, value);
            }
        }
        /// <summary>
        /// 是否启用节点组件?
        /// </summary>
        public bool IsEnableFWC
        {
            get
            {
                return this.GetValBooleanByKey(FrmNodeAttr.IsEnableFWC);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.IsEnableFWC, value);
            }
        }
        /// <summary>
        /// 是否执行1变n
        /// </summary>
        public bool Is1ToN
        {
            get
            {
                return this.GetValBooleanByKey(FrmNodeAttr.Is1ToN);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.Is1ToN, value);
            }
        }
        /// <summary>
        /// 是否默认打开
        /// </summary>
        public bool IsDefaultOpen
        {
            get
            {
                return this.GetValBooleanByKey(FrmNodeAttr.IsDefaultOpen);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.IsDefaultOpen, value);
            }
        }
        /// <summary>
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.Idx);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.Idx, value);
            }
        }
        /// <summary>
        /// 谁是主键？
        /// </summary>
        public WhoIsPK WhoIsPK
        {
            get
            {
                return (WhoIsPK)this.GetValIntByKey(FrmNodeAttr.WhoIsPK);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.WhoIsPK, (int)value);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_Frm
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.FK_Frm);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FK_Frm, value);
            }
        }
        /// <summary>
        /// 模版文件
        /// </summary>
        public string TempleteFile
        {
            get
            {
                string str = this.GetValStringByKey(FrmNodeAttr.TempleteFile);
                if (DataType.IsNullOrEmpty(str))
                    return this.FK_Frm + ".xls";
                return str;
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.TempleteFile, value);
            }
        }
        /// <summary>
        /// 是否显示
        /// </summary>
        public string IsEnable
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.IsEnable, value);
            }
        }
        /// <summary>
        /// 关键字段
        /// </summary>
        public string GuanJianZiDuan
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.GuanJianZiDuan);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.GuanJianZiDuan, value);
            }
        }
        /// <summary>
        /// 对应的解决方案
        /// 0=默认方案.节点编号 1=自定义方案, 1=不可编辑.
        /// </summary>
        public FrmSln FrmSln
        {
            get
            {
                if (this.GetValIntByKey(FrmNodeAttr.FrmSln) > 5)
                    return Template.FrmSln.Self;

                return (FrmSln)this.GetValIntByKey(FrmNodeAttr.FrmSln);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FrmSln, (int)value);
            }
        }
        /// <summary>
        /// 启用规则
        /// </summary>
        public int FrmEnableRoleInt
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.FrmEnableRole);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FrmEnableRole, value);
            }
        }
        /// <summary>
        /// 表单启用规则
        /// </summary>
        public FrmEnableRole FrmEnableRole
        {
            get
            {
                return (FrmEnableRole)this.GetValIntByKey(FrmNodeAttr.FrmEnableRole);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FrmEnableRole, (int)value);
            }
        }
        /// <summary>
        /// 启用规则.
        /// </summary>
        public string FrmEnableRoleText
        {
            get
            {
                if (this.FrmEnableRole == Template.FrmEnableRole.WhenHaveFrmPara && this.FK_Frm == "ND" + this.FK_Node)
                    return "無効";

                SysEnum se = new SysEnum(FrmNodeAttr.FrmEnableRole, this.FrmEnableRoleInt);
                return se.Lab;
            }
        }
        /// <summary>
        /// 表单启动表达式
        /// </summary>
        public string FrmEnableExp
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.FrmEnableExp);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FrmEnableExp, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 是否可以编辑？
        /// </summary>
        public bool IsEdit
        {
            get
            {
                if (this.FrmSln == Template.FrmSln.Readonly)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 是否可以编辑？
        /// </summary>
        public int IsEditInt
        {
            get
            {
                if (this.IsEdit)
                    return 1;
                return 0;
            }
        }
        /// <summary>
        /// 是否可以打印
        /// </summary>
        public bool IsPrint
        {
            get
            {
                return this.GetValBooleanByKey(FrmNodeAttr.IsPrint);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.IsPrint, value);
            }
        }
        /// <summary>
        /// 是否可以打印
        /// </summary>
        public int IsPrintInt
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.IsPrint);
            }
        }
        /// <summary>
        /// 汇总
        /// </summary>
        public string HuiZong
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.HuiZong);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.HuiZong, value);
            }
        }
        /// <summary>
        ///打开时是否关闭其它的页面？
        /// </summary>
        public bool IsCloseEtcFrm
        {
            get
            {
                return this.GetValBooleanByKey(FrmNodeAttr.IsCloseEtcFrm);
            }
        }
        public int IsCloseEtcFrmInt
        {
            get
            {
                if (this.IsCloseEtcFrm)
                    return 1;
                return 0;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点表单
        /// </summary>
        public FrmNode() { }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="mypk"></param>
        public FrmNode(string mypk)
            : base(mypk)
        {
        }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="fk_node">节点</param>
        /// <param name="fk_frm">表单</param>
        public FrmNode(string fk_flow, int fk_node, string fk_frm)
        {
            int i = this.Retrieve(FrmNodeAttr.FK_Node, fk_node, FrmNodeAttr.FK_Frm, fk_frm);
           
            if (i == 0)
            {
                this.IsPrint = false;
                //不可以编辑.
                this.FrmSln = Template.FrmSln.Default;
                // this.IsEdit = false;
                Node node = new Node(fk_node);
                if (node.FrmWorkCheckSta != FrmWorkCheckSta.Disable)
                {
                    this.IsEnableFWC = true;

                }
                return;
               // throw new Exception("@表单关联信息已被删除。");
            }
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

                Map map = new Map("WF_FrmNode", "ノードフォーム");

                map.AddMyPK();

                map.AddTBString(FrmNodeAttr.FK_Frm, null, "フォームID", true, true, 1, 200, 200);
                map.AddTBInt(FrmNodeAttr.FK_Node, 0, "ノード番号", true, false);
                map.AddTBString(FrmNodeAttr.FK_Flow, null, "フロー番号", true, true, 1, 20, 20);
                map.AddTBString(FrmNodeAttr.FrmType, "0", "フォームタイプ", true, true, 1, 20, 20);

                //菜单在本节点的权限控制.
                // map.AddTBInt(FrmNodeAttr.IsEdit, 1, "是否可以更新", true, false);
                map.AddTBInt(FrmNodeAttr.IsPrint, 0, "印刷できます", true, false);
                map.AddTBInt(FrmNodeAttr.IsEnableLoadData, 0, "イベントの読み込みと入力を有効にするかどうか", true, false);
                map.AddTBInt(FrmNodeAttr.IsDefaultOpen, 0, "デフォルトで開くかどうか", true, false);
                map.AddTBInt(FrmNodeAttr.IsCloseEtcFrm, 0, "他のページを開いたときに閉じますか？", true, false);
                map.AddTBInt(FrmNodeAttr.IsEnableFWC, 0, "監査コンポーネントは有効になっていますか？", true, false);

                //显示的
                map.AddTBInt(FrmNodeAttr.Idx, 0, "シーケンス番号", true, false);
                map.AddTBInt(FrmNodeAttr.FrmSln, 0, "フォーム制御スキーム", true, false);

                // add 2014-01-26
                map.AddTBInt(FrmNodeAttr.WhoIsPK, 0, "主キーは誰ですか？", true, false);

                //add 2016.3.25.
                map.AddTBInt(FrmNodeAttr.Is1ToN, 0, "1はNに変わりますか？", true, false);
                map.AddTBString(FrmNodeAttr.HuiZong, null, "子スレッドによって要約されるデータテーブル", true, true, 0, 300, 20);
                map.AddTBInt(FrmNodeAttr.FrmEnableRole, 0, "フォームアクティベーションルール", true, false);
                map.AddTBString(FrmNodeAttr.FrmEnableExp, null, "有効な式", true, true, 0, 900, 20);


                //模版文件，对于office表单有效.
                map.AddTBString(FrmNodeAttr.TempleteFile, null, "テンプレートファイル", true, true, 0, 500, 20);

                //是否显示
                map.AddTBInt(FrmNodeAttr.IsEnable, 1, "表示するかどうか", true, false);

                map.AddTBString(FrmNodeAttr.GuanJianZiDuan, null, "キーフィールド", true, true, 0, 20, 20);

                //@2019.09.30 by zhoupeng.
                map.AddTBString(FrmNodeAttr.FrmNameShow, null, "フォーム表示名", true, false, 0, 100, 20);
              //  map.SetHelperAlert(FrmNodeAttr.FrmNameShow, "显示在表单树上的名字,默认为空,表示与表单的实际名字相同.多用于节点表单的名字在表单树上显示.");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 方法.
        public void DoUp()
        {
            this.DoOrderUp(FrmNodeAttr.FK_Node, this.FK_Node.ToString(), FrmNodeAttr.Idx);
        }
        public void DoDown()
        {
            this.DoOrderDown(FrmNodeAttr.FK_Node, this.FK_Node.ToString(), FrmNodeAttr.Idx);
        }
        protected override bool beforeUpdateInsertAction()
        {
            if (this.FK_Frm.Length == 0)
                throw new Exception("@フォーム番号が空です");

            if (this.FK_Node == 0)
                throw new Exception("@ノードIDが空です");

            if (this.FK_Flow.Length == 0)
                throw new Exception("@フローIDが空です");

            this.MyPK = this.FK_Frm + "_" + this.FK_Node + "_" + this.FK_Flow;

            //获取表单的类型
            MapData mapData = new MapData();
            mapData.No = this.FK_Frm;
            if (mapData.RetrieveFromDBSources() == 1)
                this.HisFrmType = mapData.HisFrmType;
            else
                this.HisFrmType = FrmType.FoolForm;

            return base.beforeUpdateInsertAction();
        }
        #endregion 方法.

    }
    /// <summary>
    /// 节点表单s
    /// </summary>
    public class FrmNodes : EntitiesMyPK
    {
        #region 属性.
        /// <summary>
        /// 他的工作节点
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (FrmNode ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Node));
                }
                return ens;
            }
        }
        #endregion 属性.

        #region 构造方法..
        /// <summary>
        /// 节点表单
        /// </summary>
        public FrmNodes() { }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public FrmNodes(string fk_flow, int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmNodeAttr.FK_Flow, fk_flow);
            qo.addAnd();
            qo.AddWhere(FrmNodeAttr.FK_Node, nodeID);

            qo.addOrderBy(FrmNodeAttr.Idx);
            qo.DoQuery();
        }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="NodeNo">NodeNo </param>
        public FrmNodes(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmNodeAttr.FK_Node, nodeID);
            qo.addOrderBy(FrmNodeAttr.Idx);
            qo.DoQuery();
        }
        #endregion 构造方法..

        #region 公共方法.
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmNode();
            }
        }
        /// <summary>
        /// 节点表单s
        /// </summary>
        /// <param name="sts">节点表单</param>
        /// <returns></returns>
        public Nodes GetHisNodes(Nodes sts)
        {
            Nodes nds = new Nodes();
            Nodes tmp = new Nodes();
            foreach (Node st in sts)
            {
                tmp = this.GetHisNodes(st.No);
                foreach (Node nd in tmp)
                {
                    if (nds.Contains(nd))
                        continue;
                    nds.AddEntity(nd);
                }
            }
            return nds;
        }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="NodeNo">工作节点编号</param>
        /// <returns>节点s</returns>
        public Nodes GetHisNodes(string NodeNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmNodeAttr.FK_Node, NodeNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (FrmNode en in this)
            {
                ens.AddEntity(new Node(en.FK_Frm));
            }
            return ens;
        }
        /// <summary>
        /// 转向此节点的集合的Nodes
        /// </summary>
        /// <param name="nodeID">此节点的ID</param>
        /// <returns>转向此节点的集合的Nodes (FromNodes)</returns> 
        public Nodes GetHisNodes(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmNodeAttr.FK_Frm, nodeID);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (FrmNode en in this)
            {
                ens.AddEntity(new Node(en.FK_Node));
            }
            return ens;
        }
        #endregion 公共方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmNode> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmNode>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmNode> Tolist()
        {
            System.Collections.Generic.List<FrmNode> list = new System.Collections.Generic.List<FrmNode>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmNode)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
