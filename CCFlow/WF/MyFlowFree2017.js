﻿var flowData = null;
var IsCC = false;
var isSigantureChecked = false;
function GenerFreeFrm(wn) {

    flowData = wn;

    $('#CCForm').html('');

    //循环FrmRB
    for (var i in flowData.Sys_FrmRB) {
        var frmLab = flowData.Sys_FrmRB[i];
        var label = figure_Template_RB(frmLab);
        $('#CCForm').append(label);
    }

    //循环MapAttr
    for (var mapAtrrIndex in flowData.Sys_MapAttr) {
        var mapAttr = flowData.Sys_MapAttr[mapAtrrIndex];
        if (mapAttr.UIContralType == 3) {
            if (mapAttr.UIIsEnable == 0)
                $('input[name=RB_' + mapAttr.KeyOfEn + ']').attr("disabled", "disabled");
            continue;
        }
        var eleHtml = figure_MapAttr_Template(mapAttr);
        $('#CCForm').append(eleHtml);
    }

    //循环FrmLab
    for (var i in flowData.Sys_FrmLab) {
        var frmLab = flowData.Sys_FrmLab[i];
        var label = figure_Template_Label(frmLab);
        $('#CCForm').append(label);
    }
    

    //循环FrmBtn
    for (var i in flowData.Sys_FrmBtn) {
        var frmBtn = flowData.Sys_FrmBtn[i];
        var btn = figure_Template_Btn(frmBtn);
        $('#CCForm').append(btn);
    }

    //循环Image
    for (var i in flowData.Sys_FrmImg) {
        var frmImg = flowData.Sys_FrmImg[i];
        var createdFigure = figure_Template_Image(frmImg);
        $('#CCForm').append(createdFigure);
    }

    //循环 Link
    for (var i in flowData.Sys_FrmLink) {
        var frmLink = flowData.Sys_FrmLink[i];
        var createdFigure = figure_Template_HyperLink(frmLink);
        $('#CCForm').append(createdFigure);
    }
    //循环 图片附件
    if (flowData.Sys_FrmImgAth.length > 0) {
        var imgSrc = "<input type='hidden' id='imgSrc'/>";
        $('#CCForm').append(imgSrc);
    }
    for (var i in flowData.Sys_FrmImgAth) {
        var frmImgAth = flowData.Sys_FrmImgAth[i];
        var createdFigure = figure_Template_ImageAth(frmImgAth);
        $('#CCForm').append(createdFigure);
    }
    //循环 附件
    for (var i in flowData.Sys_FrmAttachment) {
        var frmAttachment = flowData.Sys_FrmAttachment[i];
        if (frmAttachment.IsVisable == 0)
            continue;
        var createdFigure = figure_Template_Attachment(frmAttachment);
        $('#CCForm').append(createdFigure);
    }

    //循环 从表
    for (var i in flowData.Sys_MapDtl) {
        var frmMapDtl = flowData.Sys_MapDtl[i];
        var createdFigure = figure_Template_Dtl(frmMapDtl);
        $('#CCForm').append(createdFigure);
    }

    //循环线
    for (var i in flowData.Sys_FrmLine) {
        var frmLine = flowData.Sys_FrmLine[i];
        var createdConnector = connector_Template_Line(frmLine);
        $('#CCForm').append(createdConnector);
    }

    //循环Sys_MapFrame
    for (var i in flowData.Sys_MapFrame) {
        var frame = flowData.Sys_MapFrame[i];
        var alertMsgEle = figure_Template_IFrame(frame);
        $('#CCForm').append(alertMsgEle);
    }

    //循环组件 轨迹图 审核组件 子流程 子线程
    var wf_FrmNodeComponent = flowData["WF_FrmNodeComponent"][0];
    $('#CCForm').append(figure_Template_FigureFlowChart(wf_FrmNodeComponent));
    $('#CCForm').append(figure_Template_FigureFrmCheck(wf_FrmNodeComponent));
    $('#CCForm').append(figure_Template_FigureSubFlowDtl(wf_FrmNodeComponent));
    $('#CCForm').append(figure_Template_FigureThreadDtl(wf_FrmNodeComponent));
}

