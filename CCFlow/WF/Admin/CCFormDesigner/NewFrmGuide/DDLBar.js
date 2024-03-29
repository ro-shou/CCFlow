﻿
function InitBar(optionKey) {

    /*
    FrmType_CH_0	傻瓜表单	FrmType	0	CH
    FrmType_CH_1	自由表单	FrmType	1	CH
    FrmType_CH_11	累加表单	FrmType	11	CH
    FrmType_CH_3	嵌入式表单	FrmType	3	CH
    FrmType_CH_4	Word表单	FrmType	4	CH
    FrmType_CH_5	在线编辑模式Excel表单	FrmType	5	CH
    FrmType_CH_6	VSTO模式Excel表单	FrmType	6	CH
    FrmType_CH_7	实体类组件	FrmType	7	CH
        */

    var html = "作成するフォームのタイプを選択します:";
    html += "<select id='changBar' onchange='changeOption()'>";

    var frmWorkModel = GetQueryString("FormWorkMode");

    if (frmWorkModel == 1) {
        html += "<option value=null  disabled='disabled'>+ドキュメントモード</option>";
        html += "<option value=" + FormSlnType.FoolForm + ">&nbsp;&nbsp;フールモード（デフォルト）</option>";
        html += "<option value=" + FormSlnType.FreeForm + ">&nbsp;&nbsp;自由形式</option>";
        //html += "<option value=" + FormSlnType.FoolTruck + " >&nbsp;&nbsp;内置累加模式表单</option>";
        //html += "<option value=" + FormSlnType.WebOffice + "  >&nbsp;&nbsp;公文表单(weboffice)</option>";

        html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存する' />";
        html += "<input  id='Btn_SaveAndClose' type=button onclick='SaveAndClose()' value='保存して閉じます' />";

        html += "<input  id='Btn_Help' type=button onclick='Help()' value='ビデオヘルプ' />";
        html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='オンラインヘルプ' />";


        document.getElementById("bar").innerHTML = html;
        $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
        return;
    }

    html += "<option value=null  disabled='disabled'>+ビルトインフォーム</option>";
    html += "<option value=" + FormSlnType.FoolForm + ">&nbsp;&nbsp;組み込みのばかフォーム（デフォルト）</option>";
    html += "<option value=" + FormSlnType.FreeForm + ">&nbsp;&nbsp;内蔵のフリーフォーム</option>";
  //  html += "<option value=" + FormSlnType.FoolTruck + " >&nbsp;&nbsp;内置累加模式表单</option>";
   // html += "<option value=" + FormSlnType.WebOffice + "  >&nbsp;&nbsp;公文表单(weboffice)</option>";

    html += "<option value=null  disabled='disabled'>+カスタムフォーム</option>";
    html += "<option value=" + FormSlnType.SelfForm + " >&nbsp;&nbsp;埋め込みフォーム</option>";
  //  html += "<option value=" + FormSlnType.SDKForm + " >&nbsp;&nbsp;SDK表单(我自定义的表单)</option>";

    html += "<option value=null  disabled='disabled'>+Officeフォーム</option>";
    html += "<option value=" + FormSlnType.RefOneFrmTree + " >&nbsp;&nbsp;VSTOモードExcelシート</option>";
    html += "<option value=" + FormSlnType.SheetTree + " >&nbsp;&nbsp;Wordフォーム</option>";
    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='作成' />";
    html += "<input  id='Btn_SaveAndClose' type=button onclick='SaveAndClose()' value='作成して閉じる' />";

    html += "<input  id='Btn_Help' type=button onclick='Help()' value='ビデオヘルプ' />";
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='オンラインヘルプ' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");

}

function changeOption() {
    var frmSort = GetQueryString("FK_FrmSort");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var roleName = "";
    switch (parseInt(optionKey)) {
        case FormSlnType.FoolForm:
            url = "0.FoolForm.htm";
            break;
        case FormSlnType.FreeForm:
            url = "1.FreeForm.htm";
            break;
        case FormSlnType.SelfForm:
            url = "2.SelfForm.htm";
            break;
        case FormSlnType.SDKForm:
            url = "3.SDKForm.htm";
            break;
        case FormSlnType.SLForm:
            url = "4.SLForm.htm";
            break;
        case FormSlnType.SheetTree:
            url = "5.SheetTree.htm";
            break;
        case FormSlnType.SheetAutoTree:
            url = "6.SheetAutoTree.htm";
            break;
        case FormSlnType.WebOffice:
            url = "7.WebOffice.htm";
            break;
        case FormSlnType.ExcelForm:
            url = "8.ExcelForm.htm";
            break;
        case FormSlnType.WordForm:
            url = "9.WordForm.htm";
            break;
        case FormSlnType.FoolTruck:
            url = "10.FoolTruck.htm";
            break;
        case FormSlnType.RefOneFrmTree:
            url = "11.RefOneFrmTree.htm";
            break;
        case FormSlnType.DisableIt:
            url = "100.DisableIt.htm";
            break;
        default:
            url = "0.FoolForm.htm";
            break;
    }

    window.location.href = url + "?FK_FrmSort=" + frmSort + "&FormWorkMode=" + GetQueryString("FormWorkMode");
}

function SaveAndClose() {

    Save();
    window.close();
}

//打开窗体.
function OpenEasyUiDialogExt(url, title, w, h, isReload) {

    OpenEasyUiDialog(url, "eudlgframe", title, w, h, "icon-property", true, null, null, null, function () {
        if (isReload == true) {
            window.location.href = window.location.href;
        }
    });

}
