
document.addEventListener("DOMContentLoaded", function (dcle) {
document.getElementById("button-signIn").addEventListener("click", SignIn);
document.getElementById("exampleInputPhoneNumber").addEventListener("keypress", function (e) {
	if (e.key === "Enter") {
		e.preventDefault();
		SignIn();
	} 
});
});

  function SignIn() {
  	$.ajax({
  		type: "POST",
  		url: 'http://localhost:56623/Home/SignIn',
  		contentType: 'application/json;',
  		processData: false,
  		data: JSON.stringify({ inputNumber: document.getElementById("exampleInputPhoneNumber").value }),
  		success: function (result) {

  			if (result == "all right") {

  				sessionStorage.shortNumber = document.getElementById("exampleInputPhoneNumber").value;
  				openPage();
  			} else {

  				errorItemFunc(result);
  			}},
  			error: function (xhr, status, p3) {
  				errorItemFunc(xhr.responseText);
  			}
  		});
  }

  function openPage() {

  	var openPageScript = ''
  	+ 'var iframe = document.getElementById("mySlide");'
  	+ 'iframe.src = chrome.extension.getURL("ui/popup.html");'
  	+ 'sessionStorage.shortNumber = ' + sessionStorage.shortNumber
  	chrome.tabs.executeScript({
  		code: openPageScript
  	});
  }

  function errorItemFunc(result) {

  	let elem = document.createElement("p");
  	elem.style.color = 'red';
  	elem.appendChild(document.createTextNode('Error: ' + result));
  	let firstElem = document.getElementById("errordiv");
  	if (firstElem.childElementCount > 0) firstElem.replaceChildren();
  	firstElem.appendChild(elem);
  }