//初始化框架
function figure_Template_IFrame(fram) {


    var eleHtml = $("<DIV id='Fd" + fram.MyPK + "' style='position:absolute; left:" + fram.X + "px; top:" + fram.Y + "px; width:" + fram.W + "px; height:" + fram.H + "px;text-align: left;' >");
    //获取框架的类型 0 自定义URL 1 地图开发 2流程轨迹表 3流程轨迹图
    var urlType = fram.UrlSrcType;
      var url = "";
      if (urlType == 0) {
          url = fram.URL;
          if (url.indexOf('?') == -1)
              url += "?1=2";

          if (url.indexOf("@basePath") == 0)
              url = url.replace("@basePath", basePath);

          //1.处理URL需要的参数
          var pageParams = getQueryString();
          $.each(pageParams, function (i, pageParam) {
              var pageParamArr = pageParam.split('=');
              url = url.replace("@" + pageParamArr[0], pageParamArr[1]);
          });

          var src = url.replace(new RegExp(/(：)/g), ':');
          if (src.indexOf("?") > 0) {
              var params = getQueryStringFromUrl(src);
              if (params != null && params.length > 0) {
                  $.each(params, function (i, param) {
                      if (param.indexOf('@') != -1) {//是需要替换的参数
                          paramArr = param.split('=');
                          if (paramArr.length == 2 && paramArr[1].indexOf('@') == 0) {
                              if (paramArr[1].indexOf('@WebUser.') == 0)
                                  url = url.replace(paramArr[1], flowData.MainTable[0][paramArr[1].substr('@WebUser.'.length)]);
                              else
                                  url = url.replace(paramArr[1], flowData.MainTable[0][paramArr[1].substr(1)]);
                          }
                      }
                  });
              }
          }


          //1.拼接参数
          var paras = this.pageData;
          var strs = "";
          for (var str in paras) {
              if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
                  continue
              else
                  strs += "&" + str + "=" + paras[str];
          }

          //4.追加GenerWorkFlow AtPara中的参数
          var gwf = flowData.WF_GenerWorkFlow[0];
          if (gwf != null) {
              var atPara = gwf.AtPara;
              if (atPara != null && atPara != "") {
                  atPara = atPara.replace(/@/g, '&');
                  url = url + atPara;
              }
          }

          url = url + strs + "&IsReadonly=0";
      }
      if (urlType == 2) //轨迹表
          url = "./WorkOpt/OneWork/Table.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.WorkID + "&FID=" + pageData.FID;
      if (urlType == 3)//轨迹图
          url = "./WorkOpt/OneWork/TimeBase.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.WorkID + "&FID=" + pageData.FID;

    var eleIframe = '<iframe></iframe>';
    eleIframe = $("<iframe ID='Fdg" + fram.MyPK + "' src='" + url +
	                 "' frameborder=0  style='position:absolute;width:" + fram.W + "px; height:" + fram.H +
	                 "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");

    eleHtml.append(eleIframe);

    return eleHtml;
}

function figure_MapAttr_Template(mapAttr) {


    //根据不同的类型控件，生成html.
    var ele = figure_MapAttr_TemplateEle(mapAttr);

    ele += mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '" >*</span>' : "";

    var eleHtml = $('<div>' + ele + '</div>');
    var W = mapAttr.UIWidth;
    if (mapAttr.MyDataType == 6)
        if (W < 120) W = 120;
    if (mapAttr.MyDataType == 7)
        if (W < 160) W = 160;

    if (mapAttr.IsSigan == "4") {
        eleHtml.css('position', 'absolute').css('top', mapAttr.Y).css('left', mapAttr.X);
        eleHtml.css('z-index', '999');
        return eleHtml;
    }
    if (mapAttr.MyDataType != 4) {
        eleHtml.children(0).css('width', W).css('height', mapAttr.UIHeight).css("display", "inline");
    }
    
    eleHtml.css('position', 'absolute').css('top', mapAttr.Y).css('left', mapAttr.X);

    return eleHtml;
}

