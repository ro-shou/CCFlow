﻿using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 推送的方式
    /// </summary>
    public enum PushWay
    {
        /// <summary>
        /// 当前节点的接受人
        /// </summary>
        CurrentWorkers,
        /// <summary>
        /// 指定节点的工作人员
        /// </summary>
        NodeWorker,
        /// <summary>
        /// 指定的工作人员s
        /// </summary>
        SpecEmps,
        /// <summary>
        /// 指定的工作岗位s
        /// </summary>
        SpecStations,
        /// <summary>
        /// 指定的部门人员
        /// </summary>
        SpecDepts,
        /// <summary>
        /// 指定的SQL
        /// </summary>
        SpecSQL,
        /// <summary>
        /// 按照系统指定的字段
        /// </summary>
        ByParas
    }
    /// <summary>
    /// 消息推送属性
    /// </summary>
    public class PushMsgAttr
    {
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 事件
        /// </summary>
        public const string FK_Event = "FK_Event";
        /// <summary>
        /// 推送方式
        /// </summary>
        public const string PushWay = "PushWay";
        /// <summary>
        /// 推送处理内容
        /// </summary>
        public const string PushDoc = "PushDoc";
        /// <summary>
        /// 推送处理内容 tag.
        /// </summary>
        public const string Tag = "Tag";

        #region 消息设置.
        /// <summary>
        /// 是否启用发送邮件
        /// </summary>
        public const string MailEnable = "MailEnable";
        /// <summary>
        /// 消息标题
        /// </summary>
        public const string MailTitle = "MailTitle";
        /// <summary>
        /// 消息内容模版
        /// </summary>
        public const string MailDoc = "MailDoc";
        /// <summary>
        /// 是否启用短信
        /// </summary>
        public const string SMSEnable = "SMSEnable";
        /// <summary>
        /// 短信内容模版
        /// </summary>
        public const string SMSDoc = "SMSDoc";
        /// <summary>
        /// 是否推送？
        /// </summary>
        public const string MobilePushEnable = "MobilePushEnable";
        #endregion 消息设置.

        /// <summary>
        /// 短信字段
        /// </summary>
        public const string SMSField = "SMSField";
        /// <summary>
        /// 接受短信的节点.
        /// </summary>
        public const string SMSNodes = "SMSNodes";
        /// <summary>
        /// 推送方式
        /// </summary>
        public const string SMSPushWay = "SMSPushWay";
        /// <summary>
        /// 短消息发送设置
        /// </summary>
        public const string SMSPushModel = "SMSPushModel";
        /// <summary>
        /// 邮件字段
        /// </summary>
        public const string MailAddress = "MailAddress";
        /// <summary>
        /// 邮件推送方式
        /// </summary>
        public const string MailPushWay = "MailPushWay";
        /// <summary>
        /// 推送邮件的节点s
        /// </summary>
        public const string MailNodes = "MailNodes";
        /// <summary>
        /// 按照指定的SQL
        /// </summary>
        public const string BySQL = "BySQL";
        /// <summary>
        /// 发送给指定的接受人
        /// </summary>
        public const string ByEmps = "ByEmps";
    }
    /// <summary>
    /// 消息推送
    /// </summary>
    public class PushMsg : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(PushMsgAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 事件
        /// </summary>
        public string FK_Event
        {
            get
            {
                return this.GetValStringByKey(PushMsgAttr.FK_Event);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.FK_Event, value);
            }
        }
        /// <summary>
        /// 推送方式.
        /// </summary>
        public int PushWay
        {
            get
            {
                return this.GetValIntByKey(PushMsgAttr.PushWay);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.PushWay, value);
            }
        }
        /// <summary>
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(PushMsgAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.FK_Node, value);
            }
        }
        public string PushDoc
        {
            get
            {
                string s = this.GetValStringByKey(PushMsgAttr.PushDoc);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "";
                return s;
            }
            set
            {
                this.SetValByKey(PushMsgAttr.PushDoc, value);
            }
        }
        public string Tag
        {
            get
            {
                string s = this.GetValStringByKey(PushMsgAttr.Tag);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "";
                return s;
            }
            set
            {
                this.SetValByKey(PushMsgAttr.Tag, value);
            }
        }
        #endregion

        #region 事件消息.
        /// <summary>
        /// 邮件推送方式
        /// </summary>
        public int MailPushWay
        {
            get
            {
                return this.GetValIntByKey(PushMsgAttr.MailPushWay);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.MailPushWay, value);
            }
        }
        /// <summary>
        /// 推送方式Name
        /// </summary>
        public string MailPushWayText
        {
            get
            {
                if (this.FK_Event == EventListOfNode.WorkArrive)
                {
                    if (this.MailPushWay == 0)
                        return "送信しない";

                    if (this.MailPushWay == 1)
                        return "現在のノードのすべてのプロセッサに送信されます";

                    if (this.MailPushWay == 2)
                        return "指定されたフィールドに送信する";
                }

                if (this.FK_Event == EventListOfNode.SendSuccess)
                {
                    if (this.MailPushWay == 0)
                        return "送信しない";

                    if (this.MailPushWay == 1)
                        return "次のノードのすべての受信者に送信します";

                    if (this.MailPushWay == 2)
                        return "指定されたフィールドに送信する";
                }

                if (this.FK_Event == EventListOfNode.ReturnAfter)
                {
                    if (this.MailPushWay == 0)
                        return "送信しない";

                    if (this.MailPushWay == 1)
                        return "返されたノードハンドラーに送信されました";

                    if (this.MailPushWay == 2)
                        return "指定されたフィールドに送信する";
                }

                return "わからない";
            }
        }
        /// <summary>
        /// 邮件地址
        /// </summary>
        public string MailAddress
        {
            get
            {
                return this.GetValStringByKey(PushMsgAttr.MailAddress);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.MailAddress, value);
            }
        }
        /// <summary>
        /// 邮件标题.
        /// </summary>
        public string MailTitle
        {
            get
            {
                string str = this.GetValStrByKey(PushMsgAttr.MailTitle);
                if (DataType.IsNullOrEmpty(str) == false)
                    return str;
                switch (this.FK_Event)
                {
                    case EventListOfNode.WorkArrive:
                        return "新しいジョブ{{Title}}、送信者@WebUser.No、@WebUser.Name";
                    case EventListOfNode.SendSuccess:
                        return "新しいジョブ{{Title}}、送信者@WebUser.No、@WebUser.Name";
                    case EventListOfNode.ShitAfter:
                        return "@WebUser.No、@WebUser.Nameによって転送された新しいジョブ{{Title}}を転送しました";
                    case EventListOfNode.ReturnAfter:
                        return "返された{{Title}}、@WebUser.No、@WebUser.Nameによって返されました";
                    case EventListOfNode.UndoneAfter:
                        return "ジョブがキャンセルされました{{Title}}、送信者@WebUser.No、@WebUser.Name";
                    case EventListOfNode.AskerReAfter:
                        return "新しいジョブ{{Title}}を追加、送信者@WebUser.No、@WebUser.Name";
                    case EventListOfNode.FlowOverAfter:
                        return "フロー{{Title}}が終了しました、プロセッサ@WebUser.No、@WebUser.Name";
                    case EventListOfNode.AfterFlowDel:
                        return "フロー{{Title}}が削除されました、プロセッサ@WebUser.No、@WebUser.Name";
                    default:
                        throw new Exception("イベントタイプはデフォルトのメッセージテンプレートを定義していません" + this.FK_Event);
                        break;
                }
                return str;
            }
        }
        /// <summary>
        /// Email节点s
        /// </summary>
        public string MailNodes
        {
            get
            {
                return this.GetValStringByKey(PushMsgAttr.MailNodes);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.MailNodes, value);
            }
        }
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string MailTitle_Real
        {
            get
            {
                string str = this.GetValStrByKey(PushMsgAttr.MailTitle);
                return str;
            }
            set
            {
                this.SetValByKey(PushMsgAttr.MailTitle, value);
            }
        }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string MailDoc_Real
        {
            get
            {
                return this.GetValStrByKey(PushMsgAttr.MailDoc);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.MailDoc, value);
            }
        }
        public string MailDoc
        {
            get
            {
                string str = this.GetValStrByKey(PushMsgAttr.MailDoc);
                if (DataType.IsNullOrEmpty(str) == false)
                    return str;
                switch (this.FK_Event)
                {
                    case EventListOfNode.WorkArrive:
                        str += "\t\nこんにちは:";
                        str += "\t\n    処理する必要のある新しいジョブ{{Title}}があります。ここをクリックしてジョブレポート{Url}を開いてください。";
                        str += "\t\n ";
                        str += "\t\n ";
                        str += "\t\n拝啓！";
                        str += "\t\n    @WebUser.No, @WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.SendSuccess:

                        str += "\t\nこんにちはあなたは新しい仕事をしています。";
                        str += "\t\n    タイトル：{{Title}}。";
                        str += "\t\n    単一の番号：{{BillNo}}。";
                        str += "\t\n    詳細：ジョブ{Url}を開いてください";
                        str += "\t\n ";
                        str += "\t\n ";
                        str += "\t\n 拝啓！！";
                        str += "\t\n    @WebUser.No, @WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.ReturnAfter:
                        str += "\t\nこんにちは:";
                        str += "\t\n    ジョブ{{Title}}が返されました。ここをクリックしてジョブレポート{Url}を開いてください";
                        str += "\t\n    コメントを返す:　\t\n";
                        str += "\t\n    {  @ReturnMsg }";
                        str += "\t\n ";
                        str += "\t\n ";
                        str += "\t\n ";
                        str += "\t\n 拝啓！";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.ShitAfter:
                        str += "\t\n こんにちは:";
                        str += "\t\n    仕事はあなたに引き渡されました{{Title}}、ここをクリックして仕事を開きます{Url}。";
                        str += "\t\n 拝啓！";
                        str += "\t\n ";
                        str += "\t\n ";
                        str += "\t\n ";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.UndoneAfter:
                        str += "\t\nこんにちは:";
                        str += "\t\n    あなたに渡された作業{{Title}}、ここをクリックして作業レポート{Url}を開きます。";
                        str += "\t\n ";
                        str += "\t\n ";
                        str += "\t\n ";
                        str += "\t\n 拝啓！";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.AskerReAfter: //加签.
                        str += "\t\nこんにちは:";
                        str += "\t\n    あなたに渡された作業{{Title}}、ここをクリックしてレポートを開きます{Url}。";
                        str += "\t\n ";
                        str += "\t\n ";
                        str += "\t\n ";
                        str += "\t\n 拝啓！";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.FlowOverAfter: //流程结束后.
                        str += "\t\nこんにちは:";
                        str += "\t\n    ジョブ{{Title}}が終了しました。ここをクリックしてジョブレポート{Url}を開いてください。";
                        str += "\t\n 拝啓！";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    default:
                        throw new Exception("@イベントタイプはデフォルトのメッセージテンプレートを定義していません:" + this.FK_Event);
                        break;
                }
                return str;
            }
        }
        /// <summary>
        /// 短信接收人字段
        /// </summary>
        public string SMSField
        {
            get
            {
                return this.GetValStringByKey(PushMsgAttr.SMSField);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.SMSField, value);
            }
        }
        public string SMSNodes
        {
            get
            {
                return this.GetValStringByKey(PushMsgAttr.SMSNodes);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.SMSNodes, value);
            }
        }
        public string SMSPushModel
        {
            get
            {
                return this.GetValStringByKey(PushMsgAttr.SMSPushModel);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.SMSPushModel, value);
            }
        }
        /// <summary>
        /// 短信提醒方式
        /// </summary>
        public int SMSPushWay
        {
            get
            {
                return this.GetValIntByKey(PushMsgAttr.SMSPushWay);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.SMSPushWay, value);
            }
        }
        /// <summary>
        /// 发送消息标签
        /// </summary>
        public string SMSPushWayText
        {
            get
            {
                if (this.FK_Event == EventListOfNode.WorkArrive)
                {
                    if (this.SMSPushWay == 0)
                        return "送信しない";

                    if (this.SMSPushWay == 1)
                        return "現在のノードのすべてのプロセッサに送信されます";

                    if (this.SMSPushWay == 2)
                        return "指定されたフィールドに送信する";
                }

                if (this.FK_Event == EventListOfNode.SendSuccess)
                {
                    if (this.SMSPushWay == 0)
                        return "送信しない";

                    if (this.SMSPushWay == 1)
                        return "次のノードのすべての受信者に送信します";

                    if (this.SMSPushWay == 2)
                        return "指定されたフィールドに送信する";
                }

                if (this.FK_Event == EventListOfNode.ReturnAfter)
                {
                    if (this.SMSPushWay == 0)
                        return "送信しない";

                    if (this.SMSPushWay == 1)
                        return "返されたノードハンドラーに送信されました";

                    if (this.SMSPushWay == 2)
                        return "指定されたフィールドに送信する";
                }

                if (this.FK_Event == EventListOfNode.FlowOverAfter)
                {
                    if (this.SMSPushWay == 0)
                        return "送信しない";

                    if (this.SMSPushWay == 1)
                        return "すべてのノードプロセッサに送信";

                    if (this.SMSPushWay == 2)
                        return "指定されたフィールドに送信する";
                }

                return "わからない";
            }
        }
        /// <summary>
        /// 短信模版内容
        /// </summary>
        public string SMSDoc_Real
        {
            get
            {
                string str = this.GetValStrByKey(PushMsgAttr.SMSDoc);
                return str;
            }
            set
            {
                this.SetValByKey(PushMsgAttr.SMSDoc, value);
            }
        }
        /// <summary>
        /// 短信模版内容
        /// </summary>
        public string SMSDoc
        {
            get
            {
                string str = this.GetValStrByKey(PushMsgAttr.SMSDoc);
                if (DataType.IsNullOrEmpty(str) == false)
                    return str;

                switch (this.FK_Event)
                {
                    case EventListOfNode.WorkArrive:
                    case EventListOfNode.SendSuccess:
                        str = "処理する必要がある新しいジョブ{{Title}}があります。送信者:@WebUser.No、@WebUser.Name、{Url}を開きます。";
                        break;
                    case EventListOfNode.ReturnAfter:
                        str = "ジョブ{{Title}}が返されました:@WebUser.No、@WebUser.Name、{Url}を開きます。";
                        break;
                    case EventListOfNode.ShitAfter:
                        str = "引き継ぎ作業{{Title}}、引き継ぎ人:@WebUser.No、@WebUser.Name、{Url}を開きます。";
                        break;
                    case EventListOfNode.UndoneAfter:
                        str = "仕事{{Title}}の取り消し、取り消し者：@WebUser.No、@WebUser.Name、{Url}を開きます。";
                        break;
                    case EventListOfNode.AskerReAfter: //加签.
                        str = "作業の推奨{{Title}}、推奨者：@WebUser.No、@WebUser.Name、{Url}を開きます。";
                        break;
                    case EventListOfNode.FlowOverAfter: //加签.
                        str = "フロー{{Title}}が終了しました、最後のハンドラ:@WebUser.No、@WebUser.Name、{Url}を開きます。";
                        break;
                    default:
                        throw new Exception("@イベントタイプはデフォルトのメッセージテンプレートを定義していません:" + this.FK_Event);
                        break;
                }
                return str;
            }
            set
            {
                this.SetValByKey(PushMsgAttr.SMSDoc, value);
            }
        }
        /// <summary>
        /// 按照指定的SQL
        /// </summary>
        public string BySQL
        {
            get
            {
                return this.GetValStrByKey(PushMsgAttr.BySQL);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.BySQL, value);
            }
        }
        /// <summary>
        /// 发送给指定的人员，人员之间以逗号分割
        /// </summary>
        public string ByEmps
        {
            get
            {
                return this.GetValStrByKey(PushMsgAttr.ByEmps);
            }
            set
            {
                this.SetValByKey(PushMsgAttr.ByEmps, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 消息推送
        /// </summary>
        public PushMsg()
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

                Map map = new Map("WF_PushMsg", "ニュースプッシュ");

                map.AddMyPK();

                map.AddTBString(PushMsgAttr.FK_Flow, null, "処理する", true, false, 0, 3, 10);
                map.AddTBInt(PushMsgAttr.FK_Node, 0, "ノード", true, false);
                map.AddTBString(PushMsgAttr.FK_Event, null, "イベントタイプ", true, false, 0, 20, 10);

                #region 将要删除.
                map.AddDDLSysEnum(PushMsgAttr.PushWay, 0, "プッシュ方式", true, false, PushMsgAttr.PushWay,
                    "@0=指定されたノードに応じて@1=指定されたスタッフに従って@2=指定された職位に従って@3=指定された部門に従って@4=指定されたSQLに従って@5=システム指定フィールドに従って");
                //设置内容.
                map.AddTBString(PushMsgAttr.PushDoc, null, "コンテンツのプッシュ保存", true, false, 0, 3500, 10);
                map.AddTBString(PushMsgAttr.Tag, null, "Tag", true, false, 0, 500, 10);
                #endregion 将要删除.

                #region 短消息.
                map.AddTBInt(PushMsgAttr.SMSPushWay, 0, "SMS送信方法", true, true);
                map.AddTBString(PushMsgAttr.SMSField, null, "ショートメッセージフィールド", true, false, 0, 100, 10);
                map.AddTBStringDoc(PushMsgAttr.SMSDoc, null, "ショートメッセージコンテンツテンプレート", true, false, true);
                map.AddTBString(PushMsgAttr.SMSNodes, null, "SMSノードs", true, false, 0, 100, 10);

                // 邮件,站内消息,短信,钉钉,微信,WebServices.
                map.AddTBString(PushMsgAttr.SMSPushModel, "Email", "メッセージ送信設定", true, false, 0, 50, 10);
                #endregion 短消息.

                #region 邮件.
                map.AddTBInt(PushMsgAttr.MailPushWay, 0, "メール送信方法", true, true);
                map.AddTBString(PushMsgAttr.MailAddress, null, "メールフィールド", true, false, 0, 100, 10);
                map.AddTBString(PushMsgAttr.MailTitle, null, "メールヘッダーテンプレート", true, false, 0, 200, 20, true);
                map.AddTBStringDoc(PushMsgAttr.MailDoc, null, "メールコンテンツテンプレート", true, false, true);
                map.AddTBString(PushMsgAttr.MailNodes, null, "Mailノードs", true, false, 0, 100, 10);
                #endregion 邮件.

                map.AddTBString(PushMsgAttr.BySQL, null, "SQL計算によると", true, false, 0, 500, 10);
                map.AddTBString(PushMsgAttr.ByEmps, null, "指定された人に送る", true, false, 0, 100, 10);
                map.AddTBAtParas(500);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        /// <summary>
        /// 生成提示信息.
        /// </summary>
        /// <returns></returns>
        private string generAlertMessage = null;
        /// <summary>
        /// 执行消息发送
        /// </summary>
        /// <param name="currNode">当前节点</param>
        /// <param name="en">数据实体</param>
        /// <param name="atPara">参数</param>
        /// <param name="objs">发送返回对象</param>
        /// <param name="jumpToNode">跳转到的节点</param>
        /// <param name="jumpToEmps">跳转到的人员</param>
        /// <returns>执行成功的消息</returns>
        public string DoSendMessage(Node currNode, Entity en, string atPara, SendReturnObjs objs, Node jumpToNode = null,
            string jumpToEmps = null)
        {
            if (en == null)
                return "";

            #region 处理参数.
            Row r = en.Row;
            try
            {
                //系统参数.
                r.Add("FK_MapData", en.ClassID);
            }
            catch
            {
                r["FK_MapData"] = en.ClassID;
            }

            if (atPara != null)
            {
                AtPara ap = new AtPara(atPara);
                foreach (string s in ap.HisHT.Keys)
                {
                    try
                    {
                        r.Add(s, ap.GetValStrByKey(s));
                    }
                    catch
                    {
                        r[s] = ap.GetValStrByKey(s);
                    }
                }
            }

            //生成标题.
            Int64 workid = Int64.Parse(en.PKVal.ToString());
            string title = "題名";
            if (en.Row.ContainsKey("Title") == true)
            {
                title = en.GetValStringByKey("Title"); // 获得工作标题.
                if (DataType.IsNullOrEmpty(title))
                    title = BP.DA.DBAccess.RunSQLReturnStringIsNull("SELECT Title FROM WF_GenerWorkFlow WHERE WorkID=" + en.PKVal, "題名");
            }
            else
                title = BP.DA.DBAccess.RunSQLReturnStringIsNull("SELECT Title FROM WF_GenerWorkFlow WHERE WorkID=" + en.PKVal, "題名");

            //生成URL.
            string hostUrl = BP.WF.Glo.HostURL;
            string sid = "{EmpStr}_" + workid + "_" + currNode.NodeID + "_" + DataType.CurrentDataTime;
            string openWorkURl = hostUrl + "WF/Do.htm?DoType=OF&SID=" + sid;
            openWorkURl = openWorkURl.Replace("//", "/");
            openWorkURl = openWorkURl.Replace("http:/", "http://");
            #endregion


            // 有可能是退回信息. 翻译.
            if (jumpToEmps == null)
            {
                if (atPara != null)
                {
                    AtPara ap = new AtPara(atPara);
                    jumpToEmps = ap.GetValStrByKey("SendToEmpIDs");
                }
            }

            //发送消息
            string msg = this.SendMessage(title,en,currNode,workid,jumpToEmps,openWorkURl,objs,r);

            //发送短消息.
           // string msg1 = this.SendShortMessageToSpecNodes(title, openWorkURl, en, currNode, workid, objs, null, jumpToEmps);
            //发送邮件.
            //string msg2 = this.SendEmail(title, openWorkURl, en, jumpToEmps, currNode, workid, objs, r);

            return msg;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="en">数据实体</param>
        /// <param name="currNode">当前节点</param>
        /// <param name="workid">流程WorkId</param>
        /// <param name="jumpToEmps">下一个节点的接收人</param>
        /// <param name="openUrl">打开链接的URL</param>
        /// <param name="objs">发送返回的对象</param>
        /// <param name="r">表单数据HashTable</param>
        /// <returns></returns>
        private string SendMessage(string title,Entity en,Node currNode,Int64 workid,string jumpToEmps,string openUrl, SendReturnObjs objs,Row r)
        {
            //不启用消息
            if (this.SMSPushWay == 0)
                return "";
            string generAlertMessage = ""; //定义要返回的提示消息.
            string mailTitle = this.MailTitle;// 邮件标题
            string smsDoc = this.SMSDoc;//消息模板

            #region 邮件标题
            mailTitle = this.MailTitle;
            mailTitle = mailTitle.Replace("{Title}", title);
            mailTitle = mailTitle.Replace("@WebUser.No", WebUser.No);
            mailTitle = mailTitle.Replace("@WebUser.Name", WebUser.Name);
         
            #endregion 邮件标题

            #region  处理消息内容
            smsDoc = smsDoc.Replace("{Title}", title);
            smsDoc = smsDoc.Replace("{Url}", openUrl);
            smsDoc = smsDoc.Replace("@WebUser.No", WebUser.No);
            smsDoc = smsDoc.Replace("@WebUser.Name", WebUser.Name);
            smsDoc = smsDoc.Replace("@WorkID", en.PKVal.ToString());
            smsDoc = smsDoc.Replace("@OID", en.PKVal.ToString());

            /*如果仍然有没有替换下来的变量.*/
            if (smsDoc.Contains("@") == true)
                smsDoc = BP.WF.Glo.DealExp(smsDoc, en, null);

            if (this.FK_Event == BP.Sys.EventListOfNode.ReturnAfter)
            {
                //获取退回原因
                Paras ps = new Paras();
                ps.SQL = "SELECT BeiZhu,ReturnerName,IsBackTracking FROM WF_ReturnWork WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID  ORDER BY RDT DESC";
                ps.Add(ReturnWorkAttr.WorkID, Int64.Parse(en.PKVal.ToString()));
                DataTable retunWdt = DBAccess.RunSQLReturnTable(ps);
                if (retunWdt.Rows.Count != 0)
                {
                    string returnMsg = retunWdt.Rows[0]["BeiZhu"].ToString();
                    string returner = retunWdt.Rows[0]["ReturnerName"].ToString();
                    smsDoc = smsDoc.Replace("ReturnMsg", returnMsg);
                }
            }
            #endregion 处理消息内容

            #region 消息发送
            string toEmpIDs = "";
            #region 表单字段作为接受人
            if (this.SMSPushWay == 2)
            {
                /*从字段里取数据. */
                string toEmp = r[this.SMSField] as string;
                //修改内容
                smsDoc = smsDoc.Replace("{EmpStr}", toEmp);
                openUrl = openUrl.Replace("{EmpStr}", toEmp);

                //发送消息
                BP.WF.Dev2Interface.Port_SendMessage(toEmp, smsDoc, mailTitle, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, openUrl, this.SMSPushModel);
                return "@宛先：{" + toEmp + "}リマインダーメッセージを送信。";
            }
            #endregion 表单字段作为接受人

            #region 如果发送给指定的节点处理人,就计算出来直接退回,任何方式的处理人都是一致的.
            if (this.SMSPushWay == 3)
            {
                /*如果向指定的字段作为发送邮件的对象, 从字段里取数据. */
                string[] nodes = this.SMSNodes.Split(',');

                string msg = "";
                foreach (string nodeID in nodes)
                {
                    if (DataType.IsNullOrEmpty(nodeID) == true)
                        continue;

                    string sql = "SELECT EmpFromT AS Name,EmpFrom AS No FROM ND" + int.Parse(this.FK_Flow) + "Track A  WHERE  A.ActionType=1 AND A.WorkID=" + workid + " AND A.NDFrom=" + nodeID ;
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0)
                        continue;

                    foreach (DataRow dr in dt.Rows)
                    {
                        string empName = dr["Name"].ToString();
                        string empNo = dr["No"].ToString();

                        
                        // 因为要发给不同的人，所有需要clone 一下，然后替换发送.
                        string smsDocReal = smsDoc.Clone() as string;
                        smsDocReal = smsDocReal.Replace("{EmpStr}", empName);
                        openUrl = openUrl.Replace("{EmpStr}", empNo);

                        string paras = "@FK_Flow=" + this.FK_Flow + "@WorkID=" + workid + "@FK_Node=" + this.FK_Node;

                        //发送消息
                        BP.WF.Dev2Interface.Port_SendMessage(empNo, smsDoc, mailTitle, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, openUrl, this.SMSPushModel);
                        //处理短消息.
                        toEmpIDs += empName + ",";
                    }
                }
                return "@宛先：{" + toEmpIDs + "}SMSリマインダーを送信しました。";
            }
            #endregion 如果发送给指定的节点处理人, 就计算出来直接退回, 任何方式的处理人都是一致的.

            #region 按照SQL计算
            if(this.SMSPushWay == 4)
            {
                string bySQL = this.BySQL;
                if(DataType.IsNullOrEmpty(BySQL) == true)
                {
                    return "指定されたSQLに従ってメッセージを送信します。SQLデータは空にできません";
                }
                bySQL = bySQL.Replace("~", "'");
                //替换SQL中的参数
                bySQL = bySQL.Replace("@WebUser.No", WebUser.No);
                bySQL = bySQL.Replace("@WebUser.Name", WebUser.Name);
                bySQL = bySQL.Replace("@WebUser.FK_DeptNameOfFull", WebUser.FK_DeptNameOfFull);
                bySQL = bySQL.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
                bySQL = bySQL.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                /*如果仍然有没有替换下来的变量.*/
                if (bySQL.Contains("@") == true)
                    bySQL = BP.WF.Glo.DealExp(bySQL, en, null);
                DataTable dt = DBAccess.RunSQLReturnTable(bySQL);
                foreach (DataRow dr in dt.Rows)
                {
                    string empName = dr["Name"].ToString();
                    string empNo = dr["No"].ToString();


                    // 因为要发给不同的人，所有需要clone 一下，然后替换发送.
                    string smsDocReal = smsDoc.Clone() as string;
                    smsDocReal = smsDocReal.Replace("{EmpStr}", empName);
                    openUrl = openUrl.Replace("{EmpStr}", empNo);

                    string paras = "@FK_Flow=" + this.FK_Flow + "@WorkID=" + workid + "@FK_Node=" + this.FK_Node;

                    //发送消息
                    BP.WF.Dev2Interface.Port_SendMessage(empNo, smsDoc, mailTitle, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, openUrl, this.SMSPushModel);

                    //处理短消息.
                    toEmpIDs += empName + ",";
                }
               
            }
            #endregion 按照SQL计算

            #region 发送给指定的接收人
            if (this.SMSPushWay == 5)
            {
                if (DataType.IsNullOrEmpty(this.ByEmps) == true)
                {
                    return "指定した人物に送信します。人物セットは空にできません。";
                }
                //以逗号分割开
                string[] toEmps = this.ByEmps.Split(',');
                foreach(string empNo in toEmps)
                {
                    if (DataType.IsNullOrEmpty(empNo) == true)
                        continue;
                    BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(empNo);
                    // 因为要发给不同的人，所有需要clone 一下，然后替换发送.
                    string smsDocReal = smsDoc.Clone() as string;
                    smsDocReal = smsDocReal.Replace("{EmpStr}", emp.Name);
                    openUrl = openUrl.Replace("{EmpStr}", emp.No);
                    //发送消息
                    BP.WF.Dev2Interface.Port_SendMessage(empNo, smsDoc, mailTitle, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, openUrl, this.SMSPushModel);
                    //处理短消息.
                    toEmpIDs += emp.Name + ",";
                }
            }
            #endregion 发送给指定的接收人

            #region 不同的消息事件，接收人不同的处理
            if (this.SMSPushWay == 1)
            {
                #region 工作到达、退回、移交、撤销
                if ((this.FK_Event == BP.Sys.EventListOfNode.WorkArrive
                    || this.FK_Event == BP.Sys.EventListOfNode.ReturnAfter
                    || this.FK_Event == BP.Sys.EventListOfNode.ShitAfter
                    || this.FK_Event == BP.Sys.EventListOfNode.UndoneAfter)
                     && DataType.IsNullOrEmpty(jumpToEmps) == false)
                {
                    /*当前节点的处理人.*/
                    toEmpIDs = jumpToEmps;
                    string[] emps = toEmpIDs.Split(',');
                    foreach (string empNo in emps)
                    {
                        if (DataType.IsNullOrEmpty(empNo))
                            continue;

                        // 因为要发给不同的人，所有需要clone 一下，然后替换发送.
                        string smsDocReal = smsDoc.Clone() as string;
                        smsDocReal = smsDocReal.Replace("{EmpStr}", empNo);
                        openUrl = openUrl.Replace("{EmpStr}", empNo);

                        BP.WF.Dev2Interface.Port_SendMessage(empNo, smsDoc, mailTitle, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, openUrl, this.SMSPushModel);
                    }
                    return "@宛先：{" + toEmpIDs + "}リマインダーメッセージを送信。";
                }
                #endregion 工作到达、退回、移交、撤销

                #region 节点发送成功后
                if (this.FK_Event == BP.Sys.EventListOfNode.SendSuccess && objs.VarAcceptersID != null)
                {
                    /*如果向接受人发送消息.*/
                    toEmpIDs = objs.VarAcceptersID;
                    string[] emps = toEmpIDs.Split(',');
                    foreach (string empNo in emps)
                    {
                        if (DataType.IsNullOrEmpty(empNo))
                            continue;
                        
                        // 因为要发给不同的人，所有需要clone 一下，然后替换发送.
                        string smsDocReal = smsDoc.Clone() as string;
                        smsDocReal = smsDocReal.Replace("{EmpStr}", empNo);
                        openUrl = openUrl.Replace("{EmpStr}", empNo);
                        string paras = "@FK_Flow=" + currNode.FK_Flow + "&FK_Node=" + currNode.NodeID + "@WorkID=" + workid+"_"+ empNo;
                        BP.WF.Dev2Interface.Port_SendMessage(empNo, smsDoc, mailTitle, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, openUrl, this.SMSPushModel, paras);

                    }
                    return "@宛先：{" + toEmpIDs + "}リマインダーメッセージを送信。";
                }
                #endregion 节点发送成功后

               
                #region 流程结束后、流程删除后
                if (this.FK_Event == BP.Sys.EventListOfNode.FlowOverAfter || this.FK_Event == BP.Sys.EventListOfNode.AfterFlowDel)
                {
                    /*向所有参与人发送消息. */
                    DataTable dt = DBAccess.RunSQLReturnTable("SELECT Emps,TodoEmps FROM WF_GenerWorkFlow WHERE WorkID=" + workid);
                    if (dt.Rows.Count == 0)
                        return "";
                    string empsStrs = "";
                    foreach (DataRow dr in dt.Rows)
                    {
                        empsStrs += dr["Emps"];
                        string todoEmps = dr["TodoEmps"].ToString();
                        if (DataType.IsNullOrEmpty(todoEmps) == false)
                        {
                            string[] strs = todoEmps.Split(';');
                            todoEmps = "";
                            foreach (string str in strs)
                            {
                                if (DataType.IsNullOrEmpty(str) == true || empsStrs.Contains(str) == true)
                                    continue;
                                empsStrs += str.Split(',')[0]+"@";
                            }
                        }
                    }
                    string[] emps = empsStrs.Split('@');
                    string empNo = ""; 
                    foreach (string str in emps)
                    {
                        if (DataType.IsNullOrEmpty(str))
                            continue;

                        empNo = str;
                        if (str.IndexOf(",") != -1)
                            empNo = str.Split(',')[0];

                        // 因为要发给不同的人，所有需要clone 一下，然后替换发送.
                        string smsDoccReal = smsDoc.Clone() as string;
                        smsDoc = smsDoc.Replace("{EmpStr}", empNo);
                        openUrl = openUrl.Replace("{EmpStr}", empNo);
                        string paras = "@FK_Flow=" + currNode.FK_Flow + "&FK_Node=" + currNode.NodeID + "@WorkID=" + workid+"_"+ empNo;

                        //发送消息
                        BP.WF.Dev2Interface.Port_SendMessage(empNo, smsDoc, mailTitle, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, openUrl, this.SMSPushModel, paras);
                    }
                    return "@宛先：{" + empsStrs + "}リマインダーメッセージを送信。";
                }
                #endregion 流程结束后、流程删除后

                #region 节点预警、逾期
                if(this.FK_Event == BP.Sys.EventListOfNode.NodeWarning
                    || this.FK_Event == BP.Sys.EventListOfNode.NodeOverDue)
                {
                    //获取当前节点的接收人
                    GenerWorkFlow gwf = new GenerWorkFlow(workid);
                    string[] emps = gwf.TodoEmps.Split(';');
                    foreach (string emp in emps)
                    {
                        if (DataType.IsNullOrEmpty(emp))
                            continue;
                        string[] empA = emp.Split(',');
                        if (DataType.IsNullOrEmpty(empA[0]) ==true || empA[0] == WebUser.No)
                            continue;
                        // 因为要发给不同的人，所有需要clone 一下，然后替换发送.
                        string smsDocReal = smsDoc.Clone() as string;
                        smsDocReal = smsDocReal.Replace("{EmpStr}", empA[0]);
                        openUrl = openUrl.Replace("{EmpStr}", empA[0]);
                        string paras = "@FK_Flow=" + currNode.FK_Flow + "&FK_Node=" + currNode.NodeID + "@WorkID=" + workid;
                        BP.WF.Dev2Interface.Port_SendMessage(empA[0], smsDoc, mailTitle, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, openUrl, this.SMSPushModel, paras);
                    }
                }
                #endregion 节点预警、逾期

            }
            #endregion 不同的消息事件，接收人不同的处理

            #endregion  消息发送

            return "";

        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="openWorkURl"></param>
        /// <param name="en"></param>
        /// <param name="jumpToEmps"></param>
        /// <param name="currNode"></param>
        /// <param name="workid"></param>
        /// <param name="objs"></param>
        /// <param name="r">处理好的变量集合</param>
        /// <returns></returns>
        //private string SendEmail(string title, string openWorkURl, Entity en, string jumpToEmps, Node currNode, Int64 workid, SendReturnObjs objs, Row r)
        //{
        //    if (this.MailPushWay == 0)
        //        return "";

        //    #region 生成相关的变量？
        //    string generAlertMessage = ""; //定义要返回的提示消息.
        //    // 发送邮件.
        //    string mailTitleTmp = ""; //定义标题.
        //    string mailDocTmp = ""; //定义内容.
           

            

        //    // 内容.
        //    mailDocTmp = this.MailDoc;
        //    mailDocTmp = mailDocTmp.Replace("{Url}", openWorkURl);
        //    mailDocTmp = mailDocTmp.Replace("{Title}", title);
        //    mailDocTmp = mailDocTmp.Replace("{Title}", title);

        //    mailDocTmp = mailDocTmp.Replace("@WebUser.No", WebUser.No);
        //    mailDocTmp = mailDocTmp.Replace("@WebUser.Name", WebUser.Name);

        //    /*如果仍然有没有替换下来的变量.*/
        //    if (mailDocTmp.Contains("@"))
        //        mailDocTmp = BP.WF.Glo.DealExp(mailDocTmp, en, null);

        //    #endregion 生成相关的变量？

        //    //求发送给的人员 ID.
        //    string toEmpIDs = "";

        //    #region 如果发送给指定的节点处理人, 就计算出来直接退回, 任何方式的处理人都是一致的.
        //    if (this.MailPushWay == 3)
        //    {
        //        /*如果向指定的字段作为发送邮件的对象, 从字段里取数据. */
        //        string[] nodes = this.SMSNodes.Split(',');

        //        string msg = "";
        //        foreach (string nodeID in nodes)
        //        {
        //            if (DataType.IsNullOrEmpty(nodeID) == true)
        //                continue;
        //            if (this.FK_Event == BP.Sys.EventListOfNode.ReturnAfter)
        //            {
        //                //获取退回原因
        //                Paras ps = new Paras();
        //                ps.SQL = "SELECT BeiZhu,ReturnerName,IsBackTracking FROM WF_ReturnWork WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID  ORDER BY RDT DESC";
        //                ps.Add(ReturnWorkAttr.WorkID,Int64.Parse( en.PKVal.ToString()));
        //                DataTable retunWdt = DBAccess.RunSQLReturnTable(ps);
        //                if (retunWdt.Rows.Count != 0)
        //                {
        //                    string returnMsg = retunWdt.Rows[0]["BeiZhu"].ToString();
        //                    string returner = retunWdt.Rows[0]["ReturnerName"].ToString();
        //                    mailDocTmp = mailDocTmp.Replace("ReturnMsg", returnMsg);

        //                }
        //            }

        //            string sql = "SELECT b.Name, b.Email, b.No FROM ND" + int.Parse(this.FK_Flow) + "Track a, WF_Emp b WHERE  a.ActionType=1 AND A.WorkID=" + workid + " AND a.NDFrom=" + nodeID + " AND a.EmpFrom=B.No ";
        //            DataTable dt = DBAccess.RunSQLReturnTable(sql);
        //            if (dt.Rows.Count == 0)
        //                continue;

        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                string emailAddress = dr["Email"].ToString();
        //                string empName = dr["Name"].ToString();
        //                string empNo = dr["No"].ToString();

        //                if (DataType.IsNullOrEmpty(emailAddress))
        //                    continue;

        //                string paras = "@FK_Flow=" + currNode.FK_Flow + "&FK_Node=" + currNode.NodeID + "@WorkID=" + workid;
        //                //发送邮件
        //                BP.WF.Dev2Interface.Port_SendEmail(emailAddress, mailTitleTmp, mailDocTmp, this.FK_Event, this.FK_Event + workid, BP.Web.WebUser.No, null, empNo, paras);
        //                msg += dr["Name"].ToString() + ",";
        //            }
        //        }
        //        return "@已向:{" + msg + "}发送提醒消息.";


        //    }
        //    #endregion 如果发送给指定的节点处理人, 就计算出来直接退回, 任何方式的处理人都是一致的.

        //    #region WorkArrive-工作到达. - 邮件处理.
        //    if (this.FK_Event == BP.Sys.EventListOfNode.WorkArrive || this.FK_Event == BP.Sys.EventListOfNode.ReturnAfter
        //        || this.FK_Event == BP.Sys.EventListOfNode.NodeWarning || this.FK_Event == BP.Sys.EventListOfNode.NodeOverDue)
        //    {
        //        if (this.FK_Event == BP.Sys.EventListOfNode.ReturnAfter)
        //        {
        //            //获取退回原因
        //            Paras ps = new Paras();
        //            ps.SQL = "SELECT BeiZhu,ReturnerName,IsBackTracking FROM WF_ReturnWork WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID  ORDER BY RDT DESC";
        //            ps.Add(ReturnWorkAttr.WorkID, Int64.Parse( en.PKVal.ToString()));
        //            DataTable retunWdt = DBAccess.RunSQLReturnTable(ps);
        //            if (retunWdt.Rows.Count != 0)
        //            {
        //                string returnMsg = retunWdt.Rows[0]["BeiZhu"].ToString();
        //                string returner = retunWdt.Rows[0]["ReturnerName"].ToString();
        //                mailDocTmp = mailDocTmp.Replace("ReturnMsg", returnMsg);

        //            }
        //        }
        //        /*工作到达.*/
        //        if (this.MailPushWay == 1 && !string.IsNullOrWhiteSpace(jumpToEmps))
        //        {
        //            /*如果向接受人发送邮件.*/
        //            toEmpIDs = jumpToEmps;
        //            string[] emps = toEmpIDs.Split(',');
        //            foreach (string emp in emps)
        //            {
        //                if (DataType.IsNullOrEmpty(emp))
        //                    continue;

        //                // 因为要发给不同的人，所有需要clone 一下，然后替换发送.
        //                string mailDocReal = mailDocTmp.Clone() as string;
        //                mailDocReal = mailDocReal.Replace("{EmpStr}", emp);

        //                //获得当前人的邮件.
        //                BP.WF.Port.WFEmp empEn = new BP.WF.Port.WFEmp(emp);

        //                //发送邮件.
        //                BP.WF.Dev2Interface.Port_SendEmail(empEn.Email, mailTitleTmp, mailDocReal, this.FK_Event,
        //                    "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, null, emp);
        //            }
        //            return "@已向:{" + toEmpIDs + "}发送提醒信息.";
        //        }

        //        if (this.MailPushWay == 2)
        //        {
        //            /*如果向指定的字段作为发送邮件的对象, 从字段里取数据. */
        //            string emailAddress = r[this.MailAddress] as string;

        //            //发送邮件
        //            BP.WF.Dev2Interface.Port_SendEmail(emailAddress, mailTitleTmp, mailDocTmp, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, null, null);
        //            return "@已向:{" + emailAddress + "}发送提醒信息.";
        //        }
        //    }
        //    #endregion 发送成功事件.

        //    #region SendSuccess - 发送成功事件. - 邮件处理.
        //    if (this.FK_Event == BP.Sys.EventListOfNode.SendSuccess)
        //    {
        //        /*发送成功事件.*/
        //        if (this.MailPushWay == 1 && objs.VarAcceptersID != null)
        //        {
        //            /*如果向接受人发送邮件.*/
        //            toEmpIDs = objs.VarAcceptersID;
        //            string[] emps = toEmpIDs.Split(',');
        //            foreach (string emp in emps)
        //            {
        //                if (DataType.IsNullOrEmpty(emp))
        //                    continue;
        //                if (emp == WebUser.No)
        //                    continue;

        //                // 因为要发给不同的人，所有需要clone 一下，然后替换发送.
        //                string mailDocReal = mailDocTmp.Clone() as string;
        //                mailDocReal = mailDocReal.Replace("{EmpStr}", emp);

        //                //获得当前人的邮件.
        //                BP.WF.Port.WFEmp empEn = new BP.WF.Port.WFEmp(emp);

        //                string paras = "@FK_Flow=" + currNode.FK_Flow + "&FK_Node=" + currNode.NodeID + "@WorkID=" + workid;

        //                string mail = empEn.Email;
        //                if (DataType.IsNullOrEmpty(mail) == true)
        //                {
        //                    BP.GPM.Emp empGPM = new GPM.Emp(emp);
        //                    mail = empGPM.Email;
        //                    empEn.Email = mail;
        //                    empEn.Update();
        //                }

        //                //发送邮件.
        //                BP.WF.Dev2Interface.Port_SendEmail(mail, mailTitleTmp, mailDocReal, this.FK_Event, "WKAlt" + objs.VarToNodeID + "_" + workid, BP.Web.WebUser.No, null, emp, paras);

        //            }
        //            return "@已向:{" + toEmpIDs + "}发送提醒信息.";
        //        }

        //        if (this.MailPushWay == 2)
        //        {
        //            /*如果向指定的字段作为发送邮件的对象, 从字段里取数据. */
        //            string emailAddress = r[this.MailAddress] as string;
        //            string paras = "@FK_Flow=" + currNode.FK_Flow + "&FK_Node=" + currNode.NodeID + "@WorkID=" + workid;

        //            //发送邮件
        //            BP.WF.Dev2Interface.Port_SendEmail(emailAddress, mailTitleTmp, mailDocTmp, this.FK_Event, "WKAlt" + objs.VarToNodeID + "_" + workid, BP.Web.WebUser.No, null, null, paras);

        //            return "@已向:{" + emailAddress + "}发送提醒信息.";
        //        }
        //    }
        //    #endregion 发送成功事件.



        //    #region SendSuccess - 流程结束/预警/逾期. - 邮件处理.
        //    if (this.FK_Event == BP.Sys.EventListOfNode.FlowOverAfter
        //        || this.FK_Event == BP.Sys.EventListOfNode.FlowWarning
        //        || this.FK_Event == BP.Sys.EventListOfNode.FlowOverDue)
        //    {
        //        /*发送成功事件.*/
        //        if (this.MailPushWay == 1)
        //        {
        //            /*如果向接受人发送邮件.*/
        //            /*向所有参与人. */
        //            string empsStrs = DBAccess.RunSQLReturnStringIsNull("SELECT Emps FROM WF_GenerWorkFlow WHERE WorkID=" + workid, "");
        //            string[] emps = empsStrs.Split('@');

        //            foreach (string emp in emps)
        //            {
        //                if (DataType.IsNullOrEmpty(emp))
        //                    continue;

        //                // 因为要发给不同的人，所有需要clone 一下，然后替换发送.
        //                string mailDocReal = mailDocTmp.Clone() as string;
        //                mailDocReal = mailDocReal.Replace("{EmpStr}", emp);

        //                //获得当前人的邮件.
        //                BP.WF.Port.WFEmp empEn = new BP.WF.Port.WFEmp(emp);

        //                string paras = "@FK_Flow=" + currNode.FK_Flow + "&FK_Node=" + currNode.NodeID + "@WorkID=" + workid;

        //                //发送邮件.
        //                BP.WF.Dev2Interface.Port_SendEmail(empEn.Email, mailTitleTmp, mailDocReal, this.FK_Event, "FlowOver" + workid, BP.Web.WebUser.No, null, emp, paras);
        //            }
        //            return "@已向:{" + toEmpIDs + "}发送提醒信息.";
        //        }

        //        if (this.MailPushWay == 2)
        //        {
        //            /*如果向指定的字段作为发送邮件的对象, 从字段里取数据. */
        //            string emailAddress = r[this.MailAddress] as string;

        //            string paras = "@FK_Flow=" + currNode.FK_Flow + "&FK_Node=" + currNode.NodeID + "@WorkID=" + workid;

        //            //发送邮件
        //            BP.WF.Dev2Interface.Port_SendEmail(emailAddress, mailTitleTmp, mailDocTmp, this.FK_Event, "FlowOver" + workid, BP.Web.WebUser.No, null, null, paras);
        //            return "@已向:{" + emailAddress + "}发送提醒信息.";
        //        }
        //    }
        //    #endregion 发送成功事件.

        //    return generAlertMessage;
        //}
        /// <summary>
        /// 发送短消息.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="openWorkURl"></param>
        /// <param name="en"></param>
        /// <param name="jumpToEmps"></param>
        /// <param name="currNode"></param>
        /// <param name="workid"></param>
        /// <param name="objs"></param>
        /// <param name="r">处理好的变量集合</param>
        /// <returns></returns>
        //private string SendShortMessageToSpecNodes(string title, string openWorkURL, Entity en, Node currNode, Int64 workid, SendReturnObjs objs, Row r, string SendToEmpIDs)
        //{
        //    if (this.SMSPushWay == 0)
        //        return "";

        //    //定义短信内容.......
        //    string smsDocTmp = "";

        //    #region  生成短信内容
        //    smsDocTmp = this.SMSDoc.Clone() as string;
        //    smsDocTmp = smsDocTmp.Replace("{Title}", title);
        //    smsDocTmp = smsDocTmp.Replace("{Url}", openWorkURL);
        //    smsDocTmp = smsDocTmp.Replace("@WebUser.No", WebUser.No);
        //    smsDocTmp = smsDocTmp.Replace("@WebUser.Name", WebUser.Name);
        //    smsDocTmp = smsDocTmp.Replace("@WorkID", en.PKVal.ToString());
        //    smsDocTmp = smsDocTmp.Replace("@OID", en.PKVal.ToString());

        //    /*如果仍然有没有替换下来的变量.*/
        //    if (smsDocTmp.Contains("@") == true)
        //        smsDocTmp = BP.WF.Glo.DealExp(smsDocTmp, en, null);

        //    /*如果仍然有没有替换下来的变量.*/
        //    if (smsDocTmp.Contains("@"))
        //        smsDocTmp = BP.WF.Glo.DealExp(smsDocTmp, en, null);

        //    //if (smsDocTmp.Contains("@"))
        //    //    throw new Exception("@短信消息内容配置错误,里面有未替换的变量，请确认参数是否正确:"+smsDocTmp);

        //    string toEmpIDs = "";
        //    #endregion 处理当前的内容.

        //    #region 如果发送给指定的节点处理人,就计算出来直接退回,任何方式的处理人都是一致的.
        //    if (this.SMSPushWay == 3)
        //    {
        //        /*如果向指定的字段作为发送邮件的对象, 从字段里取数据. */
        //        string[] nodes = this.SMSNodes.Split(',');

        //        string msg = "";
        //        foreach (string nodeID in nodes)
        //        {
        //            if (DataType.IsNullOrEmpty(nodeID) == true)
        //                continue;

        //            string sql = "SELECT b.Name, b.Tel ,b.No FROM ND" + int.Parse(this.FK_Flow) + "Track a, WF_Emp b WHERE  a.ActionType=1 AND A.WorkID=" + workid + " AND a.NDFrom=" + nodeID + " AND a.EmpFrom=B.No ";
        //            DataTable dt = DBAccess.RunSQLReturnTable(sql);
        //            if (dt.Rows.Count == 0)
        //                continue;

        //            if (this.FK_Event == BP.Sys.EventListOfNode.ReturnAfter)
        //            {
        //                //获取退回原因
        //                Paras ps = new Paras();
        //                ps.SQL = "SELECT BeiZhu,ReturnerName,IsBackTracking FROM WF_ReturnWork WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID  ORDER BY RDT DESC";
        //                ps.Add(ReturnWorkAttr.WorkID, Int64.Parse( en.PKVal.ToString()));
        //                DataTable retunWdt = DBAccess.RunSQLReturnTable(ps);
        //                if (retunWdt.Rows.Count != 0)
        //                {
        //                    string returnMsg = retunWdt.Rows[0]["BeiZhu"].ToString();
        //                    string returner = retunWdt.Rows[0]["ReturnerName"].ToString();
        //                    smsDocTmp = smsDocTmp.Replace("ReturnMsg", returnMsg);

        //                }
        //            }


        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                string tel = dr["Tel"].ToString();
        //                string empName = dr["Name"].ToString();
        //                string empNo = dr["No"].ToString();

        //                if (DataType.IsNullOrEmpty(tel))
        //                    continue;

        //                // 因为要发给不同的人，所有需要clone 一下，然后替换发送.
        //                string mailDocReal = smsDocTmp.Clone() as string;
        //                mailDocReal = mailDocReal.Replace("{EmpStr}", empName);
        //                openWorkURL = openWorkURL.Replace("{EmpStr}", empName);

        //                string paras = "@FK_Flow=" + this.FK_Flow + "@WorkID=" + workid + "@FK_Node=" + this.FK_Node;

        //                //发送邮件.
        //                BP.WF.Dev2Interface.Port_SendSMS(tel, mailDocReal, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, WebUser.No, null, empNo, paras, title, openWorkURL, this.SMSPushModel);

        //                //处理短消息.
        //                toEmpIDs += empName + ",";
        //            }
        //        }
        //        return "@已向:{" + toEmpIDs + "}发送了短消息提醒.";
        //    }
        //    #endregion 如果发送给指定的节点处理人, 就计算出来直接退回, 任何方式的处理人都是一致的.

        //    //求发送给的人员ID.
        //    #region WorkArrive - 工作到达事件.
        //    if (this.FK_Event == BP.Sys.EventListOfNode.WorkArrive
        //        || this.FK_Event == BP.Sys.EventListOfNode.ReturnAfter
        //        || this.FK_Event == BP.Sys.EventListOfNode.NodeWarning 
        //        || this.FK_Event == BP.Sys.EventListOfNode.NodeOverDue)
        //    {
        //        if (this.FK_Event == BP.Sys.EventListOfNode.ReturnAfter)
        //        {
        //            //获取退回原因
        //            Paras ps = new Paras();
        //            ps.SQL = "SELECT BeiZhu,ReturnerName,IsBackTracking FROM WF_ReturnWork WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID  ORDER BY RDT DESC";
        //            ps.Add(ReturnWorkAttr.WorkID, Int64.Parse( en.PKVal.ToString()));
        //            DataTable dt = DBAccess.RunSQLReturnTable(ps);
        //            if (dt.Rows.Count != 0)
        //            {
        //                string returnMsg = dt.Rows[0]["BeiZhu"].ToString();
        //                string returner = dt.Rows[0]["ReturnerName"].ToString();
        //                smsDocTmp = smsDocTmp.Replace("ReturnMsg", returnMsg);

        //            }
        //        }
        //        /*发送成功事件, 退回后事件. */
        //        if (this.SMSPushWay == 1)
        //        {
        //            /*如果向接受人发送短信.*/
        //            toEmpIDs = SendToEmpIDs;
        //            if (DataType.IsNullOrEmpty(toEmpIDs) == false)
        //            {
        //                string[] emps = toEmpIDs.Split(',');
        //                foreach (string emp in emps)
        //                {
        //                    if (DataType.IsNullOrEmpty(emp))
        //                        continue;

        //                    string smsDocTmpReal = smsDocTmp.Clone() as string;
        //                    smsDocTmpReal = smsDocTmpReal.Replace("{EmpStr}", emp);
        //                    openWorkURL = openWorkURL.Replace("{EmpStr}", emp);
        //                    BP.WF.Port.WFEmp empEn = new Port.WFEmp(emp);

        //                    string paras = "@FK_Flow=" + currNode.FK_Flow + "@WorkID=" + workid + "@FK_Node=" + currNode.NodeID;

        //                    //发送短信.
        //                    Dev2Interface.Port_SendSMS(empEn.Tel, smsDocTmpReal, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, null, emp, null, title, openWorkURL, this.SMSPushModel);
        //                }
        //                //return "@已向:{" + toEmpIDs + "}发送提醒手机短信，由 " + this.FK_Event + " 发出.";
        //                return "@已向:{" + toEmpIDs + "}发送提醒消息.";
        //            }
        //        }

        //        if (this.SMSPushWay == 2)
        //        {
        //            /*如果向指定的字段作为发送邮件的对象, 从字段里取数据. */
        //            string tel = r[this.SMSField] as string;

        //            //发送短信.
        //            string paras = "@FK_Flow=" + currNode.FK_Flow + "@WorkID=" + workid + "@FK_Node=" + currNode.NodeID;
        //            BP.WF.Dev2Interface.Port_SendSMS(tel, smsDocTmp, this.FK_Event, "WKAlt" + currNode.NodeID + "_" + workid, BP.Web.WebUser.No, null, paras, title, openWorkURL, this.SMSPushModel);
        //            return "@已向:{" + tel + "}发送提醒消息.";
        //            //  return "@已向:{" + tel + "}发送提醒手机短信，由 " + this.FK_Event + " 发出.";

        //        }
        //    }
        //    #endregion WorkArrive - 工作到达事件

        //    #region SendSuccess - 发送成功事件
        //    if (this.FK_Event == BP.Sys.EventListOfNode.SendSuccess)
        //    {
        //        /*发送成功事件.*/
        //        if (this.SMSPushWay == 1 && objs.VarAcceptersID != null)
        //        {
        //            /*如果向接受人发送短信.*/
        //            toEmpIDs = objs.VarAcceptersID;

        //            toEmpIDs = toEmpIDs.Replace("(", "");
        //            toEmpIDs = toEmpIDs.Replace(")", "");

        //            string hasSendEmps = ",";

        //            if (DataType.IsNullOrEmpty(toEmpIDs) == false)
        //            {
        //                #warning 人员是（zhangyifan,张一凡） 获取结果有问题
        //                string[] emps = toEmpIDs.Split(';');
        //                foreach (string empID in emps)
        //                {
        //                    if (DataType.IsNullOrEmpty(empID))
        //                        continue;
        //                    string[] empIDs = empID.Split(',');
        //                    if (DataType.IsNullOrEmpty(empIDs[0]))
        //                        continue;

        //                    if (hasSendEmps.Contains("," + empIDs[0] + ",") == true)
        //                        continue;

        //                    hasSendEmps += empIDs[0] + ",";

        //                    string smsDocTmpReal = smsDocTmp.Clone() as string;
        //                    smsDocTmpReal = smsDocTmpReal.Replace("{EmpStr}", empIDs[0]);
        //                    openWorkURL = openWorkURL.Replace("{EmpStr}", empIDs[0]);

        //                    string myEmpID = empIDs[0];
        //                    BP.GPM.Emp empEn = new BP.GPM.Emp();
        //                    empEn.No = myEmpID;
        //                    if (empEn.RetrieveFromDBSources() == 0)
        //                        continue;
                             
        //                    string paras = "@FK_Flow=" + currNode.FK_Flow + "@WorkID=" + workid + "@FK_Node=" + currNode.NodeID;

        //                    //发送短信.
        //                    Dev2Interface.Port_SendSMS(empEn.Tel, smsDocTmpReal, this.FK_Event, "WKAlt" + objs.VarToNodeID + "_" + workid, BP.Web.WebUser.No, null, empID, paras, title, openWorkURL, this.SMSPushModel);
        //                }
        //                return "";
        //                //return "@已向:{" + toEmpIDs + "}发送提醒消息.";
        //            }
        //        }

        //        if (this.SMSPushWay == 2)
        //        {
        //            /*如果向指定的字段作为发送短信的发送对象, 从字段里取数据. */
        //            string tel = r[this.SMSField] as string;
        //            if (tel != null || tel.Length > 6)
        //            {

        //                string paras = "@FK_Flow=" + currNode.FK_Flow + "@WorkID=" + workid + "@FK_Node=" + currNode.NodeID;

        //                //发送短信.
        //                BP.WF.Dev2Interface.Port_SendSMS(tel, smsDocTmp, this.FK_Event, "WKAlt" + objs.VarToNodeID + "_" + workid, BP.Web.WebUser.No, null, paras, title, openWorkURL, this.SMSPushModel);
        //                return "@已向:{" + tel + "}发送提醒手机短信.";
        //            }
        //        }
        //    }
        //    #endregion SendSuccess - 发送成功事件

        //    #region FlowOverAfter -  流程结束/预警/逾期  短信息事件
        //    if (this.FK_Event == BP.Sys.EventListOfNode.FlowOverAfter
        //        || this.FK_Event ==BP.Sys.EventListOfNode.FlowWarning
        //        || this.FK_Event == BP.Sys.EventListOfNode.FlowOverDue)
        //    {
        //        /*发送成功事件.*/
        //        if (this.SMSPushWay == 1)
        //        {
        //            /*向所有参与人. */
        //            string empsStrs = DBAccess.RunSQLReturnStringIsNull("SELECT Emps FROM WF_GenerWorkFlow WHERE WorkID=" + workid, "");
        //            if (DataType.IsNullOrEmpty(empsStrs) == false)
        //            {
        //                string[] emps = empsStrs.Split('@');
        //                foreach (string empID in emps)
        //                {
        //                    if (DataType.IsNullOrEmpty(empID))
        //                        continue;

        //                    if (empID == WebUser.No)
        //                        continue;

        //                    string smsDocTmpReal = smsDocTmp.Clone() as string;
        //                    smsDocTmpReal = smsDocTmpReal.Replace("{EmpStr}", empID);
        //                    openWorkURL = openWorkURL.Replace("{EmpStr}", empID);

        //                    BP.WF.Port.WFEmp empEn = new Port.WFEmp();
        //                    empEn.No = empID;
        //                    empEn.RetrieveFromDBSources();

        //                    string paras = "@FK_Flow=" + currNode.FK_Flow + "@WorkID=" + workid + "@FK_Node=" + currNode.NodeID;

        //                    //发送短信.
        //                    Dev2Interface.Port_SendSMS(empEn.Tel, smsDocTmpReal, this.FK_Event, "FlowOver" + workid, BP.Web.WebUser.No, null, empID, paras, title, openWorkURL, this.SMSPushModel);
        //                }

        //                return "";
        //                //return "@已向:{" + toEmpIDs + "}发送提醒消息.";

        //            }
        //        }

        //        if (this.SMSPushWay == 2)
        //        {
        //            /*如果向指定的字段作为发送短信的发送对象, 从字段里取数据. */
        //            string tel = r[this.SMSField] as string;
        //            if (tel != null || tel.Length > 6)
        //            {
        //                string paras = "@FK_Flow=" + currNode.FK_Flow + "@WorkID=" + workid + "@FK_Node=" + currNode.NodeID;
        //                //发送短信.
        //                BP.WF.Dev2Interface.Port_SendSMS(tel, smsDocTmp, this.FK_Event, "FlowOver" + workid, BP.Web.WebUser.No, null, paras, title, openWorkURL, this.SMSPushModel);
        //                return "@已向:{" + tel + "}发送提醒消息.";
        //            }
        //        }
        //    }
        //    #endregion SendSuccess - 发送成功事件

        //    return "";
        //}

        /// <summary>
        /// 发送短信到其它节点的处理人.
        /// </summary>
        private void SendShortMessageToSpecNodeWorks()
        {
        }
        protected override bool beforeUpdateInsertAction()
        {
            //  this.MyPK = this.FK_Event + "_" + this.FK_Node + "_" + this.PushWay;

            string sql = "UPDATE WF_PushMsg SET FK_Flow=(SELECT FK_Flow FROM WF_Node WHERE NodeID= WF_PushMsg.FK_Node)";
            BP.DA.DBAccess.RunSQL(sql);

            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 消息推送
    /// </summary>
    public class PushMsgs : EntitiesMyPK
    {
        /// <summary>
        /// 消息推送
        /// </summary>
        public PushMsgs() { }
        /// <summary>
        /// 消息推送
        /// </summary>
        /// <param name="fk_flow"></param>
        public PushMsgs(string fk_flow)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(PushMsgAttr.FK_Node, "SELECT NodeID FROM WF_Node WHERE FK_Flow='" + fk_flow + "'");
            qo.DoQuery();
        }
        /// <summary>
        /// 消息推送
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public PushMsgs(int nodeid)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(PushMsgAttr.FK_Node, nodeid);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new PushMsg();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<PushMsg> ToJavaList()
        {
            return (System.Collections.Generic.IList<PushMsg>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<PushMsg> Tolist()
        {
            System.Collections.Generic.List<PushMsg> list = new System.Collections.Generic.List<PushMsg>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((PushMsg)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
