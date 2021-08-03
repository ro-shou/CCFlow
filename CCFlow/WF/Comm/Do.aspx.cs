using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.Sys;
using BP.En;
using BP.Web;
using BP.DA;
using BP;
namespace CCFlow.Web.Comm
{
	/// <summary>
	/// Do ��ժҪ˵����
	/// </summary>
	public partial class Do : System.Web.UI.Page
    {
        #region ����.
        /// <summary>
        /// �رմ���
        /// </summary>
        protected void WinClose()
        {
            this.Response.Write("<script language='JavaScript'> window.close();</script>");
        }
        public string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public string DoWhat
        {
            get
            {
                return this.Request.QueryString["DoWhat"];
            }
        }
        public string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }
        public string EnName
        {
            get
            {
                return this.Request.QueryString["EnName"];
            }
        }
        public string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        
        #endregion ����.

        protected void Page_Load(object sender, System.EventArgs e)
		{
			switch (this.DoType)
			{
                case "SearchExp":
                    this.SearchExp();
                    break;
				 
				case "DownFile":
					Entity enF = BP.En.ClassFactory.GetEn(this.EnName);
					enF.PKVal = this.Request.QueryString["PK"];
					enF.Retrieve();
					string pPath = enF.GetValStringByKey("MyFilePath") + Path.DirectorySeparatorChar + enF.PKVal + "." + enF.GetValStringByKey("MyFileExt");

					//�ж��ļ��Ƿ����
					if (System.IO.File.Exists(pPath)==false)
					{
                        pPath = enF.EnMap.FJSavePath + Path.DirectorySeparatorChar + enF.PKVal + "." + enF.GetValStringByKey("MyFileExt");
                        if (System.IO.File.Exists(pPath) == false)
                        {
                            Response.Write("<script>alert('�ļ������ڡ�');</script>");
                            this.WinClose();
                            return;
                        }
					}

                    BP.WF.HttpHandler.HttpHandlerGlo.DownloadFile(pPath,
						enF.GetValStringByKey("MyFileName"));
					this.WinClose();
					return;
				default:
					break;
			}

			this.WinClose();
		}

        public void SearchExp()
        {
            BP.WF.HttpHandler.WF_Comm comm = new BP.WF.HttpHandler.WF_Comm();
            DataSet ds = comm.Search_Search();

            DataTable dt = ds.Tables["DT"];

            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            Map map = en.EnMapInTime;
            foreach (Attr  item in map.Attrs)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    try
                    {
                        if (item.Key == dc.ColumnName)
                            dc.ColumnName = item.Desc;
                    }
                    catch
                    {

                    }
                }
            }

            string name = map.EnDesc + ".xls";

            string filename = Request.PhysicalApplicationPath + Path.DirectorySeparatorChar + "Temp" + Path.DirectorySeparatorChar +DBAccess.GenerGUID() + ".xls";
            CCFlow.WF.Comm.Utilities.NpoiFuncs.DataTableToExcel(dt, filename, name,
                                                              BP.Web.WebUser.Name, true, true, true);
        }

		#region Web ������������ɵĴ���
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: �õ����� ASP.NET Web ���������������ġ�
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}