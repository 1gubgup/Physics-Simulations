mergeInto(LibraryManager.library, {
     //Hello测试jslib可用性
     Hello: function ()  {
          window.alert("Hello, world!");
     },

     //交互环境初始化
     JSInit: function () {
          addBtns();
     },

     //点击导入文件按钮，调用JS方法clickSelectFileBtn
     ClickSelectFileBtn: function () {
          console.log("Import");
          clickSelectFileBtn();
     }, 

     //点击脚本“保存”按钮，将脚本名和脚本内容传给JS
     ClickSaveScript: function (filename,text) {
          //window.alert(Pointer_stringify(filename));
          download(Pointer_stringify(filename),Pointer_stringify(text));
     },

     //跳转到指定地址
     gotoLogin: function () {
          GotoLogin();
     },

     //获取cookie信息
     GetCookie: function () {
          getCookie();
     },

     //保存文件时选择路径
     /*ClickSelectFileFoldBtn: function () {
           console.log("select_route");
           clickSelectFileFoldBtn();
     },*/
});