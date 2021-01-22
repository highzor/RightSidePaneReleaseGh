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
      chrome.runtime.sendMessage({callId: fields.callData.callId, completeDate: new Date(), reason: 'Звонок завершен', method: 'completeCall'}, function (response) {
      });
    } else {

      chrome.runtime.sendMessage({callId: fields.callData.callId, completeDate: new Date(), reason: 'Звонок сброшен без ответа', method: 'completeCall'}, function (response) {
      });
    }
  });
}


function openEntity() {
  chrome.runtime.sendMessage({entityId: "4357CEBC-102F-EB11-B810-005056964201", method: 'open'});
}

function answer() {
  chrome.storage.sync.get('callData', function (fields) {
    if(!fields.callData) return;
    chrome.runtime.sendMessage({callId: fields.callData.callId, method: 'answer'});
  });
}

  function successAnswer(incidentId) {
    chrome.storage.sync.get('callData', function (fields) {
      isAnswered = true;
    var incidentId = incidentId, contactId = fields.callData.contactId, phoneCallId = fields.callData.phoneCallId;
    openCurrentPage('contact', contactId);
    var answerButton = document.getElementById('button-answerCall');
    answerButton.setAttribute('disabled', 'disabled');
    answerButton.style.opacity = '0';
    InitializeButtons(incidentId, phoneCallId);
    });
  }

  function InitializeButtons(incidentId, phoneCallId) {
    document.getElementById("openCall").addEventListener("click", function() {openCurrentPage('phonecall', phoneCallId)});
    document.getElementById("openIncident").addEventListener("click", function() {openCurrentPage('incident', incidentId)});
    document.getElementById("controlPanel").style.display = 'block';
  }

  function openCurrentPage(entity, entityId) {
    chrome.runtime.sendMessage({method: 'openEntityCurrWindow', entity: entity, entityId: entityId});
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
    var phonenumber = fields.callData.phoneNumber;
    if (phonenumber == null) phonenumber = 'Номер неопределен';
    elem1.appendChild(document.createTextNode(fields.callData.phoneNumber));
    container.appendChild(elem1);
    var elem2 = document.createElement("p");
    elem2.style.fontWeight = 'bold';
    var fullname = fields.callData.fullName;
    if (fullname == null) fullname = 'Неизвестный контакт';
    elem2.appendChild(document.createTextNode(fullname));
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

  chrome.runtime.onMessage.addListener(
    (response, sender, sendResponse) => {
      switch (response.method) {
        case 'showModal':
        $('#reconnectModal').modal('show');
        break;
        case 'hideModal':
        $('#reconnectModal').modal('hide');
        break;
        case 'disconnectModal':
        $('#reconnectModal').modal('hide');
        if (response.statusReconnect == true) {
          $('#unsuccesModal').modal('show');
          setTimeout("$('#unsuccesModal').modal('hide');",3000);}
        break;
        case 'successAnswer':
        successAnswer(response.param);
        break;
        }
      });
