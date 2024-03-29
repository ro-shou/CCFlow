﻿using System;
using System.Collections.Generic;
using System.Collections;
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
using LitJson;
using BP.WF.XML;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 初始化函数
    /// </summary>
    public class WF_Admin_CCBPMDesigner2018 : DirectoryPageBase
    {
           /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_CCBPMDesigner2018()
        {
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {

            return "err@判定マークなし:" + this.DoType;
        }
        #endregion 执行父类的重写方法.
          

         

        #region 节点相关 Nodes
        /// <summary>
        /// 创建流程节点并返回编号
        /// </summary>
        /// <returns></returns>
        public string CreateNode()
        {
            try
            {
                string x = this.GetRequestVal("X");
                string y = this.GetRequestVal("Y");
                string icon = this.GetRequestVal("icon");
                int iX = 20;
                int iY = 20;
                
                if (DataType.IsNullOrEmpty(x)==false) 
                    iX = (int)double.Parse(x);

                if (DataType.IsNullOrEmpty(y)==false) 
                    iY = (int)double.Parse(y);

                int nodeId = BP.WF.Template.TemplateGlo.NewNode(this.FK_Flow, iX, iY,icon);

                BP.WF.Node node = new BP.WF.Node(nodeId);
                node.Update();

                Hashtable ht = new Hashtable();
                ht.Add("NodeID", node.NodeID);
                ht.Add("Name", node.Name);


                #region  //2019.11.08 增加如果是极简版, 就设置初始化参数.
                Flow fl = new Flow(this.FK_Flow);
                if (fl.FlowFrmType != FlowFrmType.Ver2019Earlier)
                {
                    FrmNode fm = new FrmNode();
                    fm.FK_Flow = this.FK_Flow;
                    fm.FK_Frm = "ND" + int.Parse(this.FK_Flow + "01");
                    fm.FK_Node = node.NodeID;
                    fm.FrmSln = FrmSln.Readonly;
                    fm.Insert();
                }
                #endregion  //2019.11.08 增加如果是极简版.


                return BP.Tools.Json.ToJsonEntityModel(ht);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 根据节点编号删除流程节点
        /// </summary>
        /// <returns>执行结果</returns>
        public string DeleteNode()
        {
            try
            {
                BP.WF.Node node = new BP.WF.Node();
                node.NodeID = this.FK_Node;
                if (node.RetrieveFromDBSources() == 0)
                    return "err@削除に失敗しました。データは削除されていません。ノードは削除されていないと推定されます。";

                if (node.IsStartNode == true)
                    return "err@開始ノードは削除できません。";

                node.Delete();
                return "正常に削除されました。";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 修改节点名称
        /// </summary>
        /// <returns></returns>
        public string Node_EditNodeName()
        {
            string FK_Node = this.GetValFromFrmByKey("NodeID");
            string NodeName = HttpContextHelper.UrlDecode(this.GetValFromFrmByKey("NodeName"));

            BP.WF.Node node = new BP.WF.Node();
            node.NodeID = int.Parse(FK_Node);
            int iResult = node.RetrieveFromDBSources();
            if (iResult > 0)
            {
                node.Name = NodeName;
                node.Update();
                return "@正常に変更されました。";
            }

            return "err@ノードの変更に失敗しました。ノードが存在するかどうか確認してください。";
        }
        /// <summary>
        /// 修改节点运行模式
        /// </summary>
        /// <returns></returns>
        public string Node_ChangeRunModel()
        {
            string runModel = GetValFromFrmByKey("RunModel");
            BP.WF.Node node = new BP.WF.Node(this.FK_Node);
            //节点运行模式
            switch (runModel)
            {
                case "NodeOrdinary":
                    node.HisRunModel = BP.WF.RunModel.Ordinary;
                    break;
                case "NodeFL":
                    node.HisRunModel = BP.WF.RunModel.FL;
                    break;
                case "NodeHL":
                    node.HisRunModel = BP.WF.RunModel.HL;
                    break;
                case "NodeFHL":
                    node.HisRunModel = BP.WF.RunModel.FHL;
                    break;
                case "NodeSubThread":
                    node.HisRunModel = BP.WF.RunModel.SubThread;
                    break;
            }
            node.Update();

            return "正常に設定されました。";
        }
        #endregion end Node

        /// <summary>
        /// 删除连接线
        /// </summary>
        /// <returns></returns>
        public string Direction_Delete()
        {
            try
            {
                Directions di = new Directions();
                di.Retrieve(DirectionAttr.FK_Flow, this.FK_Flow, DirectionAttr.Node, this.FK_Node, DirectionAttr.ToNode, this.GetValFromFrmByKey("ToNode"));
                foreach (Direction direct in di)
                {
                    direct.Delete();
                }
                return "@正常に削除されました！";
            }
            catch (Exception ex)
            {
                return "@err:"+ex.Message;
            }
        }
        /// <summary>
        /// 添加标签
        /// </summary>
        /// <returns></returns>
        public string CreatLabNote()
        {
            try
            {
                LabNote lb = new LabNote();

                //获取当前流程已经存在的数量
                LabNotes labNotes = new LabNotes();
                int num = labNotes.Retrieve(LabNoteAttr.FK_Flow, this.FK_Flow);

                string Name = this.GetValFromFrmByKey("LabName");
                int x = int.Parse(this.GetValFromFrmByKey("X"));
                int y = int.Parse(this.GetValFromFrmByKey("Y"));

                lb.MyPK = this.FK_Flow + "_" + x + "_" + y + "_" + (num + 1);
                lb.Name = Name;
                lb.FK_Flow = this.FK_Flow;
                lb.X = x;
                lb.Y = y;

                lb.DirectInsert();

                Hashtable ht = new Hashtable();
                ht.Add("MyPK", this.FK_Flow + "_" + x + "_" + y + "_" + (num + 1));
                ht.Add("FK_Flow", this.FK_Flow);

                return BP.Tools.Json.ToJsonEntityModel(ht);
            }
            catch (Exception ex)
            {
                return "@err:" + ex.Message;
            }
        }

        public void CheckBillFrm()
        {
            GEEntity en = new GEEntity(this.EnsName);
            en.CheckPhysicsTable();
        }
    }
}