//升级表单元素 初始化文本框、日期、时间
function figure_MapAttr_TemplateEle(mapAttr) {

    var eleHtml = '';

    /***************** 隐藏的控件 (在 CCForm/FrmEnd.js 处理.) *****************************/
    if (mapAttr.UIVisible == 0) {
        return "";
    }

    /***************** 外键 *****************************/
    if (mapAttr.LGType == 2 && mapAttr.MyDataType == "1" && mapAttr.UIContralType == "1") {

        //判断外键是否为树形结构
        var uiBindKey = mapAttr.UIBindKey;
        if (uiBindKey != null && uiBindKey != undefined && uiBindKey != "") {
            var sfTable = new Entity("BP.Sys.SFTable");
            sfTable.SetPKVal(uiBindKey);
            var count = sfTable.RetrieveFromDBSources();
            if (count!=0 && sfTable.CodeStruct == "1") {
                return "<select  id='DDL_" + mapAttr.KeyOfEn + "' class='easyui-combotree' style='width:" + parseInt(mapAttr.UIWidth) * 2 + "px;height:28px'></select>";
            }
        }
        eleHtml = "<select  style='padding:0px;' id='DDL_" + mapAttr.KeyOfEn + "' class='form-control " + mapAttr.CSS + "'  >" + InitDDLOperation(flowData, mapAttr) + "</select>";
        return eleHtml;
    }

    /***************** 外部数据源 *****************************/
    if (mapAttr.LGType == 0 && mapAttr.MyDataType == "1" && mapAttr.UIContralType == "1") {
        eleHtml = "<select  style='padding:0px;'  id='DDL_" + mapAttr.KeyOfEn + "' class='form-control " + mapAttr.CSS + "' >" + InitDDLOperation(flowData, mapAttr, "") + "</select>";
        return eleHtml;
    }

    /***************** 作为附件展示的控件. *****************************/
    if (mapAttr.UIContralType == 6) {
        var atParamObj = AtParaToJson(mapAttr.AtPara);
        if (atParamObj.AthRefObj != undefined) { //扩展设置为附件展示
            eleHtml += "<input type='hidden' class='tbAth' class='form-control' data-target='" + mapAttr.AtPara + "' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' >" + "</input>";
            defValue = defValue != undefined && defValue != '' ? defValue : '&nbsp;';
            if (defValue.indexOf('@AthCount=') == 0) {
                defValue = "添付ファイル" + "<span class='badge'>" + defValue.substring('@AthCount='.length, defValue.length) + "</span>個";
            } else {
                defValue = defValue;
            }
            eleHtml += "<div class='divAth' data-target='" + mapAttr.KeyOfEn + "'  id='DIV_" + mapAttr.KeyOfEn + "'>" + defValue + "</div>";
        }
        return eleHtml;
    }

    /***************** 其他类型的控件. *****************************/
    var str = '';
    var isInOneRow = false; //是否占一整行
    var islabelIsInEle = false; //
    eleHtml += '';

    //添加文本框 ，日期控件等
    //AppString
    if (mapAttr.MyDataType == "1") {

        //签字板
        if (mapAttr.UIContralType == "8") {
            //查找默认值
            var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
            //如果是图片签名，并且可以编辑
            var ondblclick = ""
            if (mapAttr.UIIsEnable == 1) {
                ondblclick = " ondblclick='figure_Template_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + val + "\")'";
            }

            var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden />";
            eleHtml += "<img src='" + val + "' " + ondblclick + " onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
            return eleHtml;
        }

        //普通类型的单行文本.
        if (mapAttr.UIHeight <= 40) {

            //如果是图片签名，并且可以编辑
            if (mapAttr.IsSigan == "1" && mapAttr.UIIsEnable == 1) {
                //查找默认值
                var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + GetQueryString("UserNo") + "' type=hidden />";
                //是否签过
                var sealData = new Entities("BP.Tools.WFSealDatas");
                sealData.Retrieve("OID", GetQueryString("WorkID"), "FK_Node", GetQueryString("FK_Node"), "SealData", GetQueryString("UserNo"));

                if (sealData.length > 0) {

                    //先判断是否存在签名图片
                    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
                    handler.AddPara('no', GetQueryString("UserNo"));
                    data = handler.DoMethodReturnString("HasSealPic");

                    if (data.length > 0) {
                        eleHtml += data + html;
                    }
                    else {
                        eleHtml += "<img src='../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../DataUser/Siganture/Templete.JPG'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                    }
                    isSigantureChecked = true;
                }
                else {
                    eleHtml += "<img src='../DataUser/Siganture/siganture.jpg' onerror=\"this.src='../DataUser/Siganture/Templete.JPG'\" ondblclick='figure_Template_Siganture(\"" + mapAttr.KeyOfEn + "\",\"" + GetQueryString("UserNo") + "\",\"0\")' style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                }
                return eleHtml;
            }
            if (mapAttr.IsSigan == "4" && mapAttr.UIIsEnable == 1) {
                //查找默认值
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + mapAttr.DefVal + "' type=hidden />";
                //是否签过
                var sealData = new Entities("BP.Tools.WFSealDatas");
                sealData.Retrieve("OID", GetQueryString("WorkID"), "FK_Node", GetQueryString("FK_Node"), "SealData", mapAttr.DefVal);

                if (sealData.length > 0) {
                    eleHtml += "<img src='../DataUser/Siganture/" + mapAttr.DefVal + ".jpg' style='border:0px;'  id='Img" + mapAttr.KeyOfEn + "' />" + html;
                    isSigantureChecked = true;
                }
                else {
                    eleHtml += "<img src='../DataUser/Siganture/siganture.jpg'  ondblclick='figure_Template_Siganture(\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.DefVal + "\",\"1\")' style='border:0px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                }
                //eleHtml += "<img src='../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\" style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                return eleHtml;
            }
            //如果不可编辑，并且是图片名称
            if (mapAttr.IsSigan == "1") {
                var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var handler = new HttpHandler("BP.WF.HttpHandler.WF");
                handler.AddPara('No', val);
                data = handler.DoMethodReturnString("HasSealPic");
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden />";
                if (data.length > 0) {
                    eleHtml += data + html;
                }
                else {
                    eleHtml += "<img src='../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../DataUser/Siganture/Templete.JPG'\" style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;

                }
                return eleHtml;
            }
            if (mapAttr.IsSigan == "4") {
                //var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + mapAttr.DefVal + "' type=hidden />";
                eleHtml += "<img src='../DataUser/Siganture/" + mapAttr.DefVal + ".jpg' onerror=\"this.src='../DataUser/Siganture/Templete.JPG'\"  style='border:0px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                //eleHtml += "<img src='../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\" style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                return eleHtml;
            }

            eleHtml += "<input class='form-control " + mapAttr.CSS + "' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' type='text' placeholder='" + (mapAttr.Tip || '') + "' />";

            return eleHtml;
        }

        //判断是否是富文本?
        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            //如果是富文本就使用百度 UEditor

            if (mapAttr.UIIsEnable == "0") {
                //只读状态直接 div 展示富文本内容
                //eleHtml += "<script id='" + editorPara.id + "' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
                var defValue = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                eleHtml = "<div class='richText' style='style='width:99%;'>" + defValue + "</div>";
                return eleHtml;
            }

            document.BindEditorMapAttr = mapAttr; //存到全局备用

            //设置编辑器的默认样式.
            var styleText = "text-align:left;font-size:12px;";
            styleText += "width:100%;";
            styleText += "height:" + mapAttr.UIHeight + "px;";
            //注意这里 name 属性是可以用来绑定表单提交时的字段名字的 editor是特殊约定的.
            eleHtml += "<script id='editor' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'></script>";
            // eleHtml += "<script id='editor' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";

            return eleHtml;
        }

        //多行文本模式.
        eleHtml = "<textarea maxlength=" + mapAttr.MaxLen + " style='height:" + mapAttr.UIHeight + "px;' id='TB_" + mapAttr.KeyOfEn + "' type='text' placeholder='" + (mapAttr.Tip || '') + "'/>";
        return eleHtml;
    }

    //日期类型.
    if (mapAttr.MyDataType == 6) { //AppDate
        var enableAttr = '';
        var frmDate = mapAttr.IsSupperText;//获取日期格式
        var dateFmt = '';
        if (frmDate == 0) {
            dateFmt = "yyyy-MM-dd";
        } else if (frmDate == 3) {
            dateFmt = "yyyy-MM";
        } else if (frmDate == 6) {
            dateFmt = "MM-dd";
        }
        if (mapAttr.UIIsEnable == 1) {
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'" + dateFmt + "'})" + '";';
        } else {
            enableAttr = "disabled='disabled'";
        }
            

        eleHtml = "<input  type='text' class='form-control Wdate " + mapAttr.CSS + "' " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
        return eleHtml;
    }

    //日期时间类型.
    if (mapAttr.MyDataType == 7) { // AppDateTime = 7
        var enableAttr = '';
        var frmDate = mapAttr.IsSupperText; //获取日期格式
        var dateFmt = '';
        if (frmDate == 1) {
            dateFmt = "yyyy-MM-dd HH:mm";
        } else if (frmDate == 2) {
            dateFmt = "yyyy-MM-dd HH:mm:ss";
        } else if (frmDate == 4) {
            dateFmt = "HH:mm";
        } else if (frmDate == 5) {
            dateFmt = "HH:mm:ss";
        }
        if (mapAttr.UIIsEnable == 1) {
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'" + dateFmt + "'})" + '";';
        } else {
            enableAttr = "disabled='disabled'";
        }
            
        eleHtml = "<input type='text' class='form-control Wdate " + mapAttr.CSS + "'  " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
        return eleHtml;
    }

    //checkbox 类型.
    if (mapAttr.MyDataType == 4) { // AppBoolean = 7
        eleHtml += "<label><input " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox'  id='CB_" + mapAttr.KeyOfEn + "' />";
        eleHtml += mapAttr.Name + '</label>';
        return eleHtml;
    }

    //枚举下拉框.
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
        if (mapAttr.UIContralType == 1) { //DDL
            //多选下拉框.
            eleHtml += "<select  style='padding:0px;' id='DDL_" + mapAttr.KeyOfEn + "' class='form-control " + mapAttr.CSS + "'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(flowData, mapAttr, "") + "</select>";
        }
        return eleHtml;
    }

    // 浮点类型. AppDouble  AppFloat
    if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {
        //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
        var defVal = mapAttr.DefVal;
        var bit;
        if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
            bit = defVal.substring(defVal.indexOf(".") + 1).length;

        eleHtml += "<input style='text-align:right;' class='form-control " + mapAttr.CSS + "' onkeyup=" + '"' + "valitationAfter(this, 'float');if(!(value.indexOf('-')==0&&value.length==1)&&isNaN(value))execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'float');if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/>";
        return eleHtml;
    }

    // int 类型.
    if ((mapAttr.MyDataType == 2 && mapAttr.UIContralType == 0)) { //AppInt
        eleHtml += "<input style='text-align:right;' class='form-control " + mapAttr.CSS + "' onkeyup=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
        return eleHtml;
    }

    // 金额类型. AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {

        //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
        var defVal = mapAttr.DefVal;
        var bit;
        if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
            bit = defVal.substring(defVal.indexOf(".") + 1).length;
        else
            bit = 2;

        eleHtml += "<input style='text-align:right;' class='form-control " + mapAttr.CSS + "' onkeyup=" + '"' + "valitationAfter(this, 'money');limitLength(this," + bit + "); FormatMoney(this, " + bit + ", ',')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'money');if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/>";
        return eleHtml;
    }

   
    alert(mapAttr.Name + '判断がない...');
}

