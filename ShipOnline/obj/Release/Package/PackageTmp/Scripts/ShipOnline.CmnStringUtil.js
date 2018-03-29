
function replaceALL(ValueString, TargetString, ReplaceString) {
    var ReturnString = String(ValueString);
    while (ReturnString.indexOf(TargetString) != -1) {
        ReturnString = ReturnString.replace(TargetString, ReplaceString);
    }
    return ReturnString;
}

/**
  null を別の文字列に変換します。

  targetString  : 対象文字列
  defaultString : デフォルト文字列

  戻り値： 対象文字列が null でない場合は、対象文字列をそのまま返す。
           対象文字列が null の場合は、 デフォルト文字列を返す。
**/
function nvl(targetString, defaultString) {
    var value = targetString;

    if (value == null) {
        value = defaultString;
    }

    return value;
}

