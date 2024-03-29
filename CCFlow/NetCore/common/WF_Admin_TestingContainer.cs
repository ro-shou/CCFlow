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
    public class WF_Admin_TestingContainer : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_TestingContainer()
        {
        }

        /// <summary>
        /// 左侧的树刷新
        /// </summary>
        /// <returns></returns>
        public string Left_Init()
        {
            return "";
        }
        /// <summary>
        /// 测试页面初始化
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            string userNo = this.GetRequestVal("UserNo");
            if (WebUser.No.Equals(userNo) == false)
                BP.WF.Dev2Interface.Port_Login(userNo);

            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, userNo);
            return workid.ToString();
        }

       
        /// <summary>
        /// 数据库信息
        /// </summary>
        /// <returns></returns>
        public string DBInfo_Init()
        {
            //数据容器，用于盛放数据，并返回json.
            DataSet ds = new DataSet();

            //流程引擎控制表.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            //流程引擎人员列表.
            GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID);
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.RDT);
            ds.Tables.Add(gwls.ToDataTableField("WF_GenerWorkerList"));


            //获得Track数据.
            string table = "ND" + int.Parse(this.FK_Flow) + "Track";
            string sql = "SELECT * FROM " + table + " WHERE WorkID=" + this.WorkID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Track";
            //把列大写转化为小写.
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                Track tk = new Track();
                foreach (Attr attr in tk.EnMap.Attrs)
                {
                    if (dt.Columns.Contains(attr.Key.ToUpper()) == true)
                    {
                        dt.Columns[attr.Key.ToUpper()].ColumnName = attr.Key;

                    }
                }
            }
            ds.Tables.Add(dt);

            //获得NDRpt表
            string rpt = "ND" + int.Parse(this.FK_Flow) + "Rpt";
            GEEntity en = new GEEntity(rpt);
            en.PKVal = this.WorkID;
            en.RetrieveFromDBSources();
            ds.Tables.Add(en.ToDataTableField("NDRpt"));


            //转化为json ,返回出去.
            return BP.Tools.Json.ToJson(ds);
        }



        public string Default_LetAdminerLogin()
        {
            string adminer = this.GetRequestVal("Adminer");
            string sid = this.GetRequestVal("SID");
            BP.WF.Dev2Interface.Port_LoginBySID(adminer, sid);

            return "ログイン成功。";
            //Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, userNo);
            //return workid.ToString();
        }
        /// <summary>
        /// 切换用户
        /// </summary>
        /// <returns></returns>
        public string SelectOneUser_ChangUser()
        {

            string adminer = this.GetRequestVal("Adminer");
            string SID = this.GetRequestVal("SID");


            //string userNo = DBAccess.RunSQLReturnString("SELECT No FROM Port_Emp where SID='" + SID + "'");
            //if (userNo.Equals(adminer) == false)
            //    return "err@非法用户.";

            //判断当前人员是否归属这个admin管理.  @hy. 以后在判断.

            //string adminer = this.GetRequestVal("Adminer");
            //string sid = this.GetRequestVal("SID");
            //BP.WF.Dev2Interface.Port_LoginBySID(adminer, sid);
            //string empNo = this.GetRequestVal("FK_Emp");

            try
            {
                BP.WF.Dev2Interface.Port_Login(this.FK_Emp);
                return "ログイン成功。";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

            //Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, userNo);
            //return workid.ToString();
        }




        #region TestFlow2020_Init
        /// <summary>
        /// 发起.
        /// </summary>
        /// <returns></returns>
        public string TestFlow2020_StartIt()
        {
            if (WebUser.IsAdmin == false)
                return "err@管理者以外はテストできません";

            //用户编号.
            string userNo = this.GetRequestVal("UserNo");

            //判断是否可以测试该流程？ 

            //组织url发起该流程.
            string url = "Default.html?RunModel=1&FK_Flow=" + this.FK_Flow + "&Adminer=" + WebUser.No + "&SID=" + WebUser.SID + "&UserNo=" + userNo;
            return url;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string TestFlow2020_Init()
        {
            //清除缓存.
            BP.Sys.SystemConfig.DoClearCash();

            if (1 == 2 && BP.Web.WebUser.IsAdmin == false)
                return "err@あなたは管理者ではないため、この操作を実行できません。";

            // 让admin 登录.
            //   BP.WF.Dev2Interface.Port_Login("admin");

            if (this.RefNo != null)
            {
                Emp emp = new Emp(this.RefNo);
                BP.Web.WebUser.SignInOfGener(emp);
                HttpContextHelper.SessionSet("FK_Flow", this.FK_Flow); //设置当前的流程.
                return "url@../MyFlow.htm?FK_Flow=" + this.FK_Flow;
            }

            FlowExt fl = new FlowExt(this.FK_Flow);

            if (1 == 2 && BP.Web.WebUser.No != "admin" && fl.Tester.Length <= 1)
            {
                string msg = "err@副管理者[" + BP.Web.WebUser.Name + "]こんにちは、このフローのテスターをまだ設定していません。";
                msg += "フロープロパティの下部にある[Set Process Initiation Tester]のプロパティで開始できるテスターを設定する必要があります。複数の担当者はカンマで区切られます。";
                return msg;
            }

            #region 检查.
            int nodeid = int.Parse(this.FK_Flow + "01");
            DataTable dt = null;
            string sql = "";
            BP.WF.Node nd = new BP.WF.Node(nodeid);
            if (nd.IsGuestNode)
            {
                /*如果是 guest 节点，就让其跳转到 guest登录界面，让其发起流程。*/
                //这个地址需要配置.
                return "url@/SDKFlowDemo/GuestApp/Login.htm?FK_Flow=" + this.FK_Flow;
            }
            #endregion 测试人员.



            #region 从配置里获取-测试人员.
            /* 检查是否设置了测试人员，如果设置了就按照测试人员身份进入
             * 设置测试人员的目的是太多了人员影响测试效率.
             * */
            if (fl.Tester.Length > 2)
            {
                // 构造人员表.
                DataTable dtEmps = new DataTable();
                dtEmps.Columns.Add("No");
                dtEmps.Columns.Add("Name");
                dtEmps.Columns.Add("FK_DeptText");

                string[] strs = fl.Tester.Split(',');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    Emp emp = new Emp();
                    emp.SetValByKey("No", str);
                    int i = emp.RetrieveFromDBSources();
                    if (i == 0)
                        continue;

                    DataRow dr = dtEmps.NewRow();
                    dr["No"] = emp.No;
                    dr["Name"] = emp.Name;
                    dr["FK_DeptText"] = emp.FK_DeptText;
                    dtEmps.Rows.Add(dr);
                }
                return BP.Tools.Json.ToJson(dtEmps);
            }
            #endregion 测试人员.

            //fl.DoCheck();

            #region 从设置里获取-测试人员.
            try
            {

                switch (nd.HisDeliveryWay)
                {
                    case DeliveryWay.ByStation:
                    case DeliveryWay.ByStationOnly:

                        sql = "SELECT Port_Emp.No  FROM Port_Emp LEFT JOIN Port_Dept   Port_Dept_FK_Dept ON  Port_Emp.FK_Dept=Port_Dept_FK_Dept.No  join Port_DeptEmpStation on (fk_emp=Port_Emp.No)   join WF_NodeStation on (WF_NodeStation.fk_station=Port_DeptEmpStation.fk_station) WHERE (1=1) AND  FK_Node=" + nd.NodeID;
                        // emps.RetrieveInSQL_Order("select fk_emp from Port_Empstation WHERE fk_station in (select fk_station from WF_NodeStation WHERE FK_Node=" + nodeid + " )", "FK_Dept");
                        break;
                    case DeliveryWay.ByDept:
                        sql = "select No,Name from Port_Emp where FK_Dept in (select FK_Dept from WF_NodeDept where FK_Node='" + nodeid + "') ";
                        //emps.RetrieveInSQL("");
                        break;
                    case DeliveryWay.ByBindEmp:
                        sql = "select No,Name from Port_Emp where No in (select FK_Emp from WF_NodeEmp where FK_Node='" + nodeid + "') ";
                        //emps.RetrieveInSQL("select fk_emp from wf_NodeEmp WHERE fk_node=" + int.Parse(this.FK_Flow + "01") + " ");
                        break;
                    case DeliveryWay.ByDeptAndStation:
                        //added by liuxc,2015.6.30.
                        //区别集成与BPM模式
                        if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                        {
                            sql = "SELECT No FROM Port_Emp WHERE No IN ";
                            sql += "(SELECT No as FK_Emp FROM Port_Emp WHERE FK_Dept IN ";
                            sql += "( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + nodeid + ")";
                            sql += ")";
                            sql += "AND No IN ";
                            sql += "(";
                            sql += "SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN ";
                            sql += "( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + nodeid + ")";
                            sql += ") ORDER BY No ";
                        }
                        else
                        {
                            sql = "SELECT pdes.FK_Emp AS No"
                                  + " FROM   Port_DeptEmpStation pdes"
                                  + "        INNER JOIN WF_NodeDept wnd"
                                  + "             ON  wnd.FK_Dept = pdes.FK_Dept"
                                  + "             AND wnd.FK_Node = " + nodeid
                                  + "        INNER JOIN WF_NodeStation wns"
                                  + "             ON  wns.FK_Station = pdes.FK_Station"
                                  + "             AND wnd.FK_Node =" + nodeid
                                  + " ORDER BY"
                                  + "        pdes.FK_Emp";
                        }
                        break;
                    case DeliveryWay.BySelected: //所有的人员多可以启动, 2016年11月开始约定此规则.
                        sql = "SELECT No as FK_Emp FROM Port_Emp ";
                        dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count > 300)
                        {
                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.MSSQL)
                                sql = "SELECT top 300 No as FK_Emp FROM Port_Emp ";

                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.Oracle)
                                sql = "SELECT  No as FK_Emp FROM Port_Emp WHERE ROWNUM <300 ";

                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.MySQL)
                                sql = "SELECT  No as FK_Emp FROM Port_Emp   limit 0,300 ";
                        }
                        break;
                    case DeliveryWay.BySQL:
                        if (DataType.IsNullOrEmpty(nd.DeliveryParas))
                            return "err@SQLによるアクセスの開始ノードを設定したが、SQLを設定しなかった。";
                        break;
                    default:
                        break;
                }

                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    return "err@より:" + nd.HisDeliveryWay + "開始ノードのアクセスルールが設定されていますが、開始ノードに担当者がいません。";

                if (dt.Rows.Count > 2000)
                    return "err@開始ノードを開始できる人が多すぎると、システムがクラッシュして速度が低下します。フローのプロパティで、フローを開始できるテストユーザーを設定する必要があります。";

                // 构造人员表.
                DataTable dtMyEmps = new DataTable();
                dtMyEmps.Columns.Add("No");
                dtMyEmps.Columns.Add("Name");
                dtMyEmps.Columns.Add("FK_DeptText");

                //处理发起人数据.
                string emps = "";
                foreach (DataRow dr in dt.Rows)
                {
                    string myemp = dr[0].ToString();
                    if (emps.Contains("," + myemp + ",") == true)
                        continue;

                    emps += "," + myemp + ",";
                    BP.Port.Emp emp = new Emp(myemp);

                    DataRow drNew = dtMyEmps.NewRow();

                    drNew["No"] = emp.No;
                    drNew["Name"] = emp.Name;
                    drNew["FK_DeptText"] = emp.FK_DeptText;

                    dtMyEmps.Rows.Add(drNew);
                }

                //检查物理表,避免错误.
                Nodes nds = new Nodes(this.FK_Flow);
                foreach (Node mynd in nds)
                {
                    mynd.HisWork.CheckPhysicsTable();
                }

                //返回数据源.
                return BP.Tools.Json.ToJson(dtMyEmps);
                #endregion 从设置里获取-测试人员.

            }
            catch (Exception ex)
            {
                return "err@<h2>開始ノードのアクセスルールが正しく設定されていないため、開始できる人がいません、<a href='http://bbs.ccflow.org/showtopic-4103.aspx' target=_blank ><font color=red>ここをクリックして解決策を確認してください</font>.</a>。</h2> システムエラープロンプト:" + ex.Message + "<br><h3>OSModelを切り替えた可能性もあります。OSModelとは、オンラインヘルプドキュメントを確認してください <a href='http://ccbpm.mydoc.io' target=_blank>http://ccbpm.mydoc.io</a>  .</h3>";
            }
        }
        #endregion

    }
}
