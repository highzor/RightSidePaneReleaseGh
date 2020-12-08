document.addEventListener("DOMContentLoaded", function (dcle) {
document.getElementById("button-Back").addEventListener("click", backToPage);
});

  function backToPage() {
      var leavePageScript = ''
      + 'var iframe = document.getElementById("mySlide");'
      + 'iframe.src = chrome.extension.getURL("ui/popup.html");' 
      chrome.tabs.executeScript({
        code: leavePageScript
      });
    }