$(document).ready(function() {
  $('.link a').click(function(event){
    event.stopPropagation();
  });

  $('.link').click(function() {
    var path = $( this ).data("path");
    if(path != undefined && path != "")
        window.location = path;    
  });

  $('.needconfirm').click(function(e) {
    e.preventDefault();
    that = $( this );
    target = $( this ).attr('href');
    title = $( this ).attr('title');
    text = "";
    if(title != undefined)
      text = "Are you sure want to " + title + "?";
    else
      text = "Are you sure?";
    if(target != undefined)
    {
      confirm(text, function() {
        window.location = target;
        eval(that.attr('data-action'));
      });    
    }
  });

  $('.tab-switch').click(function() {
    tabgroup = $( this ).data("tabgroup");
    target = $( this ).data("target");
    $('.'+tabgroup+'tab').removeClass('active').hide();

    tab = $('.'+tabgroup+'tab[data-tab="'+target+'"]');
    tab.addClass('active').show();
    tab.find('input')[0].focus();
    tab.find('.formatted-text')[0].focus();
  });

  $('.toggle').click(function() {  
    target = $('#'+$( this ).data("target"))      
    target.slideToggle();
    target.find('.formatted-text')[0].focus();
  });

  $('.searchinput').change(function() {            
    var results = $('#'+$( this ).data("target"));
    results.html("");    
    $.ajax({
      url: "/api/search",
      dataType: "JSON",
      data: { q: $( this ).val(), mode: "templated" },      
      success: function(data) {        
        for (var i = 0; i < data.length; i++) {          
          results.append(data[i].Rendered);
        };
      }
    });
  });

  $('.submitform').click(function() { 
    formObject = $('#'+$( this ).data("target"));    
    activeTab = formObject.find(".active").data("tab");
    editor = formObject.find(".formatted-text");
    formObject.find("input[name='text']").val(editor.html());
    formData  = new FormData(formObject[0]);
    if(['text','url','file','search'].indexOf(activeTab) < 0)
      return;    
    action = '/api/' + activeTab;

    $.ajax({
      url: action,
      type: 'POST',
      data: formData,
      async: false,
      success: function (data) {
        notify("Added", "info");          
      },
      cache: false,
      contentType: false,      
      processData: false
    });
  });
});

var initEditor = function(selector) 
{
  return new MediumEditor(selector, {
    toolbar: {
      buttons: ['bold', 'italic', 'underline', 'anchor', 'h1', 'h2', 'h3', 'quote', 'orderedlist', 'unorderedlist'],
    },
    placeholder: { text: 'Type your text here...' },
    autoLink: true,
    paste: {
      cleanPastedHTML: true
    },
    anchorPreview: {        
        hideDelay: 200,
        previewValueSelector: 'a'
    }
  });
};

function showNotification(message, type, buttonName, buttonCallback, timeout) { 
  message = typeof message !== 'undefined' ? message : 'Hello!';
  type = typeof type !== 'undefined' ? type : 'success';
  timeout = typeof timeout !== 'undefined' ? timeout : 3000;      
    
  if ($('#notification').length < 1) {
    markup = '<div id="notification" style="display:none;" class="information"><span class="text">Hello!</span><span class="set text-right"><a class="close system button small" href="javascript:;">X</a></div>';
    $('body').append(markup);
  }
  
  $notification = $('#notification');
  if(buttonName != undefined) {
    $('#notification .set').html(
      "<a href='javascript:;' class='button small yes'>" + buttonName + "</a>" +
      "<a href='javascript:;' class='system button small no'>Cancel</a>"
    )    

    $('#notification .button.yes').click(function (e) {
      buttonCallback();
      e.preventDefault();
      $notification.slideUp();
    }); 

    $('#notification .button.no').click(function (e) {
      e.preventDefault();
      $notification.slideUp();
    }); 
  }
  
  // set the message
  $('#notification .text').text(message);
  
  // setup click event
  $('#notification a.close').click(function (e) {
    e.preventDefault();
    $notification.slideUp();
  });  
  
  $notification.removeClass().addClass(type);
  $notification.slideDown();  
  setTimeout(function() {
    $notification.slideUp();
  }, timeout);  
}

function notify(message, type, timeout) {
  showNotification(message, type, undefined, undefined, timeout);
}

function confirm(message, okCallback) {
  showNotification(message, "warn", "Yes", okCallback, 1000000);
}

function send(method, path, args, callback) {
  if(callback == undefined) {
    callback = function (data) {
      notify(data, "success");          
    };
  }

  $.ajax({
    url: path,
    type: method,
    data: args,
    async: true,
    success: callback,
    error: function(err) {
      notify("Error: " + err, "error", 5000)
    },
    cache: false,
    contentType: false,      
    processData: false
  });
}