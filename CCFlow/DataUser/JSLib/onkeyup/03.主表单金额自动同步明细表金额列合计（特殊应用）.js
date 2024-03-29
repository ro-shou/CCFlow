﻿function ParentHeJi(s) {
    if (!dtlZjCtrls) {
        //dtlZjCtrl = document.getElementById('Pub1_TB_' + Dtl_ZongJi_Ctrl_ID);
        dtlZjCtrls = $("input[id*='_TB_" + Dtl_ZongJi_Ctrl_ID + "_']");
    }

    if(!frmZjCtrl)
        frmZjCtrl = GetCtrl(window.parent.document, Form_ZongJi_Ctrl_ID);

    if (dtlZjCtrls.length == 0) {
        alert('未確認TB_' + Dtl_ZongJi_Ctrl_ID + '列,管理者に連絡して、フローノードに対応するJSファイルを修正してくださいDataUser/JSLibData/' + getArgsFromHref('EnsName') + '.js中にDtl_ZongJi_Ctrl_ID変数は明細書の「合計」フィールド！');
        return false;
    }

    if (!frmZjCtrl) {
        alert('未確認TB_' + Form_ZongJi_Ctrl_ID + 'テキストボックス,管理者に連絡して、フローノードに対応するJSファイルを修正してくださいDataUser/JSLibData/' + getArgsFromHref('EnsName') + '.js中にForm_ZongJi_Ctrl_IDフォームの「合計」フィールド！');
        return false;
    }

    var zj = 0;
    var val;
    $.each(dtlZjCtrls, function () {
        val = replaceAll(this.value, ',', '');
        zj += (isNaN(val) ? 0 : parseFloat(val));
    });

    frmZjCtrl.value = zj;

    //触发frmZjCtrl onkeyup事件
    if (document.createEvent) {
        frmZjCtrl.onkeyup();//Firefox39.0,Chorme43.0,QQBrowser8.2
    }
    else if (document.createEventObject) {
        frmZjCtrl.fireEvent('onkeyup'); //IE8
    }
    else {
        alert('不支持onkeyup');
    }

    return true;
}

var Dtl_ZongJi_Ctrl_ID = "ZJ";  //明细表总计字段的名称
var Form_ZongJi_Ctrl_ID = "JinE";   //主表单总计字段的名称
var frmZjCtrl;
var dtlZjCtrls;

function GetCtrl(doc, ctrlFieldId) {
    var ctrls = doc.getElementsByTagName("input");
    var idx;
    var ctrlid = 'TB_' + ctrlFieldId;
    var ctrl;

    for (var i = 0; i < ctrls.length; i++) {
        idx = ctrls[i].id.indexOf(ctrlid);
        if (idx != -1 && (idx + ctrlid.length) == ctrls[i].id.length) {
            ctrl = ctrls[i];
            break;
        }
    }

    return ctrl;
}

function getArgsFromHref(sArgName) {
    var sHref = window.location.href;
    var args = sHref.split("?");
    var retval = "";

    if (args[0] == sHref) /*参数为空*/
    {
        return retval; /*无需做任何处理*/
    }

    var str = args[1];
    args = str.split("&");

    for (var i = 0; i < args.length; i++) {
        str = args[i];
        var arg = str.split("=");
        if (arg.length <= 1) continue;
        if (arg[0] == sArgName) retval = arg[1];
    }

    return retval;
}