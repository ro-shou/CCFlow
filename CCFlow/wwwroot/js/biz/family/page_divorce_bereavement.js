/*���O���[�o���ϐ���`�G���A��*/
var webUser = null;
var wfCommon = new wfCommon();                       // ���ʊ֘A
var dtKbn;                                           // �敪���i�[�I�u�W�F�N�g
var dtCondolence;                                    // �����A�����i�[�I�u�W�F�N�g
var dtApplicant;                                     // �\���ҏ��i�[�I�u�W�F�N�g
var dtDelegator;                                     // �ϔC�ҏ��i�[�I�u�W�F�N�g
var nextNode;                                        // ���m�[�h
var wfstate;                                         // �t���[��ԃX�e�[�^�X
var flowStater;

var agentMode;                                       // �㗝���[�h 0:�{�l�@1:�㗝
var checkRefer;                                      // �Q�Ɨp
var checkInput;                                      // ���͗p
//�c�ǉ��ϐ�

/*���O���[�o���֐���`�G���A��*/

/*����ʋN���G���A��*/
(function ($) {
    $.fn.extend({
        "getElements": function () {
            let str = STRING_EMPTY;
            let els = this[0].elements;
            for (let i = 0; i < els.length; i++) {
                let el_type = els[i].type;
                if (el_type === "checkbox" || el_type === "radio") {
                    str += els[i].checked + ";"
                } else if (el_type === "text" || el_type === "email" || el_type === "select-one") {
                    str += els[i].value + ";"
                }
            }
            return str;
        }
    });

    if (webUser === null)
        webUser = new WebUser();
    if (webUser.No === null)
        return;

    //��ʏ�����
    InitPage();

})(jQuery);
/*����ʋN���G���A��*/

/*���֐���`�G���A��*/
/**
 * ��ʏ�����
 */
function InitPage() {

    var objGetTbl = {};
    objGetTbl[0] = "KBN";
    objGetTbl[1] = "TT_WF_CONDOLENCE";
    // ��ʍ��ڂ������ݒ�
    wfCommon.initGetData(objGetTbl, iniPageItems);

}

/**
 * ��ʍ��ڂ������ݒ�
 * 
 * @param {Array<object>} data �T�[�o����擾�̃f�[�^�z��
 */
function iniPageItems(data) {
    //console.log("iniPageItems start");
    // �����f�[�^��ݒ�
    dtKbn = data["KBN"];
    dtCondolence = data["TT_WF_CONDOLENCE"][0];
    nextNode = dtCondolence["NEXTNODEID"];
    wfstate = dtCondolence["WFState"];
    flowStater = dtCondolence["FlowStarter"];
    agentMode = GetQueryString("AgentMode");
    if (agentMode === null) {
        agentMode = String(dtCondolence["SHINSEISYAKBN"]);
    }
    //console.log(data);

    // �X�֔ԍ����ڂ�������
    initPostcodeItems();
    // ���t���ڂ�������
    initDateItems();
    // �㗝���ڂ�������
    initAgentItems();
    // ���̓`�F�b�N��ݒ肷��
    setInputCheck();
    // �C�x���g��`��ݒ肷��
    setEvents();
    // START PAGE��DATA���Z�b�g
    var ht = new HashTblCommon();
    // �Ј��ԍ�
    ht.Add("SHAINBANGO", webUser.No);
    wfCommon.getApiInfoAjaxCallBack(GET_CONDOLENCE_SHAIN_INFO_APINAME, ht, setDataItems);

    //console.log("iniPageItems end");
}

/**
 * �����ݒ�F�X�֔ԍ�����
 * */
function initPostcodeItems() {

    // �ʖ�-�X�֔ԍ��̃R���|�[�l���g��������
    $("#postal").jpostal({
        click: "#btn-wake-postcode",
        postcode: "#text-wake-postcode",
        address: {
            "#text-wake-address1": "%3%4",
            "#text-wake-address2": "%5"
        }
    });

    // ���ʎ�-�X�֔ԍ��̃R���|�[�l���g��������
    $("#postal").jpostal({
        click: "#btn-funeral-postcode",
        postcode: "#text-funeral-postcode",
        address: {
            "#text-funeral-address1": "%3%4",
            "#text-funeral-address2": "%5"
        }
    });

    // �����-�X�֔ԍ��̃R���|�[�l���g��������
    $("#postal").jpostal({
        click: "#btn-rear-postcode",
        postcode: "#text-rear-postcode",
        address: {
            "#text-rear-address1": "%3%4",
            "#text-rear-address2": "%5"
        }
    });

    wfCommon.setZipCodeInput("text-wake-postcode");
    wfCommon.setZipCodeInput("text-funeral-postcode");
    wfCommon.setZipCodeInput("text-rear-postcode");
}

/**
 * �����ݒ�F���t����
 */
function initDateItems() {
    // ������
    wfCommon.setdatepicker("#text-died-death_date", DATE_FORMAT_MOMENT_PATTERN_1, true, false);
    // �ʖ���t
    wfCommon.setdatepicker("#text-wake-date", DATE_FORMAT_MOMENT_PATTERN_1, true, false);
    // ���ʎ����t
    wfCommon.setdatepicker("#text-funeral-date", DATE_FORMAT_MOMENT_PATTERN_1, true, false);
    // ���͂����t
    wfCommon.setdatepicker("#text-rear-date", DATE_FORMAT_MOMENT_PATTERN_1, true, false);
}

/**
 * �����ݒ�F�㗝�\���ւ��鍀��
 */
function initAgentItems() {

    if (agentMode === SHINSEISYA_KBN_DAIRI) { // �㗝�\���̏ꍇ
        $(".p-title").text("�����̘A��(�㗝�\��)");
        $(".f-agent").show();
        $(".f-noagent").hide();
    } else { // �{�l�\���̏ꍇ
        $(".p-title").text("�����̘A��(�{�l�\��)");
        $(".f-agent").hide();
        $(".f-noagent").show();
    }

    // �C�����[�h�̏ꍇ
    if (wfstate === WF_STATE_OVER) {
        $(".p-title").text("�����̘A��(�C���\��)");
    }

}

/**
 * �r����Z�b�g
 * */
function setMournerItems() {
    // �r��G���A��ݒ�
    setMournerArea();
    let value = dtCondolence["ORGANIZER_JUGYOIN_ZOKUGARAKBN"];
    // �r��敪�I��l��ݒ�
    wfCommon.radiosSetVal("radio-mourner", value, ORGANIZER_KBN_HONNIN);

    if (value === ORGANIZER_KBN_HONNIN_IGAI) { // �����ȊO
        // �r�厁���i���j
        $("#text-mourner-lastname").val(dtCondolence["ORGANIZER_SHIMEI_SEI"]);
        // �r�厁���i���j
        $("#text-mourner-firstname").val(dtCondolence["ORGANIZER_SHIMEI_MEI"]);
        // �r��J�i�����i���j
        $("#text-mourner-lastname_kana").val(dtCondolence["ORGANIZER_KANASHIMEI_SEI"]);
        // �r��J�i�����i���j
        $("#text-mourner-firstname_kana").val(dtCondolence["ORGANIZER_KANASHIMEI_MEI"]);
    }
}

/**
 * �r���ݒ�
 *
 * */
function setMournerArea() {

    if (dtCondolence["ORGANIZER_JUGYOIN_ZOKUGARAKBN"] === ORGANIZER_KBN_HONNIN_IGAI) { // �����ȊO
        $(".area-mourner").show();
        // �r�厁���i���j
        $("#text-mourner-lastname").val(STRING_EMPTY);
        // �r�厁���i���j
        $("#text-mourner-firstname").val(STRING_EMPTY);
        // �r��J�i�����i���j
        $("#text-mourner-lastname_kana").val(STRING_EMPTY);
        // �r��J�i�����i���j
        $("#text-mourner-firstname_kana").val(STRING_EMPTY);

    } else { // ����
        $(".area-mourner").hide();
    }
}

/**
 * �ʖ�/���ʎ��ɂ��Ă��Z�b�g
 * */
function setWakeAndFuneralItems() {

    // �ʖ�/���ʎ��G���A��ݒ�
    setWakeAndFuneralArea();
    // �ʖ�/���ʎ��I��l
    let value = String(dtCondolence["TSUYA_KOKUBETSUSHIKIKBN"]);
    // �ʖ�/���ʎ��̗L���ɂ���
    wfCommon.radiosSetVal("radio-funeral", value, FUNERAL_KBN_NONE);
    // ��ʎQ������ނ���
    $("#chk-funeral-decline").prop("checked", dtCondolence["SANRETSU_JITAI_KBN"] === CHECKBOX_CHECKED);
    // �ʖ鎞��
    wfCommon.initDropdown(true, wfCommon.getTimeData(15)["content"], dtCondolence["TSUYA_TIME"], "value", "name", "select-wake-time", "pulldown-wake-time");
    // �ʖ�J�n�����v���_�E���̍Č���
    $("#select-wake-time").on("change", function () {
        $("#form1").validate().element($("#select-wake-time"));
    });
    // ���ʎ�����
    wfCommon.initDropdown(true, wfCommon.getTimeData(15)["content"], dtCondolence["KOKUBETSUSHIKI_TIME"], "value", "name", "select-funeral-time", "pulldown-funeral-time");
    // ���ʎ��J�n�����v���_�E���̍Č���
    $("#select-funeral-time").on("change", function () {
        $("#form1").validate().element($("#select-funeral-time"));
    });
    if (value === FUNERAL_KBN_BOTH) { // �ʖ�ƍ��ʎ�
        // �ʖ�Ɠ����ꏊ
        $("#chk-funeral-sameplace").prop("checked", dtCondolence["TSUYA_SAME_KBN"] === CHECKBOX_CHECKED);
        setWakeItems();
        setFuneralItems();
        // �\���E��\��
        changeFuneralItemsDisabled($("#chk-funeral-sameplace").prop("checked"));
    } else if (value === FUNERAL_KBN_TSUYA) { // �ʖ�
        setWakeItems();
    } else if (value === FUNERAL_KBN_KOKUBETSUSHIKI) { // ���ʎ�
        setFuneralItems();
    }
}

/**
 * �ʖ�ɂ��Ă��Z�b�g
 * */
function setWakeItems() {
    // �ʖ���t
    let tsuyadate = dtCondolence["TSUYA_DATE"];
    if (tsuyadate !== null) {
        $("#text-wake-date").val(moment(tsuyadate).format(DATE_FORMAT_MOMENT_PATTERN_1));
    }
    // �ʖ��ꖼ
    $("#text-wake-placeName").val(dtCondolence["TSUYA_BASYOUMEI"]);
    // �ʖ��ꖼ�i�J�i�j
    $("#text-wake-placeName_kana").val(dtCondolence["TSUYA_BASYOUFURIGANA"]);
    // �ʖ�X�֔ԍ�
    $("#text-wake-postcode").val(dtCondolence["TSUYA_YUBINBANGO"]);
    // �ʖ�s���{���E�s�S��
    $("#text-wake-address1").val(dtCondolence["TSUYA_ADDRESS1"]);
    // �ʖ钬���E�Ԓn
    $("#text-wake-address2").val(dtCondolence["TSUYA_ADDRESS2"]);
    // �ʖ錚�����E�����ԍ�
    $("#text-wake-address3").val(dtCondolence["TSUYA_ADDRESS3"]);
    // �ʖ���d�b�ԍ�
    let wakeTel = dtCondolence["TSUYA_RENRAKUSAKITEL"];
    if (wakeTel !== null) {
        let wakeTels = wakeTel.split("-");
        $("#text-wake-tel1").val(wakeTels[0]);
        $("#text-wake-tel2").val(wakeTels[1]);
        $("#text-wake-tel3").val(wakeTels[2]);
        $("#text-wake-tel").val(wakeTel);
    }
}

/**
 * ���ʎ��ɂ��Ă��Z�b�g
 * */
function setFuneralItems() {
    // ���ʎ����t
    let kokubetsushikidate = dtCondolence["KOKUBETSUSHIKI_DATE"];
    if (kokubetsushikidate !== null) {
        $("#text-funeral-date").val(moment(kokubetsushikidate).format(DATE_FORMAT_MOMENT_PATTERN_1));
    }
    // ���ʎ���ꖼ
    $("#text-funeral-placeName").val(dtCondolence["KOKUBETSUSHIKI_BASYOUMEI"]);
    // ���ʎ���ꖼ�i�J�i�j
    $("#text-funeral-placeName_kana").val(dtCondolence["KOKUBETSUSHIKI_BASYOUFURIGANA"]);
    // ���ʎ��X�֔ԍ�
    $("#text-funeral-postcode").val(dtCondolence["KOKUBETSUSHIKI_YUBINBANGO"]);
    // ���ʎ��s���{���E�s�S��
    $("#text-funeral-address1").val(dtCondolence["KOKUBETSUSHIKI_ADDRESS1"]);
    // ���ʎ������E�Ԓn
    $("#text-funeral-address2").val(dtCondolence["KOKUBETSUSHIKI_ADDRESS2"]);
    // ���ʎ��������E�����ԍ�
    $("#text-funeral-address3").val(dtCondolence["KOKUBETSUSHIKI_ADDRESS3"]);
    // ���ʎ����d�b�ԍ�
    let funeralTel = dtCondolence["KOKUBETSUSHIKI_RENRAKUSAKITEL"];
    if (funeralTel !== null) {
        let funeralTels = funeralTel.split("-");
        $("#text-funeral-tel1").val(funeralTels[0]);
        $("#text-funeral-tel2").val(funeralTels[1]);
        $("#text-funeral-tel3").val(funeralTels[2]);
        $("#text-funeral-tel").val(funeralTel);
    }
}

/**
 * �ʖ�/���ʎ���ݒ�
 * 
 * */
function setWakeAndFuneralArea() {

    let value = String(dtCondolence["TSUYA_KOKUBETSUSHIKIKBN"]);

    if (value === FUNERAL_KBN_BOTH) { // �ʖ�ƍ��ʎ�
        // ��ʎQ������ނ���:�\��
        $("#area-decline").show();
        // �ʖ�G���A:�\��
        $(".area-wake").show();
        // ���ʎ��G���A:�\��
        $(".area-funeral").show();
        // �ʖ�Ɠ����ꏊ�G���A:�\��
        $("#area-sameplace").show();
    } else if (value === FUNERAL_KBN_TSUYA) { // �ʖ�
        // ��ʎQ������ނ���:�\��
        $("#area-decline").show();
        // �ʖ�G���A:�\��
        $(".area-wake").show();
        // ���ʎ��G���A:��\��
        $(".area-funeral").hide();
    } else if (value === FUNERAL_KBN_KOKUBETSUSHIKI) { // ���ʎ�
        // ��ʎQ������ނ���:�\��
        $("#area-decline").show();
        // �ʖ�G���A:��\��
        $(".area-wake").hide();
        // ���ʎ��G���A:�\��
        $(".area-funeral").show();
        // �ʖ�Ɠ����ꏊ�G���A:�\��
        $("#area-sameplace").hide();
    } else { // �Ȃ�
        // ��ʎQ������ނ���:��\��
        $("#area-decline").hide();
        // �ʖ�G���A:��\��
        $(".area-wake").hide();
        // ���ʎ��G���A:��\��
        $(".area-funeral").hide();
        // ��ʎQ������ނ���:�N���A
        $("#chk-funeral-decline").prop("checked", false);
    }
}

/**
 * ���ԓ͂���ꏊ�敪�ɂ���
 * */
function setAllnightItems() {
    // ����
    wfCommon.radiosSetVal("radio-opener-koryo", dtCondolence["KORYOKBN"], NECESSARY_KBN_HITUYOU);
    // ����
    wfCommon.radiosSetVal("radio-opener-kuge", dtCondolence["KYOKAKBN"], NECESSARY_KBN_HITUYOU);
    // ���d
    wfCommon.radiosSetVal("radio-opener-tyoden", dtCondolence["TYODENKBN"], NECESSARY_KBN_HITUYOU);

    // ���ԓ͂���ꏊ�敪
    let value = dtCondolence["TODOKESAKIKBN"];
    // �ʖ�/���ʎ��̗L���G���A��ݒ�
    setAllnightArea();
    // ���ԓ͂���ꏊ�敪��ݒ�
    wfCommon.radiosSetVal("radio-allnight", value, LOCATION_TSUYA_KBN);

    if (String(value) === LOCATION_ATOKA_KBN) { // �����
        // ����薼�O
        $("#text-rear-name").val(dtCondolence["ATOKAZARI_FULLNAME"]);
        // �����X�֔ԍ�
        $("#text-rear-postcode").val(dtCondolence["ATOKAZARI_YUBINBANGO"]);
        // �����s���{���E�s�S��
        $("#text-rear-address1").val(dtCondolence["ATOKAZARI_ADDRESS1"]);
        // ����蒬���E�Ԓn
        $("#text-rear-address2").val(dtCondolence["ATOKAZARI_ADDRESS2"]);
        // ����茚�����E�����ԍ�
        $("#text-rear-address3").val(dtCondolence["ATOKAZARI_ADDRESS3"]);
        // �������d�b�ԍ�
        let rearTel = dtCondolence["ATOKAZARI_RENRAKUSAKITEL"];
        if (rearTel !== null) {
            let rearTels = rearTel.split("-");
            $("#text-rear-tel1").val(rearTels[0]);
            $("#text-rear-tel2").val(rearTels[1]);
            $("#text-rear-tel3").val(rearTels[2]);
            $("#text-rear-tel").val(rearTel);
        }

        // ����肨�͂���
        let atokazaridate = dtCondolence["ATOKAZARI_DATE"];
        if (atokazaridate !== null) {
            $("#text-rear-date").val(moment(atokazaridate).format(DATE_FORMAT_MOMENT_PATTERN_1));
        }
    }

    setAddresseeDisplay();
}

