$(document).on('ready',function(){
    "use strict";

    
    /* =============== Ajax Contact Form ===================== */
    $('#contactform').submit(function(){
        var action = $(this).attr('action');
        $("#message").slideUp(750,function() {
        $('#message').hide();
            $('#submit')
            .after('<img src="images/ajax-loader.gif" class="loader" />')
            .attr('disabled','disabled');
        $.post(action, {
            name: $('#name').val(),
            email: $('#email').val(),
            comments: $('#comments').val(),
            verify: $('#verify').val()
        },
            function(data){
                document.getElementById('message').innerHTML = data;
                $('#message').slideDown('slow');
                $('#contactform img.loader').fadeOut('slow',function(){$(this).remove()});
                $('#submit').removeAttr('disabled');
                if(data.match('success') != null) $('#contactform').slideUp('slow');

            }
        );
        });
        return false;
    });  

    /*** FIXED Menu APPEARS ON SCROLL DOWN ***/ 
    $(window).scroll(function() {    
        var scroll = $(window).scrollTop();
        if (scroll >= 50) {
        $(".forsticky").addClass("sticky");
        }
        else{
        $(".forsticky").removeClass("sticky");
        $(".forsticky").addClass("");
        }
    });  

    
  
    /* Cart Delete */
    $('.cartinfo > i').on('click', function(){
        $(this).parent().parent().fadeOut('fast');
    });


    /* Search Open */
    $('.qopen').on('click', function(){
        $('.quickviewpopup').fadeIn('fast');
        $('html').addClass('noscroll');
        return false;
    });
    $('.closeview').on('click', function(){
        $('.quickviewpopup').fadeOut('fast');
        $('html').removeClass('noscroll');
    });

    /* Search Open */
    $('.searchopen').on('click', function(){
        $('.searchdialouge').fadeIn('fast');
        $('header').addClass('showup');
    });
    $('.searchpopup > span').on('click', function(){
        $(this).parent().parent().fadeOut('fast');
        $('header').removeClass('showup');
    });

    /* Cart Open */
    $('.cartopen').on('click', function(){
        $('.cartslide').addClass('slidein');
        $('body').addClass('active');
    });
    $('.closecartslide').on('click', function(){
        $('.cartslide').removeClass('slidein');
        $('body').removeClass('active');
    });
    $('.delcart').on('click', function(){
        $(this).parent().parent().fadeOut();
    });

    /* Filter Open */
    $('.filterbtn').on('click', function(){
        $(this).toggleClass('active');
        $('.filter-open').slideToggle('fast');
    });

    /* Sideheader */
    $('header.sideheader nav > ul > li.menu-item-has-children > a').on('click', function(){
        $('header.sideheader nav > ul > li.menu-item-has-children > ul').slideUp();
        $(this).next('ul').slideDown('fast');
         return false;
    });

    /* Resposnive Header Nav */
    $('.respheader nav > ul > li.menu-item-has-children > a').on('click', function(){
        $('.respheader nav > ul > li.menu-item-has-children > ul').slideUp();
        $(this).next('ul').slideDown('fast');
        $(this).toggleClass('active');
         return false;
    });

    /* Skip Loading */
    $('.page-loading > span').on('click', function(){
        $(this).parent().fadeOut();
    });

    $('.center #hamburger-two').on('click', function(){
        $(this).toggleClass('active');
        $('.menuclick').toggleClass('active');
        $('nav').fadeToggle();
    });

    /* Responsive menu */
    $('.open-minimal-menu.resopen').on('click', function(){
        $('.respheader').toggleClass('slidein');
        $('.open-minimal-menu.resopen #hamburger-two').toggleClass('active');
        $(this).toggleClass('active');
    });

      /* Sideheader */
    $('.open-minimal-menu.sideopen').on('click', function(){
        $('.sideheader').toggleClass('slidein');
        $('.open-minimal-menu.sideopen #hamburger-two').toggleClass('active');
        $(this).toggleClass('active');
    });


    /* FAQ */
    $('.faqs-sec > h2').on('click', function(){
        $(this).next().slideToggle();
        $(this).toggleClass('active');
    });


    /*** TABS SEC ***/
    $('.tab-sec .tab-content:first').fadeIn('fast');
    $('.tab-sec li a').on("click", function(){
        var tab_id = $(this).attr('data-tab');
        $('.tab-sec li a').removeClass('current');
        $('.tab-sec .tab-content').fadeOut();
        $(this).addClass('current');
        $("#"+tab_id).fadeIn('fast');
    });   
    

    /*** TABS SEC ***/
    $('.tab-sec2 .tab-content2:first').fadeIn('fast');
    $('.tab-sec2 li a').on("click", function(){
        var tab_id = $(this).attr('data-tab');
        $('.tab-sec2 li a').removeClass('current');
        $('.tab-sec2 .tab-content2').fadeOut();
        $(this).addClass('current');
        $("#"+tab_id).fadeIn('fast');
    });  

});



$(window).on('load',function(){
    "use strict";

    $('.page-loading').fadeOut();

    var full_height = $(window).height();
    $(".full-bg").css({"height":full_height});

});