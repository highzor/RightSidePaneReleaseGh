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

async function connectSignalR(shortNumber) {
	 
	var result = '404';
	$.connection.hub.url = "http://localhost:56623/signalr";
	crm = $.connection.crmHub;
	crm.client.IncomingCall = function (idOfCall, dateOfCall, caller, fullname, dateofbirth) {
		
		chrome.storage.sync.set({'callData': {callId: idOfCall, callDate: dateOfCall, phoneNumber: caller, fullName: fullname, dateOfBirth: dateofbirth}});
		openPage();
	};
	
	if (shortNumber) {
		$.connection.hub.qs = { 'shortNumber': shortNumber };
	}
	await $.connection.hub.error(function (error) {
		
		console.log(error);
	});
	await $.connection.hub.start().done(function (response) {
		
		result = '200';
		eventFunc();

	}).catch(function (error) {
		
		console.log(error.message);
	});
	$.connection.hub.reconnecting(function (item) {
		

	})	
	
	$.connection.hub.disconnected(function() {
		
		$.connection.hub.stop();
	});
	return result;
}

function openPage() {
	
	var iframe = document.getElementById("mySlide");
	iframe.src = chrome.extension.getURL("ui/popupPhone.html");
}


async function signInFunc(inputNumber) {
	
	var result;
	await crm.server.signIn(inputNumber).promise().then(res => {
		
		if (res != '200') $.connection.hub.stop();
		result = res;
	});
	return result;
}

async function signOutFunc(inputNumber) {
	
	var result;
	await crm.server.signOut(inputNumber).promise().then(res => {
		
		result = res;
		$.connection.hub.stop();
	});
	return result;
}

async function completeCallFunc(callId, completeDate, reason) {
	
	var result;
	await crm.server.completeCall(callId, completeDate, reason).promise().then(res => {
		
		result = res});
	return result;
}

async function answerFunc(callId) {
	
	var result;
	await crm.server.answer(callId).promise().then(res => {
		
		result = res});
	return result;
}

async function denyFunc(callId) {
	
	var result;
	await crm.server.deny(callId).promise().then(res => {
		
		result = res});
	return result;
}

function eventFunc() {
	chrome.runtime.onMessage.addListener(
		(response, sender, sendResponse) => {
			
			if (response.method == 'signIn' && response.inputNumber.length > 0) {
				
				signInFunc(response.inputNumber).then(sendResponse);
			}
			else if (response.method == 'signOut' && response.inputNumber.length > 0) {
				
				signOutFunc(response.inputNumber).then(sendResponse);
			} 
			else if (response.method == 'completeCall' && response.callId.length > 0) {
				
				completeCallFunc(response.callId, response.completeDate, response.reason).then(sendResponse);
			}
			else if (response.method == 'answer' && response.callId.length > 0) {
				
				answerFunc(response.callId).then(sendResponse);
			}
			else if (response.method == 'deny' && response.callId.length > 0) {
				
				denyFunc(response.callId).then(sendResponse);
			}
			return true;
		});
}

chrome.runtime.onMessage.addListener(
	(response, sender, sendResponse) => {
		if (response.method != 'connectSignalR') return;
		connectSignalR().then(sendResponse);
		return true;
	});