//将#FF000000 转换成 #FF0000
function TranColorToHtmlColor(color) {
    if (color != undefined && color.indexOf('#') == 0 && color.length == 9) {
        color = color.substring(0, 7);
    }
    return color;
}

//FontStyle, FontWeight, IsBold, IsItalic
//fontStyle font-size:19;font-family:"Portable User Interface";font-weight:bolder;color:#FF0051; 为H5设计的，不用解析后面3个
function analysisFontStyle(ele, fontStyle, isBold, isItalic) {
    if (fontStyle != undefined && fontStyle.indexOf(':') > 0) {
        var fontStyleArr = fontStyle.split(';');
        $.each(fontStyleArr, function (i, fontStyleObj) {
            if (fontStyleObj.split(':')[0] == 'font-size')
                ele.css(fontStyleObj.split(':')[0], fontStyleObj.split(':')[1] + 'px');
            else if (fontStyleObj.split(':')[0] == 'color')
                ele.css(fontStyleObj.split(':')[0], TranColorToHtmlColor(fontStyleObj.split(':')[1]));
            else
                ele.css(fontStyleObj.split(':')[0], fontStyleObj.split(':')[1]);
            
        });
    }
    else {

        if (isBold == 1) {
            ele.css('font-weight', 'bold');
        }

        if (isItalic == 1) {
            ele.css('font-style', 'italic');
        }
    }
}

