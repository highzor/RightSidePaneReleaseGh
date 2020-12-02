(function () {
	function BuildPowerPaneButton2() {
		var powerPaneButton2div = document.createElement("div");
		powerPaneButton2div.setAttribute('id', 'navTabGMCSButton2div');
		powerPaneButton2div.setAttribute('style', 'float:right; width:50px; height:48px;cursor:pointer!important; z-index: 99999; position:relative;margin-top: 30px');
		var powerPaneButton2 = document.createElement("span");
		powerPaneButton2.setAttribute('class', 'navTabGMCSButton2');
		powerPaneButton2.setAttribute('id', 'crm-gmcs-button2');
		powerPaneButton2.setAttribute('title', 'GMCS CRM2');
		powerPaneButton2.setAttribute('onmouseover', 'this.style.backgroundColor="#666666"');
		powerPaneButton2.setAttribute('onmouseout', 'this.style.backgroundColor=""');
		var linkElement2 = document.createElement("a");
		linkElement2.setAttribute("class", "navTabGMCSButtonLink2");
		linkElement2.setAttribute("title", "");
		var linkImageContainerElement2 = document.createElement("span");
		linkImageContainerElement2.setAttribute("class", "navTabGMCSButtonImageContainer2");
		var imageElement2 = document.createElement("img");
		imageElement2.setAttribute("src", chrome.extension.getURL("img/icon-24.png"));
		powerPaneButton2.setAttribute('style', 'float:right; width:50px; height:48px;cursor:pointer!important; z-index: 99999');
		linkElement2.setAttribute("style", "float:right; width:50px; height:48px;cursor:pointer!important;text-align:center");
		imageElement2.setAttribute("style", "padding-top:10px");
		linkImageContainerElement2.appendChild(imageElement2);
		linkElement2.appendChild(linkImageContainerElement2);
		powerPaneButton2.appendChild(linkElement2);
		powerPaneButton2div.appendChild(powerPaneButton2);
		return powerPaneButton2div;
	}
	
	function InjectPowerPaneButton2() {
		var powerPaneButton2div = BuildPowerPaneButton2();
		if (document) {
			document.body.appendChild(powerPaneButton2div);
			return true;
		}
		return false;
	};
	
	function Initialize2() {
		var powerPaneButton2 = document.getElementById("crm-gmcs-button2");
		if (!powerPaneButton2) {
			var injectButtonResult2 = InjectPowerPaneButton2();
			if (injectButtonResult2 == false) {
				return;
			}
			document.getElementById("crm-gmcs-button2").addEventListener("click", slideRightPane2);
		}
	}

	var mainBody2 = document.querySelectorAll('#navBar');
	if (mainBody2 && mainBody2.length > 0) {
		Initialize2();
		var button2 = document.getElementById("crm-gmcs-button2");
		if (button2) {
			button2.addEventListener("click", slideRightPane2);
		} else {
			return;
		}
	}
	
	function slideRightPane2() {
		toggle();
	}
})();