$(document).ready(function() {
  $('.link a').click(function(event){
    event.stopPropagation();
  });

  $('.link').click(function() {
    var path = $( this ).data("path");
    if(path != undefined && path != "")
        window.location = path;    
  });

  $('.tab-switch').click(function() {
    tabgroup = $( this ).data("tabgroup");
    target = $( this ).data("target");
    $('.'+tabgroup+'tab').removeClass('active').hide();
    $('.'+tabgroup+'tab[data-tab="'+target+'"]').addClass('active').show();    
  });

  $('.toggle').click(function() {        
    $('#'+$( this ).data("target")).slideToggle();    
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
    formData  = new FormData(formObject[0]);
    activeTab = formObject.find(".active").data("tab");
    
    if(['text','url','file','search'].indexOf(activeTab) < 0)
      return;
    
    action = '/' + activeTab + '/add';   

    $.ajax({
      url: action,
      type: 'POST',
      data: formData,
      async: false,
      success: function (data) {
          alert(data);
      },
      cache: false,
      contentType: false,      
      processData: false
    });
  });
});

var setRelation = function(relation) {
    $( "input[name='relation']" ).val(relation);
    $( "input[name='relation']" ).trigger('autoresize');
};

var initEditor = function(selector) 
{
  return new MediumEditor(selector, {
    toolbar: {
      buttons: ['bold', 'italic', 'underline', 'anchor', 'h1', 'h2', 'h3', 'quote', 'orderedlist', 'unorderedlist'],
    },
    placeholder: { text: 'Type your text here...' },
    autoLink: true,
    anchorPreview: {        
        hideDelay: 200,
        previewValueSelector: 'a'
    }
  });
};