//升级表单元素 初始化Label
function figure_Template_Label(frmLab) {
    var eleHtml = '';
    eleHtml = "<label id='" + frmLab.MyPK + "' ></label>";

    eleHtml = $(eleHtml);
    var text = frmLab.Text == null ? "" : frmLab.Text.replace(/@/g, "<br>");
    eleHtml.html(text);
    eleHtml.css('position', 'absolute').css('top', frmLab.Y -5).css('left', frmLab.X).css('font-size', frmLab.FontSize)
        .css('padding-top', '5px').css('color', TranColorToHtmlColor(frmLab.FontColor));
    analysisFontStyle(eleHtml, frmLab.FontStyle, frmLab.isBold, frmLab.IsItalic);
    return eleHtml;
}

//初始化按钮
function figure_Template_Btn(frmBtn) {
    var eleHtml = $('<div></div>');
    var btnId = frmBtn.BtnID;
    if (btnId == null || btnId == "")
        btnId = frmBtn.MyPK;
    var btnHtml = $("<input id='" + btnId + "' type='button' value='' >");
    btnHtml.val(frmBtn.Text).width(frmBtn.W).height(frmBtn.H).addClass('btn');
    var doc = frmBtn.EventContext;
    doc = (doc == null ? "" : doc.replace(/~/g, "'"));
    var eventType = frmBtn.EventType;
    if (eventType == 0 || pageData.IsReadOnly=="1") {//禁用
        btnHtml.attr('disabled', 'disabled').css('background', 'gray');
    } else if (eventType == 1) {//运行Exe文件. 运行JS
        $.each(flowData.Sys_MapAttr, function (i, obj) {
            if (doc != null && doc.indexOf('@' + obj.KeyOfEn) > 0) {
                //替换
                //url=  url.replace(new RegExp(/(：)/g), ':');
                //先这样吧
                doc = doc.replace('@' + obj.KeyOfEn, flowData.MainTable[0][obj.KeyOfEn]);
            }
        });

        var OID = GetQueryString("OID");
        if (OID == undefined || OID == "");
        OID = GetQueryString("WorkID");
        var FK_Node = GetQueryString("FK_Node");
        var FK_Flow = GetQueryString("FK_Flow");
        var webUser = new WebUser();
        var userNo = webUser.No;
        var SID = webUser.SID;
        if (SID == undefined)
            SID = "";
        if (doc.indexOf("?") == -1)
            doc = doc + "?1=1";
        doc = doc + "&OID=" + OID + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&SID=" + SID;

        btnHtml.attr('onclick', "window.open('" + doc + "')");
    } else {
        if (doc.indexOf("(") == -1)
            doc = doc + "()";
        btnHtml.attr('onclick', doc);
    }
    eleHtml.append(btnHtml);
    //别的一些属性先不加
    eleHtml.css('position', 'absolute').css('top', frmBtn.Y).css('left', frmBtn.X).width(frmBtn.W).height(frmBtn.H);
    return eleHtml;
}

//初始化单选按钮
function figure_Template_RB(frmRb) {
    var MyPK = frmRb.FK_MapData + "_" + frmRb.KeyOfEn;
    var mapAttr = new Entity("BP.Sys.MapAttr", MyPK);

    var eleHtml = '<div></div>';
    eleHtml = $(eleHtml);
    var childRbEle = $('<input id="RB_ChuLiFangShi2" type="radio" />');
    var childLabEle = $('<label class="labRb"></label>');
    childLabEle.html(frmRb.Lab).attr('for', 'RB_' + frmRb.KeyOfEn + "_" + frmRb.IntKey).attr('name', 'RB_' + frmRb.KeyOfEn);

    childRbEle.val(frmRb.IntKey).attr('id', 'RB_' + frmRb.KeyOfEn + "_" + frmRb.IntKey).attr('name', 'RB_' + frmRb.KeyOfEn);
    if (mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0)
        childRbEle.attr("onclick", "clickEnable( this ,\"" + frmRb.FK_MapData + "\",\"" + frmRb.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")");

//    if (frmRb.UIIsEnable == false)
//        childRbEle.attr('disabled', 'disabled');

    var defVal = ConvertDefVal(flowData, '', frmRb.KeyOfEn);
    if (defVal == frmRb.IntKey) {
        childRbEle.attr("checked", "checked");
    }

    eleHtml.append(childRbEle).append(childLabEle);
    eleHtml.css('position', 'absolute').css('top', frmRb.Y).css('left', frmRb.X);
    return eleHtml;
}