/**
 * �ʖ�/���ʎ��̗L���G���A��ݒ�
 *
 * */
function setAllnightArea() {

    if (String(dtCondolence["TODOKESAKIKBN"]) === LOCATION_ATOKA_KBN) { // �����
        $(".area-rear").show();
    } else { // �����ȊO
        $(".area-rear").hide();
    }
}

/**
 * ���ʎ��F�ʖ���R�s�[
 *
 * */
function copyWakeItemsToFuneral() {
    // ��ꖼ
    $("#text-funeral-placeName").val($("#text-wake-placeName").val());
    // ��ꖼ�i�J�i�j
    $("#text-funeral-placeName_kana").val($("#text-wake-placeName_kana").val());
    // �X�֔ԍ�
    $("#text-funeral-postcode").val($("#text-wake-postcode").val());
    // �s���{���E�s�S��
    $("#text-funeral-address1").val($("#text-wake-address1").val());
    // �����E�Ԓn
    $("#text-funeral-address2").val($("#text-wake-address2").val());
    // �������E�����ԍ�
    $("#text-funeral-address3").val($("#text-wake-address3").val());
    // ���d�b�ԍ�
    $("#text-funeral-tel1").val($("#text-wake-tel1").val());
    $("#text-funeral-tel2").val($("#text-wake-tel2").val());
    $("#text-funeral-tel3").val($("#text-wake-tel3").val());
    $("#text-funeral-tel").val($("#text-wake-tel").val());
}

/**
 * �ʖ�F���ڒl���N���A
 *
 * */
function clearWakeItems() {
    // ���t
    $("#text-wake-date").val(STRING_EMPTY);
    // �J�n����
    $("#pulldown-wake-time")[0].__component.reset();
    // ��ꖼ
    $("#text-wake-placeName").val(STRING_EMPTY);
    // ��ꖼ�i�J�i�j
    $("#text-wake-placeName_kana").val(STRING_EMPTY);
    // �X�֔ԍ�
    $("#text-wake-postcode").val(STRING_EMPTY);
    // �s���{���E�s�S��
    $("#text-wake-address1").val(STRING_EMPTY);
    // �����E�Ԓn
    $("#text-wake-address2").val(STRING_EMPTY);
    // �������E�����ԍ�
    $("#text-wake-address3").val(STRING_EMPTY);
    // ���d�b�ԍ�
    $("#text-wake-tel1").val(STRING_EMPTY);
    $("#text-wake-tel2").val(STRING_EMPTY);
    $("#text-wake-tel3").val(STRING_EMPTY);
    $("#text-wake-tel").val(STRING_EMPTY);
    // �ʖ�Ɠ����ꏊ
    $("#chk-funeral-sameplace").prop("checked", false);
}

/**
 * ���ʎ��F���ڒl���N���A
 *
 * */
function clearFuneralItems() {
    // ���t
    $("#text-funeral-date").val(STRING_EMPTY);
    // �J�n����
    $("#pulldown-funeral-time")[0].__component.reset();
    // ��ꖼ
    $("#text-funeral-placeName").val(STRING_EMPTY);
    // ��ꖼ�i�J�i�j
    $("#text-funeral-placeName_kana").val(STRING_EMPTY);
    // �X�֔ԍ�
    $("#text-funeral-postcode").val(STRING_EMPTY);
    // �s���{���E�s�S��
    $("#text-funeral-address1").val(STRING_EMPTY);
    // �����E�Ԓn
    $("#text-funeral-address2").val(STRING_EMPTY);
    // �������E�����ԍ�
    $("#text-funeral-address3").val(STRING_EMPTY);
    // ���d�b�ԍ�
    $("#text-funeral-tel1").val(STRING_EMPTY);
    $("#text-funeral-tel2").val(STRING_EMPTY);
    $("#text-funeral-tel3").val(STRING_EMPTY);
    $("#text-funeral-tel").val(STRING_EMPTY);
}

/**
 * ���ʎ��F�����E�񊈐���ݒ�
 *
 * @param {boolean} value �ʖ�Ɠ����ꏊ�̑I��l
 * */
function changeFuneralItemsDisabled(value) {
    // �X�֔ԍ� ����
    $("#btn-funeral-postcode").attr("disabled", value);
    // ��ꖼ
    $("#text-funeral-placeName").attr("disabled", value);
    // ��ꖼ�i�J�i�j
    $("#text-funeral-placeName_kana").attr("disabled", value);
    // �X�֔ԍ�
    $("#text-funeral-postcode").attr("disabled", value);
    // �s���{���E�s�S��
    $("#text-funeral-address1").attr("disabled", value);
    // �����E�Ԓn
    $("#text-funeral-address2").attr("disabled", value);
    // �������E�����ԍ�
    $("#text-funeral-address3").attr("disabled", value);
    // ���d�b�ԍ�
    $("#text-funeral-tel1").attr("disabled", value);
    $("#text-funeral-tel2").attr("disabled", value);
    $("#text-funeral-tel3").attr("disabled", value);
}

/**
 * �C�x���g��ݒ肷��
 */
function setEvents() {

    // �}�{�w���v�{�^��
    $("#help-opener-huyo").on("click", function () {
        let modalHelp = $("#modal-help-huyo")[0].__component;
        modalHelp.opened = !0;
        modalHelp.onCloseRequested = function () {
            return modalHelp.opened = !1
        }
    });

    // �����w���v�{�^��
    $("#help-opener-koryo").on("click", function () {
        let modalHelp = $("#modal-help-koryo")[0].__component;
        modalHelp.opened = !0;
        modalHelp.onCloseRequested = function () {
            return modalHelp.opened = !1
        }
    });

    // ���ԃw���v�{�^��
    $("#help-opener-kuge").on("click", function () {
        let modalHelp = $("#modal-help-kuge")[0].__component;
        modalHelp.opened = !0;
        modalHelp.onCloseRequested = function () {
            return modalHelp.opened = !1
        }
    });

    // ���d�w���v�{�^��
    $("#help-opener-tyoden").on("click", function () {
        let modalHelp = $("#modal-help-tyoden")[0].__component;
        modalHelp.opened = !0;
        modalHelp.onCloseRequested = function () {
            return modalHelp.opened = !1
        }
    });

    // �ʖ�/���ʎ��̗L���w���v�{�^��
    $("#help-opener").on("click", function () {
        let modalHelp = $("#modal-help")[0].__component;
        modalHelp.opened = !0;
        modalHelp.onCloseRequested = function () {
            return modalHelp.opened = !1
        }
    });

    // PDF�\���{�^��
    $("#link-page-pdf").on("click", function () {
        let pdfpage = $("#pdf-page")[0].__component;
        pdfpage.opened = !0;
        pdfpage.onCloseRequested = function () {
            return pdfpage.opened = !1
        }
    });

    // �ϔC�҂̎Ј��ԍ�
    $("#text-unhappiness-shainbango").on("change", function () {
        // ����������Z�b�g
        setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    });

    // �r�僉�W�I�{�^��
    $("input[name=radio-mourner]").on("change", function () {
        dtCondolence["ORGANIZER_JUGYOIN_ZOKUGARAKBN"] = this.value;
        setMournerArea();
        // ����������Z�b�g
        setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    });

    // �ʖ�/���ʎ����W�I�{�^��
    $("input[name=radio-funeral]").on("change", function () {
        dtCondolence["TSUYA_KOKUBETSUSHIKIKBN"] = this.value;
        setWakeAndFuneralArea();
        clearWakeItems();
        clearFuneralItems();
        // �\���E��\��
        changeFuneralItemsDisabled(false);
    });

    // ���͂��惉�W�I�{�^��
    $("input[name=radio-allnight]").on("change", function () {
        dtCondolence["TODOKESAKIKBN"] = this.value;
        setAllnightArea();
    });

    // �ʖ�Ɠ����ꏊ
    $("#chk-funeral-sameplace").on("change", function () {

        let value = $(this).prop("checked");

        if (value) {
            // �R�s�[
            copyWakeItemsToFuneral();
        } else {
            // �N���A
            clearFuneralItems();
        }
        // �\���E��\��
        changeFuneralItemsDisabled(value);
    });

    // ���~�{�^��
    $("#btn-page-close").on("click", function () {
        // �_�C�A���O�{�b�N�X
        wfCommon.ShowDialog(DIALOG_CONFIRM, STRING_EMPTY, msg["W0005"], null, backFromPage);
    });

    // �������{�^��
    $("#btn-form-draft_save").on("click", function () {
        // �ꎞ�ۑ��������s���܂�
        var data = saveLogic(BTN_TEMPORARILY_SAVE);

        // ��O����
        if (data.indexOf("err@") === 0) {
            wfCommon.Msgbox(data);
            return;
        }

        // �e��ʁi�\�����j���[�j�ɖ߂邱��
        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, data, null, backFromPage);

    });

    // �m�F�{�^��
    $("#btn-form-confirm").on("click", function () {
        if (wfstate >= WF_STATE_DRAFT) {
            checkInput = $("#form1").getElements();
            if (checkInput === checkRefer) {
                wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["I0010"]);
                return;
            }
        }

        // �`�F�b�N�����{����
        // �P���ڃ`�F�b�N
        let flg = $("#form1").valid();
        if (!flg) {
            return;
        }

        // �������`�F�b�N : �ʖ�/���ʎ�/�����
        let allnight = $("input[name=radio-allnight]:checked").val();
        // ���ԋ敪
        let kugekbn = $("input[name=radio-opener-kuge]:checked").val();
        // ���d�敪
        let tyodenkbn = $("input[name=radio-opener-tyoden]:checked").val();
        // �ʖ�/���ʎ�/�Ȃ�
        let funeral = $("input[name=radio-funeral]:checked").val();
        // ���� �܂��́@���d�@���ꂩ�󂯎��ꍇ
        if (kugekbn === NECESSARY_KBN_HITUYOU || tyodenkbn === NECESSARY_KBN_HITUYOU) {
            // �ʖ�
            if (allnight === LOCATION_TSUYA_KBN) {
                if (!(funeral === FUNERAL_KBN_TSUYA || funeral === FUNERAL_KBN_BOTH)) {
                    wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["I0007"]);
                    return
                }
            }
            // ���ʎ�
            else if (allnight === LOCATION_KOKUBETSUSHIKI_KBN) {
                if (!(funeral === FUNERAL_KBN_KOKUBETSUSHIKI || funeral === FUNERAL_KBN_BOTH)) {
                    wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["I0007"]);
                    return;
                }
            }
        }

        // �������`�F�b�N : ������/�ʖ��/���ʎ���/������
        // ������
        let deathdatetime = moment($("#text-died-death_date").val() + HALF_SPACE + $("#select-died-death_time").val(), DATETIME_FORMAT_MOMENT_PATTERN_1);
        if (deathdatetime.isAfter(moment(wfCommon.getServerDatetime(), DATETIME_FORMAT_MOMENT_PATTERN_1))) {
            wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "������,������"));
            return;
        }
        // �ʖ�G���A�@�\���̏ꍇ
        if ($(".area-wake").is(":visible")) {
            // �ʖ��
            let wakedatetime = moment($("#text-wake-date").val() + HALF_SPACE + $("#select-wake-time").val(), DATETIME_FORMAT_MOMENT_PATTERN_1);
            if (deathdatetime.isAfter(wakedatetime)) {
                wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "�ʖ��,�������̑O"));
                return;
            }
        }
        // ���ʎ��G���A�@�\���̏ꍇ
        if ($(".area-funeral").is(":visible")) {
            // ���ʎ���
            let funeraldatetime = moment($("#text-funeral-date").val() + HALF_SPACE + $("#select-funeral-time").val(), DATETIME_FORMAT_MOMENT_PATTERN_1);
            if (deathdatetime.isAfter(funeraldatetime)) {
                wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "���ʎ���,�������̑O"));
                return;
            }
        }
        // ������G���A�@�\���̏ꍇ
        if ($("#label-standard-info3").is(":hidden")) {
            // ���Ԃ��͂���ꏊ�F�ʖ��I���ꍇ
            if (allnight === LOCATION_TSUYA_KBN) {
                // �ʖ��
                let wakedatetime = moment($("#text-wake-date").val() + HALF_SPACE + $("#select-wake-time").val(), DATETIME_FORMAT_MOMENT_PATTERN_1);
                let serverdatetime = moment(wfCommon.getServerDatetime(), DATETIME_FORMAT_MOMENT_PATTERN_1);
                if (serverdatetime.isAfter(wakedatetime)) {

                    if (dtCondolence["WFState"] === WF_STATE_OVER) {
                        // ���V��ւ̎�z�͂ł��܂���B������I�����������B
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["E0007"]);
                    } else {
                        // �ʖ���ɉߋ����̓��t�͎w��ł��܂���B
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "�ʖ��,�ߋ���"));
                    }
                    return
                }
            }
            // ���Ԃ��͂���ꏊ�F���ʎ���I���ꍇ
            if (allnight === LOCATION_KOKUBETSUSHIKI_KBN) {
                // ���ʎ���
                let funeraldatetime = moment($("#text-funeral-date").val() + HALF_SPACE + $("#select-funeral-time").val(), DATETIME_FORMAT_MOMENT_PATTERN_1);
                let serverdatetime = moment(wfCommon.getServerDatetime(), DATETIME_FORMAT_MOMENT_PATTERN_1);
                if (serverdatetime.isAfter(funeraldatetime)) {
                    if (dtCondolence["WFState"] === WF_STATE_OVER) {
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["E0007"]);
                    } else {
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "���ʎ���,�ߋ���"));
                    }
                    return
                }
            }
            // ���Ԃ��͂���ꏊ�F������I���ꍇ
            if (allnight === LOCATION_ATOKA_KBN) {
                // ������
                let reardate = moment($("#text-rear-date").val(), DATE_FORMAT_MOMENT_PATTERN_1);
                if (reardate.isBefore(moment(wfCommon.getServerDate(), DATE_FORMAT_MOMENT_PATTERN_1))) {
                    wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "���͂���,�ߋ���"));
                    return;
                }
            }

        }

        // �C������Ƃ��ɁA���Ԃƒ��d�͗����I���̃`�F�b�N
        if (wfstate === WF_STATE_OVER) { // �C���̏ꍇ
            if (dtCondolence["KUGE_GOUKEI"] > 0 && dtCondolence["TYOUDEN_GOUKEI"] > 0) { //���Ԃƒ��d�͎g�p�̏ꍇ
                if (dtCondolence["KYOKAKBN"] === parseInt(NECESSARY_KBN_HITUYOU) && dtCondolence["TYODENKBN"] === parseInt(NECESSARY_KBN_HITUYOU)) { // ���Ԃƒ��d�̓X�i�b�v�V���b�g�J�����Ɏ󂯎��̏ꍇ
                    if ($("input[name=radio-opener-kuge]:checked").val() !== $("input[name=radio-opener-tyoden]:checked").val()) { // ���Ԃƒ��d�͑I��l���Ⴄ�ꍇ
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["E0008"]);
                        return;
                    }
                }
            }
        }

        // �d���`�F�b�N
        var count = doubleCheck();
        if (Number(count["Check_Double_Info"]) > 0) {
            wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["I0009"]);
            return;
        }

        // �m�F���
        gotoConfirmPage();

    });

    // �߂�{�^��
    $("#btn-form-back").on("click", function () {
        // ���͉�ʂ֑J��
        $("#confirm-page")[0].__component.opened = !1;
    });

    // �߂�{�^��
    $("#btn-other-back").on("click", function () {
        // �e��ʂ֑J��
        backFromPage();
    });

    // �����{�^��
    $("#btn-search-empcode").on("click", function () {
        // �Ј��ԍ��@�`�F�b�N
        let flg = $("#text-unhappiness-shainbango").valid();

        if (flg) {
            setDelegatorInfo();
        }

    });

    // �\���{�^��
    $("#btn-form-request").on("click", function () {
        // �\���������s���܂��A�\�����ꂽ���e��DB�ɓo�^����
        saveCore(BTN_SUB_SUBMIT);
    });

    // �C���{�^��
    $("#btn-form-modify").on("click", function () {
        // �C���������s���܂��A�\�����ꂽ���e��DB�ɓo�^����
        saveCore(BTN_EDIT);
    });

    // ���ԃ��W�I�{�^��
    $("input[name=radio-opener-kuge]").on("change", function () {
        // ���͂�����G���A
        setAddresseeDisplay();
    });

    // ���d���W�I�{�^��
    $("input[name=radio-opener-tyoden]").on("change", function () {
        // ���͂�����G���A
        setAddresseeDisplay();
    });

    // �A���̎���d�b�ԍ�
    wfCommon.initTelEvent("text-contact-tel");
    // �ʖ�d�b�ԍ�
    wfCommon.initTelEvent("text-wake-tel");
    // ���ʎ��d�b�ԍ�
    wfCommon.initTelEvent("text-funeral-tel");
    // �����d�b�ԍ�
    wfCommon.initTelEvent("text-rear-tel");
}

/**
 * �J�ڐ���w��
 * */
function backFromPage() {

    let pagename = GetQueryString("FromPage");

    if (pagename === null) {
        pagename = "form_applymenu.html";
    }

    window.location.href = "../../../pages/biz/menu/" + pagename;

}

/**
 * �ۑ��E�������M
 * @param {number} key�@���s���[�h
 * */
