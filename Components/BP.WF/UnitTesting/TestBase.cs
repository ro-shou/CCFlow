using System;
using System.Threading;
using System.Collections;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web.Controls;
using BP.Web;

namespace BP.UnitTesting
{
    public enum EditState
    {
        /// <summary>
        /// �Ѿ�́E�
        /// </summary>
        Passed,
        /// <summary>
        /// �༭��
        /// </summary>
        Editing,
        /// <summary>
        /// δ́E�
        /// </summary>
        UnOK
    }
	/// <summary>
	/// ���Ի���E
	/// </summary>
    abstract public class TestBase
    {
        public EditState EditState = EditState.Editing;
        /// <summary>
        /// ִ�в�����Ϣ
        /// </summary>
        public int TestStep = 0;
        public string Note = "";
        /// <summary>
        /// ���Ӳ�������.
        /// </summary>
        /// <param name="note">�������ݵ�ρE���ʁE</param>
        public void AddNote(string note)
        {
            TestStep++;
            if (Note == "")
            {
                Note += "\t\n ����:" + TestStep + "������";
                Note += "\t\n" + note;
            }
            else
            {
                Note += "\t\n����ͨ��.";
                Note += "\t\n ����:" + TestStep + "������";
                Note += "\t\n" + note;
            }
        }
        public string sql = "";
        public DataTable dt = null;
        /// <summary>
        /// ��������д
        /// </summary>
        public virtual void Do()
        {
        }

        #region ��������.
        /// <summary>
        /// ��E�E
        /// </summary>
        public string Title = "δÁE��ĵ�Ԫ����";
        public string DescIt = "��ʁE";
        /// <summary>
        /// ������Ϣ
        /// </summary>
        public string ErrInfo = "";
        #endregion
        /// <summary>
        /// ���Ի���E
        /// </summary>
        public TestBase() { }
    }

}
