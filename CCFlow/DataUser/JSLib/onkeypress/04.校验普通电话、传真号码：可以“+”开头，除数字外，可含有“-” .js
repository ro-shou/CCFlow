﻿function isTel(s) {
    //var patrn=/^[+]{0,1}(\d){1,3}[ ]?([-]?(\d){1,12})+$/; 
    var patrn = /^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$/;
    if (!patrn.exec(s.value)) 
    {
       alert('不正な電話番号フォーマット.');
       return false;
    }
    return true
} 