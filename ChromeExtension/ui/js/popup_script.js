	  var currentUrl;
	  chrome.tabs.query({active: true, currentWindow: true}, function(tabs){
	  	if(tabs.length != 0) {
	  		currentUrl = tabs[0].url;
	  		getConfigObject();
	  	}
	  });

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
	  			if (response.status == true) {
	  				$('#unsuccesModal').modal('show');
	  				setTimeout("$('#unsuccesModal').modal('hide');",3000);}
	  				break;
	  			}
	  		});

	  document.addEventListener("DOMContentLoaded", function (dcle) {
	  	var createContactButton = document.getElementById("button-newContact");
	  	var searchButton = document.getElementById("button-addon2");
	  	var openSelectEntity = document.getElementById("button-openContact");
	  	var openEntityList = document.getElementById("button-allContacts");
	  	var enterForSearchEntity = document.getElementById("searchForm");
	  	var logOutButton = document.getElementById("button-LogOut");
	  	var TESTButton = document.getElementById("button-TEST");
	  	if (createContactButton) {
	  		createContactButton.addEventListener("click", createEntity);
	  	}
	  	if (searchButton) {
	  		searchButton.addEventListener("click", searchEntity);
	  	}
	  	if (openEntityList) {
	  		openEntityList.addEventListener("click", openEntityListFunc);
	  	}
	  	if (enterForSearchEntity) {
	  		enterForSearchEntity.addEventListener("keypress", function (e) {
	  			if (e.key === "Enter") searchEntity(); 
	  		});
	  	}
	  	if (logOutButton) {
	  		logOutButton.addEventListener("click", signOut);
	  	}
	  	if (TESTButton) {
	  		TESTButton.addEventListener("click", function () {
	  			var changePageScript = ''
	  			+ 'var iframe = document.getElementById("mySlide");'
	  			+ 'iframe.src = chrome.extension.getURL("ui/popupPhone.html");'
	  			+ 'sessionStorage.shortNumber = "";' 
	  			chrome.tabs.executeScript({
	  				code: changePageScript
	  			});
	  		});
	  	}
	  });

	  function signOut() {
	  	chrome.storage.sync.get('shortNumber', function (item) {
	  		chrome.runtime.sendMessage({inputNumber: item.shortNumber, method: 'signOut'}, function (response) {
	  			var lastError = chrome.runtime.lastError;
	  			if (lastError) {
	  				console.log(lastError.message);
	  			}
	  			if (response !== '200') console.log(response);
	  			openPage();
	  		});
	  	});
	  	chrome.storage.sync.clear();
	  }

	  function openPage() {
	  	var openScript = ''
	  	+ 'var iframe = document.getElementById("mySlide");'
	  	+ 'iframe.src = chrome.extension.getURL("ui/popupAuth.html");'
	  	chrome.tabs.executeScript({
	  		code: openScript
	  	});
	  }

	  function searchEntity() {
	  	var resultTable = document.getElementById("prokrutkaid");
	  	if (resultTable.children.length > 0) {
	  		resultTable.innerHTML = '';
	  	}
	  	var inputLine = document.getElementById("searchForm");
	  	if (!inputLine && inputLine.value.length == 0) return;
	  	var config = JSON.parse(sessionStorage.config);
	  	if (!config && config == 'undefined') return;
	  	var filterParam = getFilterParamFunc(inputLine.value);
	  	var entities = new Array();
	  	getEntities(config, filterParam);
	  }

	  function putInResultTableSpan(entities) {
	  	var resultTableDiv = document.getElementById("prokrutkaid");
	  	entities.forEach(function(item, i, entities) {
	  		var divEl = document.createElement("div");
	  		var pEl = document.createElement("p");
	  		pEl.style.lineHeight = '1.1';
	  		pEl.id = item.contactid;
	  		pEl.style.cursor = 'pointer';
	  		pEl.style.marginBottom = '0';
	  		pEl.style.paddingBottom = '16px';
	  		var string = '';
	  		if (item.fullname != null) {
	  			string += 'Контакт: ' + item.fullname;
	  		}
	  		if (item.telephone1 != null) {
	  			string += '<br />Телефон: ' + item.telephone1;
	  		}
	  		if (item.birthdate != null) {
	  			string += '<br />Дата рождения: ' + item.birthdate;
	  		}
	  		pEl.innerHTML = string;
	  		resultTableDiv.appendChild(pEl);
	  		pEl.addEventListener("click", function (a) {
	  			a.preventDefault();
	  			selectEntityFunc(a)
	  		});
	  		pEl.addEventListener("dblclick", function (e) {
	  			e.preventDefault();
	  			openEntityFunc(pEl.id)
	  		});
	  	});
	  }

	  var curEntityId;
	  function selectEntityFunc(a) {
	  	curEntityId = a.path[0].id;
	  	var openSelectEntity = document.getElementById("button-openContact");
	  	var selectedPEntity = document.getElementById(a.path[1].id);
	  	if (openSelectEntity) {
	  		if (a.path[1].children.length > 1) {
	  			for (var i = 0; i < a.path[1].children.length; i++) {
	  				a.path[1].children[i].style.backgroundColor = '';
	  				openSelectEntity.removeEventListener("click", setSelect);
	  			}
	  		}
	  		a.path[0].style.backgroundColor = '#e2e7ec';
	  		openSelectEntity.addEventListener("click", setSelect);
	  	}
	  }

	  const setSelect = ( e ) => openEntityFunc(curEntityId);

	  function openEntityFunc(id) {
	  	var config = JSON.parse(sessionStorage.config);
	  	if (config && config != 'undefined') {
	  		var url = config.Address + "main.aspx?etn=" + config.FindEntityRecord + "&id={" + id + "}&pagetype=entityrecord";
	  		window.open(url, "_blank"); 
	  	}
	  }

	  function getFilterParamFunc(inputValue) {
	  	var filterParam = "&$filter=";
	  	if (inputValue.includes('+')) {
	  		inputValue = inputValue.replace("+", "%2B");
	  	}
	  	if (inputValue.includes('.')) {
	  		var dateOfBirth = inputValue.split('.');
	  		filterParam += 'birthdate eq '+dateOfBirth[2]+'-'+dateOfBirth[1]+'-'+dateOfBirth[0];
	  		return filterParam;
	  	} else {
	  		filterParam += "contains(fullname, '"+inputValue+"') or contains(telephone1, '"+inputValue+"')";
	  		return filterParam;
	  	}
	  }

	  async function getEntities(config, filterParam) {
	  	var typeEntity = config.isActivity == "true" ? "activity" : config.FindEntityRecord;
	  	var selectFields = getFieldForConnectingString(config);
	  	var oDataEndpointUrl = config.Address + "api/data/v9.0/contacts?$select=" + typeEntity + "id" + selectFields + filterParam;
	  	var xhr = new XMLHttpRequest();
	  	xhr.open("GET", oDataEndpointUrl, true);
	  	xhr.onload = function (e) {
	  		if (xhr.readyState === 4) {
	  			if (xhr.status === 200) {
	  				entities = JSON.parse(xhr.responseText);
	  				putInResultTableSpan(entities.value);
	  			} else {
	  				console.error(xhr.statusText);
	  			}
	  		}
	  	}
	  	xhr.send(null);
	  }

	  function getFieldForConnectingString(config) {
	  	var selectFields = "";
	  	for	(var i = 0; i < config.SearchFields.length; i++) {
	  		selectFields += ", " + config.SearchFields[i];
	  	}
	  	return selectFields;
	  }

	  function openEntityListFunc() {
	  	var config = JSON.parse(sessionStorage.config);
	  	if (config && config != 'undefined') {
	  		var url = config.Address+"main.aspx?etn=" + config.FindEntityRecord + "&pagetype=entitylist&viewid=%7b00000000-0000-0000-00AA-000010003006%7d&viewtype=1039";
	  		window.open(url, "_blank"); 
	  	}
	  }

	  function createEntity() {
	  	var config = JSON.parse(sessionStorage.config);
	  	if (config && config != 'undefined') {
	  		var url = config.Address+"main.aspx?etn=" + config.FindEntityRecord + "&pagetype=entityrecord";
	  		window.open(url, "_blank"); 
	  	}
	  }

	  async function getConfigObject() {
	  	if (!currentUrl.startsWith('http')) return;
	  	var cutVar = currentUrl.split('/');
	  	var address = cutVar[0] + '/' + cutVar[1] + '/' + cutVar[2] + '/' + cutVar[3];
	  	var oDataEndpointUrl = address + "/WebResources/new_SidePanelConfig";
	  	var xhr = new XMLHttpRequest();
	  	xhr.open("GET", oDataEndpointUrl, true);
	  	xhr.onload = function (e) {
	  		if (xhr.readyState === 4) {
	  			if (xhr.status === 200) {
	  				if (xhr.responseText.length == 0) return;
	  				obj = eval(xhr.responseText);
	  				sessionStorage.config = JSON.stringify(obj);
	  			} else return;
	  		}
	  	}
	  	try {
	  		xhr.send(null);
	  	} catch (e) {var bkg = chrome.extension.getBackgroundPage();
	  		bkg.console.log(e);
	  	}
	  }