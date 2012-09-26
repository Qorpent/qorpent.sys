var actionform = {
    submit: function(target) {
        target.setAttribute("action", location.href.replace("form", document.querySelector('#formrender').value));
        target.submit();
    }
}