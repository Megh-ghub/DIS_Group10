var elems = document.querySelectorAll('.carousel-slider');
var instances = M.Carousel.init(elems,{
    duration: 200,
    indicators: true,
    fullWidth: true
});
setInterval(function(){instances[0].next();}, 4000);

document.addEventListener('DOMContentLoaded', function() {
    var el = document.querySelectorAll('.sidenav');
    var ins = M.Sidenav.init(el,{});
    var el_c = document.querySelectorAll('.collapsible');
    var in_c = M.Collapsible.init(el_c,{});
});

$(document).ready(function(){
    $('.collapsible').collapsible();
});