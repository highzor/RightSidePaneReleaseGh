document.addEventListener("DOMContentLoaded", function (dcle) {
  document.getElementById("button-Back").addEventListener("click", backToPage);
  document.getElementById("button-completeCall").addEventListener("click", completeCall);
  document.getElementById("button-answerCall").addEventListener("click", answer);

  callInfo();
});

var isAnswered = false;

function completeCall() {
  chrome.storage.sync.get('callData', function (fields) {
    
    if(!fields.callData) return;
    if (isAnswered) {
      chrome.runtime.sendMessage({callId: fields.callData.callId, completeDate: new Date(), reason: 'easy Reason', method: 'completeCall'}, function (response) {
        
        if (response == 200) backToPage();
      });
    } else {
      chrome.runtime.sendMessage({callId: fields.callData.callId, method: 'deny'}, function (response) {
        
        if (response == 200) backToPage();
      });
    }
  });
}

function answer() {
  chrome.storage.sync.get('callData', function (fields) {
    
    if(!fields.callData) return;
    chrome.runtime.sendMessage({callId: fields.callData.callId, method: 'answer'}, function (response) {
      if (response == 200) isAnswered = true;
    });
  });
}

function backToPage() {
  var leavePageScript = ''
  + 'var iframe = document.getElementById("mySlide");'
  + 'iframe.src = chrome.extension.getURL("ui/popup.html");' 
  chrome.tabs.executeScript({
    code: leavePageScript
  });
}

function callInfo() {
  chrome.storage.sync.get('callData', function (fields) {
    
    if(fields.callData) {
      buildPageCall(fields);
    } else {
      buildPageCallTest();
    };
  });
}

function buildPageCall(fields) {
  
  var container = document.getElementById("callerFields");
  var elem1 = document.createElement("p");
  elem1.appendChild(document.createTextNode(fields.callData.phoneNumber));
  container.appendChild(elem1);

  var elem2 = document.createElement("p");
  elem2.style.fontWeight = 'bold';
  elem2.appendChild(document.createTextNode(fields.callData.fullName));
  container.appendChild(elem2);

  var elem3 = document.createElement("p");
  var date = fields.callData.dateOfBirth;
  if (date == null) date = 'отсутствует';
  elem3.appendChild(document.createTextNode('Дата рождения: '+ date));
  container.appendChild(elem3);
}

function buildPageCallTest() {
  var container = document.getElementById("callerFields");
  var elem1 = document.createElement("p");
  elem1.appendChild(document.createTextNode('+7(909)777-77-77'));
  container.appendChild(elem1);

  var elem2 = document.createElement("p");
  elem2.style.fontWeight = 'bold';
  elem2.appendChild(document.createTextNode('Иван Иванов'));
  container.appendChild(elem2);

  var elem3 = document.createElement("p");
  elem3.appendChild(document.createTextNode('Дата рождения: 12.12.1970'));
  container.appendChild(elem3);
}