//初始化超链接
function figure_Template_HyperLink(frmLin) {
    //URL @ 变量替换
    var url = frmLin.URL;
    $.each(flowData.Sys_MapAttr, function (i, obj) {
        if (url != null && url.indexOf('@' + obj.KeyOfEn) > 0) {
            //替换
            //url=  url.replace(new RegExp(/(：)/g), ':');
            //先这样吧
            url = url.replace('@' + obj.KeyOfEn, flowData.MainTable[0][obj.KeyOfEn]);
        }
    });
    
    var OID = GetQueryString("OID");
    if (OID == undefined || OID == "");
    OID = GetQueryString("WorkID");
    var FK_Node = GetQueryString("FK_Node");
    var FK_Flow = GetQueryString("FK_Flow");
    var webUser = new WebUser();
    var userNo = webUser.No;
    var SID = webUser.SID;
    if (SID == undefined)
        SID = "";
    if (url.indexOf("?") == -1)
        url = url + "?1=1";
    if (url.indexOf("Search.htm") != -1)
        url = url + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&SID=" + SID;
    else
        url = url + "&OID=" + OID + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&SID=" + SID;

    var eleHtml = '<span></span>';
    eleHtml = $(eleHtml);

    var a = $("<a></a>");
    a.attr('href', url).attr('target', frmLin.Target).html(frmLin.Text);
    eleHtml.append(a);
    eleHtml.css('position', 'absolute')
        .css('top', frmLin.Y)
        .css('left', frmLin.X)
        .css('color', frmLin.FontColr)
        .css('fontsize', frmLin.FontSize)
        .css('font-family', frmLin.FontName);
    return eleHtml;
}


//初始化 IMAGE  只初始化了图片类型
function figure_Template_Image(frmImage) {
    var eleHtml = '';
     //解析图片
    if (frmImage.ImgAppType == 0) { //图片类型
        //数据来源为本地.
        var imgSrc = '';
        if (frmImage.ImgSrcType == 0) {
            //替换参数
            var frmPath = frmImage.ImgPath;
            frmPath = frmPath.replace('＠', '@');
            frmPath = frmPath.replace('@basePath', basePath);
            frmPath = frmPath.replace('@basePath', basePath);
            imgSrc = DealJsonExp(flowData.MainTable[0], frmPath);
        }

        //数据来源为指定路径.
        if (frmImage.ImgSrcType == 1) {
            var url = frmImage.ImgURL;
            url = url.replace('＠', '@');
            url = url.replace('@basePath', basePath);
            imgSrc = DealJsonExp(flowData.MainTable[0], url);
        }
        // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
        if (imgSrc == "" || imgSrc == null)
            imgSrc = "../DataUser/ICON/CCFlow/LogBig.png";

        eleHtml = $('<div></div>');
        var a = $("<a></a>");
        var img = $("<img/>")

        img.attr("src", imgSrc).css('width', frmImage.W).css('height', frmImage.H).attr('onerror', "this.src='../DataUser/ICON/CCFlow/LogBig.png'");

        if (frmImage.LinkURL != undefined && frmImage.LinkURL != '') {
            a.attr('href', frmImage.LinkTarget).attr('target', frmImage.LinkTarget).css('width', frmImage.W).css('height', frmImage.H);
            a.append(img);
            eleHtml.append(a);
        } else {
            eleHtml.append(img);
        }

        eleHtml.attr("id", frmImage.MyPK);
        eleHtml.css('position', 'absolute').css('top', frmImage.Y).css('left', frmImage.X).css('width', frmImage.W).css('height', frmImage.H); ;
    } else if (frmImage.ImgAppType == 3)//二维码  手机
    {


    } else if (frmImage.ImgAppType == 1) {//暂不解析
        //电子签章  写后台
    }
    return eleHtml;
}
//图片附件编辑
function ImgAth(url, athMyPK) {
    var dgId = "iframDg";
    url = url + "&s=" + Math.random();
    OpenEasyUiDialog(url, dgId, '写真の添付', 900, 580, 'icon-new', false, function () {

    }, null, null, function () {
        //关闭也切换图片
        //var obj = document.getElementById(dgId);
        //var win =(obj.contentWindow || obj.contentDocument); 
        var imgSrc = $("#imgSrc").val();
        if (imgSrc != null && imgSrc != "")
            document.getElementById('Img' + athMyPK).setAttribute('src', imgSrc + "?t=" + Math.random());
        $("#imgSrc").val("");
    });
}

