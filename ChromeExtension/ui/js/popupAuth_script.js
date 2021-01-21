document.addEventListener("DOMContentLoaded", function (dcle) {
 document.getElementById("button-signIn").addEventListener("click", signIn);
 var enterForLogIn = document.getElementById("exampleInputPhoneNumber");
 if (enterForLogIn) {
  enterForLogIn.addEventListener("keydown", function (e) {
    if (e.keyCode === 13) {
      e.preventDefault();
      document.getElementById("button-signIn").click();
    } 
  });
}
});

function signIn() {
  var value = document.getElementById("exampleInputPhoneNumber").value;
  chrome.runtime.sendMessage({method: 'connectSignalR'}, function (response) {
    if (response == 200) {
      chrome.runtime.sendMessage({inputNumber: value, method: 'signIn'}, function (response) {
        if (response == 200) {
          chrome.storage.sync.set({'shortNumber': value});
          openPage();
        } else {
          errorItemFunc(response);
        }
      });
    } else {
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
  var elem = document.createElement("p");
  elem.style.color = 'red';
  elem.appendChild(document.createTextNode('Error: ' + result));
  var firstElem = document.getElementById("errordiv");
  if (firstElem && firstElem.childElementCount > 0) firstElem.replaceChildren();
  firstElem.appendChild(elem);
}