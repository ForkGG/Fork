// Disable all default context menus
document.addEventListener('contextmenu', e => {
    // TODO CKE enable only on PROD
    // e.preventDefault();
})


// Scroll to bottom of the element if autoscroll is enabled
function ScrollToBottom(element) {
    element.scrollTop = element.scrollHeight - element.clientHeight;
}

function IsScrolledToBottom(element) {
    return element.scrollTop + element.clientHeight >= element.scrollHeight;
}


