chrome.extension.onMessage.addListener(
	function(request, sender, sendResponse) 
	{
		chrome.tabs.sendMessage(sender.tab.id, request , function(response) {
			var lastError = chrome.runtime.lastError;
			if (lastError) {
				sendResponse(lastError.message);
				return;
			}
			sendResponse(response);
		})
		return true;
	});  