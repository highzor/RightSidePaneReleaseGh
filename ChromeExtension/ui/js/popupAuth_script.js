document.addEventListener("DOMContentLoaded", function (dcle) {
 document.getElementById("button-signIn").addEventListener("click", signIn);
 var enterForLogIn = document.getElementById("exampleInputPhoneNumber");
 debugger;
 if (enterForLogIn) {
  enterForLogIn.removeEventListener("keydown").addEventListener("keydown", function (e) {
    debugger;
    if (e.keyCode === 13) {
      debugger;
      e.preventDefault();
      document.getElementById("button-signIn").click();

    } 
  });
}
});

function signIn() {
  debugger;
  var value = document.getElementById("exampleInputPhoneNumber").value;
  chrome.runtime.sendMessage({method: 'connectSignalR'}, function (response) {
    debugger;
    if (response == '200') {
      var count = 0;
      chrome.runtime.sendMessage({inputNumber: value, method: 'signIn'}, function (response) {
        if (response == '200') {
          debugger;
          chrome.storage.sync.set({'shortNumber': value});
          openPage();
        } else {
          debugger;
          errorItemFunc(response);
        }
      });
    } else {
      debugger;
      errorItemFunc(response);
    }
  });
}

function openPage() {
 var openPageScript = ''
 + 'var iframe = document.getElementById("mySlide");'
 + 'iframe.src = chrome.extension.getURL("ui/popup.html");'
 chrome.tabs.executeScript({
  code: openPageScript
});
}

function errorItemFunc(result) {
  debugger;
  var elem = document.createElement("p");
  elem.style.color = 'red';
  elem.appendChild(document.createTextNode('Error: ' + result));
  var firstElem = document.getElementById("errordiv");
  if (firstElem && firstElem.childElementCount > 0) firstElem.replaceChildren();
  firstElem.appendChild(elem);
}