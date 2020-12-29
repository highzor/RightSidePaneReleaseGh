chrome.extension.onMessage.addListener(
	function(request, sender, sendResponse) 
	{
		debugger;
		chrome.tabs.sendMessage(sender.tab.id, request , function(response) {
			debugger;
			var lastError = chrome.runtime.lastError;
			if (lastError) {
				sendResponse(lastError.message);
				return;
			}
			sendResponse(response);
		})
		return true;
	});  