function saveCore(key) {
    var data = saveLogic(key);

    // ��O����
    if (data.indexOf("err@") === 0) {
        wfCommon.Msgbox(data);
        return;
    }

    // �������M����
    if (key === BTN_SUB_SUBMIT) {    // �\���̏ꍇ
        wfCommon.DoMailSend(AUTO_MAIL_CLASS.CONDOLENCE, MAIL_TYPE_CONDOLENCE, MAIL_STSTE_NEW, GetQueryString("WorkID"));

    } else if (key === BTN_EDIT) {        // �\���C���̏ꍇ
        wfCommon.DoMailSend(AUTO_MAIL_CLASS.CONDOLENCE, MAIL_TYPE_CONDOLENCE, MAIL_STSTE_EDIT, GetQueryString("WorkID"));
    }

    // �e��ʁi�\�����j���[��ʁj�ɖ߂�
    wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, data, null, backFromPage);
}

/**
  * ��ʂɓ��͂�������o�^���ϐ��ɐݒ肷�邱��
  * @param {number} key�@���s���[�h
  *                       �p�����[�^����
  *                       �P�F��o
  *                       �Q�F�ꎞ�ۑ�
  *                       �R�F����
  *                       �S�F���F
  *                       �T�F�۔F
  *                       �U�F����
  *                       �V�F�C��
  */
function saveLogic(key) {

    // �o�b�N�Ώۂ̍쐬
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_AppForm");
    // URL�̒ǉ�
    handler.AddUrlData();

    // �����A�����ݒ�
    saveMainTblData(handler, key);

    // �T�u�e�[�u�����ݒ�
    saveSubListData(handler);

    // ���ʑΏۂ̒ǉ�
    saveCommonData(handler, key);

    // �ݒ��̓��e���o�b�N�ɑ��M���邱��
    data = handler.DoMethodReturnString("Runing_Send");

    return data;
}

/**
  * �o�b�N�ƘA�g���鋤�ʐݒ�
  * @param {HashTblCommon} handler�@�ݒ���Ώ�
  * @param {any} key�@���s���[�h
  */
function saveCommonData(handler, key) {

    var ht = new HashTblCommon();

    //oid�̒l�Ɠ���
    ht.Add("WorkID", GetQueryString("WorkID"));
    //�m�[�hID
    ht.Add("FK_Node", GetQueryString("FK_Node"));
    //�t���[ID
    ht.Add("FK_Flow", GetQueryString("FK_Flow"));
    //���O�C�����[�U�[ID
    ht.Add("UserNo", GetQueryString("UserNo"));
    //SID�̂���
    ht.Add("SID", GetQueryString("SID"));
    //���̃m�[�hID
    ht.Add("NextNode", nextNode);
    // �������F���[�h
    ht.Add("AutoApprovalMode", MODE_KBN_YES);
    // �������F��
    ht.Add("toEmps", AUTO_APPROVAL_ID);
    // �\���敪
    ht.Add("AgentMode", agentMode);
    // ���s���[�h
    ht.Add("Mode", key);
    handler.AddPara("CommonData", ht.stringify());
}

/**
  * �����A�����ݒ�
  * @param {HashTblCommon} handler�@�o�b�N�ƘA�g����Ώ�
  * @param {any} key�@���s���[�h
  */
function saveMainTblData(handler, key) {

    var ht = new HashTblCommon();
    // �����ݒ�
    initMainTblData(ht);
    // �f�[�^�ݒ�
    setMainTblData(ht, key);
    //TT_WF_CONDOLENCE�e�[�u���i�[
    handler.AddPara("MainTblName", "TT_WF_CONDOLENCE");
    handler.AddPara("TT_WF_CONDOLENCE", ht.stringify());
}

/**
  * �o�b�N�ƘA�g����T�u���ʐݒ�
  * @param {HashTblCommon} handler�@�ݒ���Ώ�
  */
function saveSubListData(handler) {

    // �X�V�̃e�[�u����
    var opjTbNm = {};
    handler.AddPara("SubListTblName", JSON.stringify(opjTbNm));
}

/**
  * �����A����񏉊����ݒ�
  * @param {HashTblCommon} ht�@�o�b�N�ƘA�g����Ώ�
  * @param {any} key�@���s���[�h
  */
