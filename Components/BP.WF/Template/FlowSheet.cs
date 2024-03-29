﻿using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF.Data;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程
    /// </summary>
    public class FlowSheet : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 流程事件实体
        /// </summary>
        public string FlowEventEntity
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FlowEventEntity);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowEventEntity, value);
            }
        }
        /// <summary>
        /// 流程标记
        /// </summary>
        public string FlowMark
        {
            get
            {
                string str = this.GetValStringByKey(FlowAttr.FlowMark);
                if (str == "")
                    return this.No;
                return str;
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowMark, value);
            }
        }

        #region   前置导航
        /// <summary>
        /// 前置导航方式
        /// </summary>
        public StartGuideWay StartGuideWay
        {
            get
            {
                return (StartGuideWay)this.GetValIntByKey(FlowAttr.StartGuideWay);
                
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuideWay, (int)value);
            }
        
        }
        /// <summary>
        /// 前置导航参数1
        /// </summary>
        public string StartGuidePara1
        {
            get
            {
                string str= this.GetValStringByKey(FlowAttr.StartGuidePara1);
                return str.Replace("~", "'");
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara1, value);
            }
             
        }
        /// <summary>
        /// 前置导航参数2
        /// </summary>
        public string StartGuidePara2
        {

            get
            {
                string str = this.GetValStringByKey(FlowAttr.StartGuidePara2);
                return str.Replace("~", "'");
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara2, value);
            }

        }
        /// <summary>
        /// 前置导航参数3
        /// </summary>
        public string StartGuidePara3
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.StartGuidePara3);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara3, value);
            }

        }

        /// <summary>
        /// 启动方式
        /// </summary>
        public FlowRunWay FlowRunWay
        {
            get
            {
                return (FlowRunWay)this.GetValIntByKey(FlowAttr.FlowRunWay);

            }
            set
            {
                this.SetValByKey(FlowAttr.FlowRunWay, (int)value);
            }

        }

        /// <summary>
        /// 运行内容
        /// </summary>
        public string RunObj
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.RunObj);
            }
            set
            {
                this.SetValByKey(FlowAttr.RunObj, value);
            }

        }

        /// <summary>
        /// 是否启用开始节点数据重置按钮
        /// </summary>
        public bool IsResetData
        {

            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsResetData);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsResetData, value);
            }
        }

        /// <summary>
        /// 是否自动装载上一笔数据
        /// </summary>
        public bool IsLoadPriData

        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsLoadPriData);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsLoadPriData, value);
            }
        }
        #endregion        
        /// <summary>
        /// 设计者编号
        /// </summary>
        public string DesignerNo
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerNo);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerNo, value);
            }
        }
        /// <summary>
        /// 设计者名称
        /// </summary>
        public string DesignerName
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerName);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerName, value);
            }
        }
        /// <summary>
        /// 编号生成格式
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.BillNoFormat);
            }
            set
            {
                this.SetValByKey(FlowAttr.BillNoFormat, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No == "admin" || this.DesignerNo == WebUser.No)
                {
                    uac.IsUpdate = true;
                }
                return uac;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public FlowSheet()
        {
        }
        /// <summary>
        /// 流程
        /// </summary>
        /// <param name="_No">编号</param>
        public FlowSheet(string _No)
        {
            this.No = _No;
            if (SystemConfig.IsDebug)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("フローIDは存在しません");
            }
            else
            {
                this.Retrieve();
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

                Map map = new Map("WF_Flow", "処理する");
                map.Java_SetCodeStruct("3");

                #region 基本属性。
                map.AddTBStringPK(FlowAttr.No, null, "ナンバリング", true, true, 1, 10, 3);
                map.SetHelperUrl(FlowAttr.No, "http://ccbpm.mydoc.io/?v=5404&t=17023"); //使用alert的方式显示帮助信息.

                map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "フローカテゴリ", new FlowSorts(), true);

                map.SetHelperUrl(FlowAttr.FK_FlowSort, "http://ccbpm.mydoc.io/?v=5404&t=17024");
                map.AddTBString(FlowAttr.Name, null, "名前", true, false, 0, 50, 10, true);

                // add 2013-02-14 唯一确定此流程的标记
                map.AddTBString(FlowAttr.FlowMark, null, "フローマーク", true, false, 0, 150, 10);
                map.AddTBString(FlowAttr.FlowEventEntity, null, "フローイベントエンティティ", true, true, 0, 150, 10);
                map.SetHelperUrl(FlowAttr.FlowMark, "http://ccbpm.mydoc.io/?v=5404&t=16847");
                map.SetHelperUrl(FlowAttr.FlowEventEntity, "http://ccbpm.mydoc.io/?v=5404&t=17026");

                // add 2013-02-05.
                map.AddTBString(FlowAttr.TitleRole, null, "タイトル生成ルール", true, false, 0, 150, 10, true);
                map.SetHelperUrl(FlowAttr.TitleRole, "http://ccbpm.mydoc.io/?v=5404&t=17040");

                //add  2013-08-30.
                map.AddTBString(FlowAttr.BillNoFormat, null, "文書番号の形式", true, false, 0, 50, 10, false);
                map.SetHelperUrl(FlowAttr.BillNoFormat, "http://ccbpm.mydoc.io/?v=5404&t=17041");

                // add 2014-10-19.
                map.AddDDLSysEnum(FlowAttr.ChartType, (int)FlowChartType.Icon, "ノードグラフタイプ", true, true,
                    "ChartType", "@0=ジオメトリ@1=肖像画");

                map.AddBoolean(FlowAttr.IsCanStart, true, "独立して開始できますか？ （独立して開始されたフローは、開始されたフローのリストに表示できます）", true, true, true);
                map.SetHelperUrl(FlowAttr.IsCanStart, "http://ccbpm.mydoc.io/?v=5404&t=17027");


             

                map.AddBoolean(FlowAttr.IsMD5, false, "データ暗号化フローですか（MD5データ暗号化は改ざん防止）", true, true, true);
                map.SetHelperUrl(FlowAttr.IsMD5, "http://ccbpm.mydoc.io/?v=5404&t=17028");
                
                map.AddBoolean(FlowAttr.IsFullSA, false, "将来のプロセッサは自動的に計算されますか？", true, true, true);
                map.SetHelperUrl(FlowAttr.IsFullSA, "http://ccbpm.mydoc.io/?v=5404&t=17034");


                map.AddBoolean(FlowAttr.IsGuestFlow, false, "外部ユーザーがフローに参加するかどうか（非組織構造の担当者が関係するフロー）", true, true, false);
                map.SetHelperUrl(FlowAttr.IsGuestFlow, "http://ccbpm.mydoc.io/?v=5404&t=17039");
                //批量发起 add 2013-12-27. 
                map.AddBoolean(FlowAttr.IsBatchStart, false, "フローをバッチで開始することは可能ですか？ （入力する必要があるフィールドを設定する必要がある場合、複数はカンマで区切られます）", true, true, true);
                map.AddTBString(FlowAttr.BatchStartFields, null, "発信フィールドs", true, false, 0, 500, 10, true);
                map.SetHelperUrl(FlowAttr.IsBatchStart, "http://ccbpm.mydoc.io/?v=5404&t=17047");
                map.AddDDLSysEnum(FlowAttr.FlowAppType, (int)FlowAppType.Normal, "フローアプリケーションタイプ",
                  true, true, "FlowAppType", "@0=ビジネスフロー@1=エンジニアリング（プロジェクトチームフロー）@2=公式ドキュメントフロー（VSTO）");
                map.SetHelperUrl(FlowAttr.FlowAppType, "http://ccbpm.mydoc.io/?v=5404&t=17035");
                 
                // 草稿
                map.AddDDLSysEnum(FlowAttr.Draft, (int)DraftRole.None, "ドラフトルール",
               true, true, FlowAttr.Draft, "@0=なし（下書きなし）@1=to-doに保存@2=下書きボックスに保存");
                map.SetHelperUrl(FlowAttr.Draft, "http://ccbpm.mydoc.io/?v=5404&t=17037");

                // 数据存储.
                map.AddDDLSysEnum(FlowAttr.DataStoreModel, (int)DataStoreModel.ByCCFlow,
                    "フローデータストレージモード", true, true, FlowAttr.DataStoreModel,
                   "@0=データトラックモード@1=データマージモード");
                map.SetHelperUrl(FlowAttr.DataStoreModel, "http://ccbpm.mydoc.io/?v=5404&t=17038");

                //add 2013-05-22.
                map.AddTBString(FlowAttr.HistoryFields, null, "履歴ビューフィールド", true, false, 0, 500, 10, true);
                //map.SetHelperBaidu(FlowAttr.HistoryFields, "ccflow 历史查看字段");
                map.AddTBString(FlowAttr.FlowNoteExp, null, "注目の表現", true, false, 0, 500, 10, true);
                map.SetHelperUrl(FlowAttr.FlowNoteExp, "http://ccbpm.mydoc.io/?v=5404&t=17043");
                map.AddTBString(FlowAttr.Note, null, "過程説明", true, false, 0, 100, 10, true);

                map.AddDDLSysEnum(FlowAttr.FlowAppType, (int)FlowAppType.Normal, "フローアプリケーションタイプ", true, true,
                    "FlowAppType", "@0=ビジネスフロー@1=エンジニアリング（プロジェクトチームフロー）@2=公式ドキュメントフロー（VSTO）");
                map.AddTBString(FlowAttr.HelpUrl, null, "ヘルプドキュメント", true, false, 0, 300, 10, true);
                #endregion 基本属性。

                #region 启动方式
                map.AddDDLSysEnum(FlowAttr.FlowRunWay, (int)FlowRunWay.HandWork, "開始方法",
                    true, true, FlowAttr.FlowRunWay, "@0=手動スタート@1=指定された担当者による時間スタート@2=時間アクセスデータセット自動スタート@3=トリガースタート");

                map.SetHelperUrl(FlowAttr.FlowRunWay, "http://ccbpm.mydoc.io/?v=5404&t=17088");
                // map.AddTBString(FlowAttr.RunObj, null, "运行内容", true, false, 0, 100, 10, true);
                map.AddTBStringDoc(FlowAttr.RunObj, null, "コンテンツを実行する", true, false, true);
                #endregion 启动方式

                #region 流程启动限制
                string role = "@0=無制限";
                role += "@1=1人1日1回";
                role += "@2=週1回、1人あたり";
                role += "@3=1人1か月に1回";
                role += "@4=1人1シーズンに1回";
                role += "@5=1人1年に1回";
                role += "@6=開始された列を繰り返すことはできません（複数の列はコンマで区切ることができます）";
                role += "@7=設定されたSQLデータソースが空であるか、返される結果がゼロのときに開始できます";
                role += "@8=設定されたSQLデータソースが空であるか、返される結果がゼロの場合は開始できません";
                map.AddDDLSysEnum(FlowAttr.StartLimitRole, (int)StartLimitRole.None, "制限ルールを開始", true, true, FlowAttr.StartLimitRole, role, true);
                map.AddTBString(FlowAttr.StartLimitPara, null, "ルールパラメータ", true, false, 0, 500, 10, true);
                map.AddTBStringDoc(FlowAttr.StartLimitAlert, null, "制限事項", true, false, true);
                map.SetHelperUrl(FlowAttr.StartLimitRole, "http://ccbpm.mydoc.io/?v=5404&t=17872");
                //   map.AddTBString(FlowAttr.StartLimitAlert, null, "限制提示", true, false, 0, 500, 10, true);
                //    map.AddDDLSysEnum(FlowAttr.StartLimitWhen, (int)StartLimitWhen.StartFlow, "提示时间", true, true, FlowAttr.StartLimitWhen, "@0=启动流程时@1=发送前提示", false);
                #endregion 流程启动限制

                #region 发起前导航。
                //map.AddDDLSysEnum(FlowAttr.DataStoreModel, (int)DataStoreModel.ByCCFlow,
                //    "流程数据存储模式", true, true, FlowAttr.DataStoreModel,
                //   "@0=数据轨迹模式@1=数据合并模式");

                //发起前设置规则.
                map.AddDDLSysEnum(FlowAttr.StartGuideWay, 0, "フロントナビゲーション", true, true,
                    FlowAttr.StartGuideWay,
                    "@0=なし@1=システムURLによる（親子フロー）シングルモード@2=システムURLによる（子親フロー）マルチモード@3=システムURLによる（エンティティレコード、未完了）シングルモード@4=システムのURLに従って-（エンティティレコード、未完了）マルチモード@5=開始ノードからデータをコピー@10=カスタマイズされたUrlに従って@11=ユーザー入力パラメータに従って", true);
                map.SetHelperUrl(FlowAttr.StartGuideWay, "http://ccbpm.mydoc.io/?v=5404&t=17883");

                map.AddTBString(FlowAttr.StartGuidePara1, null, "パラメータ1", true, false,0,500,20,true);
                map.AddTBString(FlowAttr.StartGuidePara2, null, "パラメータ2", true, false, 0, 500, 20, true);
                map.AddTBString(FlowAttr.StartGuidePara3, null, "パラメータ3", true, false, 0, 500, 20, true);
                
                map.AddBoolean(FlowAttr.IsResetData, false, "開始ノードデータリセットボタンは有効になっていますか？", true, true, true);
                //     map.AddBoolean(FlowAttr.IsImpHistory, false, "是否启用导入历史数据按钮？", true, true, true);
                map.AddBoolean(FlowAttr.IsLoadPriData, false, "最後のデータを自動的にロードするかどうか。", true, true, true);

                #endregion 发起前导航。

                #region 延续流程。
                // add 2013-03-24.
                map.AddTBString(FlowAttr.DesignerNo, null, "デザイナー番号", false, false, 0, 32, 10);
                map.AddTBString(FlowAttr.DesignerName, null, "デザイナー名", false, false, 0, 100, 10);
                #endregion 延续流程。

                #region 数据同步方案
                //数据同步方式.
                map.AddDDLSysEnum(FlowAttr.DTSWay, (int)FlowDTSWay.None, "同期して", true, true,
                    FlowAttr.DTSWay, "@0=同期されていません@1=同期されています");
                map.SetHelperUrl(FlowAttr.DTSWay, "http://ccbpm.mydoc.io/?v=5404&t=17893");

                map.AddDDLEntities(FlowAttr.DTSDBSrc, "", "データベース", new BP.Sys.SFDBSrcs(), true);

                map.AddTBString(FlowAttr.DTSBTable, null, "ビジネステーブル名", true, false, 0, 50, 50, false);

                map.AddTBString(FlowAttr.DTSBTablePK, null, "ビジネステーブルの主キー", true, false, 0, 50, 50, false);
                map.SetHelperAlert(FlowAttr.DTSBTablePK, "同期方法がビジネステーブルの主キーフィールドに従って計算するように設定されている場合は、フローのノードフォームに同じ名前とタイプのフィールドを設定する必要があり、システムはこの主キーに従ってデータを同期します");

                map.AddTBString(FlowAttr.DTSFields, null, "同期するフィールドs、カンマで区切る", false, false, 0, 200, 100, false);

                map.AddDDLSysEnum(FlowAttr.DTSTime, (int)FlowDTSTime.AllNodeSend, "同期時点", true, true,
                   FlowAttr.DTSTime, "@0=すべてのノード@1=指定されたノードが送信された後@2=フローが終了したとき");
                map.SetHelperUrl(FlowAttr.DTSTime, "http://ccbpm.mydoc.io/?v=5404&t=17894");

                map.AddTBString(FlowAttr.DTSSpecNodes, null, "指定されたノードID", true, false, 0, 50, 50, false);
                map.SetHelperAlert(FlowAttr.DTSSpecNodes, "指定したノードが送信する同期時刻ポイントを選択した場合、複数のノードはコンマで区切られます（例：101,102,103）。");


                map.AddDDLSysEnum(FlowAttr.DTSField, (int)DTSField.SameNames, "同期するフィールドの計算方法", true, true,
                 FlowAttr.DTSField, "@0=同じフィールド名@1=設定されたフィールドに従って一致");
                map.SetHelperUrl(FlowAttr.DTSField, "http://ccbpm.mydoc.io/?v=5404&t=17895");

                map.AddTBString(FlowAttr.PTable, null, "フローデータ保存表", true, false, 0, 30, 10);
                map.SetHelperUrl(FlowAttr.PTable, "http://ccbpm.mydoc.io/?v=5404&t=17897");

                #endregion 数据同步方案

                #region 权限控制.
                map.AddBoolean(FlowAttr.PStarter, true, "イニシエーターは見ることができます（必須）", true, false,true);
                map.AddBoolean(FlowAttr.PWorker, true, "参加者は見ることができます（必須）", true, false, true);
                map.AddBoolean(FlowAttr.PCCer, true, "Ccを表示できます（必須）", true, false, true);

                map.AddBoolean(FlowAttr.PMyDept, true, "この部門の人々は見ることができます", true, true, true);
                map.AddBoolean(FlowAttr.PPMyDept, true, "直属の上司から見ることができる（例：私は）", true, true, true);

                map.AddBoolean(FlowAttr.PPDept, true, "上位部門で閲覧可能", true, true, true);
                map.AddBoolean(FlowAttr.PSameDept, true, "ピア部門で見ることができます", true, true, true);

                map.AddBoolean(FlowAttr.PSpecDept, true, "指定部門で閲覧可能", true, true, false);
                map.AddTBString(FlowAttr.PSpecDept + "Ext", null, "部門番号", true, false, 0, 200, 100, false);


                map.AddBoolean(FlowAttr.PSpecSta, true, "指定された位置を見ることができます", true, true, false);
                map.AddTBString(FlowAttr.PSpecSta + "Ext", null, "投稿番号", true, false, 0, 200, 100, false);

                map.AddBoolean(FlowAttr.PSpecGroup, true, "指定した許可グループを表示できます", true, true, false);
                map.AddTBString(FlowAttr.PSpecGroup + "Ext", null, "権利グループ", true, false, 0, 200, 100, false);

                map.AddBoolean(FlowAttr.PSpecEmp, true, "指定された担当者が見ることができます", true, true, false);
                map.AddTBString(FlowAttr.PSpecEmp + "Ext", null, "指定職員数", true, false, 0, 200, 100, false);

                #endregion 权限控制.


                //查询条件.
                map.AddSearchAttr(FlowAttr.FK_FlowSort);
             //   map.AddSearchAttr(FlowAttr.TimelineRole);


                //map.AddRefMethod(rm);
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "試運転"; // "设计检查报告";
                //rm.ToolTip = "检查流程设计的问题。";
                rm.Icon = "../../WF/Img/EntityFunc/Flow/Run.png";
                rm.ClassMethodName = this.ToString() + ".DoRunIt";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "検査報告"; // "设计検査報告";
                rm.Icon = "../../WF/Img/EntityFunc/Flow/CheckRpt.png";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
               // rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
 

                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.Title = "すべてのフローデータを削除"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.Warning = "フローデータを削除してもよろしいですか？\t\nフローデータが削除された後は復元できません。削除されたコンテンツに注意してください";// "您确定要执行删除流程数据吗？";
                rm.ClassMethodName = this.ToString() + ".DoDelData";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.Title = "ジョブIDに従って単一のフローを削除します"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.ClassMethodName = this.ToString() + ".DoDelDataOne";
                rm.HisAttrs.AddTBInt("WorkID", 0, "ジョブIDを入力してください", true, false);
                rm.HisAttrs.AddTBString("beizhu", null, "メモを削除", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Title = "レポートデータを再生成する"; // "删除数据";
                rm.Warning = "実行してもよろしいですか？注：このメソッドはリソースを消費します";// "您确定要执行删除流程数据吗？";
                rm.ClassMethodName = this.ToString() + ".DoReloadRptData";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "自動データソースを設定する";
                rm.Icon = "../../WF/Img/EntityFunc/Flow/Run.png";
                rm.ClassMethodName = this.ToString() + ".DoSetStartFlowDataSources()";
                //设置相关字段.
                // rm.RefAttrKey = FlowAttr.RunObj;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "計時タスクを手動で開始する";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Warning = "実行してもよろしいですか？注：データが大量のデータの場合、Webでの実行時間が制限されているため、実行は失敗します";// "您确定要执行删除流程数据吗？";
                rm.ClassMethodName = this.ToString() + ".DoAutoStartIt()";
                //设置相关字段.
                rm.RefAttrKey = FlowAttr.FlowRunWay;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "フロー監視";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoDataManger()";
                map.AddRefMethod(rm);

             

                rm = new RefMethod();
                rm.Title = "FlowEmpsフィールドを再生成します";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerFlowEmps()";
                rm.RefAttrLinkLabel = "wf_generworkflowおよびNDxxxRptフィールドを含むempsフィールドを補足的に修正";
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                rm.Warning = "を再生成してもよろしいですか？";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "フローのタイトルを再生成";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerTitle()";
                //设置相关字段.
                //rm.RefAttrKey = FlowAttr.TitleRole;
                rm.RefAttrLinkLabel = "フローのタイトルを再生成";
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                rm.Warning = "新しいルールに従ってタイトルを再生成してもよろしいですか？";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "ロールバックフロー";
                rm.Icon = "../../WF/Img/Btn/Back.png";
                rm.ClassMethodName = this.ToString() + ".DoRebackFlowData()";
                // rm.Warning = "您确定要回滚它吗？";
                rm.HisAttrs.AddTBInt("workid", 0, "ロールするWorkIDを入力してください", true, false);
                rm.HisAttrs.AddTBInt("nodeid", 0, "ロールバックするノードID", true, false);
                rm.HisAttrs.AddTBString("note", null, "ロールバックの理由", true, false, 0, 600, 200);
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "イベントとメッセージの処理"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                //rm = new RefMethod();
                //rm.Title = "独立表单树";
                //rm.Icon = "../../WF/Img/Btn/DTS.gif";
                //rm.ClassMethodName = this.ToString() + ".DoFlowFormTree()";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "バッチ開始ルール";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoBatchStartFields()";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "フロー軌道フォーム";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoBindFlowSheet()";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "データソース管理（新しいデータソースを追加した後に閉じて再度開く必要がある場合）"; // "抄送规则";
                rm.ClassMethodName = this.ToString() + ".DoDBSrc";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = FlowAttr.DTSDBSrc;
                rm.RefAttrLinkLabel = "データソース管理";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "ビジネステーブルのフィールド同期構成"; // "抄送规则";
                rm.ClassMethodName = this.ToString() + ".DoBTable";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = FlowAttr.DTSField;
                rm.RefAttrLinkLabel = "ビジネステーブルのフィールド同期構成";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                

                rm = new RefMethod();
                rm.Title = "実行フローデータテーブルとビジネステーブルデータの手動同期"; // "抄送规则";
                rm.ClassMethodName = this.ToString() + ".DoBTableDTS";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Warning = "実行してもよろしいですか？実行すると、ビジネステーブルデータでエラーが発生する可能性があります";
                //设置相关字段.
                rm.RefAttrKey = FlowAttr.DTSSpecNodes;
                rm.RefAttrLinkLabel = "ビジネステーブルのフィールド同期構成";
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                //  map.AddRefMethod(rm);
               

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region  公共方法
        /// <summary>
        /// 事件
        /// </summary>
        /// <returns></returns>
        public string DoAction()
        {
            return "../../Admin/AttrNode/Action.htm?NodeID=0&FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        public string DoDBSrc()
        {
            return "../../Comm/Sys/SFDBSrcNewGuide.htm";
        }
        public string DoBTable()
        {
            return "../../Admin/AttrFlow/DTSBTable.aspx?s=d34&ShowType=FlowFrms&FK_Node=" + int.Parse(this.No) + "01&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime;
        }
    
        public string DoBindFlowSheet()
        {
            return "../../Admin/Sln/BindFrms.htm?s=d34&ShowType=FlowFrms&FK_Node=0&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 批量发起字段
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            return "../../Admin/AttrFlow/BatchStartFields.htm?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 执行流程数据表与业务表数据手工同步
        /// </summary>
        /// <returns></returns>
        public string DoBTableDTS()
        {
            Flow fl = new Flow(this.No);
            return fl.DoBTableDTS();

        }
        /// <summary>
        /// 恢复已完成的流程数据到指定的节点，如果节点为0就恢复到最后一个完成的节点上去.
        /// </summary>
        /// <param name="workid">要恢复的workid</param>
        /// <param name="backToNodeID">恢复到的节点编号，如果是0，标示回复到流程最后一个节点上去.</param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string DoRebackFlowData(Int64 workid, int backToNodeID, string note)
        {
            if (note.Length <= 2)
                return "完了したフローを復元する理由を記入してください";

            Flow fl = new Flow(this.No);
            GERpt rpt = new GERpt("ND" + int.Parse(this.No) + "Rpt");
            rpt.OID = workid;
            int i = rpt.RetrieveFromDBSources();
            if (i == 0)
                throw new Exception("@エラー、失われたフローデータ");

            if (backToNodeID == 0)
                backToNodeID = rpt.FlowEndNode;

            Emp empStarter = new Emp(rpt.FlowStarter);

            // 最后一个节点.
            Node endN = new Node(backToNodeID);
            GenerWorkFlow gwf = null;
            bool isHaveGener = false;
            try
            {
                #region 创建流程引擎主表数据.
                gwf = new GenerWorkFlow();
                gwf.WorkID = workid;
                if (gwf.RetrieveFromDBSources() == 1)
                {
                    isHaveGener = true;
                    //判断状态
                    if (gwf.WFState != WFState.Complete)
                        throw new Exception("@現在のジョブIDは:" + workid + "フローは終了しておらず、この方法では回復できません");
                }

                gwf.FK_Flow = this.No;
                gwf.FlowName = this.Name;
                gwf.WorkID = workid;
                gwf.PWorkID = rpt.PWorkID;
                gwf.PFlowNo = rpt.PFlowNo;
                gwf.PNodeID = rpt.PNodeID;
                gwf.PEmp = rpt.PEmp;


                gwf.FK_Node = backToNodeID;
                gwf.NodeName = endN.Name;

                gwf.Starter = rpt.FlowStarter;
                gwf.StarterName = empStarter.Name;
                gwf.FK_FlowSort = fl.FK_FlowSort;
                gwf.SysType = fl.SysType;
                gwf.Title = rpt.Title;
                gwf.WFState = WFState.ReturnSta; /*设置为退回的状态*/
                gwf.FK_Dept = rpt.FK_Dept;

                Dept dept = new Dept(empStarter.FK_Dept);

                gwf.DeptName = dept.Name;
                gwf.PRI = 1;

                DateTime dttime = DateTime.Now;
                dttime = dttime.AddDays(3);

                gwf.SDTOfNode = dttime.ToString("yyyy-MM-dd HH:mm:ss");
                gwf.SDTOfFlow = dttime.ToString("yyyy-MM-dd HH:mm:ss");
              

                #endregion 创建流程引擎主表数据

                string ndTrack = "ND" + int.Parse(this.No) + "Track";
                string actionType = (int)ActionType.Forward + "," + (int)ActionType.FlowOver + "," + (int)ActionType.ForwardFL + "," + (int)ActionType.ForwardHL;
                string sql = "SELECT  * FROM " + ndTrack + " WHERE   ActionType IN (" + actionType + ")  and WorkID=" + workid + " ORDER BY RDT DESC, NDFrom ";
                System.Data.DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("@ジョブIDは:" + workid + "データは存在しません");

                string starter = "";
                bool isMeetSpecNode = false;
                GenerWorkerList currWl = new GenerWorkerList();
                string todoEmps="";
                int num = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    int ndFrom = int.Parse(dr["NDFrom"].ToString());
                    Node nd = new Node(ndFrom);

                    string ndFromT = dr["NDFromT"].ToString();
                    string EmpFrom = dr[TrackAttr.EmpFrom].ToString();
                    string EmpFromT = dr[TrackAttr.EmpFromT].ToString();

                    // 增加上 工作人员的信息.
                    GenerWorkerList gwl = new GenerWorkerList();
                    gwl.WorkID = workid;
                    gwl.FK_Flow = this.No;

                    gwl.FK_Node = ndFrom;
                    gwl.FK_NodeText = ndFromT;
                    gwl.IsPass = true;
                    if (gwl.FK_Node == backToNodeID)
                    {
                        gwl.IsPass = false;
                        currWl = gwl;
                    }

                    gwl.FK_Emp = EmpFrom;
                    gwl.FK_EmpText = EmpFromT;
                    if (gwl.IsExits)
                        continue; /*有可能是反复退回的情况.*/

                    Emp emp = new Emp(gwl.FK_Emp);
                    gwl.FK_Dept = emp.FK_Dept;
                    gwl.FK_DeptT = emp.FK_DeptText;


                    todoEmps += emp.No + "," + emp.Name + ";";
                    num++; 


                    gwl.SDT = dr["RDT"].ToString();
                    gwl.DTOfWarning = gwf.SDTOfNode;
                    //gwl.WarningHour = nd.WarningHour;
                    gwl.IsEnable = true;
                    gwl.WhoExeIt = nd.WhoExeIt;
                    gwl.Insert();
                }

                //设置当前处理人员.
                gwf.SetValByKey(GenerWorkFlowAttr.TodoEmps,  todoEmps);
                gwf.TodoEmpsNum = num;

                if (isHaveGener)
                    gwf.Update();
                else
                    gwf.Insert(); /*插入流程引擎数据.*/


                #region 加入退回信息, 让接受人能够看到退回原因.
                ReturnWork rw = new ReturnWork();
                rw.WorkID = workid;
                rw.ReturnNode = backToNodeID;
                rw.ReturnNodeName = endN.Name;
                rw.Returner = WebUser.No;
                rw.ReturnerName = WebUser.Name;

                rw.ReturnToNode = currWl.FK_Node;
                rw.ReturnToEmp = currWl.FK_Emp;
                rw.BeiZhu = note;
                rw.RDT = DataType.CurrentDataTime;
                rw.IsBackTracking = false;
                rw.MyPK = BP.DA.DBAccess.GenerGUID();
                #endregion   加入退回信息, 让接受人能够看到退回原因.

                //更新流程表的状态.
                rpt.FlowEnder = currWl.FK_Emp;
                rpt.WFState = WFState.ReturnSta; /*设置为退回的状态*/
                rpt.FlowEndNode = currWl.FK_Node;
                rpt.Update();

                // 向接受人发送一条消息.
                BP.WF.Dev2Interface.Port_SendMsg(currWl.FK_Emp, "作業履歴書:" + gwf.Title, "仕事は:" + WebUser.No + " 回復。" + note, "ReBack" + workid, BP.WF.SMSMsgType.SendSuccess, this.No, int.Parse(this.No + "01"), workid, 0);

                //写入该日志.
                WorkNode wn = new WorkNode(workid, currWl.FK_Node);
                wn.AddToTrack(ActionType.RebackOverFlow, currWl.FK_Emp, currWl.FK_EmpText, currWl.FK_Node, currWl.FK_NodeText, note);

                return "@正常に復元され、現在のフローがに復元されました(" + currWl.FK_NodeText + "). @人工的な現在のジョブ処理(" + currWl.FK_Emp + " , " + currWl.FK_EmpText + ")  @仕事をするように彼に通知してください。";
            }
            catch (Exception ex)
            {
                //此表的记录删除已取消
                //gwf.Delete();
                GenerWorkerList wl = new GenerWorkerList();
                wl.Delete(GenerWorkerListAttr.WorkID, workid);

                string sqls = "";
                sqls += "@UPDATE " + fl.PTable + " SET WFState=" + (int)WFState.Complete + " WHERE OID=" + workid;
                DBAccess.RunSQLs(sqls);
                return "<font color=red>ロールオーバー中にエラーが発生しました</font><hr>" + ex.Message;
            }
        }
         /// <summary>
        /// 重新产生标题，根据新的规则.
        /// </summary>
        public string DoGenerFlowEmps()
        {
            if (WebUser.No != "admin")
                return "管理者以外のユーザーは実行できません。";

            Flow fl = new Flow(this.No);

            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.FK_Flow, this.No);

            foreach (GenerWorkFlow gwf in gwfs)
            {
                string emps = "";
                string sql = "SELECT EmpFrom FROM ND" + int.Parse(this.No) + "Track  WHERE WorkID=" + gwf.WorkID;

                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    if (emps.Contains("," + dr[0].ToString()+","))
                        continue;
                }

                sql = "UPDATE " + fl.PTable + " SET FlowEmps='" + emps + "' WHERE OID=" + gwf.WorkID;
                DBAccess.RunSQL(sql);

                sql = "UPDATE WF_GenerWorkFlow SET Emps='" + emps + "' WHERE WorkID=" + gwf.WorkID;
                DBAccess.RunSQL(sql);
            }

            Node nd = fl.HisStartNode;
            Works wks = nd.HisWorks;
            wks.RetrieveAllFromDBSource(WorkAttr.Rec);
            string table = nd.HisWork.EnMap.PhysicsTable;
            string tableRpt = "ND" + int.Parse(this.No) + "Rpt";
            Sys.MapData md = new Sys.MapData(tableRpt);
            foreach (Work wk in wks)
            {
                if (wk.Rec != WebUser.No)
                {
                    BP.Web.WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(wk.Rec);
                        BP.Web.WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string sql = "";
                string title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                ps.SQL = "UPDATE " + table + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE " + md.PTable + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

               
            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);

            return "すべてが正常に生成され、データに影響します(" + wks.Count + ")件";
        }
        
        /// <summary>
        /// 重新产生标题，根据新的规则.
        /// </summary>
        public string DoGenerTitle()
        {
            if (WebUser.No != "admin")
                return "管理者以外のユーザーは実行できません。";
            Flow fl = new Flow(this.No);
            Node nd = fl.HisStartNode;
            Works wks = nd.HisWorks;
            wks.RetrieveAllFromDBSource(WorkAttr.Rec);
            string table = nd.HisWork.EnMap.PhysicsTable;
            string tableRpt = "ND" + int.Parse(this.No) + "Rpt";
            Sys.MapData md = new Sys.MapData(tableRpt);
            foreach (Work wk in wks)
            {

                if (wk.Rec != WebUser.No)
                {
                    BP.Web.WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(wk.Rec);
                        BP.Web.WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string sql = "";
                string title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                ps.SQL = "UPDATE " + table + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE " + md.PTable + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

          
            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);

            return "すべてが正常に生成され、データに影響します(" + wks.Count + ")件";
        }
        /// <summary>
        /// 流程监控
        /// </summary>
        /// <returns></returns>
        public string DoDataManger()
        {
            //PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Rpt/OneFlow.htm?FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=", 700, 500);
            return "../../Comm/Search.htm?s=d34&EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=";
        }
        /// <summary>
        /// 绑定独立表单
        /// </summary>
        /// <returns></returns>
        public string DoFlowFormTree()
        {
            return "../../Admin/FlowFormTree.aspx?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 定义报表
        /// </summary>
        /// <returns></returns>
        public string DoAutoStartIt()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoAutoStartIt();
        }
        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public string DoDelDataOne(int workid, string note)
        {
            try
            {
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.No, workid, true);
                return "ワークIDが正常に削除されました workid=" + workid + " 理由:" + note;
            }
            catch(Exception ex)
            {
                return "削除できませんでした"+ex.Message;
            }
        }
        /// <summary>
        /// 设置发起数据源
        /// </summary>
        /// <returns></returns>
        public string DoSetStartFlowDataSources()
        {
            string flowID = int.Parse(this.No).ToString() + "01";
            return "../../Admin/FoolFormDesigner/MapExt.aspx?s=d34&FK_MapData=ND" + flowID + "&ExtType=StartFlow&RefNo=";
        }
        public string DoCCNode()
        {
            return "../../Admin/CCNode.aspx?FK_Flow=" + this.No;
        }
        /// <summary>
        /// 执行运行
        /// </summary>
        /// <returns></returns>
        public string DoRunIt()
        {
            return "../../Admin/TestFlow.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 执行检查
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            //Flow fl = new Flow();
            //fl.No = this.No;
            //fl.RetrieveFromDBSources();

            return "/WF/Admin/AttrFlow/CheckFlow.htm?FK_Flow=" + this.No;

            //return fl.DoCheck();
        }
        /// <summary>
        /// 执行重新装载数据
        /// </summary>
        /// <returns></returns>
        public string DoReloadRptData()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoReloadRptData();
        }
        /// <summary>
        /// 删除数据.
        /// </summary>
        /// <returns></returns>
        public string DoDelData()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoDelData();
        }
        protected override bool beforeUpdate()
        {
            //更新流程版本
            Flow.UpdateVer(this.No);

            #region 检查数据完整性 - 同步业务表数据。
            // 检查业务是否存在.
            Flow fl = new Flow(this.No);
            fl.Row = this.Row;

            if (fl.DTSWay != FlowDTSWay.None)
            {
                /*检查业务表填写的是否正确.*/
                string sql = "select count(*) as Num from  " + fl.DTSBTable + " where 1=2";
                try
                {
                    DBAccess.RunSQLReturnValInt(sql, 0);
                }
                catch (Exception)
                {
                    throw new Exception("@ビジネステーブルの構成が無効です。ビジネスデータテーブルを構成します[" + fl.DTSBTable + "]データに存在しない場合は、スペルエラーを確認してください。クロスデータベースの場合は、次のようなユーザー名を追加してください：for sqlserver：HR.dbo.Emps、For oracle：HR.Emps");
                }

                sql = "select " + fl.DTSBTablePK + " from " + fl.DTSBTable + " where 1=2";
                try
                {
                    DBAccess.RunSQLReturnValInt(sql, 0);
                }
                catch (Exception)
                {
                    throw new Exception("@ビジネステーブルの構成が無効です。ビジネスデータテーブルを構成します[" + fl.DTSBTablePK + "]主キーが存在しません。");
                }


                //检查节点配置是否符合要求.
                if (fl.DTSTime == FlowDTSTime.SpecNodeSend)
                {
                    // 检查节点ID，是否符合格式.
                    string nodes = fl.DTSSpecNodes;
                    nodes = nodes.Replace("，", ",");
                    this.SetValByKey(FlowAttr.DTSSpecNodes, nodes);

                    if (DataType.IsNullOrEmpty(nodes) == true)
                        throw new Exception("@業務データ同期データ構成エラー、指定したとおりにノード構成を設定しましたが、ノードを設定していません。ノードの設定形式は以下のとおりです：101,102,103");

                    string[] strs = nodes.Split(',');
                    foreach (string str in strs)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;

                        if (BP.DA.DataType.IsNumStr(str) == false)
                            throw new Exception("@ビジネスデータ同期データ構成エラー、指定したとおりにノード構成を設定しましたが、ノードの形式が間違っています" + nodes + "]。正しい形式は次のとおりです：101、102、103");

                        Node nd = new Node();
                        nd.NodeID = int.Parse(str);
                        if (nd.IsExits == false)
                            throw new Exception("@ビジネスデータ同期データ構成エラー、設定したノード形式が正しくありません、ノード" + str + "]有効なノードではありません。");

                        nd.RetrieveFromDBSources();
                        if (nd.FK_Flow != this.No)
                            throw new Exception("@ビジネスデータ同期データ構成エラー、設定したノード" + str + "]このフローではもうありません。");
                    }
                }

                //检查流程数据存储表是否正确
                if (!DataType.IsNullOrEmpty(fl.PTable))
                {
                    /*检查流程数据存储表填写的是否正确.*/
                    sql = "select count(*) as Num from  " + fl.PTable + " where 1=2";
                    try
                    {
                        DBAccess.RunSQLReturnValInt(sql, 0);
                    }
                    catch (Exception)
                    {
                        throw new Exception("@フローデータストレージテーブルの構成が無効です。フローデータストレージテーブルを構成します[" + fl.PTable + "]データに存在しない場合は、スペルエラーを確認してください。クロスデータベースの場合は、次のようなユーザー名を追加してください：for sqlserver：HR.dbo.Emps、For oracle：HR.Emps");
                    }
                }
            }
            #endregion 检查数据完整性. - 同步业务表数据。

            return base.beforeUpdate();
        }
        protected override void afterInsertUpdateAction()
        {
            //同步流程数据表.
            string ndxxRpt = "ND" + int.Parse(this.No) + "Rpt";
            Flow fl = new Flow(this.No);
            if (fl.PTable != "ND" + int.Parse(this.No) + "Rpt")
            {
                BP.Sys.MapData md = new Sys.MapData(ndxxRpt);
                if (md.PTable != fl.PTable)
                    md.Update();
            }
            base.afterInsertUpdateAction();
        }
        #endregion
    }
    /// <summary>
    /// 流程集合
    /// </summary>
    public class FlowSheets : EntitiesNoName
    {
        #region 查询
        /// <summary>
        /// 查询出来全部的在生存期间内的流程
        /// </summary>
        /// <param name="FlowSort">流程类别</param>
        /// <param name="IsCountInLifeCycle">是不是计算在生存期间内 true 查询出来全部的 </param>
        public void Retrieve(string FlowSort)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(BP.WF.Template.FlowAttr.FK_FlowSort, FlowSort);
            qo.addOrderBy(BP.WF.Template.FlowAttr.No);
            qo.DoQuery();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 工作流程
        /// </summary>
        public FlowSheets() { }
        /// <summary>
        /// 工作流程
        /// </summary>
        /// <param name="fk_sort"></param>
        public FlowSheets(string fk_sort)
        {
            this.Retrieve(BP.WF.Template.FlowAttr.FK_FlowSort, fk_sort);
        }
        #endregion

        #region 得到实体
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowSheet();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowSheet> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowSheet>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowSheet> Tolist()
        {
            System.Collections.Generic.List<FlowSheet> list = new System.Collections.Generic.List<FlowSheet>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowSheet)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

