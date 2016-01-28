$(document).ready(function(){
  // the "href" attribute of .modal-trigger must specify the modal ID that wants to be triggered
  $('.modal-trigger').leanModal();
  $('.tooltipped').tooltip({delay: 50});

  $('.link').click(function() {
    var path = $( this ).data("path");
    if(path != undefined && path != "")
        window.location = path;        
    else
        console.log(path);
  });

  initSelector($('#selector'));
});


var setRelation = function(relation) {
    $( "input[name='relation']" ).val(relation);
    $( "input[name='relation']" ).trigger('autoresize');
};

var initSelector = function(elem) {
  var id = elem.data('id');
  var provider = elem.data('provider');
  //alert(provider + "/" + id);
  loadLinks(elem, id, provider);  
};

var loadLinks = function(parentElem, parentId, parentProvider) {
  $.ajax({
    url: "/api/" + parentProvider + "/" + parentId + "/links",    
    dataType: "json",
    success: function(data) {
      console.log(data);
    },          
  });
}

