﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 明细
    /// </summary>
    public class MapDtlExt : EntityNoName
    {
        #region 导入导出属性.
        /// <summary>
        /// 用户访问.
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 是否可以导出
        /// </summary>
        public bool IsImp
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsImp);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsImp, value);
            }
        }
        /// <summary>
        /// 是否可以导入
        /// </summary>
        public bool IsExp
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsExp);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsExp, value);
            }
        }
        /// <summary>
        /// 查询sql
        /// </summary>
        public string ImpSQLInit
        {
            get
            {
                return this.GetValStringByKey(MapDtlAttr.ImpSQLInit).Replace("~", "'");
            }
            set
            {
                this.SetValByKey(MapDtlAttr.ImpSQLInit, value);
            }
        }
        /// <summary>
        /// 搜索sql
        /// </summary>
        public string ImpSQLSearch
        {
            get
            {
                return this.GetValStringByKey(MapDtlAttr.ImpSQLSearch).Replace("~", "'");
            }
            set
            {
                this.SetValByKey(MapDtlAttr.ImpSQLSearch, value);
            }
        }
        /// <summary>
        /// 填充数据一行数据
        /// </summary>
        public string ImpSQLFullOneRow
        {
            get
            {
                return this.GetValStringByKey(MapDtlAttr.ImpSQLFullOneRow).Replace("~", "'");
            }
            set
            {
                this.SetValByKey(MapDtlAttr.ImpSQLFullOneRow, value);
            }
        }
        #endregion

        #region 基本设置
        /// <summary>
        /// 工作模式
        /// </summary>
        public DtlModel DtlModel
        {
            get
            {
                return (DtlModel)this.GetValIntByKey(MapDtlAttr.Model);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.Model, (int)value);
            }
        }
        /// <summary>
        /// 是否启用行锁定.
        /// </summary>
        public bool IsRowLock
        {
            get
            {
                return this.GetParaBoolen(MapDtlAttr.IsRowLock, false);
            }
            set
            {
                this.SetPara(MapDtlAttr.IsRowLock, value);
            }
        }
        #endregion 基本设置

        #region 参数属性
        /// <summary>
        /// 记录增加模式
        /// </summary>
        public DtlAddRecModel DtlAddRecModel
        {
            get
            {
                return (DtlAddRecModel)this.GetParaInt(MapDtlAttr.DtlAddRecModel);
            }
            set
            {
                this.SetPara(MapDtlAttr.DtlAddRecModel, (int)value);
            }
        }
        /// <summary>
        /// 保存方式
        /// </summary>
        public DtlSaveModel DtlSaveModel
        {
            get
            {
                return (DtlSaveModel)this.GetParaInt(MapDtlAttr.DtlSaveModel);
            }
            set
            {
                this.SetPara(MapDtlAttr.DtlSaveModel, (int)value);
            }
        }
        /// <summary>
        /// 是否启用Link,在记录的右边.
        /// </summary>
        public bool IsEnableLink
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableLink, false);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableLink, value);
                this.SetPara(MapDtlAttr.IsEnableLink, value);
            }
        }
        public string LinkLabel
        {
            get
            {
                string s = this.GetValStrByKey(MapDtlAttr.LinkLabel);
                if (DataType.IsNullOrEmpty(s))
                    return "詳細";
                return s;
            }
            set
            {
                this.SetValByKey(MapDtlAttr.LinkLabel, value);
                this.SetPara(MapDtlAttr.LinkLabel, value);
            }
        }
        public string LinkUrl
        {
            get
            {
                string s = this.GetValStrByKey(MapDtlAttr.LinkUrl);
                if (DataType.IsNullOrEmpty(s))
                    return "http://ccport.org";

                s = s.Replace("*", "@");
                return s;
            }
            set
            {
                string val = value;
                val = val.Replace("@", "*");
                this.SetValByKey(MapDtlAttr.LinkUrl, val);
                this.SetPara(MapDtlAttr.LinkUrl, val);
            }
        }
        public string LinkTarget
        {
            get
            {
                string s = this.GetValStrByKey(MapDtlAttr.LinkTarget);
                if (DataType.IsNullOrEmpty(s))
                    return "_blank";
                return s;
            }
            set
            {
                this.SetValByKey(MapDtlAttr.LinkTarget, value);
                this.SetPara(MapDtlAttr.LinkTarget, value);
            }
        }
        /// <summary>
        /// 子线程处理人字段(用于分流节点的明细表分配子线程任务).
        /// </summary>
        public string SubThreadWorker
        {
            get
            {
                string s = this.GetParaString(MapDtlAttr.SubThreadWorker);
                if (DataType.IsNullOrEmpty(s))
                    return "";
                return s;
            }
            set
            {
                this.SetPara(MapDtlAttr.SubThreadWorker, value);
            }
        }
        /// <summary>
        /// 子线程分组字段(用于分流节点的明细表分配子线程任务)
        /// </summary>
        public string SubThreadGroupMark
        {
            get
            {
                string s = this.GetParaString(MapDtlAttr.SubThreadGroupMark);
                if (DataType.IsNullOrEmpty(s))
                    return "";
                return s;
            }
            set
            {
                this.SetPara(MapDtlAttr.SubThreadGroupMark, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(MapDtlAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.FK_Node, value);
            }
        }
        #endregion 参数属性

        #region 外键属性
        /// <summary>
        /// 框架
        /// </summary>
        public MapFrames MapFrames
        {
            get
            {
                MapFrames obj = this.GetRefObject("MapFrames") as MapFrames;
                if (obj == null)
                {
                    obj = new MapFrames(this.No);
                    this.SetRefObject("MapFrames", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 分组字段
        /// </summary>
        public GroupFields GroupFields_del
        {
            get
            {
                GroupFields obj = this.GetRefObject("GroupFields") as GroupFields;
                if (obj == null)
                {
                    obj = new GroupFields(this.No);
                    this.SetRefObject("GroupFields", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 逻辑扩展
        /// </summary>
        public MapExts MapExts
        {
            get
            {
                MapExts obj = this.GetRefObject("MapExts") as MapExts;
                if (obj == null)
                {
                    obj = new MapExts(this.No);
                    this.SetRefObject("MapExts", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 事件
        /// </summary>
        public FrmEvents FrmEvents
        {
            get
            {
                FrmEvents obj = this.GetRefObject("FrmEvents") as FrmEvents;
                if (obj == null)
                {
                    obj = new FrmEvents(this.No);
                    this.SetRefObject("FrmEvents", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 从表
        /// </summary>
        public MapDtls MapDtls
        {
            get
            {
                MapDtls obj = this.GetRefObject("MapDtls") as MapDtls;
                if (obj == null)
                {
                    obj = new MapDtls(this.No);
                    this.SetRefObject("MapDtls", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 超连接
        /// </summary>
        public FrmLinks FrmLinks
        {
            get
            {
                FrmLinks obj = this.GetRefObject("FrmLinks") as FrmLinks;
                if (obj == null)
                {
                    obj = new FrmLinks(this.No);
                    this.SetRefObject("FrmLinks", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 按钮
        /// </summary>
        public FrmBtns FrmBtns
        {
            get
            {
                FrmBtns obj = this.GetRefObject("FrmLinks") as FrmBtns;
                if (obj == null)
                {
                    obj = new FrmBtns(this.No);
                    this.SetRefObject("FrmBtns", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 元素
        /// </summary>
        public FrmEles FrmEles
        {
            get
            {
                FrmEles obj = this.GetRefObject("FrmEles") as FrmEles;
                if (obj == null)
                {
                    obj = new FrmEles(this.No);
                    this.SetRefObject("FrmEles", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 线
        /// </summary>
        public FrmLines FrmLines
        {
            get
            {
                FrmLines obj = this.GetRefObject("FrmLines") as FrmLines;
                if (obj == null)
                {
                    obj = new FrmLines(this.No);
                    this.SetRefObject("FrmLines", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        public FrmLabs FrmLabs
        {
            get
            {
                FrmLabs obj = this.GetRefObject("FrmLabs") as FrmLabs;
                if (obj == null)
                {
                    obj = new FrmLabs(this.No);
                    this.SetRefObject("FrmLabs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 图片
        /// </summary>
        public FrmImgs FrmImgs
        {
            get
            {
                FrmImgs obj = this.GetRefObject("FrmLabs") as FrmImgs;
                if (obj == null)
                {
                    obj = new FrmImgs(this.No);
                    this.SetRefObject("FrmLabs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 附件
        /// </summary>
        public FrmAttachments FrmAttachments
        {
            get
            {
                FrmAttachments obj = this.GetRefObject("FrmAttachments") as FrmAttachments;
                if (obj == null)
                {
                    obj = new FrmAttachments(this.No);
                    this.SetRefObject("FrmAttachments", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 图片附件
        /// </summary>
        public FrmImgAths FrmImgAths
        {
            get
            {
                FrmImgAths obj = this.GetRefObject("FrmImgAths") as FrmImgAths;
                if (obj == null)
                {
                    obj = new FrmImgAths(this.No);
                    this.SetRefObject("FrmImgAths", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 单选按钮
        /// </summary>
        public FrmRBs FrmRBs
        {
            get
            {
                FrmRBs obj = this.GetRefObject("FrmRBs") as FrmRBs;
                if (obj == null)
                {
                    obj = new FrmRBs(this.No);
                    this.SetRefObject("FrmRBs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public MapAttrs MapAttrs
        {
            get
            {
                MapAttrs obj = this.GetRefObject("MapAttrs") as MapAttrs;
                if (obj == null)
                {
                    obj = new MapAttrs(this.No);
                    this.SetRefObject("MapAttrs", obj);
                }
                return obj;
            }
        }
        #endregion

        #region 属性
        public GEDtls HisGEDtls_temp = null;
        /// <summary>
        /// 是否显示数量
        /// </summary>
        public bool IsShowSum
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsShowSum);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsShowSum, value);
            }
        }
        /// <summary>
        /// 是否显示Idx
        /// </summary>
        public bool IsShowIdx
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsShowIdx);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsShowIdx, value);
            }
        }
        /// <summary>
        /// 是否显示标题
        /// </summary>
        public bool IsShowTitle
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsShowTitle);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsShowTitle, value);
            }
        }
        /// <summary>
        /// 是否是合流汇总数据
        /// </summary>
        public bool IsHLDtl
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsHLDtl);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsHLDtl, value);
            }
        }
        /// <summary>
        /// 是否是分流
        /// </summary>
        public bool IsFLDtl
        {
            get
            {
                return this.GetParaBoolen(MapDtlAttr.IsFLDtl);
            }
            set
            {
                this.SetPara(MapDtlAttr.IsFLDtl, value);
            }
        }
        public int _IsReadonly = 2;
        public bool IsReadonly
        {
            get
            {
                if (_IsReadonly != 2)
                {
                    if (_IsReadonly == 1)
                        return true;
                    else
                        return false;
                }

                if (this.IsDelete || this.IsInsert || this.IsUpdate)
                {
                    _IsReadonly = 0;
                    return false;
                }
                _IsReadonly = 1;
                return true;
            }
        }
        /// <summary>
        /// 是否可以删除？
        /// </summary>
        public bool IsDelete
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsDelete);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsDelete, value);
            }
        }
        /// <summary>
        /// 是否可以新增？
        /// </summary>
        public bool IsInsert
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsInsert);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsInsert, value);
            }
        }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsView
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsView);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsView, value);
            }
        }
        /// <summary>
        /// 是否可以更新？
        /// </summary>
        public bool IsUpdate
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsUpdate);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsUpdate, value);
            }
        }
        /// <summary>
        /// 是否启用多附件
        /// </summary>
        public bool IsEnableAthM
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableAthM);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableAthM, value);
            }
        }
        /// <summary>
        /// 是否启用分组字段
        /// </summary>
        public bool IsEnableGroupField
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableGroupField);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableGroupField, value);
            }
        }
        /// <summary>
        /// 是否起用审核连接
        /// </summary>
        public bool IsEnablePass
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnablePass);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnablePass, value);
            }
        }

        /// <summary>
        /// 是否copy数据？
        /// </summary>
        public bool IsCopyNDData
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsCopyNDData);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsCopyNDData, value);
            }
        }
        /// <summary>
        /// 是否启用一对多
        /// </summary>
        public bool IsEnableM2M
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableM2M);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableM2M, value);
            }
        }
        /// <summary>
        /// 是否启用一对多多
        /// </summary>
        public bool IsEnableM2MM
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableM2MM);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableM2MM, value);
            }
        }

        public bool IsUse = false;
        /// <summary>
        /// 是否检查人员的权限
        /// </summary>
        public DtlOpenType DtlOpenType
        {
            get
            {
                return (DtlOpenType)this.GetValIntByKey(MapDtlAttr.DtlOpenType);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.DtlOpenType, (int)value);
            }
        }
        /// <summary>
        /// 分组字段
        /// </summary>
        public string GroupField
        {
            get
            {
                return this.GetValStrByKey(MapDtlAttr.GroupField);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.GroupField, value);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(MapDtlAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 事件类.
        /// </summary>
        public string FEBD
        {
            get
            {
                return this.GetValStrByKey(MapDtlAttr.FEBD);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.FEBD, value);
            }
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int RowsOfList
        {
            get
            {
                //如果不可以插入，就让其返回0.
                if (this.IsInsert == false)
                    return 0;

                return this.GetValIntByKey(MapDtlAttr.RowsOfList);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.RowsOfList, value);
            }
        }
        /// <summary>
        /// 物理表
        /// </summary>
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(MapDtlAttr.PTable);
                if (s == "" || s == null)
                {
                    s = this.No;
                    if (s.Substring(0, 1) == "0")
                    {
                        return "T" + this.No;
                    }
                    else
                        return s;
                }
                else
                {
                    if (s.Substring(0, 1) == "0")
                    {
                        return "T" + this.No;
                    }
                    else
                        return s;
                }
            }
            set
            {
                this.SetValByKey(MapDtlAttr.PTable, value);
            }
        }
        /// <summary>
        /// 多表头
        /// </summary>
        //public string MTR
        //{
        //    get
        //    {
        //        string s = this.GetValStrByKey(MapDtlAttr.MTR);
        //        s = s.Replace("《", "<");
        //        s = s.Replace("》", ">");
        //        s = s.Replace("‘", "'");
        //        return s;
        //    }
        //    set
        //    {
        //        string s = value;
        //        s = s.Replace("<", "《");
        //        s = s.Replace(">", "》");
        //        s = s.Replace("'", "‘");
        //        this.SetValByKey(MapDtlAttr.MTR, value);
        //    }
        //}
        #endregion

        #region 构造方法
        /// <summary>
        /// 明细
        /// </summary>
        public MapDtlExt()
        {
        }
        public MapDtlExt(string mypk)
        {
            this.No = mypk;
            this._IsReadonly = 2;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_MapDtl", "明細");

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = MapDtlAttr.FK_MapData;

                #region 基础信息.
                map.AddTBStringPK(MapDtlAttr.No, null, "ナンバリング", true, false, 1, 100, 20);
                map.AddTBString(MapDtlAttr.Name, null, "名前", true, false, 1, 200, 20);
                map.AddTBString(MapDtlAttr.Alias, null, "エイリアス", true, false, 0, 100, 20, false);
                map.SetHelperAlert(MapDtlAttr.Alias, "Excelフォームに効果的");

                map.AddTBString(MapDtlAttr.FK_MapData, null, "フォームID", true, true, 0, 100, 20);
                map.AddTBString(MapDtlAttr.PTable, null, "収納テーブル", true, false, 0, 200, 20);
                map.SetHelperAlert(MapDtlAttr.PTable, "デフォルトと番号は同じストレージテーブルです。");

                map.AddTBString(MapDtlAttr.FEBD, null, "イベントクラスエンティティクラス", true, true, 0, 100, 20, false);

                map.AddDDLSysEnum(MapDtlAttr.Model, 0, "動作モード", true, true, MapDtlAttr.Model, "@0=普通@1=固定線");
                map.AddDDLSysEnum(MapDtlAttr.DtlVer, 0, "バージョンを使用", true, true, MapDtlAttr.DtlVer, "@0=2017従来のバージョン@1=2019EasyUIバージョン");


                //map.AddTBString(MapDtlAttr.ImpFixTreeSql, null, "固定列树形SQL", true, false, 0, 500, 20);
                //map.AddTBString(MapDtlAttr.ImpFixDataSql, null, "固定列数据SQL", true, false, 0, 500, 20);

                //map.AddTBInt(MapDtlAttr.RowsOfList, 6, "初始化行数", true, false);
                //map.SetHelperAlert(MapDtlAttr.RowsOfList, "对第1个版本有效.");

                map.AddBoolean(MapDtlAttr.IsEnableGroupField, false, "グループ化フィールドを有効にするかどうか", true, true);

                map.AddBoolean(MapDtlAttr.IsShowSum, true, "合計を表示しますか？", true, true);
                map.AddBoolean(MapDtlAttr.IsShowIdx, true, "シリアル番号は表示されていますか？", true, true);

                map.AddBoolean(MapDtlAttr.IsReadonly, false, "読み取り専用ですか？", true, true);
                map.AddBoolean(MapDtlAttr.IsShowTitle, true, "タイトルを表示しますか？", true, true);
                map.AddBoolean(MapDtlAttr.IsView, true, "見えますか？", true, true);

                map.AddBoolean(MapDtlAttr.IsInsert, true, "行を挿入することは可能ですか？", true, true);
                map.AddBoolean(MapDtlAttr.IsDelete, true, "行を削除することは可能ですか？", true, true);
                map.AddBoolean(MapDtlAttr.IsUpdate, true, "更新できますか？", true, true);
                map.AddBoolean(MapDtlAttr.IsEnableAthM, false, "複数の添付ファイルを有効にするかどうか", true, true);
                map.AddDDLSysEnum(MapDtlAttr.WhenOverSize, 0, "行数を超えました", true, true, MapDtlAttr.WhenOverSize, "@0=処理しない@1=下に行を増やす@2=次のページを表示");

                // 为浙商银行设置从表打开.翻译.
                map.AddDDLSysEnum(MapDtlAttr.ListShowModel, 0, "リストデータ表示形式", true, true, MapDtlAttr.ListShowModel, "@0=表@1=カード");

                map.AddDDLSysEnum(MapDtlAttr.EditModel, 0, "データ編集モード", true, true, MapDtlAttr.EditModel, "@0=フォームモード@1=フールフォーム@2=フリーフォーム");
                map.SetHelperAlert(MapDtlAttr.EditModel, "フォーマットは次のとおりです。最初のタイプは新しい行を作成することであり、他のタイプが作成されるとカードがポップアップします。");

                //用于控制傻瓜表单.
                map.AddTBFloat(MapDtlAttr.H, 350, "高さ", true, false);
                map.SetHelperAlert(MapDtlAttr.H, "ダミーに効果的");

                //移动端数据显示方式
                map.AddDDLSysEnum(MapDtlAttr.MobileShowModel, 0, "モバイルデータ表示方法", true, true, MapDtlAttr.MobileShowModel, "@0=新しいページの表示モード@1=リストモード");
                map.AddTBString(MapDtlAttr.MobileShowField, null, "モバイルリスト表示欄", true, false, 0, 100, 20, false);

                //map.AddTBFloat(MapDtlAttr.X, 5, "距左", false, false);
                //map.AddTBFloat(MapDtlAttr.Y, 5, "距上", false, false);

                

                //  map.AddTBFloat(MapDtlAttr.W, 200, "宽度", true, false);

                //map.AddTBFloat(MapDtlAttr.FrmW, 900, "表单宽度", true, true);
                //map.AddTBFloat(MapDtlAttr.FrmH, 1200, "表单高度", true, true);

                //对显示的结果要做一定的限制.
                map.AddTBString(MapDtlAttr.FilterSQLExp, null, "データSQL式のフィルタリング", true, false, 0, 200, 20, true);
                map.SetHelperAlert(MapDtlAttr.FilterSQLExp, "形式は次のとおりです。WFState= 1でデータをフィルタリングするにはWFState = 1");

                //列自动计算表达式.
                //map.AddTBString(MapDtlAttr.ColAutoExp, null, "列自动计算", true, false, 0, 200, 20, true);
                //map.SetHelperAlert(MapDtlAttr.ColAutoExp, "格式为:@XiaoJi:Sum@NingLing:Avg 要对小计求合计,对年龄求平均数.不配置不显示.");

                //要显示的列.
                map.AddTBString(MapDtlAttr.ShowCols, null, "表示される列", true, false, 0, 500, 20, true);
                map.SetHelperAlert(MapDtlAttr.ShowCols, "デフォルトは空で、すべて表示されます。構成されている場合、構成に従って計算されます。形式は次のとおりです：field1、field2");

                map.AddTBString(MapDtlAttr.GUID, null, "GUID", false, false, 0, 128, 20);
                #endregion 基础信息.

                #region 导入导出填充.
                // 2014-07-17 for xinchang bank.
                map.AddBoolean(MapDtlAttr.IsExp, true, "輸入できますか？", true, true);

                //导入模式.
                map.AddDDLSysEnum(MapDtlAttr.ImpModel, 0, "インポート方法", true, true, MapDtlAttr.ImpModel,
                    "@0=インポートしないでください@1=構成モードに従ってインポート@2=xlsファイルテンプレートに従ってインポート");

                string strs = "xlsに従ってインポートされる場合は、テーブルID.xlsからテンプレートファイルを作成し、/DataUser/DtlTemplate/の下に配置してください。現在、xlsファイルのみがサポートされています。";
                map.SetHelperAlert(MapDtlAttr.ImpModel, strs);

                map.AddTBStringDoc(MapDtlAttr.ImpSQLInit, null, "SQLの初期化（テーブルの初期化時のSQLデータ、空にすることができます）", true, false, true);
                map.AddTBStringDoc(MapDtlAttr.ImpSQLSearch, null, "SQLクエリ（@KeyキーワードがSQLに含まれている必要があります。）", true, false, true);
                map.AddTBStringDoc(MapDtlAttr.ImpSQLFullOneRow, null, "データはデータSQLの行を埋めます（選択された主キーである@Keyキーワードを含める必要があります）", true, false, true);
                map.AddTBString(MapDtlAttr.ImpSQLNames, null, "列の中国名", true, false, 0, 900, 20, true);
                map.AddBoolean(MapDtlAttr.IsImp, false, "エクスポートできますか？", true, true);
                #endregion 导入导出填充.

                #region 多表头.
                //MTR 多表头列.
                //map.AddTBStringDoc(MapDtlAttr.MTR, null, "请书写html标记,以《TR》开头，以《/TR》结尾。", true, false, true);
                #endregion 多表头.

                #region 超链接.
                map.AddBoolean(MapDtlAttr.IsEnableLink, false, "ハイパーリンクを有効にするかどうか", true, true);
                map.AddTBString(MapDtlAttr.LinkLabel, "", "ハイパーリンクラベル", true, false, 0, 50, 100);
                map.AddTBString(MapDtlAttr.LinkTarget, null, "接続先", true, false, 0, 10, 100);
                map.AddTBString(MapDtlAttr.LinkUrl, null, "接続URL", true, false, 0, 200, 200, true);
                #endregion 超链接.

                #region 工作流相关.
                //add 2014-02-21.
                map.AddBoolean(MapDtlAttr.IsCopyNDData, true, "ノードデータのコピーを許可するかどうか", true, true);
                map.AddTBInt(MapDtlAttr.FK_Node, 0, "ノード（ユーザーに依存しないフォーム許可制御）", false, false);
                map.AddBoolean(MapDtlAttr.IsHLDtl, false, "合流の要約ですか", true, true);
                string sql = "SELECT KeyOfEn as No, Name FROM Sys_MapAttr WHERE FK_MapData='@No' AND  ( (MyDataType =1 and UIVisible=1 ) or (UIContralType=1))";
                map.AddDDLSQL(MapDtlAttr.SubThreadWorker, null, "子スレッドハンドラーフィールド", sql, true);
                map.AddBoolean(MapDtlAttr.IsEnablePass, false, "パス監査機能は有効ですか？", true, true);
                map.AddDDLSysEnum(MapDtlAttr.DtlOpenType, 1, "データオープンタイプ", true, true, MapDtlAttr.DtlOpenType, "@0=Operator@1=作業ID@2=フローID@3=親フローの作業ID");
                #endregion 工作流相关.

                #region 相关方法.
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "隠しフィールド"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".HidAttr";
                rm.Icon = "../Img/Setting.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "英語フィールドを生成する"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".GenerAttrs";
                rm.RefMethodType = RefMethodType.Func;
                rm.Warning = "";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "他のテーブル/ビューフィールドをインポートする"; // "设计表单";
                rm.Warning = "システムはインポート後に自動的に更新されません。手動で更新してください。";
                rm.ClassMethodName = this.ToString() + ".ImpFields";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "テーブルフィールドからインポートする"; // "设计表单";
                rm.Warning = "システムはインポート後に自動的に更新されません。手動で更新してください。";
                rm.ClassMethodName = this.ToString() + ".ImpFromDtlID";
                rm.HisAttrs.AddTBString("ID", null, "インポートするスレーブテーブルのIDを入力してください", true, false, 0, 100, 100);
                rm.RefMethodType = RefMethodType.Func;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "デザインばかフォーム"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DFoolFrm";
                rm.Icon = "../Img/Setting.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "フリーフォームをデザインする"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DFreeFrm";
                rm.Icon = "../Img/Setting.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "テーブルの添付ファイル属性"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".OpenAthAttr";
                rm.Icon = "../Img/AttachmentM.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion 相关方法.

                #region 实验中的功能.
                rm = new RefMethod();
                rm.GroupName = "実験の特徴";
                rm.Title = "列の自動計算"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".ColAutoExp";
                rm.Icon = "../Img/Setting.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "実験の特徴";
                rm.Title = "データのインポート"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DtlImp";
                rm.Icon = "../Img/Setting.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "実験の特徴";
                rm.Title = "データインポートv2019"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DtlImpV2019";
                rm.Icon = "../Img/Setting.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.GroupName = "実験の特徴";
                rm.Title = "事件"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = "../Img/Setting.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion 实验中的功能.

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override void afterInsertUpdateAction()
        {
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }
        /// <summary>
        /// 导入其他从表字段
        /// </summary>
        /// <returns></returns>
        public string ImpFromDtlID(string dtlId)
        {
            MapDtl dtl = new MapDtl();
            dtl.No = dtlId;
            if (dtl.RetrieveFromDBSources() == 0)
                return "err@" + dtlId + "入力エラー。";


            MapDtl dtlOfThis = new MapDtl(this.No);
            dtlOfThis.Copy(dtl);
            dtlOfThis.FK_MapData = this.FK_MapData;
            dtlOfThis.Update();


            //删除当前从表Attrs.
            MapAttrs attrs = new MapAttrs();
            attrs.Delete(MapAttrAttr.FK_MapData, this.No);

            //查询出来要导入的.
            attrs.Retrieve(MapAttrAttr.FK_MapData, dtlId);

            //执行字段导入.
            foreach (MapAttr item in attrs)
            {
                item.FK_MapData = this.No;
                item.Save();
            }



            //删除当前从表 exts .
            MapExts exts = new MapExts();
            exts.Delete(MapAttrAttr.FK_MapData, this.No);

            //查询出来要导入的.
            exts.Retrieve(MapAttrAttr.FK_MapData, dtlId);

            //执行字段导入.
            foreach (MapExt item in exts)
            {
                item.FK_MapData = this.No;
                item.Save();
            }

            return "正常にインポートされました。";
        }
        /// <summary>
        /// 打开从表附件属性.
        /// </summary>
        /// <returns></returns>
        public string OpenAthAttr()
        {
            string url = "../../Comm/RefFunc/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PKVal=" + this.No + "_AthMDtl";
            // string url = "../../Admin/FoolFormDesigner/DtlSetting/DtlImp.htm?FK_MapData=" + this.No + "&FromDtl=1&IsFirst=1&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
            return url;
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        public string DtlImp()
        {
            string url = "../../Admin/FoolFormDesigner/DtlSetting/DtlImp.htm?FK_MapData=" + this.No + "&FromDtl=1&IsFirst=1&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
           // string url = "../../Admin/FoolFormDesigner/DtlSetting/DtlImp/Default.htm?FK_MapDtl=" + this.No + "&FromDtl=1";
            return url;
        }
        /// <summary>
        /// 导入V2019
        /// </summary>
        /// <returns></returns>
        public string DtlImpV2019()
        {
            //string url = "../../Admin/FoolFormDesigner/DtlSetting/DtlImp.htm?FK_MapData=" + this.No + "&FromDtl=1&IsFirst=1&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
            string url = "../../Admin/FoolFormDesigner/DtlSetting/DtlImp/Default.htm?FK_MapDtl=" + this.No + "&FromDtl=1";
            return url;
        }
        /// <summary>
        /// 列自动计算
        /// </summary>
        /// <returns></returns>
        public string ColAutoExp()
        {
            string url = "../../Admin/FoolFormDesigner/DtlSetting/ColAutoExp.htm?FK_MapData=" + this.No;
            return url;
        }
        /// <summary>
        /// 导入其他表字段
        /// </summary>
        /// <returns></returns>
        public string ImpFields()
        {

            //  http://localhost:18272/WF/Admin/FoolFormDesigner/ImpTableField.htm?FK_MapData=CCFrm_CZBankBXDtl1&reset=true
            string url = "../../Admin/FoolFormDesigner/ImpTableField.htm?FK_MapData=" + this.No + "&FromDtl=1&IsFirst=1&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
            return url;
        }
        /// <summary>
        /// 设计傻瓜表单
        /// </summary>
        /// <returns></returns>
        public string DFoolFrm()
        {
            string url = "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&FromDtl=1&IsFirst=1&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
            return url;
        }

        /// <summary>
        /// 设计自由表单
        /// </summary>
        /// <returns></returns>
        public string DFreeFrm()
        {
            FrmLabs labs = new Sys.FrmLabs(this.No);
            if (labs.Count == 0)
            {
                //进行初始化控件位置及标签
                MapData md = new MapData();
                md.No = this.No;
                md.RetrieveFromDBSources();
                float maxEnd = 200;
                bool isLeft = true;

                #region 字段顺序调整
                MapAttrs mapAttrs = new Sys.MapAttrs(md.No);
                foreach (MapAttr attr in mapAttrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    //生成lab.
                    FrmLab lab = null;
                    if (isLeft == true)
                    {
                        maxEnd = maxEnd + 40;
                        /* 是否是左边 */
                        lab = new FrmLab();
                        lab.MyPK = BP.DA.DBAccess.GenerGUID();
                        lab.FK_MapData = attr.FK_MapData;
                        lab.Text = attr.Name;
                        lab.FontName = "Arial";
                        lab.X = 40;
                        lab.Y = maxEnd;
                        lab.Insert();

                        attr.X = lab.X + 80;
                        attr.Y = maxEnd;
                        attr.Update();
                    }
                    else
                    {
                        lab = new FrmLab();
                        lab.MyPK = BP.DA.DBAccess.GenerGUID();
                        lab.FK_MapData = attr.FK_MapData;
                        lab.Text = attr.Name;
                        lab.FontName = "Arial";
                        lab.X = 350;
                        lab.Y = maxEnd;
                        lab.Insert();

                        attr.X = lab.X + 80;
                        attr.Y = maxEnd;
                        attr.Update();
                    }
                    isLeft = !isLeft;
                }
                #endregion

                #region 明细表重置排序
                MapDtls mapDtls = new Sys.MapDtls(this.No);
                foreach (MapDtl dtl in mapDtls)
                {
                    maxEnd = maxEnd + 40;
                    /* 是否是左边 */
                    FrmLab lab = new FrmLab();
                    lab.MyPK = BP.DA.DBAccess.GenerGUID();
                    lab.FK_MapData = dtl.FK_MapData;
                    lab.Text = dtl.Name;
                    lab.FontName = "Arial";
                    lab.X = 40;
                    lab.Y = maxEnd;
                    lab.Insert();

                    dtl.X = lab.X + 80;
                    dtl.Y = maxEnd;
                    dtl.Update();
                    maxEnd = maxEnd + dtl.H;
                }
                #endregion

                md.ResetMaxMinXY();
            }
            string url = "../../Admin/CCFormDesigner/FormDesigner.htm?FK_MapData=" + this.No + "&FromDtl=1&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
            return url;
        }

        public string GenerAttrs()
        {
            string strs = "";
            MapAttrs attrs = new MapAttrs(this.No);
            foreach (MapAttr item in attrs)
            {
                strs += "\t\n " + item.KeyOfEn + ",";
            }
            return strs;
        }

        /// <summary>
        /// 高级设置
        /// </summary>
        /// <returns></returns>
        public string DoAction()
        {
            return "../../Admin/FoolFormDesigner/ActionForDtl.htm?DoType=Edit&FK_MapData=" + this.No + "&t=" + DataType.CurrentDataTime;
        }
        public string HidAttr()
        {
            return "../../Admin/FoolFormDesigner/HidAttr.htm?DoType=Edit&FK_MapData=" + this.No + "&t=" + DataType.CurrentDataTime;
        }

        #region 基本属性.
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.X);
            }
        }
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.Y);
            }
        }
        public float W
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.W);
            }
        }
        public float H
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.H);
            }
        }
        public float FrmW
        {
            get
            {
                return this.GetValFloatByKey(MapDtlAttr.FrmW);
            }
        }
        public float FrmH
        {
            get
            {
                return this.GetValFloatByKey(MapDtlAttr.FrmH);
            }
        }
        #endregion 基本属性.


        /// <summary>
        /// 初始化自定义字段属性
        /// </summary>
        /// <returns>返回执行结果</returns>
        public string InitAttrsOfSelf()
        {
            if (this.FK_Node == 0)
                return "err@セカンダリテーブルの属性はカスタム属性ではありません";

            if (this.No.Contains("_" + this.FK_Node) == false)
                return "err@セカンダリテーブルの属性はカスタム属性ではありません";

            //求从表ID.
            string refDtl = this.No.Replace("_" + this.FK_Node, "");

            //处理属性问题.
            MapAttrs attrs = new MapAttrs();
            attrs.Delete(MapAttrAttr.FK_MapData, this.No);
            attrs.Retrieve(MapAttrAttr.FK_MapData, refDtl);
            foreach (MapAttr attr in attrs)
            {
                attr.MyPK = this.No + "_" + attr.KeyOfEn;
                attr.FK_MapData = this.No;
                attr.Insert();
            }

            //处理mapExt 的问题.
            MapExts exts = new MapExts();
            exts.Delete(MapAttrAttr.FK_MapData, this.No);//先删除，后查询.
            exts.Retrieve(MapAttrAttr.FK_MapData, refDtl);
            MapExt mapExt = null;
            foreach (MapExt ext in exts)
            {
                mapExt = new MapExt();
                mapExt = ext;
                mapExt.MyPK = ext.MyPK + "_" + this.FK_Node;
                mapExt.FK_MapData = this.No;
                mapExt.Insert();
            }

            //处理附件问题
            /* 如果启用了多附件*/
            if (this.IsEnableAthM == true)
            {
                BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment();
                //获取原始附件的属性

                athDesc.MyPK = this.No + "_AthMDtl";
                if (athDesc.RetrieveFromDBSources() == 0)
                {
                    //获取原来附件的属性
                    BP.Sys.FrmAttachment oldAthDesc = new BP.Sys.FrmAttachment();
                    oldAthDesc.MyPK = refDtl + "_AthMDtl";
                    if (oldAthDesc.RetrieveFromDBSources() == 0)
                        return "元のスレーブテーブルのアタッチメント属性が存在しません。管理者に連絡してください";
                    athDesc = oldAthDesc;
                    athDesc.MyPK = this.No + "_AthMDtl";
                    athDesc.FK_MapData = this.No;
                    athDesc.NoOfObj = "AthMDtl";
                    athDesc.Name = this.Name;
                    athDesc.DirectInsert();
                    //增加分组
                    GroupField group = new GroupField();
                    group.Lab = athDesc.Name;
                    group.FrmID = this.FK_MapData;
                    group.CtrlType = "Ath";
                    group.CtrlID = athDesc.MyPK;
                    group.Idx = 10;
                    group.Insert();
                }

                //判断是否有隐藏的AthNum 字段
                MapAttr attr = new MapAttr();
                attr.MyPK = this.No + "_AthNum";
                int count = attr.RetrieveFromDBSources();
                if (count == 0)
                {
                    attr.FK_MapData = this.No;
                    attr.KeyOfEn = "AthNum";
                    attr.Name = "添付ファイルの数";
                    attr.DefVal = "0";
                    attr.UIContralType = UIContralType.TB;
                    attr.MyDataType = DataType.AppInt;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.DirectInsert();
                }


            }

            return "実行成功";
        }

        protected override bool beforeUpdate()
        {
            MapDtl dtl = new MapDtl(this.No);
            //启用审核
            dtl.IsEnablePass = this.IsEnablePass;
            //超链接
            dtl.IsEnableLink = this.IsEnableLink;
            dtl.LinkLabel = this.LinkLabel;
            dtl.LinkUrl = this.LinkUrl;
            dtl.LinkTarget = this.LinkTarget;
            dtl.Update();

            //判断是否启用多附件
            if (this.IsEnableAthM == true)
            {
                BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment();
                athDesc.MyPK = this.No + "_AthMDtl";
                if (athDesc.RetrieveFromDBSources() == 0)
                {
                    athDesc.FK_MapData = this.No;
                    athDesc.NoOfObj = "AthMDtl";
                    athDesc.Name = this.Name;
                    athDesc.DirectInsert();
                    //增加分组
                    GroupField group = new GroupField();
                    group.Lab = athDesc.Name;
                    group.FrmID = this.FK_MapData;
                    group.CtrlType = "Ath";
                    group.CtrlID = athDesc.MyPK;
                    group.Idx = 10;
                    group.Insert();
                }

                //判断是否有隐藏的AthNum 字段
                MapAttr attr = new MapAttr();
                attr.MyPK = this.No + "_AthNum";
                int count = attr.RetrieveFromDBSources();
                if (count == 0)
                {
                    attr.FK_MapData = this.No;
                    attr.KeyOfEn = "AthNum";
                    attr.Name = "添付ファイルの数";
                    attr.DefVal = "0";
                    attr.UIContralType = UIContralType.TB;
                    attr.MyDataType = DataType.AppInt;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.DirectInsert();
                   
                }
            }


            //获得事件实体.
            var febd = BP.Sys.Glo.GetFormDtlEventBaseByEnName(this.No);
            if (febd == null)
                this.FEBD = "";
            else
                this.FEBD = febd.ToString();


            #region 检查填充的SQL是否符合要求.
            #endregion

            //更新分组标签.  @fanleiwei. 代码有变化.
            BP.Sys.GroupField gf = new GroupField();
            int i = gf.Retrieve(GroupFieldAttr.CtrlType, "Dtl", GroupFieldAttr.CtrlID, this.No);
            if (i == 0 && this.FK_Node ==0)
            {
                gf.CtrlID = this.No;
                gf.CtrlType = "Dtl";
                gf.FrmID = this.FK_MapData;
                gf.Insert();
            }

            if (i > 1)
            {
                gf.Delete();
                i = gf.Retrieve(GroupFieldAttr.CtrlType, "Dtl", GroupFieldAttr.CtrlID, this.No);
            }

            if (i == 1 && gf.Lab.Equals(this.Name) == false)
            {
                gf.Lab = this.Name;
                gf.Update();
            }

            return base.beforeUpdate();
        }

        protected override void afterDelete()
        {
            MapDtl dtl = new MapDtl();
            dtl.No = this.No;
            dtl.Delete();

            //删除分组
            GroupFields gfs = new GroupFields();
            gfs.RetrieveByLike(GroupFieldAttr.CtrlID, this.No + "%");
            gfs.Delete();

            //如果启用了附件也需要删除
            if (this.IsEnableAthM == true)
            {
                FrmAttachment ath = new FrmAttachment();
                ath.Delete(FrmAttachmentAttr.MyPK, this.No + "_AthMDtl");
            }

            base.afterDelete();
        }
        /// <summary>
        /// 获取个数
        /// </summary>
        /// <param name="fk_val"></param>
        /// <returns></returns>
        public int GetCountByFK(int workID)
        {
            return BP.DA.DBAccess.RunSQLReturnValInt("select COUNT(OID) from " + this.PTable + " WHERE WorkID=" + workID);
        }
        public int GetCountByFK(string field, string val)
        {
            return BP.DA.DBAccess.RunSQLReturnValInt("select COUNT(OID) from " + this.PTable + " WHERE " + field + "='" + val + "'");
        }
        public int GetCountByFK(string field, Int64 val)
        {
            return BP.DA.DBAccess.RunSQLReturnValInt("select COUNT(OID) from " + this.PTable + " WHERE " + field + "=" + val);
        }
        public int GetCountByFK(string f1, Int64 val1, string f2, string val2)
        {
            return BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(OID) from " + this.PTable + " WHERE " + f1 + "=" + val1 + " AND " + f2 + "='" + val2 + "'");
        }
        #endregion
    }
    /// <summary>
    /// 明细s
    /// </summary>
    public class MapDtlExts : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 明细s
        /// </summary>
        public MapDtlExts()
        {
        }
        /// <summary>
        /// 明细s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public MapDtlExts(string fk_mapdata)
        {
            this.Retrieve(MapDtlAttr.FK_MapData, fk_mapdata, MapDtlAttr.FK_Node, 0, MapDtlAttr.No);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapDtlExt();
            }
        }

        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapDtlExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapDtlExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapDtlExt> Tolist()
        {
            System.Collections.Generic.List<MapDtlExt> list = new System.Collections.Generic.List<MapDtlExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapDtlExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
