$(function () {
    /*-------------Top-Button------------------*/
    // When the user scrolls down 20px from the top of the document, show the button
    window.onscroll = function () {
        scrollFunction();
    };

    function scrollFunction() {
        if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
            document.getElementById("topBtn").style.display = "block";
        } else {
            document.getElementById("topBtn").style.display = "none";
        }
    }

    // When the user clicks on the button, scroll to the top of the document
    $('#topBtn').click(function () {
        document.body.scrollTop = 0;
        document.documentElement.scrollTop = 0;
    });

    $('input[type=radio]').change(function () {
        ReSearch();
    });

    $('#search').click(function () {
        ReSearch();
    });

    $('#clearCache').click(function () {
        jobHub.jobSearch.clearAllCache().done(function () {
            ReSearch();
        });
    });

    $('#clearCache').click(function () {

    });

    function ReSearch() {
        var form = $('#SearchForm').serializeFormToObject();
        window.location.replace('/?' + $.param(form));
    }
});
