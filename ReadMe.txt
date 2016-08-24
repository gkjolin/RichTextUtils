所有的文本必须继承RichText 这里可以重新编译UGUI 将RichText添加到UI里边去 然后修改Editor代码 以后创建的代码都是继承与RichText 而不是Text
可以再游戏开始的时候初始化
//RichTextHelper.OnAdded();


测试代码
只要添加<关键字=value>即可
var herfString = "<color=#00ff00>打开<ui=9,摄政王>复命 </color><color=#FFFF00>米莉的葡萄</color>前往<color=#00ff00>北郡</color>";
Debug.Log(herfString);