function initMainTblData(ht, key) {

    if (key === BTN_SUB_SUBMIT || key === BTN_TEMPORARILY_SAVE) { // ��o�@�܂��́@������
        // �`�[�ԍ� - �󔒐ݒ�
        ht.Add("OID", STRING_EMPTY);
        // �\���ҋ敪 - �󔒐ݒ�
        ht.Add("SHINSEISYAKBN", STRING_EMPTY);
        // �㗝�\���ҎЈ��ԍ� - �󔒐ݒ�
        ht.Add("DAIRISHINSNEISYA_SHAINBANGO", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�㗝�\���Ј���(����) - �󔒐ݒ�
        ht.Add("DAIRISHINSNEISYA_MEI", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�㗝�\���Ј���(�t���K�i) - �󔒐ݒ�
        ht.Add("DAIRISHINSNEISYA_FURIGANAMEI", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�㗝�\���Ј������R�[�h - �󔒐ݒ�
        ht.Add("DAIRISHINSNEISYA_SYOZOKUCODE", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�㗝�\���Ј������� - �󔒐ݒ�
        ht.Add("DAIRISHINSNEISYA_SYOZOKU", STRING_EMPTY);
        // �o���t���O - �󔒐ݒ�
        ht.Add("SHUKKOFLG", STRING_EMPTY);
        // �`�[���R�[�h  - �󔒐ݒ�
        ht.Add("TEAMCODE", STRING_EMPTY);
        // �`�[����  - �󔒐ݒ�
        ht.Add("TEAMMEISHO", STRING_EMPTY);
        // ���������R�[�h - �󔒐ݒ�
        ht.Add("ZAIMUBUSHOCODE", STRING_EMPTY);
        // �o��S��ЃR�[�h - �󔒐ݒ�
        ht.Add("KEIHIFUTANKAISHACODE", STRING_EMPTY);
        // �o��S��Ж� - �󔒐ݒ�
        ht.Add("KEIHIFUTANKAISHAMEI", STRING_EMPTY);
        // �s�K�]�ƈ��Ј��ԍ� - �󔒐ݒ�
        ht.Add("UNFORTUNATE_SHAINBANGO", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̎Ј���(�t���K�i) - �󔒐ݒ�
        ht.Add("UNFORTUNATE_FURIGANAMEI", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̎Ј���(����) - �󔒐ݒ�
        ht.Add("UNFORTUNATE_KANJIMEI", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̏o������ЃR�[�h - �󔒐ݒ�
        ht.Add("UNFORTUNATE_KAISYACODE", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̏o������Ж� - �󔒐ݒ�
        ht.Add("UNFORTUNATE_KAISYAMEI", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̏o�����ЃR�[�h - �󔒐ݒ�
        ht.Add("UNFORTUNATE_SYUKOSAKIKAISYACODE", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̏o�����Ж� - �󔒐ݒ�
        ht.Add("UNFORTUNATE_SYUKOSAKIKAISYAMEI", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�҂̏����R�[�h - �󔒐ݒ�
        ht.Add("UNFORTUNATE_SYOZOKUCODE", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̐����g�D�E�� - �󔒐ݒ�
        ht.Add("UNFORTUNATE_SEISHIKISOSHIKIUE", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̉� - �󔒐ݒ�
        ht.Add("UNFORTUNATE_SEISHIKISOSHIKISHITA", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̎Ј��敪�R�[�h - �󔒐ݒ�
        ht.Add("UNFORTUNATE_SYAINNKBNCODE", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̎Ј��敪�� - �󔒐ݒ�
        ht.Add("UNFORTUNATE_SYAINNKBN", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̐E�ʖ� - �󔒐ݒ�
        ht.Add("UNFORTUNATE_SYOKUIKBN", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̑g���敪�R�[�h - �󔒐ݒ�
        ht.Add("UNFORTUNATE_KUMIAIKBNCODE", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̑g���敪�� - �󔒐ݒ�
        ht.Add("UNFORTUNATE_KUMIAIKBN", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̃O�b�h���C�t�敪�R�[�h - �󔒐ݒ�
        ht.Add("UNFORTUNATE_GLCKBNCODE", STRING_EMPTY);
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̃O�b�h���C�t�敪�� - �󔒐ݒ�
        ht.Add("UNFORTUNATE_GLCKBN", STRING_EMPTY);
        // �S���Ȃ�ꂽ���J�i�����i���j - �󔒐ݒ�
        ht.Add("DEAD_KANASHIMEI_SEI", STRING_EMPTY);
        // �S���Ȃ�ꂽ���J�i�����i���j - �󔒐ݒ�
        ht.Add("DEAD_KANASHIMEI_MEI", STRING_EMPTY);
        // �S���Ȃ�ꂽ�������i���j - �󔒐ݒ�
        ht.Add("DEAD_SHIMEI_SEI", STRING_EMPTY);
        // �S���Ȃ�ꂽ�������i���j - �󔒐ݒ�
        ht.Add("DEAD_SHIMEI_MEI", STRING_EMPTY);
        // �S���Ȃ�ꂽ�������敪  - �󔒐ݒ�
        ht.Add("DEAD_JUGYOIN_ZOKUGARAKBN", STRING_EMPTY);
        // �S���Ȃ�ꂽ������ - �󔒐ݒ�
        ht.Add("DEAD_SEIBETSU", STRING_EMPTY);
        // �S���Ȃ�ꂽ�������ʋ��敪 - �󔒐ݒ�
        ht.Add("DEAD_DOKYO_BEKYO", STRING_EMPTY);
        // �S���Ȃ�ꂽ���N�� - �󔒐ݒ�
        ht.Add("DEAD_NENREI", STRING_EMPTY);
        // �S���Ȃ�ꂽ�������� - �󔒐ݒ�
        ht.Add("DEAD_DATE", STRING_EMPTY);
        // �S���Ȃ�ꂽ�������� - �󔒐ݒ�
        ht.Add("DEAD_TIME", STRING_EMPTY);
        // �S���Ȃ�ꂽ���}�{�敪 - �󔒐ݒ�
        ht.Add("DEAD_FUYOUKBN", STRING_EMPTY);
        // �r��敪 - �󔒐ݒ�
        ht.Add("ORGANIZER_JUGYOIN_ZOKUGARAKBN", STRING_EMPTY);
        // �r��J�i�����i���j - �󔒐ݒ�
        ht.Add("ORGANIZER_KANASHIMEI_SEI", STRING_EMPTY);
        // �r��J�i�����i���j - �󔒐ݒ�
        ht.Add("ORGANIZER_KANASHIMEI_MEI", STRING_EMPTY);
        // �r�厁���i���j - �󔒐ݒ�
        ht.Add("ORGANIZER_SHIMEI_SEI", STRING_EMPTY);
        // �r�厁���i���j - �󔒐ݒ�
        ht.Add("ORGANIZER_SHIMEI_MEI", STRING_EMPTY);
    }

    // �\���ҘA����d�b�ԍ� - �󔒐ݒ�
    ht.Add("RENRAKUSAKITEL", STRING_EMPTY);
    // �\���ҘA���惁�[�� - �󔒐ݒ�
    ht.Add("RENRAKUSAKIMAIL", STRING_EMPTY);
    // �ʖ�/���ʎ��敪 - �󔒐ݒ�
    ht.Add("TSUYA_KOKUBETSUSHIKIKBN", STRING_EMPTY);
    // �ʖ���t���K�i - �󔒐ݒ�
    ht.Add("TSUYA_BASYOUFURIGANA", STRING_EMPTY);
    // �ʖ��ꖼ  - �󔒐ݒ�
    ht.Add("TSUYA_BASYOUMEI", STRING_EMPTY);
    // �ʖ�X�֔ԍ� - �󔒐ݒ�
    ht.Add("TSUYA_YUBINBANGO", STRING_EMPTY);
    // �ʖ�s���{���E�s�S�� - �󔒐ݒ�
    ht.Add("TSUYA_ADDRESS1", STRING_EMPTY);
    // �ʖ钬���Ԓn - �󔒐ݒ�
    ht.Add("TSUYA_ADDRESS2", STRING_EMPTY);
    // �ʖ�}���V������ - �󔒐ݒ�
    ht.Add("TSUYA_ADDRESS3", STRING_EMPTY);
    // �ʖ���t - �󔒐ݒ�
    ht.Add("TSUYA_DATE", STRING_EMPTY);
    // �ʖ鎞�� - �󔒐ݒ�
    ht.Add("TSUYA_TIME", STRING_EMPTY);
    // �ʖ�A����d�b�ԍ� - �󔒐ݒ�
    ht.Add("TSUYA_RENRAKUSAKITEL", STRING_EMPTY);
    // ���ʎ����t���K�i - �󔒐ݒ�
    ht.Add("KOKUBETSUSHIKI_BASYOUFURIGANA", STRING_EMPTY);
    // ���ʎ���ꖼ - �󔒐ݒ�
    ht.Add("KOKUBETSUSHIKI_BASYOUMEI", STRING_EMPTY);
    // ���ʎ��X�֔ԍ� - �󔒐ݒ�
    ht.Add("KOKUBETSUSHIKI_YUBINBANGO", STRING_EMPTY);
    // ���ʎ��s���{���E�s�S�� - �󔒐ݒ�
    ht.Add("KOKUBETSUSHIKI_ADDRESS1", STRING_EMPTY);
    // ���ʎ������Ԓn - �󔒐ݒ�
    ht.Add("KOKUBETSUSHIKI_ADDRESS2", STRING_EMPTY);
    // ���ʎ��}���V������ - �󔒐ݒ�
    ht.Add("KOKUBETSUSHIKI_ADDRESS3", STRING_EMPTY);
    // ���ʎ����t - �󔒐ݒ�
    ht.Add("KOKUBETSUSHIKI_DATE", STRING_EMPTY);
    // ���ʎ����� - �󔒐ݒ�
    ht.Add("KOKUBETSUSHIKI_TIME", STRING_EMPTY);
    // ���ʎ��A����d�b�ԍ� - �󔒐ݒ�
    ht.Add("KOKUBETSUSHIKI_RENRAKUSAKITEL", STRING_EMPTY);
    // �ʖ�Ɠ����敪 - �󔒐ݒ�
    ht.Add("TSUYA_SAME_KBN", STRING_EMPTY);
    // ��ʎQ������ނ���敪 - �󔒐ݒ�
    ht.Add("SANRETSU_JITAI_KBN", STRING_EMPTY);
    // ���ԓ͂���ꏊ�敪 - �󔒐ݒ�
    ht.Add("TODOKESAKIKBN", STRING_EMPTY);
    // ����薼�O - �󔒐ݒ�
    ht.Add("ATOKAZARI_FULLNAME", STRING_EMPTY);
    // �����X�֔ԍ� - �󔒐ݒ�
    ht.Add("ATOKAZARI_YUBINBANGO", STRING_EMPTY);
    // �����s���{���E�s�S�� - �󔒐ݒ�
    ht.Add("ATOKAZARI_ADDRESS1", STRING_EMPTY);
    // ����蒬���Ԓn - �󔒐ݒ�
    ht.Add("ATOKAZARI_ADDRESS2", STRING_EMPTY);
    // �����}���V������ - �󔒐ݒ�
    ht.Add("ATOKAZARI_ADDRESS3", STRING_EMPTY);
    // �����d�b�ԍ� - �󔒐ݒ�
    ht.Add("ATOKAZARI_RENRAKUSAKITEL", STRING_EMPTY);
    // �������t - �󔒐ݒ�
    ht.Add("ATOKAZARI_DATE", STRING_EMPTY);
    // �������s�敪 - �󔒐ݒ�
    ht.Add("KORYOKBN", STRING_EMPTY);
    // GLC�����ύX�敪 - �󔒐ݒ�
    ht.Add("GLCKORYOKBN", STRING_EMPTY);
    // ���Ԕ��s�敪 - �󔒐ݒ�
    ht.Add("KYOKAKBN", STRING_EMPTY);
    // ���d���s�敪 - �󔒐ݒ�
    ht.Add("TYODENKBN", STRING_EMPTY);
    // �������v - �󔒐ݒ�
    ht.Add("KOURYOU_GOUKEI", STRING_EMPTY);
    // ���ԍ��v - �󔒐ݒ�
    ht.Add("KUGE_GOUKEI", STRING_EMPTY);
    // ���d���v - �󔒐ݒ�
    ht.Add("TYOUDEN_GOUKEI", STRING_EMPTY);
    // �����\���� - �󔒐ݒ�
    ht.Add("KOURYOU_SHINNSEIBI", STRING_EMPTY);
    // ���ԁE���d�\���� - �󔒐ݒ�
    ht.Add("KUGE_TYOUDEN_SHINNSEIBI", STRING_EMPTY);
    // �����E���ԁE���d���o�l_��Ж��i�o�����j - �󔒐ݒ�
    ht.Add("SASHIDASHI_MOTO_KAISYA1", STRING_EMPTY);
    // �����E���ԁE���d���o�l_��Б�\�Ҍ����� - �󔒐ݒ�
    ht.Add("SASHIDASHI_MOTO_KAISYA2", STRING_EMPTY);
    // �����E���ԁE���d���o�l_��Б�\�Ҏ��� - �󔒐ݒ�
    ht.Add("SASHIDASHI_MOTO_KAISYA3", STRING_EMPTY);
    // �o������Ђ�荁�� - �󔒐ݒ�
    ht.Add("KOURYOU_MOTO_KAISYA", STRING_EMPTY);
    // �o������Ђ�苟�Ԃ̐� - �󔒐ݒ�
    ht.Add("KUGE_MOTO_KAISYA", STRING_EMPTY);
    // �o������Ђ�蒢�d�L�� - �󔒐ݒ�
    ht.Add("TYOUDEN_MOTO_KAISYA", STRING_EMPTY);
    // �����E���ԁE���d���o�l_�J���g�����i�o�����j - �󔒐ݒ�
    ht.Add("SASHIDASHI_MOTO_KUMIAI1", STRING_EMPTY);
    // �����E���ԁE���d���o�l_�J���g����\�҂̌����� - �󔒐ݒ�
    ht.Add("SASHIDASHI_MOTO_KUMIAI2", STRING_EMPTY);
    // �����E���ԁE���d���o�l_�J���g����\�҂̎��� - �󔒐ݒ�
    ht.Add("SASHIDASHI_MOTO_KUMIAI3", STRING_EMPTY);
    // �o�����J���g����荁�� - �󔒐ݒ�
    ht.Add("KOURYOU_MOTO_KUMIAI", STRING_EMPTY);
    // �o�����J���g����苟�Ԃ̐� - �󔒐ݒ�
    ht.Add("KUGE_MOTO_KUMIAI", STRING_EMPTY);
    // �o�����J���g����蒢�d�L�� - �󔒐ݒ�
    ht.Add("TYOUDEN_MOTO_KUMIAI", STRING_EMPTY);
    // �����E���ԁE���d���o�l_��Ж��i�o����j - �󔒐ݒ�
    ht.Add("SASHIDASHI_SAKI_KAISYA1", STRING_EMPTY);
    // �����E���ԁE���d���o�l_��Б�\�Ҍ������i�o����j - �󔒐ݒ�
    ht.Add("SASHIDASHI_SAKI_KAISYA2", STRING_EMPTY);
    // �����E���ԁE���d���o�l_��Б�\�Ҏ����i�o����j - �󔒐ݒ�
    ht.Add("SASHIDASHI_SAKI_KAISYA3", STRING_EMPTY);
    // �o�����Ђ�荁�� - �󔒐ݒ�
    ht.Add("KOURYOU_SAKI_KAISYA", STRING_EMPTY);
    // �o�����Ђ�苟�Ԃ̐� - �󔒐ݒ�
    ht.Add("KUGE_SAKI_KAISYA", STRING_EMPTY);
    // �o�����Ђ�蒢�d�L�� - �󔒐ݒ�
    ht.Add("TYOUDEN_SAKI_KAISYA", STRING_EMPTY);
    // �������v - �󔒐ݒ�
    ht.Add("KOURYOU_GOUKEI", STRING_EMPTY);
    // ���ԍ��v - �󔒐ݒ�
    ht.Add("KUGE_GOUKEI", STRING_EMPTY);
    // ���d���v - �󔒐ݒ�
    ht.Add("TYOUDEN_GOUKEI", STRING_EMPTY);
    // �T�}�[���[ - �󔒐ݒ�
    ht.Add("SUMMRY", STRING_EMPTY);
    // PDFURL - �󔒐ݒ�
    ht.Add("PDF_URL", STRING_EMPTY);
}

/**
  * �����A����� �ݒ�
  * @param {HashTblCommon} ht�@�o�b�N�ƘA�g����Ώ�
  * @param {any} key�@���s���[�h
  */
function setMainTblData(ht, key) {

    if (key === BTN_SUB_SUBMIT || key === BTN_TEMPORARILY_SAVE) { // ��o�@�܂��́@������
        // �`�[�ԍ�
        ht.ObjArr["OID"] = GetQueryString("WorkID");
        // �\���ҋ敪
        ht.ObjArr["SHINSEISYAKBN"] = agentMode;
        // �㗝�\���ҎЈ��ԍ�
        ht.ObjArr["DAIRISHINSNEISYA_SHAINBANGO"] = dtCondolence["DAIRISHINSNEISYA_SHAINBANGO"];
        // (�X�i�b�v�V���b�g)�㗝�\���Ј���(����)
        ht.ObjArr["DAIRISHINSNEISYA_MEI"] = dtCondolence["DAIRISHINSNEISYA_MEI"];
        // (�X�i�b�v�V���b�g)�㗝�\���Ј���(�t���K�i)
        ht.ObjArr["DAIRISHINSNEISYA_FURIGANAMEI"] = dtCondolence["DAIRISHINSNEISYA_FURIGANAMEI"];
        // (�X�i�b�v�V���b�g)�㗝�\���Ј������R�[�h
        ht.ObjArr["DAIRISHINSNEISYA_SYOZOKUCODE"] = dtCondolence["DAIRISHINSNEISYA_SYOZOKUCODE"];
        // (�X�i�b�v�V���b�g)�㗝�\���Ј�������
        ht.ObjArr["DAIRISHINSNEISYA_SYOZOKU"] = dtCondolence["DAIRISHINSNEISYA_SYOZOKU"];
        // �o���t���O
        ht.ObjArr["SHUKKOFLG"] = dtCondolence["SHUKKOFLG"];
        // �`�[���R�[�h
        ht.ObjArr["TEAMCODE"] = dtCondolence["TEAMCODE"];
        // �`�[����
        ht.ObjArr["TEAMMEISHO"] = dtCondolence["TEAMMEISHO"];
        // ���������R�[�h
        ht.ObjArr["ZAIMUBUSHOCODE"] = dtCondolence["ZAIMUBUSHOCODE"];
        // �o��S��ЃR�[�h
        ht.ObjArr["KEIHIFUTANKAISHACODE"] = dtCondolence["KEIHIFUTANKAISHACODE"];
        // �o��S��Ж�
        ht.ObjArr["KEIHIFUTANKAISHAMEI"] = dtCondolence["KEIHIFUTANKAISHAMEI"];
        // �s�K�]�ƈ��Ј��ԍ�
        ht.ObjArr["UNFORTUNATE_SHAINBANGO"] = dtCondolence["UNFORTUNATE_SHAINBANGO"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̎Ј���(�t���K�i)
        ht.ObjArr["UNFORTUNATE_FURIGANAMEI"] = dtCondolence["UNFORTUNATE_FURIGANAMEI"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̎Ј���(����)
        ht.ObjArr["UNFORTUNATE_KANJIMEI"] = dtCondolence["UNFORTUNATE_KANJIMEI"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̏o������ЃR�[�h
        ht.ObjArr["UNFORTUNATE_KAISYACODE"] = dtCondolence["UNFORTUNATE_KAISYACODE"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̏o������Ж�
        ht.ObjArr["UNFORTUNATE_KAISYAMEI"] = dtCondolence["UNFORTUNATE_KAISYAMEI"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̏o�����ЃR�[�h
        ht.ObjArr["UNFORTUNATE_SYUKOSAKIKAISYACODE"] = dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYACODE"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̏o�����Ж�
        ht.ObjArr["UNFORTUNATE_SYUKOSAKIKAISYAMEI"] = dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYAMEI"];
        // (�X�i�b�v�V���b�g)�s�K�҂̏����R�[�h
        ht.ObjArr["UNFORTUNATE_SYOZOKUCODE"] = dtCondolence["UNFORTUNATE_SYOZOKUCODE"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̐����g�D�E��
        ht.ObjArr["UNFORTUNATE_SEISHIKISOSHIKIUE"] = dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̉�
        ht.ObjArr["UNFORTUNATE_SEISHIKISOSHIKISHITA"] = dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̎Ј��敪�R�[�h
        ht.ObjArr["UNFORTUNATE_SYAINNKBNCODE"] = dtCondolence["UNFORTUNATE_SYAINNKBNCODE"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̎Ј��敪��
        ht.ObjArr["UNFORTUNATE_SYAINNKBN"] = dtCondolence["UNFORTUNATE_SYAINNKBN"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̐E�ʖ�
        ht.ObjArr["UNFORTUNATE_SYOKUIKBN"] = dtCondolence["UNFORTUNATE_SYOKUIKBN"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̑g���敪�R�[�h
        ht.ObjArr["UNFORTUNATE_KUMIAIKBNCODE"] = dtCondolence["UNFORTUNATE_KUMIAIKBNCODE"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̑g���敪��
        ht.ObjArr["UNFORTUNATE_KUMIAIKBN"] = dtCondolence["UNFORTUNATE_KUMIAIKBN"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̃O�b�h���C�t�敪�R�[�h
        ht.ObjArr["UNFORTUNATE_GLCKBNCODE"] = dtCondolence["UNFORTUNATE_GLCKBNCODE"];
        // (�X�i�b�v�V���b�g)�s�K�]�ƈ��̃O�b�h���C�t�敪��
        ht.ObjArr["UNFORTUNATE_GLCKBN"] = dtCondolence["UNFORTUNATE_GLCKBN"];
        // �S���Ȃ�ꂽ���J�i�����i���j
        ht.ObjArr["DEAD_KANASHIMEI_SEI"] = $("#text-died-lastname_kana").val();
        // �S���Ȃ�ꂽ���J�i�����i���j
        ht.ObjArr["DEAD_KANASHIMEI_MEI"] = $("#text-died-firstname_kana").val();
        // �S���Ȃ�ꂽ�������i���j
        ht.ObjArr["DEAD_SHIMEI_SEI"] = $("#text-died-lastname").val();
        // �S���Ȃ�ꂽ�������i���j
        ht.ObjArr["DEAD_SHIMEI_MEI"] = $("#text-died-firstname").val();
        // �S���Ȃ�ꂽ�������敪 
        ht.ObjArr["DEAD_JUGYOIN_ZOKUGARAKBN"] = $("#select-died-relationship option:selected").val();
        // �S���Ȃ�ꂽ������
        ht.ObjArr["DEAD_SEIBETSU"] = $("input[name=radio-died-gender]:checked").val();
        // �S���Ȃ�ꂽ�������ʋ��敪
        ht.ObjArr["DEAD_DOKYO_BEKYO"] = $("input[name=radio-died-living]:checked").val();
        // �S���Ȃ�ꂽ���N��
        ht.ObjArr["DEAD_NENREI"] = $("#text-died-age").val();
        // �S���Ȃ�ꂽ��������
        ht.ObjArr["DEAD_DATE"] = $("#text-died-death_date").val();
        // �S���Ȃ�ꂽ��������
        ht.ObjArr["DEAD_TIME"] = $("#select-died-death_time option:selected").val();
        // �S���Ȃ�ꂽ���}�{�敪
        ht.ObjArr["DEAD_FUYOUKBN"] = $("input[name=radio-died-dependent]:checked").val();
        // �r��敪
        ht.ObjArr["ORGANIZER_JUGYOIN_ZOKUGARAKBN"] = $("input[name=radio-mourner]:checked").val();

        if (dtCondolence["ORGANIZER_JUGYOIN_ZOKUGARAKBN"] === ORGANIZER_KBN_HONNIN_IGAI) { // �����ȊO
            // �r��J�i�����i���j
            ht.ObjArr["ORGANIZER_KANASHIMEI_SEI"] = $("#text-mourner-lastname_kana").val();
            // �r��J�i�����i���j
            ht.ObjArr["ORGANIZER_KANASHIMEI_MEI"] = $("#text-mourner-firstname_kana").val();
            // �r�厁���i���j
            ht.ObjArr["ORGANIZER_SHIMEI_SEI"] = $("#text-mourner-lastname").val();
            // �r�厁���i���j
            ht.ObjArr["ORGANIZER_SHIMEI_MEI"] = $("#text-mourner-firstname").val();
        } else {
            // ����
            let kanjimei = dtCondolence["UNFORTUNATE_KANJIMEI"];
            if (kanjimei !== null) {
                kanjimei = kanjimei.split(FULL_SPACE);
                // �r�厁���i���j
                ht.ObjArr["ORGANIZER_SHIMEI_SEI"] = kanjimei[0];
                // �r�厁���i���j
                ht.ObjArr["ORGANIZER_SHIMEI_MEI"] = kanjimei[1];
            }
            // �����i�J�i�j
            let furiganamei = dtCondolence["UNFORTUNATE_FURIGANAMEI"]
            if (furiganamei !== null) {
                furiganamei = furiganamei.split(FULL_SPACE);
                // �r��J�i�����i���j
                ht.ObjArr["ORGANIZER_KANASHIMEI_SEI"] = furiganamei[0];
                // �r��J�i�����i���j
                ht.ObjArr["ORGANIZER_KANASHIMEI_MEI"] = furiganamei[1];
            }
        }

    }

    // �\���ҘA����d�b�ԍ�
    ht.ObjArr["RENRAKUSAKITEL"] = $("#text-contact-tel").val();
    // �\���ҘA���惁�[��
    ht.ObjArr["RENRAKUSAKIMAIL"] = $("#text-contact-email").val();
    // �ʖ�/���ʎ��敪
    let funeralkbn = $("input[name=radio-funeral]:checked").val();
    ht.ObjArr["TSUYA_KOKUBETSUSHIKIKBN"] = funeralkbn;
    // �ʖ�/���ʎ��@�܂��́@�ʖ�
    if (funeralkbn === FUNERAL_KBN_BOTH || funeralkbn === FUNERAL_KBN_TSUYA) {
        // �ʖ���t���K�i
        ht.ObjArr["TSUYA_BASYOUFURIGANA"] = $("#text-wake-placeName_kana").val();
        // �ʖ��ꖼ 
        ht.ObjArr["TSUYA_BASYOUMEI"] = $("#text-wake-placeName").val();
        // �ʖ�X�֔ԍ�
        ht.ObjArr["TSUYA_YUBINBANGO"] = $("#text-wake-postcode").val();
        // �ʖ�s���{���E�s�S��
        ht.ObjArr["TSUYA_ADDRESS1"] = $("#text-wake-address1").val();
        // �ʖ钬���Ԓn
        ht.ObjArr["TSUYA_ADDRESS2"] = $("#text-wake-address2").val();
        // �ʖ�}���V������
        ht.ObjArr["TSUYA_ADDRESS3"] = $("#text-wake-address3").val();
        // �ʖ���t
        ht.ObjArr["TSUYA_DATE"] = $("#text-wake-date").val();
        // �ʖ鎞��
        ht.ObjArr["TSUYA_TIME"] = $("#select-wake-time option:selected").val();
        // �ʖ�A����d�b�ԍ�
        ht.ObjArr["TSUYA_RENRAKUSAKITEL"] = $("#text-wake-tel").val();
    }
    // �ʖ�/���ʎ��@�܂��́@���ʎ�
    if (funeralkbn === FUNERAL_KBN_BOTH || funeralkbn === FUNERAL_KBN_KOKUBETSUSHIKI) {
        // ���ʎ����t���K�i
        ht.ObjArr["KOKUBETSUSHIKI_BASYOUFURIGANA"] = $("#text-funeral-placeName_kana").val();
        // ���ʎ���ꖼ
        ht.ObjArr["KOKUBETSUSHIKI_BASYOUMEI"] = $("#text-funeral-placeName").val();
        // ���ʎ��X�֔ԍ�
        ht.ObjArr["KOKUBETSUSHIKI_YUBINBANGO"] = $("#text-funeral-postcode").val();
        // ���ʎ��s���{���E�s�S��
        ht.ObjArr["KOKUBETSUSHIKI_ADDRESS1"] = $("#text-funeral-address1").val();
        // ���ʎ������Ԓn
        ht.ObjArr["KOKUBETSUSHIKI_ADDRESS2"] = $("#text-funeral-address2").val();
        // ���ʎ��}���V������
        ht.ObjArr["KOKUBETSUSHIKI_ADDRESS3"] = $("#text-funeral-address3").val();
        // ���ʎ����t
        ht.ObjArr["KOKUBETSUSHIKI_DATE"] = $("#text-funeral-date").val();
        // ���ʎ�����
        ht.ObjArr["KOKUBETSUSHIKI_TIME"] = $("#select-funeral-time option:selected").val();
        // ���ʎ��A����d�b�ԍ�
        ht.ObjArr["KOKUBETSUSHIKI_RENRAKUSAKITEL"] = $("#text-funeral-tel").val();
    }
    // �Ȃ�
    if (funeralkbn !== FUNERAL_KBN_NONE) {
        // ��ʎQ������ނ���敪
        ht.ObjArr["SANRETSU_JITAI_KBN"] = $("#chk-funeral-decline").prop("checked") === true ? CHECKBOX_CHECKED : CHECKBOX_UNCHECKED;
    }
    // �ʖ�/���ʎ�
    if (funeralkbn === FUNERAL_KBN_BOTH) {
        // �ʖ�Ɠ����敪
        ht.ObjArr["TSUYA_SAME_KBN"] = $("#chk-funeral-sameplace").prop("checked") === true ? CHECKBOX_CHECKED : CHECKBOX_UNCHECKED;
    }
    // ���ԓ͂���ꏊ�敪
    let allnight = $("input[name=radio-allnight]:checked").val();
    ht.ObjArr["TODOKESAKIKBN"] = allnight;
    if (String(allnight) === LOCATION_ATOKA_KBN) {
        // ����薼�O
        ht.ObjArr["ATOKAZARI_FULLNAME"] = $("#text-rear-name").val();
        // �����X�֔ԍ�
        ht.ObjArr["ATOKAZARI_YUBINBANGO"] = $("#text-rear-postcode").val();
        // �����s���{���E�s�S��
        ht.ObjArr["ATOKAZARI_ADDRESS1"] = $("#text-rear-address1").val();
        // ����蒬���Ԓn
        ht.ObjArr["ATOKAZARI_ADDRESS2"] = $("#text-rear-address2").val();
        // �����}���V������
        ht.ObjArr["ATOKAZARI_ADDRESS3"] = $("#text-rear-address3").val();
        // �����d�b�ԍ�
        ht.ObjArr["ATOKAZARI_RENRAKUSAKITEL"] = $("#text-rear-tel").val();
        // �������t
        ht.ObjArr["ATOKAZARI_DATE"] = $("#text-rear-date").val();
    }

    var koryokbn = $("input[name=radio-opener-koryo]:checked").val();
    var kugekbn = $("input[name=radio-opener-kuge]:checked").val();
    var tyodenkbn = $("input[name=radio-opener-tyoden]:checked").val();
    // �������s�敪
    ht.ObjArr["KORYOKBN"] = koryokbn;
    // GLC�����ύX�敪
    ht.ObjArr["GLCKORYOKBN"] = koryokbn;
    // ���Ԕ��s�敪
    ht.ObjArr["KYOKAKBN"] = kugekbn;
    // ���d���s�敪
    ht.ObjArr["TYODENKBN"] = tyodenkbn;
    // �����E���ԁE���d���o�l_��Ж��i�o�����j
    ht.ObjArr["SASHIDASHI_MOTO_KAISYA1"] = dtCondolence["SASHIDASHI_MOTO_KAISYA1"];
    // �����E���ԁE���d���o�l_��Б�\�Ҍ�����
    ht.ObjArr["SASHIDASHI_MOTO_KAISYA2"] = dtCondolence["SASHIDASHI_MOTO_KAISYA2"];
    // �����E���ԁE���d���o�l_��Б�\�Ҏ���
    ht.ObjArr["SASHIDASHI_MOTO_KAISYA3"] = dtCondolence["SASHIDASHI_MOTO_KAISYA3"];
    // �o������Ђ�荁��
    ht.ObjArr["KOURYOU_MOTO_KAISYA"] = dtCondolence["KOURYOU_MOTO_KAISYA"];
    // �o������Ђ�苟�Ԃ̐�
    ht.ObjArr["KUGE_MOTO_KAISYA"] = dtCondolence["KUGE_MOTO_KAISYA"];
    // �o������Ђ�蒢�d�L��
    ht.ObjArr["TYOUDEN_MOTO_KAISYA"] = dtCondolence["TYOUDEN_MOTO_KAISYA"];
    // �����E���ԁE���d���o�l_�J���g�����i�o�����j
    ht.ObjArr["SASHIDASHI_MOTO_KUMIAI1"] = dtCondolence["SASHIDASHI_MOTO_KUMIAI1"];
    // �����E���ԁE���d���o�l_�J���g����\�҂̌�����
    ht.ObjArr["SASHIDASHI_MOTO_KUMIAI2"] = dtCondolence["SASHIDASHI_MOTO_KUMIAI2"];
    // �����E���ԁE���d���o�l_�J���g����\�҂̎���
    ht.ObjArr["SASHIDASHI_MOTO_KUMIAI3"] = dtCondolence["SASHIDASHI_MOTO_KUMIAI3"];
    // �o�����J���g����荁��
    ht.ObjArr["KOURYOU_MOTO_KUMIAI"] = dtCondolence["KOURYOU_MOTO_KUMIAI"];
    // �o�����J���g����苟�Ԃ̐�
    ht.ObjArr["KUGE_MOTO_KUMIAI"] = dtCondolence["KUGE_MOTO_KUMIAI"];
    // �o�����J���g����蒢�d�L��
    ht.ObjArr["TYOUDEN_MOTO_KUMIAI"] = dtCondolence["TYOUDEN_MOTO_KUMIAI"];
    // �����E���ԁE���d���o�l_��Ж��i�o����j
    ht.ObjArr["SASHIDASHI_SAKI_KAISYA1"] = dtCondolence["SASHIDASHI_SAKI_KAISYA1"];
    // �����E���ԁE���d���o�l_��Б�\�Ҍ������i�o����j
    ht.ObjArr["SASHIDASHI_SAKI_KAISYA2"] = dtCondolence["SASHIDASHI_SAKI_KAISYA2"];
    // �����E���ԁE���d���o�l_��Б�\�Ҏ����i�o����j
    ht.ObjArr["SASHIDASHI_SAKI_KAISYA3"] = dtCondolence["SASHIDASHI_SAKI_KAISYA3"];
    // �o�����Ђ�荁��
    ht.ObjArr["KOURYOU_SAKI_KAISYA"] = dtCondolence["KOURYOU_SAKI_KAISYA"];
    // �o�����Ђ�苟�Ԃ̐�
    ht.ObjArr["KUGE_SAKI_KAISYA"] = dtCondolence["KUGE_SAKI_KAISYA"];
    // �o�����Ђ�蒢�d�L��
    ht.ObjArr["TYOUDEN_SAKI_KAISYA"] = dtCondolence["TYOUDEN_SAKI_KAISYA"];
    // �������v
    ht.ObjArr["KOURYOU_GOUKEI"] = dtCondolence["KOURYOU_GOUKEI"];
    // ���ԍ��v
    ht.ObjArr["KUGE_GOUKEI"] = dtCondolence["KUGE_GOUKEI"];
    // ���d���v
    ht.ObjArr["TYOUDEN_GOUKEI"] = dtCondolence["TYOUDEN_GOUKEI"];
    // �T�}�[���[
    ht.ObjArr["SUMMRY"] = setSummry();
    // PDFURL
    ht.ObjArr["PDF_URL"] = dtCondolence["PDF_URL"];


    // ��z��Ԃ̍X�V
    if (key === BTN_SUB_SUBMIT || key === BTN_EDIT) {

        if (dtCondolence["KOURYOU_GOUKEI"] <= 0 && dtCondolence["KUGE_GOUKEI"] <= 0 && dtCondolence["TYOUDEN_GOUKEI"] <= 0) {
            // ���t���e�����݂��Ȃ��ꍇ
            // ���ԓ͂���ꏊ�敪
            ht.ObjArr['TODOKESAKIKBN'] = null;
            // GLC�����ύX�敪
            ht.ObjArr['GLCKORYOKBN'] = null;
            // �������s�敪
            ht.ObjArr['KORYOKBN'] = null;
            // ���Ԕ��s�敪
            ht.ObjArr['KYOKAKBN'] = null;
            // ���d���s�敪
            ht.ObjArr['TYODENKBN'] = null;
            // �����\����
            ht.ObjArr['KOURYOU_SHINNSEIBI'] = null;
            // ���ԁE���d�\����
            ht.ObjArr['KUGE_TYOUDEN_SHINNSEIBI'] = null;
            // �������v
            ht.ObjArr['KOURYOU_GOUKEI'] = null;
            // ���ԍ��v
            ht.ObjArr['KUGE_GOUKEI'] = null;
            // ���d���v
            ht.ObjArr['TYOUDEN_GOUKEI'] = null;
        }

        // �����́u�󂯎��v��I���ꍇ
        if (koryokbn === NECESSARY_KBN_HITUYOU) {
            // �����\����
            ht.ObjArr["KOURYOU_SHINNSEIBI"] = wfCommon.getServerDatetime();
        } else {
            // �����\����
            ht.ObjArr['KOURYOU_SHINNSEIBI'] = null;
        }

        // ����/���d�́u�󂯎��v��I���ꍇ
        if (kugekbn === NECESSARY_KBN_HITUYOU || tyodenkbn === NECESSARY_KBN_HITUYOU) {
            // ��z��Ԃ́unull�v/�u��z�s�\�v/�u�L�����Z���v�̏ꍇ
            if (dtCondolence["TEHAIKBN"] === null || dtCondolence["TEHAIKBN"] === parseInt(STATE_TEHAIFUNO) || dtCondolence["TEHAIKBN"] === parseInt(STATE_CANCEL) || dtCondolence["TEHAIKBN"] === parseInt(STATE_CANCEL_PAID)) {
                if (dtCondolence["KUGE_GOUKEI"] > 0 && dtCondolence["TYOUDEN_GOUKEI"] > 0) { // ���Ԃƒ��d�͎g�p�̏ꍇ
                    // ���ԁE���d��z���
                    ht.ObjArr["TEHAIKBN"] = STATE_MITEHAI;
                    // ���ԁE���d�\����
                    ht.ObjArr["KUGE_TYOUDEN_SHINNSEIBI"] = wfCommon.getServerDatetime();
                }
            }
        } else {
            // ���ԓ͂���ꏊ�敪
            ht.ObjArr['TODOKESAKIKBN'] = null;
            // ���ԁE���d�\����
            ht.ObjArr['KUGE_TYOUDEN_SHINNSEIBI'] = null;
        }
    }
}

/**
 * �T�}�[���[��ݒ�
 * 
 * */
function setSummry() {

    let summry = { "AgentMode": agentMode, "AutoApprovalMode": MODE_KBN_YES, "content": [] }
    // �X�V��	�ŏI�X�V����
    summry["content"].push({ "value": moment(wfCommon.getServerDate()).format(DATE_FORMAT_MOMENT_PATTERN_1), "name": "�X�V��" });
    // �Ј��ԍ�	�s�K�]�ƈ��Ј��ԍ�
    summry["content"].push({ "value": dtCondolence["UNFORTUNATE_SHAINBANGO"], "name": "�Ј��ԍ�" });
    // ����	�S���Ȃ�ꂽ���Ƃ̑����敪
    summry["content"].push({ "value": $("#select-died-relationship").text(), "name": "����" });
    // ���� / �ʋ�	�S���Ȃ�ꂽ���̓��� / �ʋ��敪
    summry["content"].push({ "value": $("input[name=radio-died-living]:checked").next().find("div").html(), "name": "����/�ʋ�" });
    // ������	�S���Ȃ�ꂽ���̐�����
    summry["content"].push({ "value": $("#text-died-death_date").val(), "name": "������" });
    // �ʖ��or���ʎ���or������	�ʖ���������ʎ���������������
    // �ʖ�/���ʎ��敪
    let funeralkbn = $("input[name=radio-funeral]:checked").val();
    if (funeralkbn === FUNERAL_KBN_BOTH || funeralkbn === FUNERAL_KBN_TSUYA) {    // �ʖ�/���ʎ��@�܂��́@�ʖ�
        let wakedate = $("#text-wake-date").val();
        if (wakedate !== null || wakedate !== STRING_EMPTY) {
            summry["content"].push({ "value": wakedate, "name": "�ʖ��" });
        }
    } else if (funeralkbn === FUNERAL_KBN_KOKUBETSUSHIKI) {    // ���ʎ�
        let funeraldate = $("#text-funeral-date").val();
        if (funeraldate !== null || funeraldate !== STRING_EMPTY) {
            summry["content"].push({ "value": funeraldate, "name": "���ʎ���" });
        }
    } else {    // �ȊO
        let reardate = $("#text-rear-date").val();
        if (reardate !== null || reardate !== STRING_EMPTY) {
            summry["content"].push({ "value": reardate, "name": "������" });
        }
    }
    return JSON.stringify(summry);
}

/**
 * �����敪�ɂ��A���ʂ�ύX
 * @param {string} value �����敪
 * */
function changeGenderByRelationship(value) {

    switch (parseInt(value)) {
        case 4:
        case 6:
        case 8:
        case 10:
        case 12:
        case 14:
        case 16:
            wfCommon.radiosSetVal("radio-died-gender", GENDER_MALE, GENDER_MALE);
            break;
        case 5:
        case 7:
        case 9:
        case 11:
        case 13:
        case 15:
        case 17:
            wfCommon.radiosSetVal("radio-died-gender", GENDER_FMALE, GENDER_FMALE);
            break;
        default:
            break;
    }
}

/**
 * ���̓`�F�b�N��ݒ肷��
 */
function setInputCheck() {

    $("#form1").validate({
        focusCleanup: true,
        onkeyup: false,
        ignore: STRING_EMPTY,
        groups: {
            contact_tel_mail: "text-contact-tel text-contact-email",
            contact_tel: "text-contact-tel1 text-contact-tel2 text-contact-tel3",
            wake_tel: "text-wake-tel1 text-wake-tel2 text-wake-tel3",
            funeral_tel: "text-funeral-tel1 text-funeral-tel2 text-funeral-tel3",
            rear_tel: "text-rear-tel1 text-rear-tel2 text-rear-tel3",
        },
        rules: {

            // �A���̃��[���A�h���X
            "text-contact-email": {
                required: function () {
                    return $("#text-contact-tel").val() === STRING_EMPTY;
                },
                email: true
            },
            // �A���̎���d�b�ԍ�
            "text-contact-tel": {
                required: function () {
                    return $("#text-contact-email").val() === STRING_EMPTY;
                }
            },
            // �A���̎���d�b�ԍ�1
            "text-contact-tel1": {
                digits: true
            },
            // �A���̎���d�b�ԍ�2
            "text-contact-tel2": {
                digits: true
            },
            // �A���̎���d�b�ԍ�3
            "text-contact-tel3": {
                digits: true
            },
            // �ϔC�ҎЈ��ԍ�
            "text-unhappiness-shainbango": {
                required: function () {
                    return $(".f-agent").is(":visible");
                },
                sameData: webUser.No
            },
            // �S���Ȃ�ꂽ�����:��
            "text-died-lastname": {
                required: true,
                fullchar: true
            },
            // �S���Ȃ�ꂽ�����:��
            "text-died-firstname": {
                required: true,
                fullchar: true
            },
            // �S���Ȃ�ꂽ�����:���i�J�i�j
            "text-died-lastname_kana": {
                required: true,
                fullkatakana: true
            },
            // �S���Ȃ�ꂽ�����:���i�J�i�j
            "text-died-firstname_kana": {
                required: true,
                fullkatakana: true
            },
            // �S���Ȃ�ꂽ�����:���Ȃ����猩������
            "select-died-relationship": {
                required: true
            },
            // �S���Ȃ�ꂽ�����:�N��
            "text-died-age": {
                digits: true
            },
            // �S���Ȃ�ꂽ�����:������
            "text-died-death_date": {
                required: true,
                dateJP: true,
            },
            // �S���Ȃ�ꂽ�����:��������
            "select-died-death_time": {
                required: true
            },
            // �r����:��
            "text-mourner-lastname": {
                required: function () {
                    return $(".area-mourner").is(":visible");
                },
                fullchar: function () {
                    return $(".area-mourner").is(":visible");
                },
            },
            // �r����:��
            "text-mourner-firstname": {
                required: function () {
                    return $(".area-mourner").is(":visible");
                },
                fullchar: function () {
                    return $(".area-mourner").is(":visible");
                },
            },
            // �r����:���i�J�i�j
            "text-mourner-lastname_kana": {
                required: function () {
                    return $(".area-mourner").is(":visible");
                },
                fullchar: function () {
                    return $(".area-mourner").is(":visible");
                },
            },
            // �r����:���i�J�i�j
            "text-mourner-firstname_kana": {
                required: function () {
                    return $(".area-mourner").is(":visible");
                },
                fullchar: function () {
                    return $(".area-mourner").is(":visible");
                },
            },
            // �ʖ�ɂ���:���t
            "text-wake-date": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
                dateJP: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // �ʖ�ɂ���: �J�n����
            "select-wake-time": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // �ʖ�ɂ���: ��ꖼ
            "text-wake-placeName": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // �ʖ�ɂ���: ��ꖼ�i�J�i�j
            "text-wake-placeName_kana": {
                fullkatakana: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // �ʖ�ɂ���: �X�֔ԍ�
            "text-wake-postcode": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
                zipcodeJP: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // �ʖ�ɂ���: �s���{���E�s�S��
            "text-wake-address1": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // �ʖ�ɂ���: �����E�Ԓn
            "text-wake-address2": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // �ʖ�ɂ���: ���d�b�ԍ�
            "text-wake-tel1": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
                digits: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            "text-wake-tel2": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
                digits: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            "text-wake-tel3": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
                digits: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // ���ʎ��ɂ���:���t
            "text-funeral-date": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
                dateJP: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // ���ʎ��ɂ���: �J�n����
            "select-funeral-time": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // ���ʎ��ɂ���: ��ꖼ
            "text-funeral-placeName": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // ���ʎ��ɂ���: ��ꖼ�i�J�i�j
            "text-funeral-placeName_kana": {
                fullkatakana: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // ���ʎ��ɂ���: �X�֔ԍ�
            "text-funeral-postcode": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
                zipcodeJP: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // ���ʎ��ɂ���: �s���{���E�s�S��
            "text-funeral-address1": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // ���ʎ��ɂ���: �����E�Ԓn
            "text-funeral-address2": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // ���ʎ��ɂ���: ���d�b�ԍ�
            "text-funeral-tel1": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
                digits: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            "text-funeral-tel2": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
                digits: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            "text-funeral-tel3": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
                digits: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // �����: ���O
            "text-rear-name": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            // �����: �X�֔ԍ�
            "text-rear-postcode": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
                zipcodeJP: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            // �����: �s���{���E�s�S��
            "text-rear-address1": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            // �����: �����E�Ԓn
            "text-rear-address2": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            // �����: �d�b�ԍ�
            "text-rear-tel1": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
                digits: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            "text-rear-tel2": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
                digits: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            "text-rear-tel3": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
                digits: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            // �����:���t
            "text-rear-date": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
                dateJP: function () {
                    return $(".area-rear").is(":visible");
                },

            },


        },
        messages: {

            "text-contact-email": {
                required: msg["E0004"]
            },
            "text-contact-tel": {
                required: msg["E0004"]
            },
            "text-unhappiness-shainbango": {
                sameData: msg["I0006"]
            },

        }
    });
}

/**
 * ��ʍ��ڂɃf�[�^���Z�b�g
 *
 * @param {Array<object>} data �T�[�o����擾�̃f�[�^�z��
 */
function setDataItems(data) {

    //console.log(data);
    //console.log(webUser);

    // �\���ҏ��
    dtApplicant = data[0];

    // �V�K�E�������̏ꍇ
    if (wfstate <= WF_STATE_DRAFT) {

        copyApplicantToCondolence();

        if (agentMode === SHINSEISYA_KBN_DAIRI) {
            setDelegatorInfo();
        }

    }

    // PDF��URL
    setPDFUrl();
    // �\���ҁE�ϔC�ҏ��
    setApplicantItems();
    // �S���Ȃ�ꂽ��
    setDiedItems();
    // �}�{�̏�
    setDependentItems();
    // �r��
    setMournerItems();
    // �ʖ�/���ʎ�
    setWakeAndFuneralItems();
    // �����E���ԁE���d
    setAllnightItems();
    // �����
    setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    setCondolenceStandardAreaDisplay();
    // �{�^��
    setButtonDisplay();

    // ���F�ς݂̏ꍇ
    if (wfstate === WF_STATE_OVER) {

        let deathday = moment(dtCondolence["DEAD_DATE"], DATE_FORMAT_MOMENT_PATTERN_1).add(1, 'years');
        // ��N�Ԓ�����
        if (deathday.isSameOrBefore(moment(wfCommon.getServerDate(), DATE_FORMAT_MOMENT_PATTERN_1))) { // new Date("2022/07/07")
            // �m�F��ʂ֑J�ڂ���
            $(".o-modal__footer").hide();
            $("#btn-form-back").hide();
            $("#btn-other-back").show();
            gotoConfirmPage();
        } else {
            // �����E�񊈐���ݒ�
            changeItemsDisabled();
            changeOpenerItemsDisabled();
            if ($("input[name=radio-opener-koryo]").attr("disabled") && $("input[name=radio-opener-kuge]").attr("disabled") && $("input[name=radio-opener-tyoden]").attr("disabled")) {
                // �m�F��ʂ֑J�ڂ���
                $(".o-modal__footer").hide();
                $("#btn-form-back").hide();
                $("#btn-other-back").show();
                gotoConfirmPage();
            }
        }
    }

    checkRefer = $("#form1").getElements();
    //console.log(checkRefer);
}

/**
 * PDF��URL���擾
 *
 */
function setPDFUrl() {
    var url = dtCondolence["PDF_URL"];
    if (url === null) {
        setPDFInfo(setPDFInfoItems);
    } else {
        $("#iframe-pdf").attr("src", url);
    }
}

/**
 * �\���ҏ��𒢎����ɃR�s�[
 *
 */
function copyApplicantToCondolence() {

    if (agentMode === SHINSEISYA_KBN_DAIRI) { // �㗝�\���̏ꍇ
        // �Ј��ԍ�
        dtCondolence["DAIRISHINSNEISYA_SHAINBANGO"] = dtApplicant["SHAINBANGO"];
        // ����
        dtCondolence["DAIRISHINSNEISYA_MEI"] = dtApplicant["SEI_KANJI"] + FULL_SPACE + dtApplicant["MEI_KANJI"];
        // �����i�J�i�j
        dtCondolence["DAIRISHINSNEISYA_FURIGANAMEI"] = dtApplicant["SEI_KANA"] + FULL_SPACE + dtApplicant["MEI_KANA"];
        // �����R�[�h
        dtCondolence["DAIRISHINSNEISYA_SYOZOKUCODE"] = dtApplicant["SHOZOKUCODE"];
        // ����
        dtCondolence["DAIRISHINSNEISYA_SYOZOKU"] = dtApplicant["SHOZOKUMEISHOJO_KANJI"];
    } else { // �{�l�\���̏ꍇ
        // �Ј��ԍ�
        dtCondolence["UNFORTUNATE_SHAINBANGO"] = dtApplicant["SHAINBANGO"];
        // ����
        dtCondolence["UNFORTUNATE_KANJIMEI"] = dtApplicant["SEI_KANJI"] + FULL_SPACE + dtApplicant["MEI_KANJI"];
        // �����i�J�i�j
        dtCondolence["UNFORTUNATE_FURIGANAMEI"] = dtApplicant["SEI_KANA"] + FULL_SPACE + dtApplicant["MEI_KANA"];
        // �o������ЃR�[�h
        dtCondolence["UNFORTUNATE_KAISYACODE"] = dtApplicant["KAISHACODE"];
        // �o������Ж���
        dtCondolence["UNFORTUNATE_KAISYAMEI"] = dtApplicant["KAISHANAME"];
        // �o�����ЃR�[�h
        dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYACODE"] = dtApplicant["SHUKKOKAISHACODE"];
        // �o�����Ж���
        dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYAMEI"] = dtApplicant["SHUKKOKAISHANAME"];
        // �����R�[�h
        dtCondolence["UNFORTUNATE_SYOZOKUCODE"] = dtApplicant["SHOZOKUCODE"];
        // �����g�D���E��
        dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"] = dtApplicant["SHOZOKUMEISHOJO_KANJI"];
        // �����g�D���E��
        dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"] = dtApplicant["SHOZOKUMEISHOGE_KANJI"];
        // �Ј��敪
        dtCondolence["UNFORTUNATE_SYAINNKBNCODE"] = dtApplicant["JUGYOINKBN"];
        // �Ј��敪����
        dtCondolence["UNFORTUNATE_SYAINNKBN"] = dtApplicant["MEISHO_KANJI"];
        // �E�ʖ���
        dtCondolence["UNFORTUNATE_SYOKUIKBN"] = dtApplicant["PLACE_MEISHO_KANJI"];
        // �g���敪
        dtCondolence["UNFORTUNATE_KUMIAIKBNCODE"] = dtApplicant["KUMIAIKBN"];
        // �g���敪����
        dtCondolence["UNFORTUNATE_KUMIAIKBN"] = dtApplicant["KUMIAINAME"];
        // �O�b�h���C�t�敪
        dtCondolence["UNFORTUNATE_GLCKBNCODE"] = dtApplicant["GOODLIFEKBN"];
        // �O�b�h���C�t�敪����
        dtCondolence["UNFORTUNATE_GLCKBN"] = dtApplicant["GOODLIFENAME"];
        // �o���t���O
        dtCondolence["SHUKKOFLG"] = dtApplicant["SHUKKOFLG"];
        // �`�[���R�[�h
        dtCondolence["TEAMCODE"] = dtApplicant["TEAMCODE"];
        // �`�[������
        dtCondolence["TEAMMEISHO"] = dtApplicant["TEAMNAME"];
        // ���������R�[�h
        dtCondolence["ZAIMUBUSHOCODE"] = dtApplicant["BUSHOCODE"];
        if (dtCondolence["SHUKKOFLG"] === SYUKOU_KBN_YES) { // �o������
            // �o��S��ЃR�[�h
            dtCondolence["KEIHIFUTANKAISHACODE"] = dtApplicant["SHUKKOKAISHACODE"];
            // �o��S��Ж�
            dtCondolence["KEIHIFUTANKAISHAMEI"] = dtApplicant["SHUKKOKAISHANAME"];
        } else { // �o���Ȃ�
            // �o��S��ЃR�[�h
            dtCondolence["KEIHIFUTANKAISHACODE"] = dtApplicant["KAISHACODE"];
            // �o��S��Ж�
            dtCondolence["KEIHIFUTANKAISHAMEI"] = dtApplicant["KAISHANAME"];
        }

    }
}

/**
 * �ϔC�ҏ��𒢎����ɃR�s�[
 *
 */
function copyDelegatorToCondolence() {

    if (agentMode === SHINSEISYA_KBN_DAIRI) { // �㗝�\���̏ꍇ
        // �Ј��ԍ�
        dtCondolence["UNFORTUNATE_SHAINBANGO"] = dtDelegator["SHAINBANGO"];
        // ����
        dtCondolence["UNFORTUNATE_KANJIMEI"] = dtDelegator["SEI_KANJI"] + FULL_SPACE + dtDelegator["MEI_KANJI"];
        // �����i�J�i�j
        dtCondolence["UNFORTUNATE_FURIGANAMEI"] = dtDelegator["SEI_KANA"] + FULL_SPACE + dtDelegator["MEI_KANA"];
        // �o������ЃR�[�h
        dtCondolence["UNFORTUNATE_KAISYACODE"] = dtDelegator["KAISHACODE"];
        // �o������Ж���
        dtCondolence["UNFORTUNATE_KAISYAMEI"] = dtDelegator["KAISHANAME"];
        // �o�����ЃR�[�h
        dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYACODE"] = dtDelegator["SHUKKOKAISHACODE"];
        // �o�����Ж���
        dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYAMEI"] = dtDelegator["SHUKKOKAISHANAME"];
        // �����R�[�h
        dtCondolence["UNFORTUNATE_SYOZOKUCODE"] = dtDelegator["SHOZOKUCODE"];
        // �����g�D���E��
        dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"] = dtDelegator["SHOZOKUMEISHOJO_KANJI"];
        // �����g�D���E��
        dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"] = dtDelegator["SHOZOKUMEISHOGE_KANJI"];
        // �Ј��敪
        dtCondolence["UNFORTUNATE_SYAINNKBNCODE"] = dtDelegator["JUGYOINKBN"];
        // �Ј��敪����
        dtCondolence["UNFORTUNATE_SYAINNKBN"] = dtDelegator["MEISHO_KANJI"];
        // �E�ʖ���
        dtCondolence["UNFORTUNATE_SYOKUIKBN"] = dtDelegator["PLACE_MEISHO_KANJI"];
        // �g���敪
        dtCondolence["UNFORTUNATE_KUMIAIKBNCODE"] = dtDelegator["KUMIAIKBN"];
        // �g���敪����
        dtCondolence["UNFORTUNATE_KUMIAIKBN"] = dtDelegator["KUMIAINAME"];
        // �O�b�h���C�t�敪
        dtCondolence["UNFORTUNATE_GLCKBNCODE"] = dtDelegator["GOODLIFEKBN"];
        // �O�b�h���C�t�敪����
        dtCondolence["UNFORTUNATE_GLCKBN"] = dtDelegator["GOODLIFENAME"];
        // �o���t���O
        dtCondolence["SHUKKOFLG"] = dtDelegator["SHUKKOFLG"];
        // �`�[���R�[�h
        dtCondolence["TEAMCODE"] = dtDelegator["TEAMCODE"];
        // �`�[������
        dtCondolence["TEAMMEISHO"] = dtDelegator["TEAMNAME"];
        // ���������R�[�h
        dtCondolence["ZAIMUBUSHOCODE"] = dtDelegator["BUSHOCODE"];
        if (dtCondolence["SHUKKOFLG"] === SYUKOU_KBN_YES) { // �o������
            // �o��S��ЃR�[�h
            dtCondolence["KEIHIFUTANKAISHACODE"] = dtDelegator["SHUKKOKAISHACODE"];
            // �o��S��Ж�
            dtCondolence["KEIHIFUTANKAISHAMEI"] = dtDelegator["SHUKKOKAISHANAME"];
        } else { // �o���Ȃ�
            // �o��S��ЃR�[�h
            dtCondolence["KEIHIFUTANKAISHACODE"] = dtDelegator["KAISHACODE"];
            // �o��S��Ж�
            dtCondolence["KEIHIFUTANKAISHAMEI"] = dtDelegator["KAISHANAME"];
        }
    }
}

/**
 * �ϔC�ҏ��𒢎����ɃN���A
 *
 */
function clearDelegatorToCondolence() {

    // �Ј��ԍ�
    dtCondolence["UNFORTUNATE_SHAINBANGO"] = STRING_EMPTY;
    // ����
    dtCondolence["UNFORTUNATE_KANJIMEI"] = STRING_EMPTY;
    // �����i�J�i�j
    dtCondolence["UNFORTUNATE_FURIGANAMEI"] = STRING_EMPTY;
    // �o������ЃR�[�h
    dtCondolence["UNFORTUNATE_KAISYACODE"] = STRING_EMPTY;
    // �o������Ж���
    dtCondolence["UNFORTUNATE_KAISYAMEI"] = STRING_EMPTY;
    // �o�����ЃR�[�h
    dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYACODE"] = STRING_EMPTY;
    // �o�����Ж���
    dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYAMEI"] = STRING_EMPTY;
    // �����R�[�h
    dtCondolence["UNFORTUNATE_SYOZOKUCODE"] = STRING_EMPTY;
    // �����g�D���E��
    dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"] = STRING_EMPTY;
    // �����g�D���E��
    dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"] = STRING_EMPTY;
    // �Ј��敪
    dtCondolence["UNFORTUNATE_SYAINNKBNCODE"] = STRING_EMPTY;
    // �Ј��敪����
    dtCondolence["UNFORTUNATE_SYAINNKBN"] = STRING_EMPTY;
    // �E�ʖ���
    dtCondolence["UNFORTUNATE_SYOKUIKBN"] = STRING_EMPTY;
    // �g���敪
    dtCondolence["UNFORTUNATE_KUMIAIKBNCODE"] = STRING_EMPTY;
    // �g���敪����
    dtCondolence["UNFORTUNATE_KUMIAIKBN"] = STRING_EMPTY;
    // �O�b�h���C�t�敪����
    dtCondolence["UNFORTUNATE_GLCKBNCODE"] = STRING_EMPTY;
    // �O�b�h���C�t�敪����
    dtCondolence["UNFORTUNATE_GLCKBN"] = STRING_EMPTY;
    // �o���t���O
    dtCondolence["SHUKKOFLG"] = STRING_EMPTY;
    // �`�[���R�[�h
    dtCondolence["TEAMCODE"] = STRING_EMPTY;
    // �`�[������
    dtCondolence["TEAMMEISHO"] = STRING_EMPTY;
    // ���������R�[�h
    dtCondolence["ZAIMUBUSHOCODE"] = STRING_EMPTY;
    // �o��S��ЃR�[�h
    dtCondolence["KEIHIFUTANKAISHACODE"] = STRING_EMPTY;
    // �o��S��Ж�
    dtCondolence["KEIHIFUTANKAISHAMEI"] = STRING_EMPTY;
}

/**
 * �\���҂̎Ј���񍀖ڂɃf�[�^���Z�b�g
 *
 */
function setApplicantItems() {

    if (agentMode === SHINSEISYA_KBN_DAIRI) { // �㗝�\���̏ꍇ
        // �Ј��ԍ�
        $("#label-agent-shainbango").text(dtCondolence["DAIRISHINSNEISYA_SHAINBANGO"]);
        // ����
        $("#label-agent-shainshime").text(dtCondolence["DAIRISHINSNEISYA_MEI"]);
        // �����i�J�i�j
        $("#label-agent-shainkana").text(dtCondolence["DAIRISHINSNEISYA_FURIGANAMEI"]);
        // ����
        $("#label-agent-shozokuname").text(dtCondolence["DAIRISHINSNEISYA_SYOZOKU"]);
        // �ϔC�ҎЈ��ԍ�
        $("#text-unhappiness-shainbango").val(dtCondolence["UNFORTUNATE_SHAINBANGO"]);
        // �ϔC�Ҏ���
        $("#text-unhappiness-shainshime").val(dtCondolence["UNFORTUNATE_KANJIMEI"]);
        // �ϔC�Ҏ����i�J�i�j
        $("#text-unhappiness-shainkana").val(dtCondolence["UNFORTUNATE_FURIGANAMEI"]);
        // �ϔC�҉�Ж���
        $("#text-unhappiness-kaishaname").val(dtCondolence["UNFORTUNATE_KAISYAMEI"]);
        // �ϔC�Ґ����g�D���E��
        $("#text-unhappiness-shozokuname").val(dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"]);
        // �ϔC�Ґ����g�D���E��
        $("#text-unhappiness-bushoname").val(dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"]);
        // �ϔC�ҎЈ��敪
        $("#text-unhappiness-kbnname").val(dtCondolence["UNFORTUNATE_SYAINNKBN"]);
        // �ϔC�ҐE��
        $("#text-unhappiness-shokuiname").val(dtCondolence["UNFORTUNATE_SYOKUIKBN"]);
        // �ϔC�ґg���敪
        $("#text-unhappiness-kumiainame").val(dtCondolence["UNFORTUNATE_KUMIAIKBN"]);
        // �ϔC�҃O�b�h���C�t�敪
        $("#text-unhappiness-goodlifename").val(dtCondolence["UNFORTUNATE_GLCKBN"]);
    } else { // �{�l�\���̏ꍇ
        // �Ј��ԍ�
        $("#label-emp-shainbango").text(dtCondolence["UNFORTUNATE_SHAINBANGO"]);
        // ����
        $("#label-emp-shainshime").text(dtCondolence["UNFORTUNATE_KANJIMEI"]);
        // �����i�J�i�j
        $("#label-emp-shainkana").text(dtCondolence["UNFORTUNATE_FURIGANAMEI"]);
        // ��Ж���
        $("#label-emp-kaishaname").text(dtCondolence["UNFORTUNATE_KAISYAMEI"]);
        // �����g�D���E��
        $("#label-emp-shozokuname").text(dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"]);
        // �����g�D���E��
        $("#label-emp-bushoname").text(dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"]);
        // �Ј��敪
        $("#label-emp-kbnname").text(dtCondolence["UNFORTUNATE_SYAINNKBN"]);
        // �E��
        $("#label-emp-shokuiname").text(dtCondolence["UNFORTUNATE_SYOKUIKBN"]);
        // �g���敪
        $("#label-emp-kumiainame").text(dtCondolence["UNFORTUNATE_KUMIAIKBN"]);
        // �O�b�h���C�t�敪
        $("#label-emp-goodlifename").text(dtCondolence["UNFORTUNATE_GLCKBN"]);
    }

    // �A���̎���d�b�ԍ�
    let contactTel = dtCondolence["RENRAKUSAKITEL"];
    if (contactTel !== null) {
        let contactTels = contactTel.split("-");
        $("#text-contact-tel1").val(contactTels[0]);
        $("#text-contact-tel2").val(contactTels[1]);
        $("#text-contact-tel3").val(contactTels[2]);
        $("#text-contact-tel").val(contactTel);
    }
    // ���[���A�h���X
    $("#text-contact-email").val(dtCondolence["RENRAKUSAKIMAIL"]);

}

/**
 * �S���Ȃ�ꂽ���ɂ���
 * 
 */
function setDiedItems() {
    // �����i���j
    $("#text-died-lastname").val(dtCondolence["DEAD_SHIMEI_SEI"]);
    // �����i���j
    $("#text-died-firstname").val(dtCondolence["DEAD_SHIMEI_MEI"]);
    // �����J�i�i���j
    $("#text-died-lastname_kana").val(dtCondolence["DEAD_KANASHIMEI_SEI"]);
    // �����J�i�i���j
    $("#text-died-firstname_kana").val(dtCondolence["DEAD_KANASHIMEI_MEI"]);
    // �����敪
    if (agentMode !== SHINSEISYA_KBN_DAIRI) { // �{�l�\��
        dtKbn["DEAD_KBN"].splice(6, 1); // �]�ƈ��{�l�I�������폜
    }
    wfCommon.initDropdown(true, dtKbn["DEAD_KBN"], dtCondolence["DEAD_JUGYOIN_ZOKUGARAKBN"], MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "select-died-relationship", "pulldown-died-relationship");
    // ���Ȃ����猩�������v���_�E��
    $("#select-died-relationship").on("change", function () {
        // �Č���
        $("#form1").validate().element($("#select-died-relationship"));
        // ���Ȃ����猩���������Z�b�g 
        dtCondolence["DEAD_JUGYOIN_ZOKUGARAKBN"] = this.value;
        // �����敪�ɂ��A���ʂ�ݒ�
        changeGenderByRelationship(this.value);
        // �}�{�̏󋵂��Z�b�g
        setDependentItems();
        // ����������Z�b�g
        setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    });
    // ����
    wfCommon.radiosSetVal("radio-died-gender", dtCondolence["DEAD_SEIBETSU"], GENDER_MALE);
    // ����/�ʋ��敪
    wfCommon.radiosSetVal("radio-died-living", dtCondolence["DEAD_DOKYO_BEKYO"], DOKYO_BEKYO_KBN_YES);
    $("input[name=radio-died-living]").on("change", function () {
        // ����������Z�b�g
        setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    });
    // �N��
    $("#text-died-age").val(dtCondolence["DEAD_NENREI"]);
    // �������t
    let deathdate = dtCondolence["DEAD_DATE"];
    if (deathdate !== null) {
        $("#text-died-death_date").val(moment(deathdate).format(DATE_FORMAT_MOMENT_PATTERN_1));
    }
    // ��������
    wfCommon.initDropdown(true, deathTimeList["content"], dtCondolence["DEAD_TIME"], "value", "name", "select-died-death_time", "pulldown-died-death_time");
    // ���������v���_�E��
    $("#select-died-death_time").on("change", function () {
        // �Č���
        $("#form1").validate().element($("#select-died-death_time"));
    });
    // �}�{�敪
    wfCommon.radiosSetVal("radio-died-dependent", dtCondolence["DEAD_FUYOUKBN"], FUYOUKBN_BEKYO_KBN_YES);
    $("input[name=radio-died-dependent]").on("change", function () {
        // ����������Z�b�g
        setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    });
}

/**
 * �ϔC�҂̎Ј���񍀖ڂɃf�[�^���Z�b�g
 *
 */
function setDelegatorItems(data) {

    if (data.length === 1) {
        // �ϔC�ҏ��
        dtDelegator = data[0];
        // ����
        $("#text-unhappiness-shainshime").val(dtDelegator["SEI_KANJI"] + FULL_SPACE + dtDelegator["MEI_KANJI"]);
        // �����i�J�i�j
        $("#text-unhappiness-shainkana").val(dtDelegator["SEI_KANA"] + FULL_SPACE + dtDelegator["MEI_KANA"]);
        // ��Ж���
        $("#text-unhappiness-kaishaname").val(dtDelegator["KAISHANAME"]);
        // �����g�D���E��
        $("#text-unhappiness-shozokuname").val(dtDelegator["SHOZOKUMEISHOJO_KANJI"]);
        // �����g�D���E��
        $("#text-unhappiness-bushoname").val(dtDelegator["SHOZOKUMEISHOGE_KANJI"]);
        // �Ј��敪
        $("#text-unhappiness-kbnname").val(dtDelegator["MEISHO_KANJI"]);
        // �E��
        $("#text-unhappiness-shokuiname").val(dtDelegator["PLACE_MEISHO_KANJI"]);
        // �g���敪
        $("#text-unhappiness-kumiainame").val(dtDelegator["KUMIAINAME"]);
        // �O�b�h���C�t�敪
        $("#text-unhappiness-goodlifename").val(dtDelegator["GOODLIFENAME"]);
        // �R�s�[
        copyDelegatorToCondolence();
    } else {
        // ����
        $("#text-unhappiness-shainshime").val(STRING_EMPTY);
        // �����i�J�i�j
        $("#text-unhappiness-shainkana").val(STRING_EMPTY);
        // ��Ж���
        $("#text-unhappiness-kaishaname").val(STRING_EMPTY);
        // �����g�D���E��
        $("#text-unhappiness-shozokuname").val(STRING_EMPTY);
        // �����g�D���E��
        $("#text-unhappiness-bushoname").val(STRING_EMPTY);
        // �Ј��敪
        $("#text-unhappiness-kbnname").val(STRING_EMPTY);
        // �E��
        $("#text-unhappiness-shokuiname").val(STRING_EMPTY);
        // �g���敪
        $("#text-unhappiness-kumiainame").val(STRING_EMPTY);
        // �O�b�h���C�t�敪
        $("#text-unhappiness-goodlifename").val(STRING_EMPTY);
        // �N���A
        clearDelegatorToCondolence();
    }

}

/**
 * �}�{�̏� �ݒ�
 *
 */
function setDependentItems() {

    let glc_kbn = dtCondolence["UNFORTUNATE_GLCKBNCODE"];
    let relationship_kbn = null;
    let jugyoin_kbn = null;

    if (agentMode === SHINSEISYA_KBN_DAIRI) { // �㗝�\���̏ꍇ
        glc_kbn = wfCommon.FindKbnCode($("#text-unhappiness-goodlifename").val(), "GLC_KBN");
        jugyoin_kbn = wfCommon.FindKbnCode($("#text-unhappiness-kbnname").val(), "JYUUGYOUINN_KBN");

    } else { // �{�l�\���̏ꍇ
        glc_kbn = wfCommon.FindKbnCode($("#label-emp-goodlifename").text(), "GLC_KBN");
        jugyoin_kbn = wfCommon.FindKbnCode($("#label-emp-kbnname").text(), "JYUUGYOUINN_KBN");
    }

    relationship_kbn = $("#select-died-relationship option:selected").val();

    // GLC�敪�u��������v�܂��u�������*�v�@���@�S���Ȃ�ꂽ �u�q�v��I���@���@�]�ƈ��敪�u�R�~���j�e�B�Ј��i���ԋ��j�v
    if ((glc_kbn === "2" || glc_kbn === "E") && relationship_kbn === "3" && jugyoin_kbn === "T") {
        $(".area-died-dependent").show();
    } else {
        $(".area-died-dependent").hide();
        // �}�{�̏󋵁F�}�{�e���ɖ߂�
        wfCommon.radiosSetVal("radio-died-dependent", FUYOUKBN_BEKYO_KBN_YES, FUYOUKBN_BEKYO_KBN_YES);
    }
}

/**
 * �m�F��ʂɃf�[�^���Z�b�g
 * 
 * */
function setDataToConfirmPage() {
    // �A���̎���d�b�ԍ��^���[���A�h���X
    $("#p-contact-tel").html($("#text-contact-tel").val());
    $("#p-contact-email").html($("#text-contact-email").val());

    // ���s�K�ɂ���ꂽ���̎Ј����
    $("#p-delegator-shainbango").html($("#text-unhappiness-shainbango").val());
    $("#p-delegator-shainshime").html($("#text-unhappiness-shainshime").val());
    $("#p-delegator-shainkana").html($("#text-unhappiness-shainkana").val());
    $("#p-delegator-kaishaname").html($("#text-unhappiness-kaishaname").val());
    $("#p-delegator-shozokuname").html($("#text-unhappiness-shozokuname").val());
    $("#p-delegator-bushoname").html($("#text-unhappiness-bushoname").val());
    $("#p-delegator-kbnname").html($("#text-unhappiness-kbnname").val());
    $("#p-delegator-shokuiname").html($("#text-unhappiness-shokuiname").val());
    $("#p-delegator-kumiainame").html($("#text-unhappiness-kumiainame").val());
    $("#p-delegator-goodlifename").html($("#text-unhappiness-goodlifename").val());

    // �S���Ȃ�ꂽ���ɂ���
    $("#p-died-fullname").html($("#text-died-lastname").val() + FULL_SPACE + $("#text-died-firstname").val());
    $("#p-died-fullname_kana").html($("#text-died-lastname_kana").val() + FULL_SPACE + $("#text-died-firstname_kana").val());
    $("#p-died-relationship").html($("#select-died-relationship").html());
    $("#p-died-gender").html($("input[name=radio-died-gender]:checked").next().find("div").html());
    $("#p-died-living").html($("input[name=radio-died-living]:checked").next().find("div").html());
    if ($("#text-died-age").val() !== STRING_EMPTY) {
        $("#p-died-age").html($("#text-died-age").val());
    }
    $("#p-died-death_date").html($("#text-died-death_date").val());
    $("#p-died-death_time").html($("#select-died-death_time").html());
    $("#p-died-dependent").html($("input[name=radio-died-dependent]:checked").next().find("div").html());

    // ���V�ɂ���
    $("#p-mourner").html($("input[name=radio-mourner]:checked").next().find("div").html());
    $("#p-mourner-fullname").html($("#text-mourner-lastname").val() + FULL_SPACE + $("#text-mourner-firstname").val());
    $("#p-mourner-fullname_kana").html($("#text-mourner-lastname_kana").val() + FULL_SPACE + $("#text-mourner-firstname_kana").val());
    if ($("#chk-funeral-decline").prop("checked")) {
        $("#p-funeral").html($("input[name=radio-funeral]:checked").next().find("div").html() + "�^" + $("#chk-funeral-decline").next().find("div").html());
    } else {
        $("#p-funeral").html($("input[name=radio-funeral]:checked").next().find("div").html());
    }

    // �ʖ�ɂ���
    $("#p-wake-date").html($("#text-wake-date").val());
    $("#p-wake-time").html($("#select-wake-time").html());
    $("#p-wake-placeName").html($("#text-wake-placeName").val());
    if ($("#text-wake-placeName_kana").val() !== STRING_EMPTY) {
        $("#p-wake-placeName_kana").html($("#text-wake-placeName_kana").val());
    }
    $("#p-wake-postcode").html($("#text-wake-postcode").val());
    $("#p-wake-address1").html($("#text-wake-address1").val());
    $("#p-wake-address2").html($("#text-wake-address2").val());
    if ($("#text-wake-address3").val() !== STRING_EMPTY) {
        $("#p-wake-address3").html($("#text-wake-address3").val());
    }
    $("#p-wake-tel").html($("#text-wake-tel").val());

    // ���ʎ��ɂ���
    $("#p-funeral-date").html($("#text-funeral-date").val());
    $("#p-funeral-time").html($("#select-funeral-time").html());
    $("#p-funeral-placeName").html($("#text-funeral-placeName").val());
    if ($("#text-funeral-placeName_kana").val() !== STRING_EMPTY) {
        $("#p-funeral-placeName_kana").html($("#text-funeral-placeName_kana").val());
    }
    $("#p-funeral-postcode").html($("#text-funeral-postcode").val());
    $("#p-funeral-address1").html($("#text-funeral-address1").val());
    $("#p-funeral-address2").html($("#text-funeral-address2").val());
    if ($("#text-funeral-address3").val() !== STRING_EMPTY) {
        $("#p-funeral-address3").html($("#text-funeral-address3").val());
    }
    $("#p-funeral-tel").html($("#text-funeral-tel").val());

    // �����E���ԁE���d�ɂ���
    if (dtCondolence["KOURYOU_GOUKEI"] > 0) {
        $("#p-opener-koryo").html($("input[name=radio-opener-koryo]:checked").next().find("div").html());
    } else {
        $("#p-opener-koryo").html(STANDARD_ITEMS.NOT_APPLICABLE);
    }

    if (dtCondolence["KUGE_GOUKEI"] > 0) {
        $("#p-opener-kuge").html($("input[name=radio-opener-kuge]:checked").next().find("div").html());
    } else {
        $("#p-opener-kuge").html(STANDARD_ITEMS.NOT_APPLICABLE);
    }

    if (dtCondolence["TYOUDEN_GOUKEI"] > 0) {
        $("#p-opener-tyoden").html($("input[name=radio-opener-tyoden]:checked").next().find("div").html());
    } else {
        $("#p-opener-tyoden").html(STANDARD_ITEMS.NOT_APPLICABLE);
    }

    // �ʖ�/���ʎ��̗L���ɂ���
    $("#p-allnight").html($("input[name=radio-allnight]:checked").next().find("div").html())

    // �����̂��͂�����
    $("#p-rear-name").html($("#text-rear-name").val());
    $("#p-rear-postcode").html($("#text-rear-postcode").val());
    $("#p-rear-address1").html($("#text-rear-address1").val());
    $("#p-rear-address2").html($("#text-rear-address2").val());
    if ($("#text-rear-address3").val() !== STRING_EMPTY) {
        $("#p-rear-address3").html($("#text-rear-address3").val());
    }
    $("#p-rear-tel").html($("#text-rear-tel").val());
    $("#p-rear-date").html($("#text-rear-date").val());

}

/**
  * �������� �\���E��\��
  */
function setCondolenceStandardAreaDisplay() {

    if (agentMode === SHINSEISYA_KBN_DAIRI) { // �㗝�\��
        $("#area-standard").show();
    } else { // �{�l�\��
        if (wfstate === WF_STATE_OVER) {  // ���F��
            $("#area-standard").show();
        } else {
            $("#area-standard").hide();
        }
    }
}

/**
  * ����������Z�b�g
  * @param {string} callback CALLBACK�֐�
  */
function setCondolenceStandardInfo(callback) {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Condolence");
    handler.AddUrlData();

    // �o������ЃR�[�h
    handler.AddPara("kaishacode", dtCondolence["UNFORTUNATE_KAISYACODE"]);
    // �o�����ЃR�[�h
    handler.AddPara("shukokaishacode", dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYACODE"]);
    // �Ј��ԍ�
    handler.AddPara("shainbango", dtCondolence["UNFORTUNATE_SHAINBANGO"]);
    // �g���敪
    handler.AddPara("kumiai_kbn", dtCondolence["UNFORTUNATE_KUMIAIKBNCODE"]);
    // �O�b�h���C�t�敪
    handler.AddPara("glc_kbn", dtCondolence["UNFORTUNATE_GLCKBNCODE"]);
    // �Ј��敪
    handler.AddPara("jyuugyouinn_kbn", dtCondolence["UNFORTUNATE_SYAINNKBNCODE"]);
    // �o���敪
    handler.AddPara("syukou_kbn", dtCondolence["SHUKKOFLG"]);

    //�S���Ȃ�ꂽ���̑���
    handler.AddPara("dead_kbn", $("#select-died-relationship option:selected").val());
    //�r��敪
    handler.AddPara("organizer_kbn", $("input[name=radio-mourner]:checked").val());
    //�����敪
    handler.AddPara("dokyo_bekyo_kbn", $("input[name=radio-died-living]:checked").val());
    //�ŕ}�{�敪
    //if ($(".area-died-dependent").is(":visible") === true) {
    handler.AddPara("fuyou_kbn", $("input[name=radio-died-dependent]:checked").val());
    //}

    handler.DoMethodSetString("Get_Condolence_Standard_Info", function (data) {
        //��O����
        if (data.indexOf("err@") === 0) {
            wfCommon.Msgbox(data);
            return;
        }
        // JSON�Ώۂɓ]��
        var ret = JSON.parse(data);
        callback(ret);
    });
}

/**
 * ��������ڂɃf�[�^���Z�b�g
 *
 * @param {Array<object>} data �T�[�o����擾�̃f�[�^�z��
 */
function setCondolenceStandardInfoItems(data) {
    //console.log(data);
    let infoArray = data["Get_Condolence_Standard_Info"];

    copyStandardToCondolence(infoArray);

    if (agentMode === SHINSEISYA_KBN_DAIRI) {

        // ������f�[�^��0���̏ꍇ
        if (dtCondolence["KOURYOU_GOUKEI"] === 0 && dtCondolence["KUGE_GOUKEI"] === 0 && dtCondolence["TYOUDEN_GOUKEI"] === 0) {
            $("#arrange-ok").hide();
            $("#arrange-no").show();
        } else {
            $("#arrange-ok").show();
            $("#arrange-no").hide();

            setCondolenceStandardItems();
        }

    } else {
        // ������f�[�^��0���̏ꍇ
        if (dtCondolence["KOURYOU_GOUKEI"] === 0 && dtCondolence["KUGE_GOUKEI"] === 0 && dtCondolence["TYOUDEN_GOUKEI"] === 0) {
            $("#label-standard-info0").parent().hide();
            $("#label-standard-info1").parent().hide();
            $("#label-standard-info2").parent().hide();
            $("#label-standard-info3").parent().show();
        } else {  // �ȊO�̏ꍇ

            setCondolenceStandardItems();
        }
    }

    // ���o�l���
    $("#p-standard-table").empty();
    $("#label-standard-table").clone().appendTo("#p-standard-table");
}

/**
 * ������f�[�^��ݒ�
 * 
 * */
function setCondolenceStandardItems() {
    if (dtCondolence["SASHIDASHI_MOTO_KAISYA1"] === null || dtCondolence["SASHIDASHI_MOTO_KAISYA1"] === STRING_EMPTY) {
        $("#label-standard-info0").parent().hide();
    } else {
        $("#label-standard-info0").parent().show();
        $("#label-standard-kaishaname0").text(dtCondolence["SASHIDASHI_MOTO_KAISYA1"]);
        $("#label-standard-yakushoku0").text(dtCondolence["SASHIDASHI_MOTO_KAISYA2"]);
        $("#label-standard-shimei0").text(dtCondolence["SASHIDASHI_MOTO_KAISYA3"]);
        $("#label-standard-koryonum0").text(editKoryoByCondolenceStandard(dtCondolence["KOURYOU_MOTO_KAISYA"]));
        $("#label-standard-kyokanum0").text(editKyokaByCondolenceStandard(dtCondolence["KUGE_MOTO_KAISYA"]));
        $("#label-standard-tyodennum0").text(editTyodenByCondolenceStandard(dtCondolence["TYOUDEN_MOTO_KAISYA"]));
    }

    if (dtCondolence["SASHIDASHI_MOTO_KUMIAI1"] === null || dtCondolence["SASHIDASHI_MOTO_KUMIAI1"] === STRING_EMPTY) {
        $("#label-standard-info1").parent().hide();
    } else {
        $("#label-standard-info1").parent().show();
        $("#label-standard-kaishaname1").text(dtCondolence["SASHIDASHI_MOTO_KUMIAI1"]);
        $("#label-standard-yakushoku1").text(dtCondolence["SASHIDASHI_MOTO_KUMIAI2"]);
        $("#label-standard-shimei1").text(dtCondolence["SASHIDASHI_MOTO_KUMIAI3"]);
        $("#label-standard-koryonum1").text(editKoryoByCondolenceStandard(dtCondolence["KOURYOU_MOTO_KUMIAI"]));
        $("#label-standard-kyokanum1").text(editKyokaByCondolenceStandard(dtCondolence["KUGE_MOTO_KUMIAI"]));
        $("#label-standard-tyodennum1").text(editTyodenByCondolenceStandard(dtCondolence["TYOUDEN_MOTO_KUMIAI"]));
    }

    if (dtCondolence["SASHIDASHI_SAKI_KAISYA1"] === null || dtCondolence["SASHIDASHI_SAKI_KAISYA1"] === STRING_EMPTY) {
        $("#label-standard-info2").parent().hide();
    } else {
        $("#label-standard-info2").parent().show();
        $("#label-standard-kaishaname2").text(dtCondolence["SASHIDASHI_SAKI_KAISYA1"]);
        $("#label-standard-yakushoku2").text(dtCondolence["SASHIDASHI_SAKI_KAISYA2"]);
        $("#label-standard-shimei2").text(dtCondolence["SASHIDASHI_SAKI_KAISYA3"]);
        $("#label-standard-koryonum2").text(editKoryoByCondolenceStandard(dtCondolence["KOURYOU_SAKI_KAISYA"]));
        $("#label-standard-kyokanum2").text(editKyokaByCondolenceStandard(dtCondolence["KUGE_SAKI_KAISYA"]));
        $("#label-standard-tyodennum2").text(editTyodenByCondolenceStandard(dtCondolence["TYOUDEN_SAKI_KAISYA"]));
    }

    $("#label-standard-info3").parent().hide();
}

/**
 * ���������:������ҏW����
 *
 * @param {number} koryo ����
 */
function editKoryoByCondolenceStandard(koryo) {
    let display;
    if (agentMode === SHINSEISYA_KBN_DAIRI) {
        display = wfCommon.comma(koryo) + STANDARD_ITEMS.KORYO.UNIT;
    } else {
        display = String(koryo) === "0" ? STANDARD_ITEMS.KORYO.NONE : STANDARD_ITEMS.KORYO.YES;
    }

    return display;
}

/**
 * ���������:���Ԃ�ҏW����
 *
 * @param {Number} kyoka ����
 */
function editKyokaByCondolenceStandard(kyoka) {
    let display;
    if (agentMode === SHINSEISYA_KBN_DAIRI) {
        display = String(kyoka) + STANDARD_ITEMS.KYOKA.UNIT;
    } else {
        display = String(kyoka) === "0" ? STANDARD_ITEMS.KYOKA.NONE : STANDARD_ITEMS.KYOKA.YES;
    }

    return display;
}

/**
 * ���������:���d��ҏW����
 *
 * @param {Number} tyoden ���d
 */
function editTyodenByCondolenceStandard(tyoden) {
    let display;
    if (agentMode === SHINSEISYA_KBN_DAIRI) {
        display = String(tyoden) + STANDARD_ITEMS.TYODEN.UNIT;
    } else {
        display = String(tyoden) === "0" ? STANDARD_ITEMS.TYODEN.NONE : STANDARD_ITEMS.TYODEN.YES;
    }

    return display;
}

/**
 * ������𒢎����ɃR�s�[
 *
 */
function copyStandardToCondolence(infoArray) {

    let koryo_sum = 0;
    let kyoka_sum = 0;
    let tyoden_sum = 0;

    let info0 = infoArray[0];

    if (info0 !== undefined) {
        // �����E���ԁE���d���o�l_��Ж��i�o�����j
        dtCondolence["SASHIDASHI_MOTO_KAISYA1"] = info0["KAISHAMEI"];
        // �����E���ԁE���d���o�l_��Б�\�Ҍ������i�o�����j
        dtCondolence["SASHIDASHI_MOTO_KAISYA2"] = info0["YAKUSHOKU"];
        // �����E���ԁE���d���o�l_��Б�\�Ҏ����i�o�����j
        dtCondolence["SASHIDASHI_MOTO_KAISYA3"] = info0["SHIMEI"];
        // �o������Ђ�荁��
        dtCondolence["KOURYOU_MOTO_KAISYA"] = info0["KORYONUM"];
        // �o������Ђ�苟�Ԃ̐�
        dtCondolence["KUGE_MOTO_KAISYA"] = info0["KYOKANUM"];
        // �o������Ђ�蒢�d�L��
        dtCondolence["TYOUDEN_MOTO_KAISYA"] = info0["TYODENNUM"];

        koryo_sum += Number(info0["KORYONUM"]);
        kyoka_sum += Number(info0["KYOKANUM"]);
        tyoden_sum += Number(info0["TYODENNUM"]);
    }

    let info1 = infoArray[1];

    if (info1 !== undefined) {
        // �����E���ԁE���d���o�l_�J���g�����i�o�����j
        dtCondolence["SASHIDASHI_MOTO_KUMIAI1"] = info1["KAISHAMEI"];
        // �����E���ԁE���d���o�l_�J���g����\�Ҍ�����
        dtCondolence["SASHIDASHI_MOTO_KUMIAI2"] = info1["YAKUSHOKU"];
        // �����E���ԁE���d���o�l_�J���g����\�Ҏ���
        dtCondolence["SASHIDASHI_MOTO_KUMIAI3"] = info1["SHIMEI"];
        // �o�����J���g����荁��
        dtCondolence["KOURYOU_MOTO_KUMIAI"] = info1["KORYONUM"];
        // �o�����J���g����苟�Ԃ̐�
        dtCondolence["KUGE_MOTO_KUMIAI"] = info1["KYOKANUM"];
        // �o�����J���g����蒢�d�L��
        dtCondolence["TYOUDEN_MOTO_KUMIAI"] = info1["TYODENNUM"];

        koryo_sum += Number(info1["KORYONUM"]);
        kyoka_sum += Number(info1["KYOKANUM"]);
        tyoden_sum += Number(info1["TYODENNUM"]);
    }

    let info2 = infoArray[2];

    if (info2 !== undefined) {
        // �����E���ԁE���d���o�l_��Ж��i�o����j
        dtCondolence["SASHIDASHI_SAKI_KAISYA1"] = info2["KAISHAMEI"];
        // �����E���ԁE���d���o�l_��Б�\�Ҍ������i�o����j
        dtCondolence["SASHIDASHI_SAKI_KAISYA2"] = info2["YAKUSHOKU"];
        // �����E���ԁE���d���o�l_��Б�\�Ҏ����i�o����j
        dtCondolence["SASHIDASHI_SAKI_KAISYA3"] = info2["SHIMEI"];
        // �o�����Ђ�荁��
        dtCondolence["KOURYOU_SAKI_KAISYA"] = info2["KORYONUM"];
        // �o�����Ђ�苟�Ԃ̐�
        dtCondolence["KUGE_SAKI_KAISYA"] = info2["KYOKANUM"];
        // �o�����Ђ�蒢�d�L��
        dtCondolence["TYOUDEN_SAKI_KAISYA"] = info2["TYODENNUM"];

        koryo_sum += Number(info2["KORYONUM"]);
        kyoka_sum += Number(info2["KYOKANUM"]);
        tyoden_sum += Number(info2["TYODENNUM"]);
    }


    // �������v
    dtCondolence["KOURYOU_GOUKEI"] = koryo_sum;
    // ���ԍ��v
    dtCondolence["KUGE_GOUKEI"] = kyoka_sum;
    // ���d���v
    dtCondolence["TYOUDEN_GOUKEI"] = tyoden_sum;

}

/**
 * �ϔC�ҏ���ݒ�
 * */
function setDelegatorInfo() {
    var ht = new HashTblCommon();
    let shainbango = $("#text-unhappiness-shainbango").val();
    if (shainbango === STRING_EMPTY) {
        shainbango = dtCondolence["UNFORTUNATE_SHAINBANGO"];
    }

    if (shainbango !== null) {
        ht.Add("SHAINBANGO", shainbango);
        wfCommon.getApiInfoAjaxCallBack(GET_CONDOLENCE_SHAIN_INFO_APINAME, ht, setDelegatorItems);
    }

}

/**
 * �d���`�F�b�N
 * */
function doubleCheck() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Condolence");
    handler.AddUrlData();
    // �s�K�]�ƈ��Ј��ԍ�
    handler.AddPara("shainbango", dtCondolence["UNFORTUNATE_SHAINBANGO"]);
    // �S���Ȃ�ꂽ���J�i�����i���j
    handler.AddPara("kanashimeisei", $("#text-died-lastname_kana").val());
    // �S���Ȃ�ꂽ���J�i�����i���j
    handler.AddPara("kanashimeimei", $("#text-died-firstname_kana").val());
    // �S���Ȃ�ꂽ�������i���j
    handler.AddPara("shimeisei", $("#text-died-lastname").val());
    // �S���Ȃ�ꂽ�������i���j
    handler.AddPara("shimeimei", $("#text-died-firstname").val());
    // �S���Ȃ�ꂽ�������敪
    handler.AddPara("zokugarakbn", $("#select-died-relationship").val());
    // �S���Ȃ�ꂽ������
    handler.AddPara("seibetsu", $("input[name=radio-died-gender]:checked").val());
    // �S���Ȃ�ꂽ�������ʋ��敪
    handler.AddPara("dokyobekyo", $("input[name=radio-died-living]:checked").val());
    // �S���Ȃ�ꂽ���N��
    handler.AddPara("nenrei", $("#text-died-age").val());
    // �S���Ȃ�ꂽ��������
    handler.AddPara("seikyobi", $("#text-died-death_date").val());
    // �S���Ȃ�ꂽ��������
    handler.AddPara("seikyojikoku", $("#text-died-death_time").val());

    var result = handler.DoMethodReturnString("Check_Double_Info");
    if (result.indexOf("err@") === 0) {
        wfCommon.Msgbox(result);
        return;
    }
    return JSON.parse(result);
}

/**
  * PDF�����Z�b�g
  * @param {string} callback CALLBACK�֐�
  */
function setPDFInfo(callback) {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Condolence");
    handler.AddUrlData();

    // ��ЃR�[�h
    handler.AddPara("kaishacode", dtApplicant["KAISHACODE"]);

    handler.DoMethodSetString("Get_PDF_Info", function (data) {
        //��O����
        if (data.indexOf("err@") === 0) {
            wfCommon.Msgbox(data);
            return;
        }
        // JSON�Ώۂɓ]��
        var ret = JSON.parse(data);
        callback(ret);
    });
}

/**
 * PDF�@URL��ݒ�
 * 
 * @param {any} data
 */
function setPDFInfoItems(data) {

    dtCondolence["PDF_URL"] = data["Get_PDF_Info"];
    $("#iframe-pdf").attr("src", dtCondolence["PDF_URL"]);

}

/**
 * �m�F��ʂ֑J�ڂ���
 *
 * */
function gotoConfirmPage() {
    // �m�F��ʂɃf�[�^���Z�b�g
    setDataToConfirmPage();

    // �m�F��ʂ֑J��
    let confirmPage = $("#confirm-page")[0].__component;
    confirmPage.opened = !0;
}

/**
 * �\���C�� �e���ځF�����E�񊈐���ݒ�
 *
 * */
function changeItemsDisabled() {
    $("#text-unhappiness-shainbango").attr("disabled", true);
    $("#btn-search-empcode").attr("disabled", true);
    $("#text-died-lastname").attr("disabled", true);
    $("#text-died-firstname").attr("disabled", true);
    $("#text-died-lastname_kana").attr("disabled", true);
    $("#text-died-firstname_kana").attr("disabled", true);
    $("#pulldown-died-relationship")[0].__component._choices.disable();
    $("#pulldown-died-relationship").addClass("a-pulldown--disabled").children().addClass("is-disabled");
    $("input[name=radio-died-gender]").attr("disabled", true);
    $("input[name=radio-died-living]").attr("disabled", true);
    $("#text-died-age").attr("disabled", true);
    $("#text-died-death_date").attr("disabled", true);
    $("#text-died-death_date").parents(".a-calendar-field").addClass("a-calendar-field--disabled")
    $("#pulldown-died-death_time")[0].__component._choices.disable();
    $("#pulldown-died-death_time").addClass("a-pulldown--disabled").children().addClass("is-disabled");
    $("input[name=radio-died-dependent]").attr("disabled", true);
    $("input[name=radio-mourner]").attr("disabled", true);
    $("#text-mourner-lastname").attr("disabled", true);
    $("#text-mourner-firstname").attr("disabled", true);
    $("#text-mourner-lastname_kana").attr("disabled", true);
    $("#text-mourner-firstname_kana").attr("disabled", true);
}
/**
 * �����E���ԁE���d ���ځF�����E�񊈐���ݒ�
 * 
 * */
function changeOpenerItemsDisabled() {
    // �������v
    if (dtCondolence["KOURYOU_GOUKEI"] === 0) {
        wfCommon.radiosSetVal("radio-opener-koryo", NECESSARY_KBN_JITAI, NECESSARY_KBN_JITAI);
        $("input[name=radio-opener-koryo]").attr("disabled", true);
    } else {
        // �������������ꍇ
        if (String(dtCondolence["KORYOKBN"]) === NECESSARY_KBN_HITUYOU) {
            $("input[name=radio-opener-koryo]").attr("disabled", true);
        }
    }
    // ���ԍ��v
    if (dtCondolence["KUGE_GOUKEI"] === 0) {
        wfCommon.radiosSetVal("radio-opener-kuge", NECESSARY_KBN_JITAI, NECESSARY_KBN_JITAI);
        $("input[name=radio-opener-kuge]").attr("disabled", true);
    } else {
        // ��z�s�\
        if (String(dtCondolence["TEHAIKBN"]) === STATE_TEHAIZIMI) {
            $("input[name=radio-opener-kuge]").attr("disabled", true);
        }
    }
    // ���d���v
    if (dtCondolence["TYOUDEN_GOUKEI"] === 0) {
        wfCommon.radiosSetVal("radio-opener-tyoden", NECESSARY_KBN_JITAI, NECESSARY_KBN_JITAI);
        $("input[name=radio-opener-tyoden]").attr("disabled", true);
    } else {
        // ��z�s�\
        if (String(dtCondolence["TEHAIKBN"]) === STATE_TEHAIZIMI) {
            $("input[name=radio-opener-tyoden]").attr("disabled", true);
        }
    }
}

/**
 * �{�^���F�\���E��\����ݒ�
 *
 * */
function setButtonDisplay() {

    if (wfstate <= WF_STATE_DRAFT) {
        // ������
        $("#btn-form-draft_save").show();
        // �m�F
        $("#btn-form-confirm").show();
        // �\��
        $("#btn-form-request").show();
        // �C��
        $("#btn-form-modify").hide();

    } else {
        // ������
        $("#btn-form-draft_save").hide();
        // �m�F
        $("#btn-form-confirm").show();
        $("#btn-form-confirm").children().text("�\���C��");
        // �\��
        $("#btn-form-request").hide();
        // �C��
        $("#btn-form-modify").show();
        $(".f-modify").hide();
    }
}

/**
 * ���͂���G���A�F�\���E��\����ݒ�
 *
 * */
function setAddresseeDisplay() {
    let kugekbn = $("input[name=radio-opener-kuge]:checked").val();
    let tyodenkbn = $("input[name=radio-opener-tyoden]:checked").val();
    let allnight = $("input[name=radio-allnight]:checked").val();

    if (kugekbn === NECESSARY_KBN_JITAI && tyodenkbn === NECESSARY_KBN_JITAI) {
        $(".area-allnight").hide();
        if (allnight === LOCATION_ATOKA_KBN) {
            $(".area-rear").hide();
        }
    } else {
        $(".area-allnight").show();
        if (allnight === LOCATION_ATOKA_KBN) {
            $(".area-rear").show();
        }
    }
}
/*���֐���`�G���A��*/