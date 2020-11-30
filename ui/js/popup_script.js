  var currentUrl;
  chrome.tabs.query({active: true, currentWindow: true}, function(tabs){
    if(tabs.length != 0) {
        currentUrl = tabs[0].url;
    }
});

 document.addEventListener("DOMContentLoaded", function (dcle) {
     var createContactButton = document.getElementById("cc");
	 var searchButton = document.getElementById("button-addon2");
	 var openSelectEntity = document.getElementById("oc");
	 var openEntityList = document.getElementById("acc");
	 var enterForSearchEntity = document.getElementById("searchForm");
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
 });
	
function searchEntity() {
	var resultTable = document.getElementById("prokrutkaid");
	if (resultTable.children.length > 0) {
		resultTable.innerHTML = '';
	}
	var inputLine = document.getElementById("searchForm");
	if (!inputLine && inputLine.value.length == 0) return;
	var config = getConfigObject();
	var searchField = defineField(inputLine.value, config);
	var filterParam = getFilterParam(searchField, inputLine.value);
	var entities = new Array();
	entities = getEntities(searchField, config, filterParam);
	if (entities.length == 0) return;
	putInResultTableSpan(entities);
}

function putInResultTableSpan(entities) {
    debugger;
    var resultTableDiv = document.getElementById("prokrutkaid");
		entities.forEach(function(item, i, entities) {
        var divEl = document.createElement("div");
        var pEl = document.createElement("p");
        pEl.style.lineHeight = '1.1';
        pEl.id = item.contactid;
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
            openEntityTestFunc(pEl.id)
        });
    });
}

var curEntityId;
function selectEntityFunc(a) {
	curEntityId = a.path[0].id;
    var openSelectEntity = document.getElementById("oc");
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

const setSelect = ( e ) => openEntityTestFunc(curEntityId);

function openEntityTestFunc(id) {
	debugger;
	var config = getConfigObject();
	debugger;
	if (config) {
		var url = config.Address + "main.aspx?etn=" + config.FindEntityRecord + "&id={" + id + "}&pagetype=entityrecord";
		debugger;
		window.open(url, "_blank"); 
	}
}

function getFilterParam(searchField, inputValue) {
	var filterParam = "&$filter=";
	if (/\d/.test(inputValue)) {
		if (inputValue.includes('+')) {
			inputValue = inputValue.replace("+", "%2B");
		}
		filterParam += searchField + " eq '" + inputValue + "'";
		return filterParam;
	} else {
		filterParam += "startswith(" + searchField + ", '" + inputValue + "')";
		return filterParam;
	}
}

function getEntities(searchField, config, filterParam) {
	var typeEntity = config.isActivity == "true" ? "activity" : config.FindEntityRecord;
	var selectFields = getFieldForConnectingString(config);
	var oDataEndpointUrl = config.Address + "api/data/v9.0/contacts?$select=" + typeEntity + "id" + selectFields + filterParam;
    var service = GetRequestObject();
    if (service != null) {
        service.open("GET", oDataEndpointUrl, false);
        service.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        service.setRequestHeader("Accept", "application/json, text/javascript, */*");
        service.send(null);
        var retrieved = JSON.parse(service.responseText);
        if (retrieved.value.length > 0) {
            return retrieved.value;
        }
    }
}

function getFieldForConnectingString(config) {
	var selectFields = "";
	for	(var i = 0; i < config.SearchFields.length; i++) {
		selectFields += ", " + config.SearchFields[i];
	}
	return selectFields;
}

function defineField(inputValue, config) {
	if (/\d/.test(inputValue)) {
		return config.SearchFields[1];
	} else {
		return config.SearchFields[0];
	}
}

function openEntityListFunc() {
	var config = getConfigObject();
	if (config) {
		var url = config.Address+"main.aspx?etn=" + config.FindEntityRecord + "&pagetype=entitylist&viewid=%7b00000000-0000-0000-00AA-000010003006%7d&viewtype=1039";
		window.open(url, "_blank"); 
	}
}
	
function openEntity() {
	var openSelectEntity = document.getElementById("exampleFormControlSelect2");
	var config = getConfigObject();
	if (config && openSelectEntity) {
		var url = config.Address + "main.aspx?etn=" + config.FindEntityRecord + "&id={" + openSelectEntity.value + "}&pagetype=entityrecord";
		window.open(url, "_blank"); 
	}
}

	
function createEntity() {
	var config = getConfigObject();
	if (config) {
		var url = config.Address+"main.aspx?etn=" + config.FindEntityRecord + "&pagetype=entityrecord";
		window.open(url, "_blank"); 
	}
}

function getConfigObject() {
    var cutVar = currentUrl.split('/');
    var address = cutVar[0] + '/' + cutVar[1] + '/' + cutVar[2] + '/' + cutVar[3];
    var oDataEndpointUrl = address + "/WebResources/new_SidePanelConfig";
    var service = GetRequestObject();
    if (service != null) {
        service.open("GET", oDataEndpointUrl, false);
        service.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        service.setRequestHeader("Accept", "application/json, text/javascript, */*");
        service.send(null);
		var retrieved = service.responseText;
        if (retrieved.length > 0) {
			obj = eval(retrieved);
            return obj;
        }
    }
}

function GetRequestObject() {
    if (window.XMLHttpRequest) {
        return new window.XMLHttpRequest;
    } else {
        try {
            return new ActiveXObject("MSXML2.XMLHTTP.3.0");
        } catch (ex) {
            return null;
        }
    }
}
