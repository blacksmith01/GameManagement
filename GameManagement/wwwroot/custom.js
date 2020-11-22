window.customJsFunctions =
{
    focusElement: function (id) {
        const element = document.getElementById(id);
        element.focus();
    },
    showToast: function () {
        $(".toast").toast('show');
    }
};