//初始化 IMAGE附件
function figure_Template_ImageAth(frmImageAth) {
    var isEdit = frmImageAth.IsEdit;
    var eleHtml = $("<div></div>");
    var img = $("<img class='pimg'/>");

    var imgSrc = basePath + "/WF/Data/Img/LogH.PNG";

    //获取数据
    if (frmImageAth.FK_MapData.indexOf("ND") != -1)
        imgSrc = basePath + "/DataUser/ImgAth/Data/" + frmImageAth.CtrlID + "_" + pageData.WorkID + ".png";
    else
        imgSrc = basePath + "/DataUser/ImgAth/Data/" + frmImageAth.FK_MapData + "_" + frmImageAth.CtrlID + "_" + pageData.WorkID + ".png";

    //设计属性
    img.attr('id', 'Img' + frmImageAth.MyPK).attr('name', 'Img' + frmImageAth.MyPK);
    img.attr("src", imgSrc).attr('onerror', "this.src='" + basePath + "/WF/Admin/CCFormDesigner/Controls/DataView/AthImg.png'");
    img.css('width', frmImageAth.W).css('height', frmImageAth.H).css('padding', "0px").css('margin', "0px").css('border-width', "0px");
    //不可编辑
    if (isEdit == "1" && pageData.IsReadonly!="1") {
        var fieldSet = $("<fieldset></fieldset>");
        var length = $("<legend></legend>");
        var a = $("<a></a>");
        var url = basePath + "/WF/CCForm/ImgAth.htm?W=" + frmImageAth.W + "&H=" + frmImageAth.H + "&FK_MapData=" + flowData.Sys_MapData[0].No + "&RefPKVal=" + pageData.WorkID + "&CtrlID=" + frmImageAth.CtrlID;

        a.attr('href', "javascript:ImgAth('" + url + "','" + frmImageAth.MyPK + "');").html("編集");
        length.css('font-style', 'inherit').css('font-weight', 'bold').css('font-size', '12px');

        fieldSet.append(length);
        length.append(a);
        fieldSet.append(img);
        eleHtml.append(fieldSet);
    } else {
        eleHtml.append(img);
    }
    eleHtml.css('position', 'absolute').css('top', frmImageAth.Y).css('left', frmImageAth.X);
    return eleHtml;
}

//初始化 附件
function figure_Template_Attachment(frmAttachment) {
    var eleHtml = '';
    var ath = frmAttachment;
    if (ath.UploadType == 0) { //单附件上传 L4204
        return $('');
    }
    var src = "";
    if (pageData.IsReadonly)
        src = "./CCForm/Ath.htm?PKVal=" + pageData.WorkID + "&PWorkID=" + GetQueryString("PWorkID") + "&FID=" + pageData["FID"] + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1&FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow;
    else
        src = "./CCForm/Ath.htm?PKVal=" + pageData.WorkID + "&PWorkID=" + GetQueryString("PWorkID") + "&FID=" + pageData["FID"] + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow;

    eleHtml += '<div>' + "<iframe style='width:" + ath.W + "px;height:" + ath.H + "px;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', ath.Y).css('left', ath.X).css('width', ath.W).css('height', ath.H);

    return eleHtml;
}

function connector_Template_Line(frmLine) {
    var eleHtml = '';
    eleHtml = '<table><tr><td></td></tr></table>';
    eleHtml = $(eleHtml).css('position', 'absolute').css('top', frmLine.Y1).css('left', frmLine.X1);
    if (navigator.userAgent.indexOf('Firefox') >= 0) {
        eleHtml.find('td').css('padding', '0px')
        .css('width', Math.abs(frmLine.X1 - frmLine.X2) == 0 ? 1 : Math.abs(frmLine.X1 - frmLine.X2))
    .css('height', Math.abs(frmLine.Y1 - frmLine.Y2) == 0 ? 2: Math.abs(frmLine.Y1 - frmLine.Y2))
        .css("background", frmLine.BorderColor);
    } else {
        eleHtml.find('td').css('padding', '0px')
        .css('width', Math.abs(frmLine.X1 - frmLine.X2) == 0 ? 0 : Math.abs(frmLine.X1 - frmLine.X2))
    .css('height', Math.abs(frmLine.Y1 - frmLine.Y2) == 0 ? 1 : Math.abs(frmLine.Y1 - frmLine.Y2))
        .css("background", frmLine.BorderColor).css("border-color",frmLine.BorderColor == 'Black'?"":frmLine.BorderColor);
    }

    return eleHtml;
}

