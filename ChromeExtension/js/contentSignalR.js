var crm;
var crmMasthead = document.getElementById('crmMasthead');
var mainBody = document.querySelectorAll('body[scroll=no]');
if (mainBody && mainBody.length > 0 && crmMasthead) {
	chrome.storage.sync.get('shortNumber', function (item) {
		if (item.shortNumber && item.shortNumber.length > 0) {
			connectSignalR(item.shortNumber);
		}
	});
}

var tryReconnect = false;
async function connectSignalR(shortNumber) {
	var result = 404;
	$.connection.hub.url = "http://localhost:56623/signalr";
	crm = $.connection.crmHub;
	crm.client.IncomingCall = function (caller) {
		chrome.storage.sync.set({'callData': {callId: caller.CallId, callDate: caller.DateOfCall, phoneNumber: caller.PhoneOfCaller, fullName: caller.FullName, dateOfBirth: caller.DateOfBirth, contactId: caller.ContactId, phoneCallId: caller.PhoneCallId}});
		openPhonePage();
	};

	crm.client.BackToPage = function () {
		backToPageFunc();
	};
	
	if (shortNumber) {
		$.connection.hub.qs = { 'shortNumber': shortNumber };
	}
	$.connection.hub.error(function (error) {
		console.log(error);
	});
	await $.connection.hub.start().done(function (response) {
		result = 200;
	}).catch(function (error) {
		
		console.log(error.message);
	});

	$.connection.hub.reconnecting(function (item) {
		
		tryReconnect = true;
		chrome.runtime.sendMessage({method: 'showModal'});
	})	

	$.connection.hub.reconnected(function() {
		
		tryReconnect = false;
		chrome.runtime.sendMessage({method: 'hideModal'});
	});

	$.connection.hub.disconnected(function() {
		
		chrome.runtime.sendMessage({method: 'disconnectModal', status: tryReconnect});
		$.connection.hub.stop();
	});
	return result;
}

function openPhonePage() {
	var iframe = document.getElementById("mySlide");
	iframe.src = chrome.extension.getURL("ui/popupPhone.html");
}

function backToPageFunc() {
	var iframe = document.getElementById("mySlide");
	iframe.src = chrome.extension.getURL("ui/popup.html");
}

async function signInFunc(inputNumber) {
	var result;
	await crm.server.signIn(inputNumber).promise().then(res => {
		if (res.IsError) $.connection.hub.stop();
		result = res.Code;
	});
	return result;
}

async function signOutFunc(inputNumber) {
	var result;
	await crm.server.signOut(inputNumber).promise().then(res => {
		result = res.Code;
		$.connection.hub.stop();
	});
	return result;
}

async function completeCallFunc(callId, completeDate, reason) {
	var result;
	await crm.server.completeCall(callId, completeDate, reason).promise().then(res => {
		result = res.Code});
	return result;
}

async function answerFunc(callId) {
	var result;
	await crm.server.answer(callId).promise().then(res => {
		result = res});
	return result;
}

function openEntityCurrWindowFunc(entity ,entityId) {
	var entityObj = {
		entity: entity,
		entityId: entityId
	};
	var jsonObj = JSON.stringify(entityObj);
	var injectedCode = "openEntityCurrWindow("+jsonObj+")";
	var script = document.createElement('script');
	script.appendChild(document.createTextNode('('+ injectedCode +')();'));
	document.body.appendChild(script);

}

chrome.runtime.onMessage.addListener(
	(response, sender, sendResponse) => {
		
		switch (response.method) {
			case 'signIn':
			signInFunc(response.inputNumber).then(sendResponse);
			break;
			case 'signOut':
			signOutFunc(response.inputNumber).then(sendResponse);
			break;
			case 'completeCall':
			completeCallFunc(response.callId, response.completeDate, response.reason).then(sendResponse);
			break;
			case 'answer':
			answerFunc(response.callId).then(sendResponse);
			break;
			case 'openEntityCurrWindow':
			openEntityCurrWindowFunc(response.entity, response.entityId);
			break;
		}
		return true;
	});


chrome.runtime.onMessage.addListener(
	(response, sender, sendResponse) => {
		if (response.method != 'connectSignalR') return;
		connectSignalR().then(sendResponse);
		return true;
	});