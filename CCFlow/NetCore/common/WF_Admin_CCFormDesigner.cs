﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.WF.Template;
using BP.Frm;

namespace BP.WF.HttpHandler
{
    public class WF_Admin_CCFormDesigner : BP.WF.HttpHandler.DirectoryPageBase
    {

        #region 执行父类的重写方法.
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_CCFormDesigner()
        {
        }

        /// <summary>
        /// 创建枚举类型字段
        /// </summary>
        /// <returns></returns>
        public string FrmEnumeration_NewEnumField()
        {
            UIContralType ctrl = UIContralType.RadioBtn;
            string ctrlDoType = GetRequestVal("ctrlDoType");
            if (ctrlDoType == "DDL")
                ctrl = UIContralType.DDL;
            else
                ctrl = UIContralType.RadioBtn;

            string fk_mapdata = this.GetRequestVal("FK_MapData");
            string keyOfEn = this.GetRequestVal("KeyOfEn");
            string fieldDesc = this.GetRequestVal("Name");
            string enumKeyOfBind = this.GetRequestVal("UIBindKey"); //要绑定的enumKey.
            float x = this.GetRequestValFloat("x");
            float y = this.GetRequestValFloat("y");

            BP.Sys.CCFormAPI.NewEnumField(fk_mapdata, keyOfEn, fieldDesc, enumKeyOfBind, ctrl, x, y);
            return "正常にバインドします。";
        }
        /// <summary>
        /// 创建外键字段.
        /// </summary>
        /// <returns></returns>
        public string NewSFTableField()
        {
            try
            {
                string fk_mapdata = this.GetRequestVal("FK_MapData");
                string keyOfEn = this.GetRequestVal("KeyOfEn");
                string fieldDesc = this.GetRequestVal("Name");
                string sftable = this.GetRequestVal("UIBindKey");
                float x = float.Parse(this.GetRequestVal("x"));
                float y = float.Parse(this.GetRequestVal("y"));

                //调用接口,执行保存.
                BP.Sys.CCFormAPI.SaveFieldSFTable(fk_mapdata, keyOfEn, fieldDesc, sftable, x, y);
                return "正常に設定されました";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 转换拼音
        /// </summary>
        /// <returns></returns>
        public string ParseStringToPinyin()
        {
            string name = GetRequestVal("name");
            string flag = GetRequestVal("flag");
            //暂时没发现此方法在哪里有调用，edited by liuxc,2017-9-25
            return BP.Sys.CCFormAPI.ParseStringToPinyinField(name, Equals(flag, "true"), true, 20);
        }

        /// <summary>
        /// 创建隐藏字段.
        /// </summary>
        /// <returns></returns>
        public string NewHidF()
        {
            MapAttr mdHid = new MapAttr();
            mdHid.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
            mdHid.FK_MapData = this.FK_MapData;
            mdHid.KeyOfEn = this.KeyOfEn;
            mdHid.Name = this.Name;
            mdHid.MyDataType = int.Parse(this.GetRequestVal("FieldType"));
            mdHid.HisEditType = EditType.Edit;
            mdHid.MaxLen = 100;
            mdHid.MinLen = 0;
            mdHid.LGType = FieldTypeS.Normal;
            mdHid.UIVisible = false;
            mdHid.UIIsEnable = false;
            mdHid.Insert();

            return "正常に作成されました..";
        }
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            string sql = "";
            //返回值
            try
            {
                switch (this.DoType)
                {
                    default:
                        throw new Exception("判断なしの実行フラグ:" + this.DoType);
                }
            }
            catch (Exception ex)
            {
                return "err@" + this.ToString() + " msg:" + ex.Message;
            }
        }
        #endregion 执行父类的重写方法.

        #region 创建表单.
        public string NewFrmGuide_GenerPinYin()
        {
            string isQuanPin = this.GetRequestVal("IsQuanPin");
            string name = this.GetRequestVal("TB_Name");

            //表单No长度最大100，因有前缀CCFrm_，因此此处设置最大94，added by liuxc,2017-9-25
            string str = BP.Sys.CCFormAPI.ParseStringToPinyinField(name, Equals(isQuanPin, "1"), true, 94);

            MapData md = new MapData();
            md.No = str;
            if (md.RetrieveFromDBSources() == 0)
                return str;

            return "err@フォームID:" + str + "使用されています。";
        }
        /// <summary>
        /// 获得系统的表
        /// </summary>
        /// <returns></returns>
        public string NewFrmGuide_Init()
        {
            DataSet ds = new DataSet();

            SFDBSrc src = new SFDBSrc("local");
            ds.Tables.Add(src.ToDataTableField("SFDBSrc"));

            DataTable tables = src.GetTables(true);
            tables.TableName = "Tables";
            ds.Tables.Add(tables);
            return BP.Tools.Json.ToJson(ds);

        }
        public string NewFrmGuide_Create()
        {
            MapData md = new MapData();
            md.Name = this.GetRequestVal("TB_Name");
            md.No = DataType.ParseStringForNo(this.GetRequestVal("TB_No"), 100);

            md.HisFrmTypeInt = this.GetRequestValInt("DDL_FrmType");

            //表单的物理表.
            if (md.HisFrmType == BP.Sys.FrmType.Url || md.HisFrmType == BP.Sys.FrmType.Entity)
                md.PTable = this.GetRequestVal("TB_PTable");
            else
                md.PTable = DataType.ParseStringForNo(this.GetRequestVal("TB_PTable"), 100);

            //数据表模式。 @周朋 需要翻译.
            md.PTableModel = this.GetRequestValInt("DDL_PTableModel");

            //@李国文 需要对比翻译.
            string sort = this.GetRequestVal("FK_FrmSort");
            if (DataType.IsNullOrEmpty(sort) == true)
                sort = this.GetRequestVal("DDL_FrmTree");

            md.FK_FrmSort = sort;
            md.FK_FormTree = sort;

            md.AppType = "0";//独立表单
            md.DBSrc = this.GetRequestVal("DDL_DBSrc");
            if (md.IsExits == true)
                return "err@フォームID:" + md.No + "もう存在している。";

            switch (md.HisFrmType)
            {
                //自由，傻瓜，SL表单不做判断
                case BP.Sys.FrmType.FreeFrm:
                case BP.Sys.FrmType.FoolForm:
                case BP.Sys.FrmType.Develop:
                    break;
                case BP.Sys.FrmType.Url:
                case BP.Sys.FrmType.Entity:
                    md.Url = md.PTable;
                    md.PTable = "";
                    break;
                //如果是以下情况，导入模式
                case BP.Sys.FrmType.WordFrm:
                case BP.Sys.FrmType.ExcelFrm:
                case BP.Sys.FrmType.VSTOForExcel:
                    break;
                default:
                    throw new Exception("不明なフォームタイプ。" + md.HisFrmType.ToString());
            }
            md.Insert();

            //增加上OID字段.
            BP.Sys.CCFormAPI.RepareCCForm(md.No);

            BP.Frm.EntityType entityType = (EntityType)this.GetRequestValInt("EntityType");

            #region 如果是单据.
            if (entityType == EntityType.FrmBill)
            {
                BP.Frm.FrmBill bill = new FrmBill(md.No);
                bill.EntityType = EntityType.FrmBill;
                bill.BillNoFormat = "ccbpm{yyyy}-{MM}-{dd}-{LSH4}";

                //设置默认的查询条件.
                bill.SetPara("IsSearchKey", 1);
                bill.SetPara("DTSearchWay", 0);

                bill.Update();
                bill.CheckEnityTypeAttrsFor_Bill();
            }
            #endregion 如果是单据.

            #region 如果是实体 EnityNoName .
            if (entityType == EntityType.FrmDict)
            {
                BP.Frm.FrmDict entityDict = new FrmDict(md.No);
                entityDict.BillNoFormat = "3"; //编码格式.001,002,003.
                entityDict.BtnNewModel = 0;

                //设置默认的查询条件.
                entityDict.SetPara("IsSearchKey", 1);
                entityDict.SetPara("DTSearchWay", 0);

                entityDict.EntityType = EntityType.FrmDict;

                entityDict.Update();
                entityDict.CheckEnityTypeAttrsFor_EntityNoName();
            }
            #endregion 如果是实体 EnityNoName .

            //创建表与字段.
            GEEntity en = new GEEntity(md.No);
            en.CheckPhysicsTable();

            if (md.HisFrmType == BP.Sys.FrmType.WordFrm || md.HisFrmType == BP.Sys.FrmType.ExcelFrm)
            {
                /*把表单模版存储到数据库里 */
                return "url@../../Comm/RefFunc/En.htm?EnName=BP.WF.Template.MapFrmExcel&PKVal=" + md.No;
            }

            if (md.HisFrmType == BP.Sys.FrmType.Entity)
                return "url@../../Comm/Ens.htm?EnsName=" + md.PTable;

            if (md.HisFrmType == BP.Sys.FrmType.FreeFrm)
                return "url@FormDesigner.htm?FK_MapData=" + md.No + "&EntityType=" + this.GetRequestVal("EntityType");

            if (md.HisFrmType == BP.Sys.FrmType.Develop)
                return "url@../DevelopDesigner/Designer.htm?FK_MapData=" + md.No + "&FrmID="+md.No+"&EntityType=" + this.GetRequestVal("EntityType");


            return "url@../FoolFormDesigner/Designer.htm?IsFirst=1&FK_MapData=" + md.No + "&EntityType=" + this.GetRequestVal("EntityType");
        }
        #endregion 创建表单.

        public string LetLogin()
        {
            BP.Port.Emp emp = new BP.Port.Emp("admin");
            WebUser.SignInOfGener(emp);
            return "ログイン成功。";
        }
       

        public string GoToFrmDesigner_Init()
        {
            //根据不同的表单类型转入不同的表单设计器上去.
            BP.Sys.MapData md = new BP.Sys.MapData(this.FK_MapData);
            if (md.HisFrmType == BP.Sys.FrmType.FoolForm)
            {
                /* 傻瓜表单 需要翻译. */
                return "url@../FoolFormDesigner/Designer.htm?IsFirst=1&FK_MapData=" + this.FK_MapData;
            }

            if (md.HisFrmType == BP.Sys.FrmType.Develop)
            {
                /* 开发者表单 */
                return "url@../DevelopDesigner/Designer.htm?FK_MapData=" + this.FK_MapData + "&FrmID="+this.FK_MapData+"&IsFirst=1";
            }

            if (md.HisFrmType == BP.Sys.FrmType.FreeFrm)
            {
                /* 自由表单 */
                return "url@../CCFormDesigner/FormDesigner.htm?FK_MapData=" + this.FK_MapData + "&IsFirst=1";
            }

            if (md.HisFrmType == BP.Sys.FrmType.VSTOForExcel)
            {
                /* 自由表单 */
                return "url@../CCFormDesigner/FormDesigner.htm?FK_MapData=" + this.FK_MapData;
            }

            if (md.HisFrmType == BP.Sys.FrmType.Url)
            {
                /* 自由表单 */
                return "url@../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.MapDataURL&No=" + this.FK_MapData;
            }

            if (md.HisFrmType == BP.Sys.FrmType.Entity)
                return "url@../../Comm/Ens.htm?EnsName=" + md.PTable;

            return "err@ジャッジフォーム転送タイプなし" + md.HisFrmType.ToString();
        }

        public string PublicNoNameCtrlCreate()
        {
            try
            {
                float x = float.Parse(this.GetRequestVal("x"));
                float y = float.Parse(this.GetRequestVal("y"));
                BP.Sys.CCFormAPI.CreatePublicNoNameCtrl(this.FrmID, this.GetRequestVal("CtrlType"),
                    this.GetRequestVal("No"),
                    this.GetRequestVal("Name"), x, y);
                return "true";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string NewImage()
        {

            try
            {
                BP.Sys.CCFormAPI.NewImage(this.GetRequestVal("FrmID"),
                    this.GetRequestVal("KeyOfEn"), this.GetRequestVal("Name"),
                    //int.Parse(this.GetRequestVal("FieldType")),
                    float.Parse(this.GetRequestVal("x")),
                   float.Parse(this.GetRequestVal("y"))
                   );
                return "true";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }


        }
        public string NewField()
        {
            try
            {
                BP.Sys.CCFormAPI.NewField(this.GetRequestVal("FrmID"),
                    this.GetRequestVal("KeyOfEn"), this.GetRequestVal("Name"),
                    int.Parse(this.GetRequestVal("FieldType")),
                    float.Parse(this.GetRequestVal("x")),
                   float.Parse(this.GetRequestVal("y"))
                   );
                return "true";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 生成所有表单元素.
        /// </summary>
        /// <returns></returns>
        public string CCForm_AllElements_ResponseJson()
        {
            try
            {
                DataSet ds = new DataSet();

                MapData mapData = new MapData(this.FK_MapData);

                //属性.
                MapAttrs attrs = new MapAttrs(this.FK_MapData);
                attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.UIVisible, 1);
                ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr"));

                FrmBtns btns = new FrmBtns(this.FK_MapData);
                ds.Tables.Add(btns.ToDataTableField("Sys_FrmBtn"));

                FrmRBs rbs = new FrmRBs(this.FK_MapData);
                ds.Tables.Add(rbs.ToDataTableField("Sys_FrmRB"));

                FrmLabs labs = new FrmLabs(this.FK_MapData);
                ds.Tables.Add(labs.ToDataTableField("Sys_FrmLab"));

                FrmLinks links = new FrmLinks(this.FK_MapData);
                ds.Tables.Add(links.ToDataTableField("Sys_FrmLink"));

                FrmImgs imgs = new FrmImgs(this.FK_MapData);
                ds.Tables.Add(imgs.ToDataTableField("Sys_FrmImg"));

                FrmImgAths imgAths = new FrmImgAths(this.FK_MapData);
                ds.Tables.Add(imgAths.ToDataTableField("Sys_FrmImgAth"));

                FrmAttachments aths = new FrmAttachments(this.FK_MapData);
                ds.Tables.Add(aths.ToDataTableField("Sys_FrmAttachment"));

                MapDtls dtls = new MapDtls();
                dtls.Retrieve(MapDtlAttr.FK_MapData, this.FK_MapData, MapDtlAttr.FK_Node, 0);
                ds.Tables.Add(dtls.ToDataTableField("Sys_MapDtl"));

                FrmLines lines = new FrmLines(this.FK_MapData);
                ds.Tables.Add(lines.ToDataTableField("Sys_FrmLine"));

                BP.Sys.FrmUI.MapFrameExts mapFrameExts = new BP.Sys.FrmUI.MapFrameExts(this.FK_MapData);
                ds.Tables.Add(mapFrameExts.ToDataTableField("Sys_MapFrame"));

                //组织节点组件信息.
                string sql = "";
                if (this.FK_Node > 100)
                {
                    sql += "select '軌跡図' AS Name,'FlowChart' AS No,FrmTrackSta Sta,FrmTrack_X X,FrmTrack_Y Y,FrmTrack_H H,FrmTrack_W  W from WF_Node WHERE nodeid=" + SystemConfig.AppCenterDBVarStr + "nodeid";
                    sql += " union select '監査コンポーネント'AS Name, 'FrmCheck'AS No,FWCSta Sta,FWC_X X,FWC_Y Y,FWC_H H, FWC_W W from WF_Node WHERE nodeid=" + SystemConfig.AppCenterDBVarStr + "nodeid";
                    sql += " union select 'サブフロー' AS Name,'SubFlowDtl'AS  No,SFSta Sta,SF_X X,SF_Y Y,SF_H H, SF_W W from WF_Node  WHERE nodeid=" + SystemConfig.AppCenterDBVarStr + "nodeid";
                    sql += " union select '子スレッド' AS Name, 'ThreadDtl'AS  No,FrmThreadSta Sta,FrmThread_X X,FrmThread_Y Y,FrmThread_H H,FrmThread_W W from WF_Node WHERE nodeid=" + SystemConfig.AppCenterDBVarStr + "nodeid";
                    sql += " union select 'サーキュレーションカスタマイズ' AS Name,'FrmTransferCustom' AS  No,FTCSta Sta,FTC_X X,FTC_Y Y,FTC_H H,FTC_W  W FROM WF_Node WHERE nodeid=" + SystemConfig.AppCenterDBVarStr + "nodeid";
                    Paras ps = new Paras();
                    ps.SQL = sql;
                    ps.Add("nodeid", this.FK_Node);
                    DataTable dt = null;

                    try
                    {
                        dt = DBAccess.RunSQLReturnTable(ps);
                    }
                    catch (Exception ex)
                    {
                        FrmSubFlow sb = new FrmSubFlow();
                        sb.CheckPhysicsTable();

                        TransferCustom tc = new TransferCustom();
                        tc.CheckPhysicsTable();

                        FrmThread ft = new FrmThread();
                        ft.CheckPhysicsTable();

                        FrmTrack ftd = new FrmTrack();
                        ftd.CheckPhysicsTable();

                        FrmTransferCustom ftd1 = new FrmTransferCustom();
                        ftd1.CheckPhysicsTable();

                        throw ex;
                    }

                    dt.TableName = "FigureCom";

                    if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                    {
                        //  figureComCols = "Name,No,Sta,X,Y,H,W";
                        dt.Columns[0].ColumnName = "Name";
                        dt.Columns[1].ColumnName = "No";
                        dt.Columns[2].ColumnName = "Sta";
                        dt.Columns[3].ColumnName = "X";
                        dt.Columns[4].ColumnName = "Y";
                        dt.Columns[5].ColumnName = "H";
                        dt.Columns[6].ColumnName = "W";
                    }
                    ds.Tables.Add(dt);
                }

                return BP.Tools.Json.ToJson(ds);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 保存表单
        /// </summary>
        /// <returns></returns>
        public string SaveForm()
        {
            //清缓存
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            string docs = this.GetRequestVal("diagram");
            BP.Sys.CCFormAPI.SaveFrm(this.FK_MapData, docs);

            return "正常に保存。";
        }

        #region 表格处理.
        public string Tables_Init()
        {
            BP.Sys.SFTables tabs = new BP.Sys.SFTables();
            tabs.RetrieveAll();
            DataTable dt = tabs.ToDataTableField();
            dt.Columns.Add("RefNum", typeof(int));

            foreach (DataRow dr in dt.Rows)
            {
                //求引用数量.
                int refNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(KeyOfEn) FROM Sys_MapAttr WHERE UIBindKey='" + dr["No"] + "'", 0);
                dr["RefNum"] = refNum;
            }
            return BP.Tools.Json.ToJson(dt);
        }
        public string Tables_Delete()
        {
            try
            {
                BP.Sys.SFTable tab = new BP.Sys.SFTable();
                tab.No = this.No;
                tab.Delete();
                return "正常に削除されました。";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        public string TableRef_Init()
        {
            BP.Sys.MapAttrs mapAttrs = new BP.Sys.MapAttrs();
            mapAttrs.RetrieveByAttr(BP.Sys.MapAttrAttr.UIBindKey, this.FK_SFTable);

            DataTable dt = mapAttrs.ToDataTableField();
            return BP.Tools.Json.ToJson(dt);
        }


        #endregion

        /// <summary>
        /// 表单重置
        /// </summary>
        /// <returns></returns>
        public string ResetFrm_Init()
        {
            MapData md = new MapData(this.FK_MapData);
            md.ResetMaxMinXY();
            md.Update();
            return "正常にリセットしました。";
        }

        #region 复制表单
        /// <summary>
        /// 复制表单属性和表单内容
        /// </summary>
        /// <param name="frmId">新表单ID</param>
        /// <param name="frmName">新表单内容</param>
        public void DoCopyFrm()
        {
            string fromFrmID = GetRequestVal("FromFrmID");
            string toFrmID = GetRequestVal("ToFrmID");
            string toFrmName = GetRequestVal("ToFrmName");
            #region 原表单信息
            //表单信息
            MapData fromMap = new MapData(fromFrmID);
            //单据信息
            FrmBill fromBill = new FrmBill();
            fromBill.No = fromFrmID;
            int billCount = fromBill.RetrieveFromDBSources();
            //实体单据
            FrmDict fromDict = new FrmDict();
            fromDict.No = fromFrmID;
            int DictCount = fromDict.RetrieveFromDBSources();
            #endregion 原表单信息

            #region 复制表单
            MapData toMapData = new MapData();
            toMapData = fromMap;
            toMapData.No = toFrmID;
            toMapData.Name = toFrmName;
            toMapData.Insert();
            if (billCount != 0)
            {
                FrmBill toBill = new FrmBill();
                toBill = fromBill;
                toBill.No = toFrmID;
                toBill.Name = toFrmName;
                toBill.EntityType = EntityType.FrmBill;
                toBill.Update();
            }
            if (DictCount != 0)
            {
                FrmDict toDict = new FrmDict();
                toDict = fromDict;
                toDict.No = toFrmID;
                toDict.Name = toFrmName;
                toDict.EntityType = EntityType.FrmDict;
                toDict.Update();
            }
            #endregion 复制表单

            MapData.ImpMapData(toFrmID, BP.Sys.CCFormAPI.GenerHisDataSet_AllEleInfo(fromFrmID));

            //清空缓存
            toMapData.RepairMap();
            BP.Sys.SystemConfig.DoClearCash();


        }
        #endregion 复制表单

    }
}