//初始化从表
function figure_Template_Dtl(frmDtl) {
    var eleHtml = $("<DIV id='Fd" + frmDtl.No + "' style='position:absolute; left:" + frmDtl.X + "px; top:" + frmDtl.Y + "px; width:" + frmDtl.W + "px; height:" + frmDtl.H + "px;text-align: left;' >");
    var paras = this.pageData;
    var strs = "";
    for (var str in paras) {
        if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
            continue
        else
            strs += "&" + str + "=" + paras[str];
    }
    var src = "";
    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    if (frmDtl.ListShowModel == "0") {

        var dtlUrl = "Dtl2017";
        if (frmDtl.DtlVer == 1)
            dtlUrl = "Dtl2019";

        //表格模式
        if (pageData.IsReadonly) {
            src = "./CCForm/" + dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=1&" + urlParam + "&Version=1";
        } else {
            src = "./CCForm/" + dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&FK_MapData=" + frmDtl.FK_MapData +"&IsReadonly=0&" + urlParam + "&Version=1";
        }
    } else if (frmDtl.ListShowModel == "1") {
        //卡片模式
        if (pageData.IsReadonly) {
            src = "./CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=1&" + urlParam + "&Version=1";
        } else {
            src = "./CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=0&" + urlParam + "&Version=1";
        }
    }

    var eleIframe = '<iframe></iframe>';
    eleIframe = $("<iframe class='Fdtl' name='Dtl' ID='Dtl_" + frmDtl.No + "' src='" + src +
                 "' frameborder=0  style='position:absolute;width:" + frmDtl.W + "px; height:" + frmDtl.H +
                 "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
    if (pageData.IsReadonly) {

    } else {
        if (frmDtl.DtlSaveModel == 0) {
            eleHtml.append(addLoadFunction(frmDtl.No, "blur", "SaveDtl"));
            eleIframe.attr('onload', frmDtl.No + "load()");
        }
    }
    eleHtml.append(eleIframe);
    //added by liuxc,2017-1-10,此处前台JS中增加变量DtlsLoadedCount记录明细表的数量，用于加载完全部明细表的判断
    var js = "";
    if (!pageData.IsReadonly) {
        js = "<script type='text/javascript' >";
        js += " function SaveDtl(dtl) { ";
        js += "   GenerPageKVs(); //呼び出したkvs ";
        js += "\n   var iframe = document.getElementById('F' + dtl );";
        js += "   if(iframe && iframe.contentWindow){ ";
        js += "      iframe.contentWindow.SaveDtlData(); ";
        js += "   } ";
        js += " } ";
        js += " function SaveM2M(dtl) { ";
        js += "   document.getElementById('F' + dtl ).contentWindow.SaveM2M();";
        js += "} ";
        js += "</script>";
        eleHtml.append($(js));
    }
    return eleHtml;
}

//初始化轨迹图
function figure_Template_FigureFlowChart(wf_node) {

    //轨迹图
    var sta = wf_node.FrmTrackSta;
    var x = wf_node.FrmTrack_X;
    var y = wf_node.FrmTrack_Y;
    var h = wf_node.FrmTrack_H;
    var w = wf_node.FrmTrack_W;

    if (sta == 0) {
        return $('');
    }

    if (sta == undefined) {
        return;
    }

    var src = "./WorkOpt/OneWork/OneWork.htm?CurrTab=Track";
    src += '&FK_Flow=' + pageData.FK_Flow;
    src += '&FK_Node=' + pageData.FK_Node;
    src += '&WorkID=' + pageData.WorkID;
    src += '&FID=' + pageData.FID;
    var eleHtml = '<div id="divtrack' + wf_node.NodeID + '">' + "<iframe id='track" + wf_node.NodeID + "' style='width:" + w + "px;height=" + h + "px;'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//审核组件
function figure_Template_FigureFrmCheck(wf_node) {

    //审核组键FWCSta Sta,FWC_X X,FWC_Y Y,FWC_H H, FWC_W W from WF_Node
    var sta = wf_node.FWCSta;
    var x = wf_node.FWC_X;

    if (x <= 20)
        x = 200;

    var y = wf_node.FWC_Y;
    if (y <= 20)
        y = 500;

    var h = wf_node.FWC_H;
    var w = wf_node.FWC_W;
    if (sta == 0)
        return $('');

    var src = "";
    if (wf_node.FWCVer == 0 || wf_node.FWCVer == "" || wf_node.FWCVer == undefined)
        src = "./WorkOpt/WorkCheck.htm?s=2";
    else
        src = "./WorkOpt/WorkCheck2019.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.WorkID;
    if (sta == 2)//只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;

    var eleHtml = '<div >' + "<iframe style='width:100%' height=" + h + "' id='FWC' src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto ></iframe>" + '</div>';

    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);
    return eleHtml;
}

//子线程
function figure_Template_FigureThreadDtl(wf_node) {

    //FrmThreadSta Sta,FrmThread_X X,FrmThread_Y Y,FrmThread_H H,FrmThread_W
    var sta = wf_node.FrmThreadSta;
    var x = wf_node.FrmThread_X;
    var y = wf_node.FrmThread_Y;
    var h = wf_node.FrmThread_H;
    var w = wf_node.FrmThread_W;
    if (sta == 0 || sta == '0')
        return $('');

    var src = "./WorkOpt/Thread.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.WorkID;

    if (sta == 2) //只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;
    var eleHtml = '<div id=DIVFT' + wf_node.NodeID + '>' + "<iframe id=FFT" + wf_node.NodeID + " style='width:100%;height:" + h + "px;'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//子流程
function figure_Template_FigureSubFlowDtl(wf_node) {
    //SFSta Sta,SF_X X,SF_Y Y,SF_H H, SF_W W
    var sta = wf_node.SFSta;
    var x = wf_node.SF_X;
    var y = wf_node.SF_Y;
    var h = wf_node.SF_H;
    var w = wf_node.SF_W;
    if (sta == 0)
        return $('');

    var src = "./WorkOpt/SubFlow.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.WorkID;
    if (sta == 2 || pageData.IsReadonly == 1)//只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;
    var eleHtml = '<div id=DIVWC' + wf_node.NodeID + '>' + "<iframe id=FSF" + wf_node.NodeID + " style='width:" + w + "px';height:" + h + "px'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}


