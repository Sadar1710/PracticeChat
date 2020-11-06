

$("#btnGet").click(function () {
    var a = $("#phoneNumber").val();
    if (a.length != 0) {
        var url = '@Url.Action("SearchList", "Home")';
        //var data = { number: a};
        $.getJSON(url, { number: a }, function (data) {
            // var photo="~/img/" + (data.photoPath ?? "avatar-1.png");
            //console.log(data.userId);
            var items = "";
            items += '<div class="row mt-5"><div class="col-12 col-sm-12 col-md-10 offset-md-1 col-lg-10 col-xl-10 offset-xl-1" id="cols"style="box-shadow: 0 4px 6px 0 rgba(0,0,0,0.2),0 5px 6px 0 rgba(0,0,0,0.4);">';
            items += '<h1 class="text-center">Search List</h1>';
            items += '<table class="table">';
            items += '<thead>';
            items += '<tr>';
            items += '<th>UserName</th>';
            items += '<th>Action</th>';
            items += '</tr>';
            items += '</thead>';
            items += '<tbody>';
            items += '<tr>';
            items += '<td><img class="rounded-circle" src="/img/' + data.photoPath + '"sp-append-version="true" height="30px" width="30px">' + data.userName + '</td>';
            items += '<td>';
            items += '<a href="/Home/AddFriend?Id=' + data.userId + ' class="btn btn-outline-primary">Add to friendlist</a>';
            items += '</td>';
            items += '</tr>';
            items += '</tbody>';
            items += '</table>';
            items += '</div>';
            items += '</div>';
            $("#mainContent").html(items);
        });
    }
    else {

    }
});