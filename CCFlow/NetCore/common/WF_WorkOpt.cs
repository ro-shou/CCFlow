﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.IO;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体g
    /// </summary>
    public class WF_WorkOpt : DirectoryPageBase
    {
        /// <summary>
        /// 过程执行.
        /// </summary>
        /// <returns></returns>
        public string ccbpmServices()
        {
            BP.WF.DTS.ccbpmServices en = new DTS.ccbpmServices();
            en.Do();
            return "実行が成功しました。\\DataUser\\Log\\で実行情報を確認してください。";
        }
        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <returns></returns>
        public string ThreadDtl_DelSubFlow()
        {
            BP.WF.Dev2Interface.Flow_DeleteSubThread(this.FK_Flow, this.WorkID, "手動で削除");
            return "正常に削除されました";
        }
        #region 打印 rtf
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string PrintDoc_Init()
        {
            string sourceType = this.GetRequestVal("SourceType");
            string FK_MapData = "";
            Node nd = null;
            if (this.FK_Node != 0 && this.FK_Node != 9999)
            {
                nd = new Node(this.FK_Node);

                if (nd.HisFormType == NodeFormType.SheetTree)
                {
                    //获取该节点绑定的表单
                    // 所有表单集合.
                    MapDatas mds = new MapDatas();
                    mds.RetrieveInSQL("SELECT FK_Frm FROM WF_FrmNode WHERE FK_Node=" + this.FK_Node + " AND FrmEnableRole !=5");
                    return "info@" + BP.Tools.Json.ToJson(mds.ToDataTableField());
                }

                FK_MapData = "ND" + this.FK_Node;

                if (nd.HisFormType == NodeFormType.RefOneFrmTree)
                    FK_MapData = nd.NodeFrmID;

                if (nd.HisFormType == NodeFormType.SDKForm || nd.HisFormType == NodeFormType.SelfForm)
                {
                    return "err@SDKフォームと埋め込みフォームは現在印刷をサポートしていません";
                }
            }
            if (DataType.IsNullOrEmpty(sourceType) == false && sourceType.Equals("Bill"))
                FK_MapData = this.GetRequestVal("FrmID");

            BillTemplates templetes = new BillTemplates();
            string billNo = this.GetRequestVal("FK_Bill");
            if (billNo == null)
                templetes.Retrieve(BillTemplateAttr.FK_MapData, FK_MapData);
            else
                templetes.Retrieve(BillTemplateAttr.FK_MapData, this.FK_MapData, BillTemplateAttr.No, billNo);

            if (templetes.Count == 0)
                return "err@現在のノードにバインドされているドキュメントテンプレートはありません。";

            if (templetes.Count == 1)
            {
                BillTemplate templete = templetes[0] as BillTemplate;

                //单据的打印
                if (DataType.IsNullOrEmpty(sourceType) == false && sourceType.Equals("Bill"))
                    return PrintDoc_FormDoneIt(null, this.WorkID, this.FID, FK_MapData, templete);

                if (nd!=null && nd.HisFormType == NodeFormType.RefOneFrmTree)
                    return PrintDoc_FormDoneIt(null, this.WorkID, this.FID, FK_MapData, templete);

                return PrintDoc_DoneIt(templete.No);
            }
            return templetes.ToJson();
        }
        /// <summary>
        /// 执行打印
        /// </summary>
        /// <returns></returns>
        public string PrintDoc_Done()
        {
            string billTemplateNo = this.GetRequestVal("FK_Bill");
            return PrintDoc_DoneIt(billTemplateNo);
        }

        /// <summary>
        /// 打印pdf.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public string PrintDoc_DoneIt(string billTemplateNo = null)
        {
            Node nd = new Node(this.FK_Node);

            if (billTemplateNo == null)
                billTemplateNo = this.GetRequestVal("FK_Bill");

            BillTemplate func = new BillTemplate(billTemplateNo);

            //如果不是 BillTemplateExcel 打印.
            if (func.TemplateFileModel == TemplateFileModel.VSTOForExcel)
                return "url@httpccword://-fromccflow,App=BillTemplateExcel,TemplateNo=" + func.No + ",WorkID=" + this.WorkID + ",FK_Flow=" + this.FK_Flow + ",FK_Node=" + this.FK_Node + ",UserNo=" + BP.Web.WebUser.No + ",SID=" + BP.Web.WebUser.SID;

            //如果不是 BillTemplateWord 打印
            if (func.TemplateFileModel == TemplateFileModel.VSTOForWord)
                return "url@httpccword://-fromccflow,App=BillTemplateWord,TemplateNo=" + func.No + ",WorkID=" + this.WorkID + ",FK_Flow=" + this.FK_Flow + ",FK_Node=" + this.FK_Node + ",UserNo=" + BP.Web.WebUser.No + ",SID=" + BP.Web.WebUser.SID;

            string billInfo = "";

            string ccformId = this.GetRequestVal("CCFormID");
            if (DataType.IsNullOrEmpty(ccformId) == false)
                return PrintDoc_FormDoneIt(nd, this.WorkID, this.FID, ccformId, func);

            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();

            string file = DateTime.Now.Year + "_" + WebUser.FK_Dept + "_" + func.No + "_" + WorkID + ".doc";
            BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine();

            string[] paths;
            string path;
            Int64 newWorkID = 0;
            try
            {
                #region 单据变量.
                Bill bill = new Bill();
                bill.MyPK = wk.FID + "_" + wk.OID + "_" + nd.NodeID + "_" + func.No;
                #endregion

                #region 生成单据
                rtf.HisEns.Clear();
                rtf.EnsDataDtls.Clear();
                if (func.NodeID != 0)
                {
                    //把流程主表数据放入里面去.
                    GEEntity ndxxRpt = new GEEntity("ND" + int.Parse(nd.FK_Flow) + "Rpt");
                    try
                    {
                        ndxxRpt.PKVal = this.WorkID;
                        ndxxRpt.Retrieve();

                        newWorkID = this.WorkID;
                    }
                    catch (Exception ex)
                    {
                        if (FID > 0)
                        {
                            ndxxRpt.PKVal = this.FID;
                            ndxxRpt.Retrieve();

                            newWorkID = this.FID;

                            wk = null;
                            wk = nd.HisWork;
                            wk.OID = this.WorkID;
                            wk.RetrieveFromDBSources();
                        }
                        else
                        {
                            BP.WF.DTS.InitBillDir dir = new BP.WF.DTS.InitBillDir();
                            dir.Do();
                            path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + Path.DirectorySeparatorChar + WebUser.FK_Dept + Path.DirectorySeparatorChar + func.No + Path.DirectorySeparatorChar;
                            string msgErr = "@" + string.Format("ドキュメントの生成に失敗しました。管理者にディレクトリ設定の確認を依頼してください") + "[" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
                            billInfo += "@<font color=red>" + msgErr + "</font>";
                            throw new Exception(msgErr + "@その他の情報:" + ex.Message);
                        }
                    }
                    ndxxRpt.Copy(wk);

                    //把数据赋值给wk. 有可能用户还没有执行流程检查，字段没有同步到 NDxxxRpt.
                    if (ndxxRpt.Row.Count > wk.Row.Count)
                        wk.Row = ndxxRpt.Row;

                    rtf.HisGEEntity = wk;

                    //加入他的明细表.
                    List<Entities> al = wk.GetDtlsDatasOfList();
                    foreach (Entities ens in al)
                        rtf.AddDtlEns(ens);

                    //增加多附件数据
                    FrmAttachments aths = wk.HisFrmAttachments;
                    foreach (FrmAttachment athDesc in aths)
                    {
                        FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
                        if (athDBs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, athDesc.MyPK, FrmAttachmentDBAttr.RefPKVal, newWorkID, "RDT") == 0)
                            continue;

                        rtf.EnsDataAths.Add(athDesc.NoOfObj, athDBs);
                    }

                    //把审核日志表加入里面去.
                    Paras ps = new BP.DA.Paras();
                    ps.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                    ps.Add(TrackAttr.ActionType, (int)ActionType.WorkCheck);
                    ps.Add(TrackAttr.WorkID, newWorkID);

                    rtf.dtTrack = BP.DA.DBAccess.RunSQLReturnTable(ps);
                }

                paths = file.Split('_');
                path = paths[0] + "/" + paths[1] + "/" + paths[2] + "/";
                string fileModelT = "rtf";
                if ((int)func.TemplateFileModel == 1)
                    fileModelT = "word";
                string billUrl = "url@" + fileModelT+"@"+ BP.WF.Glo.CCFlowAppPath + "DataUser/Bill/" + path + file;

                if (func.HisBillFileType == BillFileType.PDF)
                    billUrl = billUrl.Replace(".doc", ".pdf");

                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + Path.DirectorySeparatorChar + WebUser.FK_Dept + Path.DirectorySeparatorChar + func.No + Path.DirectorySeparatorChar;
                //  path = Server.MapPath(path);
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                string tempFile = func.TempFilePath;
                if (tempFile.Contains(".rtf") == false)
                    tempFile = tempFile + ".rtf";

                //用于扫描打印.
                string qrUrl = SystemConfig.HostURL + "WF/WorkOpt/PrintDocQRGuide.htm?MyPK=" + bill.MyPK;
                rtf.MakeDoc(tempFile,
                    path, file, false, qrUrl);
                #endregion

                #region 转化成pdf.
                if (func.HisBillFileType == BillFileType.PDF)
                {
                    string rtfPath = path + file;
                    string pdfPath = rtfPath.Replace(".doc", ".pdf");
                    try
                    {
                        BP.WF.Glo.Rtf2PDF(rtfPath, pdfPath);
                    }
                    catch (Exception ex)
                    {
                        return "err@" + ex.Message;
                    }
                }
                #endregion

                #region 保存单据.

                bill.FID = wk.FID;
                bill.WorkID = wk.OID;
                bill.FK_Node = wk.NodeID;
                bill.FK_Dept = WebUser.FK_Dept;
                bill.FK_Emp = WebUser.No;
                bill.Url = billUrl;
                bill.RDT = DataType.CurrentDataTime;
                bill.FullPath = path + file;
                bill.FK_NY = DataType.CurrentYearMonth;
                bill.FK_Flow = nd.FK_Flow;
                // bill.FK_BillType = func.FK_BillType;
                bill.Emps = rtf.HisGEEntity.GetValStrByKey("Emps");
                bill.FK_Starter = rtf.HisGEEntity.GetValStrByKey("Rec");
                bill.StartDT = rtf.HisGEEntity.GetValStrByKey("RDT");
                bill.Title = rtf.HisGEEntity.GetValStrByKey("Title");
                bill.FK_Dept = rtf.HisGEEntity.GetValStrByKey("FK_Dept");

                try
                {
                    bill.Save();
                }
                catch
                {
                    bill.Update();
                }
                #endregion

                //在线WebOffice打开
                if (func.BillOpenModel == BillOpenModel.WebOffice)
                {
                    return "err@【/WF/WebOffice/PrintOffice.aspx】ファイルはリファクタリングされていません。古いバージョンを見つけて解決するか、自分で開発できます。";
                    //  return "err@该文件没有重构好,您可以找到旧版本解决，或者自己开发。";
                    //return "url@../WebOffice/PrintOffice.aspx?MyPK=" + bill.MyPK;
                }
                return billUrl;
            }
            catch (Exception ex)
            {
                BP.WF.DTS.InitBillDir dir = new BP.WF.DTS.InitBillDir();
                dir.Do();
                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + Path.DirectorySeparatorChar + WebUser.FK_Dept + Path.DirectorySeparatorChar + func.No + Path.DirectorySeparatorChar;
                string msgErr = "@" + string.Format("ドキュメントの生成に失敗しました。管理者にディレクトリ設定の確認を依頼してください") + "[" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
                return "err@<font color=red>" + msgErr + "</font>" + ex.Message;
            }
        }
        #endregion

        public string PrintDoc_FormDoneIt(Node nd, long workID, long fid, string formID, BillTemplate func)
        {
            Int64 pkval = workID;
            Work wk = null;
            string billInfo = "";
            if (nd != null)
            {
                BP.WF.Template.FrmNode fn = new FrmNode();
                fn = new FrmNode(nd.FK_Flow, nd.NodeID, formID);
                //先判断解决方案
                if (fn != null && fn.WhoIsPK != WhoIsPK.OID)
                {
                    if (fn.WhoIsPK == WhoIsPK.PWorkID)
                        pkval = this.PWorkID;
                    if (fn.WhoIsPK == WhoIsPK.FID)
                        pkval = fid;
                }
                wk = nd.HisWork;
                wk.OID = this.WorkID;
                wk.RetrieveFromDBSources();
            }

            MapData mapData = new MapData(formID);

            string file = DateTime.Now.Year + "_" + WebUser.FK_Dept + "_" + func.No + "_" + WorkID + ".doc";
            BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine();

            string[] paths;
            string path;
            Int64 newWorkID = 0;
            try
            {
                #region 单据变量.
                Bill bill = new Bill();
                if (nd != null)
                    bill.MyPK = wk.FID + "_" + wk.OID + "_" + nd.NodeID + "_" + func.No;
                else
                    bill.MyPK = fid + "_" + workID + "_0_" + func.No;
                #endregion

                #region 生成单据
                rtf.HisEns.Clear();
                rtf.EnsDataDtls.Clear();
                if (DataType.IsNullOrEmpty(func.FK_MapData) == false)
                {
                    //把流程主表数据放入里面去.
                    GEEntity ndxxRpt = new GEEntity(formID);
                    try
                    {
                        ndxxRpt.PKVal = pkval;
                        ndxxRpt.Retrieve();

                        newWorkID = pkval;
                    }
                    catch (Exception ex)
                    {
                        if (FID > 0)
                        {
                            ndxxRpt.PKVal = this.FID;
                            ndxxRpt.Retrieve();

                            newWorkID = this.FID;

                            wk = null;
                            wk = nd.HisWork;
                            wk.OID = this.WorkID;
                            wk.RetrieveFromDBSources();
                        }
                        else
                        {
                            BP.WF.DTS.InitBillDir dir = new BP.WF.DTS.InitBillDir();
                            dir.Do();
                            path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + Path.DirectorySeparatorChar + WebUser.FK_Dept + Path.DirectorySeparatorChar + func.No + Path.DirectorySeparatorChar;
                            string msgErr = "@" + string.Format("ドキュメントの生成に失敗しました。管理者にディレクトリ設定の確認を依頼してください") + "[" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
                            billInfo += "@<font color=red>" + msgErr + "</font>";
                            throw new Exception(msgErr + "@その他の情報:" + ex.Message);
                        }
                    }
                    //ndxxRpt.Copy(wk);

                    //把数据赋值给wk. 有可能用户还没有执行流程检查，字段没有同步到 NDxxxRpt.
                    //if (ndxxRpt.Row.Count > wk.Row.Count)
                    //   wk.Row = ndxxRpt.Row;

                    rtf.HisGEEntity = ndxxRpt;

                    //加入他的明细表.
                    List<Entities> al = mapData.GetDtlsDatasOfList(pkval.ToString());
                    if (al.Count == 0)
                    {
                        MapDtls mapdtls = mapData.MapDtls;
                        foreach (MapDtl dtl in mapdtls)
                        {
                            GEDtls dtls1 = new GEDtls(dtl.No);
                            mapData.EnMap.AddDtl(dtls1, "RefPK");

                        }
                        al = mapData.GetDtlsDatasOfList(pkval.ToString());
                    }
                    foreach (Entities ens in al)
                        rtf.AddDtlEns(ens);

                    //增加多附件数据
                    FrmAttachments aths = mapData.FrmAttachments;
                    foreach (FrmAttachment athDesc in aths)
                    {
                        FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
                        if (athDBs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, athDesc.MyPK, FrmAttachmentDBAttr.RefPKVal, newWorkID, "RDT") == 0)
                            continue;

                        rtf.EnsDataAths.Add(athDesc.NoOfObj, athDBs);
                    }

                    if (nd != null)
                    {
                        //把审核日志表加入里面去.
                        Paras ps = new BP.DA.Paras();
                        ps.SQL = "SELECT * FROM ND" + int.Parse(nd.FK_Flow) + "Track WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                        ps.Add(TrackAttr.ActionType, (int)ActionType.WorkCheck);
                        ps.Add(TrackAttr.WorkID, newWorkID);

                        rtf.dtTrack = BP.DA.DBAccess.RunSQLReturnTable(ps);
                    }
                }

                paths = file.Split('_');
                path = paths[0] + "/" + paths[1] + "/" + paths[2] + "/";
                string fileModelT = "rtf";
                if ((int)func.TemplateFileModel == 1)
                    fileModelT = "word";

                string billUrl = "url@" + fileModelT + "@" + BP.WF.Glo.CCFlowAppPath + "DataUser/Bill/" + path + file;

                if (func.HisBillFileType == BillFileType.PDF)
                    billUrl = billUrl.Replace(".doc", ".pdf");

                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + Path.DirectorySeparatorChar + WebUser.FK_Dept + Path.DirectorySeparatorChar + func.No + Path.DirectorySeparatorChar;
                //  path = Server.MapPath(path);
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                string tempFile = func.TempFilePath;
                if (tempFile.Contains(".rtf") == false)
                    tempFile = tempFile + ".rtf";

                //用于扫描打印.
                string qrUrl = SystemConfig.HostURL + "WF/WorkOpt/PrintDocQRGuide.htm?MyPK=" + bill.MyPK;
                rtf.MakeDoc(tempFile,
                    path, file, false, qrUrl);
                #endregion

                #region 转化成pdf.
                if (func.HisBillFileType == BillFileType.PDF)
                {
                    string rtfPath = path + file;
                    string pdfPath = rtfPath.Replace(".doc", ".pdf");
                    try
                    {
                        BP.WF.Glo.Rtf2PDF(rtfPath, pdfPath);
                    }
                    catch (Exception ex)
                    {
                        return "err@" + ex.Message;
                    }
                }
                #endregion

                #region 保存单据.
                if (nd != null)
                {
                    bill.FID = wk.FID;
                    bill.WorkID = wk.OID;
                    bill.FK_Node = wk.NodeID;
                    bill.FK_Flow = nd.FK_Flow;
                }
                else
                {
                    bill.FID = fid;
                    bill.WorkID = workID;
                    bill.FK_Node = 0;
                    bill.FK_Flow = "0";
                }
                bill.FK_Dept = WebUser.FK_Dept;
                bill.FK_Emp = WebUser.No;
                bill.Url = billUrl;
                bill.RDT = DataType.CurrentDataTime;
                bill.FullPath = path + file;
                bill.FK_NY = DataType.CurrentYearMonth;
                bill.Emps = rtf.HisGEEntity.GetValStrByKey("Emps");
                bill.FK_Starter = rtf.HisGEEntity.GetValStrByKey("Rec");
                bill.StartDT = rtf.HisGEEntity.GetValStrByKey("RDT");
                bill.Title = rtf.HisGEEntity.GetValStrByKey("Title");
                bill.FK_Dept = rtf.HisGEEntity.GetValStrByKey("FK_Dept");

                try
                {
                    bill.Save();
                }
                catch
                {
                    bill.Update();
                }
                #endregion

                //在线WebOffice打开
                if (func.BillOpenModel == BillOpenModel.WebOffice)
                {
                    //return "err@该部分的代码还没有重构.";
                    return "err@【/WF/WebOffice/PrintOffice.htm】ファイルはリファクタリングされていません。古いバージョンを見つけて解決するか、自分で開発できます。";
                    //return "url@../WebOffice/PrintOffice.aspx?MyPK=" + bill.MyPK;
                }
                return billUrl;
            }
            catch (Exception ex)
            {
                BP.WF.DTS.InitBillDir dir = new BP.WF.DTS.InitBillDir();
                dir.Do();
                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + Path.DirectorySeparatorChar + WebUser.FK_Dept + Path.DirectorySeparatorChar + func.No + Path.DirectorySeparatorChar;
                string msgErr = "@" + string.Format("ドキュメントの生成に失敗しました。管理者にディレクトリ設定の確認を依頼してください") + "[" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
                return "err@<font color=red>" + msgErr + "</font>" + ex.Message;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_WorkOpt()
        {
        }
        /// <summary>
        /// 打包下载
        /// </summary>
        /// <returns></returns>
        public string Packup_Init()
        {
            try
            {
                string sourceType = this.GetRequestVal("SourceType");
                //打印单据实体、单据表单
                if (DataType.IsNullOrEmpty(sourceType) == false && sourceType.Equals("Bill"))
                {
                    return MakeForm2Html.MakeBillToPDF(this.GetRequestVal("FrmID"), this.WorkID, this.GetRequestVal("BasePath"), false);
                }
                int nodeID = this.FK_Node;
                if (this.FK_Node == 0)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                    nodeID = gwf.FK_Node;
                }

                Node nd = new Node(nodeID);
                //树形表单方案单独打印
                if ((nd.HisFormType == NodeFormType.SheetTree && nd.HisPrintPDFModle == 1))
                {
                    //获取该节点绑定的表单
                    // 所有表单集合.
                    MapDatas mds = new MapDatas();
                    mds.RetrieveInSQL("SELECT FK_Frm FROM WF_FrmNode WHERE FK_Node=" + this.FK_Node + " AND FrmEnableRole != 5");
                    return "info@" + BP.Tools.Json.ToJson(mds.ToDataTableField());
                }


                return BP.WF.MakeForm2Html.MakeCCFormToPDF(nd, this.WorkID, this.FK_Flow, null, false, this.GetRequestVal("BasePath"));

            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /**
	     * 独立表单PDF打印
	     * @return
	     * @throws Exception 
	     */
        public String Packup_FromInit()
        {
            try
            {
                int nodeID = this.FK_Node;
                if (this.FK_Node == 0)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                    nodeID = gwf.FK_Node;
                }
                Node nd = new Node(nodeID);
                return MakeForm2Html.MakeFormToPDF(this.GetRequestVal("FrmID"), this.GetRequestVal("FrmName"), nd, this.WorkID, this.FK_Flow, null, false, this.GetRequestVal("BasePath"));
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 扫描二维码获得文件.
        /// </summary>
        /// <returns></returns>
        public string PrintDocQRGuide_Init()
        {
            try
            {
                int nodeID = this.FK_Node;
                if (this.FK_Node == 0)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                    nodeID = gwf.FK_Node;
                }

                Node nd = new Node(nodeID);
                Work wk = nd.HisWork;
                return BP.WF.MakeForm2Html.MakeCCFormToPDF(nd, this.WorkID, this.FK_Flow, null, false, this.GetRequestVal("BasePath"));
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 选择表单,发起前置导航.
        /// </summary>
        /// <returns></returns>
        public string StartGuideFrms_Init()
        {
            BP.WF.Template.FrmNodes fns = new BP.WF.Template.FrmNodes();

            QueryObject qo = new QueryObject(fns);
            qo.AddWhere(FrmNodeAttr.FK_Node, int.Parse(this.FK_Flow + "01"));
            qo.addAnd();
            qo.AddWhere(FrmNodeAttr.FrmEnableRole, (int)BP.WF.Template.FrmEnableRole.WhenHaveFrmPara);
            qo.addOrderBy(FrmNodeAttr.Idx);
            qo.DoQuery();

            foreach (BP.WF.Template.FrmNode item in fns)
            {
                item.GuanJianZiDuan = item.HisFrm.Name;
            }
            return fns.ToJson();
        }

        #region 通用人员选择器.
        /// <summary>
        /// 通用人员选择器Init
        /// </summary>
        /// <returns></returns>
        public string AccepterOfGener_Init()
        {
            /* 获得上一次发送的人员列表. */
            int toNodeID = this.GetRequestValInt("ToNode");

            //查询出来,已经选择的人员.
            SelectAccpers sas = new SelectAccpers();
            int i = sas.Retrieve(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID,
                this.WorkID, SelectAccperAttr.Idx);

            if (i == 0)
            {
                //获得最近的一个workid.
                string trackTable = "ND" + int.Parse(this.FK_Flow) + "Track";
                Paras ps = new Paras();
                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                {
                    ps.SQL = "SELECT TOP 1 Tag,EmpTo FROM " + trackTable + " WHERE NDTo=" + SystemConfig.AppCenterDBVarStr + "NDTo AND (ActionType=0 OR ActionType=1) AND EmpFrom=" + SystemConfig.AppCenterDBVarStr + "EmpFrom ORDER BY WorkID desc  ";
                    ps.Add("NDTo", toNodeID);
                    ps.Add("EmpFrom", WebUser.No);
                }
                else if (SystemConfig.AppCenterDBType == DBType.Oracle)
                {
                    ps.SQL = "SELECT * FROM (SELECT  Tag,EmpTo,WorkID FROM " + trackTable + " A WHERE A.EmpFrom=" + SystemConfig.AppCenterDBVarStr + "EmpFrom AND A.NDFrom=" + SystemConfig.AppCenterDBVarStr + "NDFrom AND A.NDTo=" + SystemConfig.AppCenterDBVarStr + "NDTo AND (ActionType=0 OR ActionType=1) AND EmpFrom=" + SystemConfig.AppCenterDBVarStr + "EmpFrom ORDER BY WorkID DESC ) WHERE ROWNUM =1";
                    ps.Add("EmpFrom", BP.Web.WebUser.No);
                    ps.Add("NDFrom", this.FK_Node);
                    ps.Add("NDTo", toNodeID);
                }
                else if (SystemConfig.AppCenterDBType == DBType.MySQL)
                {
                    ps.SQL = "SELECT Tag,EmpTo FROM " + trackTable + " A WHERE A.NDFrom=" + SystemConfig.AppCenterDBVarStr + "NDFrom AND A.NDTo=" + SystemConfig.AppCenterDBVarStr + "NDTo AND (ActionType=0 OR ActionType=1) AND EmpFrom=" + SystemConfig.AppCenterDBVarStr + "EmpFrom ORDER BY WorkID  DESC limit 1,1 ";
                    ps.Add("NDFrom", this.FK_Node);
                    ps.Add("NDTo", toNodeID);
                    ps.Add("EmpFrom", WebUser.No);
                }
                else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                {
                    ps.SQL = "SELECT Tag,EmpTo FROM " + trackTable + " A WHERE A.NDFrom=:NDFrom AND A.NDTo=:NDTo AND (ActionType=0 OR ActionType=1) AND EmpFrom=:EmpFrom ORDER BY WorkID  DESC limit 1 ";
                    ps.Add("NDFrom", this.FK_Node);
                    ps.Add("NDTo", toNodeID);
                    ps.Add("EmpFrom", WebUser.No);
                }

                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count != 0)
                {
                    string tag = dt.Rows[0]["Tag"].ToString();
                    string emps = dt.Rows[0]["EmpTo"].ToString();
                    if (tag != null && tag.Contains("EmpsAccepter=") == true)
                    {
                        string[] strs = tag.Split('@');
                        foreach (string str in strs)
                        {
                            if (str == null || str == "" || str.Contains("EmpsAccepter=") == false)
                                continue;
                            string[] mystr = str.Split('=');
                            if (mystr.Length == 2)
                            {
                                emps = mystr[1];
                            }
                        }
                    }

                    if (emps.Contains(",") == true)
                    {
                        if (emps.Contains("'") == true)
                        {
                            string[] strs = emps.Split(';');
                            foreach (string str in strs)
                            {
                                string[] emp = str.Split(',');
                                string empNo = emp[0];
                                BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, empNo, false);
                            }
                        }
                    }
                    else
                    {
                        BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, emps, false);
                    }
                }

                if (dt.Rows.Count != 0)
                    sas.Retrieve(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID, this.WorkID);
            }

            return sas.ToJson();
        }
        /// <summary>
        /// 增加接收人.
        /// </summary>
        /// <returns></returns>
        public string AccepterOfGener_AddEmps()
        {
            try
            {
                //到达的节点ID.
                int toNodeID = this.GetRequestValInt("ToNode");
                string emps = this.GetRequestVal("AddEmps");

                //增加到里面去.s
                BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, emps, false);

                //查询出来,已经选择的人员.
                SelectAccpers sas = new SelectAccpers();
                sas.Retrieve(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID,
                    this.WorkID, SelectAccperAttr.Idx);

                return sas.ToJson();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("INSERT") == true)
                    return "err@担当者名が重複しているため、一部の担当者が挿入に失敗する。";

                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行发送.
        /// </summary>
        /// <returns></returns>
        public string AccepterOfGener_Send()
        {
            try
            {
                int toNodeID = this.GetRequestValInt("ToNode");
                Node nd = new Node(toNodeID);
                if (nd.HisDeliveryWay == DeliveryWay.BySelected)
                {
                    /* 仅仅设置一个,检查压入的人员个数.*/
                    Paras ps = new Paras();
                    ps.SQL = "SELECT count(WorkID) as Num FROM WF_SelectAccper WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND AccType=0";
                    ps.Add("FK_Node", toNodeID);
                    ps.Add("WorkID", this.WorkID);
                    int num = DBAccess.RunSQLReturnValInt(ps, 0);
                    if (num == 0)
                        return "err@次のステップのハンドラを指定してください。";
                    Selector sr = new Selector(toNodeID);
                    if (sr.IsSimpleSelector == true)
                    {
                        if (num != 1)
                            return "err@選択できる受信者は1人だけです。他の受信者を削除して送信してください。";
                    }
                }

                SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, toNodeID, null);
                string strs = objs.ToMsgOfHtml();
                strs = strs.Replace("@", "<br>@");
                return strs;
            }
            catch (Exception ex)
            {
                AthUnReadLog athUnReadLog = new AthUnReadLog();
                athUnReadLog.CheckPhysicsTable();
                return "err@" + ex.Message;
            }
        }
        #endregion

        // 查询select集合
        public string AccepterOfGener_SelectEmps()
        {
            string sql = "";
            string emp = this.GetRequestVal("TB_Emps");

            #region 保障查询语句的安全.
            emp = emp.ToLower();
            emp = emp.Replace("'", "");
            emp = emp.Replace("&", "&amp");
            emp = emp.Replace("<", "&lt");
            emp = emp.Replace(">", "&gt");
            emp = emp.Replace("delete", "");
            emp = emp.Replace("update", "");
            emp = emp.Replace("insert", "");
            #endregion 保障查询语句的安全.

            bool isPinYin = DBAccess.IsExitsTableCol("Port_Emp", "PinYin");
            if (isPinYin == true)
            {
                //标识结束，不要like名字了.
                if (emp.Contains("/"))
                {
                    if (SystemConfig.CustomerNo == "TianYe") // 只改了oracle的
                    {
                        //string endSql = "";
                        //if (Web.WebUser.FK_Dept.IndexOf("18099") == 0)
                        //    endSql = " AND B.No LIKE '18099%' ";
                        //else
                        //    endSql = " AND B.No NOT LIKE '18099%' ";

                        string specFlowNos = SystemConfig.AppSettings["SpecFlowNosForAccpter"];
                        if (specFlowNos == "" || specFlowNos == null)
                            specFlowNos = ",001,";

                        string specEmpNos = "";
                        if (specFlowNos.Contains(this.FK_Node.ToString() + ",") == false)
                            specEmpNos = " AND a.No!='00000001' ";

                        sql = "SELECT a.No,a.Name || '/' || b.FullName as Name FROM Port_Emp a, Port_Dept b WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%') AND rownum<=12 " + specEmpNos;
                    }
                    else
                    {
                        if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                            sql = "SELECT TOP 12 a.No,a.Name +'/'+b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%') ";
                        if (SystemConfig.AppCenterDBType == DBType.Oracle)
                            sql = "SELECT a.No,a.Name || '/' || b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%') AND rownum<=12 ";
                        if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                            sql = "SELECT a.No,CONCAT(a.Name,'/',b.name) as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%') LIMIT 12";
                    }
                }
                else
                {
                    if (SystemConfig.CustomerNo == "TianYe")  //只改了oracle的
                    {
                        //string endSql = "";
                        //if (Web.WebUser.FK_Dept.IndexOf("18099") == 0)
                        //    endSql = " AND B.No LIKE '18099%' ";
                        //else
                        //    endSql = " AND B.No NOT LIKE '18099%' ";

                        string specFlowNos = SystemConfig.AppSettings["SpecFlowNosForAccpter"];
                        if (specFlowNos == "" || specFlowNos == null)
                            specFlowNos = ",001,";

                        string specEmpNos = "";
                        if (specFlowNos.Contains(this.FK_Node.ToString() + ",") == false)
                            specEmpNos = " AND a.No!='00000001' ";

                        Selector sa = new Selector(this.FK_Node);
                        //启用搜索范围限定.
                        if (sa.IsEnableStaRange == true || sa.IsEnableDeptRange == true)
                        {
                            sql = "SELECT a.No,a.Name || '/' || b.FullName as Name FROM Port_Emp a, Port_Dept b, WF_NodeDept c WHERE  C.FK_Node='" + GetRequestVal("ToNode") + "' AND C.FK_Dept=b.No AND (a.fk_dept=b.no) AND (  a.PinYin LIKE '%," + emp.ToLower() + "%') AND rownum<=12   " + specEmpNos;
                        }
                        else
                        {
                            sql = "SELECT a.No,a.Name || '/' || b.FullName as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (  a.PinYin LIKE '%," + emp.ToLower() + "%') AND rownum<=12   " + specEmpNos;
                        }
                    }
                    else
                    {
                        if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                            sql = "SELECT TOP 12 a.No,a.Name +'/'+b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and ( a.PinYin LIKE '%," + emp.ToLower() + "%')";
                        if (SystemConfig.AppCenterDBType == DBType.Oracle)
                            sql = "SELECT a.No,a.Name || '/' || b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (  a.PinYin LIKE '%," + emp.ToLower() + "%') AND rownum<=12 ";
                        if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                            sql = "SELECT a.No,CONCAT(a.Name,'/',b.name) as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (  a.PinYin LIKE '%," + emp.ToLower() + "%' ) LIMIT 12";
                    }
                }
            }
            else
            {
                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    sql = "SELECT TOP 12 a.No,a.Name +'/'+b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%')";
                if (SystemConfig.AppCenterDBType == DBType.Oracle)
                    sql = "SELECT a.No,a.Name || '/' || b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%') and rownum<=12 ";
                if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                    sql = "SELECT a.No,CONCAT(a.Name,'/',b.name) as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%') LIMIT 12";
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //  BP.DA.Log.DebugWriteError(sql);

            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns[0].ColumnName = "No";
                dt.Columns[1].ColumnName = "Name";
            }

            return BP.Tools.Json.ToJson(dt);
        }

        #region 会签.
        /// <summary>
        /// 会签
        /// </summary>
        /// <returns></returns>
        public string HuiQian_Init()
        {
            //要找到主持人.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianOver)
            {
                return "err@副署が完了しました、副署を実行できません。";
            }

            string huiQianType = this.GetRequestVal("HuiQianType");
            //查询出来集合.
            GenerWorkerLists ens = new GenerWorkerLists(this.WorkID, this.FK_Node);
            BtnLab btnLab = new BtnLab(this.FK_Node);
            if (btnLab.HuiQianRole != HuiQianRole.TeamupGroupLeader || (btnLab.HuiQianRole == HuiQianRole.TeamupGroupLeader && btnLab.HuiQianLeaderRole!= HuiQianLeaderRole.OnlyOne)) { 
                foreach (GenerWorkerList item in ens)
                {
                    
                    if ((gwf.HuiQianZhuChiRen.Contains(item.FK_Emp + ",") == true 
                        ||(DataType.IsNullOrEmpty(gwf.HuiQianZhuChiRen) == true
                            && gwf.GetParaString("AddLeader").Contains(item.FK_Emp+",") == false
                           && gwf.TodoEmps.Contains(item.FK_Emp + ",") == true))
                         && item.FK_Emp != BP.Web.WebUser.No
                         && item.IsHuiQian == false)
                    {
                        item.FK_EmpText = "<img src='../Img/zhuichiren.png' border=0 />" + item.FK_EmpText;
                        item.FK_EmpText = item.FK_EmpText;
                        if (item.IsPass == true)
                            item.IsPassInt = 1001;
                        else
                            item.IsPassInt = 100;
                        continue;
                    }

                   

                    //标记为自己.
                    if (item.FK_Emp == BP.Web.WebUser.No)
                    {
                        item.FK_EmpText = "" + item.FK_EmpText;
                        if (item.IsPass == true)
                            item.IsPassInt = 9901;
                        else
                            item.IsPassInt = 99;
                    }
                }
            }

            //赋值部门名称。
            DataTable mydt = ens.ToDataTableField("WF_GenerWorkList");
            mydt.Columns.Add("FK_DeptT", typeof(string));
            foreach (DataRow dr in mydt.Rows)
            {
                string fk_emp = dr["FK_Emp"].ToString();
                foreach (GenerWorkerList item in ens)
                {
                    if (item.FK_Emp == fk_emp)
                        dr["FK_DeptT"] = item.FK_DeptT;
                }
            }

            //获取当前人员的流程处理信息
            GenerWorkerList gwlOfMe = new GenerWorkerList();
            gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                        GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.FK_Node);

            DataSet ds = new DataSet();
            ds.Tables.Add(mydt);
            ds.Tables.Add(gwlOfMe.ToDataTableField("My_GenerWorkList"));

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <returns></returns>
        public string HuiQian_Delete()
        {
            string emp = this.GetRequestVal("FK_Emp");
            if (this.FK_Emp == WebUser.No)
                return "err@あなた自身を削除することはできません";

            //要找到主持人.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            string addLeader = gwf.GetParaString("AddLeader");
            if (gwf.TodoEmps.Contains(BP.Web.WebUser.No + ",") == false && addLeader.Contains(BP.Web.WebUser.No + ",")==false)
                return "err@あなたはホストではないため、削除できません。";

            //删除该数据.
            GenerWorkerList gwlOfMe = new GenerWorkerList();
            gwlOfMe.Delete(GenerWorkerListAttr.FK_Emp, this.FK_Emp,
                GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.FK_Node);

            //如果已经没有会签待办了,就设置当前人员状态为0.  增加这部分.
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(WorkID) FROM WF_GenerWorkerList WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND IsPass=0 ";
            ps.Add("FK_Node", this.FK_Node);
            ps.Add("WorkID", this.WorkID);
            if (DBAccess.RunSQLReturnValInt(ps) == 0)
            {
                gwf.HuiQianTaskSta = HuiQianTaskSta.None; //设置为 None . 不能设置会签完成,不然其他的就没有办法处理了.
                gwf.Update();
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=0 WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
                ps.Add("FK_Node", this.FK_Node);
                ps.Add("WorkID", this.WorkID);
                ps.AddFK_Emp();
                DBAccess.RunSQL(ps);
            }

            //从待办里移除.
            BP.Port.Emp myemp = new BP.Port.Emp(this.FK_Emp);
            string str = gwf.TodoEmps;
            str = str.Replace(myemp.No + "," + myemp.Name + ";", "");
            str = str.Replace(myemp.Name + ";", "");
            

            addLeader = addLeader.Replace(this.FK_Emp + ",", "");
            gwf.SetPara("AddLeader", addLeader);
            gwf.TodoEmps = str;
            gwf.Update();

            return HuiQian_Init();
        }
        /// <summary>
        /// 增加审核人员
        /// </summary>
        /// <returns></returns>
        public string HuiQian_AddEmps()
        {
            string huiQianType = this.GetRequestVal("HuiQianType");
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            string addLeader = gwf.GetParaString("AddLeader");
            if (gwf.TodoEmps.Contains(WebUser.No + ",") == false)
            {
                //判断是不是第二会签主持人
                if(addLeader.Contains(WebUser.No + ",") == false)
                    return "err@あなたは副署名のホストではありません。この操作は実行できません。";
            }
               

            GenerWorkerList gwlOfMe = new GenerWorkerList();
            int num = gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                 GenerWorkerListAttr.WorkID, this.WorkID,
                 GenerWorkerListAttr.FK_Node, this.FK_Node);

            Node nd = new Node(this.FK_Node);
            if (num == 0)
                return "err@現在のスタッフのワークリストデータは照会されません。";

            string empStrs = this.GetRequestVal("AddEmps");
            if (DataType.IsNullOrEmpty(empStrs) == true)
                return "err@人を選択しませんでした。";


            string err = "";

            string[] emps = empStrs.Split(',');
            foreach (string empStr in emps)
            {
                if (DataType.IsNullOrEmpty(empStr) == true)
                    continue;

                Emp emp = new Emp(empStr);

                //查查是否存在队列里？
                num = gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, emp.No,
                        GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.FK_Node);

                if (num == 1)
                {
                    err += " 人事[" + emp.No + "," + emp.Name + "]すでにキューにあります。";
                    continue;
                }

                //增加组长
                if(DataType.IsNullOrEmpty(huiQianType) == false && huiQianType.Equals("AddLeader"))
                {
                    addLeader += emp.No + ",";
                }

                //查询出来其他列的数据.
                gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                    GenerWorkerListAttr.WorkID, this.WorkID,
                    GenerWorkerListAttr.FK_Node, this.FK_Node);
                gwlOfMe.SetPara("HuiQianType", "");
                gwlOfMe.FK_Emp = emp.No;
                gwlOfMe.FK_EmpText = emp.Name;
                gwlOfMe.IsPassInt = -1; //设置不可以用.
                gwlOfMe.FK_Dept = emp.FK_Dept;
                gwlOfMe.FK_DeptT = emp.FK_DeptText; //部门名称.
                gwlOfMe.IsRead = false;
                gwlOfMe.SetPara("HuiQianZhuChiRen", WebUser.No);
                //表明后增加的组长
                if (DataType.IsNullOrEmpty(huiQianType) == false && huiQianType.Equals("AddLeader"))
                    gwlOfMe.SetPara("HuiQianType", huiQianType);

                #region 计算会签时间.
                if (nd.HisCHWay == CHWay.None)
                {
                    gwlOfMe.SDT = "なし";
                }
                else
                {
                    //给会签人设置应该完成日期. 考虑到了节假日.                
                    DateTime dtOfShould = Glo.AddDayHoursSpan(DateTime.Now, nd.TimeLimit,
                         nd.TimeLimitHH, nd.TimeLimitMM, nd.TWay);
                    //应完成日期.
                    gwlOfMe.SDT = dtOfShould.ToString(DataType.SysDataTimeFormat+":ss");
                }

                //求警告日期.
                DateTime dtOfWarning = DateTime.Now;
                if (nd.WarningDay == 0)
                {
                    //  dtOfWarning = "无";
                }
                else
                {
                    //计算警告日期。
                    // 增加小时数. 考虑到了节假日.
                    dtOfWarning = Glo.AddDayHoursSpan(DateTime.Now, nd.WarningDay, 0, 0, nd.TWay);
                }
                gwlOfMe.DTOfWarning = dtOfWarning.ToString(DataType.SysDataTimeFormat);
                #endregion 计算会签时间.

                gwlOfMe.Sender = BP.Web.WebUser.Name; //发送人为当前人.
                gwlOfMe.IsHuiQian = true;
                gwlOfMe.Insert(); //插入作为待办.

            }

            gwf.SetPara("AddLeader", addLeader);
            gwf.Update();
            if (err.Equals("") == true)
                return "成功を増やす。";

            return "err@" + err;
        }
        #endregion

        #region 与会签相关的.
        // 查询select集合
        public string HuiQian_SelectEmps()
        {
            return AccepterOfGener_SelectEmps();
        }
        /// <summary>
        /// 增加主持人
        /// </summary>
        /// <returns></returns>
        public string HuiQian_AddLeader()
        {
            //生成变量.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

            if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianOver)
            {
                /*只有一个人的情况下, 并且是会签完毕状态，就执行 */
                return "info@現在の作業があなたの待ちに来て、副署作業が完了しました。";
            }
            string leaders = gwf.GetParaString("AddLeader");
           
            //获取加签的人
            GenerWorkerLists gwfs = new GenerWorkerLists();
            gwfs.Retrieve(GenerWorkerListAttr.WorkID, gwf.WorkID,
                GenerWorkerListAttr.FK_Node, gwf.FK_Node, GenerWorkerListAttr.IsPass, -1);
            string empsLeader = "ホストを追加:";
           

            foreach (GenerWorkerList item in gwfs)
            {
                if(leaders.Contains(item.FK_Emp + ","))
                {
                    empsLeader += item.FK_Emp + "," + item.FK_EmpText + ";";
                    //发送消息
                    BP.WF.Dev2Interface.Port_SendMsg(item.FK_Emp,
                       "bpm副署の招待", "HuiQian" + gwf.WorkID + "_" + gwf.FK_Node + "_" + item.FK_Emp, BP.Web.WebUser.Name + "仕事として招待します{" + gwf.Title + "｝ホスト、訪問してください{" + item.SDT + "}前に終了。", "HuiQian", gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);
                }
               
            }
            if (DataType.IsNullOrEmpty(empsLeader) == true)
                return "新しいモデレーターは追加されていません";
            leaders = "('" + leaders.Substring(0, leaders.Length - 1).Replace(",", "','") + "')";
            //恢复他的状态.
            string sql = "UPDATE WF_GenerWorkerList SET IsPass=0 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + this.FK_Node + " AND IsPass=-1 AND FK_Emp In"+ leaders;
            DBAccess.RunSQL(sql);

            gwf.TodoEmps = gwf.TodoEmps + empsLeader;
            gwf.Update();
            return "ホストは成功を増やす";

        }
        /// <summary>
        /// 保存并关闭
        /// </summary>
        /// <returns></returns>
        public string HuiQian_SaveAndClose()
        {
            //生成变量.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

            if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianOver)
            {
                /*只有一个人的情况下, 并且是会签完毕状态，就执行 */
                return "info@現在の作業があなたの待ちに来て、副署作業が完了しました。";
            }

            if (gwf.HuiQianTaskSta == HuiQianTaskSta.None)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT COUNT(WorkID) FROM WF_GenerWorkerList WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND (IsPass=0 OR IsPass=-1) AND FK_Emp!=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
                ps.Add("FK_Node", this.FK_Node);
                ps.Add("WorkID", this.WorkID);
                ps.AddFK_Emp();
                if (DBAccess.RunSQLReturnValInt(ps, 0) == 0)
                    return "close@副署名者を設定していません。テキストボックスに副署名者を入力するか、副署名者を選択してください。";
            }

            //判断当前节点的会签类型.
            Node nd = new Node(gwf.FK_Node);

            //设置当前接单是会签的状态.
            gwf.HuiQianTaskSta = HuiQianTaskSta.HuiQianing; //设置为会签状态.
            if(nd.HuiQianLeaderRole == HuiQianLeaderRole.OnlyOne && nd.TodolistModel == TodolistModel.TeamupGroupLeader)
            {
                
                    gwf.HuiQianZhuChiRen = WebUser.No;
                    gwf.HuiQianZhuChiRenName = WebUser.Name;
               
               
            }
            else
            {
                //多人的组长模式或者协作模式
                if(DataType.IsNullOrEmpty(gwf.HuiQianZhuChiRen) == true)
                    gwf.HuiQianZhuChiRen = gwf.TodoEmps;
            }
            

            //求会签人.
            GenerWorkerLists gwfs = new GenerWorkerLists();
            gwfs.Retrieve(GenerWorkerListAttr.WorkID, gwf.WorkID,
                GenerWorkerListAttr.FK_Node, gwf.FK_Node, GenerWorkerListAttr.IsPass, -1);

            string empsOfHuiQian = "副署名者:";
            foreach (GenerWorkerList item in gwfs)
            {
                empsOfHuiQian += item.FK_Emp + "," + item.FK_EmpText + ";";

                //发送消息
                BP.WF.Dev2Interface.Port_SendMsg(item.FK_Emp,
                   "bpm副署の招待", "HuiQian" + gwf.WorkID + "_" + gwf.FK_Node + "_" + item.FK_Emp, BP.Web.WebUser.Name + "仕事に招待する{" + gwf.Title + "｝副署名については、{" + item.SDT + "}前に終了", "HuiQian", gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);
            }
                

            //改变了节点就把会签状态去掉.
            gwf.HuiQianSendToNodeIDStr = "";
            gwf.TodoEmps = gwf.TodoEmps + empsOfHuiQian;

            gwf.Update();

            string sql = "";

            //是否启用会签待办列表, 如果启用了，主持人会签后就转到了HuiQianList.htm里面了.
            if (BP.WF.Glo.IsEnableHuiQianList == true )
            {
                //设置当前操作人员的状态.
                sql = "UPDATE WF_GenerWorkerList SET IsPass=90 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + this.FK_Node + " AND FK_Emp='" + WebUser.No + "'";
                DBAccess.RunSQL(sql);
            }

            //恢复他的状态.
            sql = "UPDATE WF_GenerWorkerList SET IsPass=0 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + this.FK_Node + " AND IsPass=-1";
            DBAccess.RunSQL(sql);

            //执行会签,写入日志.
            BP.WF.Dev2Interface.WriteTrack(gwf.FK_Flow, gwf.FK_Node, gwf.NodeName, gwf.WorkID, gwf.FID, empsOfHuiQian,
                ActionType.HuiQian, "幹部副署", null);

            string str = "";
            if (nd.TodolistModel == TodolistModel.TeamupGroupLeader)
            {
                /*如果是组长模式.*/
                str = "close@正常に保存されました。\t\nジョブは副署リストに移動され、全員が副署した後にTo Doリストに表示されます。";
                str += "\t\n副署名者を追加または削除する場合は、副署名リストに移動してレコードを探し、操作を実行してください。";

                //删除自己的意见，以防止其他人员看到.
                BP.WF.Dev2Interface.DeleteCheckInfo(gwf.FK_Flow, this.WorkID, gwf.FK_Node);
                return str;
            }

            if (nd.TodolistModel == TodolistModel.Teamup)
            {
                int toNodeID = this.GetRequestValInt("ToNode");
                if (toNodeID == 0)
                    return "Send@[" + nd.Name + "]副署名が正常に実行されました。";

                Node toND = new Node(toNodeID);
                //如果到达的节点是按照接受人来选择,就转向接受人选择器.
                if (toND.HisDeliveryWay == DeliveryWay.BySelected)
                    return "url@Accepter.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&ToNode=" + toNodeID;
                else
                    return "Send@送信操作を実行する";
            }

            return str;
        }
        #endregion

        #region 审核组件.
        /// <summary>
        /// 校验密码
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_CheckPass()
        {
            string sPass = this.GetRequestVal("SPass");
            BP.WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
            if (emp.SPass == sPass)
                return "署名しました";
            return "err@間違ったパスワード";
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_ChangePass()
        {
            string sPass = this.GetRequestVal("SPass");
            string sPass1 = this.GetRequestVal("SPass1");
            string sPass2 = this.GetRequestVal("SPass2");

            BP.WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
            if (emp.SPass == sPass)
                return "古いパスワードエラー";

            if (sPass1.Equals(sPass2) == false)
                return "err@入力された2つのパスワードに矛盾があります";
            emp.SPass = sPass2;
            emp.Update();
            return "パスワードのリセットが完了しました";
        }

        /// <summary>
        /// 初始化审核组件数据.
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_Init()
        {
            if (Web.WebUser.No == null)
                return "err@ログイン情報が失われました。もう一度ログインしてください。";

			//表单库审核组件流程编号为null的异常处理
			if (string.IsNullOrWhiteSpace(this.FK_Flow))
				return null;

			#region 定义变量.
			FrmWorkCheck wcDesc = new FrmWorkCheck(this.FK_Node);
            FrmWorkCheck frmWorkCheck = null;
            FrmAttachmentDBs athDBs = null;
            Nodes nds = new Nodes(this.FK_Flow);
            FrmWorkChecks fwcs = new FrmWorkChecks();
            Node nd = null;
            WorkCheck wc = null;
            Tracks tks = null;
            Track tkDoc = null;
            string nodes = ""; //可以审核的节点.
            bool isCanDo = false;
            bool isExitTb_doc = true;
            DataSet ds = new DataSet();
            DataRow row = null;

            //是不是只读?
            bool isReadonly = false;
            if (this.GetRequestVal("IsReadonly") != null && this.GetRequestVal("IsReadonly").Equals("1"))
                isReadonly = true;
            DataTable nodeEmps = new DataTable();
            FrmWorkCheck fwc = null;
            DataTable dt = null;
            int idx = 0;
            int noneEmpIdx = 0;

            fwcs.Retrieve(NodeAttr.FK_Flow, this.FK_Flow, NodeAttr.Step);
            ds.Tables.Add(wcDesc.ToDataTableField("wcDesc")); //当前的节点审核组件定义，放入ds.

            DataTable tkDt = new DataTable("Tracks");
            tkDt.Columns.Add("NodeID", typeof(int));
            tkDt.Columns.Add("NodeName", typeof(string));
            tkDt.Columns.Add("Msg", typeof(string));
            tkDt.Columns.Add("EmpFrom", typeof(string));
            tkDt.Columns.Add("EmpFromT", typeof(string));
            tkDt.Columns.Add("RDT", typeof(string));
            tkDt.Columns.Add("IsDoc", typeof(bool));
            tkDt.Columns.Add("ParentNode", typeof(int));
            tkDt.Columns.Add("T_NodeIndex", typeof(int));    //节点排列顺序，用于后面的排序
            tkDt.Columns.Add("T_CheckIndex", typeof(int));    //审核人显示顺序，用于后面的排序
            tkDt.Columns.Add("ActionType", typeof(int));
            tkDt.Columns.Add("Tag", typeof(string));
            tkDt.Columns.Add("FWCView", typeof(string));

            //流程附件.
            DataTable athDt = new DataTable("Aths");
            athDt.Columns.Add("NodeID", typeof(int));
            athDt.Columns.Add("MyPK", typeof(string));
            athDt.Columns.Add("FK_FrmAttachment", typeof(string));
            athDt.Columns.Add("FK_MapData", typeof(string));
            athDt.Columns.Add("FileName", typeof(string));
            athDt.Columns.Add("FileExts", typeof(string));
            athDt.Columns.Add("CanDelete", typeof(bool));
            //当前节点的流程数据
            FrmAttachmentDBs frmathdbs = new FrmAttachmentDBs();
            frmathdbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment,
                "ND" + this.FK_Node + "_FrmWorkCheck", FrmAttachmentDBAttr.RefPKVal,
                this.WorkID.ToString(),FrmAttachmentDBAttr.Rec,WebUser.No, FrmAttachmentDBAttr.RDT);

            foreach (FrmAttachmentDB athDB in frmathdbs)
            {
                row = athDt.NewRow();
                row["NodeID"] = this.FK_Node;
                row["MyPK"] = athDB.MyPK;
                row["FK_FrmAttachment"] = athDB.FK_FrmAttachment;
                row["FK_MapData"] = athDB.FK_MapData;
                row["FileName"] = athDB.FileName;
                row["FileExts"] = athDB.FileExts;
                row["CanDelete"] = athDB.Rec == WebUser.No && isReadonly == false;
                athDt.Rows.Add(row);
            }
            ds.Tables.Add(athDt);

            if (this.FID != 0)
                wc = new WorkCheck(this.FK_Flow, this.FK_Node, this.FID, 0);
            else
                wc = new WorkCheck(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);

            //是否只读？
            if (isReadonly == true)
                isCanDo = false;
            else
                isCanDo = BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.WorkID, BP.Web.WebUser.No);

            //如果是查看状态, 为了屏蔽掉正在审批的节点, 在查看审批意见中.
            bool isShowCurrNodeInfo = true;
            GenerWorkFlow gwf = new GenerWorkFlow();
            if (this.WorkID != 0)
            {
                gwf.WorkID = this.WorkID;
                gwf.Retrieve();
            }

            if (isCanDo == false && isReadonly == true)
            {
                if (gwf.WFState == WFState.Runing && gwf.FK_Node == this.FK_Node)
                    isShowCurrNodeInfo = false;
            }

            /*
             * 获得当前节点已经审核通过的人员.
             * 比如：多人处理规则中的已经审核同意的人员，会签人员,组合成成一个字符串。
             * 格式为: ,zhangsan,lisi,
             * 用于处理在审核列表中屏蔽临时的保存的审核信息.
             * 12 为芒果增加一个非正常完成状态.
             * */
            string checkerPassed = ",";
            if (gwf.WFState != WFState.Complete && (int)gwf.WFState != 12)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT FK_Emp FROM WF_Generworkerlist WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND IsPass=1 AND FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node Order By RDT,CDT";
                ps.Add("WorkID", this.WorkID);
                ps.Add("FK_Node", this.FK_Node);
                DataTable checkerPassedDt = DBAccess.RunSQLReturnTable(ps);
                foreach (DataRow dr in checkerPassedDt.Rows)
                {
                    checkerPassed += dr["FK_Emp"] + ",";
                }
            }

            #endregion 定义变量.

            #region 判断是否显示 - 历史审核信息显示
            bool isDoc = false;
            if (wcDesc.FWCListEnable == true)
            {
                tks = wc.HisWorkChecks;

                //已走过节点
                int empIdx = 0;
                int lastNodeId = 0;
                foreach (BP.WF.Track tk in tks)
                {
                    if (tk.HisActionType == ActionType.FlowBBS)
                        continue;

                    if (lastNodeId == 0)
                        lastNodeId = tk.NDFrom;

                    if (lastNodeId != tk.NDFrom)
                    {
                        idx++;
                        lastNodeId = tk.NDFrom;
                    }

                    tk.Row.Add("T_NodeIndex", idx);

                    nd = nds.GetEntityByKey(tk.NDFrom) as Node;
                    if (nd == null)
                        continue;

                    fwc = fwcs.GetEntityByKey(tk.NDFrom) as FrmWorkCheck;
                    //求出主键
                    long pkVal = this.WorkID;
                    if (nd.HisRunModel == RunModel.SubThread)
                        pkVal = this.FID;

                    //排序，结合人员表Idx进行排序
                    if (fwc.FWCOrderModel == FWCOrderModel.SqlAccepter)
                    {
                        tk.Row["T_CheckIndex"] =
                            DBAccess.RunSQLReturnValInt(
                                string.Format("SELECT Idx FROM Port_Emp WHERE No='{0}'", tk.EmpFrom), 0);
                        noneEmpIdx++;
                    }
                    else
                    {
                        tk.Row["T_CheckIndex"] = noneEmpIdx++;
                    }
                    switch (tk.HisActionType)
                    {
                        case ActionType.WorkCheck:
                        case ActionType.StartChildenFlow:
                            if (nodes.Contains(tk.NDFrom + ",") == false)
                                nodes += tk.NDFrom + ",";
                            break;
                        case ActionType.Return:
                            if (wcDesc.FWCIsShowReturnMsg == true && tk.NDTo == this.FK_Node)
                            {
                                if (nodes.Contains(tk.NDFrom + ",") == false)
                                    nodes += tk.NDFrom + ",";
                            }
                            break;
                        default:
                            continue;
                    }
                }

                foreach (Track tk in tks)
                {
                    if (nodes.Contains(tk.NDFrom + ",") == false)
                        continue;


                    if (tk.HisActionType != ActionType.WorkCheck && tk.HisActionType != ActionType.StartChildenFlow && tk.HisActionType != ActionType.Return)
                        continue;


                    //退回
                    if (tk.HisActionType== ActionType.Return)
                    {
                        //1.不显示退回意见 2.显示退回意见但是不是退回给本节点的意见
                        if (wcDesc.FWCIsShowReturnMsg == false || (wcDesc.FWCIsShowReturnMsg == true && tk.NDTo != this.FK_Node))
                            continue;
                    }




                    //如果是当前的节点. 当前人员可以处理, 已经审批通过的人员.
                    if (tk.NDFrom == this.FK_Node
                        && isCanDo == true
                        && tk.EmpFrom != WebUser.No
                        && checkerPassed.Contains("," + tk.EmpFrom + ",") == false)
                        continue;


                    if (tk.NDFrom == this.FK_Node && gwf.HuiQianTaskSta != HuiQianTaskSta.None)
                    {
                        //判断会签, 去掉正在审批的节点.
                        if (tk.NDFrom == this.FK_Node && isShowCurrNodeInfo == false)
                            continue;
                    }

                    //如果是多人处理，就让其显示已经审核过的意见.
                    if (tk.NDFrom == this.FK_Node
                        && checkerPassed.IndexOf("," + tk.EmpFrom + ",") < 0
                        && (gwf.WFState != WFState.Complete && (int)gwf.WFState != 12))
                    {
                        continue;
                        //如果当前人，没有审核完成,就不显示.
                        //判断会签, 去掉正在审批的节点.
                        // if (tk.NDFrom == this.FK_Node)
                        //   continue;
                    }


                    row = tkDt.NewRow();
                    row["NodeID"] = tk.NDFrom;

                    row["NodeName"] = tk.NDFromT;
                    fwc = fwcs.GetEntityByKey(tk.NDFrom) as FrmWorkCheck;

                    // zhoupeng 增加了判断，在会签的时候最后会签人发送前不能填写意见.
                    if (tk.NDFrom == this.FK_Node && tk.EmpFrom == BP.Web.WebUser.No && isCanDo && isDoc == false)
                    {
                        isDoc = true;
                        row["IsDoc"] = true;
                    }
                    else
                        row["IsDoc"] = false;


                    row["ParentNode"] = 0;
                    row["RDT"] = DataType.IsNullOrEmpty(tk.RDT) ? "" : tk.NDFrom == tk.NDTo && DataType.IsNullOrEmpty(tk.Msg) ? "" : tk.RDT;
                    row["T_NodeIndex"] = tk.Row["T_NodeIndex"];
                    row["T_CheckIndex"] = tk.Row["T_CheckIndex"];

                    if (isReadonly == false && tk.EmpFrom == WebUser.No && this.FK_Node == tk.NDFrom && isExitTb_doc && (
                                        wcDesc.HisFrmWorkCheckType == FWCType.Check || (
                                        (wcDesc.HisFrmWorkCheckType == FWCType.DailyLog || wcDesc.HisFrmWorkCheckType == FWCType.WeekLog)
                                        && DateTime.Parse(tk.RDT).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                                        || (wcDesc.HisFrmWorkCheckType == FWCType.MonthLog
                                        && DateTime.Parse(tk.RDT).ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM"))
                                        ))
                    {
                        bool isLast = true;
                        foreach (Track tk1 in tks)
                        {
                            if (tk1.HisActionType == tk.HisActionType
                                && tk1.NDFrom == tk.NDFrom
                                && tk1.RDT.CompareTo(tk.RDT) > 0)
                            {
                                isLast = false;
                                break;
                            }
                        }

                        if (isLast && isDoc == false && gwf.WFState != WFState.Complete)
                        {
                            isExitTb_doc = false;
                            row["IsDoc"] = true;
                            isDoc = true;
                            row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node, wcDesc.FWCDefInfo);
                            tkDoc = tk;
                        }
                        else
                        {
                            row["Msg"] = tk.MsgHtml;
                        }
                    }
                    else
                    {
                        row["Msg"] = tk.MsgHtml;
                    }

                    row["EmpFrom"] = tk.EmpFrom;
                    row["EmpFromT"] = tk.EmpFromT;
                    row["ActionType"] = tk.HisActionType;
                    row["Tag"] = tk.Tag;
                    row["FWCView"] = fwc.FWCView;
                    tkDt.Rows.Add(row);

                    #region //审核组件附件数据
                    athDBs = new FrmAttachmentDBs();
                    QueryObject obj_Ath = new QueryObject(athDBs);
                    obj_Ath.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, "ND" + tk.NDFrom + "_FrmWorkCheck");
                    obj_Ath.addAnd();
                    obj_Ath.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString()); //@sly
                    obj_Ath.addAnd();
                    obj_Ath.AddWhere(FrmAttachmentDBAttr.Rec, tk.EmpFrom);
                    obj_Ath.addOrderBy(FrmAttachmentDBAttr.RDT);
                    obj_Ath.DoQuery();

                    foreach (FrmAttachmentDB athDB in athDBs)
                    {
                        row = athDt.NewRow();
                        row["NodeID"] = tk.NDFrom;
                        row["MyPK"] = athDB.MyPK;
                        row["FK_FrmAttachment"] = athDB.FK_FrmAttachment;
                        row["FK_MapData"] = athDB.FK_MapData;
                        row["FileName"] = athDB.FileName;
                        row["FileExts"] = athDB.FileExts;
                        row["CanDelete"] = athDB.FK_MapData == this.FK_Node.ToString() && athDB.Rec == WebUser.No && isReadonly == false;
                        athDt.Rows.Add(row);
                    }
                    #endregion

                    #region //子流程的审核组件数据
                    if (tk.FID != 0 && tk.HisActionType == ActionType.StartChildenFlow && tkDt.Select("ParentNode=" + tk.NDFrom).Length == 0)
                    {
                        string[] paras = tk.Tag.Split('@');
                        string[] p1 = paras[1].Split('=');
                        string fk_flow = p1[1]; //子流程编号

                        string[] p2 = paras[2].Split('=');
                        string workId = p2[1]; //子流程ID.
                        int biaoji = 0;

                        WorkCheck subwc = new WorkCheck(fk_flow, int.Parse(fk_flow + "01"), Int64.Parse(workId), 0);

                        Tracks subtks = subwc.HisWorkChecks;
                        //取出来子流程的所有的节点。
                        Nodes subNds = new Nodes(fk_flow);
                        foreach (Node item in subNds)     //主要按顺序显示
                        {
                            foreach (Track mysubtk in subtks)
                            {
                                if (item.NodeID != mysubtk.NDFrom)
                                    continue;

                                /*输出该子流程的审核信息，应该考虑子流程的子流程信息, 就不考虑那样复杂了.*/
                                if (mysubtk.HisActionType == ActionType.WorkCheck)
                                {
                                    // 发起多个子流程时，发起人只显示一次
                                    if (mysubtk.NDFrom == int.Parse(fk_flow + "01") && biaoji == 1)
                                        continue;
                                    FrmWorkCheck subFrmCheck = new FrmWorkCheck("ND"+mysubtk.NDFrom);
                                    row = tkDt.NewRow();
                                    row["NodeID"] = mysubtk.NDFrom;
                                    row["NodeName"] = string.Format("(サブフロー){0}", mysubtk.NDFromT);
                                    row["Msg"] = mysubtk.MsgHtml;
                                    row["EmpFrom"] = mysubtk.EmpFrom;
                                    row["EmpFromT"] = mysubtk.EmpFromT;
                                    row["RDT"] = mysubtk.RDT;
                                    row["IsDoc"] = false;
                                    row["ParentNode"] = tk.NDFrom;
                                    row["T_NodeIndex"] = idx++;
                                    row["T_CheckIndex"] = noneEmpIdx++;
                                    row["ActionType"] = mysubtk.HisActionType;
                                    row["Tag"] = mysubtk.Tag;
                                    row["FWCView"] = subFrmCheck.FWCView;
                                    tkDt.Rows.Add(row);

                                    if (mysubtk.NDFrom == int.Parse(fk_flow + "01"))
                                    {
                                        biaoji = 1;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                }

#warning 处理审核信息,删除掉他.
                if (tkDoc != null && 1 == 2)
                {
                    //判断可编辑审核信息是否处于最后一条，不处于最后一条，则将其移到最后一条
                    DataRow rdoc = tkDt.Select("IsDoc=True")[0];
                    if (tkDt.Rows.IndexOf(rdoc) != tkDt.Rows.Count - 1)
                    {
                        tkDt.Rows.Add(rdoc.ItemArray)["RDT"] = "";

                        rdoc["IsDoc"] = false;
                        rdoc["RDT"] = tkDoc.RDT;
                        rdoc["Msg"] = tkDoc.MsgHtml;
                    }
                    else
                    {
                        //判断刚退回时，退回接收人一打开，审核信息复制一条
                        Track lastTrack = tks[tks.Count - 1] as Track;
                        if ((lastTrack.HisActionType == ActionType.Return || lastTrack.HisActionType == ActionType.Forward)
                            && lastTrack.NDTo == tkDoc.NDFrom)
                        {
                            //  tkDt.Rows.Add(rdoc.ItemArray)["RDT"] = "";
                            //   rdoc["IsDoc"] = false;
                            //    rdoc["RDT"] = tkDoc.RDT;
                            //     rdoc["Msg"] = tkDoc.MsgHtml;
                        }
                    }
                }
            }
            #endregion 判断是否显示 - 历史审核信息显示

            #region 审核意见默认填写

            //首先判断当前是否有此意见? 如果是退回的该信息已经存在了.
            bool isHaveMyInfo = false;
            foreach (DataRow dr in tkDt.Rows)
            {
                string fk_node = dr["NodeID"].ToString();
                string empFrom = dr["EmpFrom"].ToString();
                if (int.Parse(fk_node) == this.FK_Node && empFrom == Web.WebUser.No)
                    isHaveMyInfo = true;
            }

            // 增加默认的审核意见.
            if (isExitTb_doc && wcDesc.HisFrmWorkCheckSta == FrmWorkCheckSta.Enable && isCanDo
                && isReadonly == false && isHaveMyInfo == false)
            {
                DataRow[] rows = null;
                nd = nds.GetEntityByKey(this.FK_Node) as Node;
                if (wcDesc.FWCOrderModel == FWCOrderModel.SqlAccepter)
                {
                    rows = tkDt.Select("NodeID=" + this.FK_Node + " AND Msg='' AND EmpFrom='" + WebUser.No + "'");

                    if (rows.Length == 0)
                        rows = tkDt.Select("NodeID=" + this.FK_Node + " AND EmpFrom='" + WebUser.No + "'", "RDT DESC");

                    if (rows.Length > 0)
                    {
                        row = rows[0];
                        row["IsDoc"] = true;

                        row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node, wcDesc.FWCDefInfo);
                        if (row["Msg"].ToString().Equals(""))
                            row["RDT"] = "";

                    }
                    else
                    {
                        row = tkDt.NewRow();
                        row["NodeID"] = this.FK_Node;
                        row["NodeName"] = nd.FWCNodeName;
                        row["IsDoc"] = true;
                        row["ParentNode"] = 0;
                        row["RDT"] = "";

                        row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node, wcDesc.FWCDefInfo);
                        row["EmpFrom"] = WebUser.No;
                        row["EmpFromT"] = WebUser.Name;
                        row["T_NodeIndex"] = ++idx;
                        row["T_CheckIndex"] = ++noneEmpIdx;
                        row["ActionType"] = ActionType.Forward;
                        row["Tag"] = Dev2Interface.GetCheckTag(this.FK_Flow, this.WorkID, this.FK_Node, WebUser.No); 
                        tkDt.Rows.Add(row);
                    }
                }
                else
                {
                    row = tkDt.NewRow();
                    row["NodeID"] = this.FK_Node;
                    row["NodeName"] = nd.FWCNodeName;
                    row["IsDoc"] = true;
                    row["ParentNode"] = 0;
                    row["RDT"] = "";

                    row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node, wcDesc.FWCDefInfo);
                    row["EmpFrom"] = WebUser.No;
                    row["EmpFromT"] = WebUser.Name;
                    row["T_NodeIndex"] = ++idx;
                    row["T_CheckIndex"] = ++noneEmpIdx;
                    row["ActionType"] = ActionType.Forward;
                    row["Tag"] = Dev2Interface.GetCheckTag(this.FK_Flow, this.WorkID, this.FK_Node, WebUser.No);
                    tkDt.Rows.Add(row);
                }
            }
            #endregion

            #region 显示有审核组件，但还未审核的节点. 包括退回后的.
            if (tks == null)
                tks = wc.HisWorkChecks;

            foreach (FrmWorkCheck item in fwcs)
            {
                if (item.FWCIsShowTruck == false)
                    continue;  //不需要显示历史记录.

                //是否已审核.
                bool isHave = false;
                foreach (BP.WF.Track tk in tks)
                {
                    //翻译.
                    if (tk.NDFrom == this.FK_Node && tk.HisActionType == ActionType.WorkCheck)
                    {
                        isHave = true; //已经有了
                        break;
                    }
                }

                if (isHave == true)
                    continue;

                row = tkDt.NewRow();
                row["NodeID"] = item.NodeID;

                Node mynd = (Node)nds.GetEntityByKey(item.NodeID);
                row["NodeName"] = mynd.FWCNodeName;
                row["IsDoc"] = false;
                row["ParentNode"] = 0;
                row["RDT"] = "";
                row["Msg"] = "&nbsp;";
                row["EmpFrom"] = "";
                row["EmpFromT"] = "";
                row["T_NodeIndex"] = ++idx;
                row["T_CheckIndex"] = ++noneEmpIdx;

                tkDt.Rows.Add(row);
            }
            #endregion 增加空白.

            ds.Tables.Add(tkDt);


            //如果有 SignType 列就获得签名信息.
            if (SystemConfig.CustomerNo == "TianYe")
            {
                string tTable = "ND" + int.Parse(FK_Flow) + "Track";
                string sql = "SELECT distinct a.No, a.SignType, a.EleID FROM Port_Emp a, " + tTable + " b WHERE (A.No='" + WebUser.No + "') OR B.ActionType=22 AND a.No=b.EmpFrom AND B.WorkID=" + this.WorkID;

                DataTable dtTrack = DBAccess.RunSQLReturnTable(sql);
                dtTrack.TableName = "SignType";

                dtTrack.Columns["NO"].ColumnName = "No";
                dtTrack.Columns["SIGNTYPE"].ColumnName = "SignType";
                dtTrack.Columns["ELEID"].ColumnName = "EleID";

                ds.Tables.Add(dtTrack);
            }

            string str = BP.Tools.Json.ToJson(ds);

            return str;
        }
        /// <summary>
        /// 初始化审核组件数据.
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_Init2019()
        {
            if (Web.WebUser.No == null)
                return "err@ログイン情報が失われました。もう一度ログインしてください。";

            #region 定义变量.
            FrmWorkCheck wcDesc = new FrmWorkCheck(this.FK_Node); // 当前节点的审核组件
            FrmWorkCheck frmWorkCheck = null;
            FrmAttachmentDBs athDBs = null;    //附件数据
            Nodes nds = new Nodes(this.FK_Flow); //该流程的所有节点
            FrmWorkChecks fwcs = new FrmWorkChecks();
            Node nd = null;
            WorkCheck wc = null;
            Tracks tks = null;
            Track tkDoc = null;
            string nodes = ""; //已经审核过的节点.
            bool isCanDo = false;//是否可以审批
            bool isExitTb_doc = true;
            DataSet ds = new DataSet();
            DataRow row = null;

            //是不是只读?
            bool isReadonly = false;
            if (this.GetRequestVal("IsReadonly") != null && this.GetRequestVal("IsReadonly").Equals("1"))
                isReadonly = true;

            DataTable nodeEmps = new DataTable();
            FrmWorkCheck fwc = null;
            DataTable dt = null;
            int idx = 0;
            int noneEmpIdx = 0;

            fwcs.Retrieve(NodeAttr.FK_Flow, this.FK_Flow, NodeAttr.Step);
            ds.Tables.Add(wcDesc.ToDataTableField("wcDesc")); //当前的节点审核组件定义，放入ds.

            DataTable tkDt = new DataTable("Tracks");
            tkDt.Columns.Add("NodeID", typeof(int));
            tkDt.Columns.Add("NodeName", typeof(string));
            tkDt.Columns.Add("Msg", typeof(string));
            tkDt.Columns.Add("EmpFrom", typeof(string));
            tkDt.Columns.Add("EmpFromT", typeof(string));
            tkDt.Columns.Add("RDT", typeof(string));
            tkDt.Columns.Add("IsDoc", typeof(bool));
            tkDt.Columns.Add("ParentNode", typeof(int));
            tkDt.Columns.Add("T_NodeIndex", typeof(int));    //节点排列顺序，用于后面的排序
            tkDt.Columns.Add("T_CheckIndex", typeof(int));    //审核人显示顺序，用于后面的排序
            tkDt.Columns.Add("ActionType", typeof(int));
            tkDt.Columns.Add("Tag", typeof(string));
            tkDt.Columns.Add("FWCView", typeof(string));

            //流程附件.
            DataTable athDt = new DataTable("Aths");
            athDt.Columns.Add("NodeID", typeof(int));
            athDt.Columns.Add("MyPK", typeof(string));
            athDt.Columns.Add("FK_FrmAttachment", typeof(string));
            athDt.Columns.Add("FK_MapData", typeof(string));
            athDt.Columns.Add("FileName", typeof(string));
            athDt.Columns.Add("FileExts", typeof(string));
            athDt.Columns.Add("CanDelete", typeof(bool));
            //当前节点的流程数据
            FrmAttachmentDBs frmathdbs = new FrmAttachmentDBs();
            frmathdbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, "ND" + this.FK_Node + "_FrmWorkCheck",
                FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString(),"Rec",WebUser.No, FrmAttachmentDBAttr.RDT);
            
            foreach (FrmAttachmentDB athDB in frmathdbs)
            {
                row = athDt.NewRow();
                row["NodeID"] = this.FK_Node;
                row["MyPK"] = athDB.MyPK;
                row["FK_FrmAttachment"] = athDB.FK_FrmAttachment;
                row["FK_MapData"] = athDB.FK_MapData;
                row["FileName"] = athDB.FileName;
                row["FileExts"] = athDB.FileExts;
                row["CanDelete"] =athDB.Rec == WebUser.No && isReadonly == false;
                athDt.Rows.Add(row);
            }
            ds.Tables.Add(athDt);

            if (this.FID != 0)
                wc = new WorkCheck(this.FK_Flow, this.FK_Node, this.FID, 0);
            else
                wc = new WorkCheck(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);

            //是否只读？
            if (isReadonly == true)
                isCanDo = false;
            else
                isCanDo = BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.WorkID, BP.Web.WebUser.No);

            //如果是查看状态, 为了屏蔽掉正在审批的节点, 在查看审批意见中.
            bool isShowCurrNodeInfo = true;
            GenerWorkFlow gwf = new GenerWorkFlow();
            if (this.WorkID != 0)
            {
                gwf.WorkID = this.WorkID;
                gwf.Retrieve();
            }

            if (isCanDo == false && isReadonly == true)
            {
                if (gwf.WFState == WFState.Runing && gwf.FK_Node == this.FK_Node)
                    isShowCurrNodeInfo = false;
            }

            /*
             * 获得当前节点已经审核通过的人员.
             * 比如：多人处理规则中的已经审核同意的人员，会签人员,组合成成一个字符串。
             * 格式为: ,zhangsan,lisi,
             * 用于处理在审核列表中屏蔽临时的保存的审核信息.
             * 12 为芒果增加一个非正常完成状态.
             * */
            string checkerPassed = ",";
            if (gwf.WFState != WFState.Complete && (int)gwf.WFState != 12)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT FK_Emp FROM WF_Generworkerlist WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND IsPass=1 AND FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node  Order By RDT,CDT";
                ps.Add("WorkID", this.WorkID);
                ps.Add("FK_Node", this.FK_Node);
                DataTable checkerPassedDt = DBAccess.RunSQLReturnTable(ps);
                foreach (DataRow dr in checkerPassedDt.Rows)
                {
                    checkerPassed += dr["FK_Emp"] + ",";
                }
            }

            #endregion 定义变量.

            #region 判断是否显示 - 历史审核信息显示
            bool isDoc = false;
            if (wcDesc.FWCListEnable == true)
            {
                tks = wc.HisWorkChecks;

                //已走过节点
                int empIdx = 0;
                int lastNodeId = 0;
                foreach (BP.WF.Track tk in tks)
                {
                    if (tk.HisActionType == ActionType.FlowBBS)
                        continue;

                    if (lastNodeId == 0)
                        lastNodeId = tk.NDFrom;

                    if (lastNodeId != tk.NDFrom)
                    {
                        idx++;
                        lastNodeId = tk.NDFrom;
                    }

                    tk.Row.Add("T_NodeIndex", idx);

                    nd = nds.GetEntityByKey(tk.NDFrom) as Node;
                    if (nd == null)
                        continue;

                    fwc = fwcs.GetEntityByKey(tk.NDFrom) as FrmWorkCheck;
                    //求出主键
                    long pkVal = this.WorkID;
                    if (nd.HisRunModel == RunModel.SubThread)
                        pkVal = this.FID;

                    //排序，结合人员表Idx进行排序
                    if (fwc.FWCOrderModel == FWCOrderModel.SqlAccepter)
                    {
                        tk.Row["T_CheckIndex"] =
                            DBAccess.RunSQLReturnValInt(
                                string.Format("SELECT Idx FROM Port_Emp WHERE No='{0}'", tk.EmpFrom), 0);
                        noneEmpIdx++;
                    }
                    else
                    {
                        tk.Row["T_CheckIndex"] = noneEmpIdx++;
                    }
                    switch (tk.HisActionType)
                    {
                        case ActionType.WorkCheck:
                        case ActionType.StartChildenFlow:
                            if (nodes.Contains(tk.NDFrom + ",") == false)
                                nodes += tk.NDFrom + ",";
                            break;
                        case ActionType.Return:
                            if (wcDesc.FWCIsShowReturnMsg == true && tk.NDTo== this.FK_Node )
                            {
                                if (nodes.Contains(tk.NDFrom + ",") == false)
                                    nodes += tk.NDFrom + ",";
                            }
                            break;
                        default:
                            continue;
                    }
                }

                foreach (Track tk in tks)
                {
                    if (nodes.Contains(tk.NDFrom + ",") == false)
                        continue;


                    //退回
                    if (tk.HisActionType == ActionType.Return)
                    {
                        //1.不显示退回意见 2.显示退回意见但是不是退回给本节点的意见
                        if (wcDesc.FWCIsShowReturnMsg == false || (wcDesc.FWCIsShowReturnMsg== true && tk.NDTo != this.FK_Node))
                            continue;
                    }

                    //如果是当前的节点. 当前人员可以处理, 已经审批通过的人员.
                    if (tk.NDFrom == this.FK_Node && isCanDo == true&& tk.EmpFrom != WebUser.No && checkerPassed.Contains("," + tk.EmpFrom + ",") == false)
                        continue;

                    if (tk.NDFrom == this.FK_Node && gwf.HuiQianTaskSta != HuiQianTaskSta.None) 
                    {
                        //判断会签, 去掉正在审批的节点.
                        if (tk.NDFrom == this.FK_Node && isShowCurrNodeInfo == false)
                            continue;
                    }
                    //如果是多人处理，就让其显示已经审核过的意见.
                    //if (tk.NDFrom == this.FK_Node&& checkerPassed.IndexOf("," + tk.EmpFrom + ",") < 0 && (gwf.WFState != WFState.Complete && (int)gwf.WFState != 12))
                    //    continue;

                    fwc = fwcs.GetEntityByKey(tk.NDFrom) as FrmWorkCheck;

                    //历史审核信息现在存放在流程前进的节点中
                    switch (tk.HisActionType)
                    {
                        case ActionType.Forward:
                        case ActionType.ForwardAskfor:
                        case ActionType.Start:
                        case ActionType.UnSend:
                        //case ActionType.ForwardFL:
                        case ActionType.ForwardHL:
                        case ActionType.TeampUp:
                        case ActionType.Return:
                        case ActionType.StartChildenFlow:
                        case ActionType.FlowOver:
                            row = tkDt.NewRow();
                            row["NodeID"] = tk.NDFrom;
                            row["NodeName"] = tk.NDFromT;

                            // zhoupeng 增加了判断，在会签的时候最后会签人发送前不能填写意见.
                            if (tk.NDFrom == this.FK_Node && tk.EmpFrom == BP.Web.WebUser.No && isCanDo && isDoc == false )
                            {
                                //@yuan 修改测试
                                isDoc = true;
                             
                            }
                          
                           row["IsDoc"] = false;

                            row["ParentNode"] = 0;
                            row["RDT"] = DataType.IsNullOrEmpty(tk.RDT) ? "" : tk.NDFrom == tk.NDTo && DataType.IsNullOrEmpty(tk.Msg) ? "" : tk.RDT;
                            row["T_NodeIndex"] = tk.Row["T_NodeIndex"];
                            row["T_CheckIndex"] = tk.Row["T_CheckIndex"];
                       
                            if(gwf.WFState == WFState.Complete)
                            {
                                row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node, wcDesc.FWCDefInfo);
                            }
                            else
                            {
                                row["Msg"] = tk.MsgHtml;
                            }

                            row["EmpFrom"] = tk.EmpFrom;
                            row["EmpFromT"] = tk.EmpFromT;
                            row["ActionType"] = tk.HisActionType;
                            row["Tag"] = tk.Tag;
                            row["FWCView"] = fwc.FWCView;
                            tkDt.Rows.Add(row);

                            #region //审核组件附件数据
                            athDBs = new FrmAttachmentDBs();
                            QueryObject obj_Ath = new QueryObject(athDBs);
                            obj_Ath.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, "ND" + tk.NDFrom + "_FrmWorkCheck");
                            obj_Ath.addAnd();
                            obj_Ath.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());
                            obj_Ath.addAnd();
                            obj_Ath.AddWhere(FrmAttachmentDBAttr.Rec, tk.EmpFrom);
                            obj_Ath.addOrderBy(FrmAttachmentDBAttr.RDT);
                            obj_Ath.DoQuery();

                            foreach (FrmAttachmentDB athDB in athDBs)
                            {
                                row = athDt.NewRow();
                                row["NodeID"] = tk.NDFrom;
                                row["MyPK"] = athDB.MyPK;
                                row["FK_FrmAttachment"] = athDB.FK_FrmAttachment;
                                row["FK_MapData"] = athDB.FK_MapData;
                                row["FileName"] = athDB.FileName;
                                row["FileExts"] = athDB.FileExts;
                                row["CanDelete"] = athDB.FK_MapData == this.FK_Node.ToString() && athDB.Rec == WebUser.No && isReadonly == false;
                                athDt.Rows.Add(row);
                            }
                            #endregion

                            #region //子流程的审核组件数据
                            if (tk.FID != 0 && tk.HisActionType == ActionType.StartChildenFlow && tkDt.Select("ParentNode=" + tk.NDFrom).Length == 0)
                            {
                                string[] paras = tk.Tag.Split('@');
                                string[] p1 = paras[1].Split('=');
                                string fk_flow = p1[1]; //子流程编号

                                string[] p2 = paras[2].Split('=');
                                string workId = p2[1]; //子流程ID.
                                int biaoji = 0;

                                WorkCheck subwc = new WorkCheck(fk_flow, int.Parse(fk_flow + "01"), Int64.Parse(workId), 0);

                                Tracks subtks = subwc.HisWorkChecks;
                                //取出来子流程的所有的节点。
                                Nodes subNds = new Nodes(fk_flow);
                                foreach (Node item in subNds)     //主要按顺序显示
                                {
                                    foreach (Track mysubtk in subtks)
                                    {
                                        if (item.NodeID != mysubtk.NDFrom)
                                            continue;

                                        /*输出该子流程的审核信息，应该考虑子流程的子流程信息, 就不考虑那样复杂了.*/
                                        if (mysubtk.HisActionType == ActionType.WorkCheck)
                                        {
                                            // 发起多个子流程时，发起人只显示一次
                                            if (mysubtk.NDFrom == int.Parse(fk_flow + "01") && biaoji == 1)
                                                continue;

                                            row = tkDt.NewRow();
                                            row["NodeID"] = mysubtk.NDFrom;
                                            row["NodeName"] = string.Format("(サブフロー){0}", mysubtk.NDFromT);
                                            row["Msg"] = mysubtk.MsgHtml;
                                            row["EmpFrom"] = mysubtk.EmpFrom;
                                            row["EmpFromT"] = mysubtk.EmpFromT;
                                            row["RDT"] = mysubtk.RDT;
                                            row["IsDoc"] = false;
                                            row["ParentNode"] = tk.NDFrom;
                                            row["T_NodeIndex"] = idx++;
                                            row["T_CheckIndex"] = noneEmpIdx++;
                                            row["ActionType"] = mysubtk.HisActionType;
                                            row["Tag"] = mysubtk.Tag;
                                            tkDt.Rows.Add(row);

                                            if (mysubtk.NDFrom == int.Parse(fk_flow + "01"))
                                            {
                                                biaoji = 1;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                            break;
                        default: break;
                    }

                }

                #warning 处理审核信息,删除掉他.
                if (tkDoc != null && 1 == 2)
                {
                    //判断可编辑审核信息是否处于最后一条，不处于最后一条，则将其移到最后一条
                    DataRow rdoc = tkDt.Select("IsDoc=True")[0];
                    if (tkDt.Rows.IndexOf(rdoc) != tkDt.Rows.Count - 1)
                    {
                        tkDt.Rows.Add(rdoc.ItemArray)["RDT"] = "";

                        rdoc["IsDoc"] = false;
                        rdoc["RDT"] = tkDoc.RDT;
                        rdoc["Msg"] = tkDoc.MsgHtml;
                    }
                }
            }
            #endregion 判断是否显示 - 历史审核信息显示

            #region 审核意见默认填写

            //首先判断当前是否有此意见? 如果是退回的该信息已经存在了.
            bool isHaveMyInfo = false;
            //foreach (DataRow dr in tkDt.Rows)
            //{
            //    string fk_node = dr["NodeID"].ToString();
            //    string empFrom = dr["EmpFrom"].ToString();
            //    if (int.Parse(fk_node) == this.FK_Node && empFrom == Web.WebUser.No)
            //        isHaveMyInfo = true;
            //}

            // 增加默认的审核意见.
            if (isExitTb_doc && wcDesc.HisFrmWorkCheckSta == FrmWorkCheckSta.Enable && isCanDo
                && isReadonly == false && isHaveMyInfo == false)
            {
                DataRow[] rows = null;
                nd = nds.GetEntityByKey(this.FK_Node) as Node;
                if (wcDesc.FWCOrderModel == FWCOrderModel.SqlAccepter)
                {
                    rows = tkDt.Select("NodeID=" + this.FK_Node + " AND Msg='' AND EmpFrom='" + WebUser.No + "'");

                    if (rows.Length == 0)
                        rows = tkDt.Select("NodeID=" + this.FK_Node + " AND EmpFrom='" + WebUser.No + "'", "RDT DESC");

                    if (rows.Length > 0)
                    {
                        row = rows[0];
                        row["IsDoc"] = true;

                        row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node, wcDesc.FWCDefInfo);
                        if (row["Msg"].ToString().Equals(""))
                            row["RDT"] = "";

                    }
                    else
                    {
                        row = tkDt.NewRow();
                        row["NodeID"] = this.FK_Node;
                        row["NodeName"] = nd.FWCNodeName;
                        row["IsDoc"] = true;
                        row["ParentNode"] = 0;
                        row["RDT"] = "";

                        row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node, wcDesc.FWCDefInfo);
                        row["EmpFrom"] = WebUser.No;
                        row["EmpFromT"] = WebUser.Name;
                        row["T_NodeIndex"] = ++idx;
                        row["T_CheckIndex"] = ++noneEmpIdx;
                        row["ActionType"] = ActionType.Forward;
                        row["Tag"] = Dev2Interface.GetCheckTag(this.FK_Flow, this.WorkID, this.FK_Node,WebUser.No);
                        tkDt.Rows.Add(row);
                    }
                }
                else
                {
                    row = tkDt.NewRow();
                    row["NodeID"] = this.FK_Node;
                    row["NodeName"] = nd.FWCNodeName;
                    row["IsDoc"] = true;
                    row["ParentNode"] = 0;
                    row["RDT"] = "";

                    row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node, wcDesc.FWCDefInfo);
                    row["EmpFrom"] = WebUser.No;
                    row["EmpFromT"] = WebUser.Name;
                    row["T_NodeIndex"] = ++idx;
                    row["T_CheckIndex"] = ++noneEmpIdx;
                    row["ActionType"] = ActionType.Forward;
                    row["Tag"] = Dev2Interface.GetCheckTag(this.FK_Flow, this.WorkID, this.FK_Node, WebUser.No);
                    tkDt.Rows.Add(row);
                }
            }
            #endregion

            #region 显示有审核组件，但还未审核的节点. 包括退回后的.
            if (tks == null)
                tks = wc.HisWorkChecks;

            foreach (FrmWorkCheck item in fwcs)
            {
                if (item.FWCIsShowTruck == false)
                    continue;  //不需要显示历史记录.

                //是否已审核.
                bool isHave = false;
                foreach (BP.WF.Track tk in tks)
                {
                    //翻译.
                    if (tk.NDFrom == this.FK_Node && tk.HisActionType == ActionType.WorkCheck)
                    {
                        isHave = true; //已经有了
                        break;
                    }
                }

                if (isHave == true)
                    continue;

                row = tkDt.NewRow();
                row["NodeID"] = item.NodeID;

                Node mynd = (Node)nds.GetEntityByKey(item.NodeID);
                row["NodeName"] = mynd.FWCNodeName;
                row["IsDoc"] = false;
                row["ParentNode"] = 0;
                row["RDT"] = "";
                row["Msg"] = "&nbsp;";
                row["EmpFrom"] = "";
                row["EmpFromT"] = "";
                row["T_NodeIndex"] = ++idx;
                row["T_CheckIndex"] = ++noneEmpIdx;

                tkDt.Rows.Add(row);
            }
            #endregion 增加空白.

            ds.Tables.Add(tkDt);


            //如果有 SignType 列就获得签名信息.
            if (SystemConfig.CustomerNo == "TianYe")
            {
                string tTable = "ND" + int.Parse(FK_Flow) + "Track";
                string sql = "SELECT distinct a.No, a.SignType, a.EleID FROM Port_Emp a, " + tTable + " b WHERE (A.No='" + WebUser.No + "') OR B.ActionType=22 AND a.No=b.EmpFrom AND B.WorkID=" + this.WorkID;

                DataTable dtTrack = DBAccess.RunSQLReturnTable(sql);
                dtTrack.TableName = "SignType";

                dtTrack.Columns["NO"].ColumnName = "No";
                dtTrack.Columns["SIGNTYPE"].ColumnName = "SignType";
                dtTrack.Columns["ELEID"].ColumnName = "EleID";

                ds.Tables.Add(dtTrack);
            }

            string str = BP.Tools.Json.ToJson(ds);
            //用于jflow数据输出格式对比.
            //  DataType.WriteFile("c:\\WorkCheck_Init_ccflow.txt", str);
            return str;
        }
        /// <summary>
        /// 获取审核组件中刚上传的附件列表信息
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_GetNewUploadedAths()
        {
            DataRow row = null;
            string athNames = GetRequestVal("Names");
            string attachPK = GetRequestVal("AttachPK");

            DataTable athDt = new DataTable("Aths");
            athDt.Columns.Add("NodeID", typeof(int));
            athDt.Columns.Add("MyPK", typeof(string));
            athDt.Columns.Add("FK_FrmAttachment", typeof(string));
            athDt.Columns.Add("FK_MapData", typeof(string));
            athDt.Columns.Add("FileName", typeof(string));
            athDt.Columns.Add("FileExts", typeof(string));
            athDt.Columns.Add("CanDelete", typeof(string));

            FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
            QueryObject obj_Ath = new QueryObject(athDBs);
            obj_Ath.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, "ND"+this.FK_Node + "_FrmWorkCheck");
            obj_Ath.addAnd();
            obj_Ath.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());
            obj_Ath.addOrderBy(FrmAttachmentDBAttr.RDT);
            obj_Ath.DoQuery();

            foreach (FrmAttachmentDB athDB in athDBs)
            {
                if (athNames.ToLower().IndexOf("|" + athDB.FileName.ToLower() + "|") == -1)
                    continue;

                row = athDt.NewRow();

                row["NodeID"] = this.FK_Node;
                row["MyPK"] = athDB.MyPK;
                row["FK_FrmAttachment"] = athDB.FK_FrmAttachment;
                row["FK_MapData"] = athDB.FK_MapData;
                row["FileName"] = athDB.FileName;
                row["FileExts"] = athDB.FileExts;
                row["CanDelete"] = athDB.Rec == WebUser.No;

                athDt.Rows.Add(row);
            }

            return BP.Tools.Json.ToJson(athDt);
        }
        /// <summary>
        /// 获取附件链接
        /// </summary>
        /// <param name="athDB"></param>
        /// <returns></returns>
        private string GetFileAction(FrmAttachmentDB athDB)
        {
            if (athDB == null || athDB.FileExts == "") return "#";

            FrmAttachment athDesc = new FrmAttachment(athDB.FK_FrmAttachment);
            switch (athDB.FileExts)
            {
                case "doc":
                case "docx":
                case "xls":
                case "xlsx":
                    return "javascript:AthOpenOfiice('" + athDB.FK_FrmAttachment + "','" + this.WorkID + "','" + athDB.MyPK + "','" + athDB.FK_MapData + "','" + athDB.FK_FrmAttachment + "','" + this.FK_Node + "')";
                case "txt":
                case "jpg":
                case "jpeg":
                case "gif":
                case "png":
                case "bmp":
                case "ceb":
                    return "javascript:AthOpenView('" + athDB.RefPKVal + "','" + athDB.MyPK + "','" + athDB.FK_FrmAttachment + "','" + athDB.FileExts + "','" + this.FK_Flow + "','" + athDB.FK_MapData + "','" + this.WorkID + "','false')";
                case "pdf":
                    return athDesc.SaveTo + this.WorkID + "/" + athDB.MyPK + "." + athDB.FileName;
            }

            return "javascript:AthDown('" + athDB.FK_FrmAttachment + "','" + this.WorkID + "','" + athDB.MyPK + "','" + athDB.FK_MapData + "','" + this.FK_Flow + "','" + athDB.FK_FrmAttachment + "')";
        }

        /// <summary>
        /// 审核信息保存.
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_Save()
        {
            //设计的时候,workid=0,不让其存储.
            if (this.WorkID == 0)
                return "";

            // 审核信息.
            string msg = "";
            string dotype = GetRequestVal("ShowType");
            string doc = GetRequestVal("Doc");
            bool isCC = GetRequestVal("IsCC") == "1";
            string fwcView = null;
            if (DataType.IsNullOrEmpty(GetRequestVal("FWCView")) == false)
                fwcView = "@FWCView=" + GetRequestValInt("FWCView");
            //查看时取消保存
            if (dotype != null && dotype == "View")
                return "";

            //内容为空，取消保存，20170727取消此处限制
            //if (DataType.IsNullOrEmpty(doc.Trim()))
            //    return "";

            string val = string.Empty;
            FrmWorkCheck wcDesc = new FrmWorkCheck(this.FK_Node);
            if (DataType.IsNullOrEmpty(wcDesc.FWCFields) == false)
            {
                //循环属性获取值
                Attrs fwcAttrs = new Attrs(wcDesc.FWCFields);
                foreach (Attr attr in fwcAttrs)
                {
                    if (attr.UIContralType == UIContralType.TB)
                    {
                        val = GetRequestVal("TB_" + attr.Key);

                        msg += attr.Key + "=" + val + ";";
                    }
                    else if (attr.UIContralType == UIContralType.CheckBok)
                    {
                        val = GetRequestVal("CB_" + attr.Key);

                        msg += attr.Key + "=" + Convert.ToInt32(val) + ";";
                    }
                    else if (attr.UIContralType == UIContralType.DDL)
                    {
                        val = GetRequestVal("DDL_" + attr.Key);

                        msg += attr.Key + "=" + val + ";";
                    }
                }
            }
            else
            {
                // 加入审核信息.
                msg = doc;
            }

            //在审核人打开后，申请人撤销，就不不能让其保存.
            string sql = "SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID;
            if (DBAccess.RunSQLReturnValInt(sql) != this.FK_Node)
                return "err@現在のジョブはキャンセルされたか、次のノードに移動されました。監査を実行できません。";

            // 处理人大的需求，需要把审核意见写入到FlowNote里面去.
            sql = "UPDATE WF_GenerWorkFlow SET FlowNote='" + msg + "' WHERE WorkID=" + this.WorkID;
            DBAccess.RunSQL(sql);

            // 判断是否是抄送?
            if (isCC)
            {
                // 写入审核信息，有可能是update数据。
                Dev2Interface.WriteTrackWorkCheck(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);

                //设置抄送状态 - 已经审核完毕.
                Dev2Interface.Node_CC_SetSta(this.FK_Node, this.WorkID, WebUser.No, CCSta.CheckOver);
                return "";
            }

            #region 根据类型写入数据  qin
            if (wcDesc.HisFrmWorkCheckType == FWCType.Check)  //审核组件
            {
                //判断是否审核组件中“协作模式下操作员显示顺序”设置为“按照接受人员列表先后顺序(官职大小)”，删除原有的空审核信息
                if (wcDesc.FWCOrderModel == FWCOrderModel.SqlAccepter)
                {
                    sql = "DELETE FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID = " + this.WorkID +
                          " AND ActionType = " + (int)ActionType.WorkCheck + " AND NDFrom = " + this.FK_Node +
                          " AND NDTo = " + this.FK_Node + " AND EmpFrom = '" + WebUser.No + "'";
                    DBAccess.RunSQL(sql);
                }

                Dev2Interface.WriteTrackWorkCheck(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel, fwcView);
            }

            if (wcDesc.HisFrmWorkCheckType == FWCType.DailyLog)//日志组件
            {
                Dev2Interface.WriteTrackDailyLog(this.FK_Flow, this.FK_Node, wcDesc.Name, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);
            }
            if (wcDesc.HisFrmWorkCheckType == FWCType.WeekLog)//周报
            {
                Dev2Interface.WriteTrackWeekLog(this.FK_Flow, this.FK_Node, wcDesc.Name, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);
            }
            if (wcDesc.HisFrmWorkCheckType == FWCType.MonthLog)//月报
            {
                Dev2Interface.WriteTrackMonthLog(this.FK_Flow, this.FK_Node, wcDesc.Name, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);
            }
            #endregion

            sql = "SELECT MyPK,RDT FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE NDFrom = " + this.FK_Node + " AND ActionType = " + (int)ActionType.WorkCheck + " AND EmpFrom = '" + WebUser.No + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql, 1, 1, "MyPK", "RDT", "DESC");

            return dt.Rows.Count > 0 ? dt.Rows[0]["RDT"].ToString() : "";
        }
        #endregion

        #region 工作分配.
        /// <summary>
        /// 分配工作
        /// </summary>
        /// <returns></returns>
        public string AllotTask_Init()
        {
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.FK_Node, true);
            return wls.ToJson();
        }
        /// <summary>
        /// 分配工作
        /// </summary>
        /// <returns></returns>
        public string AllotTask_Save()
        {
            return "";
        }
        #endregion

        #region 执行跳转.
        /// <summary>
        /// 返回可以跳转的节点.
        /// </summary>
        /// <returns></returns>
        public string FlowSkip_Init()
        {
            Node nd = new Node(this.FK_Node);
            BP.WF.Template.BtnLab lab = new BtnLab(this.FK_Node);

            string sql = "SELECT NodeID,Name FROM WF_Node WHERE FK_Flow='" + this.FK_Flow + "'";
            switch (lab.JumpWayEnum)
            {
                case JumpWay.Previous:
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE NodeID IN (SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " )";
                    break;
                case JumpWay.Next:
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE NodeID NOT IN (SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " ) AND FK_Flow='" + this.FK_Flow + "'";
                    break;
                case JumpWay.AnyNode:
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE FK_Flow='" + this.FK_Flow + "' ORDER BY STEP";
                    break;
                case JumpWay.JumpSpecifiedNodes:
                    sql = nd.JumpToNodes;
                    sql = sql.Replace("@WebUser.No", WebUser.No);
                    sql = sql.Replace("@WebUser.Name", WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                    if (sql.Contains("@"))
                    {
                        Work wk = nd.HisWork;
                        wk.OID = this.WorkID;
                        wk.RetrieveFromDBSources();
                        foreach (Attr attr in wk.EnMap.Attrs)
                        {
                            if (sql.Contains("@") == false)
                                break;
                            sql = sql.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                        }
                    }
                    break;
                case JumpWay.CanNotJump:
                    return "err@このノードでは遷移は許可されていません。";
                default:
                    return "err@判断されない";
            }

            sql = sql.Replace("~", "'");
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //如果是oracle,就转成小写.
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["NODEID"].ColumnName = "NodeID";
                dt.Columns["NAME"].ColumnName = "Name";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 执行跳转
        /// </summary>
        /// <returns></returns>
        public string FlowSkip_Do()
        {
            try
            {
                Node ndJump = new Node(this.GetRequestValInt("GoNode"));
                BP.WF.WorkNode wn = new BP.WF.WorkNode(this.WorkID, this.FK_Node);
                string msg = wn.NodeSend(ndJump, null).ToMsgOfHtml();
                return msg;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion 执行跳转.

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
            throw new Exception("@マーク[" + this.DoType + "]、それを見つけることができませんでした。");
        }
        #endregion 执行父类的重写方法.

        #region 抄送Adv.
        /// <summary>
        /// 选择权限组
        /// </summary>
        /// <returns></returns>
        public string CCAdv_SelectGroups()
        {
            string sql = "SELECT NO,NAME FROM GPM_Group ORDER BY IDX";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 抄送初始化.
        /// </summary>
        /// <returns></returns>
        public string CCAdv_Init()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Hashtable ht = new Hashtable();
            ht.Add("Title", gwf.Title);

            //计算出来曾经抄送过的人.
            Paras ps = new Paras();
            ps.SQL = "SELECT CCToName FROM WF_CCList WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("FK_Node", this.FK_Node);
            ps.Add("WorkID", this.WorkID);
            DataTable mydt = DBAccess.RunSQLReturnTable(ps);
            string toAllEmps = "";
            foreach (DataRow dr in mydt.Rows)
                toAllEmps += dr[0].ToString() + ",";

            ht.Add("CCTo", toAllEmps);

            // 根据他判断是否显示权限组。
            if (BP.DA.DBAccess.IsExitsObject("GPM_Group") == true)
                ht.Add("IsGroup", "1");
            else
                ht.Add("IsGroup", "0");

            //返回流程标题.
            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        /// <summary>
        /// 选择部门呈现信息.
        /// </summary>
        /// <returns></returns>
        public string CCAdv_SelectDepts()
        {
            BP.Port.Depts depts = new BP.Port.Depts();
            depts.RetrieveAll();
            return depts.ToJson();
        }
        /// <summary>
        /// 选择部门呈现信息.
        /// </summary>
        /// <returns></returns>
        public string CCAdv_SelectStations()
        {
            //岗位类型.
            string sql = "SELECT NO,NAME FROM Port_StationType ORDER BY NO";
            DataSet ds = new DataSet();
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Port_StationType";
            ds.Tables.Add(dt);

            //岗位.
            string sqlStas = "SELECT NO,NAME,FK_STATIONTYPE FROM Port_Station ORDER BY FK_STATIONTYPE,NO";
            DataTable dtSta = BP.DA.DBAccess.RunSQLReturnTable(sqlStas);
            dtSta.TableName = "Port_Station";
            ds.Tables.Add(dtSta);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 抄送发送.
        /// </summary>
        /// <returns></returns>
        public string CCAdv_Send()
        {
            //人员信息. 格式 zhangsan,张三;lisi,李四;
            string emps = this.GetRequestVal("Emps");

            //岗位信息. 格式:  001,002,003,
            string stations = this.GetRequestVal("Stations");
            stations = stations.Replace(";", ",");

            //权限组. 格式:  001,002,003,
            string groups = this.GetRequestVal("Groups");
            if (groups == null)
                groups = "";
            groups = groups.Replace(";", ",");

            //部门信息.  格式: 001,002,003,
            string depts = this.GetRequestVal("Depts");
            //标题.
            string title = this.GetRequestVal("TB_Title");
            //内容.
            string doc = this.GetRequestVal("TB_Doc");

            //调用抄送接口执行抄送.
            string ccRec = BP.WF.Dev2Interface.Node_CC_WriteTo_CClist(this.FK_Node, this.WorkID, title, doc, emps, depts, stations, groups);

            if (ccRec == "")
                return "誰にもコピーされません。";
            else
                return "今回は以下の人にコピー：" + ccRec;
            //return "执行抄送成功.emps=(" + emps + ")  depts=(" + depts + ") stas=(" + stations + ") 标题:" + title + " ,抄送内容:" + doc;
        }
        #endregion 抄送Adv.

        #region 抄送普通的抄送.
        public string CC_AddEmps()
        {
            string toEmpStrs = this.GetRequestVal("AddEmps");
            toEmpStrs = toEmpStrs.Replace(",", ";");
            string[] toEmps = toEmpStrs.Split(';');
            string infos = "";
            foreach (string empStr in toEmps)
            {
                if (DataType.IsNullOrEmpty(empStr) == true)
                    continue;

                BP.GPM.Emp emp = new GPM.Emp(empStr);

                CCList cc = new CCList();
                cc.FK_Flow = this.FK_Flow;
                cc.FK_Node = this.FK_Node;
                cc.WorkID = this.WorkID;
                cc.Rec = emp.No;

                cc.Insert();
            }

            return "";
        }

        /// <summary>
        /// 抄送发送.
        /// </summary>
        /// <returns></returns>
        public string CC_Send()
        {
            //人员信息. 格式 zhangsan,张三;lisi,李四;
            string emps = this.GetRequestVal("Emps");

            //岗位信息. 格式:  001,002,003,
            string stations = this.GetRequestVal("Stations");
            stations = stations.Replace(";", ",");

            //权限组. 格式:  001,002,003,
            string groups = this.GetRequestVal("Groups");
            if (groups == null)
                groups = "";
            groups = groups.Replace(";", ",");

            //部门信息.  格式: 001,002,003,
            string depts = this.GetRequestVal("Depts");
            //标题.
            string title = this.GetRequestVal("TB_Title");
            //内容.
            string doc = this.GetRequestVal("TB_Doc");

            //调用抄送接口执行抄送.
            string ccRec = BP.WF.Dev2Interface.Node_CC_WriteTo_CClist(this.FK_Node, this.WorkID, title, doc, emps, depts, stations, groups);

            if (ccRec == "")
                return "誰にもコピーされません。";
            else
                return "今回は以下の人にコピー：" + ccRec;
            //return "执行抄送成功.emps=(" + emps + ")  depts=(" + depts + ") stas=(" + stations + ") 标题:" + title + " ,抄送内容:" + doc;
        }
        #endregion 抄送普通的抄送.

        #region 退回到分流节点处理器.
        /// <summary>
        /// 初始化.
        /// </summary>
        /// <returns></returns>
        public string DealSubThreadReturnToHL_Init()
        {
            /* 如果工作节点退回了*/
            BP.WF.ReturnWorks rws = new BP.WF.ReturnWorks();
            rws.Retrieve(BP.WF.ReturnWorkAttr.ReturnToNode, this.FK_Node,
                BP.WF.ReturnWorkAttr.WorkID, this.WorkID,
                BP.WF.ReturnWorkAttr.RDT);

            string msgInfo = "";
            if (rws.Count != 0)
            {
                foreach (BP.WF.ReturnWork rw in rws)
                {
                    msgInfo += "<fieldset width='100%' ><legend>&nbsp; ノードから:" + rw.ReturnNodeName + " 帰国者:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='./../../DataUser/ReturnLog/" + this.FK_Flow + "/" + rw.MyPK + ".htm' target=_blank>作業ログ</a></legend>";
                    msgInfo += rw.BeiZhuHtml;
                    msgInfo += "</fieldset>";
                }
            }

            //把节点信息也传入过去，用于判断不同的按钮显示. 
            BP.WF.Template.BtnLab btn = new BtnLab(this.FK_Node);
            BP.WF.Node nd = new Node(this.FK_Node);

            Hashtable ht = new Hashtable();
            //消息.
            ht.Add("MsgInfo", msgInfo);

            //是否可以移交？
            if (btn.ShiftEnable)
                ht.Add("ShiftEnable", "1");
            else
                ht.Add("ShiftEnable", "0");

            //是否可以撤销？
            if (nd.HisCancelRole == CancelRole.None)
                ht.Add("CancelRole", "0");
            else
                ht.Add("CancelRole", "1");

            //是否可以删除子线程? 在分流节点上.
            if (btn.ThreadIsCanDel)
                ht.Add("ThreadIsCanDel", "1");
            else
                ht.Add("ThreadIsCanDel", "0");

            //是否可以移交子线程? 在分流节点上.
            if (btn.ThreadIsCanShift)
                ht.Add("ThreadIsCanShift", "1");
            else
                ht.Add("ThreadIsCanShift", "0");

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string DealSubThreadReturnToHL_Done()
        {
            //操作类型.
            string actionType = this.GetRequestVal("ActionType");
            string note = this.GetRequestVal("Note");


            if (actionType == "Return")
            {
                /*如果是退回. */
                BP.WF.ReturnWork rw = new BP.WF.ReturnWork();
                rw.Retrieve(BP.WF.ReturnWorkAttr.ReturnToNode, this.FK_Node,
                         BP.WF.ReturnWorkAttr.WorkID, this.WorkID);
                string info = BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, this.FID,
                    this.FK_Node, rw.ReturnNode, note, false);
                return info;
            }


            if (actionType == "Shift")
            {
                /*如果是移交操作.*/
                string toEmps = this.GetRequestVal("ShiftToEmp");
                return BP.WF.Dev2Interface.Node_Shift(this.WorkID, toEmps, note);
            }

            if (actionType == "Kill")
            {
                string msg = BP.WF.Dev2Interface.Flow_DeleteSubThread(this.FK_Flow, this.WorkID, "手動で削除");
                //提示信息.
                if (msg == "" || msg == null)
                    msg = "ジョブは正常に削除されました…";
                return msg;
            }

            if (actionType == "UnSend")
            {
                return BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.FID, this.FK_Node);
            }

            return "err@判断なしのタイプ" + actionType;
        }
        #endregion 退回到分流节点处理器.

        public string DeleteFlowInstance_Init()
        {
            if (BP.WF.Dev2Interface.Flow_IsCanDeleteFlowInstance(this.FK_Flow,
                this.WorkID, BP.Web.WebUser.No) == false)
                return "err@このフローを削除する権限がありません";
            //获取节点中配置的流程删除规则
            if (this.FK_Node != 0)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT wn.DelEnable FROM WF_Node wn WHERE wn.NodeID = " + SystemConfig.AppCenterDBVarStr + "NodeID";
                ps.Add("NodeID", this.FK_Node);
                return DBAccess.RunSQLReturnValInt(ps) + "";
            }

            return "正常に削除されました。";
        }

        public string DeleteFlowInstance_DoDelete()
        {
            if (BP.WF.Dev2Interface.Flow_IsCanDeleteFlowInstance(this.FK_Flow,
                this.WorkID, BP.Web.WebUser.No) == false)
                return "err@このフローを削除する権限がありません。";

            string deleteWay = this.GetRequestVal("RB_DeleteWay");
            string doc = this.GetRequestVal("TB_Doc");

            //是否要删除子流程？ 这里注意变量的获取方式，你可以自己定义.
            string isDeleteSubFlow = this.GetRequestVal("CB_IsDeleteSubFlow");

            bool isDelSubFlow = false;
            if (isDeleteSubFlow == "1")
                isDelSubFlow = true;

            //按照标记删除.
            if (deleteWay == "1")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(this.FK_Flow, this.WorkID, doc, isDelSubFlow);

            //彻底删除.
            if (deleteWay == "3")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.FK_Flow, this.WorkID, isDelSubFlow);

            //彻底并放入到删除轨迹里.
            if (deleteWay == "2")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByWriteLog(this.FK_Flow, this.WorkID, doc, isDelSubFlow);

            return "フローは正常に削除されました。";
        }
        /// <summary>
        /// 获得节点表单数据.
        /// </summary>
        /// <returns></returns>
        public string ViewWorkNodeFrm()
        {
            Node nd = new Node(this.FK_Node);
            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

            Hashtable ht = new Hashtable();
            ht.Add("FormType", nd.FormType.ToString());
            ht.Add("Url", nd.FormUrl + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node);

            if (nd.FormType == NodeFormType.SDKForm)
                return BP.Tools.Json.ToJsonEntityModel(ht);

            if (nd.FormType == NodeFormType.SelfForm)
                return BP.Tools.Json.ToJsonEntityModel(ht);

            //表单模版.
            DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(nd.NodeFrmID);
            string json = BP.WF.Dev2Interface.CCFrom_GetFrmDBJson(this.FK_Flow, this.MyPK);
            DataTable mainTable = BP.Tools.Json.ToDataTableOneRow(json);
            mainTable.TableName = "MainTable";
            myds.Tables.Add(mainTable);

            //MapExts exts = new MapExts(nd.HisWork.ToString());
            //DataTable dtMapExt = exts.ToDataTableDescField();
            //dtMapExt.TableName = "Sys_MapExt";
            //myds.Tables.Add(dtMapExt);

            return BP.Tools.Json.ToJson(myds);
        }

        /// <summary>
        /// 回复加签信息.
        /// </summary>
        /// <returns></returns>
        public string AskForRe()
        {
            string note = this.GetRequestVal("Note"); //原因.
            return BP.WF.Dev2Interface.Node_AskforReply(this.WorkID, note);
        }
        /// <summary>
        /// 执行加签
        /// </summary>
        /// <returns>执行信息</returns>
        public string Askfor()
        {
            Int64 workID = int.Parse(this.GetRequestVal("WorkID")); //工作ID
            string toEmp = this.GetRequestVal("ToEmp"); //让谁加签?
            string note = this.GetRequestVal("Note"); //原因.
            string model = this.GetRequestVal("Model"); //模式.

            BP.WF.AskforHelpSta sta = BP.WF.AskforHelpSta.AfterDealSend;
            if (model == "1")
                sta = BP.WF.AskforHelpSta.AfterDealSendByWorker;

            return BP.WF.Dev2Interface.Node_Askfor(workID, sta, toEmp, note);
        }
        /// <summary>
        /// 人员选择器
        /// </summary>
        /// <returns></returns>
        public string SelectEmps_Init()
        {
            string fk_dept = this.FK_Dept;
            if (DataType.IsNullOrEmpty(fk_dept) == true || fk_dept.Equals("undefined") == true)
                fk_dept = BP.Web.WebUser.FK_Dept;

            DataSet ds = new DataSet();

            string sql = "";
            sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE No='" + fk_dept + "' OR ParentNo='" + fk_dept + "' ORDER BY Idx";

            //如果是节水公司的.
            //if (SystemConfig.CustomerNo == "TianYe" && WebUser.FK_Dept.IndexOf("18099") == -1)
            //    sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE  No='" + fk_dept + "'  OR (ParentNo='" + fk_dept + "' AND No!='18099') ORDER BY Idx ";


            DataTable dtDept = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dtDept.Rows.Count == 0)
            {
                fk_dept = BP.Web.WebUser.FK_Dept;
                sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE No='" + fk_dept + "' OR ParentNo='" + fk_dept + "' ORDER BY Idx ";
                dtDept = BP.DA.DBAccess.RunSQLReturnTable(sql);
            }

            dtDept.TableName = "Depts";
            ds.Tables.Add(dtDept);

            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtDept.Columns[0].ColumnName = "No";
                dtDept.Columns[1].ColumnName = "Name";
                dtDept.Columns[2].ColumnName = "ParentNo";
            }

            if (SystemConfig.CustomerNo == "TianYe")
            {
                string specFlowNos = SystemConfig.AppSettings["SpecFlowNosForAccpter"];
                if (specFlowNos == "" || specFlowNos == null)
                    specFlowNos = ",,";

                string specEmpNos = "";
                if (specFlowNos.Contains("," + this.FK_Node.ToString() + ",") == false)
                    specEmpNos = " AND No!='00000001' ";

                // specEmpNos = "";
                //sql = "SELECT No,Name,FK_Dept FROM Port_Emp WHERE FK_Dept='" + fk_dept + "' " + specEmpNos + "  ORDER BY Idx " + this.FK_Node + " " + specEmpNos + " " + specFlowNos;

                sql = "SELECT No,Name,FK_Dept FROM Port_Emp WHERE FK_Dept='" + fk_dept + "' " + specEmpNos + "  ORDER BY Idx ";
                //return "err@xx" + sql + "  = " + specEmpNos + "  " + specFlowNos +" nodeID="+this.FK_Node.ToString();

            }
            else
            {
                //sql = "SELECT No,Name, FK_Dept FROM Port_Emp WHERE FK_Dept='" + fk_dept + "' ";
                sql = "SELECT distinct A.No,A.Name, '" + fk_dept + "' as FK_Dept, a.Idx FROM Port_Emp A LEFT JOIN Port_DeptEmp B  ON A.No=B.FK_Emp WHERE A.FK_Dept='" + fk_dept + "' OR B.FK_Dept='" + fk_dept + "' ";
                sql += " ORDER BY A.Idx ";
            }

            DataTable dtEmps = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtEmps.TableName = "Emps";
            ds.Tables.Add(dtEmps);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtEmps.Columns[0].ColumnName = "No";
                dtEmps.Columns[1].ColumnName = "Name";
                dtEmps.Columns[2].ColumnName = "FK_Dept";
            }

            //转化为 json 
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        #region 选择接受人.
        /// <summary>
        /// 初始化接受人.
        /// </summary>
        /// <returns></returns>
        public string Accepter_Init()
        {
            /*如果是协作模式, 就要检查当前是否主持人, 当前是否是会签模式. */
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.FK_Node != this.FK_Node)
                return "err@現在のフローは[" + gwf.NodeName + "]上記、現在の処理スタッフは[" + gwf.TodoEmps + "]";

            //当前节点ID.
            Node nd = new Node(this.FK_Node);

            //判断当前是否是协作模式.
            if (nd.TodolistModel == TodolistModel.Teamup && nd.IsStartNode == false)
            {
                if (gwf.TodoEmps.Contains(WebUser.No + ","))
                {
                    /* 说明我是主持人之一, 我就可以选择接受人,发送到下一个节点上去. */
                }
                else
                {
                    /* 不是主持人就执行发送，返回发送结果. */
                    SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);
                    return "info@" + objs.ToMsgOfHtml();
                }
            }

            int toNodeID = this.GetRequestValInt("ToNode");
            if (toNodeID == 0)
            {
                Nodes nds = nd.HisToNodes;
                if (nds.Count == 1)
                    toNodeID = nds[0].GetValIntByKey("NodeID");
                else
                    return "err@パラメータエラー。ノードID ToNodeに到達するには渡される必要があります";
            }

            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();

            Selector select = new Selector(toNodeID);

            if (select.SelectorModel == SelectorModel.GenerUserSelecter)
                return "url@AccepterOfGener.htm?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + nd.FK_Flow + "&ToNode=" + toNodeID+"&PWorkID="+ gwf.PWorkID;

            if (select.SelectorModel == SelectorModel.AccepterOfDeptStationEmp)
                return "url@AccepterOfDeptStationEmp.htm?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + nd.FK_Flow + "&ToNode=" + toNodeID + "&PWorkID=" + gwf.PWorkID;

            //获得 部门与人员.
            DataSet ds = select.GenerDataSet(toNodeID, wk);

            if (SystemConfig.CustomerNo == "TianYe") //天业集团，去掉00000001董事长
            {
               
            }

            #region 计算上一次选择的结果, 并把结果返回过去.
            string sql = "";
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            dt.TableName = "Selected";
            if (select.IsAutoLoadEmps == true)
            {
                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    sql = "SELECT  top 1 Tag,EmpTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND A.NDTo=" + toNodeID + " AND ActionType=1 ORDER BY WorkID DESC";
                else if (SystemConfig.AppCenterDBType == DBType.Oracle)
                    sql = "SELECT * FROM (SELECT  Tag,EmpTo,WorkID FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND A.NDTo=" + toNodeID + " AND ActionType=1 ORDER BY WorkID DESC ) WHERE ROWNUM =1";
                else if (SystemConfig.AppCenterDBType == DBType.MySQL)
                    sql = "SELECT  Tag,EmpTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND A.NDTo=" + toNodeID + " AND ActionType=1 ORDER BY WorkID  DESC limit 1,1 ";
                else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                    sql = "SELECT  Tag,EmpTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND A.NDTo=" + toNodeID + " AND ActionType=1 ORDER BY WorkID  DESC limit 1 ";

                DataTable mydt = DBAccess.RunSQLReturnTable(sql);
                string emps = "";
                if (mydt.Rows.Count != 0)
                {
                    emps = mydt.Rows[0]["Tag"].ToString();
                    if (emps == "" || emps == null)
                    {
                        emps = mydt.Rows[0]["EmpTo"].ToString();
                        emps = emps + "," + emps;
                    }
                }

                string[] strs = emps.Split(';');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    string[] emp = str.Split(',');
                    if (emp.Length != 2)
                        continue;

                    DataRow dr = dt.NewRow();
                    dr[0] = emp[0];
                    dt.Rows.Add(dr);
                }
            }

            //增加一个table.
            ds.Tables.Add(dt);
            #endregion 计算上一次选择的结果, 并把结果返回过去.

            //返回json.
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 保存.
        /// </summary>
        /// <returns></returns>
        public string Accepter_Save()
        {
            try
            {
                //求到达的节点. 
                int toNodeID = 0;
                if (this.GetRequestVal("ToNode") != "0")
                    toNodeID = int.Parse(this.GetRequestVal("ToNode"));

                if (toNodeID == 0)
                {   //没有就获得第一个节点.
                    Node nd = new Node(this.FK_Node);
                    Nodes nds = nd.HisToNodes;
                    toNodeID = nds[0].GetValIntByKey("NodeID");
                }

                //求发送到的人员.
                // string selectEmps = this.GetValFromFrmByKey("SelectEmps");
                string selectEmps = this.GetRequestVal("SelectEmps");
                selectEmps = selectEmps.Replace(";", ",");

                //保存接受人.
                BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, selectEmps);
                return "SaveOK@" + selectEmps;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行保存并发送.
        /// </summary>
        /// <returns>返回发送的结果.</returns>
        public string Accepter_Send()
        {
            try
            {
                //求到达的节点. 
                int toNodeID = 0;
                if (this.GetRequestVal("ToNode") != "0")
                    toNodeID = int.Parse(this.GetRequestVal("ToNode"));

                if (toNodeID == 0)
                {   //没有就获得第一个节点.
                    Node nd = new Node(this.FK_Node);
                    Nodes nds = nd.HisToNodes;
                    toNodeID = nds[0].GetValIntByKey("NodeID");
                }

                //求发送到的人员.
                // string selectEmps = this.GetValFromFrmByKey("SelectEmps");
                string selectEmps = this.GetRequestVal("SelectEmps");
                selectEmps = selectEmps.Replace(";", ",");

                //执行发送.
                SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow,
                    this.WorkID, toNodeID, selectEmps);
                return objs.ToMsgOfHtml();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion

        #region 回滚.
        /// <summary>
        /// 回滚操作.
        /// </summary>
        /// <returns></returns>
        public string Rollback_Init()
        {
            string andsql = " ";
            andsql += "  ActionType=" + (int)ActionType.Start;
            andsql += " OR ActionType=" + (int)ActionType.TeampUp;
            andsql += " OR ActionType=" + (int)ActionType.Forward;
            andsql += " OR ActionType=" + (int)ActionType.HuiQian;

            string sql = "SELECT RDT,NDFrom, NDFromT,EmpFrom,EmpFromT  FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID=" + this.WorkID + " AND(" + andsql + ") Order By RDT DESC";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns[0].ColumnName = "RDT";
                dt.Columns[1].ColumnName = "NDFrom";
                dt.Columns[2].ColumnName = "NDFromT";
                dt.Columns[3].ColumnName = "EmpFrom";
                dt.Columns[4].ColumnName = "EmpFromT";
            }


            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 执行回滚操作
        /// </summary>
        /// <returns></returns>
        public string Rollback_Done()
        {
            FlowExt flow = new FlowExt(this.FK_Flow);
            return flow.DoRebackFlowData(this.WorkID, this.FK_Node, this.GetRequestVal("Msg"));
        }
        #endregion 回滚.



        #region 工作退回.
        /// <summary>
        /// 获得可以退回的节点.
        /// </summary>
        /// <returns>退回信息</returns>
        public string Return_Init()
        {
            try
            {
                DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(this.FK_Node, this.WorkID, this.FID);

                //备注:启动子流程的或者平级子流程的节点也可以退回，退回后是否结束子流程需要在FEE事件中处理 - yln修改
                //根据WorkID查询是否有启动的子流程 
                // GenerWorkFlows gwfs = new GenerWorkFlows();
                //int count = gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);
                //if (count != 0)
                //    return "info@该流程已经启动子流程，不能执行退回";

                //该流程为子流程，启动了平级子流程
                //GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                //if (gwf.PWorkID != 0) 
                //{
                //    //存在平级子流程
                //     gwfs = new GenerWorkFlows();
                //     count = gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, gwf.PWorkID);
                //     SubFlows subFlows = new SubFlows(); 
                //     int subFlowCount = subFlows.Retrieve(SubFlowYanXuAttr.FK_Node, this.FK_Node, SubFlowYanXuAttr.SubFlowModel,1);
                //    if (subFlowCount != 0)//含有平级子流程
                //    {
                //        foreach(SubFlow subFlow in subFlows)
                //        {
                //            //根据FlowNo获取有没有发起流程
                //            var subGwf = gwfs.GetEntityByKey(GenerWorkFlowAttr.FK_Flow,subFlow.SubFlowNo);
                //            if(subGwf!=null)
                //                return "info@该流程已经启动平级子流程，不能执行退回";
                //        }
                //    } 
                //}


                //如果只有一个退回节点，就需要判断是否启用了单节点退回规则.

                if (dt.Rows.Count == 1)
                {
                    Node nd = new Node(this.FK_Node);
                    if (nd.ReturnOneNodeRole != 0)
                    {
                        /* 如果:启用了单节点退回规则.
                         */
                        string returnMsg = "";
                        if (nd.ReturnOneNodeRole == 1 && DataType.IsNullOrEmpty(nd.ReturnField) == false)
                        {
                            /*从表单字段里取意见.*/
                            Flow fl = new Flow(nd.FK_Flow);
                            string sql = "SELECT " + nd.ReturnField + " FROM " + fl.PTable + " WHERE OID=" + this.WorkID;
                            returnMsg = DBAccess.RunSQLReturnStringIsNull(sql, "未記入のコメント");
                        }

                        if (nd.ReturnOneNodeRole == 2)
                        {
                            /*从审核组件里取意见.*/
                            string sql = "SELECT Msg FROM ND" + int.Parse(nd.FK_Flow) + "Track WHERE WorkID=" + this.WorkID + " AND NDFrom=" + this.FK_Node + " AND EmpFrom='" + WebUser.No + "' AND ActionType=" + (int)ActionType.WorkCheck;
                            returnMsg = DBAccess.RunSQLReturnStringIsNull(sql, "未記入のコメント");
                        }

                        int toNodeID = int.Parse(dt.Rows[0][0].ToString());

                        string info = BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, 0, this.FK_Node, toNodeID, returnMsg, false);
                        return "info@" + info;
                    }
                }
                return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行退回,返回退回信息.
        /// </summary>
        /// <returns></returns>
        public string DoReturnWork()
        {
            string[] vals = this.GetRequestVal("ReturnToNode").Split('@');
            int toNodeID = int.Parse(vals[0]);

            string toEmp = vals[1];
            string reMesage = this.GetRequestVal("ReturnInfo");

            bool isBackBoolen = false;
            string isBack = this.GetRequestVal("IsBack");
            if (isBack == "1")
                isBackBoolen = true;

            string pageData = this.GetRequestVal("PageData");

            return BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, this.FID, this.FK_Node, toNodeID, toEmp, reMesage, isBackBoolen, pageData);
        }
        #endregion

        /// <summary>
        /// 执行移交.
        /// </summary>
        /// <returns></returns>
        public string Shift()
        {
            string msg = this.GetRequestVal("Message");
            string toEmp = this.GetRequestVal("ToEmp");
            return BP.WF.Dev2Interface.Node_Shift(this.WorkID, toEmp, msg);
        }
        /// <summary>
        /// 撤销移交
        /// </summary>
        /// <returns></returns>
        public string UnShift()
        {
            return BP.WF.Dev2Interface.Node_ShiftUn(this.FK_Flow, this.WorkID);
        }
        /// <summary>
        /// 执行催办
        /// </summary>
        /// <returns></returns>
        public string Press()
        {
            string msg = this.GetRequestVal("Msg");
            //调用API.
            return BP.WF.Dev2Interface.Flow_DoPress(this.WorkID, msg, true);
        }

        #region 流程数据模版. for 浙商银行 by zhoupeng.
        /// <summary>
        /// 流程数据模版
        /// </summary>
        /// <returns></returns>
        public string DBTemplate_Init()
        {
            DataSet ds = new DataSet();

            //获取模版.
            Paras ps = new Paras();
            ps.SQL = "SELECT WorkID,Title,AtPara FROM WF_GenerWorkFlow WHERE FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow AND WFState=3 AND Starter=" + SystemConfig.AppCenterDBVarStr + "Starter AND ATPARA LIKE '%@DBTemplate=1%'";
            ps.Add("FK_Flow", this.FK_Flow);
            ps.Add("Starter", WebUser.No);
            DataTable dtTemplate = DBAccess.RunSQLReturnTable(ps);
            dtTemplate.TableName = "DBTemplate";
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtTemplate.Columns[0].ColumnName = "WorkID";
                dtTemplate.Columns[1].ColumnName = "Title";
            }

            //把模版名称替换 title. 
            foreach (DataRow dr in dtTemplate.Rows)
            {
                string str = dr[2].ToString();
                BP.DA.AtPara ap = new AtPara(str);
                dr["Title"] = ap.GetValStrByKey("DBTemplateName");
            }

            ds.Tables.Add(dtTemplate);

            // 获取历史发起数据.
            ps = new Paras();
            if (SystemConfig.AppCenterDBType == DBType.MSSQL)
            {
                ps.SQL = "SELECT TOP 30 WorkID,Title FROM WF_GenerWorkFlow WHERE FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow AND WFState=3 AND Starter=" + SystemConfig.AppCenterDBVarStr + "Starter AND ATPARA NOT LIKE '%@DBTemplate=1%' ORDER BY RDT ";
                ps.Add("FK_Flow", this.FK_Flow);
                ps.Add("Starter", WebUser.No);
            }
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                ps.SQL = "SELECT WorkID,Title FROM WF_GenerWorkFlow WHERE FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow AND WFState=3 AND Starter=" + SystemConfig.AppCenterDBVarStr + "Starter AND ATPARA NOT LIKE '%@DBTemplate=1%' AND rownum<=30 ORDER BY RDT ";
                ps.Add("FK_Flow", this.FK_Flow);
                ps.Add("Starter", WebUser.No);
            }
            if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                ps.SQL = "SELECT WorkID,Title FROM WF_GenerWorkFlow WHERE FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow AND WFState=3 AND Starter=" + SystemConfig.AppCenterDBVarStr + "Starter AND ATPARA NOT LIKE '%@DBTemplate=1%' ORDER BY RDT LIMIT 30";
                ps.Add("FK_Flow", this.FK_Flow);
                ps.Add("Starter", WebUser.No);
            }
            DataTable dtHistroy = DBAccess.RunSQLReturnTable(ps);
            dtHistroy.TableName = "History";
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtHistroy.Columns[0].ColumnName = "WorkID";
                dtHistroy.Columns[1].ColumnName = "Title";
            }
            ds.Tables.Add(dtHistroy);

            //转化为 json.
            return BP.Tools.Json.ToJson(ds);
        }

        public string DBTemplate_SaveAsDBTemplate()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.Paras_DBTemplate = true;
            gwf.Paras_DBTemplateName = HttpUtility.UrlDecode(this.GetRequestVal("Title"), System.Text.Encoding.UTF8);//this.GetRequestVal("Title");
            gwf.Update();
            return "正常に設定されました";
        }

        public string DBTemplate_DeleteDBTemplate()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.Paras_DBTemplate = false;
            gwf.Update();

            return "正常に設定されました";
        }

        public string DBTemplate_StartFlowAsWorkID()
        {
            return "正常に設定されました";
        }
        #endregion 流程数据模版.

        #region tonodes
        /// <summary>
        /// 初始化.
        /// </summary>
        /// <returns></returns>
        public string ToNodes_Init()
        {
            //获取到下一个节点的节点Nodes

            //获得当前节点到达的节点.
            Nodes nds = new Nodes();
            string toNodes = this.GetRequestVal("ToNodes");
            if (DataType.IsNullOrEmpty(toNodes) == false)
            {
                /*解决跳转问题.*/
                string[] mytoNodes = toNodes.Split(',');
                foreach (string str in mytoNodes)
                {
                    if (string.IsNullOrEmpty(str) == true)
                        continue;
                    nds.AddEntity(new Node(int.Parse(str)));
                }
            }
            else
            {
                nds = BP.WF.Dev2Interface.WorkOpt_GetToNodes(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
            }

            //获得上次默认选择的节点
            int lastSelectNodeID = BP.WF.Dev2Interface.WorkOpt_ToNodes_GetLasterSelectNodeID(this.FK_Flow, this.FK_Node);
            if (lastSelectNodeID == 0 && nds.Count != 0)
                lastSelectNodeID = int.Parse(nds[0].PKVal.ToString());


            DataSet ds = new DataSet();
            ds.Tables.Add(nds.ToDataTableField("Nodes"));
            DataTable dt = new DataTable("SelectNode");
            dt.Columns.Add("NodeID");
            DataRow dr = dt.NewRow();
            dr["NodeID"] = lastSelectNodeID;
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);
            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        public string ToNodes_Send()
        {
            string toNodes = this.GetRequestVal("ToNodes");
            // 执行发送.
            string msg = "";
            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();

            try
            {
                string toNodeStr = int.Parse(FK_Flow) + "01";
                //如果为开始节点
                if (toNodeStr == toNodes)
                {
                    //把参数更新到数据库里面.
                    GenerWorkFlow gwf = new GenerWorkFlow();
                    gwf.WorkID = this.WorkID;
                    gwf.RetrieveFromDBSources();
                    gwf.Paras_ToNodes = toNodes;
                    gwf.Save();

                    WorkNode firstwn = new WorkNode(wk, nd);

                    Node toNode = new Node(toNodeStr);
                    msg = firstwn.NodeSend(toNode, gwf.Starter).ToMsgOfHtml();
                }
                else
                {
                    msg = BP.WF.Dev2Interface.WorkOpt_SendToNodes(this.FK_Flow,
                        this.FK_Node, this.WorkID, this.FID, toNodes).ToMsgOfHtml();
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

            GenerWorkFlow gwfw = new GenerWorkFlow();
            gwfw.WorkID = this.WorkID;
            gwfw.RetrieveFromDBSources();
            if (nd.IsRememberMe == true)
                gwfw.Paras_ToNodes = toNodes;
            else
                gwfw.Paras_ToNodes = "";
            gwfw.Save();

            //当前节点.
            Node currNode = new Node(this.FK_Node);
            Flow currFlow = new Flow(this.FK_Flow);

            #region 处理发送后转向.
            try
            {
                /*处理转向问题.*/
                switch (currNode.HisTurnToDeal)
                {
                    case TurnToDeal.SpecUrl:
                        string myurl = currNode.TurnToDealDoc.Clone().ToString();
                        if (myurl.Contains("?") == false)
                            myurl += "?1=1";
                        Attrs myattrs = currNode.HisWork.EnMap.Attrs;
                        Work hisWK = currNode.HisWork;
                        foreach (Attr attr in myattrs)
                        {
                            if (myurl.Contains("@") == false)
                                break;
                            myurl = myurl.Replace("@" + attr.Key, hisWK.GetValStrByKey(attr.Key));
                        }
                        myurl = myurl.Replace("@WebUser.No", BP.Web.WebUser.No);
                        myurl = myurl.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        myurl = myurl.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

                        if (myurl.Contains("@"))
                        {
                            BP.WF.Dev2Interface.Port_SendMsg("admin", currFlow.Name + "に" + currNode.Name + "ノードでエラーが発生しました", "流程设计错误，に节点转向url中参数没有被替换下来。Url:" + myurl, "Err" + currNode.No + "_" + this.WorkID, SMSMsgType.Err, this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
                            throw new Exception("フロー設計が間違っており、ノードリダイレクトURLのパラメーターは置き換えられません。Url:" + myurl);
                        }

                        if (myurl.Contains("PWorkID") == false)
                            myurl += "&PWorkID=" + this.WorkID;

                        myurl += "&FromFlow=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                        return "TurnUrl@" + myurl;
                    case TurnToDeal.TurnToByCond:

                        return msg;
                    default:
                        msg = msg.Replace("@WebUser.No", BP.Web.WebUser.No);
                        msg = msg.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        msg = msg.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                        return msg;
                }
            #endregion

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("作業する次のステップを選択してください") == true || ex.Message.Contains("ユーザーは送信先のノードを選択しませんでした") == true)
                {
                    if (currNode.CondModel == CondModel.ByLineCond)
                    {
                        /*如果抛出异常，我们就让其转入选择到达的节点里, 在节点里处理选择人员. */
                        //return "url@./WorkOpt/ToNodes.htm?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID;
                        return "url@./WorkOpt/ToNodes.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID;
                    }


                    return "err@次のノードの受信者ルールでは、現在のノードが選択対象として選択されています。現在のノードのプロパティで受信者ボタンをアクティブにしないと、システムが自動的に起動を開始します。ウィンドウを閉じて、もう一度開いてください。" + ex.Message;
                }

                //绑定独立表单，表单自定义方案验证错误弹出窗口进行提示.
                if (currNode.HisFrms != null && currNode.HisFrms.Count > 0 && ex.Message.Contains("送信する前に、次の必須フィールドが不完全であることを確認してください") == true)
                {
                    return "err@" + ex.Message.Replace("@@", "@").Replace("@", "<BR>@");
                }

                GenerWorkFlow HisGenerWorkFlow = new GenerWorkFlow(this.WorkID);
                //防止发送失败丢失接受人，导致不能出现下拉方向选择框. @杜.
                if (HisGenerWorkFlow != null)
                {
                    //如果是会签状态.
                    if (HisGenerWorkFlow.HuiQianTaskSta == HuiQianTaskSta.HuiQianing)
                    {
                        //如果是主持人.
                        if (HisGenerWorkFlow.HuiQianZhuChiRen == WebUser.No)
                        {
                            string empStrSepc = BP.Web.WebUser.No + "," + BP.Web.WebUser.Name + ";";
                            if (HisGenerWorkFlow.TodoEmps.Contains(empStrSepc) == false)
                            {
                                HisGenerWorkFlow.TodoEmps += empStrSepc;
                                HisGenerWorkFlow.Update();
                            }
                        }
                        else
                        {
                            //非主持人.
                            string empStrSepc = BP.Web.WebUser.No + "," + BP.Web.WebUser.Name + ";";
                            if (HisGenerWorkFlow.TodoEmps.Contains(empStrSepc) == false)
                            {
                                HisGenerWorkFlow.TodoEmps += empStrSepc;
                                HisGenerWorkFlow.Update();
                            }
                        }
                    }


                    if (HisGenerWorkFlow.HuiQianTaskSta != HuiQianTaskSta.HuiQianing)
                    {
                        if (HisGenerWorkFlow.TodoEmps.Contains(BP.Web.WebUser.No + ",") == false)
                        {
                            HisGenerWorkFlow.TodoEmps += WebUser.No + "," + BP.Web.WebUser.Name + ";";
                            HisGenerWorkFlow.Update();
                        }
                    }
                }
                return ex.Message;

            }
        }
        #endregion tonodes



        #region 自定义.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string TransferCustom_Init()
        {
            DataSet ds = new DataSet();

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.TransferCustomType != TransferCustomType.ByWorkerSet)
            {
                gwf.TransferCustomType = TransferCustomType.ByWorkerSet;
                gwf.Update();
            }

            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            //当前运行到的节点
            Node currNode = new Node(gwf.FK_Node);

            //所有的节点s.
            Nodes nds = new Nodes(this.FK_Flow);
            //ds.Tables.Add(nds.ToDataTableField("WF_Node"));

            //工作人员列表.已经走完的节点与人员.
            GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID);
            GenerWorkerList gwln = (GenerWorkerList)gwls.GetEntityByKey(GenerWorkerListAttr.FK_Node, this.FK_Node);
            if(gwln == null)
            {
                gwln = new GenerWorkerList();
                gwln.FK_Node = currNode.NodeID;
                gwln.FK_NodeText = currNode.Name;
                gwln.FK_Emp = WebUser.No;
                gwln.FK_EmpText = WebUser.Name;
                gwls.AddEntity(gwln);
            }
            ds.Tables.Add(gwls.ToDataTableField("WF_GenerWorkerList"));

            //设置的手工运行的流转信息.
            TransferCustoms tcs = new TransferCustoms(this.WorkID);
            if (tcs.Count == 0)
            {
                #region 执行计算未来处理人.

                Work wk = currNode.HisWork;
                wk.OID = this.WorkID;
                wk.Retrieve();
                WorkNode wn = new WorkNode(wk, currNode);
                wn.HisFlow.IsFullSA = true;

                //执行计算未来处理人.
                FullSA fsa = new FullSA(wn);
                #endregion 执行计算未来处理人.

                foreach (Node nd in nds)
                {
                    if (nd.NodeID == this.FK_Node)
                        continue;
                    if (nd.GetParaBoolen(NodeAttr.IsYouLiTai) == false)
                        continue;
                    var gwl = gwls.GetEntityByKey(GenerWorkerListAttr.FK_Node, nd.NodeID);
                    if (gwl == null)
                    {
                        
                        /*说明没有 */
                        TransferCustom tc = new TransferCustom();
                        tc.WorkID = this.WorkID;
                        tc.FK_Node = nd.NodeID;
                        tc.NodeName = nd.Name;

                        #region 计算出来当前节点的工作人员.
                        SelectAccpers sas = new SelectAccpers();
                        sas.Retrieve(SelectAccperAttr.WorkID, this.WorkID, SelectAccperAttr.FK_Node, nd.NodeID);

                        string workerID = "";
                        string workerName = "";
                        foreach (SelectAccper sa in sas)
                        {
                            workerID += sa.FK_Emp + ",";
                            workerName += sa.EmpName + ",";
                        }
                        #endregion 计算出来当前节点的工作人员.

                        tc.Worker = workerID;
                        tc.WorkerName = workerName;
                        tc.Idx = nd.Step;
                        tc.IsEnable = true;
                        if (nd.HisCHWay == CHWay.ByTime && nd.GetParaInt("CHWayOfTimeRole") == 2)
                        {
                            tc.PlanDT = DateTime.Now.AddDays(1).ToString(DataType.SysDataTimeFormat);
                        }
                        tc.Insert();
                    }
                }
                tcs = new TransferCustoms(this.WorkID);
            }
            
            ds.Tables.Add(tcs.ToDataTableField("WF_TransferCustoms"));

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 自定义.

        #region 时限初始化数据
        public string CH_Init()
        {
            DataSet ds = new DataSet();

            //获取处理信息的列表
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FK_Flow, this.FK_Flow,GenerWorkerListAttr.WorkID,this.WorkID, GenerWorkerListAttr.RDT);
            DataTable dt = gwls.ToDataTableField("WF_GenerWorkerList");
            ds.Tables.Add(dt);

            //获取流程信息
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            Flow flow = new Flow(this.FK_Flow);
            ds.Tables.Add(flow.ToDataTableField("WF_Flow"));

            //获取流程流转自定义的数据
            string sql = "SELECT FK_Node AS NodeID,NodeName AS Name From WF_TransferCustom WHERE WorkID=" + WorkID + " AND IsEnable=1 Order By Idx";
            DataTable dtYL = DBAccess.RunSQLReturnTable(sql);

            //删除不启用的游离态节点时限设置
            sql = "DELETE FROM WF_CHNode WHERE WorkID="+WorkID+" AND FK_Node IN(SELECT FK_Node FROM WF_TransferCustom WHERE WorkID=" + WorkID + " AND IsEnable=0 )";
            DBAccess.RunSQL(sql);

            //节点时限表
            CHNodes chNodes = new CHNodes(this.WorkID);



            #region 获取流程节点信息的列表
            Nodes nds = new Nodes(this.FK_Flow);
            //如果是游离态的节点有可能调整顺序
            dt = new DataTable();
            dt.TableName = "WF_Node";
            dt.Columns.Add("NodeID");
            dt.Columns.Add("Name");
            dt.Columns.Add("SDTOfNode");//节点应完成时间
            dt.Columns.Add("PlantStartDt");//节点计划开始时间
            dt.Columns.Add("GS");//工时
           
            DataRow dr;
            bool isFirstY = true;
            //上一个节点的时间
            string beforeSDTOfNode = "";
            //先排序运行过的节点
            CHNode chNode = null;
            foreach(GenerWorkerList gwl in gwls)
            {
                chNode = chNodes.GetEntityByKey(CHNodeAttr.FK_Node, gwl.FK_Node) as CHNode;
                if (chNode != null)
                    continue;
                chNode = new CHNode();
                chNode.WorkID = this.WorkID;
                chNode.FK_Node = gwl.FK_Node;
                chNode.NodeName = gwl.FK_NodeText;
                chNode.StartDT = gwl.RDT;
                chNode.EndDT = gwl.CDT;
                chNode.FK_Emp = gwl.FK_Emp;
                chNode.FK_EmpT = gwl.FK_EmpText;
                chNode.SetPara("RDT", gwl.RDT);
                chNode.SetPara("CDT", gwl.CDT);
                chNode.SetPara("IsPass", gwl.IsPass);
                chNodes.AddEntity(chNode);
                beforeSDTOfNode = gwl.CDT;
            }
            foreach (Node node in nds)
            {
                GenerWorkerList gwl = gwls.GetEntityByKey(GenerWorkerListAttr.FK_Node, node.NodeID) as GenerWorkerList;
                if (gwl != null)
                    continue;

                //已经设定
                chNode = chNodes.GetEntityByKey(CHNodeAttr.FK_Node, node.NodeID) as CHNode;
                if (chNode != null)
                    continue;

                string sdtOfNode = "";
                string plantStartDt = "";
                if (node.GetParaBoolen("IsYouLiTai") == true)
                {
                    if(isFirstY == true)
                    {
                        foreach(DataRow drYL in dtYL.Rows)
                        {
                            chNode = chNodes.GetEntityByKey(CHNodeAttr.FK_Node, int.Parse(drYL["NodeID"].ToString())) as CHNode;
                            if (chNode != null)
                                continue;
                            chNode = new CHNode();
                            chNode.WorkID = this.WorkID;
                            chNode.FK_Node = int.Parse(drYL["NodeID"].ToString());
                            chNode.NodeName = drYL["Name"].ToString();
                            //计划开始时间
                            plantStartDt = beforeSDTOfNode;
                            chNode.StartDT = plantStartDt;
                            //计划完成时间
                            sdtOfNode = sdtOfNode = getSDTOfNode(node, beforeSDTOfNode, gwf);
                            chNode.EndDT = sdtOfNode;
                            //工时
                            int gty = 0;
                            if(DataType.IsNullOrEmpty(plantStartDt) == false && DataType.IsNullOrEmpty(sdtOfNode) == false)
                                gty = DataType.SpanDays(plantStartDt, sdtOfNode, false);

                            chNode.GT = gty;

                            beforeSDTOfNode = sdtOfNode;
                            chNodes.AddEntity(chNode);
                        }
                        isFirstY = false;
                    }
                    continue;
                }
                chNode = new CHNode();
                chNode.WorkID = this.WorkID;
                chNode.FK_Node = node.NodeID;
                chNode.NodeName = node.Name;

                //计划开始时间
                plantStartDt = beforeSDTOfNode;
                chNode.StartDT = plantStartDt;

                //计划完成时间
                sdtOfNode = getSDTOfNode(node, beforeSDTOfNode, gwf);
                chNode.EndDT = sdtOfNode;

                //计算初始值工天
                int gs = 0;
                if(DataType.IsNullOrEmpty(plantStartDt)==false && DataType.IsNullOrEmpty(sdtOfNode)==false) 
                    gs = DataType.SpanDays(plantStartDt, sdtOfNode,false);
                chNode.GT = gs;
                beforeSDTOfNode = sdtOfNode;
                chNodes.AddEntity(chNode);

            }
            #endregion 流程节点信息

            ds.Tables.Add(chNodes.ToDataTableField("WF_CHNode"));
            //获取当前节点信息
            Node nd = new Node(this.FK_Node);
            ds.Tables.Add(nd.ToDataTableField("WF_CurrNode"));


            #region 获取剩余天数
            Part part = new Part();
            part.MyPK = nd.FK_Flow + "_0_DeadLineRole";
            int count = part.RetrieveFromDBSources();
            int day = 0; //含假期的天数
            DateTime dateT = DateTime.Now;
            if (count > 0)
            {
                //判断是否包含假期
                if (int.Parse(part.Tag4) == 0)
                {
                    string holidays = BP.Sys.GloVar.Holidays;
                    while (true)
                    {
                        if (dateT.CompareTo(DataType.ParseSysDate2DateTime(gwf.SDTOfFlow)) >= 0)
                            break;

                        if (holidays.Contains(dateT.ToString("MM-dd")))
                        {
                            dateT = dateT.AddDays(1);
                            day++;
                            continue;
                        }
                        dateT = dateT.AddDays(1);
                    }

                }

            }
            string spanTime = GetSpanTime(DateTime.Now, DataType.ParseSysDate2DateTime(gwf.SDTOfFlow), day);
            dt = new DataTable();
            dt.TableName = "SpanTime";
            dt.Columns.Add("SpanTime");
            dr = dt.NewRow();
            dr["SpanTime"] = spanTime;
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);
            #endregion 获取剩余天数

            return BP.Tools.Json.ToJson(ds);
        }


        private string getSDTOfNode(Node node,string beforeSDTOfNode,GenerWorkFlow gwf)
        {
            DateTime SDTOfNode = DateTime.Now;
            if (beforeSDTOfNode == "")
                beforeSDTOfNode = gwf.SDTOfNode;
            //按天、小时考核
            if (node.GetParaInt("CHWayOfTimeRole") == 0)
            {
                //增加天数. 考虑到了节假日. 
                int timeLimit = node.TimeLimit;
                SDTOfNode = Glo.AddDayHoursSpan(DateTime.Parse(beforeSDTOfNode), node.TimeLimit,
                    node.TimeLimitHH, node.TimeLimitMM, node.TWay);
            }
            //按照节点字段设置
            if (node.GetParaInt("CHWayOfTimeRole") == 1)
            {
                //获取设置的字段、
                string keyOfEn = node.GetParaString("CHWayOfTimeRoleField");
                if (DataType.IsNullOrEmpty(keyOfEn) == true)
                    node.HisCHWay = CHWay.None;
                else
                    SDTOfNode = DataType.ParseSysDateTime2DateTime(node.HisWork.GetValByKey(keyOfEn).ToString());

            }
            return SDTOfNode.ToString(DataType.SysDataTimeFormat);
        }
        #endregion 时限初始化数据

       #region 节点时限重新设置
        public string CH_Save()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            //获取流程应完成时间
            string sdtOfFow = this.GetRequestVal("GWF");
            if (DataType.IsNullOrEmpty(sdtOfFow) == false && gwf.SDTOfFlow != sdtOfFow)
                gwf.SDTOfFlow = sdtOfFow;

            //获取节点的时限设置
            Nodes nds = new Nodes(this.FK_Flow);
            CHNode chNode = null;
            foreach (Node nd in nds)
            {
                chNode = new CHNode();
                string startDT = this.GetRequestVal("StartDT_" + nd.NodeID);
                string endDT = this.GetRequestVal("EndDT_" + nd.NodeID);
                int gt = this.GetRequestValInt("GT_" + nd.NodeID);
                float scale = this.GetRequestValFloat("Scale_" + nd.NodeID);
                float chanzhi = this.GetRequestValFloat("ChanZhi_" + nd.NodeID);
                float totalScale = this.GetRequestValFloat("TotalScale_" + nd.NodeID);

                chNode.WorkID = this.WorkID;
                chNode.FK_Node = nd.NodeID;
                chNode.NodeName = nd.Name;
                if (DataType.IsNullOrEmpty(startDT) == false)
                    chNode.StartDT = startDT;
                if(DataType.IsNullOrEmpty(endDT) == false)
                    chNode.EndDT = endDT;

                chNode.GT = gt;
                chNode.Scale = scale;
                chNode.ChanZhi = chanzhi;
                chNode.TotalScale = totalScale;
                chNode.Save();
            }
            gwf.Update();
            return "正常に保存";
        }
        #endregion 节点时限重新设置

        #region 节点备注的设置
        public string Note_Init()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            ps.Add("WorkID", this.WorkID);

            //转化成json
            return BP.Tools.Json.ToJson(BP.DA.DBAccess.RunSQLReturnTable(ps));
        }

        /// <summary>
        /// 保存备注.
        /// </summary>
        /// <returns></returns>
        public string Note_Save()
        {
            string msg = this.GetRequestVal("Msg");
            //需要删除track表中的数据是否存在备注
            string sql = "DELETE From ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID=" + WorkID + " AND NDFrom=" + this.FK_Node + " AND EmpFrom='" + WebUser.No+"' And ActionType="+ (int)ActionType.FlowBBS;
            DBAccess.RunSQL(sql);
            //增加track
            Node nd = new Node(this.FK_Node);
            Glo.AddToTrack(ActionType.FlowBBS, this.FK_Flow, this.WorkID, this.FID, nd.NodeID, nd.Name,  WebUser.No, WebUser.Name,nd.NodeID, nd.Name, WebUser.No, WebUser.Name, msg, null);
           
            //发送消息
            string empsStrs = DBAccess.RunSQLReturnStringIsNull("SELECT Emps FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID, "");
            string[] emps = empsStrs.Split('@');
            //标题
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            string title = "フロー名は"+gwf.FlowName+"タイトルが"+gwf.Title+"ノードにメモを追加する"+msg;
           
            foreach (string emp in emps)
            {
                if (DataType.IsNullOrEmpty(emp))
                    continue;
                //获得当前人的邮件.
                BP.WF.Port.WFEmp empEn = new BP.WF.Port.WFEmp(emp);

                BP.WF.Dev2Interface.Port_SendMsg(empEn.No, title,msg, null,"NoteMessage",this.FK_Flow,this.FK_Node,this.WorkID,this.FID);

            }
            return "正常に保存";
        }

        #endregion 节点备注的设置

        private static string GetSpanTime(DateTime t1, DateTime t2,int day)
        {
            var span = t2 - t1;
            var days = span.Days;
            var hours = span.Hours;
            var minutes = span.Minutes;

            if (days == 0 && hours == 0 && minutes == 0)
                minutes = span.Seconds > 0 ? 1 : 0;

            var spanStr = string.Empty;

            if (days > 0)
                spanStr += (days-day)+ "日";

            if (hours > 0)
                spanStr += hours + "時";

            if (minutes > 0)
                spanStr += minutes + "分";

            if (spanStr.Length == 0)
                spanStr = "0分";

            return spanStr;
        }

    }
}
