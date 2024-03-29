﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.DA;
using BP.Sys;
using BP.WF.XML;
using BP.WF.Template;
using BP.Web;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_WorkOpt_OneWork : DirectoryPageBase
    {
        /// <summary>
        /// 进度图.
        /// </summary>
        /// <returns></returns>
        public string JobSchedule_Init()
        {
            DataSet ds = BP.WF.Dev2Interface.DB_JobSchedule(this.WorkID);
            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_WorkOpt_OneWork()
        {
        }
        /// <summary>
        /// 时间轴
        /// </summary>
        /// <returns></returns>
        public string TimeBase_Init()
        {
            DataSet ds = new DataSet();

            //获取track.
            DataTable dt = BP.WF.Dev2Interface.DB_GenerTrackTable(this.FK_Flow, this.WorkID, this.FID);
            ds.Tables.Add(dt);


            #region  父子流程数据存储到这里.
            Hashtable ht = new Hashtable();
            foreach (DataRow dr in dt.Rows)
            {
                ActionType at = (ActionType)int.Parse(dr[TrackAttr.ActionType].ToString());

                string tag = dr[TrackAttr.Tag].ToString(); //标识.
                string mypk = dr[TrackAttr.MyPK].ToString(); //主键.

                string msg = "";
                if (at == ActionType.CallChildenFlow)
                {
                    //被调用父流程吊起。
                    if (DataType.IsNullOrEmpty(tag) == false)
                    {
                        AtPara ap = new AtPara(tag);
                        GenerWorkFlow mygwf = new GenerWorkFlow();
                        mygwf.WorkID = ap.GetValInt64ByKey("PWorkID");
                        if (mygwf.RetrieveFromDBSources() == 1)
                            msg = "<p>オペレーター:{" + dr[TrackAttr.EmpFromT].ToString() + "}現在のノードでは、親フロー{" + mygwf.FlowName + "},<a target=b" + ap.GetValStrByKey("PWorkID") + " href='Track.aspx?WorkID=" + ap.GetValStrByKey("PWorkID") + "&FK_Flow=" + ap.GetValStrByKey("PFlowNo") + "' >" + msg + "</a></p>";
                        else
                            msg = "<p>オペレーター:{" + dr[TrackAttr.EmpFromT].ToString() + "}現在のノードで、親フロー{" + mygwf.FlowName + "}、しかしフローは削除されました。</p>" + tag;

                        msg = "<a target=b" + ap.GetValStrByKey("PWorkID") + " href='Track.aspx?WorkID=" + ap.GetValStrByKey("PWorkID") + "&FK_Flow=" + ap.GetValStrByKey("PFlowNo") + "' >" + msg + "</a>";
                    }

                    //放入到ht里面.
                    ht.Add(mypk, msg);
                }

                if (at == ActionType.StartChildenFlow)
                {
                    if (DataType.IsNullOrEmpty(tag) == false)
                    {
                        if (tag.Contains("Sub"))
                            tag = tag.Replace("Sub", "C");

                        AtPara ap = new AtPara(tag);
                        GenerWorkFlow mygwf = new GenerWorkFlow();
                        mygwf.WorkID = ap.GetValInt64ByKey("CWorkID");
                        if (mygwf.RetrieveFromDBSources() == 1)
                        {
                            msg = "<p>オペレーター:{" + dr[TrackAttr.EmpFromT].ToString() + "}サブフローは現在のノードで呼び出されます{" + mygwf.FlowName + "}, <a target=b" + ap.GetValStrByKey("CWorkID") + " href='Track.aspx?WorkID=" + ap.GetValStrByKey("CWorkID") + "&FK_Flow=" + ap.GetValStrByKey("CFlowNo") + "' >" + msg + "</a></p>";
                            msg += "<p>現在のサブフローのステータス：{" + mygwf.WFStateText + "}、実行先：{" + mygwf.NodeName + "}、最後のハンドラ{" + mygwf.TodoEmps + "}、最後の処理時間{" + mygwf.RDT + "}。</p>";
                        }
                        else
                            msg = "<p>オペレーター:{" + dr[TrackAttr.EmpFromT].ToString() + "}サブフローは現在のノードで呼び出されます{" + mygwf.FlowName + "}、しかしフローは削除されました。</p>" + tag;

                    }

                    //放入到ht里面.
                    ht.Add(mypk, msg);
                }
            }
            #endregion

            //获取 WF_GenerWorkFlow
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.WorkID;
            gwf.RetrieveFromDBSources();
            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            if (gwf.WFState != WFState.Complete)
            {
                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID);

                //warning 补偿式的更新.  做特殊的判断，当会签过了以后仍然能够看isPass=90的错误数据.
                foreach (GenerWorkerList item in gwls)
                {
                    if (item.IsPassInt == 90 && gwf.FK_Node != item.FK_Node)
                    {
                        item.IsPassInt = 0;
                        item.Update();
                    }
                }
                ds.Tables.Add(gwls.ToDataTableField("WF_GenerWorkerList"));
            }

            //把节点审核配置信息.
            FrmWorkCheck fwc = new FrmWorkCheck(gwf.FK_Node);
            ds.Tables.Add(fwc.ToDataTableField("FrmWorkCheck"));

            //返回结果.
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        public string TimeBase_OpenFrm()
        {
            WF en = new WF();
            return en.Runing_OpenFrm();
        }
        /// <summary>
        /// 返回打开路径
        /// </summary>
        /// <returns></returns>
        public string FrmGuide_Init()
        {
            WF en = new WF();
            return en.Runing_OpenFrm();
        }


        #region 执行父类的重写方法.

        #endregion 执行父类的重写方法.

        #region 属性.
        public string Msg
        {
            get
            {
                string str = this.GetRequestVal("TB_Msg");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string UserName
        {
            get
            {
                string str = this.GetRequestVal("UserName");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string Title
        {
            get
            {
                string str = this.GetRequestVal("Title");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 字典表
        /// </summary>
        public string FK_SFTable
        {
            get
            {
                string str = this.GetRequestVal("FK_SFTable");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;

            }
        }
        public string EnumKey
        {
            get
            {
                string str = this.GetRequestVal("EnumKey");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;

            }
        }


        public string Name
        {
            get
            {
                string str = BP.Web.WebUser.Name;
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        #endregion 属性.

        public string FlowBBS_Delete()
        {
            return BP.WF.Dev2Interface.Flow_BBSDelete(this.FK_Flow, this.MyPK, WebUser.No);
        }
        /// <summary>
        /// 执行撤销
        /// </summary>
        /// <returns></returns>
        public string OP_UnSend()
        {
            //获取用户当前所在的节点
            String currNode = "";
            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                    currNode = "(SELECT FK_Node FROM (SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' Order by RDT DESC ) WHERE rownum=1)";
                    break;
                case DBType.MySQL:
                case DBType.PostgreSQL:
                    currNode = "(SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' Order by RDT DESC LIMIT 1)";
                    break;
                case DBType.MSSQL:
                    currNode = "(SELECT TOP 1 FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' Order by RDT DESC)";
                    break;
                default:
                    break;
            }
            String unSendToNode = DBAccess.RunSQLReturnString(currNode);
            try
            {
                return BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.WorkID, int.Parse(unSendToNode), this.FID);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        protected override string DoDefaultMethod()
        {
            return "err@判断なしの実行タイプ：" + this.DoType + " @クラス" + this.ToString();
        }

        public string OP_ComeBack()
        {
            WorkFlow wf3 = new WorkFlow(FK_Flow, WorkID);
            wf3.DoComeBackWorkFlow("なし");
            return "フローが再アクティブ化されました。";
        }

        public string OP_UnHungUp()
        {
            WorkFlow wf2 = new WorkFlow(FK_Flow, WorkID);
            //  wf2.DoUnHungUp();
            return "フローは中断されていません。";
        }

        public string OP_HungUp()
        {
            WorkFlow wf1 = new WorkFlow(FK_Flow, WorkID);
            //wf1.DoHungUp()
            return "フローは中断されました。";
        }

        public string OP_DelFlow()
        {
            WorkFlow wf = new WorkFlow(FK_Flow, WorkID);
            wf.DoDeleteWorkFlowByReal(true);
            return "フローが削除されました！";
        }

        /// <summary>
        /// 获取可操作状态信息
        /// </summary>
        /// <returns></returns>
        public string OP_GetStatus()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Hashtable ht = new Hashtable();

            bool CanPackUp = true; //是否可以打包下载.

            #region  PowerModel权限的解析
            string psql = "SELECT A.PowerFlag,A.EmpNo,A.EmpName FROM WF_PowerModel A WHERE PowerCtrlType =1"
             + " UNION "
             + "SELECT A.PowerFlag,B.No,B.Name FROM WF_PowerModel A, Port_Emp B, Port_Deptempstation C WHERE A.PowerCtrlType = 0 AND B.No = C.FK_Emp AND A.StaNo = C.FK_Station";
            psql = "SELECT PowerFlag From(" + psql + ")D WHERE  D.EmpNo='" + WebUser.No + "'";

            string powers = DBAccess.RunSQLReturnStringIsNull(psql, "");

            #endregion PowerModel权限的解析

            #region 文件打印的权限判断，这里为天业集团做的特殊判断，现实的应用中，都可以打印.
            if (SystemConfig.CustomerNo == "TianYe" && WebUser.No != "admin")
                CanPackUp = IsCanPrintSpecForTianYe(gwf);
            #endregion 文件打印的权限判断，这里为天业集团做的特殊判断，现实的应用中，都可以打印.
            if (CanPackUp == true)
                ht.Add("CanPackUp", 1);
            else
                ht.Add("CanPackUp", 0);

            //获取打印的方式PDF/RDF,节点打印方式
            Node nd = new Node(this.FK_Node);
            if (nd.HisPrintDocEnable == true)
                ht.Add("PrintType", 1);
            else
                ht.Add("PrintType", 0);


            //是否可以打印.
            switch (gwf.WFState)
            {
                case WFState.Runing: /* 运行时*/
                    /*删除流程.*/
                    if (BP.WF.Dev2Interface.Flow_IsCanDeleteFlowInstance(this.FK_Flow, this.WorkID, WebUser.No) == true)
                        ht.Add("IsCanDelete", 1);
                    else
                        ht.Add("IsCanDelete", 0);

                    if (powers.Contains("FlowDataDelete") == true)
                    {
                        //存在移除这个键值
                        if (ht.ContainsKey("IsCanDelete") == true)
                            ht.Remove("IsCanDelete");
                        ht.Add("IsCanDelete", 1);
                    }


                    /*取回审批*/
                    string para = "";
                    string sql = "SELECT NodeID FROM WF_Node WHERE CheckNodes LIKE '%" + gwf.FK_Node + "%'";
                    int myNode = DBAccess.RunSQLReturnValInt(sql, 0);
                    if (myNode != 0)
                    {
                        GetTask gt = new GetTask(myNode);
                        if (gt.Can_I_Do_It())
                        {
                            ht.Add("TackBackFromNode", gwf.FK_Node);
                            ht.Add("TackBackToNode", myNode);
                            ht.Add("CanTackBack", 1);
                        }
                    }

                    if (SystemConfig.CustomerNo == "TianYe")
                    {
                        ht.Add("CanUnSend", 1);

                    }
                    else
                    {
                        /*撤销发送*/
                        GenerWorkerLists workerlists = new GenerWorkerLists();
                        QueryObject info = new QueryObject(workerlists);
                        info.AddWhere(GenerWorkerListAttr.FK_Emp, WebUser.No);
                        info.addAnd();
                        info.AddWhere(GenerWorkerListAttr.IsPass, "1");
                        info.addAnd();
                        info.AddWhere(GenerWorkerListAttr.IsEnable, "1");
                        info.addAnd();
                        info.AddWhere(GenerWorkerListAttr.WorkID, this.WorkID);

                        if (info.DoQuery() > 0)
                            ht.Add("CanUnSend", 1);
                        else
                            ht.Add("CanUnSend", 0);

                        if (powers.Contains("FlowDataUnSend") == true)
                        {
                            //存在移除这个键值
                            if (ht.ContainsKey("CanUnSend") == true)
                                ht.Remove("CanUnSend");
                            ht.Add("CanUnSend", 1);
                        }

                    }


                    //流程结束
                    if (powers.Contains("FlowDataOver") == true)
                    {
                        ht.Add("CanFlowOver", 1);
                    }

                    //催办
                    if (powers.Contains("FlowDataPress") == true)
                    {
                        ht.Add("CanFlowPress", 1);
                    }



                    //是否可以调整工时
                    sql = "SELECT CHRole \"CHRole\" From WF_GenerWorkerList G,WF_Node N Where G.FK_Node=N.NodeID AND N.CHRole!=0 AND WorkID=" + this.WorkID + " AND FK_Emp='" + WebUser.No + "'";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (Int32.Parse(dr["CHRole"].ToString()) == 1 || Int32.Parse(dr["CHRole"].ToString()) == 3)
                            {
                                ht.Add("CanChangCHRole", 1);
                                break;
                            }
                            else
                            {
                                ht.Add("CanChangCHRole", 2);
                            }

                        }



                    }



                    break;
                case WFState.Complete: // 完成.
                case WFState.Delete:   // 逻辑删除..
                    /*恢复使用流程*/
                    if (WebUser.No == "admin")
                        ht.Add("CanRollBack", 1);
                    else
                        ht.Add("CanRollBack", 0);

                    if (powers.Contains("FlowDataRollback") == true)
                    {
                        //存在移除这个键值
                        if (ht.ContainsKey("CanRollBack") == true)
                            ht.Remove("CanRollBack");
                        ht.Add("CanRollBack", 1);
                    }

                    if (nd.CHRole != 0)//0禁用 1 启用 2 只读 3 启用并可以调整流程应完成时间
                    {
                        ht.Add("CanChangCHRole", 2);

                    }


                    //判断是否可以打印.
                    break;
                case WFState.HungUp: // 挂起.
                    /*撤销挂起*/
                    if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(WorkID, WebUser.No) == false)
                        ht.Add("CanUnHungUp", 0);
                    else
                        ht.Add("CanUnHungUp", 1);
                    break;
                default:
                    break;
            }

            return BP.Tools.Json.ToJson(ht);

            //return json + "}";
        }
        /// <summary>
        /// 是否可以打印.
        /// </summary>
        /// <param name="gwf"></param>
        /// <returns></returns>
        private bool IsCanPrintSpecForTianYe(GenerWorkFlow gwf)
        {
            //如果已经完成了，并且节点不是最后一个节点就不能打印.
            if (gwf.WFState == WFState.Complete)
            {
                Node nd = new Node(gwf.FK_Node);
                if (nd.IsEndNode == false)
                    return false;
            }

            // 判断是否可以打印.
            string sql = "SELECT Distinct NDFrom, EmpFrom FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID=" + this.WorkID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                //判断节点是否启用了按钮?
                int nodeid = int.Parse(dr[0].ToString());
                BtnLab btn = new BtnLab(nodeid);
                if (btn.PrintPDFEnable == true || btn.PrintZipEnable == true)
                {
                    string empFrom = dr[1].ToString();
                    if (gwf.WFState == BP.WF.WFState.Complete && (BP.Web.WebUser.No == empFrom || gwf.Starter == WebUser.No))
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 获取附件列表及单据列表
        /// </summary>
        /// <returns></returns>
        public string GetAthsAndBills()
        {
            string sql = "";
            string json = "{\"Aths\":";

            if (FK_Node == 0)
                sql = "SELECT fadb.*,wn.Name NodeName FROM Sys_FrmAttachmentDB fadb INNER JOIN WF_Node wn ON wn.NodeID = fadb.NodeID WHERE fadb.FK_FrmAttachment IN (SELECT MyPK FROM Sys_FrmAttachment WHERE  " + BP.WF.Glo.MapDataLikeKey(this.FK_Flow, "FK_MapData") + "  AND IsUpload=1) AND fadb.RefPKVal='" + this.WorkID + "' ORDER BY fadb.RDT";
            else
                sql = "SELECT fadb.*,wn.Name NodeName FROM Sys_FrmAttachmentDB fadb INNER JOIN WF_Node wn ON wn.NodeID = fadb.NodeID WHERE fadb.FK_FrmAttachment IN (SELECT MyPK FROM Sys_FrmAttachment WHERE  FK_MapData='ND" + FK_Node + "' ) AND fadb.RefPKVal='" + this.WorkID + "' ORDER BY fadb.RDT";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            foreach (DataColumn col in dt.Columns)
                col.ColumnName = col.ColumnName.ToUpper();

            json += BP.Tools.Json.ToJson(dt) + ",\"Bills\":";

            Bills bills = new Bills();
            bills.Retrieve(BillAttr.WorkID, this.WorkID);

            json += bills.ToJson() + ",\"AppPath\":\"" + BP.WF.Glo.CCFlowAppPath + "\"}";

            return json;
        }
        /// <summary>
        /// 获取OneWork页面的tabs集合
        /// </summary>
        /// <returns></returns>
        public string OneWork_GetTabs()
        {
            string re = "[";

            OneWorkXmls xmls = new OneWorkXmls();
            xmls.RetrieveAll();

            int nodeID = this.FK_Node;
            if (nodeID == 0)
            {
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                nodeID = gwf.FK_Node;
            }

            Node nd = new Node(nodeID);

            foreach (OneWorkXml item in xmls)
            {
                string url = "";
                url = string.Format("{0}?FK_Node={1}&WorkID={2}&FK_Flow={3}&FID={4}&FromWorkOpt=1", item.URL, nodeID.ToString(), this.WorkID, this.FK_Flow, this.FID);
                if (item.No.Equals("Frm") && (nd.HisFormType == NodeFormType.SDKForm || nd.HisFormType == NodeFormType.SelfForm))
                {
                    if (nd.FormUrl.Contains("?"))
                        url = "@url=" + nd.FormUrl + "&IsReadonly=1&WorkID=" + this.WorkID + "&FK_Node=" + nodeID.ToString() + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FromWorkOpt=1";
                    else
                        url = "@url=" + nd.FormUrl + "?IsReadonly=1&WorkID=" + this.WorkID + "&FK_Node=" + nodeID.ToString() + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FromWorkOpt=1";
                }
                re += "{" + string.Format("\"No\":\"{0}\",\"Name\":\"{1}\", \"Url\":\"{2}\",\"IsDefault\":\"{3}\"", item.No, item.Name, url, item.IsDefault) + "},";
            }

            return re.TrimEnd(',') + "]";
        }
        /// <summary>
        /// 获取流程的JSON数据，以供显示工作轨迹/流程设计
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作编号</param>
        /// <param name="fid">父流程编号</param>
        /// <returns></returns>
        public string Chart_Init2020()
        {
            //参数.
            string fk_flow = this.FK_Flow;
            Int64 workid = this.WorkID;
            Int64 fid = this.FID;

            DataSet ds = new DataSet();
            DataTable dt = null;
            string json = string.Empty;
            try
            {
                //流程信息
                var sql = "SELECT No \"No\", Name \"Name\", Paras \"Paras\", ChartType \"ChartType\" FROM WF_Flow WHERE No='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Flow";
                ds.Tables.Add(dt);

                //节点信息 ， 
                // NodePosType=0，开始节点， 1中间节点,2=结束节点.
                // RunModel= select * from sys_enum where Enumkey='RunModel' 
                // TodolistModel= select * from sys_enum where Enumkey='TodolistModel' ;
                sql = "SELECT NodeID \"ID\", Name \"Name\", ICON \"Icon\", X \"X\", Y \"Y\", NodePosType \"NodePosType\",RunModel \"RunModel\",HisToNDs \"HisToNDs\",TodolistModel \"TodolistModel\" FROM WF_Node WHERE FK_Flow='" +
                    fk_flow + "' ORDER BY Step";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Node";
                ds.Tables.Add(dt);

                //标签信息
                sql = "SELECT MyPK \"MyPK\", Name \"Name\", X \"X\", Y \"Y\" FROM WF_LabNote WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_LabNote";
                ds.Tables.Add(dt);

                //线段方向信息
                sql = "SELECT Node \"Node\", ToNode \"ToNode\", 0 as  \"DirType\", 0 as \"IsCanBack\",Dots \"Dots\" FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Direction";
                ds.Tables.Add(dt);

                //如果workid=0就仅仅返回流程图数据.
                if (workid == 0)
                    return BP.Tools.Json.DataSetToJson(ds);


                //流程信息.
                GenerWorkFlow gwf = new GenerWorkFlow(workid);
                dt = gwf.ToDataTableField(); // DBAccess.RunSQLReturnTable(string.Format(sql, workid));
                dt.TableName = "WF_GenerWorkFlow";
                ds.Tables.Add(dt);

                //把节点审核配置信息.
                FrmWorkCheck fwc = new FrmWorkCheck(gwf.FK_Node);
                ds.Tables.Add(fwc.ToDataTableField("FrmWorkCheck"));


                //获取工作轨迹信息
                var trackTable = "ND" + int.Parse(fk_flow) + "Track";
                sql = "SELECT FID \"FID\",NDFrom \"NDFrom\",NDFromT \"NDFromT\",NDTo  \"NDTo\", NDToT \"NDToT\", ActionType \"ActionType\",ActionTypeText \"ActionTypeText\",Msg \"Msg\",RDT \"RDT\",EmpFrom \"EmpFrom\",EmpFromT \"EmpFromT\", EmpToT \"EmpToT\",EmpTo \"EmpTo\" FROM " + trackTable +
                      " WHERE WorkID=" +
                      workid + (fid == 0 ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid)) + " ORDER BY RDT DESC";
                dt = DBAccess.RunSQLReturnTable(sql);

                DataTable newdt = new DataTable();
                newdt = dt.Clone();

                #region 判断轨迹数据中，最后一步是否是撤销或退回状态的，如果是，则删除最后2条数据
                if (dt.Rows.Count > 0)
                {
                    if (Equals(dt.Rows[0]["ActionType"], (int)ActionType.Return)
                        || Equals(dt.Rows[0]["ActionType"], (int)ActionType.UnSend))
                    {
                        if (dt.Rows.Count > 1)
                        {
                            dt.Rows.RemoveAt(1);
                            dt.Rows.RemoveAt(1);
                        }
                        else
                        {
                            dt.Rows.RemoveAt(0);
                        }
                        newdt = dt;
                    }
                    else if (dt.Rows.Count > 1 && (Equals(dt.Rows[1]["ActionType"], (int)ActionType.Return) || Equals(dt.Rows[1]["ActionType"], (int)ActionType.UnSend)))
                    {
                        //删除已发送的节点，
                        if (dt.Rows.Count > 3)
                        {
                            dt.Rows.RemoveAt(1);
                            dt.Rows.RemoveAt(1);
                        }
                        else
                        {
                            dt.Rows.RemoveAt(1);
                        }

                        string fk_node = "";
                        if (dt.Rows[0]["NDFrom"].Equals(dt.Rows[0]["NDTo"]))
                            fk_node = dt.Rows[0]["NDFrom"].ToString();
                        if (DataType.IsNullOrEmpty(fk_node) == false)
                        {
                            //如果是跳转页面，则需要删除中间跳转的节点
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (Equals(dr["ACTIONTYPE"], (int)ActionType.Skip) && dr["NDFrom"].ToString().Equals(fk_node))
                                    continue;
                                DataRow newdr = newdt.NewRow();
                                newdr.ItemArray = dr.ItemArray;
                                newdt.Rows.Add(newdr);
                            }
                        }
                        else
                        {
                            newdt = dt.Copy();
                        }
                    }
                    else
                        newdt = dt.Copy();
                }
                newdt.TableName = "Track";
                ds.Tables.Add(newdt);
                #endregion

                #region 如果流程没有完成,就把工作人员列表返回过去.
                if (gwf.WFState != WFState.Complete)
                {
                    //加入工作人员列表.
                    GenerWorkerLists gwls = new GenerWorkerLists();
                    Int64 id = this.FID;
                    if (id == 0)
                        id = this.WorkID;

                    QueryObject qo = new QueryObject(gwls);
                    qo.AddWhere(GenerWorkerListAttr.FID, id);
                    qo.addOr();
                    qo.AddWhere(GenerWorkerListAttr.WorkID, id);
                    qo.DoQuery();

                    DataTable dtGwls = gwls.ToDataTableField("WF_GenerWorkerList");
                    ds.Tables.Add(dtGwls);
                }
                #endregion 如果流程没有完成,就把工作人员列表返回过去.

                string str= BP.Tools.Json.DataSetToJson(ds);
                 DataType.WriteFile("c:\\GetFlowTrackJsonData_CCflow.txt", str);
                 return str;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string Chart_Init()
        {
            string fk_flow = this.FK_Flow;
            Int64 workid = this.WorkID;
            Int64 fid = this.FID;

            DataSet ds = new DataSet();
            DataTable dt = null;
            string json = string.Empty;
            try
            {
                //获取流程信息
                var sql = "SELECT No \"No\", Name \"Name\", Paras \"Paras\", ChartType \"ChartType\" FROM WF_Flow WHERE No='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Flow";
                ds.Tables.Add(dt);

                //获取流程中的节点信息
                sql = "SELECT NodeID \"ID\", Name \"Name\", ICON \"Icon\", X \"X\", Y \"Y\", NodePosType \"NodePosType\",RunModel \"RunModel\",HisToNDs \"HisToNDs\",TodolistModel \"TodolistModel\" FROM WF_Node WHERE FK_Flow='" +
                    fk_flow + "' ORDER BY Step";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Node";
                ds.Tables.Add(dt);

                //获取流程中的标签信息
                sql = "SELECT MyPK \"MyPK\", Name \"Name\", X \"X\", Y \"Y\" FROM WF_LabNote WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_LabNote";
                ds.Tables.Add(dt);

                //获取流程中的线段方向信息
                sql = "SELECT Node \"Node\", ToNode \"ToNode\", 0 as  \"DirType\", 0 as \"IsCanBack\",Dots \"Dots\" FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Direction";
                ds.Tables.Add(dt);

                if (workid != 0)
                {
                    //获取流程信息，added by liuxc,2016-10-26
                    //sql =
                    //    "SELECT wgwf.Starter,wgwf.StarterName,wgwf.RDT,wgwf.WFSta,wgwf.WFState FROM WF_GenerWorkFlow wgwf WHERE wgwf.WorkID = " +
                    //    workid;                     
                    sql = "SELECT wgwf.Starter as \"Starter\","
                          + "        wgwf.StarterName as \"StarterName\","
                          + "        wgwf.RDT as \"RDT\","
                          + "        wgwf.WFSta as \"WFSta\","
                          + "        se.Lab  as   \"WFStaText\","
                          + "        wgwf.WFState as \"WFState\","
                          + "        wgwf.FID as \"FID\","
                          + "        wgwf.PWorkID as \"PWorkID\","
                          + "        wgwf.PFlowNo as \"PFlowNo\","
                          + "        wgwf.PNodeID as \"PNodeID\","
                          + "        wgwf.FK_Flow as \"FK_Flow\","
                          + "        wgwf.FK_Node as \"FK_Node\","
                          + "        wgwf.Title as \"Title\","
                          + "        wgwf.WorkID as \"WorkID\","
                          + "        wgwf.NodeName as \"NodeName\","
                          + "        wf.Name  as   \"FlowName\""
                          + " FROM   WF_GenerWorkFlow wgwf"
                          + "        INNER JOIN WF_Flow wf"
                          + "             ON  wf.No = wgwf.FK_Flow"
                          + "        INNER JOIN Sys_Enum se"
                          + "             ON  se.IntKey = wgwf.WFSta"
                          + "             AND se.EnumKey = 'WFSta'"
                          + " WHERE  wgwf.WorkID = {0}"
                          + "        OR  wgwf.FID = {0}"
                          + "        OR  wgwf.PWorkID = {0}"
                          + " ORDER BY"
                          + "        wgwf.RDT DESC";

                    dt = DBAccess.RunSQLReturnTable(string.Format(sql, workid));
                    dt.TableName = "FlowInfo";
                    ds.Tables.Add(dt);

                    //获得流程状态.
                    WFState wfState = (WFState)int.Parse(dt.Select("workid=" + workid + "")[0]["wfstate"].ToString());// (WFState)int.Parse(dt.Rows[0]["WFState"].ToString());

                    String fk_Node = dt.Rows[0]["FK_Node"].ToString();

                    //把节点审核配置信息.
                    FrmWorkCheck fwc = new FrmWorkCheck(fk_Node);
                    ds.Tables.Add(fwc.ToDataTableField("FrmWorkCheck"));


                    //获取工作轨迹信息
                    var trackTable = "ND" + int.Parse(fk_flow) + "Track";
                    sql = "SELECT FID \"FID\",NDFrom \"NDFrom\",NDFromT \"NDFromT\",NDTo  \"NDTo\", NDToT \"NDToT\", ActionType \"ActionType\",ActionTypeText \"ActionTypeText\",Msg \"Msg\",RDT \"RDT\",EmpFrom \"EmpFrom\",EmpFromT \"EmpFromT\", EmpToT \"EmpToT\",EmpTo \"EmpTo\" FROM " + trackTable +
                          " WHERE WorkID=" +
                          workid + (fid == 0 ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid)) + " ORDER BY RDT DESC";

                    dt = DBAccess.RunSQLReturnTable(sql);
                    DataTable newdt = new DataTable();
                    newdt = dt.Clone();

                    //判断轨迹数据中，最后一步是否是撤销或退回状态的，如果是，则删除最后2条数据
                    if (dt.Rows.Count > 0)
                    {
                        if (Equals(dt.Rows[0]["ActionType"], (int)ActionType.Return) || Equals(dt.Rows[0]["ActionType"], (int)ActionType.UnSend))
                        {
                            if (dt.Rows.Count > 1)
                            {
                                dt.Rows.RemoveAt(1);
                                dt.Rows.RemoveAt(1);
                            }
                            else
                            {
                                dt.Rows.RemoveAt(0);
                            }

                            newdt = dt;
                        }
                        else if (dt.Rows.Count > 1 && (Equals(dt.Rows[1]["ActionType"], (int)ActionType.Return) || Equals(dt.Rows[1]["ActionType"], (int)ActionType.UnSend)))
                        {
                            //删除已发送的节点，
                            if (dt.Rows.Count > 3)
                            {
                                dt.Rows.RemoveAt(1);
                                dt.Rows.RemoveAt(1);
                            }
                            else
                            {
                                dt.Rows.RemoveAt(1);
                            }

                            string fk_node = "";
                            if (dt.Rows[0]["NDFrom"].Equals(dt.Rows[0]["NDTo"]))
                                fk_node = dt.Rows[0]["NDFrom"].ToString();
                            if (DataType.IsNullOrEmpty(fk_node) == false)
                            {
                                //如果是跳转页面，则需要删除中间跳转的节点
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (Equals(dr["ACTIONTYPE"], (int)ActionType.Skip) && dr["NDFrom"].ToString().Equals(fk_node))
                                        continue;
                                    DataRow newdr = newdt.NewRow();
                                    newdr.ItemArray = dr.ItemArray;
                                    newdt.Rows.Add(newdr);
                                }
                            }
                            else
                            {
                                newdt = dt.Copy();
                            }
                        }
                        else
                            newdt = dt.Copy();
                    }

                    newdt.TableName = "Track";
                    ds.Tables.Add(newdt);

                    //获取预先计算的节点处理人，以及处理时间,added by liuxc,2016-4-15
                    sql = "SELECT wsa.FK_Node as \"FK_Node\",wsa.FK_Emp as \"FK_Emp\",wsa.EmpName as \"EmpName\",wsa.TimeLimit as \"TimeLimit\",wsa.TSpanHour as \"TSpanHour\",wsa.ADT as \"ADT\",wsa.SDT as \"SDT\" FROM WF_SelectAccper wsa WHERE wsa.WorkID = " + workid;
                    dt = DBAccess.RunSQLReturnTable(sql);
                    // dt.TableName = "POSSIBLE";
                    dt.TableName = "Possible";
                    ds.Tables.Add(dt);

                    //获取节点处理人数据，及处理/查看信息
                    sql = "SELECT wgw.FK_Emp as \"FK_Emp\",wgw.FK_Node as \"FK_Node\",wgw.FK_EmpText as \"FK_EmpText\",wgw.RDT as \"RDT\",wgw.IsRead as \"IsRead\",wgw.IsPass as \"IsPass\" FROM WF_GenerWorkerlist wgw WHERE wgw.WorkID = " +
                          workid + (fid == 0 ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid));
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "DISPOSE";
                    ds.Tables.Add(dt);


                    //如果流程没有完成.
                    if (wfState != WFState.Complete)
                    {
                        GenerWorkerLists gwls = new GenerWorkerLists();
                        Int64 id = this.FID;
                        if (id == 0)
                            id = this.WorkID;

                        QueryObject qo = new QueryObject(gwls);
                        qo.AddWhere(GenerWorkerListAttr.FID, id);
                        qo.addOr();
                        qo.AddWhere(GenerWorkerListAttr.WorkID, id);
                        qo.DoQuery();

                        DataTable dtGwls = gwls.ToDataTableField("WF_GenerWorkerList");
                        ds.Tables.Add(dtGwls);
                    }

                }
                else
                {
                    var trackTable = "ND" + int.Parse(fk_flow) + "Track";
                    sql = "SELECT NDFrom \"NDFrom\", NDTo \"NDTo\",ActionType \"ActionType\",ActionTypeText \"ActionTypeText\",Msg \"Msg\",RDT \"RDT\",EmpFrom \"EmpFrom\",EmpFromT \"EmpFromT\",EmpToT \"EmpToT\",EmpTo \"EmpTo\" FROM " + trackTable +
                          " WHERE WorkID=0 ORDER BY RDT ASC";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "TRACK";
                    ds.Tables.Add(dt);
                }

                //for (int i = 0; i < ds.Tables.Count; i++)
                //{
                //    dt = ds.Tables[i];
                //    dt.TableName = dt.TableName.ToUpper();
                //    for (int j = 0; j < dt.Columns.Count; j++)
                //    {
                //        dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.ToUpper();
                //    }
                //}

                string str = BP.Tools.Json.DataSetToJson(ds);
                //  DataType.WriteFile("c:\\GetFlowTrackJsonData_CCflow.txt", str);
                return str;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return json;
        }
        /// <summary>
        /// 获取流程的JSON数据，以供显示工作轨迹/流程设计
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作编号</param>
        /// <param name="fid">父流程编号</param>
        /// <returns></returns>
        public string GetFlowTrackJsonData()
        {
            string fk_flow = this.FK_Flow;
            Int64 workid = this.WorkID;
            Int64 fid = this.FID;


            DataSet ds = new DataSet();
            DataTable dt = null;
            try
            {
                //获取流程信息
                var sql = "SELECT No \"No\", Name \"Name\", Paras \"Paras\", ChartType \"ChartType\" FROM WF_Flow WHERE No='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_FLOW";
                ds.Tables.Add(dt);

                //获取流程中的节点信息
                sql = "SELECT NodeID \"ID\", Name \"Name\", ICON \"Icon\", X \"X\", Y \"Y\", NodePosType \"NodePosType\", RunModel \"RunModel\",HisToNDs \"HisToNDs\",TodolistModel \"TodolistModel\" FROM WF_Node WHERE FK_Flow='" +
                    fk_flow + "' ORDER BY Step";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_NODE";
                ds.Tables.Add(dt);

                //获取流程中的标签信息
                sql = "SELECT MyPK \"MyPK\", Name \"Name\", X \"X\", Y \"Y\" FROM WF_LabNote WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_LABNOTE";
                ds.Tables.Add(dt);

                //获取流程中的线段方向信息
                sql = "SELECT Node \"Node\", ToNode \"ToNode\", 0 as  \"DirType\", 0 as \"IsCanBack\",Dots \"Dots\" FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_DIRECTION";
                ds.Tables.Add(dt);

                if (workid != 0)
                {
                    //获取流程信息，added by liuxc,2016-10-26
                    //sql =
                    //    "SELECT wgwf.Starter,wgwf.StarterName,wgwf.RDT,wgwf.WFSta,wgwf.WFState FROM WF_GenerWorkFlow wgwf WHERE wgwf.WorkID = " +
                    //    workid;
                    sql = "SELECT wgwf.Starter as \"Starter\","
                          + "        wgwf.StarterName as \"StarterName\","
                          + "        wgwf.RDT as \"RDT\","
                          + "        wgwf.WFSta as \"WFSta\","
                          + "        se.Lab  as   \"WFStaText\","
                          + "        wgwf.WFState as \"WFState\","
                          + "        wgwf.FID as \"FID\","
                          + "        wgwf.PWorkID as \"PWorkID\","
                          + "        wgwf.PFlowNo as \"PFlowNo\","
                          + "        wgwf.PNodeID as \"PNodeID\","
                          + "        wgwf.FK_Flow as \"FK_Flow\","
                          + "        wgwf.FK_Node as \"FK_Node\","
                          + "        wgwf.Title as \"Title\","
                          + "        wgwf.WorkID as \"WorkID\","
                          + "        wgwf.NodeName as \"NodeName\","
                          + "        wf.Name  as   \"FlowName\""
                          + " FROM   WF_GenerWorkFlow wgwf"
                          + "        INNER JOIN WF_Flow wf"
                          + "             ON  wf.No = wgwf.FK_Flow"
                          + "        INNER JOIN Sys_Enum se"
                          + "             ON  se.IntKey = wgwf.WFSta"
                          + "             AND se.EnumKey = 'WFSta'"
                          + " WHERE  wgwf.WorkID = {0}"
                          + "        OR  wgwf.FID = {0}"
                          + "        OR  wgwf.PWorkID = {0}"
                          + " ORDER BY"
                          + "        wgwf.RDT DESC";

                    dt = DBAccess.RunSQLReturnTable(string.Format(sql, workid));
                    dt.TableName = "FLOWINFO";
                    ds.Tables.Add(dt);

                    //获取工作轨迹信息
                    var trackTable = "ND" + int.Parse(fk_flow) + "Track";
                    sql = "SELECT NDFrom \"NDFrom\",NDFromT \"NDFromT\",NDTo  \"NDTo\", NDToT \"NDToT\", ActionType \"ActionType\",ActionTypeText \"ActionTypeText\",Msg \"Msg\",RDT \"RDT\",EmpFrom \"EmpFrom\",EmpFromT \"EmpFromT\", EmpToT \"EmpToT\",EmpTo \"EmpTo\" FROM " + trackTable +
                          " WHERE WorkID=" +
                          workid + (fid == 0 ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid)) + " ORDER BY RDT ASC";

                    dt = DBAccess.RunSQLReturnTable(sql);

                    //判断轨迹数据中，最后一步是否是撤销或退回状态的，如果是，则删除最后2条数据
                    if (dt.Rows.Count > 0)
                    {
                        if (Equals(dt.Rows[0]["ACTIONTYPE"], (int)ActionType.Return) || Equals(dt.Rows[0]["ACTIONTYPE"], (int)ActionType.UnSend))
                        {
                            if (dt.Rows.Count > 1)
                            {
                                dt.Rows.RemoveAt(0);
                                dt.Rows.RemoveAt(0);
                            }
                            else
                            {
                                dt.Rows.RemoveAt(0);
                            }
                        }
                    }

                    dt.TableName = "TRACK";
                    ds.Tables.Add(dt);

                    //获取预先计算的节点处理人，以及处理时间,added by liuxc,2016-4-15
                    sql = "SELECT wsa.FK_Node as \"FK_Node\",wsa.FK_Emp as \"FK_Emp\",wsa.EmpName as \"EmpName\",wsa.TimeLimit as \"TimeLimit\",wsa.TSpanHour as \"TSpanHour\",wsa.ADT as \"ADT\",wsa.SDT as \"SDT\" FROM WF_SelectAccper wsa WHERE wsa.WorkID = " + workid;
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "POSSIBLE";
                    ds.Tables.Add(dt);

                    //获取节点处理人数据，及处理/查看信息
                    sql = "SELECT wgw.FK_Emp as \"FK_Emp\",wgw.FK_Node as \"FK_Node\",wgw.FK_EmpText as \"FK_EmpText\",wgw.RDT as \"RDT\",wgw.IsRead as \"IsRead\",wgw.IsPass as \"IsPass\" FROM WF_GenerWorkerlist wgw WHERE wgw.WorkID = " +
                          workid + (fid == 0 ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid));
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "DISPOSE";
                    ds.Tables.Add(dt);
                }
                else
                {
                    var trackTable = "ND" + int.Parse(fk_flow) + "Track";
                    sql = "SELECT NDFrom \"NDFrom\", NDTo \"NDTo\",ActionType \"ActionType\",ActionTypeText \"ActionTypeText\",Msg \"Msg\",RDT \"RDT\",EmpFrom \"EmpFrom\",EmpFromT \"EmpFromT\",EmpToT \"EmpToT\",EmpTo \"EmpTo\" FROM " + trackTable +
                          " WHERE WorkID=0 ORDER BY RDT ASC";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "TRACK";
                    ds.Tables.Add(dt);
                }

                //for (int i = 0; i < ds.Tables.Count; i++)
                //{
                //    dt = ds.Tables[i];
                //    dt.TableName = dt.TableName.ToUpper();
                //    for (int j = 0; j < dt.Columns.Count; j++)
                //    {
                //        dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.ToUpper();
                //    }
                //}

                string str = BP.Tools.Json.DataSetToJson(ds);
                //  DataType.WriteFile("c:\\GetFlowTrackJsonData_CCflow.txt", str);
                return str;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 获得发起的BBS评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBSList()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            ps.Add("WorkID", this.WorkID);

            //转化成json
            return BP.Tools.Json.ToJson(BP.DA.DBAccess.RunSQLReturnTable(ps));
        }

        /// 查看某一用户的评论.
        public string FlowBBS_Check()
        {
            Paras pss = new Paras();
            pss.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID AND  EMPFROMT='" + this.UserName + "'";
            pss.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            pss.Add("WorkID", this.WorkID);

            return BP.Tools.Json.ToJson(BP.DA.DBAccess.RunSQLReturnTable(pss));
        }
        /// <summary>
        /// 提交评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBS_Save()
        {
            string msg = this.GetValFromFrmByKey("TB_Msg");
            string mypk = BP.WF.Dev2Interface.Flow_BBSAdd(this.FK_Flow, this.WorkID, this.FID, msg, WebUser.No, WebUser.Name);
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE MyPK=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "MyPK";
            ps.Add("MyPK", mypk);
            return BP.Tools.Json.ToJson(BP.DA.DBAccess.RunSQLReturnTable(ps));
        }

        /// <summary>
        /// 回复评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBS_Replay()
        {
            SMS sms = new SMS();
            sms.RetrieveByAttr(SMSAttr.MyPK, MyPK);
            sms.MyPK = DBAccess.GenerGUID();
            sms.RDT = DataType.CurrentDataTime;
            sms.SendToEmpNo = this.UserName;
            sms.Sender = WebUser.No;
            sms.Title = this.Title;
            sms.DocOfEmail = this.Msg;
            sms.Insert();
            return null;
        }
        /// <summary>
        /// 统计评论条数.
        /// </summary>
        /// <returns></returns>
        public string FlowBBS_Count()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(ActionType) FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            ps.Add("WorkID", this.WorkID);
            string count = BP.DA.DBAccess.RunSQLReturnValInt(ps).ToString();
            return count;
        }

        public string Runing_OpenFrm()
        {
            BP.WF.HttpHandler.WF wf = new WF();
            return wf.Runing_OpenFrm();
        }
    }
}
