(function () {
    function $(selector, context) {
        context = context || document;
        return context["querySelectorAll"](selector);
    }

    function forEach(collection, iterator) {
        for (var key in Object.keys(collection)) {
            iterator(collection[key]);
        }
    }

    function showMenu(menu) {
        if (this.tagName == "a") {
            var menuli = this;
        } else {
            var menuli = menu.target;
        }
        var ul = $("ul", menuli)[0];

        if (!ul || ul.classList.contains("-visible")) return;

        ul.classList.add("-visible");
        menu.target.previousElementSibling.classList.add("-visible")

    }

    function hideMenu(menu) {
        var menu = this;
        var ul = $("ul", menu)[0];

        if (!ul || !ul.classList.contains("-visible")) return;

        menu.classList.remove("-active");
        ul.classList.add("-animating");
        setTimeout(function () {
            ul.classList.remove("-visible");
            ul.classList.remove("-animating");
        }, 300);
    }
    function showMenuMobile(menu) {
        document.querySelectorAll(".-visible").forEach(function (element) {
            element.classList.remove("-visible");
        });
        if (this.tagName == "a") {
            var menuli = this;
        } else {
            var menuli = this.parentElement;
        }
        var ul = $("ul", menuli)[0];

        if (!ul || ul.classList.contains("-visible")) return;

        ul.classList.add("-visible");
        menu.target.previousElementSibling.classList.add("-visible")
    }
    function hideMenuMobile(menu) {
        menu.target.classList.remove("-visible");
        var menu = this.parentElement;
        var ul = $("ul", menu)[0];

        menu.classList.remove("-active");
        ul.classList.add("-animating");
        setTimeout(function () {
            ul.classList.remove("-visible");
            ul.classList.remove("-animating");
        }, 300);
    }

    function checkPosition(li) {
        var positions = li.getBoundingClientRect();
        var maxH = window.innerHeight - positions.top - 1 + "px";
        var top = positions.top + "px";
        var left = positions.right + "px";
        var right = positions.left + "px";
        li.querySelectorAll("li > ul")[0].style.top = top;
        if (document.getElementsByTagName("html")[0].getAttribute("dir") == "rtl") {
            li.querySelectorAll("li > ul")[0].style.left = right;
        } else {
            li.querySelectorAll("li > ul")[0].style.left = left;
        }
        li.querySelectorAll("li > ul")[0].style.maxHeight = maxH;
    }

    window.addEventListener("load", function () {
        if (991 < window.innerWidth) {
            forEach($(".Menu > li.-hasSubmenu"), function (e) {
                checkPosition(e);
            });
            forEach($(".Menu > li.-hasSubmenu"), function (e) {
                e.addEventListener("mouseenter", showMenu);
            });
            forEach($(".Menu > li"), function (e) {
                e.addEventListener("mouseleave", hideMenu);
            });
        } else {
            forEach($(".Menu > li.-hasSubmenu > .go-forward"), function (e) {
                e.addEventListener("click", showMenuMobile);
            });
            forEach($(".Menu > li.-hasSubmenu > .go-back"), function (e) {
                e.addEventListener("click", hideMenuMobile);
            });
        }

        //forEach($(".Menu li .back"), function(e){
        //    e.addEventListener("click", hideMenu);
        //});

    });
})();
