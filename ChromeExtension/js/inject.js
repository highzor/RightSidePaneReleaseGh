(function () {
	function BuildPowerPaneButton() {
		var powerPaneButton = document.createElement("span");
		powerPaneButton.setAttribute('class', 'navTabGMCSButton');
		powerPaneButton.setAttribute('id', 'crm-gmcs-button');
		powerPaneButton.setAttribute('title', 'GMCS CRM');
		powerPaneButton.setAttribute('onmouseover', 'this.style.backgroundColor="#666666"');
		powerPaneButton.setAttribute('onmouseout', 'this.style.backgroundColor="#000000"');
		var linkElement = document.createElement("a");
		linkElement.setAttribute("class", "navTabGMCSButtonLink");
		linkElement.setAttribute("title", "");
		var linkImageContainerElement = document.createElement("span");
		linkImageContainerElement.setAttribute("class", "navTabGMCSButtonImageContainer");
		var imageElement = document.createElement("img");
		imageElement.setAttribute("src", chrome.extension.getURL("img/icon-24.png"));
		powerPaneButton.setAttribute('style', 'float:right; width:50px; height:48px;cursor:pointer!important');
		linkElement.setAttribute("style", "float:right; width:50px; height:48px;cursor:pointer!important;text-align:center");
		imageElement.setAttribute("style", "padding-top:10px");
		linkImageContainerElement.appendChild(imageElement);
		linkElement.appendChild(linkImageContainerElement);
		powerPaneButton.appendChild(linkElement);
		return powerPaneButton;
	}
	
	function InjectPowerPaneButton() {
		var powerPaneButton = BuildPowerPaneButton();
		var ribbon = window.top.document.querySelector('#navBar');
		if (ribbon) {
			ribbon.prepend(powerPaneButton);
			return true;
		}
		return false;
	};
	
	function BuildScriptTag(source) {
		var script = document.createElement("script");
		script.setAttribute('type', 'text/javascript');
		script.setAttribute('src', source);
		return script;
	}
	
	function InjectSource(sources) {
		body = window.top.document.querySelector('body[scroll=no]');
		body.appendChild(sources);
	}
	
	function Initialize() {
		var powerPaneButton = document.getElementById("crm-gmcs-button");
		if (!powerPaneButton) {
			var injectButtonResult = InjectPowerPaneButton();
			if (injectButtonResult == false) {
				return;
			}
			document.getElementById("crm-gmcs-button").addEventListener("click", slideRightPane);
		}
	}
	var crmMasthead = document.getElementById('crmMasthead');
	var mainBody = document.querySelectorAll('body[scroll=no]');
	if (mainBody && mainBody.length > 0 && crmMasthead) {
		Initialize();
		var button = document.getElementById("crm-gmcs-button");
		if (button) {
			button.addEventListener("click", slideRightPane);
		} else {
			return;
		}
	} 
	
	function slideRightPane() {
		toggle();
	}
})();
