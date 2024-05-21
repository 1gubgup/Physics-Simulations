function addBtns() {
           //导入已有脚本的按钮，id为files
           var inp1 = document.createElement("input");
           inp1.type = "file";
           inp1.id = "files";
           inp1.style = "display:none";
           inp1.onchange = function() {
                     fileImport();
           };

           var x = document.getElementsByTagName("body");
           x[0].appendChild(inp1);
           //x[0].appendChild(inp2);
}

function clickSelectFileBtn()  {
           //点击一个隐藏的id为files的“选择文件”按钮
           var tempFileLayout = document.getElementById("files");
           tempFileLayout.click();
}

function sendFileToUnity(o) {
           //将读取成功的脚本名和脚本内容发送给Unity进行处理
           unityInstance.SendMessage("Controller","DealwithImport",o);
}

function FileisNotCS() {
           //读取的不是一个cs文件，应该引起unity里的异常提示
           unityInstance.SendMessage("Controller","NotCSimport");
}

function fileImport()  {
           //获取读取文件的File对象
           var selectedFile = document.getElementById("files").files[0];
           var fileName = selectedFile.name;
           var lastName = fileName.substring(fileName.lastIndexOf(".")+1);
           if(lastName == "cs") {
                fileName = fileName.substring(0,fileName.lastIndexOf("."));
                console.log(fileName);
                if (selectedFile != null) {
                    var reader = new FileReader();
                    reader.readAsText(selectedFile);
                    reader.onload = function(){
                         sendFileToUnity(fileName+"~"+reader.result);
                    }
                }         
            }
            else {
                   FileisNotCS();
            }
}

function download(filename,text) {
        //下载文件
        var element = document.createElement('a');
        element.setAttribute('href', 'data:text/plain;charset=utf-8,'+encodeURIComponent(text));
        element.setAttribute('download',filename);

        element.style.display = 'none';
        document.body.appendChild(element);

        element.click();
        document.body.removeChild(element);
}

function GotoLogin() {
       //跳转地址
       window.location.replace("https://ilabs.sjtu.edu.cn/lesson9/#/login");
}

function getCookie() {
       //获取Cookie信息
       const name='S_N';
       let start1,end1,start2,end2;
       let result = '';
       if (document.cookie.length > 0){
           start1 = document.cookie.indexOf(name+'=');
           if (start1  !== -1) {
                start1 = start1 + name.length + 1; //start1位于 'S_N=' 后第一个字符的位置
                end1 = document.cookie.indexOf('%',start1); //end1位于 '&(%26)S_I' 的%处
                if (end1 === -1) end1 = document.cookie.length;
                result += unescape(document.cookie.substring(start1,end1)) + '~';
                start2 = document.cookie.indexOf('D',start1)+1;//start2位于 'S_I=(%3D)' 后第一个字符的位置
                end2 = document.cookie.indexOf('%',start2);
                if (end2 === -1) end2 = document.cookie.length;
                result += unescape(document.cookie.substring(start2,end2));
           }
       }
       unityInstance.SendMessage("GM_Core","DealwithCookie",result);
}